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
        public string? AlsoPascal;
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
            .MapFromNameConvention(NameConventions.PascalCaseComparer)
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
            .MapFromNameConvention(NameConventions.PascalCaseComparer)
            .ToHandler(modelSpyHandler);

        root.Invoke(new[] { expected.ThisIsPascal.ToString(), "--also-pascal", expected.AlsoPascal });

        DeepAssert.Equal(expected, actualModel);
    }

    [Fact]
    public void SilentIfNoMatch()
    {
        var expected = new PascalModel();

        var root = new RootCommand()
        {
            new Argument<int>("no-match")
        };

        PascalModel actualModel = new PascalModel();
        Action<PascalModel> modelSpyHandler = (model) => { actualModel = model; };

        root.Handler = new BinderPipeline<PascalModel>()
            .MapFromNameConvention(NameConventions.PascalCaseComparer)
            .MapFromNameConvention(NameConventions.CamelCaseComparer)
            .MapFromNameConvention(NameConventions.SnakeCaseComparer)
            .ToHandler(modelSpyHandler);

        root.Invoke(new[] { "5" });

        DeepAssert.Equal(expected, actualModel);
    }

    class SnakeModel
    {
        public int this_is_snake { get; set; }
        public string? also_snake { get; set; }
    }

    [Fact]
    public void SnakeCaseMembers()
    {
        var expected = new SnakeModel
        {
            this_is_snake = 5,
            also_snake = Guid.NewGuid().ToString(),
        };

        var root = new RootCommand()
        {
            new Argument<int>("this-is-snake"),
            new Option<string>("--also-snake"),
        };

        SnakeModel actualModel = new SnakeModel();
        Action<SnakeModel> modelSpyHandler = (model) => { actualModel = model; };

        root.Handler = new BinderPipeline<SnakeModel>()
            .MapFromNameConvention(NameConventions.SnakeCaseComparer)
            .ToHandler(modelSpyHandler);

        root.Invoke(new[] { expected.this_is_snake.ToString(), "--also-snake", expected.also_snake });

        DeepAssert.Equal(expected, actualModel);
    }

    class CamelCaseModel
    {
        public int thisIsCamel { get; set; }
        public string? alsoCamel { get; set; }
    }

    [Fact]
    public void CamelCaseMembers()
    {
        var expected = new CamelCaseModel
        {
            thisIsCamel = 5,
            alsoCamel = Guid.NewGuid().ToString(),
        };

        var root = new RootCommand()
        {
            new Argument<int>("this-is-camel"),
            new Option<string>("--also-camel"),
        };

        CamelCaseModel actualModel = new CamelCaseModel();
        Action<CamelCaseModel> modelSpyHandler = (model) => { actualModel = model; };

        root.Handler = new BinderPipeline<CamelCaseModel>()
            .MapFromNameConvention(NameConventions.CamelCaseComparer)
            .ToHandler(modelSpyHandler);

        root.Invoke(new[] { expected.thisIsCamel.ToString(), "--also-camel", expected.alsoCamel });

        DeepAssert.Equal(expected, actualModel);
    }

}
