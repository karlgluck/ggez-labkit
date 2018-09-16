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
                for (int j = aspects.Count - 1; j >= 0; --j)
                {
                    GolemAspectEditorData editableAspect = aspects[j];
                    InspectableFieldInfo[] aspectFields = editableAspect.AspectFields;
                    InspectableVariablePropertyInfo[] aspectVariables = editableAspect.AspectVariables;

                    EditorGUILayout.Space();
                    Rect foldoutRect = EditorGUILayout.GetControlRect();
                    Rect rhsToolsRect = foldoutRect;
                    foldoutRect.xMax = foldoutRect.xMax - EditorGUIUtility.singleLineHeight;
                    rhsToolsRect.xMin = foldoutRect.xMax;
                    editableAspect.Foldout = EditorGUI.Foldout(foldoutRect, editableAspect.Foldout, editableAspect.Field.Name);
                    {
                        if (GUI.Button(rhsToolsRect, "X"))
                        {
                            _editable.RemoveAspect(editableAspect);
                        }
                    }
                    if (!editableAspect.Foldout)
                    {
                        continue;
                    }
                    EditorGUI.indentLevel++;

                    // Fields
                    for (int i = 0; i < aspectFields.Length; ++i)
                    {
                        InspectableFieldInfo fieldInfo = aspectFields[i];
                        GolemEditorUtility.EditorGUILayoutGolemField(
                            fieldInfo.InspectableType,
                            fieldInfo.SpecificType,
                            fieldInfo.FieldInfo,
                            editableAspect.Aspect,
                            editableAspect.FieldsUsingSettings,
                            _editable
                            );
                    }

                    // Variables, if they exist
                    if (aspectVariables.Length > 0)
                    {
                        EditorGUILayout.Space();
                        EditorGUILayout.LabelField("Variables", EditorStyles.boldLabel);
                        EditorGUI.BeginDisabledGroup(!EditorApplication.isPlaying);
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
                            variables.InspectorSet(name, variableType, GolemEditorUtility.EditorGUIField(position, aspectVariables[i].Type, variableType, variables.InspectorGet(name, variableType)));
                        }
                        EditorGUI.EndDisabledGroup();
                    }

                    EditorGUI.indentLevel--;
                }
            }

            //-------------------------------------------------
            // Variables
            //-------------------------------------------------
            if (EditorApplication.isPlaying)
            {
                var variables = _editable.Golem.Variables;
                variables.EditorGUIInspectVariables();
            }
            else
            {
                var variables = _editable.EditorVariables;
                EditorGUILayout.Space();
                EditorGUILayout.LabelField(new GUIContent("Variable Initialization", "Derived from Aspects with properties that have the [Variable] attribute"), EditorStyles.boldLabel);

                for (int i = 0; i < variables.Count; ++i)
                {
                    var variable = variables[i];

                    var labelRect = EditorGUILayout.GetControlRect();
                    var position = new Rect(labelRect);
                    position.xMin += EditorGUIUtility.labelWidth;
                    labelRect.xMax = position.xMin;

                    EditorGUI.LabelField(labelRect, new GUIContent(variable.Name, variable.Tooltip));
                    variable.InitialValue = GolemEditorUtility.EditorGUIField(position, variable.InspectableType, variable.Type, variable.InitialValue);
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
            Settings settings = _editable.Settings;
            if (settings != null)
            {
                EditorGUI.BeginChangeCheck();
                settings.DoEditorGUILayout(true);
                if (EditorGUI.EndChangeCheck())
                {
                    _editable.Save();
                }
                EditorGUILayout.Space();
                _editable.EditorAsset.InheritSettingsFrom = EditorGUILayout.ObjectField("Inherit From", _editable.EditorAsset.InheritSettingsFrom, typeof(SettingsAsset), false) as SettingsAsset;
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

        }

        private void addAspectType(Type type)
        {
            var aspect = Activator.CreateInstance(type) as Aspect;
            _editable.AddAspect(aspect);
        }
    }
}
