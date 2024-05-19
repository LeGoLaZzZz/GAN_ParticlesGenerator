using System;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace NNParticleSystemGenerator
{
    public class MinMaxCurveConverter : JsonConverter<ParticleSystem.MinMaxCurve>
    {
        private MinMaxCurveConverterSettings _settings;

        private MinMaxConvertUtils _convertUtils;
        private AllTwoCurvesMinMaxCurveConvertHelper _twoCurvesConvert;
        private DefaultMinMaxCurveConvertHelper _defaultConvert;
        private ConstantMinMaxCurveConvertHelper _constantConvert;
        private AllToMinMaxConstantMinMaxCurveConvertHelper _minMaxConstantConvert;


        public MinMaxCurveConverter(MinMaxCurveConverterSettings settings)
        {
            _settings = settings;
            _convertUtils = new MinMaxConvertUtils(_settings);
            _twoCurvesConvert = new AllTwoCurvesMinMaxCurveConvertHelper(_settings, _convertUtils);
            _constantConvert = new ConstantMinMaxCurveConvertHelper(_settings, _convertUtils);
            _defaultConvert = new DefaultMinMaxCurveConvertHelper(_settings, _convertUtils);
            _minMaxConstantConvert = new AllToMinMaxConstantMinMaxCurveConvertHelper(_settings, _convertUtils);
        }

        #region WriteJson

        public override void WriteJson(JsonWriter writer, ParticleSystem.MinMaxCurve value, JsonSerializer serializer)
        {
            switch (_settings.Mode)
            {
                case MinMaxCurveConvertMode.Default:
                    _defaultConvert.WriteJson(writer, value, serializer);
                    break;
                case MinMaxCurveConvertMode.AllToTwoCurves:
                    _twoCurvesConvert.WriteJson(writer, value, serializer);
                    break;
                case MinMaxCurveConvertMode.AllToConstant:
                    _constantConvert.WriteJson(writer, value, serializer);
                    break;
                case MinMaxCurveConvertMode.AllToMinMaxConstant:
                    _minMaxConstantConvert.WriteJson(writer, value, serializer);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        public override ParticleSystem.MinMaxCurve ReadJson(JsonReader reader, Type objectType,
            ParticleSystem.MinMaxCurve existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            switch (_settings.Mode)
            {
                case MinMaxCurveConvertMode.Default:
                    return _defaultConvert.ReadJson(reader, objectType, existingValue, hasExistingValue, serializer);
                case MinMaxCurveConvertMode.AllToTwoCurves:
                    return _twoCurvesConvert.ReadJson(reader, objectType, existingValue, hasExistingValue, serializer);
                case MinMaxCurveConvertMode.AllToConstant:
                    return _constantConvert.ReadJson(reader, objectType, existingValue, hasExistingValue, serializer);

                case MinMaxCurveConvertMode.AllToMinMaxConstant:
                    return _minMaxConstantConvert.ReadJson(reader, objectType, existingValue, hasExistingValue,
                        serializer);
            }

            throw new NotImplementedException();
        }
    }
}