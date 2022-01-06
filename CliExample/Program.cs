using System;
using System.CommandLine;
using System.CommandLine.PropertyMapBinder;
namespace System.CommandLine.PropertyMapBinder.CliExample
{

    public class Program
    {

        public static int Main(string[] argv)
        {
            Option<int> frequencyArg = new Option<int>(new string[] { "--frequency", "-f" }, "such description");
            Option<IEnumerable<int>> listOpt = new Option<IEnumerable<int>>(new string[] { "--list", "-l" }, "make sure lists work")
            {
                Arity = ArgumentArity.ZeroOrMore
            };

            RootCommand rootCommand = new RootCommand("Test Test")
            {
                new Argument<string>("print-me", "gets printed"),
                frequencyArg, 
                listOpt
            };

            rootCommand.Handler = CommandHandler.FromPropertyMap(SuchHandler,
                    new BinderPipeline<SuchInput>()
                    .AddByName("print-me", contract => contract.PrintMe)
                    .AddByName("-l", contract => contract.List)
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
            Console.WriteLine($"printme: {input.PrintMe}; \nfrequency: {input.Frequency}; \nlist:{string.Join(",",input.List)}");
        }

        public class SuchInput {
            public int Frequency { get; set; }
            public string? PrintMe { get; set; }

            public IEnumerable<int> List { get; set; } = Enumerable.Empty<int>();

        }
    }
}
