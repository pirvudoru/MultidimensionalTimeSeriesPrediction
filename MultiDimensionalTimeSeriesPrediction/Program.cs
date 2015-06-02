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
using Encog.Neural.Networks;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Neural.Pattern;
using Encog.Util.Arrayutil;
using Encog.Util.Simple;

namespace MultiDimensionalTimeSeriesPrediction
{
    class Program
    {
        private const int PastWindowSize = 12;
        private const int FutureWindowSize = 1;
        private const double MaxError = 0.0002;

        private static NormalizeArray _normalizeClosePrices;

        static void Main(string[] args)
        {
            var candleData = ReadCandleData().Take(100).ToList();
            var normalizedCandleData = NormalizeCandleData(candleData);
            var network = BuildCandlePatternRecognitionNetwork(normalizedCandleData);
            Evaluate(network, normalizedCandleData);
        }

        private static void Evaluate(BasicNetwork network, IList<NormalizedCandleData> candleData)
        {
            using (var file = new StreamWriter("Result.txt"))
            {
                for (var index = 0; index < candleData.Count - PastWindowSize; index++)
                {
                    var windowData = candleData.Skip(index).Take(PastWindowSize).ToList();
                    var input = new BasicMLData(PastWindowSize);
                    for (var i = 0; i < windowData.Count; i++)
                    {
                        input[i] = windowData[i].Close;
                    }

                    var output = network.Compute(input);
                    var actualData = new double[] { _normalizeClosePrices.Stats.DeNormalize(candleData[index + PastWindowSize].Close) };
                    var predictedData = new double[] { _normalizeClosePrices.Stats.DeNormalize(output[0]) };

                    file.WriteLine(string.Format("{0},{1}", actualData[0], Math.Round(predictedData[0], 5)));
                }
            }
        }

        private static BasicNetwork BuildCandlePatternRecognitionNetwork(IList<NormalizedCandleData> normalizedData)
        {
            var pattern = new FeedForwardPattern
            {
                ActivationFunction = new ActivationTANH(),
                InputNeurons = PastWindowSize,
                OutputNeurons = FutureWindowSize
            };

            pattern.AddHiddenLayer(PastWindowSize * FutureWindowSize * 10);

            var network = (BasicNetwork)pattern.Generate();
            Train(network, GenerateTrainingSet(normalizedData));

            return network;
        }

        private static IList<NormalizedCandleData> NormalizeCandleData(IList<CandleData> candleData)
        {
            _normalizeClosePrices = new NormalizeArray(-1, 1);
            var normalizedClosePrices = _normalizeClosePrices.Process(candleData.Select(c => c.Close).ToArray());

            return normalizedClosePrices.Select(p => new NormalizedCandleData { Close = p }).ToList();
        }

        private static IList<CandleData> ReadCandleData()
        {
            var fileReader = new StreamReader(File.OpenRead("WorkingDays_EUR_USD_Candlestick_5_min_data.csv"));
            var csv = new CsvReader(fileReader);
            csv.Configuration.RegisterClassMap<CandleDataCsvMap>();

            return csv.GetRecords<CandleData>().ToList();
        }

        public static IMLDataSet GenerateTrainingSet(IList<NormalizedCandleData> candleData)
        {
            var result = new TemporalMLDataSet(PastWindowSize, FutureWindowSize);
            result.AddDescription(new TemporalDataDescription(TemporalDataDescription.Type.Raw, true, true));

            for (var index = 0; index < candleData.Count(); index++)
            {
                var point = new TemporalPoint(1) { Sequence = index };
                point.Data[0] = candleData[index].Close;

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
