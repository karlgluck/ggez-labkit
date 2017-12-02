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
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;


namespace GGEZ
{



[
CustomEditor(typeof (RegistrarAsset), true),
//CustomEditor(typeof (RegistrarBehaviour), true),
CanEditMultipleObjects
]
public class RegistrarEditor : Editor
{
private ReorderableList initialTable;
private SerializedProperty runtimeTable;

private FieldInfo listenersTableField;

private bool enableEditingInitialValuesAtRuntime;

private RegistrarKeyPairContainer keyPairContainer;
private SerializedProperty keyPair;

void OnEnable ()
    {
    this.initialTable = new ReorderableList (
            serializedObject, 
            serializedObject.FindProperty("initialTable"), 
            true, true, true, true
            );
    this.initialTable.drawHeaderCallback = this.drawInitialTableHeader;
    this.initialTable.drawElementCallback = this.drawInitialTableElement;
    this.initialTable.onAddDropdownCallback = this.onInitialTableAddDropdown;
    this.runtimeTable = this.serializedObject.FindProperty ("runtimeTable");
    this.keyPairContainer = (RegistrarKeyPairContainer)ScriptableObject.CreateInstance (typeof(RegistrarKeyPairContainer));
    this.keyPair = new SerializedObject (this.keyPairContainer).FindProperty ("KeyPair");
    this.listenersTableField =
            this.targets.Length == 1
            ? this.target.GetType ().GetField ("listenersTable", BindingFlags.NonPublic | BindingFlags.Instance)
            : null;
    this.enableEditingInitialValuesAtRuntime = false;
    }

void drawInitialTableHeader (Rect rect)
    {
    EditorGUI.LabelField (rect, "Initial Registers");
    }

void drawInitialTableElement (Rect position, int index, bool isActive, bool isFocused)
    {
    SerializedProperty property = this.initialTable.serializedProperty.GetArrayElementAtIndex (index);
    
    Rect nameRect = new Rect (
            position.xMin,
            position.yMin + 2f,
            EditorGUIUtility.labelWidth - 5f,
            EditorGUIUtility.singleLineHeight
            );
    Rect valueRect = new Rect (
            position.xMin + EditorGUIUtility.labelWidth,
            position.yMin + 2f,
            position.width - nameRect.width - 5f,
            EditorGUIUtility.singleLineHeight
            );
    EditorGUI.PropertyField (nameRect, property.FindPropertyRelative ("Name"), GUIContent.none);
    var typeProperty = property.FindPropertyRelative ("Type");

    var valueProperty = property.FindPropertyRelative ("Value_" + typeProperty.stringValue);
    if (valueProperty != null)
        {
        EditorGUI.PropertyField (valueRect, valueProperty, GUIContent.none);
        }
    else
        {
        EditorGUI.BeginDisabledGroup (true);
        EditorGUI.TextField (valueRect, "");
        EditorGUI.EndDisabledGroup ();
        }

    }

void onInitialTableAddDropdown (Rect buttonRect, ReorderableList list)
    {
    var menu = new GenericMenu ();

    var valueFields = typeof(RegistrarKeyPair).FindMembers (
            MemberTypes.Field,
            BindingFlags.Public | BindingFlags.Instance,
            delegate (MemberInfo m, object lastArgument)
                {
                return m.Name.StartsWith ("Value_");
                },
            null
            );
    foreach (var memberInfo in valueFields)
        {
        FieldInfo fieldInfo = (FieldInfo)memberInfo;
        menu.AddItem (
            new GUIContent (fieldInfo.FieldType.Name),
            false,
            (object _type) =>
                {
                var type = (Type)_type;
                //object obj = type.IsValueType ? Activator.CreateInstance (type) : null;
                //var element = RegistrarKeyPair.Create ("", obj, true);

                var index = list.serializedProperty.arraySize;
                list.serializedProperty.arraySize++;
                list.index = index;
                var element = list.serializedProperty.GetArrayElementAtIndex(index);
                element.FindPropertyRelative ("Name").stringValue = new GUID ().ToString ();
                element.FindPropertyRelative ("Type").stringValue = type.Name;
                this.serializedObject.ApplyModifiedProperties ();
                },
            fieldInfo.FieldType
            );
        }

    // menu.AddItem (new GUIContent ("Value/int"), false, RegistrarKeyPairDrawer.ToInt, property);
    // menu.AddItem (new GUIContent ("Value/string"), false, RegistrarKeyPairDrawer.ToString, property);
    // menu.AddItem (new GUIContent ("Vector3"), false, RegistrarKeyPairDrawer.ToVector3, property);
    menu.DropDown (buttonRect);
    }

void OnDisable ()
    {
    this.foldouts.Clear ();
    GameObject.DestroyImmediate (this.keyPairContainer);
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

    if (this.initialTable != null)
        {
        this.initialTable.DoLayoutList ();
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
    GUILayout.Label ("Listeners", EditorStyles.boldLabel);
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
                var totalRect = EditorGUILayout.GetControlRect ();
                var inputRect = new Rect (totalRect);
                inputRect.xMax -= 100;
                var buttonRect = new Rect (totalRect);
                buttonRect.xMin = inputRect.xMax + 5;
                EditorGUI.PropertyField (inputRect, this.keyPair, GUIContent.none);
                if (GUI.Button (buttonRect, "Trigger"))
                    {
                    this.keyPair.serializedObject.ApplyModifiedPropertiesWithoutUndo ();
                    var argument = this.keyPairContainer.KeyPair.GetValue ();
                    MethodInfo triggerMethodInfo = null;
                    if (argument != null)
                        {
                        triggerMethodInfo = typeof (Registrar).GetMethod (
                                "Trigger",
                                new Type[] { typeof(string), argument.GetType () }
                                );
                        }
                    if (triggerMethodInfo != null)
                        {
                        var args = new object[] { key, argument };
                        foreach (var target in this.targets)
                            {
                            triggerMethodInfo.Invoke (target, args);
                            }
                        }
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
