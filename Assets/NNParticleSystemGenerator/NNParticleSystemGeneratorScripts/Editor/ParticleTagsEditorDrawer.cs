using NNParticleSystemGenerator.DataSetGenerator.Editor;
using UnityEditor;
using UnityEngine;

namespace NNParticleSystemGenerator.Editor
{
    public class ParticleTagsEditorDrawer : IEditorPrefsSaveLoad
    {
        public ParticleForm Form;
        public ParticleElement Element;
        public ParticleColorGroup ColorGroup;
        // public Color color;
        private string _prefsSaveKey;

        public ParticleTagsEditorDrawer(string prefsSaveKey)
        {
            _prefsSaveKey = prefsSaveKey;
        }

        public void Draw()
        {
            Form = (ParticleForm)EditorGUILayout.EnumPopup("Form", Form);
            Element = (ParticleElement)EditorGUILayout.EnumPopup("Element", Element);
            ColorGroup = (ParticleColorGroup)EditorGUILayout.EnumPopup("ColorGroup", ColorGroup);
            // color = EditorGUILayout.ColorField(color);   
        }

        public ParticleTags GetTags()
        {
            return new ParticleTags
            {
                form = Form,
                element = Element,
                colorGroup = ColorGroup
            };
        }

        public void SaveEditorPrefs()
        {
            EditorPrefs.SetString(_prefsSaveKey, Form.ToString() + "," + Element.ToString() + "," + ColorGroup.ToString());
        }

        public void LoadEditorPrefs()
        {
            var str = EditorPrefs.GetString(_prefsSaveKey);
            Form = (ParticleForm)System.Enum.Parse(typeof(ParticleForm), str.Split(',')[0]);
            Element = (ParticleElement)System.Enum.Parse(typeof(ParticleElement), str.Split(',')[1]);
            ColorGroup = (ParticleColorGroup)System.Enum.Parse(typeof(ParticleColorGroup), str.Split(',')[2]);
        }
    }
}