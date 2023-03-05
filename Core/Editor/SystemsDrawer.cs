using System;
using Echo;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.Drawers;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EchoEditor
{
    [DrawerPriority(DrawerPriorityLevel.WrapperPriority)]
    class SystemsDrawer : OdinValueDrawer<GameSystem[]>
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
                return type.IsAbstract == false && typeof(GameSystem).IsAssignableFrom(type);
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

    class SystemDrawer : OdinValueDrawer<GameSystem>
    {
        private PropertyTree m_Tree;

        protected override void Initialize()
        {
            base.Initialize();
            m_Tree = PropertyTree.Create(ValueEntry.SmartValue);
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            string systemName = Property.ValueEntry.WeakSmartValue?.GetType().Name;
            Property.State.Expanded =  SirenixEditorGUI.Foldout(Property.State.Expanded, GUIHelper.TempContent(systemName)) && m_Tree.RootProperty.Children.Count > 0;
            if (SirenixEditorGUI.BeginFadeGroup(UniqueDrawerKey.Create(Property, this), Property.State.Expanded))
            {
                EditorGUI.indentLevel++;
                m_Tree.Draw();
                EditorGUI.indentLevel--;
            }
            
            SirenixEditorGUI.EndFadeGroup();
        }
    }
}