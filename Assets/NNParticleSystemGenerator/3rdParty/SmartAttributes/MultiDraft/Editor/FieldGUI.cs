using UnityEditor;
using UnityEngine;

namespace SmartAttributes.MultiDraft.Editor
{
    public class FieldGUI
    {
        public Rect Position;
        public SerializedProperty Property;
        public GUIContent Label;
        public bool NeedDraw;

        public FieldGUI(SerializedProperty property, GUIContent label, bool needDraw)
        {
            Property = property;
            Label = label;
            NeedDraw = needDraw;
        }
    }
}