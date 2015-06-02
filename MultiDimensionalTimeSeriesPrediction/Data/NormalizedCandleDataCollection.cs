using System.Collections.Generic;
using System.Linq;
using Encog.Util.Arrayutil;
using MultiDimensionalTimeSeriesPrediction.Utils;

namespace MultiDimensionalTimeSeriesPrediction.Data
{
    public class NormalizedCandleDataCollection : List<NormalizedCandleData>
    {
        private readonly NormalizeArray _closePriceNormalizeArray;

        public NormalizedCandleDataCollection(NormalizeArray closePriceNormalizeArray)
        {
            _closePriceNormalizeArray = closePriceNormalizeArray;
        }

        public static NormalizedCandleDataCollection Create(IList<CandleData> candleData)
        {
            var normalizePatternArray = new NormalizeArray(-1, 1);
            var patterns = candleData.Select(c => c.GetPatternType()).ToArray();
            var normalizedPatterns = normalizePatternArray.Process(patterns);

            var normalizeCloseChangeArray = new NormalizeArray(-1, 1);
            var closeChanges = new List<double>();
            for (var index = 0; index < candleData.Count; index++)
            {
                if (index == 0)
                {
                    closeChanges.Add(0);
                }
                else
                {
                    var currentInstance = candleData[index];
                    var previousInstance = candleData[index - 1];
                    var percentChange = MathUtils.PercentChange(previousInstance.Close, currentInstance.Close);
                    closeChanges.Add(percentChange);
                }
            }
            var normalizedCloseChanges = normalizeCloseChangeArray.Process(closeChanges.ToArray());

            var normalizeVolumeChangeArray = new NormalizeArray(-1, 1);
            var volumeChanges = new List<double>();
            for (var index = 0; index < candleData.Count; index++)
            {
                if (index == 0)
                {
                    volumeChanges.Add(0);
                }
                else
                {
                    var currentInstance = candleData[index];
                    var previousInstance = candleData[index - 1];
                    var percentChange = MathUtils.PercentChange(previousInstance.Volume, currentInstance.Volume);
                    volumeChanges.Add(percentChange);
                }
            }
            var normalizedVolumeChanges = normalizeVolumeChangeArray.Process(volumeChanges.ToArray());


            var result = new NormalizedCandleDataCollection(normalizeCloseChangeArray);

            for (var index = 1; index < candleData.Count; index++)
            {
                result.Add(new NormalizedCandleData
                {
                    EncodedCloseChange = normalizedCloseChanges[index],
                    EncodedPattern = normalizedPatterns[index],
                    EncodedVolumeChange = normalizedVolumeChanges[index]
                });
            }

            return result;
        }

        public double DeNormalize(double normalizedCloseChange)
        {
            return _closePriceNormalizeArray.Stats.DeNormalize(normalizedCloseChange);
        }
    }
}