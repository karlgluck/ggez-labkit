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

using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;
using UnityObject = UnityEngine.Object;
using SettingsDictionary = System.Collections.Generic.Dictionary<string, object>;

namespace GGEZ.Labkit
{
    /// <summary>
    ///     Used to set the values of literals in a Golem from a single place.
    ///     Settings can be spread among Golems by referencing a SettingsAsset.
    ///     The mapping is done at edit-time, so settings are not intended to
    ///     be highly performant.
    /// </summary>
    public class Settings
    {
        public Settings(UnityObject settingsOwner, SettingsAsset inheritFrom)
        {
            SettingsOwner = settingsOwner;
            InheritFrom = inheritFrom;
            Values = new List<Setting>();
        }

        public Settings(UnityObject settingsOwner, SettingsAsset inheritFrom, List<Setting> values)
        {
            SettingsOwner = settingsOwner;
            InheritFrom = inheritFrom;
            Values = values ?? new List<Setting>();
        }

        public class Setting
        {
            public Type Type;
            public string Name;
            public object Value;
        }

        public string Name { get { return SettingsOwner.name; } }
        public readonly UnityObject SettingsOwner;
        public readonly SettingsAsset InheritFrom;
        public readonly List<Setting> Values;

        public Settings Parent
        {
            get
            {
                return InheritFrom == null ? null : InheritFrom.Settings;
            }
        }

        public bool Contains(string name, Type type)
        {
            if (name == null || type == null)
            {
                return false;
            }
            for (int i = 0; i < Values.Count; ++i)
            {
                if (Values[i].Name.Equals(name))
                {
                    return type.IsAssignableFrom(Values[i].Type);
                }
            }
            var parent = Parent;
            if (parent != null)
            {
                return parent.Contains(name, type);
            }
            return false;
        }

        public object Get(string name, Type type)
        {
            if (name != null)
            {
                for (int i = 0; i < Values.Count; ++i)
                {
                    if (Values[i].Name.Equals(name))
                    {
                        if (type.IsAssignableFrom(Values[i].Type))
                        {
                            return Values[i].Value;
                        }
                        else
                        {
                            Debug.LogWarning("Setting '" + name + "' expected type '" + type.Name + "' but is saved as a '" + Values[i].Type.Name);
                        }
                    }
                }
                var parent = Parent;
                if (parent != null)
                {
                    return parent.Get(name, type);
                }
            }
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            else
            {
                return null;
            }
        }


        private bool _foldout;

        public void DoEditorGUILayout(bool isFocused)
        {
            if (!isFocused)
            {
                _foldout = EditorGUILayout.Foldout(_foldout, Name);
                if (!_foldout)
                {
                    return;
                }
            }

            string focusedControlName = GUI.GetNameOfFocusedControl();

            for (int i = 0; i < Values.Count; ++i)
            {
                var labelRect = EditorGUILayout.GetControlRect();
                var position = new Rect(labelRect);
                position.xMin += EditorGUIUtility.labelWidth;
                labelRect.xMax = position.xMin;
                string name = Values[i].Name;
                string labelControlName = i.ToString("000") + ":" + name;
                GUI.SetNextControlName(labelControlName);
                bool isSettingFocused = focusedControlName == labelControlName;
                string newName = EditorGUI.DelayedTextField(labelRect, name, isSettingFocused ? EditorStyles.textField : EditorStyles.label);
                Values[i].Value = GolemEditorUtility.EditorGUIField(position, InspectableTypeExt.GetInspectableTypeOf(Values[i].Type), Values[i].Type, Values[i].Value);
                if (newName != name)
                {
                    Values[i].Name = newName;
                    if (isSettingFocused)
                    {
                        GUI.FocusControl(null);
                    }
                }
            }

            Rect controlRect = EditorGUILayout.GetControlRect();
            if (EditorGUI.DropdownButton(controlRect, new GUIContent("+"), FocusType.Keyboard))
            {
                GenericMenu menu = new GenericMenu();
                var enumNames = Enum.GetNames(typeof(InspectableType));
                var enumValues = Enum.GetValues(typeof(InspectableType));
                for (int i = 0; i < enumNames.Length; ++i)
                {
                    var value = (InspectableType)enumValues.GetValue(i);
                    if (value == InspectableType.UnityObject || value == InspectableType.VariableRef || value == InspectableType.Enum || value == InspectableType.TriggerRef || value == InspectableType.Invalid)
                    {
                        continue;
                    }
                    menu.AddItem(new GUIContent(enumNames[i]), false, addSettingMenuCallback, InspectableTypeExt.GetRepresentedType(value));
                }
                menu.DropDown(controlRect);
            }
        }

        public void Add(string name, Type type)
        {
            object defaultValue = type.IsValueType ? Activator.CreateInstance(type) : null;
            Values.Add(
                new Setting()
                {
                    Type = type,
                    Name = name,
                    Value = defaultValue,
                }
            );
        }

        private void addSettingMenuCallback(object contextParam)
        {
            Add("New Setting", (Type)contextParam);
        }
    }

}
