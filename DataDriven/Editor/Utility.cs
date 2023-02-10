using System;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;

namespace Lucifer.DataDriven.Editor
{
    static class Utility
    {
        static Utility()
        {
        }

        public static readonly IReadOnlyList<Type> Variables = new List<Type>();
        public static readonly IReadOnlyList<Type> Inputs    = new List<Type>();
        public static readonly IReadOnlyList<Type> Features  = new List<Type>();

        public static string GetDisplayName(this Type type)
        {
            return type.GetCustomAttribute<InspectNameAttribute>()?.Name ?? type.Name;
        }

        public static string GetPropertyName(this InspectorProperty property)
        {
            return property.GetAttribute<InspectNameAttribute>()?.Name ?? property.NiceName;
        }

        public static string GetTypeName(this InspectorProperty property)
        {
            return property.Info.TypeOfValue.GetDisplayName();
        }
    }
}