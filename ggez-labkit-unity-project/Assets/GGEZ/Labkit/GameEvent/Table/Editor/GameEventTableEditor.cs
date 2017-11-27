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



[CustomEditor(typeof (BaseGameEventTable), true)]
public class GameEventTableEditor : Editor
{

private FieldInfo listenersTableField;
private bool listenersFoldout;

private MethodInfo triggerMethod;
private SerializedProperty triggerParameter;
private Labkit.ScriptablePropertyBackingObject fieldBackingObject;


void OnEnable ()
    {
    var targetType = this.target.GetType();
    this.triggerMethod = targetType.GetMethod ("Trigger");
    var parameters = this.triggerMethod == null ? null : this.triggerMethod.GetParameters ();
    if (parameters.Length == 2)
        {
        var eventTriggerType = parameters[1].ParameterType;
        this.triggerParameter = Labkit.SerializedPropertyExt.GetSerializedPropertyFor (eventTriggerType, out this.fieldBackingObject);
        }
    else
        {
        this.triggerParameter = null;
        this.fieldBackingObject = null;
        }

    this.listenersTableField =
            this.targets.Length == 1
            ? this.target.GetType ().GetField ("listenersTable", BindingFlags.NonPublic | BindingFlags.Instance)
            : null;
    }

void OnDisable ()
    {
    this.foldouts.Clear ();
    }

public override void OnInspectorGUI ()
    {
    this.serializedObject.UpdateIfRequiredOrScript ();

    EditorGUILayout.Space ();
    GUILayout.Label ("Runtime", EditorStyles.boldLabel);
    EditorGUI.BeginDisabledGroup (!Application.isPlaying);

    if (this.triggerMethod != null)
        {
        GUILayout.Label ("Manual Trigger", EditorStyles.boldLabel);

        if (this.triggerParameter != null)
            {
            EditorGUILayout.PropertyField (triggerParameter);
            }
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
    controlRect.xMax = listenersRect.xMin;
    value = EditorGUI.Foldout (controlRect, value, key, true);

    string plural = listeners == 1 ? "" : "s";
    if (GUI.Button (listenersRect, "Trigger " + listeners.ToString () + " Listener" + plural))
        {
        if (this.triggerParameter != null)
            {
            this.triggerParameter.serializedObject.ApplyModifiedPropertiesWithoutUndo ();
            foreach (UnityEngine.Object targetObject in this.serializedObject.targetObjects)
                {
                this.triggerMethod.Invoke (targetObject, new object[] { key, this.fieldBackingObject.GetValue ()});
                }
            }
        else
            {
            foreach (UnityEngine.Object targetObject in this.serializedObject.targetObjects)
                {
                this.triggerMethod.Invoke (targetObject, new object[] { key });
                }
            }
        }

    this.foldouts[key] = value;
    return value;
    }


}
}
