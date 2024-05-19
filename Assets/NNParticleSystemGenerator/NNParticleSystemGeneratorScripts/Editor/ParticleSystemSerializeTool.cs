using System;
using System.Collections.Generic;
using System.Text;
using NNParticleSystemGenerator.DataSetGenerator.Editor;
using UnityEditor;
using UnityEngine;

namespace NNParticleSystemGenerator.Editor
{
    public interface IParticleSystemJsonFiller
    {
        public void FillParticleFromJson(string json, ParticleSystem particleSystem);
    }

    public interface ICustomEditorLogger
    {
        void DebugLog(string text);
        void ClearLogger();
    }

    public class ParticleSystemSerializeTool : EditorWindow, IParticleSystemJsonFiller, ICustomEditorLogger
    {
        private const int TextAreaHeight = 400;

        private ParticleSystem particleSystemPrefab;
        private string textFieldText = "Enter text";
        private Vector2 scrollPosition = Vector2.zero;
        private Vector2 tabContentScrollPosition = Vector2.zero;
        private int selectedTab = 1;
        private GUIContent[] tabContents;
        private Action[] tabActions;
        private GUIStyle _tabStyle;
        private string selectedFolderPath = "Assets";

        private DatasetParticles _datasetParticles;

        private bool needSpawnNewParticle;
        private ParticleSystemConverterSettings_Drawer _settingsDrawer;
        private PythonParticlesGeneratorEditor _pythonParticlesGeneratorEditor;
        private ParticleSystemAssetsCreator _assetsCreator;
        private StringBuilder _debugConsoleText = new();
        private const int DebugLinesCount = 4;
        private List<int> _lastDebugStringLength = new();
        private int _linesAdded;

        private List<IEditorPrefsSaveLoad> _prefsSaveLoads = new();
        private const string DatasetParticlesPrefsKey = "DatasetParticlesPrefsKey";

        public void DebugLog(string text)
        {
            var lenBefore = _debugConsoleText.Length;
            _debugConsoleText.Append(DateTime.Now.ToString("[hh:mm:ss] "));
            _debugConsoleText.Append(text);
            _debugConsoleText.Append("\n");
            var lenAfter = _debugConsoleText.Length;

            _lastDebugStringLength.Add(lenAfter - lenBefore);
            _linesAdded++;

            if (_linesAdded > DebugLinesCount)
            {
                for (int i = 0; i < _linesAdded - DebugLinesCount; i++)
                {
                    if (_lastDebugStringLength[0] > 0) _debugConsoleText.Remove(0, _lastDebugStringLength[0]);
                    _lastDebugStringLength.RemoveAt(0);
                    _linesAdded--;
                }
            }

            Repaint();
        }

        public void ClearLogger()
        {
            _debugConsoleText.Clear();
            _linesAdded = 0;
            _lastDebugStringLength = new();
        }

        [MenuItem("PSNN_Tools/ParticleSystemSerializeTool")]
        private static void ShowWindow()
        {
            var window = GetWindow<ParticleSystemSerializeTool>();
            window.titleContent = new GUIContent("ParticleSystemSerializeTool");
            window.Show();
        }


        private void OnEnable()
        {
            // Инициализируем массив с заголовками для вкладок
            tabContents = new[]
            {
                new GUIContent("PS -> JSON"),
                new GUIContent("JSON -> PS"),
                new GUIContent("Tab 3")
            };

            // Инициализируем массив с действиями для каждой вкладки
            tabActions = new Action[]
            {
                ToJson,
                ToPs,
                DrawTab3
            };

            _assetsCreator = new ParticleSystemAssetsCreator(this);
            _pythonParticlesGeneratorEditor = new PythonParticlesGeneratorEditor(_assetsCreator, this);
            _prefsSaveLoads.Add(_pythonParticlesGeneratorEditor);

            Selection.selectionChanged += OnSelectionChanged;
            if (_settingsDrawer == null) _settingsDrawer = new ParticleSystemConverterSettings_Drawer("ParticleSystemSerializeTool");
            if (!_prefsSaveLoads.Contains(_settingsDrawer))
            {
                _prefsSaveLoads.Add(_settingsDrawer);
            }

            LoadEditorPrefs();
        }


        private void OnDisable()
        {
            Selection.selectionChanged -= OnSelectionChanged;
            SaveEditorPrefs();
        }

        private void OnGUI()
        {
            EditorHelpers.DrawScriptField(this);

            if (_tabStyle == null) CreateStyles();

            DrawTabButtons();
            DrawTabContent();

            EditorGUILayout.TextArea(_debugConsoleText.ToString(), GUILayout.Height(100));
            if (GUILayout.Button("Clear debug"))
            {
                ClearLogger();
            }

            void DrawTabContent()
            {
                tabContentScrollPosition = EditorGUILayout.BeginScrollView(tabContentScrollPosition);
                if (selectedTab >= 0 && selectedTab < tabActions.Length)
                {
                    tabActions[selectedTab]?.Invoke();
                }

                EditorGUILayout.EndScrollView();
            }

            void DrawTabButtons()
            {
                GUILayout.BeginHorizontal();
                for (int i = 0; i < tabContents.Length; i++)
                {
                    if (GUILayout.Toggle(i == selectedTab, tabContents[i], _tabStyle))
                    {
                        selectedTab = i;
                    }
                }

                GUILayout.EndHorizontal();
            }
        }

        private void ToJson()
        {
            EditorGUILayout.LabelField("Serialize Particle systems to JSON");

            GUILayout.BeginHorizontal();

            PrefabAndButton();
            TextAreaField();

            GUILayout.EndHorizontal();

            void PrefabAndButton()
            {
                GUILayout.BeginVertical();
                particleSystemPrefab =
                    EditorGUILayout.ObjectField("Prefab Particle System", particleSystemPrefab, typeof(ParticleSystem),
                        false) as ParticleSystem;

                _settingsDrawer.Draw();

                if (GUILayout.Button("PS -> JSON", GUILayout.Height(40)))
                {
                    if (particleSystemPrefab != null)
                    {
                        MaterialConverter materialConverter = new NullMaterialConverter();

                        if (_datasetParticles != null)
                        {
                            materialConverter = new DatasetMaterialConverter(_datasetParticles.ParsedDatasetParticles);
                        }

                        var settings = _settingsDrawer.GenerateSettings(materialConverter);
                        textFieldText =
                            SerializeHelpers.SerializeParticleSystemToJson(particleSystemPrefab, settings);
                    }
                }

                GUILayout.EndVertical();
            }

            void TextAreaField()
            {
                GUILayout.BeginVertical();
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(TextAreaHeight));
                textFieldText = EditorGUILayout.TextArea(textFieldText, GUILayout.ExpandHeight(true));

                EditorGUILayout.EndScrollView();
                GUILayout.EndVertical();
            }
        }

        private void ToPs()
        {
            GUILayout.Label("Parse JSON to Particle systems", EditorStyles.boldLabel);

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            LeftColumn();
            RightColumn();

            GUILayout.EndHorizontal();
            
            BottomContent();
            GUILayout.EndVertical();


            void LeftColumn()
            {
                GUILayout.BeginVertical();
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(TextAreaHeight));
                textFieldText = EditorGUILayout.TextArea(textFieldText, GUILayout.ExpandHeight(true));
                EditorGUILayout.EndScrollView();
                GUILayout.EndVertical();
            }

            void RightColumn()
            {
                GUILayout.BeginVertical();

                _datasetParticles =
                    EditorGUILayout.ObjectField("Dataset with materials(!)", _datasetParticles, typeof(DatasetParticles),
                        false) as DatasetParticles;

                _settingsDrawer.Draw();

                EditorGUILayout.Space(10);
                GUILayout.Label("Save ps to", EditorStyles.boldLabel);
                needSpawnNewParticle = EditorGUILayout.Toggle("Spawn new particle", needSpawnNewParticle);
                particleSystemPrefab =
                    EditorGUILayout.ObjectField("Prefab Particle System ", particleSystemPrefab, typeof(ParticleSystem),
                            false)
                        as ParticleSystem;

                EditorGUILayout.Space(10);
                GUILayout.Label("From field -> ps", EditorStyles.boldLabel);


                if (GUILayout.Button("JSON -> PS"))
                {
                    JsonToParticleButton(textFieldText);
                }

                EditorGUILayout.Space(10);
                GUILayout.Label("From file -> ps", EditorStyles.boldLabel);

                if (GUILayout.Button("JSON from file -> PS"))
                {
                    var fromFile = EditorHelpers.LoadTextFromFile(NnPythonParticlesGenerator.PSFromFile);
                    JsonToParticleButton(fromFile);
                }

                GUILayout.EndVertical();
            }

            void BottomContent()
            {
                EditorGUILayout.Space(10);
                GUILayout.Label("GAN Generation", EditorStyles.boldLabel);
                
                var labelWordWrap = EditorStyles.label.wordWrap;
                EditorStyles.label.wordWrap = true;
                selectedFolderPath = EditorGUILayout.TextField("Save path",selectedFolderPath);
                EditorStyles.label.wordWrap = labelWordWrap;
                
                _pythonParticlesGeneratorEditor.DrawPythonGenerateFile(selectedFolderPath);
            }
        }

        public void FillParticleFromJson(string json, ParticleSystem particleToFill)
        {
            MaterialConverter materialConverter = new NullMaterialConverter();

            if (_datasetParticles != null)
            {
                materialConverter = new DatasetMaterialConverter(_datasetParticles.ParsedDatasetParticles);
            }

            var particlesConverterSettings = _settingsDrawer.GenerateSettings(materialConverter);
            SerializeHelpers.ParseJsonToParticleSystem(particleToFill, json, particlesConverterSettings);
        }


        private void JsonToParticleButton(string json, string assetName = "Particles")
        {
            if (needSpawnNewParticle)
            {
                particleSystemPrefab = _assetsCreator.CreateAssetParticle(json, selectedFolderPath, assetName);
            }
            else
            {
                FillParticleFromJson(json, particleSystemPrefab);
            }
        }

        private void DrawTab3()
        {
            EditorGUILayout.LabelField("Tab 3 Content");
            EditorGUILayout.LabelField(selectedFolderPath);
        }


        private void CreateStyles()
        {
            if (_tabStyle == null)
            {
                _tabStyle = new GUIStyle(GUI.skin.button);
                _tabStyle.normal.textColor = Color.black;
                _tabStyle.onNormal.textColor = Color.black;
                _tabStyle.fontStyle = FontStyle.Bold;
            }
        }

        private void OnSelectionChanged()
        {
            if (Selection.activeObject == null) return;
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            selectedFolderPath = path;
        }

        private void LoadEditorPrefs()
        {
            _prefsSaveLoads.ForEach(l => l.LoadEditorPrefs());
            
            _datasetParticles = AssetDatabase.LoadAssetAtPath<DatasetParticles>(EditorPrefs.GetString(DatasetParticlesPrefsKey));
        }

        private void SaveEditorPrefs()
        {
            _prefsSaveLoads.ForEach(l => l.SaveEditorPrefs());
            EditorPrefs.SetString(DatasetParticlesPrefsKey, AssetDatabase.GetAssetPath(_datasetParticles));
        }
    }
}