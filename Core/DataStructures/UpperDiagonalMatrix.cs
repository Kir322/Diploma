using System;
using System.Diagnostics;

namespace Diploma.Core.DataStructures
{
    public struct UpperDiagonalMatrix
    {
        public float[] Data;
        public int Dimensions;

        public UpperDiagonalMatrix(int n)
        {
            this.Data = new float[n * (n + 1) / 2];
            this.Dimensions = n;
        }

        public float this[int i, int j]
        {
            get
            {
                Debug.Assert(i * this.Dimensions + j < this.Data.Length);
                return this.Data[i * this.Dimensions + j];
            }
            set
            {
                Debug.Assert(i * this.Dimensions + j < this.Data.Length);
                this.Data[i * this.Dimensions + j] = value;
            }
        }
    }
}