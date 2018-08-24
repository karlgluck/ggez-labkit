using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.IO;

namespace GGEZ
{

public static class EditorMenuForScriptTemplates
{
    static readonly string WindowsScriptPath = @"C:\Program Files\Unity\Editor\Data\Resources\ScriptTemplates";
    static readonly string WindowsScriptPathHub = @"C:\Program Files\Unity\Hub\Editor\{0}\Editor\Data\Resources\ScriptTemplates";
    static readonly string OsxScriptPath = @"/Applications/Unity/Unity.app/Contents/Resources/ScriptTemplates";

    static readonly string CsharpScriptFileName = @"81-C# Script-NewBehaviourScript.cs.txt";

    private static string getScriptPath()
    {
        string version = InternalEditorUtility.GetFullUnityVersion().Split(' ')[0];
    #if UNITY_EDITOR_WIN
        string path = string.Format(WindowsScriptPathHub, version, CsharpScriptFileName);
        if (Directory.Exists(path))
        {
            return path;
        }
        else if (Directory.Exists(WindowsScriptPath))
        {
            return WindowsScriptPath;
        }
    #elif UNITY_EDITOR_OSX
        Debug.LogError ("This code is untested!! Script path might be wrong for OSX, especially when installed using Unity Hub.");
        if (Directory.Exists(OsxScriptPath))
        {
            return OsxScriptPath;
        }
    #endif
        throw new NotImplementedException();
    }

    [MenuItem("Edit/Script Templates/C# Script Template")]
    public static void Edit_ScriptTemplates_CsharpScript()
    {
        InternalEditorUtility.OpenFileAtLineExternal(Path.Combine (getScriptPath(), CsharpScriptFileName), 0);
    }
}

}

