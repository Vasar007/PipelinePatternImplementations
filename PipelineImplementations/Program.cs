using System;
using System.Threading.Tasks;
using PipelineImplementations.Part1;
using PipelineImplementations.Part2;
using PipelineImplementations.Part3;
using PipelineImplementations.Part4;

namespace PipelineImplementations
{
    public static class Program
    {
        private static async Task Main()
        {
            const string input = "The pipeline pattern is the best pattern";
            var pipeline = new MyPipeline();
            Console.WriteLine(pipeline.Execute(input)); // Returns 'True' because 'pattern' is the most common, with 7 characters and it's indeed an odd number.
            UsagePart1.Use();

            UsagePart2.Use();
            await UsagePart2V2.Use();

            UsagePart3.Use();

            await UsagePart4.Use1();
            await UsagePart4.Use2();

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
