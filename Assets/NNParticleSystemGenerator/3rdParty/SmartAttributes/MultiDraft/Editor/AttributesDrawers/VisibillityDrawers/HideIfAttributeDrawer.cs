using SmartAttributes.MultiDraft.Attributes;
using UnityEditor;

namespace SmartAttributes.MultiDraft.Editor.AttributesDrawers.VisibillityDrawers
{
    [CustomPropertyDrawer(typeof(HideIfAttribute))]
    public class HideIfAttributeDrawer : MultiPropertyDrawer
    {
        public override AttributeDrawWorker GetDrawWorker(MultiPropertyAttribute propertyAttributeInstance)
        {
            return new HideIfAttributeDrawWorker(propertyAttributeInstance);
        }
    }

    public class HideIfAttributeDrawWorker : VisibleAttributeDrawWorker
    {
        protected override bool NeedDrawBoolField(FieldGUI fieldGUI, SerializedProperty conditionFieldProperty)
        {
            return !conditionFieldProperty.boolValue;
        }

        protected override bool NeedDrawObjectField(FieldGUI fieldGUI, SerializedProperty objectField,
            object comparisonObject)
        {
            var targetObject = objectField.serializedObject.targetObject;
            var targetObjectClassType = targetObject.GetType();
            var field = targetObjectClassType.GetField(objectField.propertyPath);
            var value = field.GetValue(targetObject);

            return !object.Equals(value, comparisonObject);
        }

        public HideIfAttributeDrawWorker(MultiPropertyAttribute propertyAttribute) : base(propertyAttribute)
        {
        }
    }
}