using Xunit;
using System;
using System.CommandLine;
using System.CommandLine.PropertyMapBinder;
using System.CommandLine.PropertyMapBinder.NameConventionBinder;

namespace NameConventionBinder.Tests;

public class NameConventionBinderTests
{

    public class PascalModel{
        public int ThisIsPascal { get; set; }
        public string AlsoPascal;
    }

    [Fact]
    public void PascalMembers()
    {
        var expected = new PascalModel{
            ThisIsPascal = 5,
            AlsoPascal = Guid.NewGuid().ToString(),
        };

        var root = new RootCommand()
        {
            new Argument<int>("this-is-pascal"),
            new Option<string>("--also-pascal"),
        };

        PascalModel actualModel = new PascalModel();
        Action<PascalModel> modelSpyHandler = (model) => { actualModel = model; };

        root.Handler = new BinderPipeline<PascalModel>()
            .MapFromNameConvention(TextCase.Pascal)
            .ToHandler(modelSpyHandler);

        root.Invoke(new[] { expected.ThisIsPascal.ToString(), "--also-pascal", expected.AlsoPascal });

        DeepAssert.Equal(expected, actualModel);
    }

    [Fact]
    public void AnyAliasIsMatched_Pascal()
    {
        var expected = new PascalModel
        {
            ThisIsPascal = 5,
            AlsoPascal = Guid.NewGuid().ToString(),
        };

        var root = new RootCommand()
        {
            new Argument<int>("this-is-pascal"),
            new Option<string>(new string[]{"-a", "--also-pascal"}),
        };

        PascalModel actualModel = new PascalModel();
        Action<PascalModel> modelSpyHandler = (model) => { actualModel = model; };

        root.Handler = new BinderPipeline<PascalModel>()
            .MapFromNameConvention(TextCase.Pascal)
            .ToHandler(modelSpyHandler);

        root.Invoke(new[] { expected.ThisIsPascal.ToString(), "--also-pascal", expected.AlsoPascal });

        DeepAssert.Equal(expected, actualModel);
    }

}
