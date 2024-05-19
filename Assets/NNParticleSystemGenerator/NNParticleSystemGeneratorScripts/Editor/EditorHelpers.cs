using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NNParticleSystemGenerator.Editor
{
    public class EditorHelpers
    {
        public static void DrawScriptField(ScriptableObject drawObject)
        {
            GUI.enabled = false;
            var script = MonoScript.FromScriptableObject(drawObject);
            EditorGUILayout.ObjectField(script, typeof(MonoScript), false);
            GUI.enabled = true;
        }

        public static ParticleSystem CreateParticleSystemPrefab(string savePath, string assetName)
        {
            var particleSystemObject = new GameObject("New ParticleSystem");
            particleSystemObject.AddComponent<ParticleSystem>();

            var prefabPath = savePath;
            

            if (!Directory.Exists(prefabPath))
            {
                string[] folders = prefabPath.Split('/', '\\');
                string lastFolder = folders[folders.Length - 1];
                string parentPath = Path.Combine(string.Join("/", folders, 0, folders.Length - 1));
                prefabPath = Path.Combine(parentPath, lastFolder);
                
                Debug.Log($"parentPath = {parentPath} lastFolder = {lastFolder}");

                var result = AssetDatabase.CreateFolder(parentPath, lastFolder);
                if (result == "")
                    throw new Exception($" Error creating folder: {prefabPath}");
                Debug.Log("Last folder created: " + prefabPath);
            }


            try
            {
                prefabPath = Path.Combine(prefabPath, assetName + ".prefab");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Debug.Log($"prefabPath = {prefabPath}");
                Debug.Log($"savePath = {savePath}");
                throw;
            }


            Debug.Log($"prefabPath = {prefabPath}");
            prefabPath = AssetDatabase.GenerateUniqueAssetPath(prefabPath);
            PrefabUtility.SaveAsPrefabAsset(particleSystemObject, prefabPath);
            GameObject.DestroyImmediate(particleSystemObject);

            AssetDatabase.Refresh();
            Debug.Log("Particle System Prefab created at: " + prefabPath);

            var loadAssetAtPath = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
            return loadAssetAtPath.GetComponent<ParticleSystem>();
        }

        public static string LoadTextFromFile(string absolutePath)
        {
            try
            {
                if (File.Exists(absolutePath))
                {
                    return File.ReadAllText(absolutePath);
                }
                else
                {
                    Debug.LogError("Файл не существует по указанному пути: " + absolutePath);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Ошибка чтения файла: " + e.Message);
            }

            return string.Empty;
        }

        public static void SaveStringToFile(string content, string directoryPath, string fileName)
        {
            // Проверяем, существует ли указанная директория. Если нет, создаем ее.
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Создаем полный путь к файлу, объединяя директорию и имя файла.
            string filePath = Path.Combine(directoryPath, fileName);

            try
            {
                // Записываем строку в файл.
                File.WriteAllText(filePath, content);
            }
            catch (Exception e)
            {
                Debug.LogError("Ошибка при записи в файл: " + e.Message);
                throw;
            }
        }
    }
}