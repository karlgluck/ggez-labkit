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
using System.Collections.Generic;

namespace GGEZ
{



[
CustomEditor(typeof (GameTableRegister), true),
CanEditMultipleObjects
]
public class GameTableRegisterEditor : Editor
{
private SerializedProperty defaultValue;
private SerializedProperty initialTable;
private SerializedProperty runtimeTable;

private FieldInfo listenersTableField;

private bool enableEditingInitialValuesAtRuntime;

void OnEnable ()
    {
    this.defaultValue = this.serializedObject.FindProperty ("defaultValue");
    this.initialTable = this.serializedObject.FindProperty ("initialTable");
    this.runtimeTable = this.serializedObject.FindProperty ("runtimeTable");
    this.listenersTableField =
            this.targets.Length == 1
            ? this.target.GetType ().GetField ("listenersTable", BindingFlags.NonPublic | BindingFlags.Instance)
            : null;
    this.enableEditingInitialValuesAtRuntime = false;
    }

void OnDisable ()
    {
    this.foldouts.Clear ();
    }


public override void OnInspectorGUI ()
    {
    this.serializedObject.UpdateIfRequiredOrScript ();

    GUILayout.Label ("Defaults", EditorStyles.boldLabel);
    bool disableEditingDefaults = EditorApplication.isPlaying && !this.enableEditingInitialValuesAtRuntime;
    if (disableEditingDefaults && GUILayout.Button ("Enable Editing Defaults at Runtime"))
        {
        this.enableEditingInitialValuesAtRuntime = true;
        }

    EditorGUI.BeginDisabledGroup (disableEditingDefaults);

    if (this.defaultValue != null)
        {
        EditorGUILayout.PropertyField (this.defaultValue);
        }

    if (this.initialTable != null)
        {
        EditorGUILayout.PropertyField (this.initialTable, true);
        }

    EditorGUI.EndDisabledGroup ();

    EditorGUILayout.Space ();
    GUILayout.Label ("Runtime", EditorStyles.boldLabel);
    EditorGUI.BeginDisabledGroup (!Application.isPlaying);

    if (this.runtimeTable != null)
        {
        EditorGUILayout.PropertyField (this.runtimeTable, true);
        }


    EditorGUILayout.Space ();
    GUILayout.Label ("Listeners Table");
    int totalListeners = 0;
    if (this.listenersTableField != null)
        {
        var listenersTable = this.listenersTableField.GetValue (this.target) as IDictionary;
        if (listenersTable != null)
            {
            foreach (string key in listenersTable.Keys)
                {
                var listeners = listenersTable[key] as ICollection;
                int numberOfListeners = listeners == null ? 0 : listeners.Count;
                totalListeners += numberOfListeners;
                if (!this.Foldout (key, numberOfListeners))
                    {
                    continue;
                    }
                EditorGUI.BeginDisabledGroup (true);
                EditorGUI.indentLevel++;
                foreach (MonoBehaviour mb in listeners)
                    {
                    EditorGUI.ObjectField (EditorGUILayout.GetControlRect (), mb, typeof(MonoBehaviour), true);
                    }
                EditorGUI.indentLevel--;
                EditorGUI.EndDisabledGroup ();
                }
            }
        EditorGUI.EndDisabledGroup ();
        }
    EditorGUILayout.LabelField ("Total Listeners", totalListeners.ToString ());

    this.serializedObject.ApplyModifiedProperties ();
    }

private Dictionary <string, bool> foldouts = new Dictionary <string, bool> ();

private bool Foldout (string key, int listeners)
    {
    bool value;
    if (!this.foldouts.TryGetValue (key, out value))
        {
        value = false;
        }

    var controlRect = EditorGUILayout.GetControlRect ();
    var listenersRect = new Rect (controlRect);
    listenersRect.xMin += EditorGUIUtility.labelWidth;
    value = EditorGUI.Foldout (controlRect, value, key, true);
    GUI.Label (listenersRect, listeners.ToString ());

    this.foldouts[key] = value;
    return value;
    }


}
}
