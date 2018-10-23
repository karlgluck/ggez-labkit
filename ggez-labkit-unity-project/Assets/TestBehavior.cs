using UnityEngine;
using UnityEditor;
using System.Reflection;


public class TestBehavior : MonoBehaviour, ISerializationCallbackReceiver
{
    public TestScriptableObject Object;

    public void OnBeforeSerialize()
    {
        GGEZ.ObjectExt.RelocateScriptableObjectField(this, "Object", ref Object);
        // GGEZ.ObjectExt.RelocateScriptableObjectField(this, "Object", ref Object);

        // switch (PrefabUtility.GetPrefabType(this))
        // {

        //     case PrefabType.PrefabInstance:
        //     {
        //         Object selfInPrefab = PrefabUtility.GetCorrespondingObjectFromSource(this);

        //         string myPath = AssetDatabase.GetAssetPath(selfInPrefab);
        //         string objectPath = AssetDatabase.GetAssetPath(Object);

        //         if (objectPath != null && objectPath != myPath)
        //         {
        //             // A prefab was instantiated and the instance was used to create a new prefab
        //             string name = Object.name;
        //             Object = ScriptableObject.Instantiate(Object);
        //             Object.name = name;
        //             objectPath = null;
        //         }

        //         if (objectPath == null)
        //         {
        //             // Put the object into a subasset of the prefab
        //             AssetDatabase.AddObjectToAsset(Object, myPath);
        //             Object.hideFlags = HideFlags.HideInHierarchy;

        //             FieldInfo[] fields = this.GetType().GetFields(BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic);
        //             // PropertyModification[] modifications = PrefabUtility.GetPropertyModifications(this);
        //             for (int i = 0; i < fields.Length; ++i)
        //             {
        //                 FieldInfo field = fields[i];
        //                 if (object.ReferenceEquals(field.GetValue(this), Object))
        //                 {
        //                     Debug.LogWarning("Assigning field " + field.Name);
        //                     field.SetValue(selfInPrefab, Object);
        //                     EditorUtility.SetDirty(selfInPrefab);

        //                     // // Remove the modification on the scriptable object field
        //                     // for (int j = 0; j < modifications.Length; ++j)
        //                     // {
        //                     //     Debug.Log("Checking " + modifications[j].propertyPath);
        //                     //     if (modifications[j].propertyPath == field.Name)
        //                     //     {
        //                     //         modifications[j] = modifications[modifications.Length - 1];
        //                     //         System.Array.Resize(ref modifications, modifications.Length - 1);
        //                     //         break;
        //                     //     }
        //                     // }
        //                 }
        //             }
        //             // PrefabUtility.SetPropertyModifications(this, modifications);
        //             EditorUtility.SetDirty(this);
        //             AssetDatabase.Refresh();
        //         }
        //     }
        //     break;

        // case PrefabType.DisconnectedPrefabInstance:
        //     {
        //         string objectPath = AssetDatabase.GetAssetPath(Object);
        //         if (objectPath != null)
        //         {
        //             string name = Object.name;
        //             Object = ScriptableObject.Instantiate(Object);
        //             Object.name = name;
        //             EditorUtility.SetDirty(this);
        //         }
        //     }
        //     break;
        // }

    }

    [ContextMenu("Create Object")]
    void CreateObject()
    {
        Object = ScriptableObject.CreateInstance<TestScriptableObject>();
        EditorUtility.SetDirty(this);
    }

    [ContextMenu("Become Asset")]
    void BecomeAsset()
    {
        string path = AssetDatabase.GetAssetPath(PrefabUtility.GetCorrespondingObjectFromSource(this));
        Debug.Log("Adding to " + path);
        AssetDatabase.AddObjectToAsset(Object, path);
        EditorUtility.SetDirty(Object);
        EditorUtility.SetDirty(this);
    }

    public void OnAfterDeserialize()
    {
        Object = Object ?? ScriptableObject.CreateInstance<TestScriptableObject>();
        // Debug.Log("OnAfterDeserialize Text = " + (Object == null ? "(object is null)" : Object.Text));
    }

    void OnValidate()
    {
        // Debug.Log("OnValidate Text = " + (Object == null ? "(object is null)" : Object.Text));
        // Object = GGEZ.ObjectExt.ValidateOwnedScriptableObject(this, Object);
    }
}
