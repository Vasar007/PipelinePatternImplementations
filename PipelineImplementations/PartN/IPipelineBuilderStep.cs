using System;

namespace PipelineImplementations.PartN
{
    internal interface IPipelineBuilderStep<TIn, TOut>
    {
        IPipelineBuilderStep<TIn, TNewStepOut> AddStep<TNewStepOut>(Func<TOut, TNewStepOut> stepFunc, int workerCount);

        IPipeline<TIn, TOut> Create();
    }
}