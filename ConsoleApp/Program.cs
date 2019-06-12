using System;
using Diploma.Core.Clustering.KMeans;
using Diploma.Core.Data;

namespace Diploma.ConsoleApp
{
    class Program
    {
        static void Main()
        {
            var frame = DataFrame.ReadFromCsv("data.csv");
            frame.Normalize();
            var c = KMeansClustering.Clusterize((float[][])frame, 3);

            Console.Write(string.Join(',', c));
            Console.Read();
        }
    }
}
