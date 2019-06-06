using System;
using System.Numerics;
using System.Threading.Tasks;
using Diploma.Core.Interfaces;
using Diploma.Core.Interfaces.KMeans;

namespace Diploma.Core.Clustering.KMeans
{
    public class RandomPartitioningCentroidInitializer :
        ICentroidInitializer
    {
        private readonly IDistanceCalculator distance;

        public RandomPartitioningCentroidInitializer(IDistanceCalculator distance)
        {
            this.distance = distance;
        }

        public Span<float> Initialize(ReadOnlySpan<float> records, int d, int k)
        {
            var n = records.Length / d;
            var (labels, sizes) = this.MakePartition(n, k);
            var centroids = new float[k * d];
            Parallel.For(0, k, clusterIndex =>
            {
                var stride = Vector<float>.Count;
                for (var recordIndex = 0;
                     recordIndex < n;
                     recordIndex += d)
                {
                    var allignment = 0;
                    for (;
                         allignment + stride < d;
                         allignment += stride)
                    {
                        
                    }
                }
            });

            return centroids;
        }

        private (int[], int[]) MakePartition(int n, int k)
        {
            var labels = new int[n];
            var sizes = new int[k];
            var rng = new Random((int)DateTime.Now.Ticks);
            for (int i = 0;
                 i < n;
                 ++i)
            {
                var random = rng.Next(k);
                labels[i] = random;
                sizes[random]++;
            }

            return (labels, sizes);
        }
    }
}