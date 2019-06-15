using System;
using System.Diagnostics;
using System.Linq;

namespace Diploma.Core.Helpers
{
    public static class JaggedArray
    {
        public static void Copy<T>(T[][] sourceArray, T[][] destinationArray, int length)
        {
            Debug.Assert(sourceArray.Length == destinationArray.Length);

            for (var i = 0; i < length; ++i)
            {
                Array.Copy(sourceArray[i], destinationArray[i], sourceArray[i].Length);
            }
        }

        public static T[][] InitAndCopy<T>(T[][] sourceArray, int length)
        {
            var result = new T[length][];
            for (var i = 0; i < length; ++i)
            {
                result[i] = new T[sourceArray[i].Length];
                Array.Copy(sourceArray[i], result[i], sourceArray[i].Length);
            }

            return result;
        }

        public static T[][] Init<T>(int rows, int cols)
        {
            var result = new T[rows][];
            for (var i = 0; i < rows; ++i)
            {
                result[i] = new T[cols];
            }

            return result;
        }

        public static T[] GetColumn<T>(T[][] arr, int col)
        {
            // @Refactor: Very slow
            return arr.Select(a => a[col]).ToArray();
        }
    }
}
