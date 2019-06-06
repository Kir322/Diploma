using System;

namespace Diploma.Core.Interfaces
{
    public interface IDistanceCalculator
    {
        float Calculate(ReadOnlySpan<float> left, ReadOnlySpan<float> right);
    }
}