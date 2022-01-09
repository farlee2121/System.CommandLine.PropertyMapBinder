# System.CommandLine.PropertyMapBinding

## Motivation / what is this
This library is an experiment. The goal is to create an intuitive handler binding experience for [System.CommandLine](https://github.com/dotnet/command-line-api).
A few goals
- intuitive binding of complex types
- support handler declaraction as a self-contained expression (no reference to symbol instances)
- blending multiple binding rules for a customizable and consistent binding experience
- easy extension of the binding pipeline



## Examples

All examples assume this following definitions are available

```cs
Option<int> frequencyOpt = new Option<int>(new string[] { "--frequency", "-f" }, "such description");

RootCommand rootCommand = new RootCommand("Test Test")
{
    new Argument<string>("print-me", "gets printed"),
    frequencyOpt, 
    new Option<IEnumerable<int>>(new string[] { "--list", "-l" }, "make sure lists work")
    {
        Arity = ArgumentArity.ZeroOrMore
    };
};

public static async Task SuchHandler(SuchInput input)
{
    Console.WriteLine($"printme: {input.PrintMe}; \nfrequency: {input.Frequency}; \nlist:{string.Join(",",input.SuchList)}");
}

public class SuchInput {
    public int Frequency { get; set; }
    public string? PrintMe { get; set; }
    public IEnumerable<int> SuchList { get; set; } = Enumerable.Empty<int>();
}
```

### Pipeline
The backbone construct is `BinderPipeline`. 

```cs
rootCommand.Handler = CommandHandler.FromPropertyMap(SuchHandler,
    new BinderPipeline<SuchInput>{
        PropertyMap.FromName<SuchInput, string>("print-me", model => model.PrintMe ),
        PropertyMap.FromReference<SuchInput, int>(frequencyOpt, model => model.Frequency),
        PropertyMap.FromName<SuchInput, IEnumerable<int>>("-l", model => model.SuchList)
    });
```

`BinderPipeline` is really a collection of `IPropertyBinder`. Each `IPropertyBinder` defines a strategy for assigning input to the target object.
The pipeline executes each binder in the order they are given. This means later binders will override earlier ones. This means we can
- use multiple rules to bind properties
- define a priority/fallback chain for any given property

### Builder

We can also build the pipeline through a set of extension methods. The primary benefit is improved type inference (thus less explicit typing).
Binders will still be called in the order registered.

```cs
rootCommand.Handler = CommandHandler.FromPropertyMap(SuchHandler,
    new BinderPipeline<SuchInput>()
    .MapFromName("print-me", model => model.PrintMe)
    .MapFromReference(frequencyOpt, model => model.Frequency)
    .MapFromName("-l", model => model.SuchList)
);
```

### Blended Conventions

The pipeline can handle many approaches binding input. Here's an example of a simple naming convention with an explicit mapping fallback

```cs
rootCommand.Handler = CommandHandler.FromPropertyMap(SuchHandler,
    new BinderPipeline<SuchInput>()
    .MapFromNameConvention(TextCase.Pascal)
    .MapFromName("-l", model => model.SuchList)
);
```
### Possible extensions to the pipeline
Here are some cases I haven't implemented, but would be fairly easy to add
- map default values from configuration 
- Ask a user for any missing inputs 
  - can be done with the existing setter overload, but prompts could be automated with a signature like `.PromptIfMissing(name, selector)`
- match properties based on type
- Set a value directly 
  - can be done with the existing setter overload, but could be simpler `.MapFromValue(c => c.Frequency, 5)`


## How to extend

Extending the pipeline is fairly easy.

The core contract is 
```cs
public interface IPropertyBinder<InputModel>
{
    InputModel Bind(InputModel InputModel, InvocationContext context);
}
```
`IPropertyBinder` takes an instance of the target input class and the invocation context provided by the parser.

Input definitions (i.e. options and arguments) can be found in `context.ParserResult.CommandResult.Symbol.Children`
and values can be fetched by functions like `context.ParseResult.GetValueForOption`.

Examples exist for [symbol name and property path](./Core/PropertyMap.cs) and [simple name conventions](./CliExample/NamingConventionPipelineBinder.cs).

The other key step is to register extension methods on `BinderPipeline`. The main behaviors to consider
- the extension should add it's binder to the end of the pipeline (e.g. `pipeline.Add(yourBinder)`)
- The extension should return the modified copy of the pipeline (i.e. always has return type `BinderPipeline<T>`)

## Status of project

A successful experiment. Usable, but not production-tested. No guarantees of support


<!-- ## How to Contribute -->