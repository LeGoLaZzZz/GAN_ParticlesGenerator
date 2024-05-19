using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Exception = System.Exception;

namespace NNParticleSystemGenerator.Editor
{
    public class EnumSelectorEditorDrawer<T> : IEditorPrefsSaveLoad where T : Enum
    {
        private List<T> selectedEnumList = new List<T>();
        private string editorPrefsKey;

        public EnumSelectorEditorDrawer(string prefsKey)
        {
            editorPrefsKey = prefsKey;
            LoadEditorPrefs();
        }

        public List<T> GetSelected() => selectedEnumList;

        public void OnGUI()
        {
            GUILayout.BeginVertical();
            GUILayout.Label($"Select {typeof(T).Name} ", EditorStyles.boldLabel);

            foreach (T enumValue in Enum.GetValues(typeof(T)))
            {
                bool isSelected = selectedEnumList.Contains(enumValue);
                bool newSelected = EditorGUILayout.ToggleLeft(enumValue.ToString(), isSelected);
                if (newSelected != isSelected)
                {
                    if (newSelected)
                        selectedEnumList.Add(enumValue);
                    else
                        selectedEnumList.Remove(enumValue);
                }
            }

            GUILayout.EndVertical();
        }

        public void SaveEditorPrefs()
        {
            string serializedList = string.Join(",", selectedEnumList);
            EditorPrefs.SetString(editorPrefsKey, serializedList);
        }

        public void LoadEditorPrefs()
        {
            if (EditorPrefs.HasKey(editorPrefsKey))
            {
                string serializedList = EditorPrefs.GetString(editorPrefsKey);
                if (string.IsNullOrEmpty(serializedList)) return;

                string[] enumNames = serializedList.Split(',');
                selectedEnumList.Clear();
                foreach (string enumName in enumNames)
                {
                    try
                    {
                        T enumValue = (T)Enum.Parse(typeof(T), enumName);
                        selectedEnumList.Add(enumValue);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                   
                }
            }
        }
    }
}