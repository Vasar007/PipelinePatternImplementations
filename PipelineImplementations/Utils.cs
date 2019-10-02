using System;
using System.Threading;
using System.Threading.Tasks;

namespace PipelineImplementations
{
    internal static class Utils
    {
        public static int GetThreadPoolThreadsInUse()
        {
            ThreadPool.GetMaxThreads(out int max, out int _);
            ThreadPool.GetAvailableThreads(out int available, out int _);
            int running = max - available;
            return running;
        }

        public static async Task<string> FindMostCommonAsync(string input)
        {
            if (Options.Output)
            {
                Console.WriteLine("FindMostCommonAsync");
            }

            await Task.Delay(Options.Delay);

            if (Options.ShouldThrowException)
            {
                ThrowException();
            }

            return WordFinder.FindMostCommonWord(input);
        }

        public static string FindMostCommon(string input)
        {
            if (Options.Output)
            {
                Console.WriteLine("FindMostCommon");
            }

            if (Options.ShouldThrowException)
            {
                ThrowException();
            }

            return WordFinder.FindMostCommonWord(input);
        }

        public static int CountChars(string input)
        {
            if (Options.Output)
            {
                Console.WriteLine("CountChars");
            }

            if (Options.ShouldThrowException)
            {
                ThrowException();
            }

            return input.Length;
        }

        public static bool IsOdd(int number)
        {
            if (Options.Output)
            {
                Console.WriteLine("IsOdd");
            }

            if (Options.ShouldThrowException)
            {
                ThrowException();
            }

            return number % 2 == 1;
        }

        private static async Task<string> SomeAsync(string input)
        {
            if (Options.Output)
            {
                Console.WriteLine("SomeAsync");
            }

            await Task.Delay(Options.Delay);

            if (Options.ShouldThrowException)
            {
                ThrowException();
            }

            return $"asdf{input}asdf";
        }

        public static void ThrowException()
        {
            Console.WriteLine("Exception was thrown.");
            throw new Exception("It is a critical exception.");
        }
    }
}
