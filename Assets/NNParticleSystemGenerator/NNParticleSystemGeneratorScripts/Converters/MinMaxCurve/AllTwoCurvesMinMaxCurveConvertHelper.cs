using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace NNParticleSystemGenerator
{
    public class AllTwoCurvesMinMaxCurveConvertHelper : MinMaxCurveConvertHelper
    {
        public AllTwoCurvesMinMaxCurveConvertHelper(MinMaxCurveConverterSettings settings, MinMaxConvertUtils utils) : base(settings, utils)
        {
        }

        public override void WriteJson(JsonWriter writer, ParticleSystem.MinMaxCurve value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            // writer.WritePropertyName("m_Mode");
            // writer.WriteValue((int)ParticleSystemCurveMode.TwoCurves);

            switch (value.mode)
            {
                case ParticleSystemCurveMode.Constant:

                    // var constantMinCurve = CreateCurve(value.constant);
                    // var constantMaxCurve = CreateCurve(value.constantMax);

                    var constantMinCurve = CreateCurve(1);
                    var constantMaxCurve = CreateCurve(1);

                    WriteTwoCurves(value.constant, constantMinCurve, constantMaxCurve);

                    break;
                case ParticleSystemCurveMode.Curve:

                    WriteTwoCurves(value.curveMultiplier, value.curve, value.curve);

                    break;
                case ParticleSystemCurveMode.TwoCurves:

                    WriteTwoCurves(value.curveMultiplier, value.curveMin, value.curveMax);

                    break;
                case ParticleSystemCurveMode.TwoConstants:

                    var normilizeForCurve = NormilizeForCurve(value.constant, value.constantMax);
                    WriteTwoCurves(normilizeForCurve.curveMultiplier, normilizeForCurve.curveMin,
                        normilizeForCurve.curveMax);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            writer.WriteEndObject();

            void WriteTwoCurves(float multiplier, AnimationCurve curveMin, AnimationCurve curveMax)
            {
                if (Settings.NeedLimitPoints)
                {
                    SmoothCurve(curveMin, Settings.LimitPoints);
                    SmoothCurve(curveMax, Settings.LimitPoints);
                }

                writer.WritePropertyName("curveMultiplier");
                writer.WriteValue(multiplier);

                writer.WritePropertyName("curveMax");
                writer.WriteRawValue(JsonConvert.SerializeObject(curveMax, Formatting.Indented));

                writer.WritePropertyName("curveMin");
                writer.WriteRawValue(JsonConvert.SerializeObject(curveMin, Formatting.Indented));
            }

            AnimationCurve CreateCurve(float constant)
            {
                var constantCurve = new AnimationCurve();
                constantCurve.AddKey(0, constant);
                constantCurve.AddKey(1, constant);
                return constantCurve;
            }

            void SmoothCurve(AnimationCurve originalCurve, int pointsLimit)
            {
                Keyframe[] keyframes = originalCurve.keys;
                int totalPoints = keyframes.Length;

                if (totalPoints == pointsLimit)
                {
                    // Debug.Log("Кривая уже столько же точек, чем лимит.");
                    return;
                }

                // Сортировка ключевых кадров по времени (если необходимо)
                // System.Array.Sort(keyframes, (a, b) => a.time.CompareTo(b.time));

                float startTime = 0;
                float endTime = 1;
                float step = (endTime - startTime) / (pointsLimit - 1);

                AnimationCurve smoothedCurve = new AnimationCurve();

                for (int i = 0; i < pointsLimit; i++)
                {
                    float time = startTime + step * i;
                    float evalValue = originalCurve.Evaluate(time);
                    smoothedCurve.AddKey(new Keyframe(time, evalValue));
                }

                Debug.Log("SmoothCurve " + originalCurve.keys.Length + " -> " + smoothedCurve.keys.Length);
                originalCurve.keys = smoothedCurve.keys;
            }
        }

        private ParticleSystem.MinMaxCurve NormilizeForCurve(
            float constantMin,
            float constantMax)
        {
// Вычисляем разницу между constantMax и constantMin
            float range = constantMax - constantMin;

// Нормализуем значения к диапазону от 0 до 1
            float normalizedMin = (constantMin - constantMin) / range;
            float normalizedMax = (constantMax - constantMin) / range;

// Создаем две кривые AnimationCurve с нормализованными значениями
            AnimationCurve minCurve = AnimationCurve.Constant(0.0f, 1.0f, normalizedMin);
            AnimationCurve maxCurve = AnimationCurve.Constant(0.0f, 1.0f, normalizedMax);

// Создаем объект MinMaxCurve с кривыми и множителем (равным range)
            ParticleSystem.MinMaxCurve minMaxCurve = new ParticleSystem.MinMaxCurve(range, minCurve, maxCurve);

            return minMaxCurve;
// Теперь у вас есть объект MinMaxCurve с правильными нормализованными значениями и множителем, равным разнице между constantMax и constantMin.
        }

        public override ParticleSystem.MinMaxCurve ReadJson(JsonReader reader, Type objectType,
            ParticleSystem.MinMaxCurve existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            string json = jo.ToString();
            var twoCurves = Utils.GetTwoCurves(json);

            return twoCurves;
        }
    }
}