using System;
using SmartAttributes.MultiDraft.Attributes;
using UnityEditor;

namespace SmartAttributes.MultiDraft.Editor.AttributesDrawers.VisibillityDrawers
{
    public abstract class VisibleAttributeDrawWorker : AttributeDrawWorker
    {
        public VisibleAttributeDrawWorker(MultiPropertyAttribute propertyAttribute) : base(propertyAttribute)
        {
        }

        public override int GetOrder()
        {
            return (int) MultiAttributeDrawerOrder.First;
        }

        public override void DrawGUI(FieldGUI fieldGUI, Action callNextDrawer)
        {
            var visibilityAttribute = (VisibilityAttribute) PropertyAttribute;

            if (visibilityAttribute.BoolFieldName != null)
            {
                BoolFieldAttribute();
                return;
            }

            if (visibilityAttribute.ObjectFieldName != null)
            {
                ObjectFieldAttribute();
                return;
            }


            void BoolFieldAttribute()
            {
                var conditionField = fieldGUI.Property.serializedObject.FindProperty(visibilityAttribute.BoolFieldName);

                if (NeedDrawBoolField(fieldGUI, conditionField))
                {
                    callNextDrawer.Invoke();
                }
            }


            void ObjectFieldAttribute()
            {
                var objectField = fieldGUI.Property.serializedObject.FindProperty(visibilityAttribute.ObjectFieldName);

                if (NeedDrawObjectField(fieldGUI, objectField, visibilityAttribute.ComparisonObject))
                {
                    callNextDrawer.Invoke();
                }
            }
        }


        protected abstract bool NeedDrawBoolField(FieldGUI fieldGUI, SerializedProperty conditionFieldProperty);

        protected abstract bool NeedDrawObjectField(FieldGUI fieldGUI, SerializedProperty objectField,
            object comparisonObject);
    }
}