using System.Collections.Generic;
using NNParticleSystemGenerator.DataSetGenerator.Editor;
using UnityEngine;

namespace NNParticleSystemGenerator.Editor
{
    public class FormElementParticleTagsSelectorType : ParticleTagsSelectorType
    {
        private EnumSelectorEditorDrawer<ParticleForm> _particleFormSelector;
        private EnumSelectorEditorDrawer<ParticleElement> _particleElementSelector;

        public FormElementParticleTagsSelectorType()
        {
            _particleFormSelector = new EnumSelectorEditorDrawer<ParticleForm>("Key_particleFormSelector");
            _particleElementSelector = new EnumSelectorEditorDrawer<ParticleElement>("Key_particleElementSelector");
        }

        public override void Draw()
        {
            GUILayout.BeginHorizontal("box");

            _particleFormSelector.OnGUI();
            _particleElementSelector.OnGUI();

            GUILayout.EndHorizontal();
        }

        public override List<ParticleTags> GetTagsToGenerate()
        {
            var selectedTags = new List<ParticleTags>();
            foreach (var element in _particleElementSelector.GetSelected())
            {
                foreach (var form in _particleFormSelector.GetSelected())
                {
                    var tag = new ParticleTags
                    {
                        form = form,
                        element = element,
                    };
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
            _particleElementSelector.SaveEditorPrefs();
            _particleFormSelector.SaveEditorPrefs();
        }

        public override void LoadEditorPrefs()
        {
            _particleElementSelector.LoadEditorPrefs();
            _particleFormSelector.LoadEditorPrefs();
        }
    }
}