using BenchmarkDotNet.Running;

namespace ResourceAPIBenchmark
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var summary = BenchmarkRunner
                .Run<ResourceBenchmark>();
        }
    }
}