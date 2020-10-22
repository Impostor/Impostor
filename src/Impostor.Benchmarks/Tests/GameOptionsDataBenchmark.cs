using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BenchmarkDotNet.Attributes;

using Impostor.Benchmarks.Logic.GameOptionsDataLogic;

namespace Impostor.Benchmarks.Tests
{
    public class GameOptionsDataBenchmark
    {
        private ReadOnlyMemory<byte> _data;

        /// <summary>
        /// Use this in a way that neither roslyn nor the jitter can just compile this away...
        /// </summary>
        private static bool aye = false;

        public GameOptionsDataBenchmark()
        {
            var dataStr = @"2e040a01000000000000803f0000803f0000c03f000034420101020100000003010f00000078000000010f01010000";
            _data = StringToByteArray(dataStr).AsMemory();
        }

        [Benchmark]
        public void TestOriginal()
        {
            var data = _data;
            for (uint i = 0; i < 21_333; i++)
            {
                GameOptionsData.DeserializeOld(data);
            }
        }

        [Benchmark]
        public void TestOptimized()
        {
            var data = _data;
            for (uint i = 0; i < 21_333; i++)
            {
                GameOptionsData.DeserializeNew03(data);
            }
        }

        [Benchmark]
        public void TestOptimizedStruct_GetSet()
        {
            GameOptionsDataStruct_GetSet res = default;

            var data = _data;
            for (uint i = 0; i < 21_333; i++)
            {
                res = new GameOptionsDataStruct_GetSet(data.Span);
            }

            aye = !(aye & res.ConfirmImpostor);
        }

        [Benchmark]
        public void TestOptimizedStruct_Fields()
        {
            GameOptionsDataStruct_Fields res = default;

            var data = _data;
            for (uint i = 0; i < 21_333; i++)
            {
                res = new GameOptionsDataStruct_Fields(data.Span);
            }

            aye = !(aye & res.ConfirmImpostor);
        }

        [Benchmark]
        public void TestOptimizedStruct_CacheSpan()
        {
            GameOptionsDataStruct_GetSet res = default;

            var data = _data.Span;
            for (uint i = 0; i < 21_333; i++)
            {
                res = new GameOptionsDataStruct_GetSet(data);
            }

            aye = !(aye & res.ConfirmImpostor);
        }

        [Benchmark]
        public void TestOptimizedStruct_Fields_CacheSpan()
        {
            GameOptionsDataStruct_Fields res = default;

            var data = _data.Span;
            for (uint i = 0; i < 21_333; i++)
            {
                res = new GameOptionsDataStruct_Fields(data);
            }

            aye = !(aye & res.ConfirmImpostor);
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }
    }
}