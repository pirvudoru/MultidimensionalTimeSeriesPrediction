using MultiDimensionalTimeSeriesPrediction.Utils;
using NUnit.Framework;

namespace MultiDimensionalTimeSeriesPredictionTests.Utils
{
    [TestFixture]
    public class MathUtilsTests
    {
        [Test]
        [TestCase(1.20000, 1.32000, 10.0)]
        [TestCase(1.20000, 1.20012, 0.01)]
        [TestCase(1.10000, 1.100011, 0.001)]
        public void PercentChange_Always_ReturnsCorrectValue(double oldValue, double newValue, double result)
        {
            Assert.AreEqual(result, MathUtils.PercentChange(oldValue, newValue));
        }
    }
}
