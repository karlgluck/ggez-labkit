using UnityEngine;
using UnityEditor;
using System.Reflection;


public class TestBehavior : MonoBehaviour, ISerializationCallbackReceiver
{
    public TestScriptableObject Object;

    public void OnBeforeSerialize()
    {
        // Debug.Log("OnBeforeSerialize " + AssetDatabase.GetAssetPath(this));
        GGEZ.ObjectExt.RelocateScriptableObjectField(this, "Object", ref Object);

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
        // Debug.Log("OnAfterDeserialize Text = " + (Object == null ? "(object is null)" : Object.Text));
    }

    void OnValidate()
    {
        // Debug.Log("OnValidate " + AssetDatabase.GetAssetPath(this));
        Object = Object ?? ScriptableObject.CreateInstance<TestScriptableObject>();
        // Debug.Log("OnValidate Text = " + (Object == null ? "(object is null)" : Object.Text));
        // Object = GGEZ.ObjectExt.ValidateOwnedScriptableObject(this, Object);
    }
}
