using System;

namespace MultiDimensionalTimeSeriesPrediction
{
    public class PatternIdentifier
    {
        public const double EQUAL_PRECISION = 0.01;

        public const int UNKNOWN = -1;
        public const int BLACK_CANDLE = 0;
        public const int DOJI = 1;
        public const int DOJI_DRAGONFLY = 2;
        public const int DOJI_GRAVESTONE = 3;
        public const int DOJI_LONGSHADOW = 4;
        public const int HAMMER = 5;
        public const int HAMMER_INVERTED = 6;
        public const int LONG_LOWER = 7;
        public const int LONG_UPPER = 8;
        public const int MARUBOZU_BLACK = 9;
        public const int MARUBOZU_WHITE = 10;
        public const int TOP_BLACK = 11;
        public const int TOP_WHITE = 12;
        public const int WHITE_CANDLE = 13;

        private double open;
        private double close;
        private double high;
        private double low;
        private double bodyTop;
        private double bodyBottom;

        public PatternIdentifier(CandleData candleData)
        {
            open = candleData.Open;
            close = candleData.Close;
            high = candleData.High;
            low = candleData.Low;
            bodyTop = Math.Max(open, close);
            bodyBottom = Math.Min(open, close);
        }

        public int DeterminePattern()
        {
            if (IsMarubozuBlack())
            {
                return MARUBOZU_BLACK;
            }
            else if (IsMarubozuWhite())
            {
                return MARUBOZU_WHITE;
            }
            else if (IsLongUpper())
            {
                return LONG_UPPER;
            }
            else if (IsLongLower())
            {
                return LONG_LOWER;
            }
            else if (IsTopWhite())
            {
                return TOP_WHITE;
            }
            else if (IsTopBlack())
            {
                return TOP_BLACK;
            }
            else if (IsWhiteCandle())
            {
                return WHITE_CANDLE;
            }
            else if (IsBlackCandle())
            {
                return BLACK_CANDLE;
            }
            else if (IsDojiLongShadow())
            {
                return DOJI_LONGSHADOW;
            }
            else if (IsDojiGraveStone())
            {
                return DOJI_GRAVESTONE;
            }
            else if (IsDojiDragonfly())
            {
                return DOJI_DRAGONFLY;
            }
            else if (IsHammer())
            {
                return HAMMER;
            }
            else if (IsInvertedHammer())
            {
                return HAMMER_INVERTED;
            }
            return UNKNOWN;
        }

        public bool IsBlackCandle()
        {
            return (HasBody() &&
                    HasUpperShadow() &&
                    HasLowerShadow() &&
                    IsBlack());
        }

        public bool IsWhiteCandle()
        {
            return (HasBody() &&
                    HasUpperShadow() &&
                    HasLowerShadow() &&
                    IsWhite());
        }

        public bool IsDoji()
        {
            return (!HasBody() &&
                    HasUpperShadow() &&
                    HasLowerShadow());
        }

        public bool IsDojiGraveStone()
        {
            return (!HasBody() &&
                    HasUpperShadow() &&
                    !HasLowerShadow());
        }

        public bool IsDojiDragonfly()
        {
            return (!HasBody() &&
                    !HasUpperShadow() &&
                    HasLowerShadow());
        }

        public bool IsDojiLongShadow()
        {
            return (!HasBody() &&
                    HasLongLowerShadow() &&
                    HasLongUpperShadow());
        }

        public bool IsHammer()
        {
            return (IsWhite() &&
                    HasSmallBody() &&
                    !HasUpperShadow() &&
                    HasLowerShadow());
        }

        public bool IsInvertedHammer()
        {
            return (IsBlack() &&
                    HasSmallBody() &&
                    HasUpperShadow() &&
                    !HasLowerShadow());
        }

        public bool IsLongLower()
        {
            return (IsWhite() &&
                    HasLongLowerShadow() &&
                    !HasLongUpperShadow() &&
                    HasBody() &&
                    !HasSmallBody() &&
                    HasUpperShadow() &&
                    HasLowerShadow());
        }

        public bool IsLongUpper()
        {
            return (IsBlack() &&
                    !HasLongLowerShadow() &&
                    HasLongUpperShadow() &&
                    HasBody() &&
                    !HasSmallBody() &&
                    HasUpperShadow() &&
                    HasLowerShadow());
        }

        public bool IsMarubozuWhite()
        {
            return (IsWhite() &&
                    HasBody() &&
                    !HasLowerShadow() &&
                    !HasUpperShadow());
        }

        public bool IsMarubozuBlack()
        {
            return (IsBlack() &&
                    HasBody() &&
                    !HasLowerShadow() &&
                    !HasUpperShadow());
        }

        public bool IsTopBlack()
        {
            return IsBlackCandle() && HasSmallBody();
        }

        public bool IsTopWhite()
        {
            return IsWhiteCandle() && HasSmallBody();
        }


        public bool HasSmallBody()
        {
            return (Math.Abs(open - close) < 1.0);
        }

        public bool HasLowerShadow()
        {
            return ((bodyBottom - low) > EQUAL_PRECISION);
        }

        public bool HasUpperShadow()
        {
            return ((high - bodyTop) > EQUAL_PRECISION);
        }

        public bool HasBody()
        {
            return (Math.Abs(open - close) > EQUAL_PRECISION);
        }

        public bool IsWhite()
        {
            return close > open;
        }

        public bool IsBlack()
        {
            return !IsWhite();
        }

        public double LowerShadowLength
        {
            get
            {
                return high - bodyTop;
            }
        }

        public double UpperShadowLength
        {
            get
            {
                return bodyBottom - low;
            }
        }

        public bool HasLongLowerShadow()
        {
            return (LowerShadowLength > (UpperShadowLength * 2));
        }

        public bool HasLongUpperShadow()
        {
            return (UpperShadowLength > (UpperShadowLength * 2));
        }
    }
}
