using System;
using System.Collections;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using TtlDumper.Services;

namespace TtlDumper.Commands
{
    /// <summary>
    /// Command to read an input file and write its content into a chip.
    /// </summary>
    public class WriteToChipCommand
    {
        private readonly IoService service;

        public WriteToChipCommand(IConfiguration configuration)
        {
            this.service = new IoService(configuration);
        }

        /// <summary>
        /// Entry point for the command.
        /// </summary>
        /// <param name="input">Information about the file to read data from.</param>
        public void EntryPoint(FileInfo input)
        {
            service.AddressBus.Bus.Connected += (sender, args) =>
            {
                // Bus 1
                service.InitialisePins(service.AddressBus);

                service.DataBus.Bus.Connected += (sender, args) =>
                {
                    // Bus 2
                    service.InitialisePins(service.DataBus);
                    service.InitialisePins(service.ChipEnableBus);
                    service.InitialisePins(service.OutputEnableBus);
                    service.InitialisePins(service.WriteEnableBus);

                    DoWork(input);
                };

                service.DataBus.Bus.Connect();
            };
            service.AddressBus.Bus.Connect();

            service.Dispose();
        }

        /// <summary>
        /// Actual work (runs once buses have been initialised)
        /// </summary>
        private void DoWork(FileInfo input)
        {
            throw new NotImplementedException("Untested");

            UInt16 offset = 0;

            using (var reader = new BinaryReader(input.OpenRead(), Encoding.Default, false))
            {
                try
                {
                    var address = new BitArray(new int[] { offset });

                    var value = reader.ReadByte();

                    var data = new BitArray(new int[] { value });

                    service.WriteByte(address, data);

                    if (offset % 1000 == 0)
                    {
                        //Console.WriteLine(".");
                        LogHelper.Log(address, value);
                    }

                    offset++;
                }
                catch (EndOfStreamException ex)
                {
                    Console.WriteLine($"{offset} byte(s) write to chip from {input.FullName}");
                }
            }
        }
    }
}