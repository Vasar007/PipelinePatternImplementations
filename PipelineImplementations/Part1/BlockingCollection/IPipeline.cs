using System;

namespace PipelineImplementations.Part1.BlockingCollection
{
    internal interface IPipeline
    {
        void Execute(object input);

        event Action<object> Finished;
    }
}
