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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace GGEZ
{
    [CustomEditor(typeof(AssetPackageConfig))]
    public class AssetPackageConfigEditor : Editor
    {
        [MenuItem("Labkit/Build All")]
        private static void BuildAll()
        {
            foreach (var obj in AssetDatabase.FindAssets("t:AssetPackageConfig")
                    .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                    .Select(path => AssetDatabase.LoadAssetAtPath(path, typeof(AssetPackageConfig))))
            {
                (obj as AssetPackageConfig).Build();
            }
        }

        public override void OnInspectorGUI()
        {
            GUI.changed = false;
            var t = this.target as AssetPackageConfig;
            t.AssetPackageName = EditorGUILayout.TextField("Name", t.AssetPackageName);
            EditorGUILayout.Space();
            EditorGUI.BeginDisabledGroup(t.Assets == null || t.Assets.Length < 1);
            if (GUILayout.Button("Build"))
            {
                t.Build();
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space();
            GUILayout.Label("Assets ", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            if (t.Assets == null)
            {
                t.Assets = new Object[0];
            }
            for (int i = t.Assets.Length - 1; i >= 0; --i)
            {
                var assetObject = t.Assets[i];
                bool shouldDelete = assetObject == null;
                if (!shouldDelete)
                {
                    EditorGUILayout.BeginHorizontal();
                    shouldDelete = GUILayout.Button("x", EditorStyles.miniButton, GUILayout.Width(17f));
                    bool isAnotherAssetPackage = assetObject as AssetPackageConfig != null;
                    Color oldBackgroundColor = GUI.backgroundColor;
                    if (isAnotherAssetPackage)
                    {
                        GUI.backgroundColor = Color.green;
                    }
                    assetObject = EditorGUILayout.ObjectField(GUIContent.none, assetObject, typeof(Object), false);
                    GUI.backgroundColor = oldBackgroundColor;
                    shouldDelete = shouldDelete || assetObject == null;
                    EditorGUILayout.EndHorizontal();
                }
                if (shouldDelete)
                {
                    for (int a = i; a < t.Assets.Length - 1; ++a)
                    {
                        t.Assets[a] = t.Assets[a + 1];
                    }
                    System.Array.Resize(ref t.Assets, t.Assets.Length - 1);
                }
                else
                {
                    t.Assets[i] = assetObject;
                }
            }
            EditorGUILayout.Space();
            var newAssets = AssetPackageConfigEditor.dropAssetsArea("Drop New Assets Here", "Add {0} Asset(s)");
            if (newAssets != null)
            {
                int oldLength = t.Assets.Length;
                System.Array.Resize(ref t.Assets, oldLength + newAssets.Length);
                newAssets.CopyTo(t.Assets, oldLength);
                t.Assets = t.Assets.Distinct().ToArray();
            }
            if (GUI.changed)
            {
                EditorUtility.SetDirty(this.target);
            }
        }


        private static Object[] dropAssetsArea(string idlePrompt, string dropPrompt)
        {
            Event e = Event.current;
            Rect drop_area = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
            bool isDragPerformEvent = e.type == EventType.DragPerform;
            bool isAnyDragEvent = e.type == EventType.DragUpdated || isDragPerformEvent;
            bool isMouseInArea = drop_area.Contains(e.mousePosition);
            bool dragAndDropHasSomething = DragAndDrop.objectReferences != null && DragAndDrop.objectReferences.Length > 0;

            var centeredStyle = GUI.skin.GetStyle("Box");
            centeredStyle.alignment = TextAnchor.MiddleCenter;
            GUI.Box(
                    drop_area,
                    isMouseInArea && dragAndDropHasSomething
                            ? string.Format(dropPrompt, DragAndDrop.objectReferences.Length)
                            : idlePrompt,
                    centeredStyle
                    );

            if (isAnyDragEvent && isMouseInArea)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                if (isDragPerformEvent)
                {
                    DragAndDrop.AcceptDrag();
                    return DragAndDrop.objectReferences;
                }
            }

            return null;
        }
    }
}
