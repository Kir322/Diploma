using System;
using System.Diagnostics;
using Diploma.Core.Data;
using Diploma.Core.DataRepresentation;
using Diploma.Core.Clustering.KMeans;
using Diploma.Core.Helpers;

using System.Numerics;

namespace Diploma.ConsoleApp
{
    class Program
    {
        static void Main()
        {
            var frame = DataFrame.ReadFromCsv("data.csv");
            frame.Normalize();

            var watch = new Stopwatch();

            // watch.Start();
            // var c = KMeansClustering.Clusterize((float[][])frame, 10, 0, 10000);
            // Console.WriteLine($"Done in {watch.ElapsedMilliseconds}");

            watch.Restart();
            SammonsMapping.Map3D((float[][])frame);
            //Console.Write(string.Join(',', c));
            Console.WriteLine($"Done in {watch.ElapsedMilliseconds}");
            //Console.Read();
        }
    }
}