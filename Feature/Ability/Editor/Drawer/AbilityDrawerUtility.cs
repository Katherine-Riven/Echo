using System;
using System.Collections.Generic;
using System.Reflection;
using Echo.Abilities;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace EchoEditor.Abilities
{
    public static class AbilityDrawerUtility
    {
        private static readonly List<GenericSelectorItem<Type>>       s_FeatureItems          = new List<GenericSelectorItem<Type>>();
        private static readonly List<GenericSelectorItem<Type>>       s_EffectItems           = new List<GenericSelectorItem<Type>>();
        private static readonly List<GenericSelectorItem<Type>>       s_CancelableEffectItems = new List<GenericSelectorItem<Type>>();
        private static readonly List<GenericSelectorItem<Type>>       s_VariableItems         = new List<GenericSelectorItem<Type>>();
        private static readonly List<GenericSelectorItem<AbilityTag>> s_TagItems              = new List<GenericSelectorItem<AbilityTag>>();

        public static IReadOnlyList<GenericSelectorItem<Type>>       FeatureItems          => s_FeatureItems;
        public static IReadOnlyList<GenericSelectorItem<Type>>       EffectItems           => s_EffectItems;
        public static IReadOnlyList<GenericSelectorItem<Type>>       CancelableEffectItems => s_CancelableEffectItems;
        public static IReadOnlyList<GenericSelectorItem<Type>>       VariableItems         => s_VariableItems;
        public static IReadOnlyList<GenericSelectorItem<AbilityTag>> TagItems              => s_TagItems;

        static AbilityDrawerUtility()
        {
            foreach (Type type in EditorHelper.AllTypes)
            {
                if (type.IsInterface             ||
                    type.IsGenericType           ||
                    type.IsAbstract              ||
                    type.IsClass        == false ||
                    type.IsSerializable == false)
                {
                    continue;
                }

                if (type.IsSubclassOf(typeof(AbilityFeature)))
                {
                    s_FeatureItems.Add(new GenericSelectorItem<Type>(MenuItemAttribute.GetMenuPath(type, typeof(AbilityFeature)), type));
                }
                else if (type.IsSubclassOf(typeof(AbilityEffect)))
                {
                    GenericSelectorItem<Type> item = new GenericSelectorItem<Type>(MenuItemAttribute.GetMenuPath(type, typeof(AbilityEffect)), type);
                    s_EffectItems.Add(item);
                    if (type.IsDefined(typeof(CancelableEffectAttribute)))
                    {
                        s_CancelableEffectItems.Add(item);
                    }
                }
                else if (typeof(IAbilityVariable).IsAssignableFrom(type))
                {
                    s_VariableItems.Add(new GenericSelectorItem<Type>(type.Name, type));
                }
            }

            foreach (FieldInfo field in typeof(AbilityTag).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (field.Name == nameof(AbilityTag.All) || field.Name == nameof(AbilityTag.None))
                {
                    continue;
                }

                string itemName = GetItemName(field);
                if (field.FieldType == typeof(AbilityTag))
                {
                    s_TagItems.Add(new GenericSelectorItem<AbilityTag>(itemName, (AbilityTag) field.GetValue(null)));
                }
                else if (field.FieldType.IsSubclassOf(typeof(AbilityTagPack)))
                {
                    AddTagPack($"{itemName}/", field.GetValue(null));
                }
            }

            void AddTagPack(string path, object pack)
            {
                foreach (FieldInfo field in pack.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
                {
                    string itemName = GetItemName(field);
                    if (field.FieldType == typeof(AbilityTag))
                    {
                        s_TagItems.Add(new GenericSelectorItem<AbilityTag>($"{path}{itemName}", (AbilityTag) field.GetValue(pack)));
                    }
                    else if (field.FieldType.IsSubclassOf(typeof(AbilityTagPack)))
                    {
                        AddTagPack($"{path}{itemName}/", field.GetValue(pack));
                    }
                }
            }

            string GetItemName(FieldInfo field)
            {
                string itemName      = field.Name;
                var    inspectorName = field.GetCustomAttribute<InspectorNameAttribute>();
                if (inspectorName != null)
                {
                    itemName = $"{itemName} ({inspectorName.displayName})";
                }

                return itemName;
            }
        }
    }
}