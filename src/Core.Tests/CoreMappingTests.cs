using Xunit;
using FsCheck;
using FsCheck.Xunit;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.PropertyMapBinder;
using System;
using System.CommandLine.Parsing;
using System.CommandLine.IO;

namespace Core.Tests;

//public class CliName{
//    public static Arbitrary<string> String()
//    {
//        Arb.
//    }
//}

public class CoreMappingTests
{
    public class InputModel
    {
        public int IntProperty { get; set; }
        public int IntField { get; set; }
    }

    static class ExitCodes
    {
        public const int Normal = 0;
        public const int UnspecifiedError = 1;
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

    
    // TEST: bind to field
    // TEST: cannot bind to private members
}