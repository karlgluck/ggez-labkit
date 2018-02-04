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

namespace GGEZ.Omnibus
{




class CreateOmnibusTypeSupport : EditorWindow
{
public string Folder = "";
public string Name = "";
public string CSharpDeclaringType = "";
public string CSharpTypeName = "";

private bool writeTypeSupport
    {
    get { return EditorPrefs.GetBool ("LabkitTypeSupport_writeTypeSupport", true); }
    set { EditorPrefs.SetBool ("LabkitTypeSupport_writeTypeSupport", value); }
    }
private bool writeTerminal
    {
    get { return EditorPrefs.GetBool ("LabkitTypeSupport_writeTerminal", true); }
    set { EditorPrefs.SetBool ("LabkitTypeSupport_writeTerminal", value); }
    }
private bool writeBuffer
    {
    get { return EditorPrefs.GetBool ("LabkitTypeSupport_writeBuffer", true); }
    set { EditorPrefs.SetBool ("LabkitTypeSupport_writeBuffer", value); }
    }
private bool writeUnityEventTerminal
    {
    get { return EditorPrefs.GetBool ("LabkitTypeSupport_writeUnityEventTerminal", true); }
    set { EditorPrefs.SetBool ("LabkitTypeSupport_writeUnityEventTerminal", value); }
    }
private bool writeUnityEventModule
    {
    get { return EditorPrefs.GetBool ("LabkitTypeSupport_writeUnityEventModule", true); }
    set { EditorPrefs.SetBool ("LabkitTypeSupport_writeUnityEventModule", value); }
    }
private bool writeMux
    {
    get { return EditorPrefs.GetBool ("LabkitTypeSupport_writeMux", true); }
    set { EditorPrefs.SetBool ("LabkitTypeSupport_writeMux", value); }
    }

[MenuItem ("Labkit/Create/Omnibus Type Support")]
static void CreateNewOmnibusRegisterType ()
    {
    string savedPathKey = "LabkitSaveFilePanelPath" + Application.productName;
    string path = EditorUtility.SaveFilePanelInProject (
            "Add Omnibus Type Support",
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
    CreateOmnibusTypeSupport.CreateAndShow (path);
    }


public static EditorWindow CreateAndShow (string path)
    {
    var window = CreateOmnibusTypeSupport.CreateInstance<CreateOmnibusTypeSupport> ();
    var width = Screen.width;
    var height = Screen.height;
    Vector2 size = new Vector2 (400f, 220f);
    window.ShowAsDropDown (new Rect (width/2f-size.x/2f, height/2 - size.y/2f, size.x, size.y), size);
    window.titleContent = new GUIContent ("Create Omnibus Type Support");
    window.Folder = Path.GetDirectoryName (path);
    window.Name = Path.GetFileNameWithoutExtension (path);
    window.CSharpDeclaringType = window.Name;
    window.CSharpTypeName = window.Name;
    return window;
    }

public void DoWork ()
    {

    Debug.LogFormat ("Creating type support for {0} serialized as {1} in MemoryCell", this.CSharpDeclaringType, this.CSharpTypeName);

    if (this.writeTypeSupport) this.write ("OmnibusTypeSupport_cs", "{0}TypeSupport.cs");
    if (this.writeTerminal) this.write ("OmnibusTypeTerminal_cs", "{0}Terminal.cs");
    if (this.writeBuffer) this.write ("OmnibusTypeBuffer_cs", "{0}Buffer.cs");
    if (this.writeUnityEventTerminal) this.write ("OmnibusTypeUnityEventTerminal_cs", "{0}UnityEventTerminal.cs");
    if (this.writeUnityEventModule) this.write ("OmnibusTypeUnityEventModule_cs", "{0}UnityEventModule.cs");
    if (this.writeMux) this.write ("OmnibusTypeMux_cs", "{0}Mux.cs");
    }

private void write (string template, string filenameFormat)
    {
    string upperName = this.Name.Substring (0, 1).ToUpper () + this.Name.Substring (1);
    string lowerName = this.Name.Substring (0, 1).ToLower () + this.Name.Substring (1);
    Labkit.LabkitEditorUtility.WriteFileUsingTemplate (
            template,
            Path.Combine (this.Folder, string.Format (filenameFormat, upperName)),
            "%NAME% " + this.Name,
            "%UPPERNAME% " + upperName,
            "%LOWERNAME% " + lowerName,
            "%CSHARPTYPE% " + this.CSharpDeclaringType,
            "%CSHARPTYPENAME% " + this.CSharpTypeName
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
    GUILayout.Label ("Path", GUILayout.Width (120f));
    this.Folder = GUILayout.TextField (this.Folder);
    GUILayout.EndHorizontal ();

    GUILayout.BeginHorizontal ();
    GUILayout.Label ("Name", GUILayout.Width (120f));
    this.Name = GUILayout.TextField (this.Name);
    GUILayout.EndHorizontal ();

    GUILayout.BeginHorizontal ();
    GUILayout.Label ("C# Declaring Type", GUILayout.Width (120f));
    this.CSharpDeclaringType = GUILayout.TextField (this.CSharpDeclaringType);
    GUILayout.EndHorizontal ();

    GUILayout.BeginHorizontal ();
    GUILayout.Label ("C# Type Name", GUILayout.Width (120f));
    this.CSharpTypeName = GUILayout.TextField (this.CSharpTypeName);
    GUILayout.EndHorizontal ();

    GUILayout.FlexibleSpace ();

    GUILayout.BeginHorizontal ();
    this.writeTypeSupport = GUILayout.Toggle (this.writeTypeSupport, "Type Support");
    GUILayout.EndHorizontal ();
    GUILayout.BeginHorizontal ();
    this.writeBuffer = GUILayout.Toggle (this.writeBuffer, "Buffer");
    this.writeTerminal = GUILayout.Toggle (this.writeTerminal, "Terminal");
    this.writeUnityEventTerminal = GUILayout.Toggle (this.writeUnityEventTerminal, "UnityEventTerminal");
    GUILayout.EndHorizontal ();
    GUILayout.BeginHorizontal ();
    this.writeUnityEventModule = GUILayout.Toggle (this.writeUnityEventModule, "UnityEventModule");
    this.writeMux = GUILayout.Toggle (this.writeMux, "Mux");
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
