using System;
#if UNITY_EDITOR
using Sirenix.Utilities;
#endif

namespace Echo.Abilities
{
    /// <summary>
    /// 能力菜单选项名称
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class DisplayNameAttribute : Attribute
    {
        public readonly string Name;

        public DisplayNameAttribute(string name)
        {
            Name = name;
        }

#if UNITY_EDITOR

        public static string GetDisplay(Type type)
        {
            return type.GetAttribute<DisplayNameAttribute>()?.Name ?? type.Name;
        }

        public static string GetDisplayPath(Type type, Type stopType)
        {
            string menuItem = type.GetAttribute<DisplayNameAttribute>()?.Name ?? type.Name;
            Type   baseType = type.BaseType;
            while (baseType != null && baseType != stopType)
            {
                string temp = baseType.GetAttribute<DisplayNameAttribute>()?.Name;
                if (string.IsNullOrEmpty(temp) == false)
                {
                    menuItem = $"{temp}/{menuItem}";
                }

                baseType = baseType.BaseType;
            }

            return menuItem;
        }
#endif
    }
}