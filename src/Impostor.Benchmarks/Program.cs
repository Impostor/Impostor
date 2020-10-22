using System;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Running;
using Impostor.Benchmarks.Tests;

namespace Impostor.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<GameOptionsDataBenchmark>
            (
                DefaultConfig.Instance
                    .AddDiagnoser(MemoryDiagnoser.Default)
                    .AddDiagnoser(ThreadingDiagnoser.Default)
                    //.AddDiagnoser(new EtwProfiler()) // ... only for Windows ¯\_(ツ)_/¯
            );
        }
    }
}
