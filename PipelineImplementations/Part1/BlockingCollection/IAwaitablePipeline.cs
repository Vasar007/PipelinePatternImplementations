using System.Threading.Tasks;

namespace PipelineImplementations.Part1.BlockingCollection
{
    internal interface IAwaitablePipeline<TOutput>
    {
        Task<TOutput> Execute(object input);
    }
}
