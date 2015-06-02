namespace MultiDimensionalTimeSeriesPrediction.Data
{
    public class CandleData
    {
        public string Date { get; set; }

        public double Open { get; set; }

        public double High { get; set; }

        public double Low { get; set; }
        
        public double Close { get; set; }

        public double Volume { get; set; }

        public double GetPatternType()
        {
            return new PatternIdentifier(this).DeterminePattern();
        }
    }
}
