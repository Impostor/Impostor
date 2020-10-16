using Impostor.Shared.Unity;

namespace Impostor.Shared.Innersloth
{
    public class FloatRange
    {
        private readonly float _min;
        private readonly float _max;

        public FloatRange(float min, float max)
        {
            _min = min;
            _max = max;
        }

        public float Lerp(float v) => Mathf.Lerp(this._min, this._max, v);
    }
}