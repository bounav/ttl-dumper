using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.IO;
using System.Threading.Tasks;
using LightInject;
using Microsoft.Extensions.Configuration;
using TtlDumper.Commands;
using TtlDumper.Services;

namespace TtlDumper
{
    public class Program
    {
        /// <summary>
        /// The IoC container.
        /// </summary>
        public static IServiceContainer Container;
        
        /// <summary>
        /// Program entry point
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            Container = new ServiceContainer(new ContainerOptions { EnablePropertyInjection = false });

            Container.RegisterInstance<IConfiguration>(configuration);
            
            Container.Register<IoService>();

            var readFromChipCommand = new Command("read-from-chip")
                                      {
                                          new Argument<FileInfo>("output", "The name of the file to dump the memory content to")
                                      };
            
            readFromChipCommand.Handler = CommandHandler.Create<FileInfo>((output) =>
            {
                Container.Register<ReadFromChipCommand>();

                var command = Container.GetInstance<ReadFromChipCommand>();

                command.EntryPoint(output);
            });

            var writeToChipCommand = new Command("write-to-chip")
                                     {
                                         new Argument<FileInfo>("input", "The name of the file to read bytes from")
                                     };

            writeToChipCommand.Handler = CommandHandler.Create<FileInfo>((input) =>
            {
                Container.Register<WriteToChipCommand>();

                var command = Container.GetInstance<WriteToChipCommand>();

                command.EntryPoint(input);
            });

            var rootCommand = new RootCommand
                              {
                                  new Command("blink-address-bus")
                                  {
                                      Handler = BlinkCommand.Execute
                                  },
                                  readFromChipCommand,
                                  writeToChipCommand
                              };
            
            var builder = new CommandLineBuilder(rootCommand).UseDefaults();
            
            var parser = builder.Build();
            
            await parser.InvokeAsync(args);
        }
    }
}
