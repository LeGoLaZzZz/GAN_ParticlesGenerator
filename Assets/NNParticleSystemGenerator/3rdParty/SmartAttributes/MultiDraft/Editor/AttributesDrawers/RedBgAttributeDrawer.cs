using System;
using SmartAttributes.MultiDraft.Attributes;
using UnityEditor;
using UnityEngine;

namespace SmartAttributes.MultiDraft.Editor.AttributesDrawers
{
    [CustomPropertyDrawer(typeof(RedBgAttribute))]
    public class RedBgAttributeDrawer : MultiPropertyDrawer
    {
        public override AttributeDrawWorker GetDrawWorker(MultiPropertyAttribute propertyAttributeInstance)
        {
            return new RedBgAttributeDrawWorker(propertyAttributeInstance);
        }
    }

    public class RedBgAttributeDrawWorker : AttributeDrawWorker
    {
        public RedBgAttributeDrawWorker(MultiPropertyAttribute propertyAttribute) : base(propertyAttribute)
        {
        }

        public override void DrawGUI(FieldGUI fieldGUI, Action callNextDrawer)
        {
            EditorGUI.DrawRect(
                new Rect(fieldGUI.Position.x,
                    fieldGUI.Position.y,
                    fieldGUI.Position.width,
                    fieldGUI.Position.height),
                new Color(0.29f, 0f, 0.05f, 0.6f));
            
            callNextDrawer.Invoke();
            
        }

    }
}