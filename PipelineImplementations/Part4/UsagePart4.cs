using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PipelineImplementations.Part4.Implementation;

namespace PipelineImplementations.Part4
{
    internal static class UsagePart4
    {
        public static async Task Use1()
        {
            var builder = new DisruptorPipelineBuilder();

            var pipeline = builder
                .Build<string, string>(Utils.FindMostCommon, 2)
                .AddStep(x => Utils.CountChars(x), 2)
                .AddStep(x => Utils.IsOdd(x), 2)
                .Create();

            Console.WriteLine(await pipeline.Execute("The pipeline pattern is the best pattern"));
            Console.WriteLine(await pipeline.Execute("The pipeline pattern is the best pattern"));
            Console.WriteLine(await pipeline.Execute("The pipeline pattern is the best pattern"));
            Console.WriteLine(await pipeline.Execute("The pipeline patter is the best patter"));
            Console.WriteLine(await pipeline.Execute("The pipeline pattern is the best pattern"));

            pipeline.Dispose();
        }

        public static async Task Use2()
        {
            var builder = new DisruptorPipelineBuilder();
            var results = new HashSet<string>();

            var pipeline = builder
                .Build<string, char>(x => x.Max(), 2)
                .AddStep(x => new string(x, 20), 2)
                .AddStep(x => $"[{x}]", 1)
                .AddStep(x => results.Add(x), 1)
                .Create();

            for (var i = 0; i < 1_000_000; i++)
            {
                _ = pipeline.Execute(i.ToString());
            }

            await pipeline.Execute("X");

            Console.WriteLine("Completed!");

            foreach (var result in results)
            {
                Console.WriteLine(result);
            }

            pipeline.Dispose();
        }
    }
}
