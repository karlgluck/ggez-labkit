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
using System.Linq;


namespace GGEZ
{
namespace Omnibus
{



public class BusEditor : Editor
{
private ReorderableList initialTable;
private ReorderableList runtimeTable;

private FieldInfo listenersTableField;

private bool enableEditingInitialValuesAtRuntime;

private string triggerParameterTypeName;
private MethodInfo triggerMethod;
private SerializedProperty triggerParameter;
private Labkit.ScriptablePropertyBackingObject fieldBackingObject;

void OnEnable ()
    {
    this.initialTable = new ReorderableList (
            serializedObject, 
            serializedObject.FindProperty (Bus.nameof_serializedRom), 
            true, // draggable
            true, // displayHeader
            true, // displayAddButton
            true  // displayRemoveButton
            );
    this.initialTable.drawHeaderCallback = this.drawInitialTableHeader;
    this.initialTable.drawElementCallback = this.drawInitialTableElement;
    this.initialTable.onAddDropdownCallback = this.onInitialTableAddDropdown;

    this.runtimeTable = new ReorderableList (
            serializedObject, 
            serializedObject.FindProperty (Bus.nameof_serializedMemory), 
            false, // draggable
            true,  // displayHeader
            false, // displayAddButton
            false  // displayRemoveButton
            );
    this.runtimeTable.drawHeaderCallback = this.drawRuntimeTableHeader;
    this.runtimeTable.drawElementCallback = this.drawRuntimeTableElement;

    this.setTriggerParameterType (null);

    this.listenersTableField =
            this.targets.Length == 1
            ? this.target.GetType ().GetField (Bus.nameof_connections, BindingFlags.NonPublic | BindingFlags.Instance)
            : null;
    this.enableEditingInitialValuesAtRuntime = false;
    }

void drawInitialTableHeader (Rect rect)
    {
    EditorGUI.LabelField (rect, "ROM");
    }

void drawInitialTableElement (Rect position, int index, bool isActive, bool isFocused)
    {
    SerializedProperty property = this.initialTable.serializedProperty.GetArrayElementAtIndex (index);
    
    Rect nameRect = new Rect (
            position.xMin,
            position.yMin + 2f,
            EditorGUIUtility.labelWidth - 5f - 20f,
            EditorGUIUtility.singleLineHeight
            );
    Rect valueRect = new Rect (
            position.xMin + nameRect.width + 5f,
            position.yMin + 2f,
            position.width - nameRect.width - 5f,
            EditorGUIUtility.singleLineHeight
            );

    if (isFocused || isActive)
        {
        EditorGUI.PropertyField (nameRect, property.FindPropertyRelative (SerializedMemoryCell.nameof_Key), GUIContent.none);
        }
    else
        {
        EditorGUI.LabelField (nameRect, property.FindPropertyRelative (SerializedMemoryCell.nameof_Key).stringValue);
        }

    var typeProperty = property.FindPropertyRelative (SerializedMemoryCell.nameof_Type);
    var valueProperty = property.FindPropertyRelative (SerializedMemoryCell.nameof_Value_ + typeProperty.stringValue);
    if (valueProperty != null)
        {
        EditorGUI.PropertyField (valueRect, valueProperty, GUIContent.none);
        }
    else
        {
        EditorGUI.LabelField (valueRect, typeProperty.stringValue);
        }
    }


void onInitialTableAddDropdown (Rect buttonRect, ReorderableList list)
    {
    var menu = new GenericMenu ();

    foreach (var fieldType in SerializedMemoryCell.GetFieldTypes ())
        {
        menu.AddItem (
            new GUIContent (fieldType.Name),
            false,
            (object _type) =>
                {
                var type = (Type)_type;
                var index = list.serializedProperty.arraySize;
                list.serializedProperty.arraySize++;
                list.index = index;
                var element = list.serializedProperty.GetArrayElementAtIndex(index);
                element.FindPropertyRelative (SerializedMemoryCell.nameof_Key).stringValue = GUID.Generate ().ToString ().Substring (0, 7);
                element.FindPropertyRelative (SerializedMemoryCell.nameof_Type).stringValue = type.Name;
                list.serializedProperty.serializedObject.ApplyModifiedProperties ();
                },
            fieldType
            );
        }

    menu.DropDown (buttonRect);
    }

void drawRuntimeTableHeader (Rect rect)
    {
    EditorGUI.LabelField (rect, "Memory");
    }

void drawRuntimeTableElement (Rect position, int index, bool isActive, bool isFocused)
    {
    SerializedProperty property = this.runtimeTable.serializedProperty.GetArrayElementAtIndex (index);
        
    Rect nameRect = new Rect (
            position.xMin,
            position.yMin + 2f,
            EditorGUIUtility.labelWidth - 5f - 20f,
            EditorGUIUtility.singleLineHeight
            );
    Rect valueRect = new Rect (
            position.xMin + nameRect.width + 5f,
            position.yMin + 2f,
            position.width - nameRect.width - 5f,
            EditorGUIUtility.singleLineHeight
            );

    EditorGUI.LabelField (nameRect, property.FindPropertyRelative (SerializedMemoryCell.nameof_Key).stringValue);

    var typeProperty = property.FindPropertyRelative (SerializedMemoryCell.nameof_Type);
    var valueProperty = property.FindPropertyRelative (SerializedMemoryCell.nameof_Value_ + typeProperty.stringValue);
    if (valueProperty != null)
        {
        EditorGUI.PropertyField (valueRect, valueProperty, GUIContent.none);
        }
    else
        {
        EditorGUI.LabelField (valueRect, typeProperty.stringValue);
        }
    }


void OnDisable ()
    {
    this.foldouts.Clear ();
    if (this.fieldBackingObject != null)
        {
        GameObject.DestroyImmediate (this.fieldBackingObject);
        }
    }


void setTriggerParameterType (Type type)
    {
    if (this.fieldBackingObject != null)
        {
        GameObject.DestroyImmediate (this.fieldBackingObject);
        }
    if (type == null)
        {
        this.triggerMethod = typeof(IBus).GetMethod ("Trigger", new Type[] { typeof(string) });
        this.triggerParameter = null;
        this.fieldBackingObject = null;
        this.triggerParameterTypeName = "void";
        }
    else
        {
        this.triggerMethod = typeof(IBus).GetMethod ("Trigger", new Type[] { typeof(string), type });
        this.triggerParameter = Labkit.SerializedPropertyExt.GetSerializedPropertyFor (type, out this.fieldBackingObject);
        this.triggerParameterTypeName = type.Name;
        }
    }


private void drawManualTriggerElement ()
    {
    GUILayout.Label ("Manual Event (" + this.triggerParameterTypeName + ")", EditorStyles.boldLabel);
    var triggerRect = EditorGUILayout.GetControlRect ();
    var buttonRect = new Rect (triggerRect);
    buttonRect.xMax = buttonRect.xMin + 75f;
    var parameterRect = new Rect (triggerRect);
    parameterRect.xMin = buttonRect.xMax + 5f;
    if (EditorGUI.DropdownButton (buttonRect, new GUIContent ("Type..."), FocusType.Passive))
        {
        var menu = new GenericMenu ();

        menu.AddItem (new GUIContent ("void"), false, (object _unused) => this.setTriggerParameterType (null), null);
        menu.AddSeparator ("");
        foreach (var fieldType in SerializedMemoryCell.GetFieldTypes ())
            {
            menu.AddItem (
                new GUIContent (fieldType.Name),
                false,
                (object _type) => this.setTriggerParameterType ((Type)_type),
                fieldType
                );
            }

        menu.DropDown (buttonRect);
        }
    if (this.triggerParameter != null)
        {
        EditorGUI.PropertyField (parameterRect, this.triggerParameter, GUIContent.none);
        }
    }


public override void OnInspectorGUI ()
    {
    this.serializedObject.UpdateIfRequiredOrScript ();

    bool disableEditingDefaults = EditorApplication.isPlaying && !this.enableEditingInitialValuesAtRuntime;
    bool allObjectsAreAssets = this.serializedObject.targetObjects.All ((obj) => AssetDatabase.Contains (obj));
    if (disableEditingDefaults && allObjectsAreAssets && GUILayout.Button ("Unlock Editing ROM at Runtime"))
        {
        this.enableEditingInitialValuesAtRuntime = true;
        }

    EditorGUI.BeginDisabledGroup (disableEditingDefaults);
    EditorGUILayout.Space ();
    if (allObjectsAreAssets || !EditorApplication.isPlaying)
        {
        this.initialTable.DoLayoutList ();
        }
    EditorGUI.EndDisabledGroup ();

    if (EditorApplication.isPlaying)
        {

        EditorGUILayout.Space ();
        this.runtimeTable.DoLayoutList ();

        EditorGUILayout.Space ();
        this.drawManualTriggerElement ();

        EditorGUILayout.Space ();
        GUILayout.Label ("Fubs", EditorStyles.boldLabel);
        int total = 0;
        if (this.listenersTableField != null)
            {
            var fubsTable = this.listenersTableField.GetValue (this.target) as IDictionary;
            if (fubsTable != null)
                {

                foreach (string key in fubsTable.Keys)
                    {
                    var fubs = fubsTable[key] as ICollection;
                    int count = fubs == null ? 0 : fubs.Count;
                    total += count;
                    if (!this.Foldout (key, count))
                        {
                        continue;
                        }
                    EditorGUI.BeginDisabledGroup (true);
                    EditorGUI.indentLevel++;
                    foreach (MonoBehaviour mb in fubs)
                        {
                        EditorGUI.ObjectField (EditorGUILayout.GetControlRect (), mb, typeof(MonoBehaviour), true);
                        }
                    EditorGUI.indentLevel--;
                    EditorGUI.EndDisabledGroup ();
                    }
                }
            }
        EditorGUILayout.LabelField ("Total Fubs", total.ToString ());
        }

    this.serializedObject.ApplyModifiedProperties ();
    }

private Dictionary <string, bool> foldouts = new Dictionary <string, bool> ();

private bool Foldout (string key, int count)
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

    string plural = count == 1 ? "" : "s";
    if (this.triggerMethod != null && GUI.Button (listenersRect, "Trigger " + count.ToString () + " Fub" + plural))
        {
        try
            {
            this.manualTrigger (key);
            }
        catch (InvalidCastException e)
            {
            Debug.LogErrorFormat (e.Message);
            }
        }

    this.foldouts[key] = value;
    return value;
    }

private void manualTrigger (string key)
    {
    if (this.triggerParameter != null)
        {
        this.triggerParameter.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        foreach (UnityEngine.Object targetObject in this.serializedObject.targetObjects)
            {
            this.triggerMethod.Invoke(targetObject, new object[] { key, this.fieldBackingObject.GetValue() });
            }
        }
    else
        {
        foreach (UnityEngine.Object targetObject in this.serializedObject.targetObjects)
            {
            this.triggerMethod.Invoke(targetObject, new object[] { key });
            }
        }
    }

}
}

}
