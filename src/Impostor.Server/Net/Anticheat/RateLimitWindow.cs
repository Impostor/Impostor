namespace Impostor.Server.Net.Anticheat
{
    internal struct RateLimitWindow
    {
        private const int WindowSize = 64;

        private readonly double[] _stamps;

        private int _next;

        public RateLimitWindow()
        {
            _stamps = new double[WindowSize];
        }

        public bool Hit(double now, int maxCount, double windowSeconds)
        {
            if (maxCount <= 0 || windowSeconds <= 0)
            {
                return false;
            }

            var used = 0;
            for (var i = 0; i < WindowSize; i++)
            {
                if (_stamps[i] > 0 && now - _stamps[i] < windowSeconds && ++used >= maxCount)
                {
                    return true;
                }
            }

            _stamps[_next] = now;
            _next = (_next + 1) % WindowSize;
            return false;
        }
    }
}
