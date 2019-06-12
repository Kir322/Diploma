using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Diploma.Core.Distances;
using Diploma.Core.Helpers;

namespace Diploma.Core.Clustering.KMeans
{
    public static class KMeansClustering
    {
        public static int[] Clusterize(float[][] records, int k, float error = 1e-5f, int maxIter = 10)
        {
            var distances = new float[k, records.Length];
            var labels = new int[records.Length];

            var centroidsPrev = KMeansCentroid.RandomPartitioning(records, k);
            var centroidsNext = JaggedArray.InitAndCopy(centroidsPrev, k);
            var iteration = 0;

            float e;
            do
            {
                CalculateDistancesBetweenRecordsAndCentroids(distances, records, centroidsNext);
                ComputeLabels(labels, distances, k);
                KMeansCentroid.Compute(centroidsNext, records, labels);

                e = ComputeError(centroidsPrev, centroidsNext);
                JaggedArray.Copy(centroidsNext, centroidsPrev, k);
            } while (iteration++ < maxIter && e > error);

            return labels;
        }

        private static void CalculateDistancesBetweenRecordsAndCentroids(float[,] distances, float[][] records, float[][] centroids)
        {
            Parallel.For(0, centroids.Length, clusterIndex => {
                for (int numRecords = 0;
                     numRecords < records.Length;
                     ++numRecords)
                {
                    distances[clusterIndex, numRecords] = Distance.Euclidean(records[numRecords],
                                                                              centroids[clusterIndex]);
                }
            });
        }

        private static void ComputeLabels(int[] labels, float[,] distances, int k)
        {
            Parallel.For(0, labels.Length, recordIndex =>
            {
                float minDistance = distances[0, recordIndex];
                int minDistanceIndex = 0;
                for(int clusterIndex = 1;
                    clusterIndex < k;
                    ++clusterIndex)
                {
                    if (minDistance > distances[clusterIndex, recordIndex])
                    {
                        minDistance = distances[clusterIndex, recordIndex];
                        minDistanceIndex = clusterIndex;
                    }
                }

                labels[recordIndex] = minDistanceIndex;
            });
        }

        private static float ComputeError(float[][] centroidsPrev, float[][] centroidsNext)
        {
            Debug.Assert(centroidsPrev.Length == centroidsNext.Length);

            var error = 0.0f;
            for (var centroidIndex = 0;
                 centroidIndex < centroidsPrev.Length;
                 ++centroidIndex)
            {
                error += Distance.Euclidean(centroidsPrev[centroidIndex], centroidsNext[centroidIndex]);
            }

            return error / centroidsPrev.Length;
        }
    }
}
