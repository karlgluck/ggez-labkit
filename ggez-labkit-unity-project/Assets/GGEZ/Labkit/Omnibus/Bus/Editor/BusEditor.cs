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


namespace GGEZ.Omnibus
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

// private SerializedProperty aliases;
// private bool aliasesFoldout;
// private Dictionary<string, string> aliasedStandardPinsSuffixes = new Dictionary<string,string> ();

private FieldInfo connectionsField;

private bool enableEditingInitialValuesAtRuntime;

private string triggerParameterTypeName;
private MethodInfo triggerMethod;
private SerializedProperty triggerParameter;
private Labkit.ScriptablePropertyBackingObject fieldBackingObject;

// void updateAliasedStandardPinsSuffixes ()
//     {
//     this.aliasedStandardPinsSuffixes.Clear ();
//     if (this.aliases != null)
//         {
//         for (int i = 0; i < this.aliases.arraySize; ++i)
//             {
//             var pin = this.aliases.GetArrayElementAtIndex (i).stringValue;
//             if (Pin.IsValid (pin))
//                 {
//                 this.aliasedStandardPinsSuffixes.Add (pin, " [" + Pin.StdPin[i] + "]");
//                 }
//             }
//         }
//     }

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

    // this.aliases = this.serializedObject.FindProperty (Bus.nameof_aliases);
    // this.updateAliasedStandardPinsSuffixes ();


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
    }



void drawInitialTableElement (Rect position, int index, bool isActive, bool isFocused)
    {
    var element = this.initialTableCache.GetOrAddElement (index, position);

    if (isFocused || isActive)
        {
        EditorGUI.BeginChangeCheck ();
        EditorGUI.PropertyField (element.NameRect, element.KeyProperty, GUIContent.none);
        if (EditorGUI.EndChangeCheck ())
            {
            element.Key = element.KeyProperty.stringValue;
            }
        }
    else
        {
        EditorGUI.LabelField (element.NameRect, element.Key);
        }

    // string aliasOf;
    // if (this.aliasedStandardPinsSuffixes.TryGetValue (element.Key, out aliasOf))
    //     {
    //     EditorGUI.LabelField (element.AliasRect, aliasOf);
    //     }

    if (element.ValueProperty != null)
        {
        EditorGUI.PropertyField (element.ValueRect, element.ValueProperty, GUIContent.none);
        }
    else
        {
        EditorGUI.LabelField (element.ValueRect, element.TypeName);
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
    }

void drawRuntimeTableElement (Rect position, int index, bool isActive, bool isFocused)
    {
    var element = this.runtimeTableCache.GetOrAddElement (index, position);
    
    EditorGUI.LabelField (element.NameRect, element.Key);

    // string aliasOf;
    // if (this.aliasedStandardPinsSuffixes.TryGetValue (element.Key, out aliasOf))
    //     {
    //     EditorGUI.LabelField (element.AliasRect, aliasOf);
    //     }

    if (element.ValueProperty != null)
        {
        EditorGUI.PropertyField (element.ValueRect, element.ValueProperty, GUIContent.none);
        }
    else
        {
        EditorGUI.LabelField (element.ValueRect, element.TypeName);
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
                    string foldoutText = key;
                    // string aliasSuffix;
                    // if (this.aliasedStandardPinsSuffixes.TryGetValue (key, out aliasSuffix))
                    //     {
                    //     foldoutText += aliasSuffix;
                    //     }
                    if (!this.Foldout (foldoutText, count))
                        {
                        continue;
                        }
                    EditorGUI.BeginDisabledGroup (true);
                    EditorGUI.indentLevel++;
                    foreach (Wire wire in wires)
                        {
                        var labelRect = EditorGUILayout.GetControlRect ();
                        var cellRefRect = new Rect (labelRect);
                        cellRefRect.xMin += EditorGUIUtility.labelWidth;
                        labelRect.xMax = cellRefRect.xMin;
                        EditorGUI.LabelField (labelRect, wire.CellPin);
                        EditorGUI.ObjectField (cellRefRect, wire.Cell as MonoBehaviour, typeof(MonoBehaviour), true);
                        }
                    EditorGUI.indentLevel--;
                    EditorGUI.EndDisabledGroup ();
                    }
                }
            }
        EditorGUILayout.LabelField ("Total Cells", total.ToString ());
        }
    
    // this.aliasesFoldout = EditorGUILayout.Foldout (this.aliasesFoldout, "Aliases");
    // if (this.aliasesFoldout)
    //     {
    //     EditorGUI.BeginChangeCheck ();
    //     EditorGUI.indentLevel++;
    //     for (int i = 0; i < Pin.StdPinCount; ++i)
    //         {
    //         var element = this.aliases.GetArrayElementAtIndex (i);
    //         var labelRect = EditorGUILayout.GetControlRect ();
    //         var pinRect = new Rect (labelRect);
    //         pinRect.xMin += EditorGUIUtility.labelWidth;
    //         labelRect.xMax = pinRect.xMin;
    //         EditorGUI.LabelField (labelRect, "[" + Pin.StdPin[i] + "]");
    //         EditorGUI.PropertyField (pinRect, element, GUIContent.none);
    //         }
    //     EditorGUI.indentLevel--;
    //     if (EditorGUI.EndChangeCheck ())
    //         {
    //         this.updateAliasedStandardPinsSuffixes ();
    //         }
    //     }

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
        // var valueProperty = this.GetOrAddElement (arrayIndex, null).ValueProperty;
        // return (valueProperty == null ? EditorGUIUtility.singleLineHeight : EditorGUI.GetPropertyHeight (valueProperty)) + 4f;

        return this.getOrAddElement (arrayIndex).Height;
        }

    public ListElementData GetOrAddElement (int index, Rect position)
        {
        var retval = this.getOrAddElement (index);
        if (!retval.Position.Equals (position))
            {
            retval.NameRect = new Rect (
                    position.xMin,
                    position.yMin + 2f,
                    EditorGUIUtility.labelWidth - 5f - 20f,
                    EditorGUIUtility.singleLineHeight
                    );
            retval.ValueRect = new Rect (
                    position.xMin + retval.NameRect.width + 5f,
                    position.yMin + 2f,
                    position.width - retval.NameRect.width - 5f,
                    retval.Height - 4f
                    );
            // retval.AliasRect = new Rect (
            //         retval.NameRect.xMax - 25f,
            //         retval.NameRect.yMin,
            //         25f,
            //         EditorGUIUtility.singleLineHeight
            //         );
            retval.Position = position;
            }
        return retval;
        }

    private ListElementData getOrAddElement (int index)
        {
        ListElementData retval;
        if (!this.elements.TryGetValue (index, out retval))
            {
            retval = new ListElementData ();

            SerializedProperty property = this.list.serializedProperty.GetArrayElementAtIndex (index);
            retval.KeyProperty = property.FindPropertyRelative (SerializedMemoryCell.nameof_Key);
            retval.Key = retval.KeyProperty.stringValue;
            var typeProperty = property.FindPropertyRelative (SerializedMemoryCell.nameof_Type);
            retval.TypeName = typeProperty.stringValue;
            retval.ValueProperty = property.FindPropertyRelative (SerializedMemoryCell.nameof_Value_ + retval.TypeName);
            retval.Height = (retval.ValueProperty == null ? EditorGUIUtility.singleLineHeight : EditorGUI.GetPropertyHeight (retval.ValueProperty)) + 4f;
            retval.Position = Rect.zero;

            this.elements.Add (index, retval);
            }
        return retval;
        }

    private ReorderableList list;

    public class ListElementData
        {
        public float Height;
        public string Key;
        public SerializedProperty KeyProperty;
        public string TypeName;
        public SerializedProperty ValueProperty;
        public Rect Position;
        public Rect NameRect;
        public Rect ValueRect;
        // public Rect AliasRect;
        }

    private Dictionary <int, ListElementData> elements = new Dictionary <int, ListElementData> ();
    }

}
}
