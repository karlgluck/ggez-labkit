
using UnityEngine;
using UnityEditor;
using GGEZ;

namespace GGEZ
{
namespace Labkit
{

[CustomEditor (typeof(LabkitProjectSettings))]
public class LabkitProjectSettingsEditor : Editor
{

void OnDisable ()
    {
    AssetDatabase.SaveAssets ();
    }

public override void OnInspectorGUI ()
	{
    LabkitProjectSettings t = this.target as LabkitProjectSettings;
    if (t == null)
        {
        return;
        }

    t.name = "Labkit Settings";

    var serializedObject = new SerializedObject (t);
	GUI.changed = false;
    bool anythingChanged = false;

	GUILayout.Label ("Asset Processing", EditorStyles.boldLabel);

    t.TextureDefaults = (LabkitProjectSettings_TextureDefaults)EditorGUILayout.EnumPopup ("Texture Defaults", t.TextureDefaults);
    anythingChanged = anythingChanged || GUI.changed;
    GUI.changed = false;

    bool inPixelPerfect2DMode = t.TextureDefaults == LabkitProjectSettings_TextureDefaults.PixelPerfect2D;

	GUILayout.Label ("2D", EditorStyles.boldLabel, GUILayout.MinWidth (100f));
    GUILayout.BeginHorizontal ();
    GUILayout.Label ("Pixels Per Unit", GUILayout.ExpandWidth (true));
    float powerOfTwo = GUILayout.HorizontalSlider (Mathf.Log (t.PixelsPerUnit) / Mathf.Log (2), 0, 8, GUILayout.ExpandWidth (true), GUILayout.MinWidth (50f));
    int pixelsPerUnit = Mathf.NextPowerOfTwo (Mathf.RoundToInt (Mathf.Pow (2, powerOfTwo)));
    pixelsPerUnit = EditorGUILayout.IntField (pixelsPerUnit, GUILayout.Width (50f), GUILayout.ExpandWidth (false));
    GUILayout.EndHorizontal ();
    t.PixelsPerUnit = pixelsPerUnit;
    anythingChanged = anythingChanged || GUI.changed;
    GUI.changed = false;

	GUILayout.Label ("Development", EditorStyles.boldLabel);
    EditorGUILayout.PropertyField (serializedObject.FindProperty ("BreakNonPowerOfTwoTextures"));
    if (inPixelPerfect2DMode && !t.BreakNonPowerOfTwoTextures)
        {
        GUILayout.Label ("Turn this on for pixel-perfect 2d", EditorStyles.miniLabel);
        EditorGUILayout.Space ();
        }

    anythingChanged = anythingChanged || GUI.changed;
    GUI.changed = false;
    EditorGUILayout.PropertyField (serializedObject.FindProperty ("PurpleEditorInPlayMode"));
    if (GUI.changed)
        {
        bool purple = serializedObject.FindProperty ("PurpleEditorInPlayMode").boolValue;
        EditorPrefs.SetString ("Playmode tint", purple ? "Playmode tint;1;0.4;1;1" : "Playmode tint;1;1;1;1");
        }

    anythingChanged = anythingChanged || GUI.changed;
    GUI.changed = false;
    t.MetaFilesInVersionControl = EditorGUILayout.Toggle ("Meta Files in Version Control", t.MetaFilesInVersionControl);
    if (GUI.changed && t.MetaFilesInVersionControl)
        {
        UnityEditor.EditorSettings.serializationMode = SerializationMode.ForceText;
        UnityEditor.EditorSettings.externalVersionControl = "Visible Meta Files";
        }

	GUILayout.Label ("Optimization", EditorStyles.boldLabel);

    anythingChanged = anythingChanged || GUI.changed;
    GUI.changed = false;
    t.DisableAccelerometer = EditorGUILayout.Toggle ("Disable Accelerometer", t.DisableAccelerometer);
    if (GUI.changed)
        {
        UnityEditor.PlayerSettings.accelerometerFrequency = t.DisableAccelerometer ? 0 : 60;
        }

    anythingChanged = anythingChanged || GUI.changed;
    GUI.changed = false;

    t.DontAutoSimulate2DPhysics = EditorGUILayout.Toggle ("Disable 2D Physics", t.DontAutoSimulate2DPhysics);
    t.DontAutoSimulate3DPhysics = EditorGUILayout.Toggle ("Disable 3D Physics", t.DontAutoSimulate3DPhysics);
    

    if (GUI.changed || anythingChanged)
        {
        serializedObject.ApplyModifiedProperties ();
        EditorUtility.SetDirty (this.target);
        }
    }

}
}
}
