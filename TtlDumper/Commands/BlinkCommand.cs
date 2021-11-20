using System;
using System.Collections.Generic;
using System.CommandLine.Invocation;
using LightInject;
using Microsoft.Extensions.Configuration;
using TtlDumper.Services;

namespace TtlDumper.Commands
{
    public class BlinkCommand
    {
        // Create an instance of the IOPi object with an I2C address of 0x20
        private readonly IoService service;

        public BlinkCommand(IConfiguration configuration)
        {
            this.service = new IoService(configuration);
        }

        public static ICommandHandler Execute =>
            CommandHandler.Create(() =>
            {
                Program.Container.Register<BlinkCommand>();

                var demo = Program.Container.GetInstance<BlinkCommand>();

                demo.Connect();
            });

        public void Connect()
        {
            service.AddressBus.Bus.Connected += (sender, args) =>
            {
                EntryPoint();
            };
            service.AddressBus.Bus.Connect();

            service.Dispose();
        }

        /// <summary>
        /// Actual work (runs once buses have been initialised)
        /// </summary>
        public void EntryPoint()
        {
            var queue = new Queue<byte>();

            foreach (var pin in service.AddressBus.Io)
            {
                service.AddressBus.Bus.WritePin(pin, true);

                queue.Enqueue(pin);
            }

            do
            {
                var pinToTurnOff = queue.Dequeue();

                service.AddressBus.Bus.WritePin(pinToTurnOff, false);
            } while (queue.Count > 0);
            
            Console.WriteLine("Done.");
        }
    }
}