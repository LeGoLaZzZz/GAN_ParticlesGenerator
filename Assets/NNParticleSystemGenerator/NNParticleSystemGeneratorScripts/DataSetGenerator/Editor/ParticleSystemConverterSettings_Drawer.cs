using NNParticleSystemGenerator.Editor;
using UnityEditor;
using UnityEngine;

namespace NNParticleSystemGenerator.DataSetGenerator.Editor
{
    public class ParticleSystemConverterSettings_Drawer : IEditorPrefsSaveLoad
    {
        private MinMaxCurveConvertMode _convertCurveMode;
        private bool _needCurveLimitPoints = true;
        private int _limitCurvePoints = 3;

        private MinMaxGradientConvertMode _convertGradientMode;
        private bool _needGradientLimitPoints = true;
        private int _limitGradientPoints = 3;

        private string _savePrefsKey = "ParticleSystemConverterSettings";

        public ParticleSystemConverterSettings_Drawer(string savePrefsKey)
        {
            _savePrefsKey = savePrefsKey;
        }

        public ParticlesConverterSettings GenerateSettings(MaterialConverter materialConverter)
        {
            var curveSettings =
                MinMaxCurveConverterSettings.Create(_convertCurveMode, _needCurveLimitPoints,
                    _limitCurvePoints);

            var gradientSettings =
                MinMaxGradientSettings.Create(_convertGradientMode, _needGradientLimitPoints,
                    _limitGradientPoints);

            var settings = new ParticlesConverterSettings(curveSettings, gradientSettings, materialConverter);
            return settings;
        }

        public void Draw()
        {
            GUILayout.BeginVertical("Box");
            GUILayout.Label("PS parse settings", EditorStyles.largeLabel);
            GUILayout.Space(10);
            
            GUILayout.Label("Curve settings", EditorStyles.boldLabel);
            _convertCurveMode = (MinMaxCurveConvertMode)EditorGUILayout.EnumPopup("Curve Type", _convertCurveMode);
            _needCurveLimitPoints = EditorGUILayout.Toggle("Need Curve limit points", _needCurveLimitPoints);
            _limitCurvePoints = EditorGUILayout.IntField("Limit Curve points", _limitCurvePoints);

            GUILayout.Space(10);
            GUILayout.Label("Gradient settings", EditorStyles.boldLabel);
            _convertGradientMode =
                (MinMaxGradientConvertMode)EditorGUILayout.EnumPopup("Gradient Type", _convertGradientMode);
            _needGradientLimitPoints =
                EditorGUILayout.Toggle("Need gradient limit points", _needGradientLimitPoints);
            _limitGradientPoints = EditorGUILayout.IntField("Limit gradient points", _limitGradientPoints);

            GUILayout.EndVertical();
        }

        public void SaveEditorPrefs()
        {
            EditorPrefs.SetInt(_savePrefsKey + "_convertCurveMode", (int)_convertCurveMode);
            EditorPrefs.SetBool(_savePrefsKey + "_needCurveLimitPoints", _needCurveLimitPoints);
            EditorPrefs.SetInt(_savePrefsKey + "_limitCurvePoints", _limitCurvePoints);
            
            EditorPrefs.SetInt(_savePrefsKey + "_convertGradientMode", (int)_convertGradientMode);
            EditorPrefs.SetBool(_savePrefsKey + "_needGradientLimitPoints", _needGradientLimitPoints);
            EditorPrefs.SetInt(_savePrefsKey + "_limitGradientPoints", _limitGradientPoints);
            
        }

        public void LoadEditorPrefs()
        {
            _convertCurveMode = (MinMaxCurveConvertMode)EditorPrefs.GetInt(_savePrefsKey + "_convertCurveMode", 0);
            _needCurveLimitPoints = EditorPrefs.GetBool(_savePrefsKey + "_needCurveLimitPoints", true);
            _limitCurvePoints = EditorPrefs.GetInt(_savePrefsKey + "_limitCurvePoints", 3);
            
            _convertGradientMode = (MinMaxGradientConvertMode)EditorPrefs.GetInt(_savePrefsKey + "_convertGradientMode", 0);
            _needGradientLimitPoints = EditorPrefs.GetBool(_savePrefsKey + "_needGradientLimitPoints", true);
            _limitGradientPoints = EditorPrefs.GetInt(_savePrefsKey + "_limitGradientPoints", 3);
        }
    }
}