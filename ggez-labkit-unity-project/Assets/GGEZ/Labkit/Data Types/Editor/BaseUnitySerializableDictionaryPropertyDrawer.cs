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

using System;
using UnityEngine;
using UnityObject = UnityEngine.Object;
using UnityObjectList = System.Collections.Generic.List<UnityEngine.Object>;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEditor;
using UnityEditorInternal;

namespace GGEZ.Labkit
{
    [CustomPropertyDrawer(typeof(UntypedBaseUnitySerializableDictionary), true)]
    public class BaseUnitySerializableDictionaryPropertyDrawer : PropertyDrawer
    {
        private SerializedProperty _property;
        private ReorderableList _list;
        private ReorderableListCache _listCache;
        private SerializedProperty _keys;
        private SerializedProperty _values;

        private void Initialize(SerializedProperty property)
        {
            if (_list == null)
            {
                #warning this isn't done yet...
                _property = property;
                _list = new ReorderableList (
                        property.serializedObject,
                        property.FindPropertyRelative("_keys"),
                        false, // draggable
                        true, // displayHeader
                        false, // displayAddButton
                        false  // displayRemoveButton
                        );
                _list.drawHeaderCallback = this.drawHeader;
                _list.drawElementCallback = this.drawElement;
                _list.onAddDropdownCallback = this.onTableAddDropdown;
                _list.onChangedCallback = (_list) => _listCache.Clear ();
                _list.onReorderCallback = onReorder;
                _list.elementHeightCallback = this.elementHeight;

                _keys = property.FindPropertyRelative("_keys");
                _values = property.FindPropertyRelative("_values");

                _listCache = new ReorderableListCache (_keys, _values);
            }
        }

        void onReorder(ReorderableList list)
        {
        }

        void onTableAddDropdown (Rect buttonRect, ReorderableList list)
        {
            var menu = new GenericMenu ();

            menu.AddItem (
                new GUIContent ("Test"),
                false,
                (object arg) =>
                    {
                    int index = _keys.arraySize;
                    _keys.arraySize++;
                    _values.arraySize++;
                    SerializedProperty element = _keys.GetArrayElementAtIndex(index);
                    element.stringValue = GUID.Generate ().ToString ().Substring (0, 7);
                    list.serializedProperty.serializedObject.ApplyModifiedProperties ();
                    },
                null
                );

            menu.DropDown (buttonRect);
        }

        void drawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, _property.name);
        }

        float elementHeight(int index)
        {
            return this._listCache.GetElementInTableHeight(index);
        }

        void drawElement(Rect position, int index, bool isActive, bool isFocused)
        {

            var element = this._listCache.GetOrAddElement(index, position);

            if(isFocused || isActive)
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

            if (element.ValueProperty != null)
            {
                EditorGUI.PropertyField(element.ValueRect, element.ValueProperty, GUIContent.none);
            }
            else
            {
                EditorGUI.LabelField(element.ValueRect, "element.TypeName");
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            Initialize(property);
            return this._list.GetHeight();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Initialize(property);

            EditorGUI.BeginProperty(position, label, property);

            this._list.DoList (position);

            EditorGUI.EndProperty();
        }


        private class ReorderableListCache
        {
            private SerializedProperty _keys;
            private SerializedProperty _values;

            public ReorderableListCache (SerializedProperty keys, SerializedProperty values)
            {
                _keys = keys;
                _values = values;
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

                    retval.KeyProperty = this._keys.GetArrayElementAtIndex (index);
                    retval.Key = retval.KeyProperty.stringValue;
                    retval.ValueProperty = this._values.GetArrayElementAtIndex (index);
                    retval.Height = EditorGUI.GetPropertyHeight(retval.ValueProperty) + 4f;
                    retval.Position = Rect.zero;

                    this.elements.Add (index, retval);
                }
                return retval;
            }

            public class ListElementData
            {
                public float Height;
                public string Key;
                public SerializedProperty KeyProperty;
                public SerializedProperty ValueProperty;
                public Rect Position;
                public Rect NameRect;
                public Rect ValueRect;
            }

            private Dictionary <int, ListElementData> elements = new Dictionary <int, ListElementData> ();
        }
    }

}
