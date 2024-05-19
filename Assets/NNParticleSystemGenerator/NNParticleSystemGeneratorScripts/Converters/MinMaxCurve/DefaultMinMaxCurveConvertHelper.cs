using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace NNParticleSystemGenerator
{
    public class DefaultMinMaxCurveConvertHelper : MinMaxCurveConvertHelper
    {
        public DefaultMinMaxCurveConvertHelper(MinMaxCurveConverterSettings settings, MinMaxConvertUtils utils) : base(settings, utils)
        {
        }

        public override void WriteJson(JsonWriter writer, ParticleSystem.MinMaxCurve value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("m_Mode");
            writer.WriteValue((int)value.mode);

            switch (value.mode)
            {
                case ParticleSystemCurveMode.Constant:

                    writer.WritePropertyName("constant");
                    writer.WriteValue(value.constant);

                    break;
                case ParticleSystemCurveMode.Curve:

                    writer.WritePropertyName("curveMultiplier");
                    writer.WriteValue(value.curveMultiplier);

                    writer.WritePropertyName("curveMax");
                    writer.WriteRawValue(JsonConvert.SerializeObject(value.curveMax, Formatting.Indented));

                    break;
                case ParticleSystemCurveMode.TwoCurves:


                    writer.WritePropertyName("curveMultiplier");
                    writer.WriteValue(value.curveMultiplier);

                    writer.WritePropertyName("curveMax");
                    writer.WriteRawValue(JsonConvert.SerializeObject(value.curveMax, Formatting.Indented));

                    writer.WritePropertyName("curveMin");
                    writer.WriteRawValue(JsonConvert.SerializeObject(value.curveMin, Formatting.Indented));

                    break;
                case ParticleSystemCurveMode.TwoConstants:


                    writer.WritePropertyName("constantMin");
                    writer.WriteValue(value.constantMin);

                    writer.WritePropertyName("constantMax");
                    writer.WriteValue(value.constantMax);

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            writer.WriteEndObject();
        }

        public override ParticleSystem.MinMaxCurve ReadJson(JsonReader reader, Type objectType,
            ParticleSystem.MinMaxCurve existingValue, bool hasExistingValue,
            JsonSerializer serializer)

        {
            JObject jo = JObject.Load(reader);
            string json = jo.ToString();

            ParticleSystem.MinMaxCurve returnCurve = Utils.GetCurve(json);

            if (hasExistingValue)
            {
                existingValue.mode = returnCurve.mode;
                existingValue.curve = returnCurve.curve;
                existingValue.curveMultiplier = returnCurve.curveMultiplier;
                existingValue.constant = returnCurve.constant;
                existingValue.curveMax = returnCurve.curveMax;
                existingValue.curveMin = returnCurve.curveMin;
                existingValue.constantMax = returnCurve.constantMax;
                existingValue.constantMin = returnCurve.constantMin;
            }

            return existingValue;
        }
    }
}