using SmartAttributes.InspectorButton;
using SmartAttributes.MultiDraft.Attributes;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace NNParticleSystemGenerator.DataSetGenerator.Editor
{
    [InspectorButtonClass]
    public class TagsEditor : MonoBehaviour
    {
        [SerializeField, Required] private ParticleSpawner particleSpawner;
        [SerializeField, Required] private DatasetParticles datasetParticles;
        [SerializeField, Required] private Transform spawnParent;

        [SerializeField] private int currentParticleIndex;

        [SerializeField] private ParticleTags tags;

        [SerializeField, Disabled] private ParticleSystem _currentPrefabParticleSystem;
        [SerializeField, Disabled] private ParticleSystem _spawnedParticleSystem;

        private GUIStyle _guiStyle;
        public ParticleTags Tags => tags;


        [InspectorButton("SaveTags")]
        public void SaveTags()
        {
            Undo.RecordObject(datasetParticles, "Save Tags");
            datasetParticles.ParsedDataset[currentParticleIndex].tags = tags;
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(datasetParticles);
#endif
        }

        public void SaveTagForm(ParticleForm form)
        {
            Undo.RecordObject(datasetParticles, "Save form");
            tags.form = form;
            datasetParticles.ParsedDataset[currentParticleIndex].tags = tags;
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(datasetParticles);
#endif
        }

        public void SaveTagElement(ParticleElement element)
        {
            Undo.RecordObject(datasetParticles, "Save element");
            tags.element = element;
            datasetParticles.ParsedDataset[currentParticleIndex].tags = tags;
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(datasetParticles);
#endif
        }

        public void SaveTagColorGroup(ParticleColorGroup obj)
        {
            Undo.RecordObject(datasetParticles, "Save color group");
            tags.colorGroup = obj;
            datasetParticles.ParsedDataset[currentParticleIndex].tags = tags;

#if UNITY_EDITOR

            UnityEditor.EditorUtility.SetDirty(datasetParticles);
#endif
        }


        [InspectorButton("NextButton")]
        public void NextButton()
        {
            SaveTags();
            currentParticleIndex++;
            currentParticleIndex = currentParticleIndex % datasetParticles.ParsedDataset.Count;
            SpawnParticle(currentParticleIndex);
        }

        [InspectorButton("PrevButton")]
        public void PrevButton()
        {
            SaveTags();
            currentParticleIndex--;
            currentParticleIndex = (datasetParticles.ParsedDataset.Count + currentParticleIndex) %
                                   datasetParticles.ParsedDataset.Count;
            SpawnParticle(currentParticleIndex);
        }

        [InspectorButton("RespawnCurrent")]
        public void RespawnCurrent()
        {
            SaveTags();
            SpawnParticle(currentParticleIndex);
        }


        [InspectorButton("Test Auto Color Set")]
        public void TestAutoColorSet()
        {
            foreach (var particleTagPair in datasetParticles.ParsedDataset)
            {
                var color = ParticleColorAnalyzer.GetAverageColor(particleTagPair.particleSystem);
                particleTagPair.tags.color = color;
            }
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(datasetParticles);
#endif
        }

        [InspectorButton("ALL Set color group by color")]
        public void TestAllSetColorGroupByColor()
        {
            foreach (var particleTagPair in datasetParticles.ParsedDataset)
            {
                particleTagPair.tags.colorGroup = ParticleColorAnalyzer.ClassifyColorGroup(particleTagPair.tags.color);
            }
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(datasetParticles);
#endif
        }


        private void SpawnParticle(int index)
        {
            var particleTagPair = datasetParticles.ParsedDataset[index];
            var particle = particleTagPair.particleSystem;
            _currentPrefabParticleSystem = particle;
            DestroyChildren(spawnParent);

            var instance = particleSpawner.SpawnParticle(index, datasetParticles, spawnParent);
            instance.transform.position = Vector3.zero;
            _spawnedParticleSystem = instance;

            DestroyChildren(instance.transform);

            Selection.SetActiveObjectWithContext(instance, instance);
            tags = particleTagPair.tags;
        }

        private void DestroyChildren(Transform parent)
        {
            var childCount = parent.childCount;
            for (var i = 0; i < childCount; i++)
            {
                DestroyImmediate(parent.GetChild(0).gameObject);
            }
        }

        private void OnDrawGizmos()
        {
            var current = datasetParticles.ParsedDataset[currentParticleIndex];
            var color = current.tags.color;
            color.a = 1f;

            Vector3 labelPosition = transform.position + Vector3.up * 2f;
#if UNITY_EDITOR

            if (_guiStyle == null)
            {
                _guiStyle = new GUIStyle();
                _guiStyle.fontSize = 24;
                _guiStyle.fontStyle = FontStyle.Bold;
            }

            _guiStyle.normal.textColor = color;
            Handles.Label(labelPosition, current.tags.colorGroup.ToString(), _guiStyle);
            Handles.Label(labelPosition - Vector3.up * 6f, current.tags.form.ToString(), _guiStyle);
#endif
        }
    }
}