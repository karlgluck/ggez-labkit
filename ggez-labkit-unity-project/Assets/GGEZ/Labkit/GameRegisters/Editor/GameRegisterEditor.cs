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
using System.Reflection;

namespace GGEZ
{



[
CustomEditor(typeof (GameRegister), true),
CanEditMultipleObjects
]
public class GameRegisterEditor : Editor
{
private SerializedProperty initialValue;
private SerializedProperty willChangeValue;
private SerializedProperty didChangeValue;

private FieldInfo runtimeValueField;

// have a list of runtime properties here and set their values
// by copying whichever one's type matches from the serializedObject's .object field

// draw the property field for whichever type

// then set all values for the serializedobject's .object property by
// cloning the values from the property into the .object

void OnEnable ()
    {
    this.initialValue    = this.serializedObject.FindProperty ("initialValue");
    this.willChangeValue = this.serializedObject.FindProperty ("willChangeValue");
    this.didChangeValue  = this.serializedObject.FindProperty ("didChangeValue");
    this.runtimeValueField = this.target.GetType ().GetField ("runtimeValue", BindingFlags.NonPublic | BindingFlags.Instance);
    }


public override void OnInspectorGUI ()
    {
    if (this.initialValue != null)
        {
        EditorGUILayout.PropertyField (this.initialValue);
        }

    EditorGUI.BeginDisabledGroup (!Application.isPlaying);

    // For all objects, get the 1 runtime value
    // Add an edit field

    var runtimeValue = this.runtimeValueField.GetValue (this.target);
    var type = runtimeValue.GetType ();
    if (type.IsAssignableFrom (typeof(float)))
        {
        EditorGUILayout.FloatField ("Runtime Value", (float)runtimeValue);
        }

    EditorGUI.EndDisabledGroup ();

    EditorGUILayout.Space ();
    GUILayout.Label ("Events", EditorStyles.boldLabel);
    EditorGUILayout.PropertyField (this.willChangeValue);
    EditorGUILayout.PropertyField (this.didChangeValue);

    this.serializedObject.ApplyModifiedProperties ();
    }
}
}
