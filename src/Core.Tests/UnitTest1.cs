using Xunit;
using FsCheck;
using FsCheck.Xunit;
using System.CommandLine;
using System.CommandLine.PropertyMapBinder;
using System;

namespace Core.Tests;

//public class CliName{
//    public static Arbitrary<string> String()
//    {
//        Arb.
//    }
//}

public class UnitTest1
{
    public class InputModel
    {
        public int IntProperty { get; set; }
        public int IntField { get; set; }
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
}