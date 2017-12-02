using UnityEngine;
using UnityEditor;
using System.Collections;

namespace GGEZ
{

[CustomPropertyDrawer (typeof (RegistrarKeyPair))]
public class RegistrarKeyPairDrawer : PropertyDrawer
{
    
public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
    {
    // Using BeginProperty / EndProperty on the parent property means that
    // prefab override logic works on the entire property.
    EditorGUI.BeginProperty (position, label, property);
    
    // Don't make child fields be indented
    var indent = EditorGUI.indentLevel;
    EditorGUI.indentLevel = 0;
    
    Rect nameRect = new Rect (position);
    Rect valueRect = new Rect (position);
    if (label != GUIContent.none)
        {
        nameRect.xMax = nameRect.xMin + EditorGUIUtility.labelWidth - 5;
        EditorGUI.PropertyField (nameRect, property.FindPropertyRelative ("Name"), GUIContent.none);
        valueRect.xMin += EditorGUIUtility.labelWidth;
        }

    var typeProperty = property.FindPropertyRelative ("Type");
    if (property.FindPropertyRelative ("CanEditType").boolValue)
        {
        valueRect.xMax = valueRect.xMin + valueRect.width * 0.8f - 5;
        
        Rect typeButtonRect = new Rect (position);
        typeButtonRect.xMin = valueRect.xMax + 5;
        
        if (EditorGUI.DropdownButton (typeButtonRect, new GUIContent (typeProperty.stringValue), FocusType.Passive))
            {
            var menu = new GenericMenu ();
            menu.AddItem (new GUIContent ("Value/int"), false, RegistrarKeyPairDrawer.ToInt, property);
            menu.AddItem (new GUIContent ("Value/string"), false, RegistrarKeyPairDrawer.ToString, property);
            menu.AddItem (new GUIContent ("Vector3"), false, RegistrarKeyPairDrawer.ToVector3, property);
            menu.DropDown (typeButtonRect);
            }
        }

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
    
    EditorGUI.indentLevel = indent;
    EditorGUI.EndProperty ();
    }

public static void ToInt (object _property)
    {
    var property = _property as SerializedProperty;
    property.FindPropertyRelative ("Type").stringValue = "int";
    property.FindPropertyRelative ("Value_int").intValue = 0;
    property.serializedObject.ApplyModifiedProperties ();
    }

public static void ToString (object _property)
    {
    var property = _property as SerializedProperty;
    property.FindPropertyRelative ("Type").stringValue = "string";
    property.FindPropertyRelative ("Value_string").stringValue = "";
    property.serializedObject.ApplyModifiedProperties ();
    }

public static void ToVector3 (object _property)
    {
    var property = _property as SerializedProperty;
    property.FindPropertyRelative ("Type").stringValue = "Vector3";
    property.FindPropertyRelative ("Value_Vector3").vector3Value = Vector3.zero;
    property.serializedObject.ApplyModifiedProperties ();
    }

}



}
