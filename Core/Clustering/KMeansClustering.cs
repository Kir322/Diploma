using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Collections.Concurrent;

using Diploma.Core.Data;
using Diploma.Core.Distances;
using Diploma.Core.DataStructures;

namespace Diploma.Core.Clustering
{
    public static class KMeansClustering
    {
        public static int[] Clusterize(DataFrame frame, int k)
        {
            var centroids = new float[k][];
            var distances = new float[k, frame.NumRecords];
            var clusterIndicies = new int[frame.NumRecords];

            var records = (float[][])frame;

            ComputeInitialCentroids(centroids, records, k);

            int iteration = 0;
            while(iteration < 100)
            {
                CalculateDistancesBetweenRecordsAndCentroids(distances, records, centroids);
                ComputeClusterIndicies(clusterIndicies, distances, k);
                RecomputeCentroids(centroids, records, clusterIndicies);

                iteration++;
            }

            return clusterIndicies;
        }

        private static void ComputeInitialCentroids(float[][] centroids, float[][] records, int k)
        {
            for (int centroidIndex = 0;
                 centroidIndex < k;
                 ++centroidIndex)
            {
                centroids[centroidIndex] = records[centroidIndex];
            }
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

        private static void  ComputeClusterIndicies(int[] clusterIndicies, float[,] distances, int k)
        {
            Parallel.For(0, clusterIndicies.Length, recordIndex =>
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

                clusterIndicies[recordIndex] = minDistanceIndex;
            });
        }

        private static void RecomputeCentroids(float[][] centroids, float[][] records, int[] clusterIndicies)
        {
            Parallel.For(0, centroids.Length, centroidIndex =>
            {
                for (int featureIndex = 0;
                     featureIndex < centroids[0].Length;
                     ++featureIndex)
                {
                    float featureSum = 0.0f;
                    int numRecordsInCluster = 1;
                    for (int recordIndex = 0;
                         recordIndex < records.Length;
                         ++recordIndex)
                    {
                        if (clusterIndicies[recordIndex] != centroidIndex) continue;
                        
                        featureSum += records[recordIndex][featureIndex];
                        numRecordsInCluster++;
                    }

                    centroids[centroidIndex][featureIndex] = featureSum / numRecordsInCluster;
                }
            });
        }
    }
}
