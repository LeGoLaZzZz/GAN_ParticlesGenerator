using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NNParticleSystemGenerator.DataSetGenerator.Editor;
using Unity.EditorCoroutines.Editor;
using UnityEngine;

namespace NNParticleSystemGenerator.Editor
{
    public class NnPythonParticlesGenerator
    {
        private enum CollectionGenerationState
        {
            None,
            Started,
            GenerationsJsons,
            SavingPrefabs,
            Ended
        }

        public static string PSFromFile =>
            $@"{Application.dataPath}\..\NN_Particles_Generator_Python\NN_Scripts\output.json";

        private static string GetPythonFileToExecute() =>
            $@"{Application.dataPath}\..\NN_Particles_Generator_Python\NN_Scripts\GenerateFromFile.py";

        private readonly IParticleSystemAssetsCreator _assetsCreator;
        private readonly ICustomEditorLogger _logger;
        private readonly ITagsToGenerateProvider _tagsProvider;

        private EditorCoroutine _collectionRoutine;
        private List<string> _particlesJsons;
        private List<string> _particlesNames;
        private CollectionGenerationState _collectionGenerationState;

        public NnPythonParticlesGenerator(IParticleSystemAssetsCreator assetsCreator, ICustomEditorLogger logger,
            ITagsToGenerateProvider tagsProvider)
        {
            _tagsProvider = tagsProvider;
            _logger = logger;
            _assetsCreator = assetsCreator;
        }


        public ParticleSystem GenerateParticle(int epoch, ParticleTags tags, string savePath)
        {
            PythonParticlesGeneratorRunner.RunPython(epoch, tags, GetPythonFileToExecute());
            var fromFile = EditorHelpers.LoadTextFromFile(PSFromFile);
            var fileName = tags.form.ToString() + "_" + tags.element.ToString();
            return _assetsCreator.CreateAssetParticle(fromFile, savePath, fileName);
        }

        private IEnumerator GenerateCollectionRoutineMain(List<int> epochList, string pythonFileToExecute,
            string saveFolderPath,
            List<ParticleTags> tagsList, TagsProvidedType tagsProvidedType)
        {
            foreach (var i in epochList.ToArray())
            {
                Debug.Log($"Generating Epoch {i}");
                _logger.DebugLog($"Generating Epoch {i}");
                var epochFolder = Path.GetDirectoryName(saveFolderPath);
                epochFolder = Path.Combine(epochFolder, "Epoch" + i);
                yield return GenerateCollectionRoutine(i, pythonFileToExecute, epochFolder, tagsList,
                    tagsProvidedType);
            }
        }

        private IEnumerator GenerateCollectionRoutine(int epoch, string pythonFileToExecute, string saveFolderPath,
            List<ParticleTags> tagsList, TagsProvidedType tagsProvidedType)
        {
            ChangeState(CollectionGenerationState.Started);


            _particlesJsons = new List<string>();
            _particlesNames = GetSaveNames(tagsList, tagsProvidedType);

            ChangeState(CollectionGenerationState.GenerationsJsons);

            for (var index = 0; index < tagsList.Count; index++)
            {
                var tags = tagsList[index];
                var fileName = _particlesNames[index];

                Debug.Log($"Generating {fileName}");
                _logger.DebugLog($"Generating {fileName}");
                yield return null;

                PythonParticlesGeneratorRunner.RunPython(epoch, tags, pythonFileToExecute);
                var fromFile = EditorHelpers.LoadTextFromFile(PSFromFile);

                _particlesJsons.Add(fromFile);

                Debug.Log($"Generated {fileName}");
                _logger.DebugLog($"Generated {fileName}");
                yield return null;
                // yield return new WaitForSecondsRealtime(pauseBetweenGenerations);
            }

            ChangeState(CollectionGenerationState.SavingPrefabs);
            yield return null;
            // yield return new WaitForSecondsRealtime(pauseBetweenGenerations);

            yield return _assetsCreator.CreateListAssetParticlesRoutine(_particlesJsons, saveFolderPath,
                _particlesNames);
            ChangeState(CollectionGenerationState.Ended);
        }

        public void StartCollectionGeneration(List<int> epochList, string saveFolderPath)
        {
            _logger.ClearLogger();
            var particleTagsList = _tagsProvider.GetTagsToGenerate();
            var generateCollectionRoutine = GenerateCollectionRoutineMain(
                epochList,
                GetPythonFileToExecute(),
                saveFolderPath,
                particleTagsList,
                _tagsProvider.GetTagsProvidedType()
            );

            _collectionRoutine =
                EditorCoroutineUtility.StartCoroutineOwnerless(generateCollectionRoutine);
        }

        public void StopCollectionGeneration()
        {
            if (_collectionRoutine != null)
            {
                EditorCoroutineUtility.StopCoroutine(_collectionRoutine);
                _collectionRoutine = null;

                ChangeState(CollectionGenerationState.Ended);
                Debug.Log("Collection routine stopped");
            }
        }

        public void StopAndSaveCollectionGeneration(string saveFolderPath)
        {
            if (_collectionRoutine != null)
            {
                EditorCoroutineUtility.StopCoroutine(_collectionRoutine);
                _collectionRoutine = null;


                ChangeState(CollectionGenerationState.SavingPrefabs);
                _collectionRoutine = EditorCoroutineUtility.StartCoroutineOwnerless(
                    _assetsCreator.CreateListAssetParticlesRoutine(_particlesJsons, saveFolderPath, _particlesNames));
                ChangeState(CollectionGenerationState.Ended);

                Debug.Log("Collection routine stopped");
            }
        }

        private void ChangeState(CollectionGenerationState state)
        {
            _collectionGenerationState = state;
            Debug.Log($"State changed to {_collectionGenerationState}");
            _logger.DebugLog($"State changed to {_collectionGenerationState}");
        }

        private List<string> GetSaveNames(List<ParticleTags> tagsList, TagsProvidedType tagsProvidedType)
        {
            var saveNames = new List<string>();
            switch (tagsProvidedType)
            {
                case TagsProvidedType.Form_Element:
                    saveNames.AddRange(tagsList.Select(tags => tags.form + "_" + tags.element));
                    break;
                case TagsProvidedType.Color_Group:
                    saveNames.AddRange(tagsList.Select(tags => tags.colorGroup.ToString()));
                    break;
                case TagsProvidedType.Color_Group_Form:
                    saveNames.AddRange(tagsList.Select(tags => tags.colorGroup.ToString() + "_" + tags.form));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(tagsProvidedType), tagsProvidedType, null);
            }

            return saveNames;
        }
    }
}