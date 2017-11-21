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
using System.Collections;

namespace GGEZ
{



[
CustomEditor(typeof (GameRegister), true),
CanEditMultipleObjects
]
public class GameRegisterEditor : Editor
{
private SerializedProperty initialValue;
private SerializedProperty runtimeValue;

private FieldInfo listenersField;

void OnEnable ()
    {
    this.initialValue = this.serializedObject.FindProperty ("initialValue");
    this.runtimeValue = this.serializedObject.FindProperty ("runtimeValue");
    this.listenersField =
            this.targets.Length == 1
            ? this.target.GetType ().GetField ("listeners", BindingFlags.NonPublic | BindingFlags.Instance)
            : null;
    }


public override void OnInspectorGUI ()
    {
    this.serializedObject.UpdateIfRequiredOrScript ();

    if (this.initialValue != null)
        {
        EditorGUILayout.PropertyField (this.initialValue);
        }

    EditorGUI.BeginDisabledGroup (!Application.isPlaying);

    if (this.runtimeValue != null)
        {
        EditorGUILayout.PropertyField (this.runtimeValue);
        }

    if (this.listenersField != null)
        {
        var listeners = this.listenersField.GetValue (this.target) as ICollection;
        if (listeners != null)
            {
            EditorGUILayout.LabelField ("Listeners", listeners.Count.ToString ());
            }
        EditorGUI.EndDisabledGroup ();
        }

    EditorGUILayout.Space ();

    this.serializedObject.ApplyModifiedProperties ();
    }
}
}
