using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Diploma.Core.Data
{
    public class DataFrame
    {
        private float[][] data;

        public float[] Min { get; private set; }
        public float[] Max { get; private set; }
        public string[] ColumnNames { get; private set; }

        public float[] this[int i]
        {
            get { return this.data[i]; }
            set { this.data[i] = value; }
        }

        public float[] this[string name]
        {
            get
            {
                int index = Array.FindIndex(this.ColumnNames, s => s == name);
                if (index == -1)
                    throw new ArgumentException(nameof(name), $"DataFrame does not contains that column name: {name}");

                return this.data[index];
            }
        }

        public float this[int i, int j]
        {
            get { return this.data[i][j]; }
            set { this.data[i][j] = value; }
        }

        public static explicit operator float[][](DataFrame frame)
        {
            return frame.data;
        }

        public int NumRecords => this.data.Length;
        public int NumFeatures => this.data[0].Length;

        public void Normalize(int a = 0, int b = 1)
        {
            this.NormalizeImpl(a, b);
        }

        public DataTable ToDataTable()
        {
            var dt = new DataTable();

            if (this.ColumnNames == null)
                for (int i = 0; i < this.data[0].Length; i++)
                {
                    dt.Columns.Add(new DataColumn($"Feature {i}"));
                }
            else
                for (int i = 0; i < this.data[0].Length; i++)
                {
                    dt.Columns.Add(new DataColumn(this.ColumnNames[i]));
                }

            for (int i = 0; i < this.data.Length; i++)
            {
                var row = dt.NewRow();
                for (int j = 0; j < this.data[0].Length; j++)
                {
                    row[j] = this.data[i][j];
                }
                dt.Rows.Add(row);
            }

            return dt;
        }

        public static DataFrame ReadFromExcel(string filepath)
        {
            return new DataFrame();
        }

        public static DataFrame ReadFromCsv(string filepath, bool header = false)
        {
            if (!File.Exists(filepath))
                throw new ArgumentException(nameof(filepath), $"File does not exist: {filepath}");

            var lines = File.ReadAllLines(filepath);
            if (lines.Length == 0)
                throw new ArgumentException(nameof(filepath), $"File is empty: {filepath}");

            var headerLine = lines[0].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                     .Select(h => h.Trim())
                                     .ToArray();

            var rows = lines.Length;
            var cols = headerLine.Length;

            var result = new DataFrame();

            if (header)
            {
                result.ColumnNames = headerLine;
                lines = lines.Skip(1).ToArray();
                rows--;
            }

            result.InitArrays(rows, cols);

            for (int i = 0; i < rows; i++)
            {
                var features = lines[i].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                       .Select(f => f.Trim())
                                       .ToArray();
                for (int j = 0; j < cols; j++)
                {
                    var feature = float.Parse(features[j], CultureInfo.InvariantCulture);
                    result.data[i][j] = feature;
                    if (result.Min[j] > feature)
                    {
                        result.Min[j] = feature;
                    }

                    if (result.Max[j] < feature)
                    {
                        result.Max[j] = feature;
                    }
                }
            }

            return result;
        }

        private void InitArrays(int rows, int cols)
        {
            this.data = new float[rows][];
            for (int i = 0; i < rows; i++)
            {
                this.data[i] = new float[cols];
            }

            this.Min = Enumerable.Repeat(float.MaxValue, cols).ToArray();
            this.Max = Enumerable.Repeat(float.MinValue, cols).ToArray();
        }

        private void NormalizeImpl(int a, int b)
        {
            a = Math.Abs(a); b = Math.Abs(b);
            for (int row = 0; row < this.data.Length; row++)
            {
                // @TODO: use SIMD
                for (int col = 0; col < this.data[row].Length; col++)
                {
                    var value = (a + b) * (this.data[row][col] - this.Min[col]) /
                                          (this.Max[col] - this.Min[col]) - a;
                    this.data[row][col] = value;
                    if (this.Min[col] > value)
                    {
                        this.Min[col] = value;
                    }

                    if (this.Max[col] < value)
                    {
                        this.Max[col] = value;
                    }
                }
            }
        }
    }
}
