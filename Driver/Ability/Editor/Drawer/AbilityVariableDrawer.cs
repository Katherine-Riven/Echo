using System;
using System.Collections.Generic;
using System.Linq;
using Echo.Abilities;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.Drawers;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace EchoEditor.Abilities
{
    [DrawerPriority(DrawerPriorityLevel.WrapperPriority)]
    class AbilityVariableCollectionDrawer<T> : OdinValueDrawer<T> where T : ICollection<IAbilityVariable>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            Action nextCustomAddFunction = CollectionDrawerStaticInfo.NextCustomAddFunction;
            CollectionDrawerStaticInfo.NextCustomAddFunction = AddNewVariable;
            CallNextDrawer(label);
            CollectionDrawerStaticInfo.NextCustomAddFunction = nextCustomAddFunction;
        }

        private void AddNewVariable()
        {
            GenericSelector<Type> selector = new GenericSelector<Type>(string.Empty, false, AbilityDrawerUtility.VariableItems);
            selector.EnableSingleClickToSelect();
            selector.SelectionConfirmed += delegate(IEnumerable<Type> types)
            {
                Type type = types.FirstOrDefault();
                if (type == null)
                {
                    return;
                }

                object              newVariable = Activator.CreateInstance(type);
                ICollectionResolver resolver    = (ICollectionResolver) Property.ChildResolver;
                resolver.QueueAdd(new object[] {newVariable});
                resolver.ApplyChanges();
                Property.ValueEntry.ApplyChanges();
            };

            selector.ShowInPopup(Event.current.mousePosition);
        }
    }

    [DrawerPriority(value: 2000)]
    class AbilityVariableDrawer<T> : OdinValueDrawer<T> where T : IAbilityVariable
    {
        private InspectorProperty m_NameProperty;
        private InspectorProperty m_ValueProperty;
        private string            m_EditingName;

        protected override void Initialize()
        {
            base.Initialize();
            m_NameProperty  = Property.FindChild(x => x.Name == "m_Name",  false);
            m_ValueProperty = Property.FindChild(x => x.Name == "m_Value", false);
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            bool isNameEmpty = string.IsNullOrEmpty(m_NameProperty.ValueEntry.WeakSmartValue as string);
            EditorGUILayout.BeginHorizontal();
            SirenixEditorGUI.Title(typeof(T).Name, string.Empty, TextAlignment.Left, false);
            if (isNameEmpty)
            {
                SirenixEditorGUI.ErrorMessageBox("Name can't be empty.");
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUIHelper.PushColor(isNameEmpty ? Color.red : GUI.color);
            EditorGUILayout.PrefixLabel(m_NameProperty.Label);
            GUIContent content = GUIHelper.TempContent(m_NameProperty.ValueEntry.WeakSmartValue as string);
            Rect       rect    = GUILayoutUtility.GetRect(content, SirenixGUIStyles.DropDownMiniButton);
            if (GUI.Button(rect, content, SirenixGUIStyles.DropDownMiniButton))
            {
                PopupWindow.Show(rect, new AbilityVariableSelectContent(rect.width, (name) =>
                {
                    m_NameProperty.ValueEntry.WeakSmartValue = name;
                    m_NameProperty.ValueEntry.ApplyChanges();
                }));
            }

            GUIHelper.PopColor();
            EditorGUILayout.EndHorizontal();

            m_ValueProperty.Draw();
        }
    }

    class AbilityVariableSelectContent : PopupWindowContent
    {
        private const string VariablesAbsolutePath    = "m_Ability.m_VariableTable.m_Variables";
        private const string VariableNameRelativePath = "m_Name";

        public AbilityVariableSelectContent(float width, Action<string> onSureVariable)
        {
            m_OnSureVariable = onSureVariable;
            m_WindowSize     = new Vector2(width, 200f);
            m_Variables      = GetVariableTree();

            Variable[] GetVariableTree()
            {
                HashSet<string> existVariables = new HashSet<string>();
                string[]        assets         = AssetDatabase.FindAssets($"t:{nameof(AbilityProfile)}");
                foreach (string guid in assets)
                {
                    string             path                  = AssetDatabase.GUIDToAssetPath(guid);
                    AbilityProfile     abilityProfile        = AssetDatabase.LoadAssetAtPath<AbilityProfile>(path);
                    SerializedObject   serializedObject      = new SerializedObject(abilityProfile);
                    SerializedProperty variableArrayProperty = serializedObject.FindProperty(VariablesAbsolutePath);
                    for (int i = 0; i < variableArrayProperty.arraySize; i++)
                    {
                        SerializedProperty variableProperty     = variableArrayProperty.GetArrayElementAtIndex(i);
                        SerializedProperty variableNameProperty = variableProperty.FindPropertyRelative(VariableNameRelativePath);
                        if (string.IsNullOrEmpty(variableNameProperty.stringValue))
                        {
                            continue;
                        }

                        existVariables.Add(variableNameProperty.stringValue);
                    }

                    serializedObject.Dispose();
                }

                string[] variableNames = existVariables.ToArray();
                Array.Sort(variableNames);
                return variableNames.Select(x => new Variable() {Name = x}).ToArray();
            }
        }

        private Action<string> m_OnSureVariable;
        private Vector2        m_WindowSize;
        private string         m_SearchTerm;
        private Vector2        m_ScrollPosition;
        private Variable[]     m_Variables;

        public override Vector2 GetWindowSize() => m_WindowSize;

        public override void OnGUI(Rect rect)
        {
            SirenixEditorGUI.BeginHorizontalToolbar();
            m_SearchTerm = SirenixEditorGUI.ToolbarSearchField(m_SearchTerm);
            GUIHelper.PushGUIEnabled(string.IsNullOrEmpty(m_SearchTerm) == false);
            if (SirenixEditorGUI.ToolbarButton(EditorIcons.Plus))
            {
                OnSure(m_SearchTerm);
            }

            GUIHelper.PopGUIEnabled();

            SirenixEditorGUI.EndHorizontalToolbar();
            m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);
            foreach (Variable variable in m_Variables)
            {
                if (string.IsNullOrEmpty(m_SearchTerm) || FuzzySearch.Contains(m_SearchTerm, variable.Name))
                {
                    variable.OnGUI(m_Variables, OnSure);
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private void OnSure(string name)
        {
            m_OnSureVariable.Invoke(name);
            editorWindow.Close();
        }

        private class Variable
        {
            public string Name;
            public bool   IsEditing;
            public string EditingName;

            public void OnGUI(Variable[] variables, Action<string> onSure)
            {
                SirenixEditorGUI.BeginHorizontalToolbar();
                if (IsEditing)
                {
                    EditingName = SirenixEditorFields.TextField(EditingName);
                    GUIHelper.PushGUIEnabled(string.IsNullOrEmpty(EditingName) == false && variables.Any(x => x.Name == EditingName && x != this) == false);
                    if (SirenixEditorGUI.ToolbarButton(EditorIcons.Checkmark))
                    {
                        if (string.IsNullOrEmpty(EditingName) || variables.Any(x => x.Name == EditingName && x != this))
                        {
                            return;
                        }

                        string oldName = Name;
                        IsEditing = false;
                        Name      = EditingName;
                        string[] assets = AssetDatabase.FindAssets($"t:{nameof(AbilityProfile)}");
                        foreach (string guid in assets)
                        {
                            string             path                  = AssetDatabase.GUIDToAssetPath(guid);
                            AbilityProfile     abilityProfile        = AssetDatabase.LoadAssetAtPath<AbilityProfile>(path);
                            SerializedObject   serializedObject      = new SerializedObject(abilityProfile);
                            SerializedProperty variableArrayProperty = serializedObject.FindProperty(VariablesAbsolutePath);
                            for (int i = 0; i < variableArrayProperty.arraySize; i++)
                            {
                                SerializedProperty variableProperty     = variableArrayProperty.GetArrayElementAtIndex(i);
                                SerializedProperty variableNameProperty = variableProperty.FindPropertyRelative(VariableNameRelativePath);
                                if (variableNameProperty.stringValue != oldName)
                                {
                                    continue;
                                }

                                variableNameProperty.stringValue = Name;
                            }

                            serializedObject.ApplyModifiedPropertiesWithoutUndo();
                            serializedObject.Dispose();
                        }
                    }

                    GUIHelper.PopGUIEnabled();
                }
                else
                {
                    if (GUILayout.Button(Name, EditorStyles.toolbarButton, GUILayout.ExpandWidth(true)))
                    {
                        onSure.Invoke(Name);
                    }

                    if (SirenixEditorGUI.ToolbarButton(EditorIcons.Refresh))
                    {
                        IsEditing   = true;
                        EditingName = Name;
                    }
                }

                SirenixEditorGUI.EndHorizontalToolbar();
            }
        }
    }
}