// See https://aka.ms/new-console-template for more information
using System;
using System.CommandLine;
using System.CommandLine.PropertyMapBinder;
namespace System.CommandLine.PropertyMapBinder.CliTest
{

    public class Program
    {

        public static int Main(string[] argv)
        {
            Argument<string> printMeArg = new Argument<string>("print-me", "gets printed");
            Option<int> frequencyArg = new Option<int>(new string[] { "--frequency", "-f" }, "such description");

            RootCommand rootCommand = new RootCommand("Test Test")
            {
                printMeArg,
                frequencyArg
            };

            rootCommand.Handler = CommandHandler.FromPropertyMap(SuchHandler,
                    new BinderPipeline<SuchInput>()
                    .AddByName("print-me", contract => contract.PrintMe)
                    .AddByReference(frequencyArg, contract=> contract.Frequency)
                );
            // rootCommand.Handler = CommandHandler.FromPropertyMap(SuchHandler,
            //     new BinderPipeline<SuchInput>{
            //         PropertyMap.FromName<SuchInput, string>("print-me", contract => contract.PrintMe ),
            //         PropertyMap.FromName<SuchInput, int>("-f", contract => contract.Frequency)
            //     });

            return rootCommand.Invoke(argv);
        }

        public static async Task SuchHandler(SuchInput input)
        {
            Console.WriteLine($"printme: {input.PrintMe}; \nfrequency: {input.Frequency}");
        }

        public class SuchInput {
            public int Frequency { get; set; }
            public string? PrintMe { get; set; }

        }
    }
}
