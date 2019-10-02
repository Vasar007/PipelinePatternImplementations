using System.Threading.Tasks;

namespace PipelineImplementations.Part2.TPLDataflow
{
    internal sealed class TC<TInput, TOutput>
    {
        public TInput Input { get; set; }

        public TaskCompletionSource<TOutput> TaskCompletionSource { get; set; }

        public TC(TInput input, TaskCompletionSource<TOutput> tcs)
        {
            Input = input;
            TaskCompletionSource = tcs;
        }
    }
}
