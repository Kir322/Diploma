using System;
using System.Diagnostics;
using System.Numerics;

namespace Diploma.Core.Distances
{
    public static class Distance
    {
        public static float Euclidean(ReadOnlySpan<float> vec1, ReadOnlySpan<float> vec2)
        {
            Debug.Assert(vec1.Length == vec2.Length);

            var slots = Vector<float>.Count;
            var allignment = 0;
            var distanceSquared = 0.0f;

            for (;
                 allignment + slots < vec1.Length;
                 allignment += slots)
            {
                var v1 = new Vector<float>(vec1.ToArray(), allignment);
                var v2 = new Vector<float>(vec2.ToArray(), allignment);
                var dif = v1 - v2;
                distanceSquared += Vector.Dot(dif * dif, Vector<float>.One);
            }

            for (;
                 allignment < vec1.Length;
                 ++allignment)
            {
                distanceSquared += (float)Math.Pow(vec1[allignment] - vec2[allignment], 2);
            }

            return (float)Math.Sqrt(distanceSquared);
        }
    }
}