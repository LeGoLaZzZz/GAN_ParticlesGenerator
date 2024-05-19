using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace NNParticleSystemGenerator
{
    public enum MinMaxGradientConvertMode
    {
        AllToColor,
        AllToGradient
    }

    public class MinMaxGradientSettings
    {
        public MinMaxGradientConvertMode Mode;
        public int LimitPoints;
        public bool NeedLimitPoints;

        public MinMaxGradientSettings(MinMaxGradientConvertMode mode, int limitPoints)
        {
            Mode = mode;
            LimitPoints = limitPoints;
            NeedLimitPoints = true;
        }

        public MinMaxGradientSettings(MinMaxGradientConvertMode mode)
        {
            NeedLimitPoints = false;
            Mode = mode;
        }


        public static MinMaxGradientSettings Create(MinMaxGradientConvertMode mode, bool needLimitPoints,
            int limitPoints)
        {
            if (needLimitPoints)
            {
                return new MinMaxGradientSettings(mode, limitPoints);
            }
            else
            {
                return new MinMaxGradientSettings(mode);
            }
        }
    }

    public class MinMaxGradientConverter : JsonConverter<ParticleSystem.MinMaxGradient>
    {
        private const string ColorKeys = @"colorKeys.:(?'colorKeys' \[[\d\D-[]]]*])";
        private const string AlphaKeys = @"alphaKeys.:(?'alphaKeys' \[[\d\D-[]]]*])";

        private MinMaxGradientSettings _settings;

        public MinMaxGradientConverter(MinMaxGradientSettings settings)
        {
            _settings = settings;
        }

        public void WriteJsonToOneColor(JsonWriter writer, ParticleSystem.MinMaxGradient value,
            JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.Evaluate(0.5f));
        }
        public ParticleSystem.MinMaxGradient ReadJsonToOneColor(JsonReader reader, Type objectType,
            ParticleSystem.MinMaxGradient existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var color = (Color) serializer.Deserialize(reader,typeof(Color));
            var minmaxGradient = new ParticleSystem.MinMaxGradient(color);

            if (hasExistingValue)
            {
                existingValue.mode = minmaxGradient.mode;
                existingValue.color = minmaxGradient.color;
                existingValue.gradient = minmaxGradient.gradient;
                existingValue.colorMax = minmaxGradient.colorMax;
                existingValue.colorMin = minmaxGradient.colorMin;
                existingValue.gradientMax = minmaxGradient.gradientMax;
                existingValue.gradientMin = minmaxGradient.gradientMin;
                return existingValue;
            }

            return minmaxGradient;
        }
        
        
        
        
        public override void WriteJson(JsonWriter writer, ParticleSystem.MinMaxGradient value,
            JsonSerializer serializer)
        {
            
            if(_settings.Mode == MinMaxGradientConvertMode.AllToColor)
            {
                WriteJsonToOneColor(writer, value, serializer);
                return;
            }
            
            writer.WriteStartObject();

            switch (value.mode)
            {
                case ParticleSystemGradientMode.Color:

                    var gradientColor = CreateGradient(value.color);
                    WriteGradient(gradientColor);
                    break;
                case ParticleSystemGradientMode.Gradient:


                    WriteGradient(value.gradient);
                    break;
                case ParticleSystemGradientMode.TwoColors:

                    var gradientTwoColors = CreateGradientTwoColors(value.colorMin, value.colorMax);
                    WriteGradient(gradientTwoColors);

                    break;
                case ParticleSystemGradientMode.TwoGradients:

                    var gradientFromTwo = CreateGradientFromTwo(value.gradientMin, value.gradientMax);
                    WriteGradient(gradientFromTwo);

                    break;
                case ParticleSystemGradientMode.RandomColor:

                    var gradientRandomColor = CreateGradient(value.Evaluate(1));
                    WriteGradient(gradientRandomColor);

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            writer.WriteEndObject();


            Gradient CreateGradient(Color color)
            {
                int points = 1;
                if (_settings.NeedLimitPoints)
                    points = _settings.LimitPoints;

                var gradient = new Gradient();
                var colorKeys = new GradientColorKey[points];
                var alphaKeys = new GradientAlphaKey[points];

                for (var i = 0; i < points; i++)
                {
                    colorKeys[i] = new GradientColorKey(color, 0);
                    alphaKeys[i] = new GradientAlphaKey(color.a, 0);
                }

                gradient.SetKeys(colorKeys, alphaKeys);
                
                if (_settings.NeedLimitPoints)
                    EvaluateGradientLimitPoints(gradient, _settings.LimitPoints);

                return gradient;
            }

            Gradient CreateGradientTwoColors(Color color1, Color color2)
            {
                int points = 2;
                if (_settings.NeedLimitPoints)
                    points = _settings.LimitPoints;

                var initialGradient = new Gradient();
                var twoColorKeys = new GradientColorKey[2];
                var twoAlphaKeys = new GradientAlphaKey[2];

                twoColorKeys[0] = new GradientColorKey(color1, 0);
                twoAlphaKeys[0] = new GradientAlphaKey(color1.a, 0);

                twoColorKeys[1] = new GradientColorKey(color2, 1);
                twoAlphaKeys[1] = new GradientAlphaKey(color2.a, 1);

                initialGradient.SetKeys(twoColorKeys, twoAlphaKeys);

                var returnGradient = new Gradient();
                var colorKeys = new GradientColorKey[points];
                var alphaKeys = new GradientAlphaKey[points];

                for (var i = 0; i < points; i++)
                {
                    float time = (float)i / (points - 1);
                    var color = initialGradient.Evaluate(time);
                    colorKeys[i] = new GradientColorKey(color, time);
                    alphaKeys[i] = new GradientAlphaKey(color.a, time);
                }

                returnGradient.SetKeys(colorKeys, alphaKeys);
                
                if (_settings.NeedLimitPoints)
                    EvaluateGradientLimitPoints(returnGradient, _settings.LimitPoints);

                return returnGradient;
            }


            Gradient CreateGradientFromTwo(Gradient gradient1, Gradient gradient2)
            {
                int initialPoints = gradient1.alphaKeys.Length;

                var returnGradient = new Gradient();
                var colorKeys = new GradientColorKey[initialPoints];
                var alphaKeys = new GradientAlphaKey[initialPoints];


                for (int i = 0; i < initialPoints; i++)
                {
                    float time = (float)i / (initialPoints - 1);
                    var color1 = gradient1.Evaluate(time);
                    var color2 = gradient2.Evaluate(time);
                    var newColor = (color1 + color2) / 2;
                    var newAlpha = (color1.a + color2.a) / 2;

                    colorKeys[i] = new GradientColorKey(newColor, time);
                    alphaKeys[i] = new GradientAlphaKey(Mathf.Clamp01(newAlpha), time);
                }

                returnGradient.SetKeys(colorKeys, alphaKeys);


                if (_settings.NeedLimitPoints)
                    EvaluateGradientLimitPoints(returnGradient, _settings.LimitPoints);

                return returnGradient;
            }


            void EvaluateGradientLimitPoints(Gradient gradient, int points)
            {
                if (gradient.alphaKeys.Length == points && gradient.colorKeys.Length == points) return;

                var colorKeys = new GradientColorKey[points];
                var alphaKeys = new GradientAlphaKey[points];

                for (var i = 0; i < points; i++)
                {
                    float time = (float)i / (points - 1);
                    var color = gradient.Evaluate(time);
                    colorKeys[i] = new GradientColorKey(color, time);
                    alphaKeys[i] = new GradientAlphaKey(color.a, time);
                }

                gradient.SetKeys(colorKeys, alphaKeys);
            }

            void WriteGradient(Gradient gradient)
            {
                if (_settings.NeedLimitPoints)
                    EvaluateGradientLimitPoints(gradient, _settings.LimitPoints);

                writer.WritePropertyName("gradient");
                writer.WriteRawValue(JsonConvert.SerializeObject(gradient, Formatting.Indented));
            }
        }

        public override ParticleSystem.MinMaxGradient ReadJson(JsonReader reader, Type objectType,
            ParticleSystem.MinMaxGradient existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            
            
            if(_settings.Mode == MinMaxGradientConvertMode.AllToColor)
            {
                return ReadJsonToOneColor(reader, objectType, existingValue, hasExistingValue, serializer);
            }
            
            JObject jo = JObject.Load(reader);

            var colorKeysJArray = jo["gradient"]["colorKeys"].Value<JArray>();
            GradientColorKey[] colorKeys = new GradientColorKey[colorKeysJArray.Count];
            colorKeys = JsonConvert.DeserializeObject<GradientColorKey[]>(colorKeysJArray.ToString());

            var alphaKeysJArray = jo["gradient"]["alphaKeys"].Value<JArray>();
            GradientAlphaKey[] aplhaKeys = new GradientAlphaKey[alphaKeysJArray.Count];
            aplhaKeys = JsonConvert.DeserializeObject<GradientAlphaKey[]>(alphaKeysJArray.ToString());

            var gradient = new Gradient();
            gradient.SetKeys(colorKeys, aplhaKeys);

            var minmaxGradient = new ParticleSystem.MinMaxGradient(gradient);


            if (hasExistingValue)
            {
                existingValue.mode = minmaxGradient.mode;
                existingValue.color = minmaxGradient.color;
                existingValue.gradient = minmaxGradient.gradient;
                existingValue.colorMax = minmaxGradient.colorMax;
                existingValue.colorMin = minmaxGradient.colorMin;
                existingValue.gradientMax = minmaxGradient.gradientMax;
                existingValue.gradientMin = minmaxGradient.gradientMin;
            }

            return minmaxGradient;
        }
    }
}