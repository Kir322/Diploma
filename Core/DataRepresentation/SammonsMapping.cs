using System;
using System.Threading;
using System.Threading.Tasks;

namespace Diploma.Core.DataRepresentation
{
    public static class SammonsMapping
    {
        #region Public Static Methods

        public static float[][] MapTo2D(float[][] records, float error = 1e-5f, int maxIter = 100)
        {
            return Map(records, error, maxIter, resultDimensions: 2);
        }

        public static float[][] MapTo3D(float[][] records, float error = 1e-5f, int maxIter = 100)
        {
            return Map(records, error, maxIter, resultDimensions: 3);
        }

        #endregion

        #region Private Static Methods

        private static float[][] Map(float[][] records, float error, int maxIter, int resultDimensions)
        {
            var n = records.Length;
            var dDistances = new float[n * (n + 1) / 2];
            var yDistances = new float[n * (n + 1) / 2];

            // Sammon's error
            var e = 0.0f;
            var iteration = 0;

            var ys = CalculateInitialConfiguration(n: records.Length, d: resultDimensions);

            while(e > error || iteration++ < maxIter)
            {

            }

            return ys;
        }

        private static float[][] CalculateInitialConfiguration(int n, int d)
        {
            var ys = new float[n][];
            var rng = new Random((int)DateTime.Now.Ticks);

            for (int yIndex = 0;
                 yIndex < n;
                 ++yIndex)
            {
                ys[yIndex] = new float[d];

                for (int dimension = 0;
                     dimension < d;
                     ++dimension)
                {
                    ys[yIndex][dimension] = (float)rng.NextDouble();
                }
            }

            return ys;
        }

        private static void CalculateDistances(float[] disatanceMatrix, int dimensions, float[][] data)
        {
            
        }

        #endregion
    }
}