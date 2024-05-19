using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace NNParticleSystemGenerator
{
    public class DatasetMaterialConverter : MaterialConverter
    {
        private List<Material> _materials;

        public DatasetMaterialConverter(List<ParticleSystem> particleSystems)
        {
            _materials = new List<Material>();
            foreach (var particleSystem in particleSystems)
            {
                var sharedMaterial = particleSystem.GetComponent<ParticleSystemRenderer>().sharedMaterial;
                _materials.Add(sharedMaterial);
            }
        }

        public override int GetIndexFromMaterial(Material value)
        {
            return _materials.IndexOf(value);
        }

        public override Material GetMaterialFromIndex(int index)
        {
            if (index < 0 || index >= _materials.Count)
                return _materials[0];
            return _materials[index];
        }
    }

    public class NullMaterialConverter : MaterialConverter
    {
        public override int GetIndexFromMaterial(Material value)
        {
            return -1;
        }

        public override Material GetMaterialFromIndex(int index)
        {
            return null;
        }
    }

    public abstract class MaterialConverter
    {
        public abstract int GetIndexFromMaterial(Material value);
        public abstract Material GetMaterialFromIndex(int index);
    }

    public class ParticlesConverterSettings
    {
        public MinMaxCurveConverterSettings CurveConverterSettings;
        public MinMaxGradientSettings GradientConverterSettings;
        public MaterialConverter MaterialConverter;

        public ParticlesConverterSettings(MinMaxCurveConverterSettings curveConverterSettings,
            MinMaxGradientSettings gradientConverterSettings, MaterialConverter materialConverter)
        {
            CurveConverterSettings = curveConverterSettings;
            GradientConverterSettings = gradientConverterSettings;
            MaterialConverter = materialConverter;
        }

        public static ParticlesConverterSettings GetDefault()
        {
            var minMaxCurveConverterSettings = new MinMaxCurveConverterSettings(MinMaxCurveConvertMode.Default);
            var minMaxGradientSettings = new MinMaxGradientSettings(MinMaxGradientConvertMode.AllToGradient);
            var nullMaterialConverter = new NullMaterialConverter();

            var particleSystemConverterSettings =
                new ParticlesConverterSettings(minMaxCurveConverterSettings, minMaxGradientSettings,
                    nullMaterialConverter);


            return particleSystemConverterSettings;
        }
    }

    public class ParticleSystemConverter : JsonConverter<ParticleSystem>
    {
        private ParticleSystem _fillParticleSystem;
        private ParticlesConverterSettings _settings;

        public ParticleSystemConverter(ParticleSystem fillParticleSystem, ParticlesConverterSettings settings)
        {
            _settings = settings;
            _fillParticleSystem = fillParticleSystem;
        }

        PrimitiveType GetMeshPrimitiveType(ParticleSystemRenderer value)
        {
            if (value.mesh == null)
            {
                Debug.LogError("null mesh primitive type (used cube)", value.gameObject);
                return PrimitiveType.Cube;
            }

            try
            {
                if (Enum.TryParse<PrimitiveType>(value.mesh.name, out var primitiveType))
                {
                    return primitiveType;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message, value.gameObject);
            }

            Debug.LogError("Unknown mesh primitive type (used cube)" + value.mesh.name, value.gameObject);
            return PrimitiveType.Cube;
        }

        Mesh GetMeshFromInt(PrimitiveType value)
        {
            return AssetDatabaseHelper.GetPrimitiveMeshAssetRef(value);
        }


        protected object DeserializeRendererObject(string name, object value)
        {
            switch (name)
            {
                case "mesh":
                    return GetMeshFromInt((PrimitiveType)value);

                case "material":
                    return _settings.MaterialConverter.GetMaterialFromIndex((int)value);
            }

            return value;
        }


        protected class RendererField
        {
            public object Value;
            public Action<ParticleSystemRenderer, object> Setter;

            public RendererField(object value, Action<ParticleSystemRenderer, object> setter)
            {
                Value = value;
                Setter = setter;
            }
        }

        protected Dictionary<string, RendererField> GetRendererFields(ParticleSystemRenderer value)
        {
            Dictionary<string, RendererField> fields = new Dictionary<string, RendererField>()
            {
                {
                    "renderMode",
                    new RendererField(value.renderMode, (r, o) => r.renderMode = RenderModeFromInt(Convert.ToInt32(o)))
                },

                //Default
                // { "shadowCastingMode", value.shadowCastingMode },
                // { "receiveShadows", value.receiveShadows },
                // { "shadowBias", value.shadowBias },
                // { "motionVectorGenerationMode", value.motionVectorGenerationMode },
                // { "sortingLayerID", value.sortingLayerID },


                {
                    "sortingOrder", new RendererField(value.sortingOrder, (r, o) => r.sortingOrder = Convert.ToInt32(o))
                },


                // { "lightProbeUsage", value.lightProbeUsage },
                // { "reflectionProbeUsage", value.reflectionProbeUsage }

                //Mesh
                {
                    "mesh",
                    new RendererField(
                        GetMeshPrimitiveType(value),
                        (r, o) => r.mesh = GetMeshFromInt((PrimitiveType)Convert.ToInt32(o)))
                },
                // { "meshDistribution", value.meshDistribution },
                // { "normalDirection", value.normalDirection },

                {
                    "material",
                    new RendererField(
                        _settings.MaterialConverter.GetIndexFromMaterial(value.sharedMaterial),
                        (r, o) => r.sharedMaterial =
                            _settings.MaterialConverter.GetMaterialFromIndex(Convert.ToInt32(o)))
                },


                // { "sortMode", value.sortMode },
                // { "sortingFudge", value.sortingFudge },
                // { "alignment", value.alignment },
                // { "flip", value.flip },
                // { "enableGPUInstancing", value.enableGPUInstancing },
                // { "pivot", value.pivot },
                // { "maskInteraction", value.maskInteraction },
                // { "activeVertexStreamsCount", value.activeVertexStreamsCount },

                //Vertical Billboard
                // { "minParticleSize", value.minParticleSize },
                // { "maxParticleSize", value.maxParticleSize },

                //Streched Billboard
                // { "cameraVelocityScale", value.cameraVelocityScale },
                // { "velocityScale", value.velocityScale },
                // { "lengthScale", value.lengthScale },
                // { "freeformStretching", value.freeformStretching },
                // { "rotateWithStretchDirection", value.rotateWithStretchDirection },
            };

            return fields;

            ParticleSystemRenderMode RenderModeFromInt(int i)
            {
                if (i > 5)
                {
                    Debug.LogError("Unknown ParticleSystemRenderMode used Billboard", value.gameObject);
                    return ParticleSystemRenderMode.Billboard;
                }

                return i switch
                {
                    0 => ParticleSystemRenderMode.Billboard,
                    1 => ParticleSystemRenderMode.Stretch,
                    2 => ParticleSystemRenderMode.HorizontalBillboard,
                    3 => ParticleSystemRenderMode.VerticalBillboard,
                    4 => ParticleSystemRenderMode.Mesh,
                    5 => ParticleSystemRenderMode.None,
                    _ => ParticleSystemRenderMode.Billboard
                };

                return ParticleSystemRenderMode.None;
            }
        }

        protected Dictionary<string, object> GetModules(ParticleSystem value)
        {
            Dictionary<string, object> modules = new Dictionary<string, object>()
            {
                { "main", value.main },
                { "emission", value.emission },
                { "shape", value.shape },
                { "velocityOverLifetime", value.velocityOverLifetime },
                // { "limitVelocityOverLifetime", value.limitVelocityOverLifetime },
                // { "inheritVelocity", value.inheritVelocity },
                // { "lifetimeByEmitterSpeed", value.lifetimeByEmitterSpeed },
                { "forceOverLifetime", value.forceOverLifetime },
                { "colorOverLifetime", value.colorOverLifetime },
                // { "colorBySpeed", value.colorBySpeed },
                { "sizeOverLifetime", value.sizeOverLifetime },
                // { "sizeBySpeed", value.sizeBySpeed },
                { "rotationOverLifetime", value.rotationOverLifetime },
                // { "rotationBySpeed", value.rotationBySpeed },
                // { "externalForces", value.externalForces },
                // { "noise", value.noise },
                // { "collision", value.collision },
                // { "trigger", value.trigger },
                // {"subEmitters", value.subEmitters},
                // { "textureSheetAnimation", value.textureSheetAnimation },
                // { "lights", value.lights },
                // { "trails", value.trails },
                // { "customData", value.customData },
            };

            return modules;
        }

        public override void WriteJson(JsonWriter writer, ParticleSystem value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            Dictionary<string, object> modules = GetModules(value);

            WriteEnables();
            WriteRenderer();

            foreach (var modulePair in modules)
            {
                writer.WritePropertyName(modulePair.Key);
                serializer.Serialize(writer, modulePair.Value);

                // if(modulePair.Value is ParticleSystem.EmissionModule emissionModule)
                // {
                //     writer.WritePropertyName(modulePair.Key);
                //     serializer.Serialize(writer, emissionModule);
                //     continue;
                // }
                //
                // writer.WritePropertyName(modulePair.Key);
                //
                //
                // // AdditionalJsonPreWrite(modulePair.Value, writer, value, serializer);
                // var fieldsJson =
                // SerializeHelpers.GetFieldsJson(modulePair.Value.GetType(), modulePair.Value, _settings);
                // writer.WriteRawValue(fieldsJson);
                //
                // AdditionalJsonWrite(modulePair.Value, writer, value, serializer);
            }


            writer.WriteEndObject();

            void WriteEnables()
            {
                writer.WritePropertyName("enables");
                var enablesList = new List<int>();
                foreach (var modulesValue in modules.Values)
                {
                    var fieldsObjects = SerializeHelpers.GetFieldsObjects(modulesValue.GetType(), modulesValue);

                    if (!fieldsObjects.ContainsKey("enabled"))
                    {
                        Debug.LogError($" {modulesValue.GetType().Name} has no enabled field");
                        continue;
                    }

                    var isEnabled = (bool)fieldsObjects["enabled"];
                    enablesList.Add(isEnabled ? 1 : 0);
                }

                serializer.Serialize(writer, enablesList);

                Debug.LogError($"enables count {enablesList.Count}");
            }

            void WriteRenderer()
            {
                var particleSystemRenderer = value.GetComponent<ParticleSystemRenderer>();

                writer.WritePropertyName("renderer");
                writer.WriteStartObject();
                var rendererFields = GetRendererFields(particleSystemRenderer);

                foreach (var fieldPair in rendererFields)
                {
                    writer.WritePropertyName(fieldPair.Key);
                    writer.WriteValue(fieldPair.Value.Value);
                }

                writer.WriteEndObject();
            }
        }

        public override ParticleSystem ReadJson(JsonReader reader, Type objectType, ParticleSystem existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            var enablesList = new List<float>();
            var converters = JsonConvertersGenerator.GetParticlesConvertersRead(_settings);
            var settings = new JsonSerializerSettings();
            settings.Converters = converters;

            var modules = GetModules(_fillParticleSystem);
            JObject jo = JObject.Load(reader);
            foreach (var jProperty in jo.Properties())
            {
                if (jProperty.Name == "enables")
                {
                    JsonConvert.PopulateObject(jProperty.Value.ToString(), enablesList, settings);
                    continue;
                }

                if (jProperty.Name == "renderer")
                {
                    ReadRenderer(jProperty);
                    continue;
                }

                if (jProperty.Name == "emission")
                {
                    var emmisionConverter = new EmissionConverter();
                    var moduleObject = (ParticleSystem.EmissionModule)modules[jProperty.Name];
                    emmisionConverter.ReadJson(jProperty.CreateReader(), typeof(ParticleSystem.EmissionModule),
                        moduleObject, hasExistingValue, serializer);
                    continue;
                }

                if (modules.ContainsKey(jProperty.Name))
                {
                    var moduleObject = modules[jProperty.Name];
                    // Debug.Log($"Populating property: {jProperty.Name} with value: {jProperty.Value.ToString()}");
                    JsonConvert.PopulateObject(jProperty.Value.ToString(), moduleObject, settings);

                    if (jProperty.Name == "main")
                    {
                        // Debug.LogError($"maxParticles {((ParticleSystem.MainModule)moduleObject).maxParticles}");
                        // var selectToken = jProperty.Value.SelectToken("maxParticles");
                        // Debug.LogError($"maxParticles {selectToken.Value<float>()}");
                    }
                }
                else
                {
                    Debug.LogError("Not populated property: " + jProperty.Name);
                }
            }

            FillEnables(enablesList);
            return _fillParticleSystem;

            void FillEnables(List<float> enablesList)
            {
                var i = -1;
                foreach (var (key, modulesValue) in modules)
                {
                    
                    var fieldsObjects = SerializeHelpers.GetFieldsObjects(modulesValue.GetType(), modulesValue);
                    if (!fieldsObjects.ContainsKey("enabled"))
                    {
                        continue;
                    }
                    i++;
                    System.Type moduleType = modulesValue.GetType();
                    PropertyInfo[] properties = moduleType.GetProperties();
                    var wasSet = false;
                    foreach (PropertyInfo field in properties)
                    {
                        string fieldName = field.Name;
                        object fieldValue = field.GetValue(modulesValue);
                        
                        if (fieldName == "enabled")
                        {
                            // Debug.LogError($"{key} enabled to {enablesList[i]}");
                            field.SetValue(modulesValue, enablesList[i] >= 0.5f);
                            wasSet = true;
                            break;      
                        }
                    }

                    if (!wasSet) Debug.LogError($"{key} has no enabled field");
                }
            }

            void ReadRenderer(JProperty rendererProperty)
            {
                var value = rendererProperty.Value.Value<JObject>();
                var enumerable = value.Properties();
                var particleSystemRenderer = _fillParticleSystem.GetComponent<ParticleSystemRenderer>();
                var rendererFields = GetRendererFields(particleSystemRenderer);

                foreach (var jp in enumerable)
                {
                    if (rendererFields.ContainsKey(jp.Name))
                    {
                        var field = rendererFields[jp.Name];

                        field.Setter(particleSystemRenderer, jp.Value.Value<object>());
                    }
                }
            }
        }
    }
}