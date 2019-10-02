using System;
using PipelineImplementations.Part1.BlockingCollection;
using PipelineImplementations.Part1.BlockingCollection.CastingPipeline;
using PipelineImplementations.Part1.BlockingCollection.GenericBCPipeline;

namespace PipelineImplementations.Part1
{
    internal static class UsagePart1
    {
        public static void Use()
        {
            //GenericBCPipeline();
            //var pipeline = CastingPipeline();
            //var pipeline = InnerCastingPipeline();
            //var pipeline = GenericBCPipeline();
            //var pipeline = CastingPipelineWithParallelism();
            var pipeline = CastingPipelineWithAwait();
            //var pipeline = CreateGenericBCPipelineAwait();

            var tsk = System.Threading.Tasks.Task.Run(async () =>
            {
                Console.WriteLine(await pipeline.Execute("The pipeline pattern is the best pattern"));
                Console.WriteLine(await pipeline.Execute("The pipeline pattern is the best pattern"));
                Console.WriteLine(await pipeline.Execute("The pipeline pattern is the best pattern"));
                Console.WriteLine(await pipeline.Execute("The pipeline patter is the best patter"));
                Console.WriteLine(await pipeline.Execute("The pipeline pattern is the best pattern"));
            });
            tsk.Wait();

            //pipeline.Execute("The pipeline pattern is the best pattern");
            //pipeline.Execute("The pipeline pattern is the best pattern");
            //pipeline.Execute("The pipeline patter is the best patter");
            //pipeline.Execute("The pipeline pattern is the best pattern");

            //pipeline.Finished += res => Console.WriteLine(res);
        }

        private static GenericBCPipelineAwait<string, bool> CreateGenericBCPipelineAwait()
        {
            var pipeline = new GenericBCPipelineAwait<string, bool>((inputFirst, builder) =>
                inputFirst.Step2(builder, input => Utils.FindMostCommon(input))
                    .Step2(builder, input => Utils.CountChars(input))
                    .Step2(builder, input => Utils.IsOdd(input)));
            return pipeline;
        }

        private static IAwaitablePipeline<bool> CastingPipelineWithAwait()
        {
            var builder = new CastingPipelineWithAwait<bool>();
            builder.AddStep(input => Utils.FindMostCommon(input as string), 2, 10);
            builder.AddStep(input => Utils.CountChars(input as string), 2, 10);
            builder.AddStep(input => Utils.IsOdd((int) input), 2, 10);
            var pipeline = builder.GetPipeline();
            return pipeline;
        }

        private static IPipeline CastingPipelineWithParallelism()
        {
            var builder = new CastingPipelineWithParallelism();
            builder.AddStep(input => Utils.FindMostCommon(input as string), 2);
            builder.AddStep(input => Utils.CountChars(input as string), 2);
            builder.AddStep(input => Utils.IsOdd((int)input), 2);
            var pipeline = builder.GetPipeline();
            return pipeline;
        }

        private static IPipeline CastingPipeline()
        {
            var builder = new CastingPipelineBuilder();
            builder.AddStep(input => Utils.FindMostCommon(input as string));
            builder.AddStep(input => Utils.CountChars(input as string));
            builder.AddStep(input => Utils.IsOdd((int)input));
            var pipeline = builder.GetPipeline();
            return pipeline;
        }

        private static IPipeline InnerCastingPipeline()
        {
            var builder = new InnerPipelineBuilder();
            builder.AddStep<string, string>(input => Utils.FindMostCommon(input));
            builder.AddStep<string, int>(input => Utils.CountChars(input));
            builder.AddStep<int, bool>(input => Utils.IsOdd(input));
            var pipeline = builder.GetPipeline();
            return pipeline;
        }

        private static GenericBCPipeline<string,bool> GenericBCPipeline()
        {
            var pipeline = new GenericBCPipeline<string, bool>((inputFirst, builder) =>
                inputFirst.Step(builder, input => Utils.FindMostCommon(input))
                    .Step(builder, input => Utils.CountChars(input))
                    .Step(builder, input => Utils.IsOdd(input)));
            return pipeline;

            //pipeline.Execute("The pipeline pattern is the best pattern");
            //pipeline.Execute("The pipeline pattern is the best pattern");
            //pipeline.Execute("The pipeline pattern is the best pattern");
            //pipeline.Execute("The pipeline patter is the best patter");
            //pipeline.Execute("The pipeline pattern is the best pattern");

            //pipeline.Finished += res => Console.WriteLine(res);
        }
    }
}
