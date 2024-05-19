using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEngine;

namespace NNParticleSystemGenerator
{
    public class MinMaxConvertUtils
    {
        private const string RegexForMode = @".m_Mode.: (.*),";
        private const string RegexForConstant = @".constant.: ([-\d.]*)";
        private const string RegexForConstantMax = @".constantMax.: ([-\d.]*)";
        private const string RegexForConstantMin = @".constantMin.: ([-\d.]*)";
        private const string RegexForCurveMultiplier = @"curveMultiplier.: (?'curveMultiplier'[-\d.]*)";
        private const string RegexForMinCurve =
            @"curveMultiplier.: (?'curveMultiplier'[-\d.]*),[\d\D]*.curveMin.: {.*\n\s*.keys.:(?'keys' \[[\d\D-[]]]*])\D*length.: (?'length'[-\d.]*),\D*preWrapMode.: (?'preWrapMode'[-\d.]*),\D*postWrapMode.: (?'postWrapMode'[-\d.]*)";

        private const string RegexForMaxCurve =
            @"curveMultiplier.: (?'curveMultiplier'[-\d.]*),[\d\D]*.curveMax.: {.*\n\s*.keys.:(?'keys' \[[\d\D-[]]]*])\D*length.: (?'length'[-\d.]*),\D*preWrapMode.: (?'preWrapMode'[-\d.]*),\D*postWrapMode.: (?'postWrapMode'[-\d.]*)";

        protected MinMaxCurveConverterSettings Settings;

        public MinMaxConvertUtils(MinMaxCurveConverterSettings settings)
        {
            Settings = settings;
        }

        public static float GetConstantFromCurve(AnimationCurve curve)
        {
            int sampleCount = 1000; // Количество выборок для оценки кривой.

            float curveLength = curve.keys[curve.length - 1].time;
            float step = curveLength / sampleCount;

            float totalValue = 0f;

            for (int i = 0; i < sampleCount; i++)
            {
                float time = i * step;
                float evaluatedValue = curve.Evaluate(time);
                totalValue += evaluatedValue;
            }

            float averageValue = totalValue / sampleCount;
            return averageValue;
        }
        
        public static void GetMinMaxFromCurve(AnimationCurve curve, out float minValue, out float maxValue)
        {
            minValue = float.MaxValue;
            maxValue = float.MinValue;

            int numSamples = 1000; // Количество выборок для вычисления минимального и максимального значения.

            for (int i = 0; i <= numSamples; i++)
            {
                float time = i / (float)numSamples;
                float value = curve.Evaluate(time);

                if (value < minValue)
                {
                    minValue = value;
                }

                if (value > maxValue)
                {
                    maxValue = value;
                }
            }
        }

        
        public ParticleSystem.MinMaxCurve GetCurve(string json)
        {
            Regex regex = new Regex(RegexForMode);
            MatchCollection matches = regex.Matches(json);
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    var minMaxCurve = new ParticleSystem.MinMaxCurve();
                    minMaxCurve.mode = (ParticleSystemCurveMode)Int32.Parse(match.Groups[1].Value);

                    switch (minMaxCurve.mode)
                    {
                        case ParticleSystemCurveMode.Constant:
                            return GetConstant(json);
                        case ParticleSystemCurveMode.Curve:
                            return GetSoloCurve(json);
                            break;
                        case ParticleSystemCurveMode.TwoCurves:
                            return GetTwoCurves(json);
                            break;
                        case ParticleSystemCurveMode.TwoConstants:
                            return GetTwoConstants(json);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }


                    return minMaxCurve;
                }
            }

            throw new Exception("Failed to read type 'MinMaxCurve: " + json);
        }

        private ParticleSystem.MinMaxCurve GetTwoConstants(string s)
        {
            float min = GetFloat(s, RegexForConstantMin);
            float max = GetFloat(s, RegexForConstantMax);
            return new ParticleSystem.MinMaxCurve(min, max);
        }

        private ParticleSystem.MinMaxCurve GetConstant(string s)
        {
            Regex regex = new Regex(RegexForConstant);
            MatchCollection matches = regex.Matches(s);
            if (matches.Count > 0)
            {
                var value = matches[0].Groups[1].Value;
                var constant = ParseFloat(value);
                return new ParticleSystem.MinMaxCurve(constant);
            }


            throw new Exception("Failed to read type 'MinMaxCurve: " + s);
        }


        private ParticleSystem.MinMaxCurve GetSoloCurve(string s)
        {
            float multiplier = GetCurveMultiplier(s);
            AnimationCurve curve = GetMaxAnimationCurve(s);
            return new ParticleSystem.MinMaxCurve(multiplier, curve);
        }

        public ParticleSystem.MinMaxCurve GetTwoCurves(string s)
        {
            float multiplier = GetCurveMultiplier(s);
            AnimationCurve maxCurve = GetMaxAnimationCurve(s);
            AnimationCurve minCurve = GetMinAnimationCurve(s);
            return new ParticleSystem.MinMaxCurve(multiplier, minCurve, maxCurve);
        }


        private AnimationCurve GetMaxAnimationCurve(string s)
        {
            return GetAnimationCurve(RegexForMaxCurve, s);
        }

        private AnimationCurve GetMinAnimationCurve(string s)
        {
            return GetAnimationCurve(RegexForMinCurve, s);
        }

        private AnimationCurve GetAnimationCurve(string regexPattern, string s)
        {
            Regex regex = new Regex(regexPattern);
            MatchCollection matches = regex.Matches(s);
            if (matches.Count > 0)
            {
                int length = 0;
                int postWrapMode = 0;
                int preWrapMode = 0;
                string keysJson = "[]";

                foreach (Group group in matches[0].Groups)
                {
                    try
                    {
                        switch (group.Name)
                        {
                            case "length":
                                length = GetClampedInt(group.Value);
                                break;
                            case "preWrapMode":
                                preWrapMode = GetClampedInt(group.Value);
                                break;
                            case "postWrapMode":
                                postWrapMode = GetClampedInt(group.Value);
                                break;
                            case "keys":
                                keysJson = group.Value;
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"group.Name: {group.Name} group.Value: {group.Value}");
                        throw;
                    }
                }


                length = Math.Clamp(length, 0, Math.Max(0, length));

                if (length != Settings.LimitPoints && Settings.NeedLimitPoints)
                {
                    Debug.LogError("length != _settings.LimitPoints && _settings.NeedLimitPoints" + length + " != " +
                                   Settings.LimitPoints);
                    length = Settings.LimitPoints;
                }

                Keyframe[] keys;
                try
                {
                    keys = new Keyframe[length];
                }
                catch (Exception e)
                {
                    Debug.Log("length " + length);
                    Console.WriteLine(e);
                    throw;
                }

                keys = JsonConvert.DeserializeObject<Keyframe[]>(keysJson,
                    JsonConvertersGenerator.GetEnumConverters().ToArray());

                var curve = new AnimationCurve(keys);
                curve.postWrapMode = (WrapMode)postWrapMode;
                curve.preWrapMode = (WrapMode)preWrapMode;

                return curve;
            }

            throw new Exception("Failed to read type 'AnimationCurve: " + s);
        }

        private static int GetClampedInt(string s)
        {
            var f = ParseFloat(s);
            return Mathf.RoundToInt(f);
        }

        private static float GetCurveMultiplier(string s)
        {
            Regex regex = new Regex(RegexForCurveMultiplier);
            float curveMultiplier = 1;
            MatchCollection matches = regex.Matches(s);
            if (matches.Count > 0)
            {
                foreach (Group group in matches[0].Groups)
                {
                    switch (group.Name)
                    {
                        case "curveMultiplier":
                            curveMultiplier = ParseFloat(group.Value);
                            break;
                    }
                }
            }

            return curveMultiplier;
        }

        private static float GetFloat(string s, string pattern)
        {
            Regex regex = new Regex(pattern);
            Match match = regex.Match(s);
            if (match.Success)
            {
                return ParseFloat(match.Groups[1].Value);
            }

            throw new Exception($"Failed to read type 'float: pattern:\n {pattern} \n json:\n {s}");
        }

        private static float ParseFloat(string s)
        {
            try
            {
                return float.Parse(s, NumberStyles.Any, CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed Parse: " + s);
                throw;
            }
        }
    }
}