using System;
using Newtonsoft.Json;

namespace NNParticleSystemGenerator
{
    public class FloatEnumConverter<T> : JsonConverter<T> where T : Enum
    {
        public override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer)
        {
            writer.WriteValue((Convert.ToInt32(value)));
        }

        public override T ReadJson(JsonReader reader, Type objectType, T existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            string json = reader.Value.ToString();
            var clampedInt = SerializeHelpers.GetClampedInt(json);
            return (T)Enum.ToObject(typeof(T), clampedInt);
        }
    }
}