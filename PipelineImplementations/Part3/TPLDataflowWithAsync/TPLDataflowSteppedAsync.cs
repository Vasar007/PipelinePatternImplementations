using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace PipelineImplementations.Part3.TPLDataflowWithAsync
{
    internal sealed class TPLDataflowSteppedAsync<TIn, TOut>
    {
        private readonly List<IDataflowBlock> _steps = new List<IDataflowBlock>();

        public TPLDataflowSteppedAsync()
        {
        }

        public void AddStep<TLocalIn, TLocalOut>(Func<TLocalIn, TLocalOut> stepFunc)
        {
            if (_steps.Count == 0)
            {
                var step = new TransformBlock<TLocalIn, TLocalOut>(input => stepFunc(input));
                _steps.Add(step);
            }
            else
            {

                var lastStep = _steps.Last();
                var targetBlock = (lastStep as ISourceBlock<TLocalIn>);
                if (targetBlock != null)
                {
                    var step = new TransformBlock<TLocalIn, TLocalOut>(input => stepFunc(input));
                    targetBlock.LinkTo(step, new DataflowLinkOptions());
                    _steps.Add(step);
                }
                else
                {
                    var step = new TransformBlock<Task<TLocalIn>, TLocalOut>(input => stepFunc(input.Result));
                    var targetBlock1 = lastStep as ISourceBlock<Task<TLocalIn>>;
                    targetBlock1.LinkTo(step, new DataflowLinkOptions());
                    _steps.Add(step);
                }
            }

        }

        public void AddStepAsync<TLocalIn, TLocalOut>(Func<TLocalIn, Task<TLocalOut>> stepFunc)
        {
            if (_steps.Count == 0)
            {
                var step = new TransformBlock<TLocalIn, Task<TLocalOut>>(input => stepFunc(input));
                _steps.Add(step);
            }
            else 
            {
                var lastStep = _steps.Last();
                if (lastStep is ISourceBlock<Task<TLocalIn>> targetBlock)
                {
                    var step = new TransformBlock<Task<TLocalIn>, Task<TLocalOut>>(input => stepFunc(input.Result));

                    targetBlock.LinkTo(step, new DataflowLinkOptions());
                    _steps.Add(step);
                }
                else
                {
                    var targetBlock1 = lastStep as ISourceBlock<TLocalIn>;
                    var step = new TransformBlock<TLocalIn, Task<TLocalOut>>(input => stepFunc(input));

                    targetBlock1.LinkTo(step, new DataflowLinkOptions());
                    _steps.Add(step);
                }
            }
            
        }

        public void CreatePipeline(Action<TOut> resultCallback)
        {
            var lastStep = _steps.Last();
            if (lastStep is ISourceBlock<Task<TOut>> targetBlock)
            {
                var callBackStep = new ActionBlock<Task<TOut>>(t => resultCallback(t.Result));
                targetBlock.LinkTo(callBackStep);
            }
            else
            {
                var callBackStep1 = new ActionBlock<TOut>(t => resultCallback(t));
                var targetBlock1 = (lastStep as ISourceBlock<TOut>);
                targetBlock1.LinkTo(callBackStep1);
            }
        }

        public void Execute(TIn input)
        {
            var firstStep = _steps.First() as ITargetBlock<TIn>;
            firstStep.SendAsync(input);
        }
    }
}
