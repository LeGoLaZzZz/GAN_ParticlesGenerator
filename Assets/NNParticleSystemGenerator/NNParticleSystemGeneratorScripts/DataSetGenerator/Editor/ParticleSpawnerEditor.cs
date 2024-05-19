using UnityEditor;

namespace NNParticleSystemGenerator.DataSetGenerator.Editor
{
    [CustomEditor(typeof(ParticleSpawner))]
    public class ParticleSpawnerEditor : UnityEditor.Editor
    {
        private ParticleSystemConverterSettings_Drawer _settingsDrawer;
        private void OnEnable()
        {
            _settingsDrawer = new ParticleSystemConverterSettings_Drawer("TagsEditor");
            _settingsDrawer.LoadEditorPrefs();
            var particleSpawner = (ParticleSpawner)target;
            particleSpawner.GetParticlesConverterSettingsFunc = _settingsDrawer.GenerateSettings;
        }
        
        public override void OnInspectorGUI()
        {
            _settingsDrawer.Draw();
            base.OnInspectorGUI();
        }
    }
}