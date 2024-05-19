using System;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;

namespace NNParticleSystemGenerator
{
    public class ConstantMinMaxCurveConvertHelper : MinMaxCurveConvertHelper
    {
        public ConstantMinMaxCurveConvertHelper(MinMaxCurveConverterSettings settings, MinMaxConvertUtils utils) : base(
            settings, utils)
        {
        }

        public override void WriteJson(JsonWriter writer, ParticleSystem.MinMaxCurve value, JsonSerializer serializer)
        {
            switch (value.mode)
            {
                case ParticleSystemCurveMode.Constant:
                    serializer.Serialize(writer, value.constant);
                    break;
                case ParticleSystemCurveMode.Curve:

                    serializer.Serialize(writer,
                        value.curveMultiplier * MinMaxConvertUtils.GetConstantFromCurve(value.curve));
                    break;
                case ParticleSystemCurveMode.TwoCurves:
                    var minCurve = value.curveMultiplier * MinMaxConvertUtils.GetConstantFromCurve(value.curve);
                    var maxCurve = value.curveMultiplier * MinMaxConvertUtils.GetConstantFromCurve(value.curveMax);
                    serializer.Serialize(writer, (minCurve + maxCurve) / 2);
                    break;
                case ParticleSystemCurveMode.TwoConstants:
                    var min = value.constantMin;
                    var max = value.constantMax;
                    serializer.Serialize(writer, (min + max) / 2);

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        public override ParticleSystem.MinMaxCurve ReadJson(JsonReader reader, Type objectType,
            ParticleSystem.MinMaxCurve existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var constant = serializer.Deserialize<float>(reader);
            if (hasExistingValue)
            {
                existingValue.mode = ParticleSystemCurveMode.Constant;
                existingValue.constant = constant;
                return existingValue;
            }
            else
            {
                var newValue = new ParticleSystem.MinMaxCurve(constant);
                return newValue;
            }
        }
    }
}