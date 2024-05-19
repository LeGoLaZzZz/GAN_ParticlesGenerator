using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NNParticleSystemGenerator.Editor;
using UnityEditor;
using UnityEngine;

namespace NNParticleSystemGenerator.DataSetGenerator.Editor
{
    public class PSNN_DatasetGenerator : EditorWindow
    {
        [MenuItem("PSNN_Tools/PSNN_DatasetGenerator")]
        private static void ShowWindow()
        {
            var window = GetWindow<PSNN_DatasetGenerator>();
            window.titleContent = new GUIContent("PSNN_DatasetGenerator");
            window.Show();
        }

        private string datasetName = "";
        private string saveDatasetDirectoryPath = "";
        private string directoryPath = "";
        private List<ParticleSystem> particleSystems = new List<ParticleSystem>();

        private ParticleSystemConverterSettings_Drawer _settingsDrawer;
        private Vector2 scrollPosition;
        private DatasetParticles _datasetParticles;

        private ParticlesDataSet _particlesDataSet;
        private EnumSelectorEditorDrawer<ParticleForm> _particleFormSelector;
        private bool _useOnlySelectedForms;

        private void OnEnable()
        {
            _particleFormSelector = new EnumSelectorEditorDrawer<ParticleForm>("PSNN_DatasetGenerator_particleFormSelector");
            _settingsDrawer = new ParticleSystemConverterSettings_Drawer("PSNN_DatasetGenerator");
            
            _settingsDrawer.LoadEditorPrefs();
            _particleFormSelector.LoadEditorPrefs();
        }

        private void OnDisable()
        {
            _settingsDrawer.SaveEditorPrefs();
            _particleFormSelector.SaveEditorPrefs();
        }

        private void OnGUI()
        {
            EditorHelpers.DrawScriptField(this);

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);


            DrawDatasetParticlesFinderGUI();

            datasetName = EditorGUILayout.TextField("datasetName: ", datasetName);
            saveDatasetDirectoryPath = EditorGUILayout.TextField("save to path: ", saveDatasetDirectoryPath);

            _settingsDrawer.Draw();


            GUILayout.Space(10);

            GUILayout.Label("Particle Systems Found:", EditorStyles.boldLabel);

            foreach (var ps in particleSystems)
            {
                EditorGUILayout.ObjectField(ps, typeof(ParticleSystem), true);
            }

            if (GUILayout.Button("Generate Dataset"))
            {
                GenerateDatasetFromConfig();
            }

            _useOnlySelectedForms = EditorGUILayout.Toggle("Use only selected forms", _useOnlySelectedForms);
            if(_useOnlySelectedForms) _particleFormSelector.OnGUI();

            GUILayout.EndScrollView();
        }


        private void GenerateDatasetFromConfig()
        {
            if (!_datasetParticles.ParsedDataset.Any()) return;

            _particlesDataSet = new ParticlesDataSet();
            var selectedForms = _particleFormSelector.GetSelected();

            var settings =
                _settingsDrawer.GenerateSettings(
                    new DatasetMaterialConverter(_datasetParticles.ParsedDatasetParticles));
            foreach (var tagPair in _datasetParticles.ParsedDataset)
            {
                if (tagPair.tags.forceRemoveFromDataset) continue;
                if(_useOnlySelectedForms && !selectedForms.Contains(tagPair.tags.form)) continue;

                var psJson = SerializeHelpers.SerializeParticleSystemToJson(tagPair.particleSystem, settings);
                // EditorHelpers.SaveStringToFile(psJson, saveDatasetDirectoryPath + "/" + datasetName,
                // datasetName + "_" + (i++) + ".json");
                _particlesDataSet.particlesJson.Add(psJson);

                var tagsJson = JsonConvert.SerializeObject(tagPair.tags);
                // EditorHelpers.SaveStringToFile(tagsJson, saveDatasetDirectoryPath + "/" + datasetName,
                // datasetName + "_" + (i++) + ".json");
                _particlesDataSet.tagsJson.Add(tagsJson);
            }

            var combinedJson = JsonConvert.SerializeObject(_particlesDataSet);
            EditorHelpers.SaveStringToFile(combinedJson, saveDatasetDirectoryPath + "/" + datasetName,
                datasetName + "_FULL.json");
            AssetDatabase.Refresh();
        }

        private void DrawDatasetParticlesFinderGUI()
        {
            _datasetParticles =
                EditorGUILayout.ObjectField(_datasetParticles, typeof(DatasetParticles), false) as DatasetParticles;
        }

        private void DrawPathParticlesFinderGUI()
        {
            GUILayout.Label("Enter Directory Path:", EditorStyles.boldLabel);
            directoryPath = EditorGUILayout.TextField(directoryPath);
            if (GUILayout.Button("Find Particle Systems"))
            {
                FindParticleSystemsInDirectory();
            }

            GUILayout.Space(10);

            GUILayout.Label("Particle Systems Found:", EditorStyles.boldLabel);

            foreach (var ps in particleSystems)
            {
                EditorGUILayout.ObjectField(ps, typeof(ParticleSystem), true);
            }
        }

        private void FindParticleSystemsInDirectory()
        {
            if (string.IsNullOrEmpty(directoryPath))
            {
                Debug.LogError("Directory path is empty.");
                return;
            }

            particleSystems.Clear();

            DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
            FileInfo[] files = directoryInfo.GetFiles("*.prefab", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                string assetPath = RemovePathBeforeAssets(file.FullName);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

                ParticleSystem[] psArray = prefab.GetComponentsInChildren<ParticleSystem>(true);

                foreach (var ps in psArray)
                {
                    particleSystems.Add(ps);
                }
            }

            Repaint();


            string RemovePathBeforeAssets(string fullPath)
            {
                int index = fullPath.IndexOf("Assets");
                if (index >= 0)
                {
                    return fullPath.Substring(index);
                }
                else
                {
                    return fullPath; // Return the original path if "Assets/" is not found.
                }
            }
        }
    }

    [System.Serializable]
    public class ParticlesDataSet
    {
        public List<string> particlesJson = new List<string>();
        public List<string> tagsJson = new List<string>();
    }
}