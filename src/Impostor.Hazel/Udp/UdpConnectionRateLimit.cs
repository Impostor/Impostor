using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;
using Serilog;

namespace Impostor.Hazel.Udp
{
    public class UdpConnectionRateLimit : IDisposable
    {
        private static readonly ILogger Logger = Log.ForContext<UdpConnectionRateLimit>();

        // Allow burst to 5 connections.
        // Decrease by 1 every second.
        private const int MaxConnections = 5;
        private const int FalloffMs = 1000;

        private readonly ConcurrentDictionary<IPAddress, int> _connectionCount;
        private readonly Timer _timer;
        private bool _isDisposed;

        public UdpConnectionRateLimit()
        {
            _connectionCount = new ConcurrentDictionary<IPAddress, int>();
            _timer = new Timer(UpdateRateLimit, null, FalloffMs, Timeout.Infinite);
        }

        private void UpdateRateLimit(object state)
        {
            try
            {
                foreach (var pair in _connectionCount)
                {
                    var count = pair.Value - 1;
                    if (count > 0)
                    {
                        _connectionCount.TryUpdate(pair.Key, count, pair.Value);
                    }
                    else
                    {
                        _connectionCount.TryRemove(pair);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Exception caught in UpdateRateLimit.");
            }
            finally
            {
                if (!_isDisposed)
                {
                    _timer.Change(FalloffMs, Timeout.Infinite);
                }
            }
        }

        public bool IsAllowed(IPAddress key)
        {
            if (_connectionCount.TryGetValue(key, out var value) && value >= MaxConnections)
            {
                return false;
            }

            _connectionCount.AddOrUpdate(key, _ => 1, (_, i) => i + 1);
            return true;
        }

        public void Dispose()
        {
            _isDisposed = true;
            _timer.Dispose();
        }
    }
}