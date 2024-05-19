using System.Reflection;
using SmartAttributes.MultiDraft.Attributes;
using UnityEditor;

namespace SmartAttributes.MultiDraft.Editor.AttributesDrawers.VisibillityDrawers
{
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIfAttributeDrawer : MultiPropertyDrawer
    {
        public override AttributeDrawWorker GetDrawWorker(MultiPropertyAttribute propertyAttributeInstance)
        {
            return new ShowIfAttributeDrawWorker(propertyAttributeInstance);
        }
    }


    public class ShowIfAttributeDrawWorker : VisibleAttributeDrawWorker
    {
        public ShowIfAttributeDrawWorker(MultiPropertyAttribute propertyAttribute) : base(propertyAttribute)
        {
        }

        protected override bool NeedDrawBoolField(FieldGUI fieldGUI, SerializedProperty conditionFieldProperty)
        {
            return conditionFieldProperty.boolValue;
        }

        protected override bool NeedDrawObjectField(FieldGUI fieldGUI, SerializedProperty objectField,
            object comparisonObject)
        {
            var targetObject = objectField.serializedObject.targetObject;
            var targetObjectClassType = targetObject.GetType();
            var field = targetObjectClassType.GetField(objectField.propertyPath, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            var value = field.GetValue(targetObject);

            return object.Equals(value, comparisonObject);
        }
    }
}