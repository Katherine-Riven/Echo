using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Lucifer.Editor
{
    [CustomEditor(typeof(GameManager))]
    public class GameManagerEditor : UnityEditor.Editor
    {
        private SerializedProperty m_LaunchStage;
        private ReorderableList    m_Systems;

        private void OnEnable()
        {
            m_LaunchStage = serializedObject.FindProperty(nameof(m_LaunchStage));
            m_Systems     = CreateSystemList();
        }

        public override void OnInspectorGUI()
        {
            bool deleteAnyNull = false;
            for (int i = m_Systems.serializedProperty.arraySize - 1; i >= 0; i--)
            {
                if (m_Systems.serializedProperty.GetArrayElementAtIndex(i).objectReferenceValue == null)
                {
                    m_Systems.serializedProperty.DeleteArrayElementAtIndex(i);
                    deleteAnyNull = true;
                }
            }

            if (deleteAnyNull)
            {
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_LaunchStage);
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }

            m_Systems.DoLayoutList();
        }

        private ReorderableList CreateSystemList()
        {
            ReorderableList systemList = new ReorderableList(serializedObject, serializedObject.FindProperty(nameof(m_Systems)), true, true, true, false);
            systemList.drawHeaderCallback            = DrawListHeader;
            systemList.drawElementCallback           = DrawListElement;
            systemList.drawNoneElementCallback       = DrawListNoneElement;
            systemList.drawElementBackgroundCallback = DrawListElementBackground;
            systemList.onChangedCallback             = ChangeCallback;
            systemList.onAddDropdownCallback         = AddDropdown;
            return systemList;

            void DrawListHeader(Rect rect)
            {
                EditorGUI.LabelField(rect, EditorHelper.TempContent("System Sequence"));
            }

            void DrawListElement(Rect rect, int index, bool isActive, bool isFocused)
            {
                if (index >= systemList.serializedProperty.arraySize) return;
                rect = rect.AlignCenterY(20f);
                Object system = systemList.serializedProperty.GetArrayElementAtIndex(index).objectReferenceValue;
                GUI.Button(rect.AlignLeft(rect.width - 30f), system.GetType().Name, ReorderableList.defaultBehaviours.preButton);
                if (GUI.Button(rect.AlignRight(30f), ReorderableList.defaultBehaviours.iconToolbarMinus, ReorderableList.defaultBehaviours.preButton))
                {
                    systemList.serializedProperty.DeleteArrayElementAtIndex(index);
                    systemList.onChangedCallback?.Invoke(systemList);
                }
            }

            void DrawListNoneElement(Rect rect)
            {
                EditorGUI.LabelField(rect, "There is no system!");
            }

            void DrawListElementBackground(Rect rect, int index, bool isActive, bool isFocused)
            {
            }

            void ChangeCallback(ReorderableList list)
            {
                list.serializedProperty.serializedObject.ApplyModifiedProperties();
            }

            void AddDropdown(Rect rect, ReorderableList list)
            {
                GenericMenu menu = new GenericMenu();
                foreach (Type type in EditorHelper.SearchTypes(IsSystemType))
                {
                    AddItem(type);
                }

                menu.DropDown(rect);

                bool IsSystemType(Type type)
                {
                    return type.IsAbstract == false && typeof(System).IsAssignableFrom(type);
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
                            list.serializedProperty.arraySize++;
                            Object system = CreateInstance(type);
                            system.name = type.Name;
                            SerializedProperty newProperty = list.serializedProperty.GetArrayElementAtIndex(list.serializedProperty.arraySize - 1);
                            newProperty.objectReferenceValue = system;
                            list.onChangedCallback?.Invoke(list);
                        });
                    }
                }

                bool HasTargetTypeSystem(Type type)
                {
                    for (int i = 0; i < systemList.serializedProperty.arraySize; i++)
                    {
                        Object system = systemList.serializedProperty.GetArrayElementAtIndex(i).objectReferenceValue;
                        if (system.GetType() == type)
                        {
                            return true;
                        }
                    }

                    return false;
                }
            }
        }
    }
}