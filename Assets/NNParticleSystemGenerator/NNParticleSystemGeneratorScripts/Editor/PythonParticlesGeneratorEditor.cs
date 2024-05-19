using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NNParticleSystemGenerator.Editor
{
    public class PythonParticlesGeneratorEditor : IEditorPrefsSaveLoad
    {
        private readonly ParticleTagsEditorDrawer _particleTagsDrawer;
        private EpochSelectorDrawer _epochSelectorDrawer;

        private NnPythonParticlesGenerator _nnPythonParticlesGenerator;
        private ParticleTagsSelector _particleTagsSelector;

        private int _epochForSingleGeneration;


        public PythonParticlesGeneratorEditor(IParticleSystemAssetsCreator assetsCreator, ICustomEditorLogger logger)
        {
            _particleTagsDrawer = new ParticleTagsEditorDrawer("PythonParticlesGeneratorEditor");
            _particleTagsSelector = new ParticleTagsSelector();
            _nnPythonParticlesGenerator = new NnPythonParticlesGenerator(assetsCreator, logger, _particleTagsSelector);
            _epochSelectorDrawer = new EpochSelectorDrawer();
        }

        public void DrawPythonGenerateFile(string saveFolderPath)
        {
            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.Space(10);
            GUILayout.Label("NN Python Generate", EditorStyles.boldLabel);

            _particleTagsDrawer.Draw();

            EditorGUILayout.BeginHorizontal();
            _epochForSingleGeneration =
                EditorGUILayout.IntField("Epoch for single generation", _epochForSingleGeneration);

            if (GUILayout.Button("Python generate single"))
            {
                var tags = _particleTagsDrawer.GetTags();
                _nnPythonParticlesGenerator.GenerateParticle(_epochForSingleGeneration, tags, saveFolderPath);
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();


            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("NN Python Generate collection");
            
            DrawEpochSelector();
            
            _particleTagsSelector.Draw();

            if (GUILayout.Button("Python generate collection"))
            {
                _nnPythonParticlesGenerator.StartCollectionGeneration(_epochSelectorDrawer.GetIntList(),saveFolderPath);
            }

            if (GUILayout.Button("Stop collection routine"))
            {
                _nnPythonParticlesGenerator.StopCollectionGeneration();
            }

            if (GUILayout.Button("Stop collection routine and save"))
            {
                _nnPythonParticlesGenerator.StopAndSaveCollectionGeneration(saveFolderPath);
            }

            EditorGUILayout.EndVertical();
        }

        public void SaveEditorPrefs()
        {
            _particleTagsSelector.SaveEditorPrefs();
            _epochSelectorDrawer.SaveEditorPrefs();
        }

        public void LoadEditorPrefs()
        {
            _particleTagsSelector.LoadEditorPrefs();
            _epochSelectorDrawer.LoadEditorPrefs();
        }

        private void DrawEpochSelector()
        {
            GUILayout.Label("NN Python Generate", EditorStyles.boldLabel);
            _epochSelectorDrawer.OnGUI();
        }
    }
}