namespace PipelineImplementations.Part1
{
    internal sealed class MyPipeline
    {
        public MyPipeline()
        {
        }

        public bool Execute(string input)
        {
            string mostCommon = Utils.FindMostCommon(input);
            int characters = Utils.CountChars(mostCommon);
            bool isOdd = Utils.IsOdd(characters);
            return isOdd;
        }
    }
}
