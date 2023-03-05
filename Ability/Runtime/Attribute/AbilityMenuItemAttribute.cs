using System;
#if UNITY_EDITOR
using System.IO;
using Sirenix.Utilities;
#endif

namespace Echo.Abilities
{
    /// <summary>
    /// 能力菜单选项名称
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class AbilityMenuItemAttribute : Attribute
    {
        public readonly string MenuItem;

        public AbilityMenuItemAttribute(string menuItem)
        {
            MenuItem = menuItem;
        }

#if UNITY_EDITOR
        
        public static string GetMenuName(Type type)
        {
            string menuItem = type.GetAttribute<AbilityMenuItemAttribute>()?.MenuItem ?? type.Name;
            return Path.GetFileNameWithoutExtension(menuItem);
        }
        
        public static string GetMenuPath(Type type, Type stopType)
        {
            string menuItem = type.GetAttribute<AbilityMenuItemAttribute>()?.MenuItem ?? type.Name;
            Type   baseType = type.BaseType;
            while (baseType != null && baseType != stopType)
            {
                string temp = baseType.GetAttribute<AbilityMenuItemAttribute>()?.MenuItem ?? baseType.Name;
                menuItem = $"{temp}/{menuItem}";
                baseType = type.BaseType;
            }

            return menuItem;
        }
#endif
    }
}