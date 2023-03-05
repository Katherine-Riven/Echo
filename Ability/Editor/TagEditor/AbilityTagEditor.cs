using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Echo;
using Echo.Abilities;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace EchoEditor.Abilities
{
    sealed class AbilityTagEditor : OdinEditorWindow
    {
        [MenuItem(nameof(Echo) + "/" + nameof(AbilityTagEditor))]
        private static void ShowWindow()
        {
            var window = GetWindow<AbilityTagEditor>("Ability Tag Editor");
            window.Show();
        }

        [SerializeField] [LabelText("Ability Tags")] [ListDrawerSettings(Expanded = true, HideRemoveButton = true, CustomAddFunction = nameof(AddNewTag))]
        private List<TagConfig> Children = new List<TagConfig>();

        private static AbilityTagEditor s_Current;

        protected override void OnEnable()
        {
            base.OnEnable();
            s_Current = this;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            s_Current = null;
        }

        private void CreateGUI()
        {
            ReloadTagConfig(Children);
            UpdateEditors();
        }

        protected override void OnGUI()
        {
            SirenixEditorGUI.BeginHorizontalToolbar();
            if (GUILayout.Button("Reset", SirenixGUIStyles.ToolbarButton, GUILayout.ExpandWidth(true)))
            {
                ResetTags();
            }

            if (GUILayout.Button("Save", SirenixGUIStyles.ToolbarButton, GUILayout.ExpandWidth(true)))
            {
                Save();
            }

            SirenixEditorGUI.EndHorizontalToolbar();
            base.OnGUI();
        }

        private void ResetTags()
        {
            ReloadTagConfig(Children);
            UpdateEditors();
        }

        private void Save()
        {
            Selection.activeObject = null;
            List<TagConfig> valueChangedTags = new List<TagConfig>();
            int             tagCounter       = 0;
            foreach (var child in Children)
            {
                child.GenerateTagValue(ref tagCounter, valueChangedTags);
            }

            CodeAbilityTag(Children);
            UpdateAbilityTag(valueChangedTags);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private string MakeUniqueName(string tagName)
        {
            string uniqueName = tagName;
            int    index      = 1;
            while (AnTagIsName(uniqueName))
            {
                uniqueName = $"{tagName}_{index}";
                index++;
            }

            return uniqueName;

            bool AnTagIsName(string targetName)
            {
                foreach (var config in Children)
                {
                    if (HasName(config, targetName))
                    {
                        return true;
                    }
                }

                return false;
            }

            bool HasName(TagConfig config, string targetName)
            {
                if (config.Name == targetName) return true;
                foreach (var child in config.Children)
                {
                    if (HasName(child, targetName))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        private static void AddNewTag(InspectorProperty property)
        {
            ICollectionResolver resolver = (ICollectionResolver) property.ChildResolver;
            resolver.QueueAdd(new object[] {new TagConfig(string.Empty, String.Empty, default)});
            resolver.ApplyChanges();
            property.ValueEntry.ApplyChanges();
        }

        private static void CodeAbilityTag(List<TagConfig> rootTags)
        {
            Coder coder = new Coder();
            coder.AppendLine("using UnityEngine;");
            coder.AppendLine();
            using (coder.NewNamespace(nameof(Echo) + "." + nameof(Abilities)))
            {
                using (coder.NewStruct("public partial", "AbilityTag"))
                {
                    CodeChildren(rootTags, "public static readonly", "AbilityTagPack.");
                }

                using (coder.NewClass("public partial", "AbilityTagPack"))
                {
                    List<TagConfig> tagPacks = new List<TagConfig>();
                    foreach (var child in rootTags)
                    {
                        CollectTagPack(child);
                    }

                    foreach (TagConfig tag in tagPacks)
                    {
                        CodeTagPack(tag);
                    }

                    void CollectTagPack(TagConfig tag)
                    {
                        if (tag.Children.Count == 0) return;
                        tagPacks.Add(tag);
                        foreach (var child in tag.Children)
                        {
                            CollectTagPack(child);
                        }
                    }
                }
            }

            File.WriteAllText("Packages/Echo/Ability/Runtime/AutoCode/AbilityTag.Definition.cs", coder.ToString(), Encoding.UTF8);

            void CodeChildren(List<TagConfig> children, string modifier, string typeDefine)
            {
                foreach (TagConfig child in children)
                {
                    string fieldType = child.Children.Count > 0 ? $"{typeDefine}{child.Name}" : "AbilityTag";
                    string arg       = Convert.ToString(child.Value, 2).PadLeft(32, '0');
                    if (string.IsNullOrEmpty(child.InspectName) == false)
                    {
                        coder.AppendLine($"[InspectorName(\"{child.InspectName}\")]");
                    }

                    coder.AppendLine($"{modifier} {fieldType} {child.Name} = new (0b{arg});");
                }
            }

            void CodeTagPack(TagConfig tag)
            {
                using (coder.NewClass("public", $"{tag.Name}", "AbilityTagPack"))
                {
                    coder.AppendLine($"public {tag.Name}(int tag) : base(tag) {{ }}");
                    CodeChildren(tag.Children, "public readonly", String.Empty);
                }
            }
        }

        private static void UpdateAbilityTag(List<TagConfig> valueChangedTags)
        {
            const string TagField = "m_TagValue";

            if (valueChangedTags.Count == 0) return;

            string[] assets = AssetDatabase.FindAssets("t:ScriptableObject");
            for (int i = 0; i < assets.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(assets[i]);
                if (ContainsAbilityTag(path))
                {
                    ScriptableObject   asset            = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
                    SerializedObject   serializedObject = new SerializedObject(asset);
                    SerializedProperty iterator         = serializedObject.GetIterator();
                    while (iterator.Next(true))
                    {
                        if (iterator.type.Contains(nameof(AbilityTag)) == false)
                        {
                            continue;
                        }

                        SerializedProperty tagValueProperty = iterator.FindPropertyRelative(TagField);
                        if (tagValueProperty == null)
                        {
                            continue;
                        }

                        int oldValue = tagValueProperty.intValue;
                        int newValue = 0;
                        foreach (TagConfig renamedTag in valueChangedTags)
                        {
                            // 判断旧值是否包含重命名的值
                            if ((oldValue & renamedTag.OriginValue) != renamedTag.OriginValue)
                            {
                                continue;
                            }

                            // 把重命名的源值从旧值中移除
                            oldValue &= ~renamedTag.OriginValue;
                            // 把重命名的当前值放到新值中
                            newValue |= renamedTag.Value;
                        }

                        // 重命名后的值等于去掉重命名前的值的原值 加上 重命名后的值
                        tagValueProperty.intValue = newValue | oldValue;
                    }

                    if (serializedObject.ApplyModifiedPropertiesWithoutUndo())
                    {
                        EditorUtility.SetDirty(asset);
                    }

                    serializedObject.Dispose();
                }
            }

            bool ContainsAbilityTag(string file)
            {
                string[] lines = File.ReadAllLines(file);
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].AsSpan().TrimStart(' ').StartsWith(TagField))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        private static void ReloadTagConfig(List<TagConfig> rootTags)
        {
            rootTags.Clear();
            FieldInfo[] fields = typeof(AbilityTag).GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (FieldInfo field in fields)
            {
                if (field.Name == nameof(AbilityTag.None) || field.Name == nameof(AbilityTag.All))
                {
                    continue;
                }

                rootTags.Add(Load(null, field));
            }

            TagConfig Load(object target, FieldInfo field)
            {
                object    fieldValue = field.GetValue(target);
                TagConfig config     = new TagConfig(field.Name, field.GetAttribute<InspectorNameAttribute>()?.displayName, fieldValue is AbilityTag tag ? tag : default);
                if (field.FieldType.IsSubclassOf(typeof(AbilityTagPack)))
                {
                    FieldInfo[] children = field.FieldType.GetFields(BindingFlags.Public | BindingFlags.Instance);
                    foreach (FieldInfo child in children)
                    {
                        config.Children.Add(Load(fieldValue, child));
                    }
                }

                return config;
            }
        }

        [Serializable]
        private class TagConfig
        {
            public TagConfig()
            {
                Name        = String.Empty;
                InspectName = string.Empty;
            }

            public TagConfig(string name, string inspectName, AbilityTag value)
            {
                m_OriginValue = value;
                Name          = name;
                InspectName   = inspectName;
            }

            [HideInInspector] [SerializeField] private int    m_OriginValue;
            [HideInInspector] [SerializeField] public  string Name;
            [HideInInspector] [SerializeField] public  string InspectName;

            [SerializeField] [LabelText("@GetHeader")] [ListDrawerSettings(Expanded = true, HideRemoveButton = true, OnTitleBarGUI = nameof(OnTitleBarGUI), CustomAddFunction = nameof(ChildAddNewTag))]
            public List<TagConfig> Children = new List<TagConfig>();

            private bool   m_IsEditing;
            private string m_EditingNameText;
            private string m_EditingInspectNameText;

            private string GetHeader => string.IsNullOrEmpty(Name) ? "尚未命名" : string.IsNullOrEmpty(InspectName) ? Name : $"{Name} ({InspectName})";

            public int OriginValue => m_OriginValue;
            public int Value       { get; private set; }

            private static void ChildAddNewTag(InspectorProperty property) => AddNewTag(property);

            private void OnTitleBarGUI()
            {
                if (m_IsEditing || string.IsNullOrEmpty(Name))
                {
                    Event e = Event.current;
                    EditorGUILayout.BeginVertical();
                    m_EditingNameText        = SirenixEditorFields.TextField("名字", m_EditingNameText)        ?? String.Empty;
                    m_EditingInspectNameText = SirenixEditorFields.TextField("备注", m_EditingInspectNameText) ?? String.Empty;
                    EditorGUILayout.EndVertical();
                    if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
                    {
                        m_IsEditing = false;
                        e.Use();
                    }
                    else if (SirenixEditorGUI.ToolbarButton(EditorIcons.Checkmark))
                    {
                        m_IsEditing              = false;
                        m_EditingNameText        = m_EditingNameText.Replace(" ", string.Empty);
                        m_EditingInspectNameText = m_EditingInspectNameText.Replace(" ", string.Empty);
                        if (string.IsNullOrEmpty(m_EditingNameText) == false && Name != m_EditingNameText)
                        {
                            Name = s_Current.MakeUniqueName(m_EditingNameText);
                        }

                        if (string.IsNullOrEmpty(m_EditingNameText) == false && InspectName != m_EditingInspectNameText)
                        {
                            InspectName = m_EditingInspectNameText;
                        }
                    }
                }
                else if (SirenixEditorGUI.ToolbarButton("Edit"))
                {
                    m_IsEditing              = true;
                    m_EditingNameText        = Name;
                    m_EditingInspectNameText = InspectName;
                }
            }

            public void GenerateTagValue(ref int tagCounter, List<TagConfig> valueChangedTags)
            {
                if (Children.Count > 0)
                {
                    foreach (var child in Children)
                    {
                        child.GenerateTagValue(ref tagCounter, valueChangedTags);
                        Value |= child.Value;
                    }
                }
                else
                {
                    Value = 1 << tagCounter;
                    tagCounter++;
                    if (OriginValue != 0 && OriginValue != Value)
                    {
                        valueChangedTags.Add(this);
                    }
                }
            }
        }
    }
}