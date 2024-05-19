using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace NNParticleSystemGenerator
{
    public class EmissionConverter : JsonConverter<ParticleSystem.EmissionModule>
    {
        public override void WriteJson(JsonWriter writer, ParticleSystem.EmissionModule value,
            JsonSerializer serializer)
        {
            var limitCount = 1;


            writer.WriteStartObject();

            var fieldsObjects = SerializeHelpers.GetFieldsObjects(typeof(ParticleSystem.EmissionModule), value);

            foreach (var fieldObject in fieldsObjects)
            {
                if (fieldObject.Key == "burstCount")
                {
                    writer.WritePropertyName(fieldObject.Key);
                    serializer.Serialize(writer, limitCount);
                }
                else
                {
                    writer.WritePropertyName(fieldObject.Key);
                    serializer.Serialize(writer, fieldObject.Value);
                }
            }

            writer.WritePropertyName("bursts");
            var origBursts = new ParticleSystem.Burst[value.burstCount];
            value.GetBursts(origBursts);

            var newBursts = new ParticleSystem.Burst[limitCount];
            if (value.burstCount > limitCount)
            {
                for (int i = 0; i < limitCount; i++)
                {
                    newBursts[i] = origBursts[i];
                }

                Debug.Log($"BURSTS COUNT: {value.burstCount} -> {limitCount}");
            }
            else if (value.burstCount < limitCount)
            {
                for (int i = 0; i < value.burstCount; i++)
                {
                    newBursts[i] = origBursts[i];
                }

                for (int i = value.burstCount; i < limitCount; i++)
                {
                    newBursts[i] = new ParticleSystem.Burst(0, 0);
                }
            }

            for (var index = 0; index < newBursts.Length; index++)
            {
                var minMaxCurve = newBursts[index].count;
                minMaxCurve.mode = ParticleSystemCurveMode.Constant;
                newBursts[index].count = minMaxCurve;
            }

            serializer.Converters.Add(new BurstConverter());
            serializer.Serialize(writer, newBursts, typeof(ParticleSystem.Burst[]));

            writer.WriteEndObject();
        }

        public override ParticleSystem.EmissionModule ReadJson(JsonReader reader, Type objectType,
            ParticleSystem.EmissionModule existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            serializer.Converters.Add(new BurstConverter());
            JProperty emissionJProperty = JProperty.Load(reader);
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.AddRange(serializer.Converters);
            
            JsonConvert.PopulateObject(emissionJProperty.Value.ToString(), existingValue, settings);

            foreach (var jProperty in JObject.Parse(emissionJProperty.Value.ToString()).Properties())
            {
                if (jProperty.Name == "bursts")
                {
                    var deserializeBursts = serializer.Deserialize<ParticleSystem.Burst[]>(jProperty.Value.CreateReader());
                    existingValue.SetBursts(deserializeBursts);
                }
            }
            return existingValue;
        }
    }
}