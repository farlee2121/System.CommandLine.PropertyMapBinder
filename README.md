
# System.CommandLine.PropertyMapBinding
[![](https://badgen.net/nuget/v/farlee2121.System.CommandLine.PropertyMapBinder)](https://www.nuget.org/packages/farlee2121.System.CommandLine.PropertyMapBinder)

## Motivation / what is this
The goal is to create an intuitive handler binding experience for [System.CommandLine](https://github.com/dotnet/command-line-api).
A few goals
- intuitive binding of complex types
- support handler declaraction as a self-contained expression (no reference to symbol instances)
- blending multiple binding rules for a customizable and consistent binding experience
- easy extension of the binding pipeline


## Examples

All examples assume the following definitions are available

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
rootCommand.Handler = new BinderPipeline<SuchInput>()
    .MapFromName("print-me", model => model.PrintMe)
    .MapFromReference(frequencyOpt, model => model.Frequency)
    .MapFromName("-l", model => model.SuchList)
    .ToHandler(SuchHandler);
```

`BinderPipeline` is really a collection of `IPropertyBinder`. Each `IPropertyBinder` defines a strategy for assigning input to the target object.
The pipeline executes each binder in the order they are given. This means later binders will override earlier ones. This also means we can
- use multiple rules to bind properties
- define a priority/fallback chain for any given property

### Blended Conventions

The pipeline can handle many approaches binding input. Here's an example of a simple naming convention with an explicit mapping fallback

```cs
rootCommand.Handler = new BinderPipeline<SuchInput>()
    .MapFromNameConvention(NameConvention.PascalCaseComparer)
    .MapFromName("-l", model => model.SuchList)
    .ToHandler(SuchHandler);
```

More conventions can be added to this pipeline. Here are some cases I haven't implemented, but would be fairly easy to add
- map default values from configuration 
- Ask a user for any missing inputs 
  - can be done with the existing setter overload, but prompts could be automated with a signature like `.PromptIfMissing(name, selector)`
- match properties based on type

See [How to Extend](#how-to-extend) for more detail.

### Binding To Existing Models

Sometimes we might want to initialize our input model separately from the input binding process (e.g. default model from configuration).

That's easy enough
```cs
SuchInput existingModelInstance = //...
rootCommand.Handler = new BinderPipeline<SuchInput>()
    .ToHandler(SuchHandler, existingModelInstance);
```

### Initializing a model with required data

Some models may want to enforce guarantees about data through the constructor, or some fields may not allow modification after initialization.

This can be handled similarly to the core library's `SetHandler`.

```cs
IModelFactory<SuchInput> modelFactory = ModelFactor.FromSymbolMap((int frequency, string printMe) => new InputModel(frequency, printMe), frequencyOpt, printMeArg);
rootCommand.Handler = new BinderPipeline<SuchInput>()
    .ToHandler(SuchHandler, modelFactory);
```

The same can be accomplished with option and argument aliases

```cs
IModelFactory<SuchInput> modelFactory = ModelFactor.FromNameMap((int frequency, string printMe) => new InputModel(frequency, printMe), "-f", "print-me");
rootCommand.Handler = new BinderPipeline<SuchInput>()
    .ToHandler(SuchHandler, modelFactory);
```


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

```cs
// Example pipeline extension
public static class BinderPipelineExtensions{
    public static BinderPipeline<InputModel> MapFromNameConvention<InputModel>(this BinderPipeline<InputModel> pipeline, NameConventionComparer comparer)
    {
        pipeline.Add(new NameConventionBinder<InputModel>(comparer)); // this adds an IPropertyBinder<T>
        return pipeline; // be sure to return the pipeline for further chaining
    }
}
```

## How to handle Dependency Injection?

Short: Invoke the dependency container in the handler function (i.e `ToHandler(handlerFunction)`)

This position of the library is to keep dependency injection separate from model binding. Some reasons include
- Keeping the two activities separate simplifies error diagnosis and improves code clarity
- Dependency containers can easily be invoked from within the handlers
- Input values may need registered with the dependency container, which requires the input model to be complete before the dependency container
- The input model should *only* model the possible input. It is not responsible for composition or behavior. 
  - The handler function exists to bridge between the input model and consumers.


## Status of project

A successful experiment. The core builder experience is likely stable, but the API could still change given feedback/experience.

The library is usable, has tests, but has no guarantees (including support).

<!-- ## How to Contribute -->