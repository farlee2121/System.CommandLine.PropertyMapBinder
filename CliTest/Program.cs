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

            rootCommand.Handler = CommandHandler.FromPropertyMap(SuchHandler, new[]
            {
                //TODO: see if I can use a delegate to improve the list expected type and get better type inference on these individual methods
                PropertyMap.FromName<SuchInput, string>("print-me", (contract, val) => {contract.PrintMe = val; return contract; }),
                PropertyMap.FromName<SuchInput, int>("-f", (contract, val) => {contract.Frequency = val; return contract; })
            });

            //Console.WriteLine("Hello, World!");
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
