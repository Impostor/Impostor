using System;
using System.Collections.Generic;

namespace Impostor.Server.Net.Inner.Objects.Systems
{
    internal sealed class SequenceBuffer<T>
    {
        private readonly List<SequencedData> _buffer = new();

        public SequenceBuffer(ushort sidStart = 0)
        {
            LastSid = sidStart;
        }

        public ushort LastSid { get; set; }

        public void Add(ushort sid, T info)
        {
            _buffer.Add(new SequencedData(sid, info));
        }

        public void BumpSid()
        {
            LastSid++;
        }

        public bool IsInvalidSid(ushort sid)
        {
            return !SidGreaterThan(sid, LastSid);
        }

        public bool IsNextSid(ushort sid)
        {
            return sid == (ushort)(LastSid + 1);
        }

        public IEnumerable<T> SubsequentObjs()
        {
            _buffer.Sort();
            while (_buffer.Count > 0 && IsNextSid(_buffer[0].Sid))
            {
                var result = _buffer[0].Data;
                _buffer.RemoveAt(0);
                yield return result;
            }
        }

        private static bool SidGreaterThan(ushort newSid, ushort prevSid)
        {
            var num = (ushort)(prevSid + (uint)short.MaxValue);

            return (int)prevSid < (int)num
                ? newSid > prevSid && newSid <= num
                : newSid > prevSid || newSid <= num;
        }

        private readonly struct SequencedData : IComparable<SequencedData>
        {
            public SequencedData(ushort sid, T data)
            {
                Sid = sid;
                Data = data;
            }

            public ushort Sid { get; }

            public T Data { get; }

            public int CompareTo(SequencedData other)
            {
                return Sid.CompareTo(other.Sid);
            }
        }
    }
}
