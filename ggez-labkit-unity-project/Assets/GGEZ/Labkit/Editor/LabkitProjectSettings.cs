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
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;
using GGEZ;

namespace GGEZ
{

public enum LabkitProjectSettings_TextureDefaults
    {
    Off,
    PixelPerfect2D
    };

[InitializeOnLoad]
public class LabkitProjectSettings : ScriptableObject
{
private static LabkitProjectSettings _instance;
public static LabkitProjectSettings Instance
    {
    get
        {
        if (LabkitProjectSettings._instance == null)
            {
            LabkitProjectSettings.scanForInstance ();
            }
        return LabkitProjectSettings._instance;
        }
    }

static LabkitProjectSettings ()
    {
    EditorApplication.update += OnLoad;
    }

static void OnLoad ()
    {
    try
        {
        LabkitProjectSettings.scanForInstance ();
        }
    catch
        {
        }
    if (LabkitProjectSettings._instance != null)
        {
        LabkitProjectSettings._instance.ApplySettingsToProject ();
        }
    EditorApplication.update -= OnLoad;
    }

public LabkitProjectSettings_TextureDefaults TextureDefaults = LabkitProjectSettings_TextureDefaults.Off;

[Range (1, 256)]
public int PixelsPerUnit = 16;
public bool BreakNonPowerOfTwoTextures = true;
public bool DisableAccelerometer = true;
public bool PurpleEditorInPlayMode = true;
public bool MetaFilesInVersionControl = true;
public bool DontAutoSimulate2DPhysics = false;
public bool DontAutoSimulate3DPhysics = false;
public bool UseVisualStudioCode = false;




public void ApplySettingsToProject ()
    {
    if (this.DisableAccelerometer)
        {
        UnityEditor.PlayerSettings.accelerometerFrequency = 0;
        }
    if (this.PurpleEditorInPlayMode)
        {
        EditorPrefs.SetString ("Playmode tint", "Playmode tint;1;0.4;1;1");
        }
    if (this.MetaFilesInVersionControl)
        {
        UnityEditor.EditorSettings.serializationMode = SerializationMode.ForceText;
        UnityEditor.EditorSettings.externalVersionControl = "Visible Meta Files";
        }
    Physics2D.autoSimulation = !this.DontAutoSimulate2DPhysics;
    Physics.autoSimulation = !this.DontAutoSimulate3DPhysics;
    if (this.UseVisualStudioCode)
        {
        var vsCodePath = findVisualStudioCode ();
        if (vsCodePath != null)
            {
            EditorPrefs.SetString ("kScriptsDefaultApp", vsCodePath);
            EditorPrefs.SetString ("kScriptEditorArgs", "$(File)");
            }
        }
    }





[
    MenuItem ("Edit/Project Settings/Labkit Settings"),
    MenuItem ("Labkit/Labkit Settings")
]
static void SelectLabkitProjectSettings ()
    {
    Selection.SetActiveObjectWithContext (LabkitProjectSettings.Instance, null);
    }




const string projectSettingsAssetPath = "Assets/GGEZ/Labkit/LabkitSettings.asset";




static void scanForInstance ()
    {
    var allAssets = AssetDatabase.LoadAllAssetsAtPath (projectSettingsAssetPath);
    LabkitProjectSettings settings = null;
    bool newSettings = allAssets.Length == 0;
    if (newSettings)
        {
        string tempAssetPath = AssetDatabase.GenerateUniqueAssetPath ("Assets/Labkit Settings.asset");
        AssetDatabase.CreateAsset (ScriptableObject.CreateInstance (typeof(LabkitProjectSettings)), tempAssetPath);
        if (File.Exists (projectSettingsAssetPath))
            {
            File.Delete (projectSettingsAssetPath);
            }
        File.Move (tempAssetPath, projectSettingsAssetPath);
        AssetDatabase.Refresh ();
        allAssets = AssetDatabase.LoadAllAssetsAtPath (projectSettingsAssetPath);
        }
    if (allAssets.Length == 0)
        {
        throw new System.InvalidOperationException ("Couldn't load or create settings asset");
        }
    settings = allAssets[0] as LabkitProjectSettings;
    if (newSettings)
        {
        initializeNewSettings (settings);
        }
    LabkitProjectSettings._instance = settings;
    }



static void initializeNewSettings (LabkitProjectSettings settings)
    {
    var vsCodePath = findVisualStudioCode ();
    if (vsCodePath != null)
        {
        settings.UseVisualStudioCode = true;
        }
    }

public static bool CanFindVisualStudioCode ()
    {
    return findVisualStudioCode () != null;
    }


#if UNITY_EDITOR_WIN
static string findVisualStudioCode ()
    {
    var possiblePaths = new string[] {
            (Environment.GetEnvironmentVariable("ProgramFiles") ?? "") + @"\Microsoft VS Code\Code.exe",
            (Environment.GetEnvironmentVariable("ProgramFiles") ?? "") + @"\Microsoft VS Code Insiders\Code.exe",
            (Environment.GetEnvironmentVariable("ProgramFiles(x86)") ?? "") + @"\Microsoft VS Code\Code.exe",
            (Environment.GetEnvironmentVariable("ProgramFiles(x86)") ?? "") + @"\Microsoft VS Code Insiders\Code.exe",
            };
    foreach (var path in possiblePaths)
        {
        if (System.IO.File.Exists (path))
            {
            return path;
            }
        }
    return null;
    }

#elif UNITY_EDITOR_OSX

static string findVisualStudioCode ()
    {
    var possiblePaths = new string[] {
            "/Applications/Visual Studio Code.app",
            "/Applications/Visual Studio Code - Insiders.app",
            };
    foreach (var path in possiblePaths)
        {
        if (System.IO.Directory.Exists (path))
            {
            return path;
            }
        }
    return null;
    }
#endif

}
}
