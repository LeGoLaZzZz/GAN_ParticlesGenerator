using System;
using SmartAttributes.MultiDraft.Attributes;
using UnityEditor;
using UnityEngine;

namespace SmartAttributes.MultiDraft.Editor.AttributesDrawers
{
    [CustomPropertyDrawer(typeof(RequiredAttribute))]
    public class RequiredAttributeDrawer : MultiPropertyDrawer
    {
        public override AttributeDrawWorker GetDrawWorker(MultiPropertyAttribute propertyAttributeInstance)
        {
            return new RequiredAttributeDrawWorker(propertyAttributeInstance);
        }
    }

    public class RequiredAttributeDrawWorker : AttributeDrawWorker
    {
        private float _bottomPadding = 3;
        private float _topPadding = 0;
        private Color _bgColor = new Color(0.67f, 0.43f, 0f, 0.3f);
        private GUIStyle _requiredLabelStyle;

        public override int GetOrder()
        {
            return 10;
        }

        public RequiredAttributeDrawWorker(MultiPropertyAttribute propertyAttribute) : base(propertyAttribute)
        {
            _requiredLabelStyle = new GUIStyle(EditorStyles.miniLabel);
            _requiredLabelStyle.normal.textColor = new Color(1f, 0.41f, 0.29f, 0.65f);
        }


        public override void DrawGUI(FieldGUI fieldGUI, Action callNextDrawer)
        {
            if (!IsRequired(fieldGUI.Property))
            {
                callNextDrawer.Invoke();
                return;
            }

            var beginVertical = EditorGUILayout.BeginVertical();
            
            EditorGUI.DrawRect(
                new Rect(beginVertical.x,
                    beginVertical.y,
                    beginVertical.width,
                    beginVertical.height),
                _bgColor);

            
            EditorGUILayout.Space(_topPadding);

            var label = EditorGUIUtility.IconContent("redLight");
            label.text = "Required";
            EditorGUIUtility.SetIconSize(Vector2.one*10);
            GUILayout.Label(label, _requiredLabelStyle);

            fieldGUI.Position.y += _topPadding;
            fieldGUI.Position.height += (_bottomPadding + _topPadding);


            EditorGUIUtility.SetIconSize(Vector2.one*20);
            var iconContent = EditorGUIUtility.IconContent("CollabConflict Icon");
            iconContent.text = fieldGUI.Label.text;
            fieldGUI.Label = iconContent;

            
            callNextDrawer.Invoke();

            EditorGUILayout.Space(_bottomPadding);
            EditorGUILayout.EndVertical();
        }


        private bool IsRequired(SerializedProperty property) =>
            property.propertyType == SerializedPropertyType.ObjectReference
            && property.objectReferenceValue == null;
    }
}