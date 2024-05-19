using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Rendering;

namespace NNParticleSystemGenerator
{
    public static class JsonConvertersGenerator
    {
        public static List<JsonConverter> GetAllConverters(
            ParticlesConverterSettings settings,
            ParticleSystem particleSystemToWrite)
        {
            var converters = new List<JsonConverter>();           
            converters.Add(new EmissionConverter());
            converters.Add(new BurstConverter());
            converters.Add(new ParticleSystemConverter(particleSystemToWrite, settings));
            converters.Add(new MinMaxCurveConverter(settings.CurveConverterSettings));
            converters.Add(new MinMaxGradientConverter(settings.GradientConverterSettings));

            // converters.Add(new BurstConverter(settings));
            foreach (var enumConverter in GetEnumConverters())
            {
                converters.Add(enumConverter);
            }

            return converters;
        }

        public static List<JsonConverter> GetParticlesConvertersRead(ParticlesConverterSettings settings)
        {
            return GetAllConverters(settings, null);
        }


        public static List<JsonConverter> GetParticlesConvertersWrite(
            ParticleSystem particleSystem,
            ParticlesConverterSettings settings)
        {
            return GetAllConverters(settings, particleSystem);
        }

        public static List<JsonConverter> GetEnumConverters()
        {
            var converters = new List<JsonConverter>();
            converters.Add(new CustomIntConverter());
            converters.Add(new FloatEnumConverter<WeightedMode>());
            converters.Add(new FloatEnumConverter<SimulationMode2D>());
            converters.Add(new FloatEnumConverter<ParticleSystemSimulationSpace>());
            converters.Add(new FloatEnumConverter<ParticleSystemScalingMode>());
            converters.Add(new FloatEnumConverter<ParticleSystemEmitterVelocityMode>());
            converters.Add(new FloatEnumConverter<ParticleSystemStopAction>());
            converters.Add(new FloatEnumConverter<ParticleSystemRingBufferMode>());
            converters.Add(new FloatEnumConverter<ParticleSystemCullingMode>());
            converters.Add(new FloatEnumConverter<ParticleSystemEmissionType>());
            converters.Add(new FloatEnumConverter<ParticleSystemShapeType>());
            converters.Add(new FloatEnumConverter<ParticleSystemShapeMultiModeValue>());
            converters.Add(new FloatEnumConverter<ParticleSystemMeshShapeType>());
            converters.Add(new FloatEnumConverter<ParticleSystemInheritVelocityMode>());
            converters.Add(new FloatEnumConverter<ParticleSystemGameObjectFilter>());
            converters.Add(new FloatEnumConverter<ParticleSystemNoiseQuality>());
            converters.Add(new FloatEnumConverter<ParticleSystemCollisionType>());
            converters.Add(new FloatEnumConverter<ParticleSystemCollisionMode>());
            converters.Add(new FloatEnumConverter<ParticleSystemOverlapAction>());
            converters.Add(new FloatEnumConverter<ParticleSystemColliderQueryMode>());
            converters.Add(new FloatEnumConverter<ParticleSystemAnimationMode>());
            converters.Add(new FloatEnumConverter<ParticleSystemAnimationTimeMode>());
            converters.Add(new FloatEnumConverter<ParticleSystemAnimationType>());
            converters.Add(new FloatEnumConverter<ParticleSystemAnimationRowMode>());
            converters.Add(new FloatEnumConverter<UVChannelFlags>());
            converters.Add(new FloatEnumConverter<ParticleSystemTrailMode>());
            converters.Add(new FloatEnumConverter<ParticleSystemTrailTextureMode>());
            converters.Add(new FloatEnumConverter<ParticleSystemCollisionQuality>());
            return converters;
        }

        public static List<JsonConverter> GetDefaultParticlesConverters(MaterialConverter materialConverter)
        {
            var minMaxCurveConverterSettings = new MinMaxCurveConverterSettings(MinMaxCurveConvertMode.Default);
            var minMaxGradientSettings = new MinMaxGradientSettings(MinMaxGradientConvertMode.AllToGradient);


            var particleSystemConverterSettings =
                new ParticlesConverterSettings(minMaxCurveConverterSettings, minMaxGradientSettings, materialConverter);

            var converters = GetAllConverters(particleSystemConverterSettings, null);
            
            return converters;
        }
    }
}