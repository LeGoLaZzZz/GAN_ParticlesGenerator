using System;
using System.Collections.Generic;
using NNParticleSystemGenerator.DataSetGenerator.Editor;
using UnityEditor;
using UnityEngine;

namespace NNParticleSystemGenerator.Editor
{
    public class ParticleTagsSelector : ITagsToGenerateProvider, IEditorPrefsSaveLoad
    {
        private TagsProvidedType _tagsProvidedType;
        private readonly ColorGroupParticleTagsSelectorType _particleColorGroupSelector;
        private readonly FormElementParticleTagsSelectorType _particleFormElementSelector;
        private readonly ColorGroupFormParticleTagsSelectorType _particleColorGroupFormSelector;

        private const string TagsProvidedTypePrefsKey = "_tagsProvidedType";

        public ParticleTagsSelector()
        {
            _particleColorGroupSelector = new ColorGroupParticleTagsSelectorType();
            _particleFormElementSelector = new FormElementParticleTagsSelectorType();
            _particleColorGroupFormSelector = new ColorGroupFormParticleTagsSelectorType();
        }

        public void Draw()
        {

            GUILayout.BeginVertical();
            _tagsProvidedType = (TagsProvidedType)EditorGUILayout.EnumPopup("TagsProvidedType", _tagsProvidedType);
            
            GetSelector(_tagsProvidedType).Draw();
            GUILayout.EndVertical();

        }

        public List<ParticleTags> GetTagsToGenerate()
        {
            return GetSelector(_tagsProvidedType).GetTagsToGenerate();
        }

        public TagsProvidedType GetTagsProvidedType()
        {
            return _tagsProvidedType;
        }

        public void SaveEditorPrefs()
        {
            _particleColorGroupSelector.SaveEditorPrefs();
            _particleFormElementSelector.SaveEditorPrefs();
            _particleColorGroupFormSelector.SaveEditorPrefs();
            EditorPrefs.SetInt(TagsProvidedTypePrefsKey, (int)_tagsProvidedType);
        }

        public void LoadEditorPrefs()
        {
            _particleColorGroupSelector.LoadEditorPrefs();
            _particleFormElementSelector.LoadEditorPrefs();
            _particleColorGroupFormSelector.LoadEditorPrefs();
            _tagsProvidedType = (TagsProvidedType)EditorPrefs.GetInt(TagsProvidedTypePrefsKey, 0);
        }

        private ParticleTagsSelectorType GetSelector(TagsProvidedType tagsProvidedType)
        {
            return tagsProvidedType switch
            {
                TagsProvidedType.Form_Element => _particleFormElementSelector,
                TagsProvidedType.Color_Group => _particleColorGroupSelector,
                TagsProvidedType.Color_Group_Form => _particleColorGroupFormSelector,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}