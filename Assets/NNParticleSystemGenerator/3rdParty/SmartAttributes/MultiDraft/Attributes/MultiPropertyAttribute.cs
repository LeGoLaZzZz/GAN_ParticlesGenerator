using System.Diagnostics;
using UnityEngine;

namespace SmartAttributes.MultiDraft.Attributes
{
    [Conditional("UNITY_EDITOR")]
    public abstract class MultiPropertyAttribute : PropertyAttribute
    {
    }
}