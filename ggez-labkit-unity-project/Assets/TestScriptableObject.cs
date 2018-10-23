using UnityEngine;
using UnityEditor;

public class TestScriptableObject : ScriptableObject, ISerializationCallbackReceiver
{
    public string Text = "Initializer Text";

    void Reset()
    {
        Text = "Reset Text";
    }

    public void OnBeforeSerialize()
    {
        // Debug.Log("OnBeforeSerialize Text = " + Text);
    }

    public void OnAfterDeserialize()
    {
        // Debug.Log("OnAfterDeserialize Text = " + Text);
    }

    void OnValidate()
    {
        // Debug.Log("OnValidate Text = " + Text);
        Debug.Log("Is TestScriptableObject #" + GetInstanceID() + " a prefab? ==> " + PrefabUtility.GetPrefabType(this));
    }
}
