using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Dataflow;

namespace PipelineImplementations.Part3.TPLDataflowWithAsync
{
    internal sealed class TPLDataflowSteppedSimple<TIn, TOut>
    {
        private readonly List<IDataflowBlock> _steps = new List<IDataflowBlock>();

        public TPLDataflowSteppedSimple()
        {
        }

        public void AddStep<TLocalIn, TLocalOut>(Func<TLocalIn, TLocalOut> stepFunc)
        {
            var step = new TransformBlock<TLocalIn, TLocalOut>(input => stepFunc(input));
            if (_steps.Count > 0)
            {
                var lastStep = _steps.Last();
                var targetBlock = lastStep as ISourceBlock<TLocalIn>;
                targetBlock.LinkTo(step, new DataflowLinkOptions());
            }
            _steps.Add(step);
        }

        public void CreatePipeline(Action<TOut> resultCallback)
        {
            var callBackStep = new ActionBlock<TOut>(resultCallback);
            var lastStep = _steps.Last();
            var targetBlock = lastStep as ISourceBlock<TOut>;
            targetBlock.LinkTo(callBackStep);
        }

        public void Execute(TIn input)
        {
            var firstStep = _steps.First() as ITargetBlock<TIn>;
            firstStep.SendAsync(input);
        }
    }
}
