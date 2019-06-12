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
                distanceSquared += MathF.Pow(vec1[allignment] - vec2[allignment], 2);
            }

            return MathF.Sqrt(distanceSquared);
        }
    
        public static float Manhattan(ReadOnlySpan<float> vec1, ReadOnlySpan<float> vec2)
        {
            Debug.Assert(vec1.Length == vec2.Length);

            var slots = Vector<float>.Count;
            var allignment = 0;
            var distance = 0.0f;
            for (;
                 allignment + slots < vec1.Length;
                 allignment += slots)
            {
                var v1 = new Vector<float>(vec1.ToArray(), allignment);
                var v2 = new Vector<float>(vec2.ToArray(), allignment);
                distance += Vector.Dot(Vector.Abs(v1 - v2), Vector<float>.One);
            }

            for (;
                 allignment < vec1.Length;
                 ++allignment)
            {
                distance += MathF.Abs(vec1[allignment] - vec2[allignment]);
            }

            return distance;
        }

        public static float Maximum(ReadOnlySpan<float> vec1, ReadOnlySpan<float> vec2)
        {
            Debug.Assert(vec1.Length == vec2.Length);

            var slots = Vector<float>.Count;
            var allignment = 0;
            var distance = float.MinValue;
            var maxs = new Vector<float>(float.MinValue);
            for (;
                 allignment + slots < vec1.Length;
                 allignment += slots)
            {
                var v1 = new Vector<float>(vec1.ToArray(), allignment);
                var v2 = new Vector<float>(vec2.ToArray(), allignment);
                maxs = Vector.Max(Vector.Abs(v1 - v2), maxs);
            }

            for (var slot = 0;
                 slot < slots;
                 ++slot)
            {
                distance = MathF.Max(distance, maxs[slot]);
            }

            for (;
                 allignment < vec1.Length;
                 ++allignment)
            {
                distance = MathF.Max(distance, MathF.Abs(vec1[allignment] - vec2[allignment]));
            }

            return distance;
        }
    }
}