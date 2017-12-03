using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditorInternal;

namespace GGEZ
{

[CustomPropertyDrawer (typeof(ListenerKeys))]
class ListenerKeysPropertyDrawer : PropertyDrawer
{
private ReorderableList reorderableList = null;
private ReorderableList getReorderableList (SerializedProperty property)
    {
    if (this.reorderableList != null)
        {
        return this.reorderableList;
        }

    this.reorderableList = new ReorderableList (
            property.serializedObject, 
            property.FindPropertyRelative ("Keys"), 
            true, // draggable
            true, // displayHeader
            true, // displayAddButton
            true  // displayRemoveButton
            );
    this.reorderableList.drawHeaderCallback =
            delegate (Rect rect)
                {
                EditorGUI.LabelField (rect, "Keys");
                };
    this.reorderableList.drawElementCallback = 
            delegate (Rect elementPosition, int index, bool isActive, bool isFocused)
                {
                Rect rect = new Rect (
                        elementPosition.xMin,
                        elementPosition.yMin + 2f,
                        elementPosition.width,
                        EditorGUIUtility.singleLineHeight
                        );
                SerializedProperty elementProperty = this.reorderableList.serializedProperty.GetArrayElementAtIndex (index);
                EditorGUI.PropertyField (rect, elementProperty, GUIContent.none);
                };
    this.reorderableList.onAddDropdownCallback =
            delegate (Rect buttonRect, ReorderableList list)
                {
                list.serializedProperty.arraySize++;
                };
    return this.reorderableList;
    }

public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
    {
    return this.getReorderableList (property).GetHeight ();
    }

public override void OnGUI (
        Rect position,
        SerializedProperty property,
        GUIContent label
        )
    {
    this.getReorderableList (property).DoList (position);
    }
}


}
