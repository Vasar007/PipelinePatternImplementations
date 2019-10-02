using System;
using System.Threading.Tasks.Dataflow;

namespace PipelineImplementations.Part2.TPLDataflow
{
    internal static class TPLDataflowPipelineSimple
    {
        public static TransformBlock<string, string> CreatePipeline(Action<bool> resultCallback)
        {
            var step1 = new TransformBlock<string, string>(sentence => Utils.FindMostCommon(sentence));
            var step2 = new TransformBlock<string, int>(word => Utils.CountChars(word));
            var step3 = new TransformBlock<int, bool>(length =>
            {
                Console.WriteLine(Utils.GetThreadPoolThreadsInUse());
                return Utils.IsOdd(length);
            });

            var callBackStep = new ActionBlock<bool>(resultCallback);
            step1.LinkTo(step2, new DataflowLinkOptions());
            step2.LinkTo(step3, new DataflowLinkOptions());
            step3.LinkTo(callBackStep);

            return step1;
        }
    }
}
