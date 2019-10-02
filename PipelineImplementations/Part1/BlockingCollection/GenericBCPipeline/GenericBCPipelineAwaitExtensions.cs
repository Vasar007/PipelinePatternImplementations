using System;

namespace PipelineImplementations.Part1.BlockingCollection.GenericBCPipeline
{
    internal static class GenericBCPipelineAwaitExtensions
    {
        public static TOutput Step<TInput, TOutput, TInputOuter, TOutputOuter>(
            this TInput inputType,
            GenericBCPipeline<TInputOuter, TOutputOuter> pipelineBuilder,
            Func<TInput, TOutput> step)
        {
            var pipelineStep = pipelineBuilder.GenerateStep<TInput, TOutput>();
            pipelineStep.StepAction = step;
            return default;
        }

        public static TOutput Step2<TInput, TOutput, TInputOuter, TOutputOuter>(
            this TInput inputType,
            GenericBCPipelineAwait<TInputOuter, TOutputOuter> pipelineBuilder,
            Func<TInput, TOutput> step)
        {
            var pipelineStep = pipelineBuilder.GenerateStep<TInput, TOutput>();
            pipelineStep.StepAction = step;
            return default;
        }
    }
}
