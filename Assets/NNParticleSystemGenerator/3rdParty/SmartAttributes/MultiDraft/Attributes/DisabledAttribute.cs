using System.Diagnostics;

namespace SmartAttributes.MultiDraft.Attributes
{
    [Conditional("UNITY_EDITOR")]
    public class DisabledAttribute : MultiPropertyAttribute
    {
    }
}