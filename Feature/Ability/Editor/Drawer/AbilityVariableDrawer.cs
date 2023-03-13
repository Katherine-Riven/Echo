using System;
using System.Collections.Generic;
using System.Linq;
using Echo.Abilities;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

// ReSharper disable UnusedType.Global

namespace EchoEditor.Abilities
{
    class AbilityVariableTableDrawer<T> : OdinValueDrawer<T> where T : ICollection<IAbilityVariable>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            SirenixEditorGUI.BeginVerticalList();
            SirenixEditorGUI.BeginHorizontalToolbar();
            GUILayout.Label(label);
            if (SirenixEditorGUI.ToolbarButton(EditorIcons.Plus))
            {
                AddNewVariable();
            }

            SirenixEditorGUI.EndHorizontalToolbar();

            EditorGUI.indentLevel++;
            foreach (InspectorProperty variable in Property.Children)
            {
                SirenixEditorGUI.BeginListItem(false);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical();
                GUIHelper.PushHierarchyMode(false);
                variable.Draw(null);
                GUIHelper.PopHierarchyMode();
                EditorGUILayout.EndVertical();
                if (RemoveButton())
                {
                    if (IsUsing(variable))
                    {
                        EditorUtility.DisplayDialog("提示", "该变量正在使用，因此无法删除，先取消引用后再删除。", "好的");
                    }
                    else
                    {
                        Property.Tree.DelayAction(() =>
                        {
                            ((ICollectionResolver) Property.ChildResolver).QueueRemove(new[]
                            {
                                variable.ValueEntry.WeakSmartValue,
                            });
                            Property.ValueEntry.ApplyChanges();
                        });
                    }
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(2f);
                SirenixEditorGUI.EndListItem();
            }

            EditorGUI.indentLevel--;

            SirenixEditorGUI.EndVerticalList();
        }

        private bool IsUsing(InspectorProperty variable)
        {
            foreach (InspectorProperty child in Property.Tree.EnumerateTree(true, true))
            {
                if (child == variable)
                {
                    continue;
                }

                if (child.ValueEntry?.WeakSmartValue == variable.ValueEntry.WeakSmartValue)
                {
                    return true;
                }
            }

            return false;
        }

        private bool RemoveButton()
        {
            Rect rect = GUILayoutUtility.GetRect(GUIContent.none, SirenixGUIStyles.IconButton, GUILayout.Width(18f), GUILayout.ExpandHeight(true));
            if (GUI.Button(rect, GUIContent.none, SirenixGUIStyles.IconButton))
            {
                GUIHelper.RemoveFocusControl();
                return true;
            }

            if (Event.current.type == EventType.Repaint)
            {
                float drawSize = Mathf.Min(rect.height, rect.width);
                EditorIcons.X.Draw(rect, drawSize);
            }

            return false;
        }

        private void AddNewVariable()
        {
            GenericSelector<Type> selector = new GenericSelector<Type>(string.Empty, false, AbilityEditorUtility.VariableItems);
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
                resolver.QueueAdd(new[] {newVariable});
                resolver.ApplyChanges();
                Property.ValueEntry.ApplyChanges();
            };

            selector.ShowInPopup();
        }
    }

    [DrawerPriority(value: 3000)]
    class AbilityVariableDrawer<T> : OdinValueDrawer<T> where T : IAbilityVariable
    {
        private InspectorProperty m_NameProperty;
        private InspectorProperty m_ValueProperty;
        private string            m_EditingName;

        protected override void DrawPropertyLayout(GUIContent label)
        {
            m_NameProperty  ??= Property.FindChild(x => x.Name == "m_Name",  false);
            m_ValueProperty ??= Property.FindChild(x => x.Name == "m_Value", false);
            bool isNameEmpty = string.IsNullOrEmpty(m_NameProperty.ValueEntry.WeakSmartValue as string);
            if (isNameEmpty)
            {
                EditorGUILayout.HelpBox("Name can't be empty.", MessageType.Error);
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(typeof(T).Name, SirenixGUIStyles.DropDownMiniButton, SirenixGUIStyles.BoldLabel);
            DrawName();
            EditorGUILayout.EndHorizontal();
            m_ValueProperty.Draw(GUIHelper.TempContent(string.IsNullOrEmpty(ValueEntry.SmartValue.Name) ? "Value" : ValueEntry.SmartValue.Name));

            void DrawName()
            {
                GUIHelper.PushColor(isNameEmpty ? Color.red : GUI.color);
                GUIContent content = GUIHelper.TempContent(m_NameProperty.ValueEntry.WeakSmartValue as string);
                Rect       rect    = GUILayoutUtility.GetRect(content, SirenixGUIStyles.DropDownMiniButton);
                if (GUI.Button(rect, content, SirenixGUIStyles.DropDownMiniButton))
                {
                    PopupWindow.Show(rect, new AbilityVariableSelectContent(rect.width, (x) =>
                    {
                        m_NameProperty.ValueEntry.WeakSmartValue = MakeUnique(x);
                        m_NameProperty.ValueEntry.ApplyChanges();
                    }));
                }

                GUIHelper.PopColor();
            }
        }

        private string MakeUnique(string name)
        {
            string uniqueName = name;
            int    tryTimes   = 1;
            while (IsUnique() == false)
            {
                uniqueName = $"{name}{tryTimes}";
                tryTimes++;
            }

            return uniqueName;

            bool IsUnique()
            {
                foreach (InspectorProperty child in Property.Parent.Children)
                {
                    if (child == Property) continue;
                    if (((IAbilityVariable) child.ValueEntry.WeakSmartValue).Name == uniqueName)
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }

    class AbilityVariableSelectContent : PopupWindowContent
    {
        private const string VariablesPath = "m_Variables";

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
                    string            path                  = AssetDatabase.GUIDToAssetPath(guid);
                    AbilityProfile    abilityProfile        = AssetDatabase.LoadAssetAtPath<AbilityProfile>(path);
                    Ability           ability               = AbilityEditorUtility.GetAbility(abilityProfile);
                    PropertyTree      abilityTree           = PropertyTree.Create(ability);
                    InspectorProperty variableArrayProperty = abilityTree.GetPropertyAtPath(VariablesPath);
                    foreach (var variableProperty in variableArrayProperty.Children)
                    {
                        InspectorProperty variableNameProperty = variableProperty.FindChild(x => x.Name == "m_Name", false);
                        string            variableName         = variableNameProperty.ValueEntry.WeakSmartValue?.ToString();
                        if (string.IsNullOrEmpty(variableName))
                        {
                            continue;
                        }

                        existVariables.Add(variableName);
                    }

                    abilityTree.Dispose();
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

            private bool   m_IsEditing;
            private string m_EditingName;

            public void OnGUI(Variable[] variables, Action<string> onSure)
            {
                SirenixEditorGUI.BeginHorizontalToolbar();
                if (m_IsEditing)
                {
                    m_EditingName = SirenixEditorFields.TextField(m_EditingName);
                    GUIHelper.PushGUIEnabled(string.IsNullOrEmpty(m_EditingName) == false && variables.Any(x => x.Name == m_EditingName && x != this) == false);
                    if (SirenixEditorGUI.ToolbarButton(EditorIcons.Checkmark))
                    {
                        if (string.IsNullOrEmpty(m_EditingName) || variables.Any(x => x.Name == m_EditingName && x != this))
                        {
                            return;
                        }

                        string oldName = Name;
                        m_IsEditing = false;
                        Name        = m_EditingName;
                        string[] assets = AssetDatabase.FindAssets($"t:{nameof(AbilityProfile)}");
                        foreach (string guid in assets)
                        {
                            string            path                  = AssetDatabase.GUIDToAssetPath(guid);
                            AbilityProfile    abilityProfile        = AssetDatabase.LoadAssetAtPath<AbilityProfile>(path);
                            Ability           ability               = AbilityEditorUtility.GetAbility(abilityProfile);
                            PropertyTree      abilityTree           = PropertyTree.Create(ability);
                            InspectorProperty variableArrayProperty = abilityTree.GetPropertyAtPath(VariablesPath);
                            foreach (InspectorProperty variableProperty in variableArrayProperty.Children)
                            {
                                InspectorProperty variableNameProperty = variableProperty.FindChild(x => x.Name == "m_Name", false);
                                string            variableName         = variableNameProperty.ValueEntry.WeakSmartValue.ToString();
                                if (variableName != oldName)
                                {
                                    continue;
                                }

                                variableNameProperty.ValueEntry.WeakSmartValue = Name;
                            }

                            if (abilityTree.ApplyChanges())
                            {
                                SerializedObject abilitySerializedObject = new SerializedObject(abilityProfile);
                                abilitySerializedObject.Update();
                                abilitySerializedObject.FindProperty("m_Json").stringValue = JsonUtility.ToJson(ability);
                                abilitySerializedObject.ApplyModifiedPropertiesWithoutUndo();
                                abilitySerializedObject.Dispose();
                            }

                            abilityTree.Dispose();
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
                        m_IsEditing   = true;
                        m_EditingName = Name;
                    }
                }

                SirenixEditorGUI.EndHorizontalToolbar();
            }
        }
    }
}