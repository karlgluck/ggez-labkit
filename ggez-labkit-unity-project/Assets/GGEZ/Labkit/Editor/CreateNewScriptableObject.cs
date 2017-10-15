// This is free and unencumbered software released into the public domain.
// 
// Anyone is free to copy, modify, publish, use, compile, sell, or
// distribute this software, either in source code form or as a compiled
// binary, for any purpose, commercial or non-commercial, and by any
// means.
// 
// In jurisdictions that recognize copyright laws, the author or authors
// of this software dedicate any and all copyright interest in the
// software to the public domain. We make this dedication for the benefit
// of the public at large and to the detriment of our heirs and
// successors. We intend this dedication to be an overt act of
// relinquishment in perpetuity of all present and future rights to this
// software under copyright law.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// 
// For more information, please refer to <http://unlicense.org/>

using UnityEngine;
using UnityEditor;
using System.IO;

namespace GGEZ
{
namespace Labkit
{

public static partial class LabkitEditorUtility
{
public static void WriteFileUsingTemplate (string templateAssetName, string path, params string[] args)
    {
    string[] assets;
    TextAsset template;
    string contents;
    
    assets = AssetDatabase.FindAssets (templateAssetName + " t:TextAsset");
    if (assets == null || assets.Length < 1)
        {
        throw new System.InvalidOperationException ("Missing " + templateAssetName);
        }
    template = AssetDatabase.LoadAssetAtPath (
            AssetDatabase.GUIDToAssetPath (assets[0]),
            typeof(TextAsset)
            ) as TextAsset;
    contents = template.text;
    foreach (string substitution in args)
        {
        string key = substitution.Substring (0, substitution.IndexOf (' '));
        string value = substitution.Substring (key.Length + 1);
        contents = contents.Replace (key, value);
        }
    File.WriteAllText (path, contents);
    AssetDatabase.Refresh (ImportAssetOptions.Default);
    }
}

public static class CreateNewScriptableObject
{

[MenuItem ("Labkit/Create/ScriptableObject")]
static void LabkitNewScriptableObject ()
    {
    string path = EditorUtility.SaveFilePanelInProject (
            "New Scriptable Object",
            "NewScriptableObject.cs",
            "cs",
            "Create a source file for the ScriptableObject"
            );
    if (string.IsNullOrEmpty (path))
        {
        return;
        }

    string newAssetName = System.IO.Path.GetFileNameWithoutExtension (path);
    LabkitEditorUtility.WriteFileUsingTemplate (
            "ScriptableObject_So",
            path,
            "%NEWASSETNAME% " + newAssetName
            );

    string editorPath = Path.Combine (Path.GetDirectoryName (path), "Editor");
    Directory.CreateDirectory (editorPath);
    LabkitEditorUtility.WriteFileUsingTemplate (
            "ScriptableObject_SoEditor",
            Path.Combine (editorPath, newAssetName + "Editor.cs"),
            "%NEWASSETNAME% " + newAssetName
            );
    }

[MenuItem ("Labkit/Create/MonoBehaviour with Settings")]
static void LabkitNewControllerClass ()
    {
    string path = EditorUtility.SaveFilePanelInProject (
            "New MonoBehaviour with Settings",
            "NewMonoBehaviour.cs",
            "cs",
            "Create a source file for the MonoBehaviour"
            );
    if (string.IsNullOrEmpty (path))
        {
        return;
        }

    string newAssetName = System.IO.Path.GetFileNameWithoutExtension (path);
    LabkitEditorUtility.WriteFileUsingTemplate (
            "MonoBehaviourWithSettings_Mb",
            path,
            "%NEWASSETNAME% " + newAssetName
            );
    LabkitEditorUtility.WriteFileUsingTemplate (
            "MonoBehaviourWithSettings_MbSo",
            Path.Combine (Path.GetDirectoryName (path), newAssetName + "Settings.cs"),
            "%NEWASSETNAME% " + newAssetName
            );

    string editorPath = Path.Combine (Path.GetDirectoryName (path), "Editor");
    Directory.CreateDirectory (editorPath);
    LabkitEditorUtility.WriteFileUsingTemplate (
            "MonoBehaviourWithSettings_MbEditor",
            Path.Combine (editorPath, newAssetName + "Editor.cs"),
            "%NEWASSETNAME% " + newAssetName
            );
    LabkitEditorUtility.WriteFileUsingTemplate (
            "MonoBehaviourWithSettings_MbSoEditor",
            Path.Combine (editorPath, newAssetName + "SettingsEditor.cs"),
            "%NEWASSETNAME% " + newAssetName
            );
    }

}
}
}
