using System;
using System.CommandLine;
using System.CommandLine.PropertyMapBinder;
using System.CommandLine.PropertyMapBinder.NameConventionBinder;

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

            var subCommand = new Command("sub");
            subCommand.Handler = new BinderPipeline<SuchInput>()
                                    .MapFromName("-global", model => model.Global)
                                    .ToHandler(SuchHandler);



            var globalOption = new Option<bool>("-global");
            rootCommand.AddGlobalOption(globalOption);

            rootCommand.Handler = new BinderPipeline<SuchInput>()
                                    .MapFromNameConvention(TextCase.Pascal)
                                    // .MapFromName("print-me", contract => contract.PrintMe)
                                    // .MapFromReference(frequencyArg, contract=> contract.Frequency)
                                    .MapFromName("-l", contract => contract.SuchList)
                                    .MapFromValue(model => model.Frequency, 9000)
                                    .MapFromName("-global", model => model.Global)
                                    .ToHandler(SuchHandler);
                
            //rootCommand.Handler = CommandHandler.FromPropertyMap(SuchHandler,
            //    new BinderPipeline<SuchInput>{
            //         PropertyMap.FromName<SuchInput, string>("print-me", contract => contract.PrintMe ),
            //         PropertyMap.FromName<SuchInput, int>("-f", contract => contract.Frequency)
            //    });

            return rootCommand.Invoke(argv);
        }

        public static async Task SuchHandler(SuchInput input)
        {
            Console.WriteLine($"printme: {input.PrintMe}; \nfrequency: {input.Frequency}; \nlist:{string.Join(",",input.SuchList)} \nglobal: {input.Global}");
        }

        public class SuchInput {
            public int Frequency { get; set; }
            public string? PrintMe { get; set; }

            public bool Global { get; set; }
            public IEnumerable<int> SuchList { get; set; } = Enumerable.Empty<int>();

        }
    }
}
