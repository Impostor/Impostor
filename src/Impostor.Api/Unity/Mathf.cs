namespace Impostor.Api.Unity
{
    public static class Mathf
    {
        /// <summary>
        ///     <para>Clamps the given value between the given minimum float and maximum float values.  Returns the given value if it is within the min and max range.</para>
        /// </summary>
        /// <param name="value">The floating point value to restrict inside the range defined by the min and max values.</param>
        /// <param name="min">The minimum floating point value to compare against.</param>
        /// <param name="max">The maximum floating point value to compare against.</param>
        /// <returns>
        ///     <para>The float result between the min and max values.</para>
        /// </returns>
        public static float Clamp(float value, float min, float max)
        {
            if (value < (double)min)
            {
                value = min;
            }
            else if (value > (double)max)
            {
                value = max;
            }

            return value;
        }

        /// <summary>
        ///     <para>Clamps value between 0 and 1 and returns value.</para>
        /// </summary>
        /// <param name="value">Value.</param>
        /// <returns>Clamped value.</returns>
        public static float Clamp01(float value)
        {
            if (value < 0.0)
            {
                return 0.0f;
            }

            return (double)value > 1.0 ? 1f : value;
        }

        /// <summary>
        ///     <para>Linearly interpolates between a and b by t.</para>
        /// </summary>
        /// <param name="a">The start value.</param>
        /// <param name="b">The end value.</param>
        /// <param name="t">The interpolation value between the two floats.</param>
        /// <returns>
        ///     <para>The interpolated float result between the two float values.</para>
        /// </returns>
        public static float Lerp(float a, float b, float t) => a + ((b - a) * Clamp01(t));

        public static float ReverseLerp(float t)
        {
            const float range = 50f;

            return Clamp((t - -range) / (range - -range), 0f, 1f);
        }
    }
}
