using UnityEngine;
using UnityEditor;

public class TestScriptableObject : ScriptableObject, ISerializationCallbackReceiver
{
    public string Text = "Initializer Text";

    public TestScriptableObject[] Objects;

    void Reset()
    {
        Text = "Reset Text";
    }

    [ContextMenu("Add Object")]
    void AddObject()
    {
        if (Objects == null) Objects = new TestScriptableObject[0];
        System.Array.Resize(ref Objects, Objects.Length+1);
        var newOne = ScriptableObject.CreateInstance<TestScriptableObject>();
        Objects[Objects.Length - 1] = newOne;
        newOne.name = newOne.Text = System.DateTime.Now.ToString();
        EditorUtility.SetDirty(this);
    }

    public void OnBeforeSerialize()
    {
        GGEZ.ObjectExt.RelocateScriptableObjectArrayField(this, "Objects", ref Objects);
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

    public override string ToString()
    {
        return "{"+Text+"}";
    }
}
