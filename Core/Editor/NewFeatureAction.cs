using System;
using System.IO;
using Echo;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEditorInternal;

namespace EchoEditor
{
    sealed class NewFeatureAction : EndNameEditAction
    {
        [MenuItem("Assets/Create/" + nameof(Echo) + "/Feature")]
        private static void Execute()
        {
            NewFeatureAction action = CreateInstance<NewFeatureAction>();
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, action, "New Feature", EditorIcons.UnityFolderIcon, String.Empty);
        }

        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            string featureName = Path.GetFileName(pathName);
            Directory.CreateDirectory(pathName);
            Directory.CreateDirectory($"{pathName}/Editor");
            Directory.CreateDirectory($"{pathName}/Runtime");
            AssemblyDefinitionAsset echo       = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>("Packages/com.echo.core/Core/Runtime/Echo.asmdef");
            AssemblyDefinitionAsset echoEditor = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>("Packages/com.echo.core/Core/Editor/EchoEditor.asmdef");
            AssemblyDefinitionAsset runtime    = CreateAssembly($"{pathName}/Runtime", $"Echo.{featureName}",       false, echo);
            AssemblyDefinitionAsset editor     = CreateAssembly($"{pathName}/Editor",  $"EchoEditor.{featureName}", true,  echo, echoEditor, runtime);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            AssemblyDefinitionAsset CreateAssembly(string folder, string assembly, bool editorOnly, params AssemblyDefinitionAsset[] dependencies)
            {
                Coder text = new Coder();
                text.PushBlock();
                text.AppendLine($"\"name\": \"{assembly}\",");
                text.AppendLine("\"rootNamespace\": \"\",");
                if (dependencies.Length > 0)
                {
                    text.AppendLine("\"references\": [");
                    text.IndentLevel++;
                    for (int i = 0; i < dependencies.Length; i++)
                    {
                        string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(dependencies[i]));
                        text.AppendLine(i + 1 == dependencies.Length ? $"\"GUID:{guid}\"" : $"\"GUID:{guid}\",");
                    }

                    text.IndentLevel--;
                    text.AppendLine("],");
                }
                else
                {
                    text.AppendLine("\"references\": [],");
                }

                if (editorOnly)
                {
                    text.AppendLine("\"includePlatforms\": [");
                    text.IndentLevel++;
                    text.AppendLine("\"Editor\"");
                    text.IndentLevel--;
                    text.AppendLine("],");
                }
                else
                {
                    text.AppendLine("\"includePlatforms\": [],");
                }

                text.AppendLine("\"excludePlatforms\": [],");
                text.AppendLine("\"allowUnsafeCode\": false,");
                text.AppendLine("\"overrideReferences\": false,");
                text.AppendLine("\"precompiledReferences\": [],");
                text.AppendLine("\"autoReferenced\": true,");
                text.AppendLine("\"defineConstraints\": [],");
                text.AppendLine("\"versionDefines\": [],");
                text.AppendLine("\"noEngineReferences\": false");
                text.PopBlock();
                string assetPath = $"{folder}/{assembly}.asmdef";
                File.WriteAllText(assetPath, text.ToString());
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                return AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(assetPath);
            }
        }
    }
}