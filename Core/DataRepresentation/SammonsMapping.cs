using System;
using System.Numerics;
using System.Threading.Tasks;
using Diploma.Core.Distances;
using Diploma.Core.Helpers;

namespace Diploma.Core.DataRepresentation
{
    public static class SammonsMapping
    {
        #region Public Static Methods

        public static float[][] Map2D(float[][] records, float error = 1e-5f, int maxIter = 100)
        {
            return Map(records, error, maxIter, resultDimensions: 2);
        }

        public static float[][] Map3D(float[][] records, float error = 1e-5f, int maxIter = 100)
        {
            return Map(records, error, maxIter, resultDimensions: 3);
        }

        #endregion

        #region Private Static Methods

        private static float[][] Map(float[][] records, float error, int maxIter, int resultDimensions)
        {
            var n = records.Length;
            var dDistances = JaggedArray.Init<float>(n, n);
            var yDistances = JaggedArray.Init<float>(n, n);
            var dedy       = JaggedArray.Init<float>(n, resultDimensions);
            var d2edy2     = JaggedArray.Init<float>(n, resultDimensions); 
            var deltas     = JaggedArray.Init<float>(n, resultDimensions);
            var ys = CalculateInitialConfiguration(n: records.Length, d: resultDimensions);

            var alpha = 0.3f;
            var iteration = 0;
            var e = 0.0f;
            do
            {
                CalculateDistances(dDistances, yDistances, records, ys);
                CalculatePartialDerivatives(dedy, d2edy2, dDistances, yDistances, ys);
                CalculateDeltas(deltas, dedy, d2edy2);
                CalculateNewConfiguration(ys, deltas, alpha);

                e = CalculateError(dDistances, yDistances);
            } while(e > error || iteration++ < maxIter);

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

        private static void CalculateDistances(float[][] dds,float[][] yds, float[][] ds, float[][] ys)
        {
            Parallel.For(0, ds.Length, row =>
            {
                for (var col = row + 1;
                     col < ds.Length;
                     ++col)
                {
                    dds[row][col] = Distance.Euclidean(ds[row], ds[col]);
                    yds[row][col] = Distance.Euclidean(ys[row], ys[col]);
                }
            });
        }

        private static float CalculateError(float[][] dds, float[][] yds)
        {
            var errSum = 0.0f;
            var ddsSum = 0.0f;
            
            Parallel.For(0, dds.Length, row =>
            {
                var slots = Vector<float>.Count;
                var ddsAccum = Vector<float>.Zero;
                var errAccum = Vector<float>.Zero;
                var col = row + 1;
                for (;
                     col + slots < dds[row].Length - row;
                     col += slots)
                {
                    var ddsVec = new Vector<float>(dds[row], col);
                    var ydsVec = new Vector<float>(yds[row], col);
                    var dif = ddsVec - ydsVec;

                    ddsAccum += ddsVec;
                    errAccum += dif * dif / ddsVec;
                }

                ddsSum += Vector.Dot(ddsAccum, Vector<float>.One);
                errSum += Vector.Dot(errAccum, Vector<float>.One);

                for (;
                     col < dds[row].Length;
                     ++col)
                {
                    ddsSum += dds[row][col];

                    var dif = dds[row][col] - yds[row][col];
                    errSum += MathF.Pow(dif, 2) / dds[row][col];
                }
            });

            return errSum / ddsSum;
        }

        private static void CalculatePartialDerivatives(float[][] dedy, float[][] d2edy2, float[][] dds, float[][] yds, float[][] ys)
        {
            var c = 0.0f;
            Parallel.For(0, ys.Length, row =>
            {
                var slots = Vector<float>.Count;
                var accum = Vector<float>.Zero;
                var col = row + 1;
                for (; col + slots < ys[row].Length - row; ++col)
                {
                    var v = new Vector<float>(dds[row], col);
                    accum += v;
                }

                c += Vector.Dot(accum, Vector<float>.One);

                for (; col < ys[row].Length; ++col)
                {
                    c += dds[row][col];
                }
            });

            Parallel.For(0, ys.Length, i =>
            {
                var ysColumns = new float[ys[0].Length][];
                for (var k = 0; k < ys[0].Length; ++k) ysColumns[k] = JaggedArray.GetColumn(ys, k);

                for (var j = 0; j < ys[i].Length; ++j)
                {
                    var slots       = Vector<float>.Count;
                    var dedyAccum   = Vector<float>.Zero;
                    var d2edy2Accum = Vector<float>.Zero;
                    var dedySum     = 0.0f;
                    var d2edy2Sum   = 0.0f;
                    var s           = 0;
                    for (; s + slots < ys.Length; s += slots)
                    {
                        var dis             = new Vector<float>(dds[i], s);
                        var disStar         = new Vector<float>(yds[i], s);
                        var yij             = new Vector<float>(ys[i][j]);
                        var ysj             = new Vector<float>(ysColumns[j], s);
                        var disMinusDisStar = dis - disStar;
                        var disMulDisStar   = dis * disStar;
                        var yijMinusYsj     = yij - ysj;
                        var one             = Vector<float>.One;

                        dedyAccum += disMinusDisStar / disMulDisStar * yijMinusYsj;
                        d2edy2Accum += one / disMulDisStar *
                                       ( 
                                         disMinusDisStar -
                                         (yijMinusYsj * yijMinusYsj) / disStar *
                                         (one + disMinusDisStar / dis)
                                       );
                    }

                    dedySum += Vector.Dot(dedyAccum, Vector<float>.One);
                    d2edy2Sum += Vector.Dot(d2edy2Accum, Vector<float>.One);

                    for (; s < ys.Length; ++s)
                    {
                        var disMinusDisStar = dds[i][s] - yds[i][s];
                        var disMulDisStar   = dds[i][s] * yds[i][s];
                        var yijMinusYsj     =  ys[i][j] -  ys[s][j];
                        var one             = 1.0f;

                        dedySum += disMinusDisStar /
                                   disMulDisStar *
                                   yijMinusYsj;

                        d2edy2Sum += one / disMulDisStar *
                                       ( 
                                         disMinusDisStar -
                                         (yijMinusYsj * yijMinusYsj) / yds[i][s] *
                                         (one + disMinusDisStar / dds[i][s])
                                       );
                    }

                    dedy[i][j]   = dedySum * -2.0f / c;
                    d2edy2[i][j] = d2edy2Sum * -2.0f / c;
                };
            });
        }

        private static void CalculateDeltas(float[][] deltas, float[][] dedy, float[][] d2edy2)
        {
            Parallel.For(0, deltas.Length, i =>
            {
                var slots = Vector<float>.Count;
                var allign = 0;
                for (; allign + slots < deltas[i].Length; allign += slots)
                {
                    var dedyVec   = new Vector<float>(dedy[i], allign);
                    var d2edy2Vec = new Vector<float>(d2edy2[i], allign);
                    var v         = dedyVec / d2edy2Vec;
                    v.CopyTo(deltas[i], allign);
                }

                for (; allign < deltas[i].Length; ++allign)
                {
                    deltas[i][allign] = dedy[i][allign] / d2edy2[i][allign];
                }
            });
        }

        private static void CalculateNewConfiguration(float[][] ys, float[][] deltas, float alpha)
        {
            Parallel.For(0, ys.Length, i =>
            {
                for (var j = 0; j < ys[i].Length; ++j)
                {
                    ys[i][j] -= alpha * deltas[i][j];
                }
            });
        }

        #endregion
    }
}