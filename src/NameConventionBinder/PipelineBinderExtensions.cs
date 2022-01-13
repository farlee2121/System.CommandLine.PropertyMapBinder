namespace System.CommandLine.PropertyMapBinder.NameConventionBinder
{
    public static class PipelineBinderExtensions
    {
        public static BinderPipeline<InputModel> MapFromNameConvention<InputModel>(this BinderPipeline<InputModel> pipeline, TextCase casing){
            pipeline.Add(new NamingConventionPipelineBinder<InputModel>(casing));
            return pipeline;
        }
    }
}
