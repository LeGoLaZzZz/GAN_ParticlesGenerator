using System;
using Newtonsoft.Json;
using UnityEngine;

namespace NNParticleSystemGenerator
{
    public abstract class MinMaxCurveConvertHelper
    {
        protected MinMaxCurveConverterSettings Settings;
        protected MinMaxConvertUtils Utils;

        public MinMaxCurveConvertHelper(MinMaxCurveConverterSettings settings, MinMaxConvertUtils utils)
        {
            Settings = settings;
            Utils = utils;
        }

        public abstract void WriteJson(JsonWriter writer, ParticleSystem.MinMaxCurve value, JsonSerializer serializer);

        public abstract ParticleSystem.MinMaxCurve ReadJson(JsonReader reader, Type objectType,
            ParticleSystem.MinMaxCurve existingValue, bool hasExistingValue,
            JsonSerializer serializer);
    }
}