namespace System.CommandLine.PropertyMapBinder.NameConventionBinder
{
    public static class PipelineBinderExtensions
    {

        public static BinderPipeline<InputModel> MapFromNameConvention<InputModel>(this BinderPipeline<InputModel> pipeline, NameConventionComparer comparer)
        {
            pipeline.Add(new NameConventionBinder<InputModel>(comparer));
            return pipeline;
        }
    }
}
