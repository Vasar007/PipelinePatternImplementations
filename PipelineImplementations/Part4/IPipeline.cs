using System;
using System.Threading.Tasks;

namespace PipelineImplementations.Part4
{
    internal interface IPipeline<TIn, TOut> : IDisposable
    {
        // TODO: use ValueTask + IValueTaskSource to avoid allocations.
        Task<TOut> Execute(TIn data);
    }
}
