using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace NNParticleSystemGenerator
{
    public static class SerializeHelpers
    {
        public static int GetClampedInt(string s)
        {
            var f = ParseFloat(s);
            return Mathf.RoundToInt(f);
        }

        public static float ParseFloat(string s)
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

        public static string RemoveParticlesFields(string json, List<string> fieldsToRemove)
        {
            string updatedJson = RemoveFields(json, fieldsToRemove);
            return updatedJson;
        }

        private static string RemoveParticlesFields(string json)
        {
            // Список полей для удаления
            List<string> fieldsToRemove = new List<string>
            {
                "shape.mesh",
                "shape.meshRenderer",
                "shape.skinnedMeshRenderer",
                "shape.sprite",
                "shape.spriteRenderer",
                "shape.texture",
                "shape.textureClipChannel",
                "shape.textureClipThreshold",
                "shape.textureColorAffectsParticles",
                "shape.textureAlphaAffectsParticles",
                "shape.textureBilinearFiltering",
                "shape.textureUVChannel",
                "main.customSimulationSpace",
                "externalForces.influenceMask",
                "collision.collidesWith",
                "lights.light",
            };

            // удаление Enabled
            fieldsToRemove.AddRange(new List<string>
            {
                // main
                { "main.enabled" },

                { "main.randomizeRotationDirection" },
                { "main.emitterVelocity" },
                // {"main.duration"},
                // {"main.loop"},
                // {"main.prewarm"},
                // {"main.startDelay"},
                { "main.startDelayMultiplier" },
                // {"main.startLifetime"},
                { "main.startLifetimeMultiplier" },
                // {"main.startSpeed"},
                { "main.startSpeedMultiplier" },
                // {"main.startSize3D"},
                // {"main.startSize"},
                { "main.startSizeMultiplier" },
                // {"main.startSizeX"},
                { "main.startSizeXMultiplier" },
                // {"main.startSizeY"},
                { "main.startSizeYMultiplier" },
                // {"main.startSizeZ"},
                { "main.startSizeZMultiplier" },
                // {"main.startRotation3D"},
                // {"main.startRotation"},
                { "main.startRotationMultiplier" },
                // {"main.startRotationX"},
                { "main.startRotationXMultiplier" },
                // {"main.startRotationY"},
                { "main.startRotationYMultiplier" },
                // {"main.startRotationZ"},
                { "main.startRotationZMultiplier" },
                { "main.flipRotation" },
                // {"main.gravityModifier"},
                { "main.gravityModifierMultiplier" },
                { "main.simulationSpace" },
                { "main.simulationSpeed" },
                { "main.useUnscaledTime" },
                { "main.scalingMode" },
                { "main.playOnAwake" },
                // { "main.maxParticles" },
                { "main.emitterVelocityMode" },
                { "main.stopAction" },
                { "main.ringBufferMode" },
                { "main.ringBufferLoopRange" },
                { "main.cullingMode" },

                // emission
                { "emission.enabled" },

                { "emission.rate" },
                { "emission.rateMultiplier" },
                { "emission.rateOverTimeMultiplier" },
                { "emission.rateOverDistanceMultiplier" },
                { "emission.burstCount" },

                // shape
                { "shape.enabled" },

                // {"shape.box"},
                // {"shape.meshScale"},
                // {"shape.randomDirection"},
                // {"shape.shapeType"},
                // {"shape.randomDirectionAmount"},
                // {"shape.sphericalDirectionAmount"},
                // {"shape.randomPositionAmount"},
                // {"shape.alignToDirection"},
                // {"shape.radius"},
                // {"shape.radiusMode"},
                // {"shape.radiusSpread"},
                // {"shape.radiusSpeed"},
                { "shape.radiusSpeedMultiplier" },
                // {"shape.radiusThickness"},
                // {"shape.angle"},
                // {"shape.length"},
                // {"shape.boxThickness"},
                // {"shape.meshShapeType"},
                { "shape.useMeshMaterialIndex" },
                { "shape.meshMaterialIndex" },
                { "shape.useMeshColors" },
                { "shape.normalOffset" },
                // {"shape.meshSpawnMode"},
                // {"shape.meshSpawnSpread"},
                // {"shape.meshSpawnSpeed"},
                { "shape.meshSpawnSpeedMultiplier" },
                // {"shape.arc"},
                // {"shape.arcMode"},
                // {"shape.arcSpread"},
                // {"shape.arcSpeed"},
                { "shape.arcSpeedMultiplier" },
                // {"shape.donutRadius"},
                // {"shape.position"},
                // {"shape.rotation"},
                // {"shape.scale"},


                // velocityOverLifetime
                { "velocityOverLifetime.enabled" },

                // { "velocityOverLifetime.x"},
                // { "velocityOverLifetime.y"},
                // { "velocityOverLifetime.z"},
                { "velocityOverLifetime.xMultiplier" },
                { "velocityOverLifetime.yMultiplier" },
                { "velocityOverLifetime.zMultiplier" },
                // { "velocityOverLifetime.orbitalX"},
                // { "velocityOverLifetime.orbitalY"},
                // { "velocityOverLifetime.orbitalZ"},
                { "velocityOverLifetime.orbitalXMultiplier" },
                { "velocityOverLifetime.orbitalYMultiplier" },
                { "velocityOverLifetime.orbitalZMultiplier" },
                // { "velocityOverLifetime.orbitalOffsetX"},
                // { "velocityOverLifetime.orbitalOffsetY"},
                // { "velocityOverLifetime.orbitalOffsetZ"},
                { "velocityOverLifetime.orbitalOffsetXMultiplier" },
                { "velocityOverLifetime.orbitalOffsetYMultiplier" },
                { "velocityOverLifetime.orbitalOffsetZMultiplier" },
                // { "velocityOverLifetime.radial"},
                { "velocityOverLifetime.radialMultiplier" },
                // { "velocityOverLifetime.speedModifier"},
                { "velocityOverLifetime.speedModifierMultiplier" },
                // { "velocityOverLifetime.space"},


                // forceOverLifetime
                { "forceOverLifetime.enabled" },

                // { "forceOverLifetime.x" },
                // { "forceOverLifetime.y" },
                // { "forceOverLifetime.z" },
                { "forceOverLifetime.xMultiplier" },
                { "forceOverLifetime.yMultiplier" },
                { "forceOverLifetime.zMultiplier" },
                // { "forceOverLifetime.space" },
                // { "forceOverLifetime.randomized" },


                // colorOverLifetime
                { "colorOverLifetime.enabled" },
                // { "colorOverLifetime.color" },

                // sizeOverLifetime
                { "sizeOverLifetime.enabled" },

                // {"sizeOverLifetime.size"},
                { "sizeOverLifetime.sizeMultiplier" },
                // {"sizeOverLifetime.x"},
                { "sizeOverLifetime.xMultiplier" },
                // {"sizeOverLifetime.y"},
                { "sizeOverLifetime.yMultiplier" },
                // {"sizeOverLifetime.z"},
                { "sizeOverLifetime.zMultiplier" },
                // {"sizeOverLifetime.separateAxes"},

                // rotationOverLifetime
                { "rotationOverLifetime.enabled" },
                // { "x" },
                { "rotationOverLifetime.xMultiplier" },
                // { "y" },
                { "rotationOverLifetime.yMultiplier" },
                // { "z" },
                { "rotationOverLifetime.zMultiplier" },
                // { "separateAxes" },

                // other
                { "limitVelocityOverLifetime.enabled" },
                { "inheritVelocity.enabled" },
                { "lifetimeByEmitterSpeed.enabled" },
                { "colorBySpeed.enabled" },
                { "sizeBySpeed.enabled" },
                { "rotationBySpeed.enabled" },
                { "externalForces.enabled" },
                { "noise.enabled" },
                { "collision.enabled" },
                { "trigger.enabled" },
                { "subEmitters.enabled" },
                { "textureSheetAnimation.enabled" },
                { "lights.enabled" },
                { "trails.enabled" },
                { "customData.enabled" },
            });


            return RemoveParticlesFields(json, fieldsToRemove);
        }

        static string RemoveFields(string json, List<string> fields)
        {
            JObject jsonObj = JObject.Parse(json);

            foreach (var field in fields)
            {
                RemoveField(jsonObj, field);
            }

            return jsonObj.ToString();
        }

        static void RemoveField(JToken token, string fieldPath)
        {
            string[] parts = fieldPath.Split('.');
            for (int i = 0; i < parts.Length; i++)
            {
                if (i == parts.Length - 1)
                {
                    // Удаление поля

                    var jToken = token[parts[i]];
                    if (jToken != null)
                    {
                        JProperty propertyToRemove = jToken.Parent as JProperty;
                        if (propertyToRemove != null)
                        {
                            propertyToRemove.Remove();
                        }
                    }
                }
                else
                {
                    token = token[parts[i]];
                    if (token == null) return;
                }
            }
        }


        public static string SerializeParticleSystemToJson(ParticleSystem particleSystem,
            ParticlesConverterSettings settings)
        {
            var jsonSettings = new JsonSerializerSettings();
            jsonSettings.Converters = JsonConvertersGenerator.GetParticlesConvertersWrite(particleSystem, settings);
            var psJson = JsonConvert.SerializeObject(particleSystem, jsonSettings);
            var newJson = RemoveParticlesFields(psJson);
            var count = CountNumberFields(JObject.Parse(newJson));
            Debug.Log($"Count fields: {count}");
            return newJson;
        }

        private static int CountNumberFields(JToken token)
        {
            int count = 0;

            switch (token.Type)
            {
                case JTokenType.Object:
                    foreach (JProperty property in token.Children<JProperty>())
                    {
                        count += CountNumberFields(property.Value);
                    }
                    break;
                case JTokenType.Array:
                    foreach (JToken value in token.Children())
                    {
                        count += CountNumberFields(value);
                    }
                    break;
                case JTokenType.Integer:
                case JTokenType.Float:
                case JTokenType.String:
                case JTokenType.Boolean:
                    count++;
                    break;
            }

            return count;
        }

        public static void ParseJsonToParticleSystem(ParticleSystem particleSystemToFill, string json,
            ParticlesConverterSettings settings)
        {
            var newJson = RemoveParticlesFields(json);
            var jsonSettings = new JsonSerializerSettings();
            jsonSettings.Converters =
                JsonConvertersGenerator.GetParticlesConvertersWrite(particleSystemToFill, settings);
            var deserializeObject = JsonConvert.DeserializeObject<ParticleSystem>(newJson, jsonSettings);
        }

        public static Dictionary<string, object> GetFieldsObjects(System.Type objectType, object objectValue)
        {
            var moduleObject = objectValue;
            System.Type mainModuleType = objectType;

            FieldInfo[] fields = mainModuleType.GetFields();
            PropertyInfo[] properties = mainModuleType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Create a dictionary to store the field names and values
            var values = new System.Collections.Generic.Dictionary<string, object>();

            // Iterate over the fields and store their names and values in the dictionary
            foreach (FieldInfo field in fields)
            {
                string fieldName = field.Name;
                object fieldValue = field.GetValue(moduleObject);
                values[fieldName] = fieldValue;
            }

            // Iterate over the fields and store their names and values in the dictionary
            foreach (PropertyInfo property in properties)
            {
                string name = property.Name;
                object value = property.GetValue(moduleObject);
                values[name] = value;
            }

            return values;
        }

        public static string GetFieldsJson(System.Type objectType, object objectValue,
            ParticlesConverterSettings settings)
        {
            var values = GetFieldsObjects(objectType, objectValue);

            var jsonsettings = new JsonSerializerSettings();
            jsonsettings.Converters = JsonConvertersGenerator.GetParticlesConvertersRead(settings);
            jsonsettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

            string json = JsonConvert.SerializeObject(values, Formatting.Indented, jsonsettings);
            return json;
        }

        public static void WriteJsonByFields<T>(JsonWriter writer, T value, JsonSerializer serializer,
            ParticlesConverterSettings settings)
        {
            var fieldsJson =
                SerializeHelpers.GetFieldsJson(value.GetType(), value, settings);
            writer.WriteRawValue(fieldsJson);
        }
    }
}