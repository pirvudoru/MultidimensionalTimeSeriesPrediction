using System.Linq;
using FluentAssertions;
using MultiDimensionalTimeSeriesPrediction.Data;
using MultiDimensionalTimeSeriesPrediction.Utils;
using NUnit.Framework;

namespace MultiDimensionalTimeSeriesPredictionTests.Data
{
    [TestFixture]
    public class NormalizedCandleDataCollectionTests
    {
        [Test]
        public void Create_Always_NormalizesCloseChange()
        {
            var items = CandleDataReader.Read("Support/EurUsdSample.csv");
            var collection = NormalizedCandleDataCollection.Create(items);

            for (var index = 0; index < collection.Count; index++)
            {
                Assert.LessOrEqual(collection[index].EncodedCloseChange, 1);
                Assert.GreaterOrEqual(collection[index].EncodedCloseChange, -1);
            }
        }

        [Test]
        public void Create_Always_NormalizesVolumes()
        {
            var items = CandleDataReader.Read("Support/EurUsdSample.csv");
            var collection = NormalizedCandleDataCollection.Create(items);

            for (var index = 0; index < collection.Count; index++)
            {
                Assert.LessOrEqual(collection[index].EncodedVolumeChange, 1);
                Assert.GreaterOrEqual(collection[index].EncodedVolumeChange, -1);
            }
        }

        [Test]
        public void Create_Always_NormalizesPattern()
        {
            var items = CandleDataReader.Read("Support/EurUsdSample.csv");
            var collection = NormalizedCandleDataCollection.Create(items);

            for (var index = 0; index < collection.Count; index++)
            {
                Assert.LessOrEqual(collection[index].EncodedPattern, 1);
                Assert.GreaterOrEqual(collection[index].EncodedPattern, -1);
            }
        }
    }
}
