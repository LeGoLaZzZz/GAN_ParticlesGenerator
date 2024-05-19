using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace SmartAttributes.InspectorButton
{
    [AttributeUsage(AttributeTargets.Method)]
    public class InspectorButtonAttribute : PropertyAttribute
    {
        public string ButtonName { get; }

        public string FunctionName { get; }


        public InspectorButtonAttribute(string buttonText, [CallerMemberName] string propertyName = "NoName")
        {
            ButtonName = buttonText;
            FunctionName = propertyName;
        }
    }
}