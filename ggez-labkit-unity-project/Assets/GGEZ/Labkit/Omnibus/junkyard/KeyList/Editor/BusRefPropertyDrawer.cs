// // This is free and unencumbered software released into the public domain.
// //
// // Anyone is free to copy, modify, publish, use, compile, sell, or
// // distribute this software, either in source code form or as a compiled
// // binary, for any purpose, commercial or non-commercial, and by any
// // means.
// //
// // In jurisdictions that recognize copyright laws, the author or authors
// // of this software dedicate any and all copyright interest in the
// // software to the public domain. We make this dedication for the benefit
// // of the public at large and to the detriment of our heirs and
// // successors. We intend this dedication to be an overt act of
// // relinquishment in perpetuity of all present and future rights to this
// // software under copyright law.
// //
// // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// // EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// // MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// // IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// // OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// // ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// // OTHER DEALINGS IN THE SOFTWARE.
// //
// // For more information, please refer to <http://unlicense.org/>

// using UnityEngine;
// using UnityEditor;
// using System.Collections;
// using UnityEditorInternal;
// using System.IO;

// namespace GGEZ
// {
// namespace Omnibus
// {

// [
// CustomPropertyDrawer (typeof(BusRef))
// ]
// public sealed class BusRefPropertyDrawer : PropertyDrawer
// {
// [MenuItem("Assets/Create/OMNIBUS BUS ASSET")]
// public static void CreateBusAsset ()
//     {
//     string assetPath = "Assets";
//     foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
// 		{
//         string path = AssetDatabase.GetAssetPath(obj);
//         if (string.IsNullOrEmpty (path))
//             {
//             continue;
//             }
//         string absolutePath = Path.Combine (Path.GetDirectoryName (Application.dataPath), path);
//         if (Directory.Exists (absolutePath))
// 			{
//             assetPath = path;
//             break;
// 			}
// 		}
//     assetPath = AssetDatabase.GenerateUniqueAssetPath (Path.Combine (assetPath, "New Bus Asset.prefab"));

//     var go = new GameObject ();
//     var prefab = PrefabUtility.CreatePrefab (assetPath, go);
//     prefab.AddComponent <Bus> ();
//     GameObject.DestroyImmediate (go);
//     Selection.activeGameObject = prefab;
//     EditorGUIUtility.PingObject (prefab);
//     }


// public override void OnGUI (
//         Rect position,
//         SerializedProperty property,
//         GUIContent label
//         )
//     {
//     var reference = property.FindPropertyRelative ("reference");
//     EditorGUI.BeginChangeCheck ();
//     var value = EditorGUI.ObjectField (position, label, reference.objectReferenceValue, typeof(System.Object), true);
//     value = EditorGUI.ObjectField (position, label, value ?? reference.objectReferenceValue, typeof(IBus), true);
//     if (EditorGUI.EndChangeCheck ())
//         {
//         reference.objectReferenceValue = value;
//         }
//     }
// }


// }

// }
