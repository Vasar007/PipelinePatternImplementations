using System;
using System.Threading.Tasks.Dataflow;

namespace PipelineImplementations.Part2.TPLDataflow
{
    internal static class TPLDataflowPipelineSimpleWithOptionalRequirements
    {
        public static TransformBlock<string, string> CreatePipeline(Action<bool> resultCallback)
        {
            var step1 = new TransformBlock<string, string>(sentence => Utils.FindMostCommon(sentence), 
                new ExecutionDataflowBlockOptions()
                {
                    MaxDegreeOfParallelism = 3,
                    BoundedCapacity = 5,
                });
            var step2 = new TransformBlock<string, int>(word => Utils.CountChars(word), 
                new ExecutionDataflowBlockOptions()
                {
                    MaxDegreeOfParallelism = 1,
                    BoundedCapacity = 13,
                });

            var step3 = new TransformBlock<int, bool>(length => Utils.IsOdd(length), 
                new ExecutionDataflowBlockOptions()
                {
                    MaxDegreeOfParallelism = 11,
                    BoundedCapacity = 6,
                });
            
            var callBackStep = new ActionBlock<bool>(resultCallback);
            step1.LinkTo(step2, new DataflowLinkOptions());
            step2.LinkTo(step3, new DataflowLinkOptions());
            step3.LinkTo(callBackStep);
            return step1;
        }
    }
}
