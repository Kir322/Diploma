using Diploma.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Diploma.Core.Clustering.KMeans
{
    public static class KMeansCentroid
    {
        public static float[][] Forgy(float[][] records, int k)
        {
            Debug.Assert(k > 0);

            // Just take k random records as centroids
            var rng = new Random((int)DateTime.Now.Ticks);
            var centroids = new float[k][];
            for (var i = 0; i < k; ++i)
            {
                centroids[i] = records[rng.Next(records.Length)];
            }

            return centroids;
        }

        public static float[][] RandomPartitioning(float[][] records, int k)
        {
            Debug.Assert(k > 0);

            var rng = new Random((int)DateTime.Now.Ticks);
            var centroids = JaggedArray.Init<float>(k, records[0].Length);
            var labels = new int[records.Length];

            // Assign each record to random cluster
            for (var i = 0; i < records.Length; ++i)
            {
                labels[i] = rng.Next(k);
            }

            Compute(centroids, records, labels);

            return centroids;
        }

        public static void Compute(float[][] centroids, float[][] records, int[] labels)
        {
            Parallel.For(0, centroids.Length, clusterIndex =>
            {
                var slots = 4;
                var accum = Vector4.Zero;
                
                for (var featureIndex = 0;
                         featureIndex < records[0].Length;
                         ++featureIndex)
                {
                    var recordIndex = 0;
                    var sum = 0.0f;
                    var clusterSize = 1;
                    for (;
                         recordIndex + slots < records.Length;
                         recordIndex += slots)
                    {
                        if (labels[recordIndex] == clusterIndex)
                        {
                            var v = new Vector4(records[recordIndex + 0][featureIndex],
                                                records[recordIndex + 1][featureIndex],
                                                records[recordIndex + 2][featureIndex],
                                                records[recordIndex + 3][featureIndex]);

                            accum += v;
                            ++clusterSize;
                        }
                    }

                    sum += Vector4.Dot(accum, Vector4.One);
                    accum = Vector4.Zero;
                    
                    for (;
                         recordIndex < records.Length;
                         ++recordIndex)
                    {
                        sum += records[recordIndex][featureIndex];
                    }

                    centroids[clusterIndex][featureIndex] = sum / clusterSize;
                }
            });
        }
    }
}
