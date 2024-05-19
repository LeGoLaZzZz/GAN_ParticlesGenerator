using System;
using System.Globalization;
using Newtonsoft.Json;
using UnityEngine;

namespace NNParticleSystemGenerator
{
    public class CustomIntConverter : JsonConverter<int>
    {
        public override void WriteJson(JsonWriter writer, int value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }

        public override int ReadJson(JsonReader reader, Type objectType, int existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            string json = reader.Value.ToString();
            return GetClampedInt(json);
        }

        private int GetClampedInt(string s)
        {
            var f = ParseFloat(s);
            // Debug.LogError($"{s} -> {f}");
            return Mathf.RoundToInt(f);
        }

        private float ParseFloat(string s)
        {
            try
            {
                return float.Parse(s.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed Parse: " + s);
                throw;
            }
        }
    }
}