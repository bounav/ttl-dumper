using System.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using ABElectronicsUK;
using Microsoft.Extensions.Configuration;
using TtlDumper.Configuration;

namespace TtlDumper.Services
{
    /// <summary>
    /// Service to help configure IO and expose a simple interface to read/write data.
    /// </summary>
    public class IoService : IDisposable
    {
        // Addresses bus A and B on "IO pi plus" board https://www.abelectronics.co.uk/p/54/io-pi-plus
        private readonly List<IOPi> buses;
        
        // Address bus
        public BusInfo AddressBus;

        // Control buses
        public BusInfo ChipEnableBus;
        public BusInfo OutputEnableBus;
        public BusInfo WriteEnableBus;

        // Data bus
        public BusInfo DataBus;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="configuration"></param>
        public IoService(IConfiguration configuration)
        {
            // Pin config
            var config = configuration.GetSection("buses")
                                      .Get<BusDefinition[]>();

            buses = new List<IOPi>();

            foreach (var definition in config)
            {
                var bus = FindOrCreateBus(definition);

                var busInfo = new BusInfo(bus, definition);
                
                switch (definition.Type)
                {
                    case BusType.Address:
                        AddressBus = busInfo;
                        break;
                    case BusType.ChipEnable:
                        ChipEnableBus = busInfo;
                        break;
                    case BusType.OutputEnable:
                        OutputEnableBus = busInfo;
                        break;
                    case BusType.WriteEnable:
                        WriteEnableBus = busInfo;
                        break;
                    case BusType.Data:
                        DataBus = busInfo;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        
        public void InitialisePins(BusInfo info)
        {
            bool isInput;

            switch (info.Type)
            {
                case BusType.Address:
                case BusType.ChipEnable:
                case BusType.OutputEnable:
                case BusType.WriteEnable:
                    isInput = false;
                    break;
                case BusType.Data:
                    // Depending on the function (read or write), the direction of the data bits will have to be set again
                    isInput = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            foreach (var pinNumber in info.Io)
            {
                var pin = pinNumber;
                
                info.Bus.SetPinDirection(pin, isInput);
                info.Bus.SetPinPullup(pin, info.EnablePullUpResistors);
                info.Bus.InvertPin(pin, info.InvertPins);
            }
        }

        /// <summary>
        /// Initialises a new bus if needed or "recycles" one from the <see cref="buses" /> list when matched on the bus address.
        /// </summary>
        public IOPi FindOrCreateBus(BusDefinition definition)
        {
            var bus = buses.FirstOrDefault(x => x.Address == definition.Address);
            
            if (bus == null)
            {
                bus = new IOPi(definition.Address);
                buses.Add(bus);
            }

            return bus;
        }

        /// <summary>
        /// Set the control levels to obtain de desired function (e.g. Read or Write)
        /// </summary>
        private void SetMode(TargetMode mode)
        {
            var controlLevels = mode.ToControlLevels();

            ChipEnableBus.Bus.WritePin(ChipEnableBus.Io[0], controlLevels.ChipEnableLevel);
            OutputEnableBus.Bus.WritePin(OutputEnableBus.Io[0], controlLevels.OutputEnableLevel);
            WriteEnableBus.Bus.WritePin(WriteEnableBus.Io[0], controlLevels.WriteEnableLevel);
        }

        /// <summary>
        /// Outputs value
        /// </summary>
        /// <param name="bus">The bus to output to.</param>
        /// <param name="levels">true is high, false is low</param>
        /// <param name="pinDirection">Optional, when unset the direction is not set, when true is input, otherwise when false is output</param>
        private static void OutputOnBus(BusInfo bus, BitArray levels, bool? pinDirection = false)
        {
            // Write levels pin by pins when IO mapping is used
            if (bus.Io?.Length > 0)
            {
                for (byte i = 0; i < bus.Io.Length; i++)
                {
                    var pinNumber = bus.Io[i];

                    var isHigh = levels[i];

                    if (pinDirection.HasValue)
                    {
                        bus.Bus.SetPinDirection(pinNumber, pinDirection.Value);
                    }

                    bus.Bus.WritePin(pinNumber, isHigh);
                }
            }
            // Otherwise write byte using port
            else
            {
                var value = levels.ToByte();

                bus.Bus.WritePort(bus.Port, value);
            }
        }

        /// <summary>
        /// Writes values to the address bus and sets the <see cref="chipEnable"/>, <see cref="outputEnable"/> and <see cref="writeEnable"/> levels.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="data">The bit array representing the levels.</param>
        public void WriteByte(BitArray address, BitArray data)
        {
            if (AddressBus.Io.Length != address.Length)
            {
                throw new ArgumentException("The address bus width mismatch", nameof(address));
            }

            if (DataBus.Io.Length != data.Length)
            {
                throw new ArgumentException("The data bus width mismatch", nameof(data));
            }

            SetMode(TargetMode.OutputDisable);

            // Output levels on address bus
            OutputOnBus(AddressBus, address);

            SetMode(TargetMode.WriteCycle);

            // Output levels on data bus
            OutputOnBus(DataBus, data, false);

            // Wait a bit to give time for the chip to store the value
            Thread.Sleep(1);

            SetMode(TargetMode.OutputDisable);
        }

        /// <summary>
        /// The <see cref="address"/> array length MUST match the address length defined in the configuration.
        /// </summary>
        /// <param name="address">An array of high/low values.</param>
        /// <returns>The value read on the the data bus at the given address.</returns>
        public byte ReadByte(BitArray address)
        {
            if (DataBus.Io.Length != 8)
            {
                throw new Exception("The data bus must be 8 bits long");
            }
            
            SetMode(TargetMode.ReadCycle);

            // Output levels on address bus and configures chip "mode"
            OutputOnBus(AddressBus, address);

            byte read;

            // When IO mapping of pins is set, read the levels pin by pin
            if (DataBus.Io?.Length > 0)
            {
                var readBits = new BitArray(DataBus.Io.Length);

                // Read levels on data bus
                for (byte i = 0; i < DataBus.Io.Length; i++)
                {
                    var pinNumber = DataBus.Io[i];

                    DataBus.Bus.SetPinDirection(pinNumber, true);

                    var isHigh = DataBus.Bus.ReadPin(pinNumber);

                    readBits[i] = isHigh;
                }
                
                read = readBits.ToByte();
            }
            // Otherwise read the byte in one operation from the port.
            else
            {
                read = DataBus.Bus.ReadPort(DataBus.Port);
            }

            //SetMode(TargetMode.OutputDisable);

            return read;
        }

        public void Dispose()
        {
            foreach (var bus in buses)
            {
                bus.Dispose();
            }
        }
    }
}
