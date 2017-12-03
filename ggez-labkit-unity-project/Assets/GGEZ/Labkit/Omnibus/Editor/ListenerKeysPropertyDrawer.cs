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
