using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PipelineImplementations.Part1.BlockingCollection.CastingPipeline
{
    internal sealed class CastingPipelineWithParallelism : IPipeline
    {
        private sealed class StepInfo
        {
            public Func<object, object> Func { get; set; }
            
            public int DegreeOfParallelism { get; set; }

            public StepInfo()
            {
            }
        }

        private readonly List<StepInfo> _pipelineSteps = new List<StepInfo>();

        private IReadOnlyList<BlockingCollection<object>> _buffers;

        public event Action<object> Finished;

        public CastingPipelineWithParallelism()
        {
        }

        public void AddStep(Func<object, object> stepFunc, int degreeOfParallelism)
        {
            _pipelineSteps.Add(new StepInfo()
            {
                Func = stepFunc,
                DegreeOfParallelism = degreeOfParallelism
            });
        }

        public void Execute(object input)
        {
            var first = _buffers.First();
            first.Add(input);
        }

        public IPipeline GetPipeline()
        {
            _buffers = _pipelineSteps.Select(step => new BlockingCollection<object>()).ToArray();

            int bufferIndex = 0;
            foreach (var pipelineStep in _pipelineSteps)
            {
                var bufferIndexLocal = bufferIndex;

                for (int i = 0; i < pipelineStep.DegreeOfParallelism; ++i)
                {
                    Task.Run(() => StartStep(bufferIndexLocal, pipelineStep));
                }

                ++bufferIndex;
            }
            return this;
        }

        private void StartStep(int bufferIndexLocal, StepInfo pipelineStep)
        {
            foreach (var input in _buffers[bufferIndexLocal].GetConsumingEnumerable())
            {
                var output = pipelineStep.Func.Invoke(input);
                bool isLastStep = bufferIndexLocal == _pipelineSteps.Count - 1;
                if (isLastStep)
                {
                    // This is dangerous as the invocation is added to the last step.
                    // Alternatively, you can utilize 'BeginInvoke' like here: https://stackoverflow.com/a/16336361/1229063
                    Finished?.Invoke(output);
                }
                else
                {
                    var next = _buffers[bufferIndexLocal + 1];
                    next.Add(output);
                }
            }
        }
    }
}
