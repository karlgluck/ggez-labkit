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
using System.Linq;
using System.Collections.Generic;
using UnityObject = UnityEngine.Object;

namespace GGEZ.Labkit
{
    //-----------------------------------------------------------------------------
    // EntityEditor
    //-----------------------------------------------------------------------------
    [CustomEditor(typeof(Golem))]
    public class GolemEditor : Editor
    {
        private Golem _golem;
        private GolemEditorData _editable;

        private Settings _settings;


        //-----------------------------------------------------
        // OnEnable
        //-----------------------------------------------------
        private void OnEnable()
        {
            _golem = target as Golem;
            _editable = GolemEditorData.Load(_golem);
        }

        //-----------------------------------------------------
        // OnDisable
        //-----------------------------------------------------
        private void OnDisable()
        {
            _editable = null;
        }

        //-----------------------------------------------------
        // OnInspectorGUI
        //-----------------------------------------------------
        public override void OnInspectorGUI()
        {
            if (_editable == null)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Golem failed to load");
                EditorGUILayout.Space();
                return;
            }

            //-------------------------------------------------
            // Assets
            //-------------------------------------------------
            // _golem.Asset = EditorGUILayout.ObjectField(new GUIContent("Asset"), _golem.Asset, typeof(GolemAsset), false) as GolemAsset;
            // EditorGUI.BeginDisabledGroup(_golem.Asset != null);
            // if (GUILayout.Button("Get from Prefab"))
            // {
            //     _golem.Asset = Helper.FindAssetInPrefab<GolemAsset>(_golem);
            // }
            // EditorGUI.EndDisabledGroup();
            // _golem.EditorAsset = EditorGUILayout.ObjectField(new GUIContent("Editor Asset"), _golem.EditorAsset, typeof(EntityEditorAsset), false) as EntityEditorAsset;
            // EditorGUI.BeginDisabledGroup(_golem.Asset != null);
            // if (GUILayout.Button("Get from Prefab"))
            // {
            //     _golem.EditorAsset = Helper.FindAssetInPrefab<EntityEditorAsset>(_golem);
            // }
            // EditorGUI.EndDisabledGroup();

            //-------------------------------------------------
            // Graph editor link
            //-------------------------------------------------
            EditorGUILayout.Space();
            if (GUILayout.Button("Open Editor"))
            {
                GolemEditorWindow.Open(_editable);
            }
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();

            //-------------------------------------------------
            // Aspects
            //-------------------------------------------------
            EditorGUILayout.LabelField("Aspects", EditorStyles.boldLabel);

            // Dropdown for adding an aspect of a new type
            {
                var labelRect = EditorGUILayout.GetControlRect();
                var dropdownRect = new Rect(labelRect);
                dropdownRect.xMin += EditorGUIUtility.labelWidth;
                labelRect.xMax = dropdownRect.xMin;
                if (EditorGUI.DropdownButton(dropdownRect, new GUIContent("New Aspect..."), FocusType.Passive))
                {
                    var menu = new GenericMenu();
                    var aspectTypes = Assembly.GetAssembly(typeof(Aspect))
                        .GetTypes()
                        .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Aspect)))
                        .Where(myType => !_editable.EditorAspects.Any(existing => existing.Aspect.GetType() == myType))
                        .ToList();
                    aspectTypes.Sort((a, b) => a.Name.CompareTo(b.Name));
                    if (aspectTypes.Count == 0)
                    {
                        menu.AddDisabledItem(new GUIContent("No other aspects exist!"));
                    }
                    foreach (var type in aspectTypes)
                    {
                        menu.AddItem(
                            new GUIContent(ObjectNames.NicifyVariableName(type.Name)),
                            false,
                            (object _type) => this.addAspectType((Type)_type),
                            type
                            );
                    }

                    menu.DropDown(dropdownRect);
                }
            }

            // Each existing aspect
            {
                var aspects = _editable.EditorAspects;
                for (int j = 0; j < aspects.Count; ++j)
                {
                    var editableAspect = aspects[j];
                    var aspectFields = editableAspect._aspectFields;
                    var aspectVariables = editableAspect._aspectVariables;

                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField(editableAspect.Field.Name, EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;

                    // Fields
                    for (int i = 0; i < aspectFields.Length; ++i)
                    {
                        var labelRect = EditorGUILayout.GetControlRect();
                        var position = new Rect(labelRect);
                        position.xMin += EditorGUIUtility.labelWidth;
                        labelRect.xMax = position.xMin;
                        EditorGUI.LabelField(labelRect, aspectFields[i].FieldInfo.Name);
                        var fieldInfo = aspectFields[i];
                        fieldInfo.FieldInfo.SetValue(editableAspect.Aspect, GolemEditorUtility.EditorGUIField(position, fieldInfo.Type, fieldInfo.FieldInfo.FieldType, fieldInfo.FieldInfo.GetValue(editableAspect.Aspect)));
                    }

                    // Variables, if they exist
                    if (aspectVariables.Length > 0)
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.LabelField("Variables", EditorStyles.boldLabel);
                        var variables = _editable.Golem.Variables;
                        for (int i = 0; i < aspectVariables.Length; ++i)
                        {
                            var labelRect = EditorGUILayout.GetControlRect();
                            var position = new Rect(labelRect);
                            position.xMin += EditorGUIUtility.labelWidth;
                            labelRect.xMax = position.xMin;
                            var name = aspectVariables[i].VariableAttribute.Name;
                            EditorGUI.LabelField(labelRect, new GUIContent(aspectVariables[i].PropertyInfo.Name, aspectVariables[i].VariableAttribute.Tooltip));
                            var variableType = aspectVariables[i].PropertyInfo.PropertyType;
                            variables.InspectorSet(name, variableType, GolemEditorUtility.EditorGUIField(position, aspectVariables[i].Type, variableType, variables.InspectorGet(name, variableType, aspectVariables[i].VariableAttribute.DefaultValue)));
                        }
                    }

                    EditorGUI.indentLevel--;
                }
            }

            //-------------------------------------------------
            // Variables
            //-------------------------------------------------
            {
                var variables = _editable._variables;
                if (variables.Length > 0)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Variables", EditorStyles.boldLabel);
                }

                for (int i = 0; i < variables.Length; ++i)
                {
                    var labelRect = EditorGUILayout.GetControlRect();
                    var position = new Rect(labelRect);
                    position.xMin += EditorGUIUtility.labelWidth;
                    labelRect.xMax = position.xMin;
                    var name = variables[i].Key;
                    EditorGUI.LabelField(labelRect, new GUIContent(name));
                    var variableType = variables[i].Value.GetType();
                    _editable.Golem.Variables.InspectorSet(name, variableType, GolemEditorUtility.EditorGUIField(position, variables[i].Type, variableType, _editable.Golem.Variables.InspectorGet(name, variableType)));
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                _editable.Save();
            }

            //-------------------------------------------------
            // Settings
            //-------------------------------------------------
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
            _editable.EditorAsset.InheritSettingsFrom = EditorGUILayout.ObjectField("Inherit From", _editable.EditorAsset.InheritSettingsFrom, typeof(SettingsAsset), false) as SettingsAsset;
            Settings settings = _editable.Settings;
            if (settings != null)
            {
                EditorGUI.BeginChangeCheck();
                settings.DoEditorGUILayout(true);
                if (EditorGUI.EndChangeCheck())
                {
                    _editable.Save();
                }
                Settings current = settings.Parent;
                while (current != null)
                {
                    EditorGUI.BeginChangeCheck();
                    current.DoEditorGUILayout(false);
                    if (EditorGUI.EndChangeCheck() && current.SettingsOwner)
                    {
                        EditorUtility.SetDirty(current.SettingsOwner);
                    }
                    current = current.Parent;
                }
            }


            // if (_editable.Golem.SettingsAsset != null)
            // {
            //     var settings = _editable.Golem.SettingsAsset.Settings;

            //     if (settings != _settings)
            //     {
            //         _settings = settings;
            //         _inspectableSettings = InspectableDictionaryKeyValuePair.GetDictionaryKeyValuePairs(settings.Values);
            //     }

            //     if (_inspectableSettings.Length > 0)
            //     {
            //         EditorGUILayout.Space();
            //         EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
            //     }

            //     EditorGUI.BeginChangeCheck();

            //     for (int i = 0; i < _inspectableSettings.Length; ++i)
            //     {
            //         var labelRect = EditorGUILayout.GetControlRect();
            //         var position = new Rect(labelRect);
            //         position.xMin += EditorGUIUtility.labelWidth;
            //         labelRect.xMax = position.xMin;
            //         var name = _inspectableSettings[i].Key;
            //         EditorGUI.LabelField(labelRect, new GUIContent(name));
            //         var variableType = _inspectableSettings[i].Value.GetType();
            //         settings.InspectorSet(name, variableType, GolemEditorUtility.EditorGUIField(position, _inspectableSettings[i].Type, variableType, settings.InspectorGet(name, variableType)));
            //     }

            //     if (EditorGUI.EndChangeCheck())
            //     {
            //         EditorUtility.SetDirty(_editable.Golem.SettingsAsset);
            //     }
            // }
        }

        private void addAspectType(Type type)
        {
            var field = typeof(Golem).GetField(type.Name);
            if (field == null)
            {
                throw new InvalidProgramException(typeof(Golem).Name + " must contain a field for " + type.Name);
            }
            var item = new GolemAspectEditorData();
            var aspect = Activator.CreateInstance(type) as Aspect;
            item.Field = field;
            item.Aspect = aspect;
            item._aspectFields = InspectableFieldInfo.GetFields(aspect);
            item._aspectVariables = InspectableVariablePropertyInfo.GetVariableProperties(aspect);
            // item._aspectSettings = InspectableSettingPropertyInfo.GetSettingProperties(aspect);
            _editable.EditorAspects.Add(item);
        }
    }
}
