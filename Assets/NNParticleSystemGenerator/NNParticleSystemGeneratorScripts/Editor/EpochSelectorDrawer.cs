using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NNParticleSystemGenerator.Editor
{
    public class EpochSelectorDrawer : IEditorPrefsSaveLoad
    {
        private List<int> intList = new List<int>();
        private const string EditorPrefsKey = "EpochSelector_IntList";
       
        public void OnGUI()
        {
            EditorGUILayout.Space(10);
            GUILayout.Label("Epoch Selector", EditorStyles.boldLabel);

            GUIStyle elementStyle = new GUIStyle(GUI.skin.box);
            elementStyle.margin = new RectOffset(0, 0, 0, 5);

            // Draw the list of integers
            for (int i = 0; i < intList.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField("Epoch " + i, GUILayout.Width(80));
                intList[i] = EditorGUILayout.IntField(intList[i], GUILayout.ExpandWidth(true));

                if (GUILayout.Button("-", GUILayout.Width(30)))
                {
                    intList.RemoveAt(i);
                    GUI.FocusControl(null); // Deselect any control to prevent errors
                    EditorGUILayout.EndHorizontal();
                    break;
                }

                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(10);

            // Button to add a new integer to the list
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Epoch", GUILayout.Width(100)))
            {
                intList.Add(0);
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
        
        public List<int> GetIntList() => intList;
        

        private List<int> StringToIntList(string serializedList)
        {
            // Deserialize the string to a list of integers
            List<int> result = new List<int>();
            string[] elements = serializedList.Split(',');
            foreach (string element in elements)
            {
                int value;
                if (int.TryParse(element, out value))
                {
                    result.Add(value);
                }
            }
            return result;
        }

        private string IntListToString(List<int> list)
        {
            // Serialize the list of integers to a string
            return string.Join(",", list);
        }

        public void SaveEditorPrefs()
        {
            // Convert the list to a serialized string and save it to EditorPrefs
            string serializedList = IntListToString(intList);
            EditorPrefs.SetString(EditorPrefsKey, serializedList);
        }

        public void LoadEditorPrefs()
        {
            // Load the serialized list from EditorPrefs
            string serializedList = EditorPrefs.GetString(EditorPrefsKey);
            intList = new List<int>(StringToIntList(serializedList));
        }
    }
}