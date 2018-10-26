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
#warning todo unify namespaces for ggez
using GGEZ.FullSerializer;

#if UNITY_EDITOR
using UnityEditorInternal;
#endif

namespace GGEZ.Labkit
{
    /// <summary>
    ///     Used to set the values of literals in a Golem from a single place.
    ///     Settings can be spread among Golems by referencing a SettingsAsset.
    ///     The mapping is done at edit-time, so settings are not intended to
    ///     be highly performant.
    /// </summary>
    [Serializable]
    public class Settings : ISerializationCallbackReceiver
    {
        public string Name { get { return Owner == null ? null : Owner.name; } }

        public UnityObject Owner;

        public Settings Parent
        {
            get
            {
                IHasSettings ownerWithSettings = Owner as IHasSettings;
                if (ownerWithSettings == null)
                    return null;

                IHasSettings hasSettingsWeInherit = ownerWithSettings.InheritsSettingsFrom;
                if (hasSettingsWeInherit == null)
                    return null;

                Settings settingsWeInherit = hasSettingsWeInherit.Settings;
                return settingsWeInherit;
            }
        }

        [NonSerialized]
        public List<Setting> Values;

        /// <summary>
        ///     Object references the settings use
        /// </summary>
        [SerializeField, HideInInspector]
        public List<UnityObject> SettingsObjectReferences = new List<UnityObject>();

        /// <summary>
        ///     Serialized data for settings
        /// </summary>
        [SerializeField, HideInInspector]
        public string Json;

#if UNITY_EDITOR
        /// <summary>
        ///     Pass this as the allowSceneObjects parameter in ObjectField GUI fields
        /// </summary>
        public bool CanReferenceSceneObjects
        {
            get
            {
                return Owner != null && string.IsNullOrEmpty(AssetDatabase.GetAssetPath(Owner));
            }
        }
#endif

        public void OnBeforeSerialize()
        {
            Values = Values ?? new List<Setting>();
            SettingsObjectReferences.Clear();
            var serializer = Serialization.GetSerializer(SettingsObjectReferences);
            fsData data;
            serializer.TrySerialize(Values, out data);
            Json = fsJsonPrinter.PrettyJson(data);

            if (!CanReferenceSceneObjects)
            {
                for (int i = 0; i < SettingsObjectReferences.Count; ++i)
                {
                    if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(SettingsObjectReferences[i])))
                    {
                        Debug.LogWarning("TODO: make sure the check on not serializing asset references into Settings actually doesn't break deserialization; this warning is from code that hasn't been tested but should work, remove if it does!");
                        SettingsObjectReferences[i] = null;
                    }
                }
            }
        }

        public void OnAfterDeserialize()
        {
            var serializer = Serialization.GetSerializer(SettingsObjectReferences);
            fsData data;
            fsResult result;

            result = fsJsonParser.Parse(Json, out data);
            if (result.Failed)
            {
                Debug.LogError(result, Owner);
                Values = new List<Setting>();
                return;
            }

            result = serializer.TryDeserialize(data, ref Values);
            if (result.Failed)
            {
                Debug.LogError(result, Owner);
                Values = new List<Setting>();
                return;
            }
        }

        public Settings(UnityObject settingsOwner)
        {
            Owner = settingsOwner;
            Values = new List<Setting>();
        }

        public class Setting
        {
            public Type Type;
            public string Name;
            public object Value;

            public bool CheckType(Type type)
            {
                var valueType = Value == null ? Type : Value.GetType();
                return type.IsAssignableFrom(valueType);
            }
        }

        public bool Contains(string name, Type type)
        {
            Debug.Assert(Values != null);
            if (name == null || type == null)
            {
                return false;
            }
            for (int i = 0; i < Values.Count; ++i)
            {
                if (Values[i].Name.Equals(name))
                {
                    return Values[i].CheckType(type);
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
            Debug.Assert(Values != null);
            if (name != null)
            {
                for (int i = 0; i < Values.Count; ++i)
                {
                    Setting setting = Values[i];
                    if (setting.Name.Equals(name))
                    {
                        if (setting.CheckType(type))
                        {
                            return setting.Value;
                        }
                        else
                        {
                            Debug.LogWarning("Setting '" + name + "' expected type '" + type.Name + "' but is saved as a '" + setting.Type.Name);
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


#if UNITY_EDITOR
        private bool _foldout;
        private string _searchText;

        public void DoEditorGUILayout(bool isFocused)
        {
            EditorGUILayout.Space();

            {
                EditorGUILayout.BeginHorizontal((GUIStyle)"Toolbar");
                EditorGUILayout.LabelField(new GUIContent(" Settings", EditorGUIUtility.FindTexture("FilterByType")));

                GUILayout.FlexibleSpace();

                // Allow filtering
                _searchText = EditorGUILayout.TextField(_searchText, (GUIStyle)"ToolbarSeachTextField");
                if (GUILayout.Button("", string.IsNullOrEmpty(_searchText) ? (GUIStyle)"ToolbarSeachCancelButtonEmpty" : (GUIStyle)"ToolbarSeachCancelButton"))
                {
                    // Clear search
                    GUI.FocusControl("");
                    GUIUtility.hotControl = 0;
                    _searchText = null;
                }
                var dropdownRect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight, EditorStyles.toolbarDropDown, GUILayout.Width(38f));
                if (EditorGUI.DropdownButton(dropdownRect, new GUIContent("New"), FocusType.Keyboard, EditorStyles.toolbarDropDown))
                {
                    // Clear search
                    GUI.FocusControl("");
                    GUIUtility.hotControl = 0;
                    _searchText = null;

                    // Offer options for a new setting
                    GenericMenu menu = new GenericMenu();
                    var enumNames = Enum.GetNames(typeof(InspectableType));
                    var enumValues = Enum.GetValues(typeof(InspectableType));
                    for (int i = 0; i < enumNames.Length; ++i)
                    {
                        var value = (InspectableType)enumValues.GetValue(i);
                        #warning TODO: InspectableType should have a "can be a setting" type
                        if (value == InspectableType.VariableRef
                        || value == InspectableType.Enum || value == InspectableType.TriggerRef
                        || value == InspectableType.Invalid || value == InspectableType.Golem
                        || value == InspectableType.Aspect || value == InspectableType.Variable)
                        {
                            continue;
                        }
                        menu.AddItem(new GUIContent(enumNames[i]), false, addSettingMenuCallback, InspectableTypeExt.GetRepresentedType(value));
                    }
                    menu.DropDown(dropdownRect);
                }

                EditorGUILayout.EndHorizontal();
            }

            if (!isFocused)
            {
                _foldout = EditorGUILayout.Foldout(_foldout, Name);
                if (!_foldout)
                {
                    return;
                }
            }

            EditorGUILayout.BeginVertical();

            string focusedControlName = GUI.GetNameOfFocusedControl();

            for (int i = 0; i < Values.Count; ++i)
            {
                Setting setting = Values[i];
                if (!string.IsNullOrEmpty(_searchText) && !setting.Name.Contains(_searchText))
                    continue;

                int id = GUIUtility.GetControlID(FocusType.Keyboard);
                GUI.SetNextControlName(id.ToString());
                bool isSelected = focusedControlName == id.ToString();
                Rect rect = EditorGUILayout.BeginHorizontal();
                if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
                {
                    GUI.FocusControl(id.ToString());
                }

                EditorGUI.BeginChangeCheck();

                string name = setting.Name;
                string newName = EditorGUILayout.DelayedTextField(name, isFocused ? EditorStyles.textField : EditorStyles.label);

                GUILayout.FlexibleSpace();

                object value = setting.Value;
                GenericMenu menu = null;
                if (GUILayout.Button("", GolemEditorUtility.SettingsButtonStyle))
                {
                    menu = new GenericMenu();
                }

                Rect position = EditorGUILayout.GetControlRect();
                object newValue = GolemEditorUtility.EditorGUIField(position, InspectableTypeExt.GetInspectableTypeOf(setting.Type), setting.Type, value);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RegisterCompleteObjectUndo(Owner, Owner.name + " Settings");
                    EditorUtility.SetDirty(Owner);
                }

                setting.Value = newValue;
                if (newName != name)
                {
                    #warning TODO update everything that references a setting when the name changes
                    setting.Name = newName;
                    if (isFocused)
                        GUI.FocusControl(null);
                }

                if (menu != null)
                {
                    if (value != null && value.GetType().IsSubclassOf(setting.Type))
                    {
                        menu.AddItem(
                            new GUIContent("Become " + value.GetType().Name),
                            false,
                            (object arg) =>
                            {
                                Undo.RegisterCompleteObjectUndo(Owner, Owner.name + " Settings");
                                Values[(int)arg].Type = Values[(int)arg].Value.GetType();
                            },
                            i
                        );
                        menu.AddSeparator("");
                    }

                    menu.AddItem(
                        new GUIContent("Delete"),
                        false,
                        (object arg) =>
                        {
                            int index = (int)arg;
                            Undo.RegisterCompleteObjectUndo(Owner, Owner.name + " Settings");
                            Values.RemoveAt(index);
                        },
                        i
                        );

                    menu.ShowAsContext();
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        /// <summary>
        ///     Creates a new setting of the given type with a default value
        /// </summary>
        public void Add(string name, Type type)
        {
            Undo.RegisterCompleteObjectUndo(Owner, Owner.name + " Settings");
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

        /// <summary>
        ///     Adds an object as a new setting
        /// </summary>
        /// <param name="type">
        ///     What kind of setting is being passed. Pass "null" to infer the type. This
        ///     is not done automatically because you might want a supertype or pass a null object.
        /// <param>
        public void Add(string name, Type type, object value)
        {
            Undo.RegisterCompleteObjectUndo(Owner, Owner.name + " Settings");
            type = type ?? (value == null ? typeof(object) : value.GetType());
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
            Add("New " + InspectableTypeExt.GetInspectableTypeOf(contextParam as Type).ToString() + " Setting", (Type)contextParam);
        }
#endif

    }

}
