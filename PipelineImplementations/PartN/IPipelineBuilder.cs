using System;

namespace PipelineImplementations.PartN
{
    internal interface IPipelineBuilder
    {
        IPipelineBuilderStep<TStepIn, TStepOut> Build<TStepIn, TStepOut>(Func<TStepIn, TStepOut> stepFunc, int workerCount);
    }
}