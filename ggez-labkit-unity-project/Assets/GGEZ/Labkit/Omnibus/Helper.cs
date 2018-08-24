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
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace GGEZ.Omnibus
{

public static class Helper
{

    //-----------------------------------------------------
    // FindAssetInPrefab
    //-----------------------------------------------------
    public static T FindAssetInPrefab<T>(UnityEngine.Object obj) where T : ScriptableObject
    {
#if UNITY_EDITOR
		var prefab = PrefabUtility.GetCorrespondingObjectFromSource(obj);
		if (prefab == null)
		{
			return null;
		}
		var prefabPath = AssetDatabase.GetAssetPath(prefab);
		T settings = AssetDatabase.LoadAssetAtPath<T>(prefabPath);
		if (settings == null)
		{
			settings = ScriptableObject.CreateInstance<T>();
			AssetDatabase.AddObjectToAsset(settings, prefabPath);
			settings.name = typeof(T).Name;
            AssetDatabase.Refresh();
		}
		return settings;
#else
        throw new InvalidOperationException();
#endif
    }

//     //-----------------------------------------------------
//     // LinkAssetInPrefab
//     //-----------------------------------------------------
//     public static void LinkAssetInPrefab<T>(MonoBehaviour component, string fieldName) where T : ScriptableObject
//     {
// #if UNITY_EDITOR
// 		var prefab = PrefabUtility.GetCorrespondingObjectFromSource(component);
// 		if (prefab == null)
// 		{
//             return;
// 		}
// 		var prefabPath = AssetDatabase.GetAssetPath(prefab);
// 		T settings = AssetDatabase.LoadAssetAtPath<T>(prefabPath);
// 		if (settings == null)
// 		{
// 			settings = ScriptableObject.CreateInstance<T>();
// 			AssetDatabase.AddObjectToAsset(settings, prefabPath);
// 			settings.name = typeof(T).Name;
//             AssetDatabase.Refresh();
// 		}
//         var componentType = component.GetType();
//         var field = componentType.GetField(fieldName);
//         field.SetValue(component, settings);
//         field.SetValue((prefab as GameObject).GetComponent(componentType), settings);
// #else
//         throw new InvalidOperationException();
// #endif
//     }

}

}
