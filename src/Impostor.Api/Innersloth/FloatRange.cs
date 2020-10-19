using Impostor.Api.Unity;

namespace Impostor.Api.Innersloth
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

        public float Width => _max - _min;

        public float Lerp(float v) => Mathf.Lerp(_min, _max, v);

        public float ReverseLerp(float t) => Mathf.Clamp((t - _min) / Width, 0.0f, 1f);
    }
}