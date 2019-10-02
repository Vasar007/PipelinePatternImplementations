using System.Threading.Tasks.Dataflow;

namespace PipelineImplementations.Part2.TPLDataflow
{
    internal static class TPLDataflowPipelineWithAwaitAttempt1
    {
        public static TransformBlock<TC<string, bool>, TC<string, bool>> CreatePipeline()
        {
            var step1 = new TransformBlock<TC<string, bool>, TC<string, bool>>(
                tc =>  new TC<string,bool>(Utils.FindMostCommon(tc.Input), tc.TaskCompletionSource)
            );

            var step2 = new TransformBlock<TC<string, bool>, TC<int, bool>>(
                tc => new TC<int,bool>(Utils.CountChars(tc.Input), tc.TaskCompletionSource)
            );

            var step3 = new TransformBlock<TC<int, bool>, TC<bool, bool>>(
                tc => new TC<bool,bool>(Utils.IsOdd(tc.Input), tc.TaskCompletionSource)
            );

            var setResultStep = new ActionBlock<TC<bool, bool>>(
                tc => tc.TaskCompletionSource.SetResult(tc.Input)
            );
        
            step1.LinkTo(step2, new DataflowLinkOptions());
            step2.LinkTo(step3, new DataflowLinkOptions());
            step3.LinkTo(setResultStep, new DataflowLinkOptions());
            return step1;
        }
    }
}
