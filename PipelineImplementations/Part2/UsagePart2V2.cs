using System;
using System.Threading.Tasks;
using PipelineImplementations.Part2.TPLDataflowV2;

namespace PipelineImplementations.Part2
{
    internal static class UsagePart2V2
    {
        public static async Task Use()
        {
            Console.WriteLine("Usage example of part 2 V2 started.");

            try
            {
                await TplDataflowPipeline();

                //Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occured:{Environment.NewLine}{ex}");
            }
            finally
            {
                Console.WriteLine("Usage example of part 2 V2 finished.");
            }
        }

        private static async Task TplDataflowPipeline()
        {
            var pipeline = new TplAsyncPipelineBuilder<string, bool>()
                .AddStep(str => Utils.FindMostCommon(str))
                .AddStep(word => Utils.CountChars(word))
                .AddStep(length => Utils.IsOdd(length))
                .Build();

            const string sentence = "The pipeline pattern is the best pattern";

            Console.WriteLine($"Pipeline executed. Sentence: {sentence}");
            var result = await pipeline.Execute(sentence);

            Console.WriteLine($"Is most common word odd? {result.ToString()}.");
        }
    }
}
