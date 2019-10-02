using System;
using System.Collections.Concurrent;

namespace PipelineImplementations.Part1.BlockingCollection.GenericBCPipeline
{
    internal sealed class GenericBCPipelineStep<TStepIn, TStepOut> : IPipelineStep<TStepIn>
    {
        public BlockingCollection<TStepIn> Buffer { get; set; } = new BlockingCollection<TStepIn>();

        public Func<TStepIn, TStepOut> StepAction { get; set; }

        public GenericBCPipelineStep()
        {
        }
    }
}
