namespace NNParticleSystemGenerator
{
    public class MinMaxCurveConverterSettings
    {
        public MinMaxCurveConvertMode Mode;
        public bool NeedLimitPoints;
        public int LimitPoints;

        public MinMaxCurveConverterSettings(MinMaxCurveConvertMode mode, int limitPoints)
        {
            Mode = mode;
            LimitPoints = limitPoints;
            NeedLimitPoints = true;
        }

        public MinMaxCurveConverterSettings(MinMaxCurveConvertMode mode)
        {
            Mode = mode;
            NeedLimitPoints = false;
        }


        public static MinMaxCurveConverterSettings Create(MinMaxCurveConvertMode mode, bool needLimitPoints,
            int limitPoints)
        {
            if (needLimitPoints)
            {
                return new MinMaxCurveConverterSettings(mode, limitPoints);
            }
            else
            {
                return new MinMaxCurveConverterSettings(mode);
            }
        }
    }
}