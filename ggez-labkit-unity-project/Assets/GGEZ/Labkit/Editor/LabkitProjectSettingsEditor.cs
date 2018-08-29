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
using GGEZ;
using System.IO;


namespace GGEZ.Labkit
{
    [CustomEditor(typeof(LabkitProjectSettings))]
    public class LabkitProjectSettingsEditor : Editor
    {
        // TODO: add menu item to write a default .GITIGNORE file

        private void OnDisable()
        {
            AssetDatabase.SaveAssets();
        }

        public override void OnInspectorGUI()
        {
            LabkitProjectSettings t = this.target as LabkitProjectSettings;
            if (t == null)
            {
                return;
            }

            t.name = "Labkit Settings";

            var serializedObject = new SerializedObject(t);
            GUI.changed = false;
            bool anythingChanged = false;

            //-----------------------------------------------------------------------------------
            GUILayout.Label("Asset Processing", EditorStyles.boldLabel);
            //-----------------------------------------------------------------------------------

            t.TextureDefaults = (LabkitProjectSettings_TextureDefaults)EditorGUILayout.EnumPopup("Texture Defaults", t.TextureDefaults);


            bool inPixelPerfect2DMode = t.TextureDefaults == LabkitProjectSettings_TextureDefaults.PixelPerfect2D;

            //-----------------------------------------------------------------------------------
            GUILayout.Label("2D", EditorStyles.boldLabel, GUILayout.MinWidth(100f));
            //-----------------------------------------------------------------------------------
            GUILayout.BeginHorizontal();
            GUILayout.Label("Pixels Per Unit", GUILayout.ExpandWidth(true));
            float powerOfTwo = GUILayout.HorizontalSlider(Mathf.Log(t.PixelsPerUnit) / Mathf.Log(2), 0, 8, GUILayout.ExpandWidth(true), GUILayout.MinWidth(50f));
            int pixelsPerUnit = Mathf.NextPowerOfTwo(Mathf.RoundToInt(Mathf.Pow(2, powerOfTwo)));
            pixelsPerUnit = EditorGUILayout.IntField(pixelsPerUnit, GUILayout.Width(50f), GUILayout.ExpandWidth(false));
            GUILayout.EndHorizontal();
            t.PixelsPerUnit = pixelsPerUnit;


            //-----------------------------------------------------------------------------------
            GUILayout.Label("Rendering", EditorStyles.boldLabel);
            //-----------------------------------------------------------------------------------
            UnityEditor.PlayerSettings.colorSpace = (ColorSpace)EditorGUILayout.EnumPopup("Color Space", UnityEditor.PlayerSettings.colorSpace);
            if (UnityEditor.PlayerSettings.colorSpace == ColorSpace.Linear)
            {
                GUILayout.Label("Careful! Linear is better but breaks older mobile devices.", EditorStyles.miniLabel);
                EditorGUILayout.Space();
            }

            //-----------------------------------------------------------------------------------
            GUILayout.Label("Development", EditorStyles.boldLabel);
            //-----------------------------------------------------------------------------------
            EditorGUILayout.PropertyField(serializedObject.FindProperty("BreakNonPowerOfTwoTextures"));
            if (inPixelPerfect2DMode && !t.BreakNonPowerOfTwoTextures)
            {
                GUILayout.Label("Turn this on to easily spot NPOT textures", EditorStyles.miniLabel);
                EditorGUILayout.Space();
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("PurpleEditorInPlayMode"));

            t.MetaFilesInVersionControl = EditorGUILayout.Toggle("Meta Files in Version Control", t.MetaFilesInVersionControl);

            //-----------------------------------------------------------------------------------
            GUILayout.Label("Optimization", EditorStyles.boldLabel);
            //-----------------------------------------------------------------------------------
            anythingChanged = anythingChanged || GUI.changed;
            GUI.changed = false;
            t.DisableAccelerometer = EditorGUILayout.Toggle("Disable Accelerometer", t.DisableAccelerometer);
            if (GUI.changed && !t.DisableAccelerometer)
            {
                UnityEditor.PlayerSettings.accelerometerFrequency = 60;
            }
            anythingChanged = anythingChanged || GUI.changed;
            GUI.changed = false;

            t.DontAutoSimulate2DPhysics = EditorGUILayout.Toggle("Disable 2D Physics", t.DontAutoSimulate2DPhysics);
            t.DontAutoSimulate3DPhysics = EditorGUILayout.Toggle("Disable 3D Physics", t.DontAutoSimulate3DPhysics);

            t.UseVisualStudioCode = EditorGUILayout.Toggle("Use Visual Studio Code", t.UseVisualStudioCode);
            if (!LabkitProjectSettings.CanFindVisualStudioCode())
            {
                GUILayout.Label("Can't find VS Code; is it installed?", EditorStyles.miniLabel);
            }
            EditorGUILayout.Space();


            //-----------------------------------------------------------------------------------
            GUILayout.Label("Fixes", EditorStyles.boldLabel);
            //-----------------------------------------------------------------------------------
            anythingChanged = anythingChanged || GUI.changed;
            EditorGUILayout.BeginHorizontal();
            string gitignorePath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, ".gitignore");
            bool gitignoreExists = File.Exists(gitignorePath);
            GUILayout.Label(".gitignore" + (gitignoreExists ? " (exists)" : ""));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Write"))
            {
                bool shouldWriteFile = true;
                if (gitignoreExists)
                {
                    shouldWriteFile = EditorUtility.DisplayDialog(
                            "Overwrite?",
                            "Overwrite project .gitignore file?",
                            "Overwrite",
                            "Keep Existing"
                            );
                }
                if (shouldWriteFile)
                {
                    LabkitEditorUtility.WriteFileUsingTemplate(
                            "gitignore",
                            gitignorePath
                            );
                }
            }
            EditorGUI.BeginDisabledGroup(!gitignoreExists);
            if (GUILayout.Button("Open"))
            {
                UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(
                        gitignorePath,
                        1
                        );
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
            anythingChanged = anythingChanged || GUI.changed;
            GUI.changed = false;


            if (GUI.changed || anythingChanged)
            {
                t.ApplySettingsToProject();
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(this.target);
            }
        }
    }
}
