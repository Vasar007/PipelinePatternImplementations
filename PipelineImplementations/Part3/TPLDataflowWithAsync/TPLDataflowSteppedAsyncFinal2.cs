﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace PipelineImplementations.Part3.TPLDataflowWithAsync
{
    internal sealed class TPLDataflowSteppedAsyncFinal2<TIn, TOut>
    {
        private readonly List<(IDataflowBlock Block, bool IsAsync)> _steps = new List<(IDataflowBlock Block, bool IsAsync)>();

        public TPLDataflowSteppedAsyncFinal2()
        {
        }

        public void AddStep<TLocalIn, TLocalOut>(Func<TLocalIn, TLocalOut> stepFunc)
        {
            if (_steps.Count == 0)
            {
                var step = new TransformBlock<TLocalIn, TLocalOut>(stepFunc);
                _steps.Add((step, IsAsync: false));
            }
            else
            {
                var lastStep = _steps.Last();
                if (!lastStep.IsAsync)
                {
                    var step = new TransformBlock<TLocalIn, TLocalOut>(stepFunc);
                    var targetBlock = lastStep.Block as ISourceBlock<TLocalIn>;
                    targetBlock.LinkTo(step, new DataflowLinkOptions());
                    _steps.Add((step, IsAsync: false));
                }
                else
                {
                    var step = new TransformBlock<Task<TLocalIn>, TLocalOut>(async input => stepFunc(await input));
                    var targetBlock = lastStep.Block as ISourceBlock<Task<TLocalIn>>;
                    targetBlock.LinkTo(step, new DataflowLinkOptions());
                    _steps.Add((step, IsAsync: false));
                }
            }
        }

        public void AddStepAsync<TLocalIn, TLocalOut>(Func<TLocalIn, Task<TLocalOut>> stepFunc)
        {
            if (_steps.Count == 0)
            {
                var step = new TransformBlock<TLocalIn, Task<TLocalOut>>(input => stepFunc(input));
                _steps.Add((step, IsAsync: true));
            }
            else
            {
                var lastStep = _steps.Last();
                if (lastStep.IsAsync)
                {
                    var step = new TransformBlock<Task<TLocalIn>, Task<TLocalOut>>(
                        async (input) => await stepFunc(await input)
                    );
                    var targetBlock = (lastStep.Block as ISourceBlock<Task<TLocalIn>>);
                    targetBlock.LinkTo(step, new DataflowLinkOptions());
                    _steps.Add((step, IsAsync: true));
                }
                else
                {
                    var step = new TransformBlock<TLocalIn, Task<TLocalOut>>(input => stepFunc(input));
                    var targetBlock = (lastStep.Block as ISourceBlock<TLocalIn>);
                    targetBlock.LinkTo(step, new DataflowLinkOptions());
                    _steps.Add((step, IsAsync: true));
                }
            }
        }

        public void CreatePipeline(Action<TOut> resultCallback)
        {
            var lastStep = _steps.Last();
            if (lastStep.IsAsync)
            {
                var targetBlock = (lastStep.Block as ISourceBlock<Task<TOut>>);
                var callBackStep = new ActionBlock<Task<TOut>>(async t => resultCallback(await t));
                targetBlock.LinkTo(callBackStep);
            }
            else
            {
                var callBackStep = new ActionBlock<TOut>(t => resultCallback(t));
                var targetBlock = lastStep.Block as ISourceBlock<TOut>;
                targetBlock.LinkTo(callBackStep);
            }
        }

        public void Execute(TIn input)
        {
            var firstStep = _steps.First().Block as ITargetBlock<TIn>;
            firstStep.SendAsync(input);
        }
    }
}
