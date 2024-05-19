namespace _3rdParty.SmartAttributes.Editor
{
    using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

    public class InspectorComment : MonoBehaviour
    {
        public string CommentText;

#if UNITY_EDITOR
        [CustomEditor(typeof(InspectorComment))]
        public class InspectorCommentEditor : Editor
        {
            private InspectorComment _targetComponent;
            private string _commentText;
            private bool _isEditMode = false;

            private GUIStyle _headerStyle;
            private GUIStyle _commentStyle;
            private GUIContent _iconContent;

            private void CreateStyles()
            {
                // Create header style with icon
                _headerStyle = new GUIStyle(EditorStyles.boldLabel);
                _headerStyle.alignment = TextAnchor.MiddleLeft;
                _headerStyle.imagePosition = ImagePosition.ImageLeft;
                _headerStyle.margin = new RectOffset(0, 0, 10, 0);
                _headerStyle.padding = new RectOffset(0, 0, 0, 0);
                _headerStyle.normal.background = null;
                _headerStyle.normal.textColor = Color.white;
                _headerStyle.fontSize = 14;
                _headerStyle.fontStyle = FontStyle.Bold;
                _headerStyle.fixedHeight = 24;
                _headerStyle.contentOffset = new Vector2(0, 0);

                // Create comment text style
                _commentStyle = new GUIStyle(GUI.skin.box);
                _commentStyle.normal.textColor = Color.white;
                _commentStyle.fontSize = 16;
                _commentStyle.padding = new RectOffset(10, 10, 10, 10);
                _commentStyle.alignment = TextAnchor.UpperLeft;


                _iconContent = new GUIContent("Comment",EditorGUIUtility.IconContent("console.infoicon@2x").image);
            }

            private void OnEnable()
            {
                _targetComponent = (InspectorComment)target;
                _commentText = _targetComponent.CommentText;
                Undo.undoRedoPerformed += OnUndoRedoPerformed;
            }

            private void OnDisable()
            {
                Undo.undoRedoPerformed -= OnUndoRedoPerformed;
            }

            private void OnUndoRedoPerformed()
            {
                _commentText = _targetComponent.CommentText;
            }

            public override void OnInspectorGUI()
            {
                if (_headerStyle == null) CreateStyles();

                GUILayout.BeginVertical(GUI.skin.box);

                GUILayout.BeginHorizontal();

                var iconSize = EditorGUIUtility.GetIconSize();
                EditorGUIUtility.SetIconSize(new Vector2(24, 24));

                GUILayout.Label(_iconContent, _headerStyle);
                GUILayout.EndHorizontal();

                EditorGUIUtility.SetIconSize(iconSize);

                if (_isEditMode)
                {
                    DrawCommentEdit();
                }
                else
                {
                    DrawCommentDisplay();
                }

                GUILayout.EndVertical();
            }

            private void DrawCommentDisplay()
            {
                if (string.IsNullOrEmpty(_commentText))
                {
                    GUILayout.Label("No comment", EditorStyles.wordWrappedLabel);
                }
                else
                {
                    GUILayout.Box(_commentText, _commentStyle, GUILayout.ExpandWidth(true),
                        GUILayout.ExpandHeight(true));
                }

                GUILayout.Space(10);

                DrawButton("Edit Comment", () => _isEditMode = true);
            }

            private void DrawCommentEdit()
            {
                EditorGUI.BeginChangeCheck();

                _commentText = EditorGUILayout.TextArea(_commentText, GUILayout.ExpandHeight(true));

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_targetComponent, "Changed Comment Text");
                    _targetComponent.CommentText = _commentText;
                }

                GUILayout.Space(10);

                DrawButton("Save Comment", () => _isEditMode = false);
            }

            private void DrawButton(string buttonText, System.Action onClickAction)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                if (GUILayout.Button(buttonText, GUILayout.Width(100)))
                {
                    onClickAction?.Invoke();
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
        }
#endif
    }
}