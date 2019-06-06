using System;
using System.Numerics;
using Diploma.Core.Interfaces;

namespace Diploma.Core.Clustering.Distances
{
    public class EuclideanDistanceCalculator : IDistanceCalculator
    {
        public float Calculate(ReadOnlySpan<float> left, ReadOnlySpan<float> right)
        {
            var stride = Vector<float>.Count;
            var allignment = 0;
            var distanceSquared = 0.0f;
            for (;
                 allignment + stride < left.Length;
                 allignment += stride)
            {
                var l = new Vector<float>(left.ToArray(), allignment);
                var r = new Vector<float>(right.ToArray(), allignment);
                var dif = l - r;
                distanceSquared += Vector.Dot(dif * dif, Vector<float>.One);
            }

            for (;
                 allignment < left.Length;
                 ++allignment)
            {
                distanceSquared += (float)Math.Pow(left[allignment] - right[allignment], 2);
            }

            return (float)Math.Sqrt(distanceSquared);
        }
    }
}