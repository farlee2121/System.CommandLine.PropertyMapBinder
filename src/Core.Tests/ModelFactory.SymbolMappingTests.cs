using Xunit;
using FsCheck;
using System.CommandLine;
using System.CommandLine.PropertyMapBinder;
using System;
using System.CommandLine.Parsing;
using System.CommandLine.IO;
using System.Collections.Generic;
using System.Linq;

namespace Core.Tests;


public class ModelFactorySymbolMappingTests
{

    public class Model{
        public List<Guid> Inputs {get; set;} = new List<Guid>();

        public Model(List<Guid> inputs)
        {
            Inputs = inputs;
        }
        public Model(params Guid[] inputs){
            Inputs = inputs.ToList();
        }
    }

    //TODO: Don't for get error behaviors (missing symbol)

    [Fact]
    public void ErrorOnMissingSymbol()
    {
        string symbolName = "-opt";
        var option = new Option<Guid>(symbolName);
        var root = new RootCommand();

        var modelFactory = ModelFactory.FromSymbolMap((Guid opt) => new Model(opt), option);
        root.Handler = new BinderPipeline<Model>()
            .ToHandler((model) => { }, modelFactory);

        TestConsole testConsole = new TestConsole();
        var exitCode = root.Invoke(new string[] { }, testConsole);


        Assert.Equal(ExitCodes.UnspecifiedError, exitCode);
        Assert.Contains(nameof(ArgumentException), testConsole.Error.ToString());
    }

    
    internal void ConstructModelFromIModelFactory(int numberOfInputs, Func<Symbol[], IModelFactory<Model>> factory)
    {
        var options = Enumerable.Range(0, numberOfInputs).Select(i => new Option<Guid>($"-opt{i}")).ToList();
        var root = new RootCommand()
            .AddOptions(options);
        
        var expectedValues = Enumerable.Range(0, numberOfInputs).Select(i => Guid.NewGuid()).ToList();
        Model expectedModel = new Model(expectedValues);

        Model? actualModel = null;
        Action<Model> boundModelSpy = (model) => { actualModel = model; };

        root.Handler = new BinderPipeline<Model>()
            .ToHandler(boundModelSpy, factory(options.ToArray()));

        var argv = Enumerable.Range(0,numberOfInputs)
            .SelectMany(i => new []{options[i].Aliases.First(), expectedValues[i].ToString()})
            .ToArray();
        root.Invoke(argv);

        Assert.Equal(expectedModel.Inputs, actualModel?.Inputs);
    }

    [Fact]
    public void ConstructModelFromSymbolMap1(){
        ConstructModelFromIModelFactory(1, (options) =>{
            return ModelFactory.FromSymbolMap((Guid arg1) => new Model(arg1), options);
        });
    }

    [Fact]
    public void ConstructModelFromSymbolMap2()
    {
        ConstructModelFromIModelFactory(2, (options) => {
            return ModelFactory.FromSymbolMap((Guid arg1, Guid arg2) => new Model(arg1, arg2), options);
        });
    }

    [Fact]
    public void ConstructModelFromSymbolMap3()
    {
        ConstructModelFromIModelFactory(3, (options) => {
            return ModelFactory.FromSymbolMap((Guid arg1, Guid arg2, Guid arg3) => new Model(arg1, arg2, arg3), options);
        });
    }

    [Fact]
    public void ConstructModelFromSymbolMap4()
    {
        ConstructModelFromIModelFactory(4, (options) => {
            return ModelFactory.FromSymbolMap((Guid arg1, Guid arg2, Guid arg3, Guid arg4 ) => new Model(arg1, arg2, arg3, arg4), options);
        });
    }

    [Fact]
    public void ConstructModelFromSymbolMap5()
    {
        ConstructModelFromIModelFactory(5, (options) => {
            return ModelFactory.FromSymbolMap((Guid arg1, Guid arg2, Guid arg3, Guid arg4, Guid arg5) => new Model(arg1, arg2, arg3, arg4, arg5), options);
        });
    }

    [Fact]
    public void ConstructModelFromSymbolMap6()
    {
        ConstructModelFromIModelFactory(6, (options) => {
            return ModelFactory.FromSymbolMap((Guid arg1, Guid arg2, Guid arg3, Guid arg4, Guid arg5, Guid arg6) => new Model(arg1, arg2, arg3, arg4, arg5, arg6), options);
        });
    }

    [Fact]
    public void ConstructModelFromSymbolMap7()
    {
        ConstructModelFromIModelFactory(7, (options) => {
            return ModelFactory.FromSymbolMap((Guid arg1, Guid arg2, Guid arg3, Guid arg4, Guid arg5, Guid arg6, Guid arg7) => new Model(arg1, arg2, arg3, arg4, arg5, arg6, arg7), options);
        });
    }
}

public static class CommandExtensions 
{
    public static RootCommand AddOptions(this RootCommand command, IEnumerable<Option> options){
        options.ToList().ForEach(opt => command.AddOption(opt));
        return command;
    }
}