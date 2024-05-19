using System;
using SmartAttributes.MultiDraft.Attributes;
using UnityEditor;
using UnityEngine;

namespace SmartAttributes.MultiDraft.Editor.AttributesDrawers
{
    [CustomPropertyDrawer(typeof(DisabledAttribute))]
    public class DisabledAttributeDrawer : MultiPropertyDrawer
    {
        public override AttributeDrawWorker GetDrawWorker(MultiPropertyAttribute propertyAttributeInstance)
        {
            return new DisabledAttributeDrawWorker(propertyAttributeInstance);
        }
    }


    public class DisabledAttributeDrawWorker : AttributeDrawWorker
    {
        public DisabledAttributeDrawWorker(MultiPropertyAttribute propertyAttribute) : base(propertyAttribute)
        {
        }

        public override void DrawGUI(FieldGUI fieldGUI, Action callNextDrawer)
        {
            GUI.enabled = false;
            callNextDrawer.Invoke();
            GUI.enabled = true;
        }
    }
}