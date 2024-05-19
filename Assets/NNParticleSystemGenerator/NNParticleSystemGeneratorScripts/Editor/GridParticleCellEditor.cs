using System;
using System.Collections.Generic;
using System.Linq;
using NNParticleSystemGenerator.DataSetGenerator.Editor;
using UnityEditor;
using UnityEngine;

namespace NNParticleSystemGenerator.Editor
{
    [CustomEditor(typeof(GridParticleCell))] [CanEditMultipleObjects]
    public class GridParticleCellEditor : UnityEditor.Editor
    {
        private enum UpdateType
        {
            UpdateColorGroup = 0,
            UpdateForm = 1,
        }

        private GUIStyle _indexStyle;
        private GUIStyle _tagStyle;
        private UpdateType _currentUpdateType;

        private ParticleColorGroupStyle[] _colorStyles;
        private ParticleTags _particleTags;
        private ParticleTagsEditorDrawer _particleTagsEditorDrawer;
        private bool _showColorGroup;
        private bool _hideButton;

        private void OnEnable()
        {
            _particleTagsEditorDrawer = new ParticleTagsEditorDrawer("GridParticleCellEditor");
            _particleTagsEditorDrawer.LoadEditorPrefs();
            _showColorGroup = EditorPrefs.GetBool("GridParticleCellEditor_ShowColorGroup", true);
            _currentUpdateType = (UpdateType)EditorPrefs.GetInt("GridParticleCellEditor_UpdateType", 0);
        }

        private void OnDisable()
        {
            _particleTagsEditorDrawer.SaveEditorPrefs();
            EditorPrefs.SetBool("GridParticleCellEditor_ShowColorGroup", _showColorGroup);
            EditorPrefs.SetInt("GridParticleCellEditor_UpdateType", (int)_currentUpdateType);
        }


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            _particleTagsEditorDrawer.Draw();
            _showColorGroup = EditorGUILayout.Toggle("Show ColorGroup", _showColorGroup);
            _currentUpdateType = (UpdateType)EditorGUILayout.EnumPopup("Update Type", _currentUpdateType);
            _hideButton = EditorGUILayout.Toggle("Hide Button", _hideButton);
        }

        private void OnSceneGUI()
        {
            if (_colorStyles == null) InitializeGUIStyles();

            var cell = target as GridParticleCell;
            var numberOfButtons = 1;
            var color = Handles.color;
            color.a = 0.3f;
            Handles.color = color;

            Vector3 objPosition = cell.transform.position;
            float handleSize = HandleUtility.GetHandleSize(objPosition) * 0.1f;

            // Calculate the total width of the button group
            float totalWidth = handleSize * (numberOfButtons);

            // Calculate the starting position for the first button
            Vector3 firstButtonPosition = objPosition - Vector3.right * totalWidth;


            // Draw buttons
            for (int i = 0; i < numberOfButtons; i++)
            {
                Vector3 buttonPosition = firstButtonPosition + Vector3.right * handleSize * i;
                buttonPosition += Vector3.up * -5f;
                if (!_hideButton)
                {
                    if (Handles.Button(buttonPosition, Quaternion.identity, handleSize, handleSize * 2f,
                            Handles.DotHandleCap))
                    {
                        var particleTags = _particleTagsEditorDrawer.GetTags();
                        UpdateTags(cell, particleTags, _currentUpdateType);
                    }
                }

                if (_indexStyle == null)
                {
                    _indexStyle = new GUIStyle();
                    _indexStyle.fontSize = 8;
                    _indexStyle.fontStyle = FontStyle.Bold;
                    _indexStyle.normal.textColor = new Color(0f, 0f, 0f, 0.27f);
                }

                if (_tagStyle == null)
                {
                    _tagStyle = new GUIStyle();
                    _tagStyle.fontSize = 12;
                    _tagStyle.fontStyle = FontStyle.Bold;
                    _tagStyle.normal.textColor = new Color(1f, 1f, 1f, 0.52f);
                }

                Handles.Label(buttonPosition + Vector3.left * 5f, (cell.IndexInDataset).ToString(), _indexStyle);
                var tags = cell.GetTags();
                Handles.Label(buttonPosition, (tags.form).ToString(), _tagStyle);

                if (_showColorGroup)
                {
                    var particleColorGroup = tags.colorGroup;
                    var s = particleColorGroup.ToString();
                    s = s.Substring(0, Math.Min(6, s.Length));
                    Handles.Label(buttonPosition + Vector3.up * -5f, s, GetGUIStyle(particleColorGroup));
                }
            }
        }

        private GUIStyle GetGUIStyle(ParticleColorGroup colorGroup)
        {
            foreach (var particleColorGroupStyle in _colorStyles)
            {
                if (particleColorGroupStyle.ColorGroup == colorGroup)
                {
                    return particleColorGroupStyle.style;
                }
            }

            return null;
        }


        private void UpdateTags(GridParticleCell cell, ParticleTags tags, UpdateType updateType)
        {
            switch (updateType)
            {
                case UpdateType.UpdateColorGroup:
                    cell.UpdateColorGroup(tags.colorGroup);
                    Debug.Log($"ColorGroup changed to {tags.colorGroup}");
                    break;
                case UpdateType.UpdateForm:
                    cell.UpdateForm(tags.form);
                    Debug.Log($"Form changed to {tags.form}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(updateType), updateType, null);
            }
        }

        private void InitializeGUIStyles()
        {
            _colorStyles = new ParticleColorGroupStyle[4];

            // Style 1: Red background
            _colorStyles[0] = new ParticleColorGroupStyle();
            _colorStyles[0].ColorGroup = ParticleColorGroup.Orange_Red_Yellow;
            _colorStyles[0].style = new GUIStyle();
            _colorStyles[0].style.fontSize = 12;
            _colorStyles[0].style.fontStyle = FontStyle.Bold;
            _colorStyles[0].style.normal.textColor = new Color(1f, 0f, 0.07f, 0.52f);

            // Style 2: Green background

            _colorStyles[1] = new ParticleColorGroupStyle();
            _colorStyles[1].ColorGroup = ParticleColorGroup.Green;
            _colorStyles[1].style = new GUIStyle();
            _colorStyles[1].style.fontSize = 12;
            _colorStyles[1].style.fontStyle = FontStyle.Bold;
            _colorStyles[1].style.normal.textColor = new Color(0.07f, 1f, 0.07f, 0.52f);


            // Style 3: Blue background
            _colorStyles[2] = new ParticleColorGroupStyle();
            _colorStyles[2].ColorGroup = ParticleColorGroup.Blue_White;
            _colorStyles[2].style = new GUIStyle();
            _colorStyles[2].style.fontSize = 12;
            _colorStyles[2].style.fontStyle = FontStyle.Bold;
            _colorStyles[2].style.normal.textColor = new Color(0.07f, 0.07f, 1f, 0.52f);

            // Style 4: Purple background
            _colorStyles[3] = new ParticleColorGroupStyle();
            _colorStyles[3].ColorGroup = ParticleColorGroup.Purple;
            _colorStyles[3].style = new GUIStyle();
            _colorStyles[3].style.fontSize = 12;
            _colorStyles[3].style.fontStyle = FontStyle.Bold;
            _colorStyles[3].style.normal.textColor = new Color(1f, 0.07f, 1f, 0.52f);
        }

        private class ParticleColorGroupStyle
        {
            public ParticleColorGroup ColorGroup;
            public GUIStyle style;
        }
    }
}