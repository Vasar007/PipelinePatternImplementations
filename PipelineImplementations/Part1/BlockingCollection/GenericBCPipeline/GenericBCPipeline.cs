using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PipelineImplementations.Part1.BlockingCollection.GenericBCPipeline
{
    internal sealed class GenericBCPipeline<TPipeIn, TPipeOut>
    {
        private readonly List<object> _pipelineSteps = new List<object>();

        public event Action<TPipeOut> Finished;

        public GenericBCPipeline(Func<TPipeIn, GenericBCPipeline<TPipeIn, TPipeOut>, TPipeOut> steps)
        {
            steps.Invoke(default, this); // Invoke just once to build blocking collections.
        }

        public void Execute(TPipeIn input)
        {
            var first = _pipelineSteps.First() as IPipelineStep<TPipeIn>;
            first.Buffer.Add(input);
        }

        public GenericBCPipelineStep<TStepIn, TStepOut> GenerateStep<TStepIn, TStepOut>()
        {
            var pipelineStep = new GenericBCPipelineStep<TStepIn, TStepOut>();
            var stepIndex = _pipelineSteps.Count;

            Task.Run(() =>
            {
                IPipelineStep<TStepOut> nextPipelineStep = null;

                foreach (var input in pipelineStep.Buffer.GetConsumingEnumerable())
                {
                    bool isLastStep = stepIndex == _pipelineSteps.Count - 1;
                    var output = pipelineStep.StepAction(input);
                    if (isLastStep)
                    {
                        // This is dangerous as the invocation is added to the last step.
                        // Alternatively, you can utilize BeginInvoke like here: https://stackoverflow.com/a/16336361/1229063
                        Finished?.Invoke((TPipeOut)(object)output);
                    }
                    else
                    {
                        nextPipelineStep = nextPipelineStep ?? (isLastStep ? null : _pipelineSteps[stepIndex + 1] as IPipelineStep<TStepOut>);
                        nextPipelineStep.Buffer.Add(output);
                    }
                }
            });

            _pipelineSteps.Add(pipelineStep);
            return pipelineStep;

        }
    }
}
