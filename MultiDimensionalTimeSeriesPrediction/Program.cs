using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using Encog.Engine.Network.Activation;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Data.Temporal;
using Encog.ML.Train;
using Encog.ML.Train.Strategy;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Anneal;
using Encog.Neural.Networks.Training.Lma;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Neural.Pattern;
using Encog.Util.Arrayutil;
using Encog.Util.Banchmark;
using Encog.Util.Simple;

namespace MultiDimensionalTimeSeriesPrediction
{
    class Program
    {
        private const int PastWindowSize = 12;
        private const int FutureWindowSize = 1;
        private const double MaxError = 0.01;

        private 

        static void Main(string[] args)
        {
            var candleData = ReadCandleData().Take(1000).ToList();
            var network = BuildCandlePatternRecognitionNetwork(candleData);
            Evaluate(network, candleData);
        }

        private static void Evaluate(BasicNetwork network, IList<CandleData> candleData)
        {
            using (var file = new StreamWriter("Result.txt"))
            {
                for (var index = PastWindowSize; index < candleData.Count - FutureWindowSize; index++)
                {
                    var input = new BasicMLData(PastWindowSize);
                    for (var i = 0; i < input.Count; i++)
                    {
                        input[i] = candleData.Skip(index + i).Select(m => m.Close).First();
                    }

                    var output = network.Compute(input);
                    var actualData = new double[] { candleData[index + FutureWindowSize].Close };
                    var predictedData = new double[] { _normalizedCloses.Stats.DeNormalize(output[0]) };
                    //var predictedData = new double[] { output[0], output[1], output[2] };

                    file.WriteLine(string.Format("{0},{1}", actualData[0], predictedData[0]));
                }
            }
        } 

        private static BasicNetwork BuildCandlePatternRecognitionNetwork(IList<CandleData> candleData)
        {
            var pattern = new FeedForwardPattern
            {
                ActivationFunction = new ActivationTANH(),
                InputNeurons = PastWindowSize,
                OutputNeurons = FutureWindowSize
            };

            pattern.AddHiddenLayer(PastWindowSize * FutureWindowSize * 10);

            var network = (BasicNetwork)pattern.Generate();

            Train(network, GenerateTrainingSet(candleData));

            return network;
        }

        private static IList<CandleData> ReadCandleData()
        {
            var fileReader = new StreamReader(File.OpenRead("WorkingDays_EUR_USD_Candlestick_5_min_data.csv"));
            var csv = new CsvReader(fileReader);
            csv.Configuration.RegisterClassMap<CandleDataCsvMap>();

            return csv.GetRecords<CandleData>().ToList();
        }

        private static NormalizeArray _normalizedCloses;

        public static IMLDataSet GenerateTrainingSet(IList<CandleData> candleData)
        {
            var result = new TemporalMLDataSet(PastWindowSize, FutureWindowSize);
            result.AddDescription(new TemporalDataDescription(TemporalDataDescription.Type.Raw, true, false));
            result.AddDescription(new TemporalDataDescription(TemporalDataDescription.Type.Raw, true, true));
            result.AddDescription(new TemporalDataDescription(TemporalDataDescription.Type.Raw, true, false));

            var normalizePatternArray = new NormalizeArray(-1, 1);
            var normalizedCandleTypes = normalizePatternArray.Process(candleData.Select(c => c.GetPatternType()).ToArray());

            var normalizeVolumeArray = new NormalizeArray(-1, 1);
            var normalizedVolumes = normalizeVolumeArray.Process(candleData.Select(c => c.Volume).ToArray());

            _normalizedCloses = new NormalizeArray(-1, 1);
            var normalizedCloses = _normalizedCloses.Process(candleData.Select(c => c.Close).ToArray());

            for (var index = 0; index < candleData.Count(); index++)
            {
                var point = new TemporalPoint(3) { Sequence = index };
                point.Data[0] = normalizedCandleTypes[index];
                point.Data[1] = normalizedCloses[index];
                point.Data[2] = normalizedVolumes[index];

                result.Points.Add(point);
            }

            result.Generate();

            return result;
        }

        public static void Train(BasicNetwork network, IMLDataSet training)
        {
            IMLTrain trainMain = new ResilientPropagation(network, training);
            EncogUtility.TrainToError(trainMain, MaxError);
        }
    }
}
