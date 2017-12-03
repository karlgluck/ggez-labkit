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



public class RegistrarEditor : Editor
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
            serializedObject.FindProperty ("initialTable"), 
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
            serializedObject.FindProperty ("runtimeTable"), 
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
        EditorGUI.PropertyField (nameRect, property.FindPropertyRelative ("Name"), GUIContent.none);
        }
    else
        {
        EditorGUI.LabelField (nameRect, property.FindPropertyRelative ("Name").stringValue);
        }

    var typeProperty = property.FindPropertyRelative ("Type");
    var valueProperty = property.FindPropertyRelative ("Value_" + typeProperty.stringValue);
    if (valueProperty != null)
        {
        EditorGUI.PropertyField (valueRect, valueProperty, GUIContent.none);
        }
    else
        {
        EditorGUI.LabelField (valueRect, typeProperty.stringValue);
        }
    }

static IEnumerable<Type> getFieldTypes ()
    {
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
        yield return fieldInfo.FieldType;
        }
    }

void onInitialTableAddDropdown (Rect buttonRect, ReorderableList list)
    {
    var menu = new GenericMenu ();

    foreach (var fieldType in getFieldTypes ())
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
                element.FindPropertyRelative ("Name").stringValue = GUID.Generate ().ToString ().Substring (0, 7);
                element.FindPropertyRelative ("Type").stringValue = type.Name;
                list.serializedProperty.serializedObject.ApplyModifiedProperties ();
                },
            fieldType
            );
        }

    menu.DropDown (buttonRect);
    }

void drawRuntimeTableHeader (Rect rect)
    {
    EditorGUI.LabelField (rect, "Runtime Registers");
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

    EditorGUI.LabelField (nameRect, property.FindPropertyRelative ("Name").stringValue);

    var typeProperty = property.FindPropertyRelative ("Type");
    var valueProperty = property.FindPropertyRelative ("Value_" + typeProperty.stringValue);
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
        this.triggerMethod = typeof(Registrar).GetMethod ("Trigger", new Type[] { typeof(string) });
        this.triggerParameter = null;
        this.fieldBackingObject = null;
        this.triggerParameterTypeName = "void";
        }
    else
        {
        this.triggerMethod = typeof(Registrar).GetMethod ("Trigger", new Type[] { typeof(string), type });
        this.triggerParameter = Labkit.SerializedPropertyExt.GetSerializedPropertyFor (type, out this.fieldBackingObject);
        this.triggerParameterTypeName = type.Name;
        }
    }


private void drawManualTriggerElement ()
    {
    GUILayout.Label ("Manual Trigger (" + this.triggerParameterTypeName + ")", EditorStyles.boldLabel);
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
        foreach (var fieldType in getFieldTypes ())
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
    if (disableEditingDefaults && allObjectsAreAssets && GUILayout.Button ("Enable Editing Asset at Runtime"))
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
            }
        EditorGUILayout.LabelField ("Total Listeners", totalListeners.ToString ());
        }

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
    if (this.triggerMethod != null && GUI.Button (listenersRect, "Trigger " + listeners.ToString () + " Listener" + plural))
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
