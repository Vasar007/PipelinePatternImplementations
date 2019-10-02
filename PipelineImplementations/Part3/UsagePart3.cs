using System;
using System.Threading.Tasks;
using PipelineImplementations.Part3.Disruptor;
using PipelineImplementations.Part3.TPLDataflowWithAsync;

namespace PipelineImplementations.Part3
{
    internal static class UsagePart3
    {
        public static void Use()
        {
            //DisruptorExample();
            DisruptorSimple();
            //DisruptorAwaitable();
        }

        private static async Task DisruptorAwaitable()
        {
            var pipeline = new DisruptorSimpleAwaitable<bool>()
                .AddStep<string, string>(Utils.FindMostCommon)
                .AddStep<string, int>(Utils.CountChars)
                .AddStep<int, bool>(Utils.IsOdd)
                .CreatePipeline();

            Console.WriteLine(await pipeline.Execute("The pipeline patter is the best pattern"));
        }

        private static void DisruptorSimple()
        {
            var pipeline = new DisruptorSimple()
                .AddStep<string, string>(Utils.FindMostCommon)
                .AddStep<string, int>(Utils.CountChars)
                .AddStep<int, bool>(Utils.IsOdd)
                // This last step is kind of a result callback. We'll solve it better in a minute.
                .AddStep<bool, bool>((res) =>
                {
                    Console.WriteLine(res);
                    return res;
                })
                .CreatePipeline();

            pipeline.Execute("The pipeline pattern is the best pattern");
        }
        
        private static void Simple()
        {
            var pipeline = new TPLDataflowSteppedSimple<string, bool>();
            pipeline.AddStep<string, string>(input => Utils.FindMostCommon(input));
            pipeline.AddStep<string, int>(input => Utils.CountChars(input));
            pipeline.AddStep<int, bool>(input => Utils.IsOdd(input));
            pipeline.CreatePipeline(resultCallback: res => Console.WriteLine(res));

            pipeline.Execute("The pipeline patter is the best patter");
        }

        private static void SimpleAsyncFinal2()
        {
            var pipeline = new TPLDataflowSteppedAsyncFinal2<string, bool>();

            pipeline.AddStepAsync<string, string>(input => Task.FromResult(Utils.FindMostCommon(input)));

            //pipeline.AddStepAsync<string,string>(str => Utils.FindMostCommonAsync(str));
            pipeline.AddStep<string, int>(input => Utils.CountChars(input));
            pipeline.AddStepAsync<int, bool>(input => Task.FromResult(Utils.IsOdd(input)));


            pipeline.CreatePipeline(res => Console.WriteLine(res));

            pipeline.Execute("The pipeline patter is the best patter");

        }
    }
}
