using System;

namespace Diploma.Core.Interfaces.KMeans
{
    public interface ICentroidInitializer
    {
        Span<float> Initialize(ReadOnlySpan<float> records, int d, int k);
    }
}