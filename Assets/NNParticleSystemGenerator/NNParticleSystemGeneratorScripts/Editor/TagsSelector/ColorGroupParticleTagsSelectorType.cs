using System.Collections.Generic;
using NNParticleSystemGenerator.DataSetGenerator.Editor;
using UnityEditor;
using UnityEngine;

namespace NNParticleSystemGenerator.Editor
{
    public class ColorGroupParticleTagsSelectorType : ParticleTagsSelectorType
    {
        private EnumSelectorEditorDrawer<ParticleColorGroup> _particleColorGroupSelector;
        private int _generateCountPerColorGroup = 5;
        private const string GenerateCountPerColorGroupKey = "GenerateCountPerColorGroup";

        public ColorGroupParticleTagsSelectorType()
        {
            _particleColorGroupSelector =
                new EnumSelectorEditorDrawer<ParticleColorGroup>("Key_particleColorGroupSelector");
        }

        public override void Draw()
        {
            GUILayout.BeginHorizontal("box");

            _generateCountPerColorGroup =
                EditorGUILayout.IntField("Generate count per color group", _generateCountPerColorGroup);
            _particleColorGroupSelector.OnGUI();

            GUILayout.EndHorizontal();
        }

        public override List<ParticleTags> GetTagsToGenerate()
        {
            var selectedTags = new List<ParticleTags>();
            foreach (var colorGroup in _particleColorGroupSelector.GetSelected())
            {
                var tag = new ParticleTags
                {
                    colorGroup = colorGroup,
                };

                for (var i = 0; i < _generateCountPerColorGroup; i++)
                    selectedTags.Add(tag);
            }

            return selectedTags;
        }

        public override TagsProvidedType GetTagsProvidedType()
        {
            return TagsProvidedType.Color_Group;
        }

        public override void SaveEditorPrefs()
        {
            _particleColorGroupSelector.SaveEditorPrefs();
            EditorPrefs.SetInt(GenerateCountPerColorGroupKey, _generateCountPerColorGroup);
        }

        public override void LoadEditorPrefs()
        {
            _particleColorGroupSelector.LoadEditorPrefs();
            _generateCountPerColorGroup = EditorPrefs.GetInt(GenerateCountPerColorGroupKey, 5);
        }
    }
}