using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using MultiDimensionalTimeSeriesPrediction.Data;

namespace MultiDimensionalTimeSeriesPrediction.Utils
{
    public class CandleDataReader
    {
        public static IList<CandleData> Read(string path)
        {
            var fileReader = new StreamReader(File.OpenRead(path));
            var csv = new CsvReader(fileReader);
            csv.Configuration.RegisterClassMap<CandleDataCsvMap>();

            return csv.GetRecords<CandleData>().ToList();
        } 
    }
}
