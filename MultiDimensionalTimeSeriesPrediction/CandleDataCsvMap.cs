using CsvHelper.Configuration;
using MultiDimensionalTimeSeriesPrediction.Data;

namespace MultiDimensionalTimeSeriesPrediction
{
    public sealed class CandleDataCsvMap : CsvClassMap<CandleData>
    {
        public CandleDataCsvMap()
        {
            Map(m => m.Date).Index(0);
            Map(m => m.Open).Index(1);
            Map(m => m.High).Index(2);
            Map(m => m.Low).Index(3);
            Map(m => m.Close).Index(4);
            Map(m => m.Volume).Index(5);
        }
    }
}
