using System;
using UnityEngine;

namespace NNParticleSystemGenerator.DataSetGenerator.Editor
{
    public class ParticleSpawner : MonoBehaviour
    {
        public Func<MaterialConverter, ParticlesConverterSettings> GetParticlesConverterSettingsFunc;

        [SerializeField] private bool spawnFromJson;

        public ParticleSystem SpawnParticle(int index, DatasetParticles datasetParticles, Transform spawnParent)
        {
            var particleTagPair = datasetParticles.ParsedDataset[index];
            var particle = particleTagPair.particleSystem;


            var instance = Instantiate(particle, spawnParent);
            instance.transform.position = Vector3.zero;

            DestroyChildren(instance.transform);

            if (spawnFromJson)
            {
                FillPsFromJson(instance, datasetParticles);
            }

            return instance;
        }

        private void DestroyChildren(Transform parent)
        {
            var childCount = parent.childCount;
            for (var i = 0; i < childCount; i++)
            {
                DestroyImmediate(parent.GetChild(0).gameObject);
            }
        }


        private void FillPsFromJson(ParticleSystem particleToFill, DatasetParticles datasetParticles)
        {
            if (GetParticlesConverterSettingsFunc == null)
            {
                Debug.LogError($"Try open ParticleSpawner in inspector first.");
                return;
            }
            MaterialConverter materialConverter = new DatasetMaterialConverter(datasetParticles.ParsedDatasetParticles);
            var particlesConverterSettings = GetParticlesConverterSettingsFunc(materialConverter);
            var psJson = SerializeHelpers.SerializeParticleSystemToJson(particleToFill, particlesConverterSettings);
            SerializeHelpers.ParseJsonToParticleSystem(particleToFill, psJson, particlesConverterSettings);
        }
    }
}