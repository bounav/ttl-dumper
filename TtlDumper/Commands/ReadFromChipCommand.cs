using System;
using System.Collections;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using TtlDumper.Services;

namespace TtlDumper.Commands
{
    /// <summary>
    /// Command to dump the content of a memory chip to a file.
    /// </summary>
    public class ReadFromChipCommand
    {
        private readonly IoService service;

        public ReadFromChipCommand(IConfiguration configuration)
        {
            this.service = new IoService(configuration);
        }
        
        /// <summary>
        /// Entry point for the command.
        /// </summary>
        /// <param name="output">Information about the file to write data to.</param>
        public void EntryPoint(FileInfo output)
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

                    DoWork(output);
                };

                service.DataBus.Bus.Connect();
            };
            service.AddressBus.Bus.Connect();

            service.Dispose();
        }

        /// <summary>
        /// Actual work (runs once buses have been initialised)
        /// </summary>
        private void DoWork(FileInfo output)
        {
            var maxAddress = new BitArray(service.AddressBus.Io.Length, true).ToUInt16();

            var numberOfBytes = 0;

            using (var writer = new BinaryWriter(output.OpenWrite(), Encoding.Default, false))
            {
                for (UInt16 i = 0; i < maxAddress; i++)
                //for (UInt16 i = 0; i < 0b11111111111; i++)
                {
                    var address = new BitArray(new int[] { i });

                    var value = service.ReadByte(address);

                    writer.Write(value);

                    numberOfBytes++;
                    
                    if (i % 1000 == 0)
                    {
                        //Console.WriteLine(".");
                        LogHelper.Log(address, value);
                    }
                }
                
                Console.WriteLine($"{numberOfBytes} byte(s) dumped in {output.FullName}");
            }
        }
    }
}