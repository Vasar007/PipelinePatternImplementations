using System;

namespace PipelineImplementations.Part4
{
    internal interface IPipelineBuilder
    {
        IPipelineBuilderStep<TStepIn, TStepOut> Build<TStepIn, TStepOut>(Func<TStepIn, TStepOut> stepFunc, int workerCount);
    }
}