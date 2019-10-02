using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PipelineImplementations.Part1.BlockingCollection.GenericBCPipeline
{
    internal sealed class GenericBCPipelineAwait<TPipeIn, TPipeOut>
    {
        public interface IPipelineAwaitStep<TStepIn>
        {
            BlockingCollection<Item<TStepIn>> Buffer { get; set; }
        }

        public sealed class GenericBCPipelineAwaitStep<TStepIn, TStepOut> : IPipelineAwaitStep<TStepIn>
        {
            public BlockingCollection<Item<TStepIn>> Buffer { get; set; } = new BlockingCollection<Item<TStepIn>>();

            public Func<TStepIn, TStepOut> StepAction { get; set; }

            public GenericBCPipelineAwaitStep()
            {
            }
        }

        public sealed class Item<T>
        {
            public T Input { get; set; }

            public TaskCompletionSource<TPipeOut> TaskCompletionSource { get; set; }

            public Item()
            {
            }
        }

        private readonly List<object> _pipelineSteps = new List<object>();

        public GenericBCPipelineAwait()
        {
        }

        public GenericBCPipelineAwait(Func<TPipeIn, GenericBCPipelineAwait<TPipeIn, TPipeOut>, TPipeOut> steps)
        {
            steps.Invoke(default, this); // Invoke just once to build blocking collections.
        }

        public Task<TPipeOut> Execute(TPipeIn input)
        {
            var first = _pipelineSteps[0] as IPipelineAwaitStep<TPipeIn>;
            TaskCompletionSource<TPipeOut> tsk = new TaskCompletionSource<TPipeOut>();
            first.Buffer.Add(/*input*/new Item<TPipeIn>()
            {
                Input = input,
                TaskCompletionSource = tsk
            });
            return tsk.Task;
        }

        public GenericBCPipelineAwaitStep<TStepIn, TStepOut> GenerateStep<TStepIn, TStepOut>()
        {
            var pipelineStep = new GenericBCPipelineAwaitStep<TStepIn, TStepOut>();
            var stepIndex = _pipelineSteps.Count;

            Task.Run(() =>
            {
                IPipelineAwaitStep<TStepOut> nextPipelineStep = null;

                foreach (var input in pipelineStep.Buffer.GetConsumingEnumerable())
                {
                    bool isLastStep = stepIndex == _pipelineSteps.Count - 1;
                    TStepOut output;
                    try
                    {
                        output = pipelineStep.StepAction(input.Input);
                    }
                    catch (Exception ex)
                    {
                        input.TaskCompletionSource.SetException(ex);
                        continue;
                    }
                    if (isLastStep)
                    {
                        input.TaskCompletionSource.SetResult((TPipeOut)(object)output);
                    }
                    else
                    {
                        nextPipelineStep = nextPipelineStep ?? (isLastStep ? null : _pipelineSteps[stepIndex + 1] as IPipelineAwaitStep<TStepOut>);
                        nextPipelineStep.Buffer.Add(new Item<TStepOut>() { Input  = output, TaskCompletionSource = input.TaskCompletionSource });
                    }
                }
            });

            _pipelineSteps.Add(pipelineStep);
            return pipelineStep;
        }
    }
}
