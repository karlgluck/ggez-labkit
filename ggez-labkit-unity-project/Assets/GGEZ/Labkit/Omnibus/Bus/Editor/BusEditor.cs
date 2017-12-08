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
using System.IO;


namespace GGEZ
{
namespace Omnibus
{



[
CustomEditor(typeof (Bus), true),
CanEditMultipleObjects
]
public class BusEditor : Editor
{

[MenuItem("Assets/Create/GGEZ/Omnibus/Bus")]
public static void CreateBusAsset ()
    {
    string assetPath = "Assets";
    foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
		{
        string path = AssetDatabase.GetAssetPath(obj);
        if (string.IsNullOrEmpty (path))
            {
            continue;
            }
        string absolutePath = Path.Combine (Path.GetDirectoryName (Application.dataPath), path);
        if (Directory.Exists (absolutePath))
			{
            assetPath = path;
            break;
			}
		}
    assetPath = AssetDatabase.GenerateUniqueAssetPath (Path.Combine (assetPath, "New Bus Asset.prefab"));

    var go = new GameObject ();
    var prefab = PrefabUtility.CreatePrefab (assetPath, go);
    prefab.AddComponent <Bus> ();
    GameObject.DestroyImmediate (go);
    Selection.activeGameObject = prefab;
    EditorGUIUtility.PingObject (prefab);
    }




private ReorderableList initialTable;
private ReorderableListCache initialTableCache;
private ReorderableList runtimeTable;
private ReorderableListCache runtimeTableCache;

private FieldInfo connectionsField;

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
    this.initialTableCache = new ReorderableListCache (this.initialTable);
    this.initialTable.drawHeaderCallback = this.drawInitialTableHeader;
    this.initialTable.drawElementCallback = this.drawInitialTableElement;
    this.initialTable.onAddDropdownCallback = this.onTableAddDropdown;
    this.initialTable.onChangedCallback = (_list) => this.initialTableCache.Clear ();
    this.initialTable.onReorderCallback = (_list) => this.initialTableCache.Clear ();
    this.initialTable.elementHeightCallback = this.elementInInitialTableHeight;

    this.runtimeTable = new ReorderableList (
            serializedObject, 
            serializedObject.FindProperty (Bus.nameof_serializedMemory), 
            false, // draggable
            true,  // displayHeader
            false, // displayAddButton
            false  // displayRemoveButton
            );
    this.runtimeTableCache = new ReorderableListCache (this.runtimeTable);
    this.runtimeTable.drawHeaderCallback = this.drawRuntimeTableHeader;
    this.runtimeTable.drawElementCallback = this.drawRuntimeTableElement;
    this.runtimeTable.onAddDropdownCallback = this.onTableAddDropdown;
    this.runtimeTable.onChangedCallback = (_list) => this.runtimeTableCache.Clear ();
    this.runtimeTable.onReorderCallback = (_list) => this.runtimeTableCache.Clear ();
    this.runtimeTable.elementHeightCallback = this.elementInRuntimeTableHeight;

    this.setTriggerParameterType (null);

    this.connectionsField =
            this.targets.Length == 1
            ? this.target.GetType ().GetField (Bus.nameof_connections, BindingFlags.NonPublic | BindingFlags.Instance)
            : null;
    this.enableEditingInitialValuesAtRuntime = false;
    }

void drawInitialTableHeader (Rect rect)
    {
    EditorGUI.LabelField (rect, "ROM");
    }

float elementInInitialTableHeight (int index)
    {
    return this.initialTableCache.GetElementInTableHeight (index);
    // SerializedProperty property = this.initialTable.serializedProperty.GetArrayElementAtIndex (index);
    // var typeProperty = property.FindPropertyRelative (SerializedMemoryCell.nameof_Type);
    // var valueProperty = property.FindPropertyRelative (SerializedMemoryCell.nameof_Value_ + typeProperty.stringValue);
    // return (valueProperty == null ? EditorGUIUtility.singleLineHeight : EditorGUI.GetPropertyHeight (valueProperty)) + 4f;
    }



void drawInitialTableElement (Rect position, int index, bool isActive, bool isFocused)
    {
    string key;
    SerializedProperty keyProperty;
    string typeName;
    SerializedProperty valueProperty;

    this.initialTableCache.GetDrawTableElementInfo (index, out key, out keyProperty, out typeName, out valueProperty);
    
    // SerializedProperty property = this.initialTable.serializedProperty.GetArrayElementAtIndex (index);
    
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
        // EditorGUI.PropertyField (nameRect, property.FindPropertyRelative (SerializedMemoryCell.nameof_Key), GUIContent.none);
        EditorGUI.PropertyField (nameRect, keyProperty, GUIContent.none);
        }
    else
        {
        // EditorGUI.LabelField (nameRect, property.FindPropertyRelative (SerializedMemoryCell.nameof_Key).stringValue);
        EditorGUI.LabelField (nameRect, key);
        }

    // var typeProperty = property.FindPropertyRelative (SerializedMemoryCell.nameof_Type);
    // var valueProperty = property.FindPropertyRelative (SerializedMemoryCell.nameof_Value_ + typeProperty.stringValue);
    if (valueProperty != null)
        {
        EditorGUI.PropertyField (valueRect, valueProperty, GUIContent.none);
        }
    else
        {
        // EditorGUI.LabelField (valueRect, typeProperty.stringValue);
        EditorGUI.LabelField (valueRect, typeName);
        }
    }


private static string getFriendlyNameOfType (string name)
    {
    switch (name)
        {
        case "Boolean": return "bool";
        case "Byte":    return "byte";
        case "SByte":   return "sbyte";
        case "Char":    return "char";
        case "Decimal": return "decimal";
        case "Double":  return "double";
        case "Single":  return "float";
        case "Int32":   return "int";
        case "UInt32":  return "uint";
        case "Int64":   return "long";
        case "UInt64":  return "ulong";
        case "Object":  return "object";
        case "Int16":   return "short";
        case "UInt16":  return "ushort";
        case "String":  return "string";
        }
    return name;
    }

void onTableAddDropdown (Rect buttonRect, ReorderableList list)
    {
    var menu = new GenericMenu ();

    foreach (var fieldType in SerializedMemoryCell.GetFieldTypes ())
        {
        menu.AddItem (
            new GUIContent (getFriendlyNameOfType (fieldType.Name)),
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

float elementInRuntimeTableHeight (int index)
    {
    return this.runtimeTableCache.GetElementInTableHeight (index);
    // SerializedProperty property = this.runtimeTable.serializedProperty.GetArrayElementAtIndex (index);
    // var typeProperty = property.FindPropertyRelative (SerializedMemoryCell.nameof_Type);
    // var valueProperty = property.FindPropertyRelative (SerializedMemoryCell.nameof_Value_ + typeProperty.stringValue);
    // return (valueProperty == null ? EditorGUIUtility.singleLineHeight : EditorGUI.GetPropertyHeight (valueProperty)) + 4f;
    }

void drawRuntimeTableElement (Rect position, int index, bool isActive, bool isFocused)
    {

    string key;
    string typeName;
    SerializedProperty valueProperty;

    this.runtimeTableCache.GetDrawTableElementInfo (index, out key, out typeName, out valueProperty);
    
    // SerializedProperty property = this.runtimeTable.serializedProperty.GetArrayElementAtIndex (index);
        
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

    // EditorGUI.LabelField (nameRect, property.FindPropertyRelative (SerializedMemoryCell.nameof_Key).stringValue);
    EditorGUI.LabelField (nameRect, key);

    // var typeProperty = property.FindPropertyRelative (SerializedMemoryCell.nameof_Type);
    // var valueProperty = property.FindPropertyRelative (SerializedMemoryCell.nameof_Value_ + typeProperty.stringValue);
    if (valueProperty != null)
        {
        EditorGUI.PropertyField (valueRect, valueProperty, GUIContent.none);
        }
    else
        {
        // EditorGUI.LabelField (valueRect, typeProperty.stringValue);
        EditorGUI.LabelField (valueRect, typeName);
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
        this.triggerMethod = typeof(Bus).GetMethod (Bus.nameof_Signal, new Type[] { typeof(string) });
        this.triggerParameter = null;
        this.fieldBackingObject = null;
        this.triggerParameterTypeName = "void";
        }
    else
        {
        this.triggerMethod = typeof(Bus).GetMethod (Bus.nameof_SignalObject, new Type[] { typeof(string), typeof(object) });
        this.triggerParameter = Labkit.SerializedPropertyExt.GetSerializedPropertyFor (type, out this.fieldBackingObject);
        this.triggerParameterTypeName = type.Name;
        }
    }


private void drawManualSignalElement ()
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
                new GUIContent (getFriendlyNameOfType (fieldType.Name)),
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
        this.drawManualSignalElement ();

        EditorGUILayout.Space ();
        GUILayout.Label ("Wires", EditorStyles.boldLabel);
        int total = 0;
        if (this.connectionsField != null)
            {
            var connections = this.connectionsField.GetValue (this.target) as IDictionary;
            if (connections != null)
                {

                foreach (string key in connections.Keys)
                    {
                    var wires = connections[key] as ICollection;
                    int count = wires == null ? 0 : wires.Count;
                    total += count;
                    if (!this.Foldout (key, count))
                        {
                        continue;
                        }
                    EditorGUI.BeginDisabledGroup (true);
                    EditorGUI.indentLevel++;
                    foreach (Wire wire in wires)
                        {
                        EditorGUI.ObjectField (EditorGUILayout.GetControlRect (), wire.Cell as MonoBehaviour, typeof(MonoBehaviour), true);
                        }
                    EditorGUI.indentLevel--;
                    EditorGUI.EndDisabledGroup ();
                    }
                }
            }
        EditorGUILayout.LabelField ("Total Cells", total.ToString ());
        }

    if (this.serializedObject.ApplyModifiedProperties ())
        {
        // this.initialTableCache.Clear ();
        // this.runtimeTableCache.Clear ();
        }
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
    if (this.triggerMethod != null && GUI.Button (listenersRect, "Signal " + count.ToString () + " Cell" + plural))
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


private class ReorderableListCache
    {

    public ReorderableListCache (ReorderableList list)
        {
        this.list = list;
        }
    
    public void Clear ()
        {
        this.elements.Clear ();
        }

    public float GetElementInTableHeight (int arrayIndex)
        {
        // Do this if we need dynamic height
        // var valueProperty = this.GetOrAddElement (arrayIndex).ValueProperty;
        // return (valueProperty == null ? EditorGUIUtility.singleLineHeight : EditorGUI.GetPropertyHeight (valueProperty)) + 4f;
        return this.GetOrAddElement (arrayIndex).Height;
        }

    public void GetDrawTableElementInfo (int arrayIndex, out string key, out string typeName, out SerializedProperty valueProperty)
        {
        var element = this.GetOrAddElement (arrayIndex);
        key = element.Key;
        typeName = element.TypeName; 
        valueProperty = element.ValueProperty;
        }

    public void GetDrawTableElementInfo (int arrayIndex, out string key, out SerializedProperty keyProperty, out string typeName, out SerializedProperty valueProperty)
        {
        var element = this.GetOrAddElement (arrayIndex);
        key = element.Key;
        keyProperty = element.KeyProperty;
        typeName = element.TypeName; 
        valueProperty = element.ValueProperty;
        }

    private Element GetOrAddElement (int index)
        {
        Element retval;
        if (!this.elements.TryGetValue (index, out retval))
            {
            retval = new Element ();

            SerializedProperty property = this.list.serializedProperty.GetArrayElementAtIndex (index);
            retval.KeyProperty = property.FindPropertyRelative (SerializedMemoryCell.nameof_Key);
            retval.Key = retval.KeyProperty.stringValue;
            var typeProperty = property.FindPropertyRelative (SerializedMemoryCell.nameof_Type);
            retval.TypeName = typeProperty.stringValue;
            retval.ValueProperty = property.FindPropertyRelative (SerializedMemoryCell.nameof_Value_ + retval.TypeName);
            retval.Height = (retval.ValueProperty == null ? EditorGUIUtility.singleLineHeight : EditorGUI.GetPropertyHeight (retval.ValueProperty)) + 4f;

            this.elements.Add (index, retval);
            }
        return retval;
        }

    private ReorderableList list;

    private class Element
        {
        public float Height;
        public string Key;
        public SerializedProperty KeyProperty;
        public string TypeName;
        public SerializedProperty ValueProperty;
        }

    private Dictionary <int, Element> elements = new Dictionary <int, Element> ();
    }

}
}

}
