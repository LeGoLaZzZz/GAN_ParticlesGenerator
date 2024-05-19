using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NNParticleSystemGenerator.Editor
{
    public interface IParticleSystemAssetsCreator
    {
        public ParticleSystem CreateAssetParticle(string json, string savePath, string assetName);

        public List<ParticleSystem> CreateListAssetParticles(List<string> jsons, string savePath,
            List<string> assetNames);
        
        public IEnumerator CreateListAssetParticlesRoutine(List<string> jsons, string savePath,
            List<string> assetNames);
    }

    public class ParticleSystemAssetsCreator : IParticleSystemAssetsCreator
    {
        private readonly IParticleSystemJsonFiller _filler;

        public ParticleSystemAssetsCreator(IParticleSystemJsonFiller filler)
        {
            _filler = filler;
        }

        public ParticleSystem CreateAssetParticle(string json, string savePath, string assetName)
        {
            var particleSystem = EditorHelpers.CreateParticleSystemPrefab(savePath, assetName);

            _filler.FillParticleFromJson(json, particleSystem);

            AssetDatabase.SaveAssetIfDirty(particleSystem);
            AssetDatabase.Refresh();
            Selection.activeGameObject = particleSystem.gameObject;
            AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<GameObject>(
                PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(particleSystem.gameObject)));
            SceneView.FrameLastActiveSceneView();

            return particleSystem;
        }

        public List<ParticleSystem> CreateListAssetParticles(List<string> jsons, string savePath,
            List<string> assetNames)
        {
            var resultParticles = new List<ParticleSystem>();

            for (var i = 0; i < jsons.Count; i++)
            {
                var particleSystem = EditorHelpers.CreateParticleSystemPrefab(savePath, assetNames[i]);

                _filler.FillParticleFromJson(jsons[i], particleSystem);
                resultParticles.Add(particleSystem);
            }

            foreach (var resultParticle in resultParticles)
            {
                AssetDatabase.SaveAssetIfDirty(resultParticle);
            }

            AssetDatabase.Refresh();

            return resultParticles;
        }


        public IEnumerator CreateListAssetParticlesRoutine(List<string> jsons, string savePath, List<string> assetNames)
        {
            var resultParticles = new List<ParticleSystem>();

            for (var i = 0; i < jsons.Count; i++)
            {
                var particleSystem = EditorHelpers.CreateParticleSystemPrefab(savePath, assetNames[i]);

                Debug.Log($"start fill particle {assetNames[i]}");
                yield return null;
                
                _filler.FillParticleFromJson(jsons[i], particleSystem);
                resultParticles.Add(particleSystem);
            }

            for (var index = 0; index < resultParticles.Count; index++)
            {
                var resultParticle = resultParticles[index];
                Debug.Log($"SaveAssetIfDirty {assetNames[index]}");
                yield return null;
                AssetDatabase.SaveAssetIfDirty(resultParticle);
            }

            Debug.Log($"AssetDatabase.Refresh");
            yield return null;
            AssetDatabase.Refresh();
        }
    }
}