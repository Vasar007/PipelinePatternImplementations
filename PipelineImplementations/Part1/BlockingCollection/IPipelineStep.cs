using System.Collections.Concurrent;

namespace PipelineImplementations.Part1.BlockingCollection
{
    internal interface IPipelineStep<TStepIn>
    {
        BlockingCollection<TStepIn> Buffer { get; set; }
    }
}
