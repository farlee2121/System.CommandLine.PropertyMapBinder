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


public partial class CoreMappingTests
{
    public class InputModel
    {
        public int IntProperty { get; set; }
        public int IntField { get; set; }
        public IEnumerable<int>? Collection { get; set; }
    }

    [Fact]
    public void SymbolNotRegisteredOnCommand()
    {
        string symbolName = "-int";
        var option = new Option<int>(symbolName);
        var root = new RootCommand()
        {
        };

        root.Handler = new BinderPipeline<InputModel>()
            .MapFromReference(option, m => m.IntProperty)
            .ToHandler((model) => { });

        TestConsole testConsole = new TestConsole();
        var exitCode = root.Invoke(new string[] { }, testConsole);

        
        Assert.Equal(ExitCodes.UnspecifiedError, exitCode);
        Assert.Contains(nameof(ArgumentException), testConsole.Error.ToString());
    }

    [Fact]
    public void NoSymbolWithMappedName()
    {
        string symbolName = "-int";
        var option = new Option<int>(symbolName);
        var root = new RootCommand()
        {
        };
        root.Handler = new BinderPipeline<InputModel>()
                .MapFromName(symbolName, m => m.IntProperty)
                .ToHandler((model) => { });

        TestConsole testConsole = new TestConsole();
        var exitCode = root.Invoke(new string[] { }, testConsole);

        Assert.Equal(ExitCodes.UnspecifiedError, exitCode);
        Assert.Contains(nameof(ArgumentException), testConsole.Error.ToString());
    }

    [Fact]
    public void BindOptionReferenceWithSelector()
    {
        string symbolName = "-int";
        var option = new Option<int>(symbolName);
        var root = new RootCommand()
        {
            option
        };

        InputModel expectedModel = new InputModel()
        {
            IntProperty = 5
        };
        InputModel actualModel = new InputModel();
        Action<InputModel> boundModelSpy = (model) => { actualModel = model; };

        root.Handler = new BinderPipeline<InputModel>()
            .MapFromReference(option, m => m.IntProperty)
            .ToHandler(boundModelSpy);

        root.Invoke(new string[] { symbolName, expectedModel.IntProperty.ToString() });

        Assert.Equal(expectedModel.IntProperty, actualModel.IntProperty);
    }

    [Fact]
    public void BindArgumentReferenceWithSelector()
    {
        string symbolName= "int-argument";
        var argument = new Argument<int>(symbolName);
        var root = new RootCommand()
        {
            argument
        };

        InputModel expectedModel = new InputModel()
        {
            IntProperty = 5
        };
        InputModel actualModel = new InputModel();
        Action<InputModel> boundModelSpy = (model) => { actualModel = model; };

        root.Handler = new BinderPipeline<InputModel>()
            .MapFromReference(argument, m => m.IntProperty)
            .ToHandler(boundModelSpy);

        root.Invoke(new string[] { expectedModel.IntProperty.ToString() });

        Assert.Equal(expectedModel.IntProperty, actualModel.IntProperty);
    }

    [Fact]
    public void BindOptionNameWithSelector()
    {
        string symbolName = "-int";
        var option = new Option<int>(symbolName);
        var root = new RootCommand()
        {
            option
        };

        InputModel expectedModel = new InputModel()
        {
            IntProperty = 5
        };
        InputModel actualModel = new InputModel();
        Action<InputModel> boundModelSpy = (model) => { actualModel = model; };

        root.Handler = new BinderPipeline<InputModel>()
            .MapFromName(symbolName, m => m.IntProperty)
            .ToHandler(boundModelSpy);

        root.Invoke(new string[] { symbolName, expectedModel.IntProperty.ToString() });

        Assert.Equal(expectedModel.IntProperty, actualModel.IntProperty);
    }

    [Fact]
    public void BindArgumentNameWithSelector()
    {
        string symbolName = "int-argument";
        var argument = new Argument<int>(symbolName);
        var root = new RootCommand()
        {
            argument
        };

        InputModel expectedModel = new InputModel()
        {
            IntProperty = 5
        };
        InputModel actualModel = new InputModel();
        Action<InputModel> boundModelSpy = (model) => { actualModel = model; };

        root.Handler = new BinderPipeline<InputModel>()
            .MapFromName(symbolName, m => m.IntProperty)
            .ToHandler(boundModelSpy);

        root.Invoke(new string[] { expectedModel.IntProperty.ToString() });

        Assert.Equal(expectedModel.IntProperty, actualModel.IntProperty);
    }

    [Fact]
    public void BindOptionNameWithSetter()
    {
        string symbolName = "-int";
        var option = new Option<int>(symbolName);
        var root = new RootCommand()
        {
            option
        };

        InputModel expectedModel = new InputModel()
        {
            IntProperty = 5
        };
        InputModel actualModel = new InputModel();
        Action<InputModel> boundModelSpy = (model) => { actualModel = model; };

        root.Handler = new BinderPipeline<InputModel>()
            .MapFromName(symbolName, (InputModel m, int val) => { m.IntProperty = val; return m; })
            .ToHandler(boundModelSpy);

        root.Invoke(new string[] { symbolName, expectedModel.IntProperty.ToString() });

        Assert.Equal(expectedModel.IntProperty, actualModel.IntProperty);
    }

    [Fact]
    public void BindArgumentNameWithSetter()
    {
        string symbolName = "int-argument";
        var argument = new Argument<int>(symbolName);
        var root = new RootCommand()
        {
            argument
        };

        InputModel expectedModel = new InputModel()
        {
            IntProperty = 5
        };
        InputModel actualModel = new InputModel();
        Action<InputModel> boundModelSpy = (model) => { actualModel = model; };

        root.Handler = new BinderPipeline<InputModel>()
            .MapFromName(symbolName, (InputModel m, int val) => { m.IntProperty = val; return m; })
            .ToHandler(boundModelSpy);

        root.Invoke(new string[] { expectedModel.IntProperty.ToString() });

        Assert.Equal(expectedModel.IntProperty, actualModel.IntProperty);
    }

    [Fact]
    public void EnsureCollectionsBindCorrectly()
    {
        string symbolName = "-int";
        var option = new Option<IEnumerable<int>>(symbolName)
        {
            Arity = ArgumentArity.ZeroOrMore
        };
        var root = new RootCommand()
        {
            option
        };

        InputModel expectedModel = new InputModel()
        {
            Collection = new int[] {1,2,4,8}
        };
        InputModel actualModel = new InputModel();
        Action<InputModel> boundModelSpy = (model) => { actualModel = model; };

        root.Handler = new BinderPipeline<InputModel>()
            .MapFromName(symbolName, m => m.Collection)
            .ToHandler(boundModelSpy);

        var collectionCliInput = expectedModel.Collection.SelectMany((val) => new string[] { symbolName, val.ToString() });
        int exitCode = root.Invoke(collectionCliInput.ToArray());

        Assert.Equal(ExitCodes.Normal, exitCode);
        Assert.Equal(expectedModel.Collection, actualModel.Collection);
    }

    [Fact]
    public void BindToFieldWithSelector()
    {
        string symbolName = "-int";
        var option = new Option<int>(symbolName);
        var root = new RootCommand()
        {
            option
        };

        InputModel expectedModel = new InputModel()
        {
            IntField = 5
        };
        InputModel actualModel = new InputModel();
        Action<InputModel> boundModelSpy = (model) => { actualModel = model; };

        root.Handler = new BinderPipeline<InputModel>()
            .MapFromReference(option, m => m.IntField)
            .ToHandler(boundModelSpy);

        root.Invoke(new string[] { symbolName, expectedModel.IntField.ToString() });

        Assert.Equal(expectedModel.IntField, actualModel.IntField);
    }

    [Fact]
    public void ExecutePipelineOnCallerProvidedModelInstance()
    {
        string symbolName = "-int";
        var option = new Option<int>(symbolName);
        var root = new RootCommand()
        {
            option
        };

        InputModel expectedModel = new InputModel()
        {
            IntField = 5
        };
        InputModel actualModel = new InputModel();
        Action<InputModel> boundModelSpy = (model) => { actualModel = model; };

        root.Handler = new BinderPipeline<InputModel>()
            .MapFromReference(option, m => m.IntField)
            .ToHandler(boundModelSpy, actualModel);

        root.Invoke(new string[] { symbolName, expectedModel.IntField.ToString() });

        Assert.Equal(expectedModel.IntField, actualModel.IntField);
    }

    [Fact]
    public void LaterBindersOverrideEarlierValues()
    {
        string symbolName = "-int";
        var option = new Option<int>(symbolName);
        var root = new RootCommand()
        {
            option
        };

        InputModel expectedModel = new InputModel()
        {
            IntField = 5
        };
        InputModel actualModel = new InputModel();
        Action<InputModel> boundModelSpy = (model) => { actualModel = model; };

        root.Handler = new BinderPipeline<InputModel>()
            .MapFromValue(m => m.IntField, 20)
            .MapFromValue(m => m.IntField, expectedModel.IntField)
            .ToHandler(boundModelSpy, actualModel);

        root.Invoke(new string[] { symbolName, expectedModel.IntField.ToString() });

        Assert.Equal(expectedModel.IntField, actualModel.IntField);
    }

}