using System.Collections.Generic;
using NNParticleSystemGenerator.DataSetGenerator.Editor;
using UnityEditor;
using UnityEngine;

namespace NNParticleSystemGenerator.Editor
{
    public class ColorGroupFormParticleTagsSelectorType : ParticleTagsSelectorType
    {
        private EnumSelectorEditorDrawer<ParticleForm> _particleFormSelector;
        private EnumSelectorEditorDrawer<ParticleColorGroup> _particleColorGroupSelector;
        private int _generateCountPerColorGroup = 2;

        public ColorGroupFormParticleTagsSelectorType()
        {
            _particleFormSelector = new EnumSelectorEditorDrawer<ParticleForm>("Key_ColorGroupFormParticleTagsSelectorType_ParticleForm");
            _particleColorGroupSelector = new EnumSelectorEditorDrawer<ParticleColorGroup>("Key_ColorGroupFormParticleTagsSelectorType_ParticleColorGroup");
        }

        public override void Draw()
        {
            GUILayout.BeginHorizontal("box");

            _generateCountPerColorGroup =
                EditorGUILayout.IntField("Generate count", _generateCountPerColorGroup);
            _particleFormSelector.OnGUI();
            _particleColorGroupSelector.OnGUI();

            GUILayout.EndHorizontal();
        }

        public override List<ParticleTags> GetTagsToGenerate()
        {
            var selectedTags = new List<ParticleTags>();
            foreach (var colorGroup in _particleColorGroupSelector.GetSelected())
            {
                foreach (var form in _particleFormSelector.GetSelected())
                {
                    var tag = new ParticleTags
                    {
                        form = form,
                        colorGroup = colorGroup,
                    };
                    for (var i = 0; i < _generateCountPerColorGroup; i++)
                        selectedTags.Add(tag);
                }
            }

            return selectedTags;
        }

        public override TagsProvidedType GetTagsProvidedType()
        {
            return TagsProvidedType.Form_Element;
        }

        public override void SaveEditorPrefs()
        {
            _particleColorGroupSelector.SaveEditorPrefs();
            _particleFormSelector.SaveEditorPrefs();
        }

        public override void LoadEditorPrefs()
        {
            _particleColorGroupSelector.LoadEditorPrefs();
            _particleFormSelector.LoadEditorPrefs();
        }
    }
}