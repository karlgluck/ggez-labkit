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

using System;
using UnityEngine;
using UnityObject = UnityEngine.Object;

#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
#endif

namespace GGEZ
{
    public static partial class ObjectExt
    {
        public static UnityObject FindByName(this UnityObject[] self, string name)
        {
            for (int i = 0; i < self.Length; ++i)
            {
                if (self[i].name.Equals(name))
                {
                    return self[i];
                }
            }
            return null;
        }

#if UNITY_EDITOR

        /// <summary>
        ///     Call this from OnBeforeSerialize.
        ///
        ///     Keeps a field referencing a ScriptableObject valid regardless
        ///     of whether the owning UnityObject is saved in a scene or a prefab.
        /// </summary>
        public static void RelocateScriptableObjectField<T>(this UnityObject self, string fieldName, ref T scriptableObject) where T : ScriptableObject
        {
            // Ignore invalid references
            if (scriptableObject == null)
                return;

            // Ignore an object that is explicitly its own asset file
            if (AssetDatabase.IsMainAsset(scriptableObject))
                return;


            switch (PrefabUtility.GetPrefabType(self))
            {
                case PrefabType.PrefabInstance:
                {
                    // Grab this object's prefab
                    UnityObject selfInPrefab = PrefabUtility.GetCorrespondingObjectFromSource(self);

                    string selfPrefabAssetPath = AssetDatabase.GetAssetOrScenePath(selfInPrefab);

                    // Copy the scriptable object when a prefab exists, 'self' has not explicitly
                    // modified this object field, and yet the object not parented to the prefab
                    string scriptableObjectAssetPath = AssetDatabase.GetAssetPath(scriptableObject);
                    if (scriptableObjectAssetPath != selfPrefabAssetPath)
                    {
                        PropertyModification[] modifications = PrefabUtility.GetPropertyModifications(self);
                        bool unmodified = true;
                        for (int i = 0; i < modifications.Length; ++i)
                        {
                            if (modifications[i].propertyPath == fieldName && !object.ReferenceEquals(modifications[i].objectReference, scriptableObject))
                                unmodified = false;
                        }

                        if (unmodified)
                        {
                            if (!string.IsNullOrEmpty(scriptableObjectAssetPath))
                            {
                                // Break the reference to the object in the old asset
                                string oldName = scriptableObject.name;
                                scriptableObject = ScriptableObject.Instantiate(scriptableObject);
                                scriptableObject.name = oldName;
                            }

                            // Move the object into the prefab
                            AssetDatabase.AddObjectToAsset(scriptableObject, selfPrefabAssetPath);

                            FieldInfo field = self.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic);
                            Debug.Assert(object.ReferenceEquals(field.GetValue(self), scriptableObject), "was not passed the same scriptable object value that the field '" + fieldName + "' contains");

                            // Remove the object that was there before. This happens if the prefab
                            // connection is broken then re-applied.
                            T existingObject = AssetDatabase.LoadAssetAtPath<T>(selfPrefabAssetPath);
                            if (existingObject != null && !object.ReferenceEquals(existingObject, scriptableObject))
                                GameObject.DestroyImmediate(existingObject, true);

                            // Replace the field in the prefab version of self
                            field.SetValue(selfInPrefab, scriptableObject);

                            EditorUtility.SetDirty(selfInPrefab);
                            EditorUtility.SetDirty(self);
                            AssetDatabase.ImportAsset(selfPrefabAssetPath);

                        }
                    }
                }
                break;

            case PrefabType.None:
            case PrefabType.MissingPrefabInstance:
            case PrefabType.DisconnectedPrefabInstance:
                {
                    // If we aren't reading from a prefab, the scriptableObject should be stored inside the current object
                    string scriptableObjectAssetPath = AssetDatabase.GetAssetPath(scriptableObject);
                    if (scriptableObjectAssetPath != null)
                    {
                        if (!string.IsNullOrEmpty(scriptableObjectAssetPath))
                        {
                            string oldName = scriptableObject.name;
                            scriptableObject = ScriptableObject.Instantiate(scriptableObject);
                            scriptableObject.name = oldName;
                        }
                        EditorUtility.SetDirty(self);
                    }
                }
                break;

            case PrefabType.Prefab:
                {
                    // If the asset being referenced isn't from this prefab, it means the prefab
                    // asset was duplicated. Create a copy for us and reassign our field's value.
                    string selfPrefabAssetPath = AssetDatabase.GetAssetPath(self);
                    if (AssetDatabase.GetAssetPath(scriptableObject) != selfPrefabAssetPath)
                    {
                        string oldName = scriptableObject.name;
                        scriptableObject = ScriptableObject.Instantiate(scriptableObject);
                        scriptableObject.name = oldName;
                        AssetDatabase.AddObjectToAsset(scriptableObject, selfPrefabAssetPath);
                        EditorUtility.SetDirty(self);
                        AssetDatabase.ImportAsset(selfPrefabAssetPath);
                    }
                }
                break;
            }

        }


        // public static void RelocateScriptableObjectField<T>(this UnityObject self, string fieldName, ref T scriptableObject) where T : ScriptableObject
        // {
        //     switch (PrefabUtility.GetPrefabType(self))
        //     {

        //         case PrefabType.PrefabInstance:
        //         {
        //             UnityObject selfInPrefab = PrefabUtility.GetCorrespondingObjectFromSource(self);

        //             // If we don't have an object yet, pull the current value from the prefab
        //             if (scriptableObject == null)
        //             {
        //                 FieldInfo fieldInfo = self.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic);
        //                 scriptableObject = fieldInfo.GetValue(selfInPrefab) as T;
        //             }

        //             // Compare the object I'm using to the one the prefab is using
        //             string myPath = AssetDatabase.GetAssetPath(selfInPrefab);
        //             string objectPath = AssetDatabase.GetAssetPath(scriptableObject);
        //             if (objectPath != null && objectPath != myPath)
        //             {
        //                 // A prefab was instantiated and the instance was used to create a new prefab
        //                 string name = scriptableObject.name;
        //                 scriptableObject = ScriptableObject.Instantiate(scriptableObject);
        //                 scriptableObject.name = name;
        //                 objectPath = null;
        //             }

        //             // This was an attempt to remove the bolding modfication on the gameobject
        //             // that is used to create the prefab. It doesn't really matter because the
        //             // value is correct, but it would be nice to make it go away. Unfortunately,
        //             // this totally doesn't work.

        //             // // Remove the modification of the scriptable object field itself
        //             // PropertyModification[] modifications = null;
        //             // bool foundModification = false;
        //             // if (objectPath == null)
        //             // {
        //             //     modifications = PrefabUtility.GetPropertyModifications(self);
        //             //     for (int j = 0; j < modifications.Length; ++j)
        //             //     {
        //             //         if (modifications[j].propertyPath == fieldName)
        //             //         {
        //             //             Debug.Log("Found and removed modification " + modifications[j].propertyPath);
        //             //             modifications[j] = modifications[modifications.Length - 1];
        //             //             System.Array.Resize(ref modifications, modifications.Length - 1);
        //             //             foundModification = true;
        //             //             break;
        //             //         }
        //             //     }
        //             // }

        //             if (objectPath == null)
        //             {
        //                 // Put the object into a subasset of the prefab
        //                 AssetDatabase.AddObjectToAsset(scriptableObject, myPath);
        //                 scriptableObject.hideFlags = HideFlags.HideInHierarchy;

        //                 // Replace the field in the prefab instance of this object
        //                 FieldInfo field = self.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic);
        //                 Debug.Assert(object.ReferenceEquals(field.GetValue(self), scriptableObject), "was not passed the same scriptable object value that the field '" + fieldName + "' contains");

        //                 ScriptableObject existingValue = field.GetValue(selfInPrefab) as ScriptableObject;
        //                 if (!object.ReferenceEquals(scriptableObject, existingValue) && !object.ReferenceEquals(existingValue, null))
        //                 {
        //                     GameObject.DestroyImmediate(existingValue, true);
        //                 }

        //                 // Change the field value on the source
        //                 field.SetValue(selfInPrefab, scriptableObject);

        //                 // Make sure the asset database reloads
        //                 EditorUtility.SetDirty(selfInPrefab);
        //                 EditorUtility.SetDirty(self);
        //                 AssetDatabase.Refresh();

        //                 // PrefabUtility.SetPropertyModifications(self, modifications);
        //             }
        //         }
        //         break;

        //     case PrefabType.None:
        //     case PrefabType.MissingPrefabInstance:
        //     case PrefabType.DisconnectedPrefabInstance:
        //         {
        //             // Make sure we stop referencing the copy of the object in the original prefab
        //             string objectPath = scriptableObject == null ? null : AssetDatabase.GetAssetPath(scriptableObject);
        //             if (objectPath != null)
        //             {
        //                 string name = scriptableObject.name;
        //                 scriptableObject = ScriptableObject.Instantiate(scriptableObject);
        //                 scriptableObject.name = name;
        //                 EditorUtility.SetDirty(self);
        //             }
        //         }
        //         break;
        //     }
        // }

#endif
    }
}
