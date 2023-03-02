using System;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.Drawers;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Echo.Editor
{
    [DrawerPriority(DrawerPriorityLevel.WrapperPriority)]
    class ManagersDrawer : OdinValueDrawer<Manager[]>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            CollectionDrawerStaticInfo.NextCustomAddFunction = NextCustomAddFunction;
            CallNextDrawer(label);
        }

        private void NextCustomAddFunction()
        {
            GenericMenu menu = new GenericMenu();
            foreach (Type type in EditorHelper.SearchTypes(IsSystemType))
            {
                AddItem(type);
            }

            menu.ShowAsContext();

            bool IsSystemType(Type type)
            {
                return type.IsAbstract == false && typeof(Manager).IsAssignableFrom(type);
            }

            void AddItem(Type type)
            {
                if (HasTargetTypeSystem(type))
                {
                    menu.AddDisabledItem(new GUIContent(type.Name), true);
                }
                else
                {
                    menu.AddItem(new GUIContent(type.Name), false, () =>
                    {
                        ICollectionResolver resolver = (ICollectionResolver) Property.ChildResolver;
                        Object              manager  = ScriptableObject.CreateInstance(type);
                        manager.name = type.Name;
                        resolver.QueueAdd(new object[] {manager});
                    });
                }
            }

            bool HasTargetTypeSystem(Type type)
            {
                foreach (var child in Property.Children)
                {
                    if (child.ValueEntry.WeakSmartValue?.GetType() == type)
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    }

    class ManagerDrawer : OdinValueDrawer<Manager>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (GUILayout.Button(GUIHelper.TempContent(Property.ValueEntry.WeakSmartValue?.GetType().Name)))
            {
            }
        }
    }
}