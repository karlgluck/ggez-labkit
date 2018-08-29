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

namespace GGEZ.Omnibus
{
    //-----------------------------------------------------------------------------
    // EntityEditor
    //-----------------------------------------------------------------------------
    [CustomEditor(typeof(EntityContainer))]
    public class EntityEditor : Editor
    {
        private EntityContainer _entity;
        private EntityEditorData _editable;

        private Settings _settings;
        private InspectableDictionaryKeyValuePair[] _inspectableSettings;

        //-----------------------------------------------------
        // OnEnable
        //-----------------------------------------------------
        private void OnEnable()
        {
            _entity = target as EntityContainer;
            _editable = EntityEditorData.Load(_entity);
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
                EditorGUILayout.LabelField("prefab required for editing");
                EditorGUILayout.Space();
                return;
            }

            //-------------------------------------------------
            // Assets
            //-------------------------------------------------
            _entity.Asset = EditorGUILayout.ObjectField(new GUIContent("Asset"), _entity.Asset, typeof(EntityAsset), false) as EntityAsset;
            EditorGUI.BeginDisabledGroup(_entity.Asset != null);
            if (GUILayout.Button("Get from Prefab"))
            {
                _entity.Asset = Helper.FindAssetInPrefab<EntityAsset>(_entity);
            }
            EditorGUI.EndDisabledGroup();
            _entity.EditorAsset = EditorGUILayout.ObjectField(new GUIContent("Editor Asset"), _entity.EditorAsset, typeof(EntityEditorAsset), false) as EntityEditorAsset;
            EditorGUI.BeginDisabledGroup(_entity.Asset != null);
            if (GUILayout.Button("Get from Prefab"))
            {
                _entity.EditorAsset = Helper.FindAssetInPrefab<EntityEditorAsset>(_entity);
            }
            EditorGUI.EndDisabledGroup();

            //-------------------------------------------------
            // Graph editor link
            //-------------------------------------------------
            EditorGUILayout.Space();
            if (GUILayout.Button("Open Editor"))
            {
                OmnibusEditorWindow.Open(_editable);
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
                        .Where(myType => !_editable.EditorAspects.Any(existing => existing._aspect.GetType() == myType))
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
                    var aspectSettings = editableAspect._aspectSettings;

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
                        fieldInfo.FieldInfo.SetValue(editableAspect._aspect, OmnibusEditorUtility.EditorGUIField(position, fieldInfo.Type, fieldInfo.FieldInfo.FieldType, fieldInfo.FieldInfo.GetValue(editableAspect._aspect)));
                    }

                    // Variables, if they exist
                    if (aspectVariables.Length > 0)
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.LabelField("Variables", EditorStyles.boldLabel);
                        var variables = _editable.Entity.Variables;
                        for (int i = 0; i < aspectVariables.Length; ++i)
                        {
                            var labelRect = EditorGUILayout.GetControlRect();
                            var position = new Rect(labelRect);
                            position.xMin += EditorGUIUtility.labelWidth;
                            labelRect.xMax = position.xMin;
                            var name = aspectVariables[i].VariableAttribute.Name;
                            EditorGUI.LabelField(labelRect, new GUIContent(aspectVariables[i].PropertyInfo.Name, aspectVariables[i].VariableAttribute.Tooltip));
                            var variableType = aspectVariables[i].PropertyInfo.PropertyType;
                            variables.InspectorSet(name, variableType, OmnibusEditorUtility.EditorGUIField(position, aspectVariables[i].Type, variableType, variables.InspectorGet(name, variableType, aspectVariables[i].VariableAttribute.DefaultValue)));
                        }
                    }

                    // Settings, if they exist
                    if (aspectSettings.Length > 0)
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
                        var settings = _editable.Entity.Settings;
                        if (settings == null)
                        {
                            EditorGUILayout.LabelField("Create a prefab for this object or manually select the Settings asset for this object.");
                        }
                        else
                        {
                            EditorGUI.BeginChangeCheck();
                            for (int i = 0; i < aspectSettings.Length; ++i)
                            {
                                var labelRect = EditorGUILayout.GetControlRect();
                                var position = new Rect(labelRect);
                                position.xMin += EditorGUIUtility.labelWidth;
                                labelRect.xMax = position.xMin;
                                var name = aspectSettings[i].SettingAttribute.Name;
                                EditorGUI.LabelField(labelRect, new GUIContent(aspectSettings[i].PropertyInfo.Name, aspectSettings[i].SettingAttribute.Tooltip));
                                var variableType = aspectSettings[i].PropertyInfo.PropertyType;
                                settings.InspectorSet(name, variableType, OmnibusEditorUtility.EditorGUIField(position, aspectSettings[i].Type, variableType, settings.InspectorGet(name, variableType, aspectSettings[i].SettingAttribute.DefaultValue)));
                            }
                            if (EditorGUI.EndChangeCheck())
                            {
                                EditorUtility.SetDirty(_editable.Entity.SettingsAsset);
                            }
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
                    _editable.Entity.Variables.InspectorSet(name, variableType, OmnibusEditorUtility.EditorGUIField(position, variables[i].Type, variableType, _editable.Entity.Variables.InspectorGet(name, variableType)));
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                _editable.Save();
            }

            //-------------------------------------------------
            // Settings
            //-------------------------------------------------
            if (_editable.Entity.SettingsAsset != null)
            {
                var settings = _editable.Entity.SettingsAsset.Settings;

                if (settings != _settings)
                {
                    _settings = settings;
                    _inspectableSettings = InspectableDictionaryKeyValuePair.GetDictionaryKeyValuePairs(settings.Values);
                }

                if (_inspectableSettings.Length > 0)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
                }

                EditorGUI.BeginChangeCheck();

                for (int i = 0; i < _inspectableSettings.Length; ++i)
                {
                    var labelRect = EditorGUILayout.GetControlRect();
                    var position = new Rect(labelRect);
                    position.xMin += EditorGUIUtility.labelWidth;
                    labelRect.xMax = position.xMin;
                    var name = _inspectableSettings[i].Key;
                    EditorGUI.LabelField(labelRect, new GUIContent(name));
                    var variableType = _inspectableSettings[i].Value.GetType();
                    settings.InspectorSet(name, variableType, OmnibusEditorUtility.EditorGUIField(position, _inspectableSettings[i].Type, variableType, settings.InspectorGet(name, variableType)));
                }

                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(_editable.Entity.SettingsAsset);
                }
            }
        }

        private void addAspectType(Type type)
        {
            var field = typeof(EntityContainer).GetField(type.Name);
            if (field == null)
            {
                throw new InvalidProgramException(typeof(EntityContainer).Name + " must contain a field for " + type.Name);
            }
            var item = new EntityAspectEditorData();
            var aspect = Activator.CreateInstance(type) as Aspect;
            item.Field = field;
            item._aspect = aspect;
            item._aspectFields = InspectableFieldInfo.GetFields(aspect);
            item._aspectVariables = InspectableVariablePropertyInfo.GetVariableProperties(aspect);
            item._aspectSettings = InspectableSettingPropertyInfo.GetSettingProperties(aspect);
            _editable.EditorAspects.Add(item);
        }
    }
}
