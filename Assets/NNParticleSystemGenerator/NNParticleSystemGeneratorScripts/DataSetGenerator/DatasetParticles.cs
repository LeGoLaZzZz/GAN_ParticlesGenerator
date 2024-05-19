using System;
using System.Collections.Generic;
using System.Linq;
using SmartAttributes.InspectorButton;
using SmartAttributes.MultiDraft.Attributes;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace NNParticleSystemGenerator.DataSetGenerator.Editor
{
    [InspectorButtonClass]
    [CreateAssetMenu(fileName = "DatasetParticles", menuName = "PSNN/DatasetParticles", order = 0)]
    public class DatasetParticles : ScriptableObject
    {
        [SerializeField, TextArea(5,20)] private string comment;
        [SerializeField] private List<ParticleSystem> particlesAssets;
        [SerializeField] private List<GameObject> particlesGameObjects;
        [SerializeField] private List<ParticleTagPair> datasetParticles;
        public List<ParticleSystem> ParsedDatasetParticles => datasetParticles.Select(p => p.particleSystem).ToList();
        public List<ParticleTagPair> ParsedDataset => datasetParticles;

        public void UpdateTags(int particleIndex, ParticleTags newTags)
        {
            datasetParticles[particleIndex].tags = newTags;
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        [InspectorButton("ParseDataset")]
        public void ParseDataset()
        {
            var particles = GetAllParticlesWithChildren();
            datasetParticles = new List<ParticleTagPair>();
            foreach (var particle in particles)
            {
                var particleTagPair = new ParticleTagPair();
                particleTagPair.particleSystem = particle;
                datasetParticles.Add(particleTagPair);
            }
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        [InspectorButton("RemoveCustomData")]
        public void RemoveCustomData()
        {
            for (var index = datasetParticles.Count - 1; index >= 0; index--)
            {
                var particleTagPair = datasetParticles[index];
                var particle = particleTagPair.particleSystem;
                if (particle.customData.enabled)
                {
                    datasetParticles.RemoveAt(index);
                }
            }
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        [InspectorButton("RemoveEmptyEmission")]
        public void RemoveEmptyEmission()
        {
            for (var index = datasetParticles.Count - 1; index >= 0; index--)
            {
                var particleTagPair = datasetParticles[index];
                var particle = particleTagPair.particleSystem;
                if (IsEmptyEmission(particle))
                {
                    datasetParticles.RemoveAt(index);
                }
            }
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        [InspectorButton("RemoveDisabledRenderer")]
        public void RemoveDisabledRenderer()
        {
            for (var index = datasetParticles.Count - 1; index >= 0; index--)
            {
                var particleTagPair = datasetParticles[index];
                var particle = particleTagPair.particleSystem;
                if (!particle.GetComponent<Renderer>().enabled)
                {
                    datasetParticles.RemoveAt(index);
                }
            }
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        private bool IsEmptyEmission(ParticleSystem ps)
        {
            if (!ps.emission.enabled) return true;

            var isRateOverDistanceEmpty = ps.emission.rateOverDistance.Evaluate(0.5f) == 0;
            var isRateOverTimeEmpty = ps.emission.rateOverTime.Evaluate(0.5f) == 0;
            return isRateOverDistanceEmpty && isRateOverTimeEmpty;
        }

        private List<ParticleSystem> GetAllParticlesWithChildren()
        {
            var returnList = new List<ParticleSystem>();
            foreach (var particlesAsset in particlesAssets)
            {
                ParticleSystem[] psArray = particlesAsset.GetComponentsInChildren<ParticleSystem>(true);
                foreach (var ps in psArray)
                {
                    returnList.Add(ps);
                }
            }

            foreach (var particlesGameObject in particlesGameObjects)
            {
                ParticleSystem[] psArray = particlesGameObject.GetComponentsInChildren<ParticleSystem>(true);
                foreach (var ps in psArray)
                {
                    returnList.Add(ps);
                }
            }

            return returnList;
        }
    }


    [Serializable]
    public class ParticleTagPair
    {
        public ParticleTags tags;
        public ParticleSystem particleSystem;
    }

    [Serializable]
    public class TestParticleTagsObject : Object
    {
        public ParticleTags tags;
    }

    [Serializable]
    public struct ParticleTags
    {
        public ParticleForm form;
        public ParticleElement element;
        public ParticleColorGroup colorGroup;
        public Color color;
        public bool forceRemoveFromDataset;
    }

    public enum ParticleForm
    {
        NoneRandom = 0,
        Smoke = 1,
        ArtIcon = 2,
        BluredCircles = 3,
        ThinLines = 4,
        Rays = 5,
        SharpCircles = 6,
        StarsSquares = 7
    }


    public enum ParticleElement
    {
        NoneRandom = 0,
        Fire = 1,
        Water = 2,
        Air = 3,
        Earth = 4,
        Lightning = 5,
        Ice = 6,
        Poison = 7,
        Light = 8,
        Magic = 9,
    }

    public enum ParticleColorGroup
    {
        Orange_Red_Yellow = 0,
        Green = 1,
        Blue_White = 2,
        Purple = 3,
    }
}