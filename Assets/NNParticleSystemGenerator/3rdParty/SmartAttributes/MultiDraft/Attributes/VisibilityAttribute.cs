using System.Diagnostics;

namespace SmartAttributes.MultiDraft.Attributes
{
    [Conditional("UNITY_EDITOR")]
    public abstract class VisibilityAttribute : MultiPropertyAttribute
    {
        public string BoolFieldName;
        public string ObjectFieldName;
        public object ComparisonObject;

        public VisibilityAttribute(string boolFieldName)
        {
            this.BoolFieldName = boolFieldName;
        }

        public VisibilityAttribute(string objectFieldName, object comparisonObject)
        {
            ObjectFieldName = objectFieldName;
            ComparisonObject = comparisonObject;
        }
    }
}