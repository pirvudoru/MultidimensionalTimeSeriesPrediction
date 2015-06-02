using System;

namespace MultiDimensionalTimeSeriesPrediction.Utils
{
    public static class MathUtils
    {
        public static double PercentChange(double oldValue, double newValue)
        {
            return Math.Round((newValue - oldValue) / Math.Abs(oldValue) * 100, 5);
        }
    }
}
