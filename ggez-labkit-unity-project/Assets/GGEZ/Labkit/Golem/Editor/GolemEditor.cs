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


        //-----------------------------------------------------
        // OnEnable
        //-----------------------------------------------------
        private void OnEnable()
        {
            _golem = target as Golem;
        }

        //-----------------------------------------------------
        // OnInspectorGUI
        //-----------------------------------------------------
        public override void OnInspectorGUI()
        {

            //-------------------------------------------------
            // Settings
            //-------------------------------------------------
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
            Settings settings = _golem.Archetype.Settings;
            if (settings != null)
            {
                EditorGUI.BeginChangeCheck();
                settings.DoEditorGUILayout(true);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(_golem.Archetype);
                }
                EditorGUILayout.Space();
                _golem.Archetype.InheritSettingsFrom = EditorGUILayout.ObjectField("Inherit From", _golem.Archetype.InheritSettingsFrom, typeof(SettingsAsset), false) as SettingsAsset;
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

            EditorGUI.BeginChangeCheck();

            //-------------------------------------------------
            // Aspects
            //-------------------------------------------------
            EditorGUILayout.LabelField("Aspects", EditorStyles.boldLabel);

            // Dropdown for adding an aspect of a new type
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
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
                        .Where(myType => !_golem.Archetype.EditorAspects.Any(existing => existing.Aspect.GetType() == myType))
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
            EditorGUI.EndDisabledGroup();

            //-------------------------------------------------
            // Aspects
            //-------------------------------------------------
            if (EditorApplication.isPlaying && !_golem.gameObject.IsPrefab())
            {
                var aspects = _golem.Aspects;
                var editorAspects = _golem.Archetype.EditorAspects;
                for (int j = editorAspects.Count - 1; j >= 0; --j)
                {
                    EditorAspect editorAspect = editorAspects[j];
                    var inspectableAspectType = InspectableAspectType.GetInspectableAspectType(editorAspect.Aspect.GetType());

                    EditorGUILayout.LabelField(editorAspect.GetType().Name);
                    #warning TODO draw the live fields for the aspect
                }
            }
            else
            {
                var editorAspects = _golem.Archetype.EditorAspects;
                for (int j = editorAspects.Count - 1; j >= 0; --j)
                {
                    EditorAspect editorAspect = editorAspects[j];
                    var inspectableAspectType = InspectableAspectType.GetInspectableAspectType(editorAspect.Aspect.GetType());

                    EditorGUILayout.Space();
                    Rect foldoutRect = EditorGUILayout.GetControlRect();
                    Rect rhsToolsRect = foldoutRect;
                    foldoutRect.xMax = foldoutRect.xMax - EditorGUIUtility.singleLineHeight;
                    rhsToolsRect.xMin = foldoutRect.xMax;
                    editorAspect.Foldout = EditorGUI.Foldout(foldoutRect, editorAspect.Foldout, inspectableAspectType.Name);
                    {
                        if (GUI.Button(rhsToolsRect, "X"))
                        {
                            _golem.Archetype.RemoveAspect(editorAspect);
                        }
                    }
                    if (!editorAspect.Foldout)
                    {
                        continue;
                    }
                    EditorGUI.indentLevel++;

                    // Fields
                    //--------------------------
                    var fields = inspectableAspectType.Fields;
                    for (int i = 0; i < fields.Length; ++i)
                    {
                        InspectableAspectType.Field field = fields[i];
                        GolemEditorUtility.EditorGUILayoutGolemField(
                            field.Type,
                            field.SpecificType,
                            field.FieldInfo,
                            editorAspect.Aspect,
                            editorAspect.FieldsUsingSettings,
                            _golem.Archetype
                            );
                    }

                    EditorGUI.indentLevel--;
                }
            }

            //-------------------------------------------------
            // Variables
            //-------------------------------------------------
            EditorGUILayout.Space();
            if (EditorApplication.isPlaying)
            {
                var variables = _golem.Variables;
                var editorVariables = _golem.Archetype.EditorVariables;
                EditorGUILayout.LabelField(new GUIContent("Variables"), EditorStyles.boldLabel);

                for (int i = 0; i < editorVariables.Count; ++i)
                {
                    var editorVariable = editorVariables[i];
                    var variable = variables[editorVariable.Name];

                    var labelRect = EditorGUILayout.GetControlRect();
                    var position = new Rect(labelRect);
                    position.xMin += EditorGUIUtility.labelWidth;
                    labelRect.xMax = position.xMin;

                    EditorGUI.LabelField(labelRect, new GUIContent(editorVariable.Name, editorVariable.Tooltip));
                    #warning Draw variable values at runtime on Golem
                    // editorVariable.InitialValue = GolemEditorUtility.EditorGUIField(position, editorVariable.InspectableType, editorVariable.Type, editorVariable.InitialValue);
                }
            }
            else
            {
                var editorVariables = _golem.Archetype.EditorVariables;
                EditorGUILayout.LabelField(new GUIContent("Variables"), EditorStyles.boldLabel);

                for (int i = 0; i < editorVariables.Count; ++i)
                {
                    var variable = editorVariables[i];

                    var labelRect = EditorGUILayout.GetControlRect();
                    var position = new Rect(labelRect);
                    position.xMin += EditorGUIUtility.labelWidth;
                    labelRect.xMax = position.xMin;

                    EditorGUI.LabelField(labelRect, new GUIContent(variable.Name, variable.Tooltip));
                    variable.InitialValue = GolemEditorUtility.EditorGUIField(position, variable.InspectableType, variable.Type, variable.InitialValue);
                }
            }

            //-------------------------------------------------
            // Components
            //-------------------------------------------------
            EditorGUILayout.Space();
            if (EditorApplication.isPlaying)
            {
                #warning TODO draw cell stats on golem at runtime
            }
            else
            {
                var components = _golem.Archetype.Components;
                for (int i = 0; i < components.Length; ++i)
                {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(new GUIContent("Open " + components[i].name)))
                    {
                        _golem.Archetype.EditorWindowSelectedComponent = i;
                        GolemEditorWindow.Open(_golem);
                    }
                    if (GUILayout.Button(new GUIContent("Remove " + components[i].name)))
                    {
                        if (_golem.Archetype.EditorWindowSelectedComponent >= i)
                        {
                            --_golem.Archetype.EditorWindowSelectedComponent;
                        }
                        var list = new List<GolemComponent>(components);
                        list.RemoveAt(i);
                        components = list.ToArray();
                        --i;
                    }
                    GUILayout.EndHorizontal();
                }

                #warning Create a better way to add components

                var newComponent = EditorGUILayout.ObjectField(new GUIContent("Add Component"), null, typeof(GolemComponent), false) as GolemComponent;
                if (newComponent != null)
                {
                    Array.Resize(ref components, components.Length + 1);
                    components[components.Length-1] = newComponent;
                }
            }

            // EditorGUILayout.Space();
            // if (GUILayout.Button("Open Editor"))
            // {
            //     GolemEditorWindow.Open(_editable);
            // }
            // EditorGUILayout.Space();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(_golem.Archetype);
            }


        }

        private void addAspectType(Type type)
        {
            Debug.Assert(typeof(Aspect).IsAssignableFrom(type));
            _golem.Archetype.AddNewAspect(Activator.CreateInstance(type) as Aspect);
        }
    }
}
