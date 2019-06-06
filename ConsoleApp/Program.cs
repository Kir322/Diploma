using System;
using System.Text;
using Diploma.Core.Clustering;
using Diploma.Core.Data;

namespace Diploma.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var frame = DataFrame.ReadFromCsv("data.csv");
            frame.Normalize();
            var c = KMeansClustering.Clusterize(frame, 3);

            var builder = new StringBuilder();
            for (int i = 0; i < c.Length; ++i)
            {
                builder.Append(string.Join(',', c[i]));
                builder.Append('\n');
            }

            Console.Write(builder.ToString());
        }
    }
}
