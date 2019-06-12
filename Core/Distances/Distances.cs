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

            // @Info: Changed Vector<float>.Count to Vector4 4 because 
            // Vector<float>.Count == 8 but takes only 4 floats
            var slots = 4; // Vector<float>.Count;
            var allignment = 0;
            var distanceSquared = 0.0f;

            for (;
                 allignment + slots < vec1.Length;
                 allignment += slots)
            {
                var v1 = new Vector4(vec1[allignment],
                                     vec1[allignment + 1],
                                     vec1[allignment + 2],
                                     vec1[allignment + 3]);

                var v2 = new Vector4(vec2[allignment],
                                     vec2[allignment + 1], 
                                     vec2[allignment + 2], 
                                     vec2[allignment + 3]);

                distanceSquared += Vector4.DistanceSquared(v1, v2);
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

            // @Info: Changed Vector<float>.Count to Vector4 4 because 
            // Vector<float>.Count == 8 but takes only 4 floats
            var slots = 4; // Vector<float>.Count;
            var allignment = 0;
            var distance = 0.0f;
            for (;
                 allignment + slots < vec1.Length;
                 allignment += slots)
            {
                var v1 = new Vector4(vec1[allignment],
                                     vec1[allignment + 1],
                                     vec1[allignment + 2],
                                     vec1[allignment + 3]);

                var v2 = new Vector4(vec2[allignment],
                                     vec2[allignment + 1],
                                     vec2[allignment + 2],
                                     vec2[allignment + 3]);

                distance += Vector4.Dot(Vector4.Abs(v1 - v2), Vector4.One);
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

            // @Info: Changed Vector<float>.Count to Vector4 4 because 
            // Vector<float>.Count == 8 but takes only 4 floats
            var slots = 4; // Vector<float>.Count;
            var allignment = 0;
            var distance = float.MinValue;
            var maxs = new Vector4(float.MinValue);
            for (;
                 allignment + slots < vec1.Length;
                 allignment += slots)
            {
                var v1 = new Vector4(vec1[allignment],
                                     vec1[allignment + 1],
                                     vec1[allignment + 2],
                                     vec1[allignment + 3]);

                var v2 = new Vector4(vec2[allignment],
                                     vec2[allignment + 1],
                                     vec2[allignment + 2],
                                     vec2[allignment + 3]);

                maxs = Vector4.Max(Vector4.Abs(v1 - v2), maxs);
            }

            
            distance = MathF.Max(distance, maxs.X);
            distance = MathF.Max(distance, maxs.Y);
            distance = MathF.Max(distance, maxs.Z);
            distance = MathF.Max(distance, maxs.W);
            
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