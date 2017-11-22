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




class CreateNewGameTableRegisterDialog : EditorWindow
{
public string Folder = "";
public string Name = "";
public string CSharpType = "";


[MenuItem ("Labkit/Create/Game Register Type/Table")]
static void LabkitNewGameTableRegisterType ()
    {
    const string savedPathKey = "LabkitSaveFilePanelPath";
    string path = EditorUtility.SaveFilePanelInProject (
            "New Game Register Type",
            "type.cs",
            "cs",
            "Name the source file <type>.cs",
            EditorPrefs.GetString (savedPathKey, "")
            );
    if (string.IsNullOrEmpty (path))
        {
        return;
        }
    EditorPrefs.SetString (savedPathKey, path);
    CreateNewGameTableRegisterDialog.CreateAndShow (path);
    }


public static EditorWindow CreateAndShow (string path)
    {
    var window = CreateNewGameTableRegisterDialog.CreateInstance<CreateNewGameTableRegisterDialog> ();
    var width = Screen.width;
    var height = Screen.height;
    Vector2 size = new Vector2 (400f, 180f);
    window.ShowAsDropDown (new Rect (width/2f-size.x/2f, height/2 - size.y/2f, 1f, 1f), size);
    window.titleContent = new GUIContent ("Create Table Register Type");
    window.Folder = Path.GetDirectoryName (path);
    window.Name = Path.GetFileNameWithoutExtension (path);
    window.CSharpType = window.Name;
    return window;
    }

public void DoWork ()
    {
    string upperName = this.Name.Substring (0, 1).ToUpper () + this.Name.Substring (1);
    string lowerName = this.Name.Substring (0, 1).ToLower () + this.Name.Substring (1);
    Debug.LogFormat ("Creating {0}TableRegister for data of type {1}", upperName, this.CSharpType);

    LabkitEditorUtility.WriteFileUsingTemplate (
            "GameTableRegister_So",
            Path.Combine (this.Folder, upperName + "TableRegister.cs"),
            "%NAME% " + this.Name,
            "%UPPERNAME% " + upperName,
            "%LOWERNAME% " + lowerName,
            "%CSHARPTYPE% " + this.CSharpType
            );

    LabkitEditorUtility.WriteFileUsingTemplate (
            "GameTableRegisterListener_Mb",
            Path.Combine (this.Folder, upperName + "TableRegisterListener.cs"),
            "%NAME% " + this.Name,
            "%UPPERNAME% " + upperName,
            "%LOWERNAME% " + lowerName,
            "%CSHARPTYPE% " + this.CSharpType
            );
    }

protected virtual void OnLostFocus ()
    {
    this.Close ();
    }

private void OnGUI()
    {
    const float titleHeight = 35f;

    GUILayout.BeginArea (new Rect(0, 0, this.position.width, this.position.height));
    GUIStyle titleStyle = new GUIStyle ("Toolbar");
    titleStyle.fontSize = 12;
    titleStyle.fontStyle = FontStyle.Bold;

    titleStyle.alignment = TextAnchor.MiddleCenter;
    GUILayout.BeginHorizontal (titleStyle, GUILayout.Height (titleHeight));
    GUILayout.Label (this.titleContent, titleStyle, GUILayout.ExpandWidth (true));
    GUILayout.EndHorizontal ();

    GUILayout.Space (16f);

    GUILayout.BeginHorizontal ();
    GUILayout.Label ("Path", GUILayout.Width (80f));
    this.Folder = GUILayout.TextField (this.Folder);
    GUILayout.EndHorizontal ();

    GUILayout.BeginHorizontal ();
    GUILayout.Label ("Name", GUILayout.Width (80f));
    this.Name = GUILayout.TextField (this.Name);
    GUILayout.EndHorizontal ();

    GUILayout.BeginHorizontal ();
    GUILayout.Label ("C# Type", GUILayout.Width (80f));
    this.CSharpType = GUILayout.TextField (this.CSharpType);
    GUILayout.EndHorizontal ();

    GUILayout.FlexibleSpace ();

    GUILayout.BeginHorizontal (GUILayout.Height (40f));
    EditorGUI.BeginDisabledGroup (!Directory.Exists (this.Folder));

    if (GUILayout.Button ("OK"))
        {
        this.DoWork ();
        this.Close ();
        }
    EditorGUI.EndDisabledGroup ();
    if (GUILayout.Button ("Cancel"))
        {
        this.Close ();
        }
    GUILayout.EndHorizontal ();

    GUILayout.FlexibleSpace ();

    GUILayout.EndArea ();
    }
}

}
}
