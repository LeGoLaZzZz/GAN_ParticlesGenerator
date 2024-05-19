using System.Diagnostics;

namespace SmartAttributes.MultiDraft.Attributes
{
    [Conditional("UNITY_EDITOR")]
    public class ShowIfAttribute : VisibilityAttribute
    {
        public ShowIfAttribute(string boolFieldName) : base(boolFieldName)
        {
        }

        public ShowIfAttribute(string objectFieldName, object comparisonObject) : base(objectFieldName, comparisonObject)
        {
        }
    }
}