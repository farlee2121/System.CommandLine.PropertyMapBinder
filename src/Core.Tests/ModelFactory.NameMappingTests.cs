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


public class ModelFactoryNameMappingTests
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
    public void ErrorOnMissingAlias()
    {
        string symbolName = "-opt";
        var option = new Option<Guid>(symbolName);
        var root = new RootCommand();

        var modelFactory = ModelFactory.FromNameMap((Guid opt) => new Model(opt), option.Aliases.First());
        root.Handler = new BinderPipeline<Model>()
            .ToHandler((model) => { }, modelFactory);

        TestConsole testConsole = new TestConsole();
        var exitCode = root.Invoke(new string[] { }, testConsole);


        Assert.Equal(ExitCodes.UnspecifiedError, exitCode);
        Assert.Contains(nameof(ArgumentException), testConsole.Error.ToString());
    }

    
    internal void ConstructModelFromIModelFactory(int numberOfInputs, Func<Option[], IModelFactory<Model>> factory)
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
    public void ConstructModelFromNameMap1(){
        ConstructModelFromIModelFactory(1, (options) =>{
            var optionAliases = options.Select(opt => opt.Aliases.First()).ToArray();
            return ModelFactory.FromNameMap((Guid arg1) => new Model(arg1), optionAliases);
        });
    }

    [Fact]
    public void ConstructModelFromNameMap2()
    {
        ConstructModelFromIModelFactory(2, (options) => {
            var optionAliases = options.Select(opt => opt.Aliases.First()).ToArray();
            return ModelFactory.FromNameMap((Guid arg1, Guid arg2) => new Model(arg1, arg2), optionAliases);
        });
    }

    [Fact]
    public void ConstructModelFromNameMap3()
    {
        ConstructModelFromIModelFactory(3, (options) => {
            var optionAliases = options.Select(opt => opt.Aliases.First()).ToArray();
            return ModelFactory.FromNameMap((Guid arg1, Guid arg2, Guid arg3) => new Model(arg1, arg2, arg3), optionAliases);
        });
    }

    [Fact]
    public void ConstructModelFromNameMap4()
    {
        ConstructModelFromIModelFactory(4, (options) => {
            var optionAliases = options.Select(opt => opt.Aliases.First()).ToArray();
            return ModelFactory.FromNameMap((Guid arg1, Guid arg2, Guid arg3, Guid arg4 ) => new Model(arg1, arg2, arg3, arg4), optionAliases);
        });
    }

    [Fact]
    public void ConstructModelFromNameMap5()
    {
        ConstructModelFromIModelFactory(5, (options) => {
            var optionAliases = options.Select(opt => opt.Aliases.First()).ToArray();
            return ModelFactory.FromNameMap((Guid arg1, Guid arg2, Guid arg3, Guid arg4, Guid arg5) => new Model(arg1, arg2, arg3, arg4, arg5), optionAliases);
        });
    }

    [Fact]
    public void ConstructModelFromNameMap6()
    {
        ConstructModelFromIModelFactory(6, (options) => {
            var optionAliases = options.Select(opt => opt.Aliases.First()).ToArray();
            return ModelFactory.FromNameMap((Guid arg1, Guid arg2, Guid arg3, Guid arg4, Guid arg5, Guid arg6) => new Model(arg1, arg2, arg3, arg4, arg5, arg6), optionAliases);
        });
    }

    [Fact]
    public void ConstructModelFromNameMap7()
    {
        ConstructModelFromIModelFactory(7, (options) => {
            var optionAliases = options.Select(opt => opt.Aliases.First()).ToArray();
            return ModelFactory.FromNameMap((Guid arg1, Guid arg2, Guid arg3, Guid arg4, Guid arg5, Guid arg6, Guid arg7) => new Model(arg1, arg2, arg3, arg4, arg5, arg6, arg7), optionAliases);
        });
    }
}