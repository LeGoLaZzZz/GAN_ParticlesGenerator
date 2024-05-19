using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace NNParticleSystemGenerator
{
    public class BurstConverter : JsonConverter<ParticleSystem.Burst>
    {
        public override void WriteJson(JsonWriter writer, ParticleSystem.Burst value, JsonSerializer serializer)
        {
            var fieldsObjects = SerializeHelpers.GetFieldsObjects(typeof(ParticleSystem.Burst), value);

            writer.WriteStartObject();


            foreach (var fieldObject in fieldsObjects)
            {
                if (fieldObject.Key == "minCount" || fieldObject.Key == "maxCount")
                {
                    continue;
                }

                if (fieldObject.Key == "count")
                {
                    writer.WritePropertyName(fieldObject.Key);
                    serializer.Serialize(writer, value.count.Evaluate(0.5f));
                    continue;
                }

                writer.WritePropertyName(fieldObject.Key);
                serializer.Serialize(writer, fieldObject.Value);
            }

            writer.WriteEndObject();
        }

        public override ParticleSystem.Burst ReadJson(JsonReader reader, Type objectType,
            ParticleSystem.Burst existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);

            var Time = (float)obj["time"];
            var Count = (float)obj["count"];
            var CycleCount = (int)obj["cycleCount"];
            var RepeatInterval = (float)obj["repeatInterval"];
            var Probability = (float)obj["probability"];

            var newBurst = new ParticleSystem.Burst(Time, Count, CycleCount, RepeatInterval);
            newBurst.probability = Mathf.Clamp(Probability,0f,1f);
            
            var minMaxCurve = newBurst.count;
            minMaxCurve.mode = ParticleSystemCurveMode.Constant;
            newBurst.count = minMaxCurve;
            return newBurst;
        }
    }
}