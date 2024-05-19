using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NNParticleSystemGenerator.DataSetGenerator.Editor
{
    [CustomEditor(typeof(TagsEditor))]
    public class TagsEditorEditor : UnityEditor.Editor
    {
        private ParticleElement[] elementValues;
        private ParticleForm[] formValues;
        private ParticleColorGroup[] colorGroupValues;
        private TagsEditor _tagsEditor;
        private Color _tempBackgroundColor;

        private void OnEnable()
        {
            _tagsEditor = (TagsEditor)target;
            elementValues = (ParticleElement[])Enum.GetValues(typeof(ParticleElement));
            formValues = (ParticleForm[])Enum.GetValues(typeof(ParticleForm));
            colorGroupValues = (ParticleColorGroup[])Enum.GetValues(typeof(ParticleColorGroup));
        }

        private void OnDisable()
        {
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Space(50);
            NextPrevButtons();
            GUILayout.Space(10);
            
            GUILayout.BeginVertical("box");
            
            GUILayout.Label("Tags selector", EditorStyles.boldLabel);
            ColorGroupButtons();
            GUILayout.Space(10);

            FormsButtons();
            
            GUILayout.EndVertical();
            
            GUILayout.Space(20);

            if (GUILayout.Button("Save Tags"))
            {
                _tagsEditor.SaveTags();
            }

            if (GUILayout.Button("Respawn"))
            {
                _tagsEditor.RespawnCurrent();
            }
            
            _tempBackgroundColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.red;
                
            if (GUILayout.Button("Auto color set",  GUILayout.Width(200),  GUILayout.Height(50)))
            {
                _tagsEditor.TestAutoColorSet();
            }
            
            if (GUILayout.Button("Auto set color_group by color",  GUILayout.Width(200),  GUILayout.Height(50)))
            {
                _tagsEditor.TestAllSetColorGroupByColor();
            }
                
            GUI.backgroundColor = _tempBackgroundColor;

            GUILayout.Space(10);
        }

        private void NextPrevButtons()
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Prev", GUILayout.Height(50)))
            {
                _tagsEditor.PrevButton();
            }

            if (GUILayout.Button("Next", GUILayout.Height(50)))
            {
                _tagsEditor.NextButton();
            }

            GUILayout.EndHorizontal();
        }

        private void ColorGroupButtons()
        {
            GUILayout.Label("Particle Color Groups", EditorStyles.boldLabel);
            TableButtons(colorGroupValues.ToList(), _tagsEditor.SaveTagColorGroup, _tagsEditor.Tags.colorGroup);
        }
        private void FormsButtons()
        {
            GUILayout.Space(10);
            GUILayout.Label("Particle Elements", EditorStyles.boldLabel);
            TableButtons(elementValues.ToList(), _tagsEditor.SaveTagElement, _tagsEditor.Tags.element);

            GUILayout.Space(20);
            GUILayout.Label("Particle Forms", EditorStyles.boldLabel);
            TableButtons(formValues.ToList(), _tagsEditor.SaveTagForm , _tagsEditor.Tags.form);
        }

        private void TableButtons<T>(List<T> values, Action<T> saveAction, T currentValue, int maxButtonsInRow = 4)
        {
            GUILayout.BeginHorizontal();
            int buttonsInRow = 0;

            foreach (var enumValue in values)
            {
                if (buttonsInRow >= maxButtonsInRow)
                {
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    buttonsInRow = 0;
                }

                if (Equals(enumValue, currentValue))
                {
                    _tempBackgroundColor = GUI.backgroundColor;
                    GUI.backgroundColor = Color.green;
                }
                
                if (GUILayout.Button(enumValue.ToString(),  GUILayout.Width(90)))
                {
                    saveAction(enumValue);
                }
                
                if (Equals(enumValue, currentValue))
                {
                    GUI.backgroundColor = _tempBackgroundColor;
                }

                buttonsInRow++;
            }

            GUILayout.EndHorizontal();
        }
    }
}