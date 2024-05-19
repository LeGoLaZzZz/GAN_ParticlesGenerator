using System;
using Newtonsoft.Json;
using UnityEngine;

namespace NNParticleSystemGenerator
{
    public class AllToMinMaxConstantMinMaxCurveConvertHelper : MinMaxCurveConvertHelper
    {
        public class MinMaxConstant
        {
            public float minConstant;
            public float maxConstant;
        }

        public AllToMinMaxConstantMinMaxCurveConvertHelper(MinMaxCurveConverterSettings settings,
            MinMaxConvertUtils utils) : base(settings, utils)
        {
        }

        public override void WriteJson(JsonWriter writer, ParticleSystem.MinMaxCurve value, JsonSerializer serializer)
        {
            var minConstant = 0f;
            var maxConstant = 0f;

            switch (value.mode)
            {
                case ParticleSystemCurveMode.Constant:
                    minConstant = value.constant;
                    maxConstant = value.constant;
                    break;
                case ParticleSystemCurveMode.Curve:

                    MinMaxConvertUtils.GetMinMaxFromCurve(value.curve, out minConstant, out maxConstant);
                    maxConstant *= value.curveMultiplier;
                    minConstant *= value.curveMultiplier;

                    break;
                case ParticleSystemCurveMode.TwoCurves:
                    minConstant = value.curveMultiplier * MinMaxConvertUtils.GetConstantFromCurve(value.curve);
                    maxConstant = value.curveMultiplier * MinMaxConvertUtils.GetConstantFromCurve(value.curveMax);

                    break;
                case ParticleSystemCurveMode.TwoConstants:
                    minConstant = value.constantMin;
                    maxConstant = value.constantMax;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var minMaxConstant = new MinMaxConstant();
            minMaxConstant.minConstant = minConstant;
            minMaxConstant.maxConstant = maxConstant;
            serializer.Serialize(writer, minMaxConstant);
        }

        public override ParticleSystem.MinMaxCurve ReadJson(JsonReader reader, Type objectType,
            ParticleSystem.MinMaxCurve existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var minMaxConstant = serializer.Deserialize<MinMaxConstant>(reader);
            return new ParticleSystem.MinMaxCurve(minMaxConstant.minConstant, minMaxConstant.maxConstant);
        }
    }
}