using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Running;
using Impostor.Benchmarks.Tests;

namespace Impostor.Benchmarks
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            // BenchmarkRunner.Run<EventManagerBenchmark>(
            //     DefaultConfig.Instance
            //         .AddDiagnoser(MemoryDiagnoser.Default)
            // );

            BenchmarkRunner.Run<MessageReaderBenchmark>(
                DefaultConfig.Instance
                    .AddDiagnoser(MemoryDiagnoser.Default)
            );
        }
    }
}
