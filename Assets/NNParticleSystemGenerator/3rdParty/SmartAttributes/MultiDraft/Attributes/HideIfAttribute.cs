using System.Diagnostics;

namespace SmartAttributes.MultiDraft.Attributes
{
    [Conditional("UNITY_EDITOR")]
    public class HideIfAttribute : VisibilityAttribute
    {
        public HideIfAttribute(string boolFieldName) : base(boolFieldName)
        {
        }

        public HideIfAttribute(string objectFieldName, object comparisonObject) : base(objectFieldName, comparisonObject)
        {
        }
    }
}