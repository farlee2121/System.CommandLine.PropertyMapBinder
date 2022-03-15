namespace System.CommandLine.PropertyMapBinder.NameConventionBinder
{
    public static class PipelineBinderExtensions
    {

        public static BinderPipeline<TInputModel> MapFromNameConvention<TInputModel>(this BinderPipeline<TInputModel> pipeline, NameConventionComparer comparer)
        {
            pipeline.Add(new NameConventionBinder<TInputModel>(comparer));
            return pipeline;
        }
    }
}
