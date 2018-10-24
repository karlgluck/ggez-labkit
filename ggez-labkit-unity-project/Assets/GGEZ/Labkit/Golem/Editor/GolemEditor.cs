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

        private SerializedProperty _archetype;
        private SerializedProperty _settings;


        //-----------------------------------------------------
        // OnEnable
        //-----------------------------------------------------
        private void OnEnable()
        {
            _golem = target as Golem;
        }
bool fold;
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

            EditorGUILayout.LabelField("Local Settings", EditorStyles.boldLabel);
            _golem.Settings.DoEditorGUILayout(true);

            EditorGUILayout.Space();
            var style = new GUIStyle(EditorStyles.foldout);
            style.fontStyle = FontStyle.Bold;
            // fold = EditorGUILayout.Foldout(fold, _golem.Archetype.name + "(Archetype)", style);

            {
                Rect rect = GUILayoutUtility.GetLastRect();
                rect.xMin -= 50f;
                rect.xMax += 50f;
                rect.yMin = rect.yMax - 1;
                EditorGUI.DrawRect(rect, Color.gray);
            }

            // if (_golem.Archetype == null)
            {
                _golem.Archetype = EditorGUILayout.ObjectField("Archetype", _golem.Archetype, typeof(GolemArchetype), false) as GolemArchetype;
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (_golem.Archetype == null)
                {
                    if (GUILayout.Button("New"))
                    {
                        _golem.Archetype = ScriptableObject.CreateInstance<GolemArchetype>();
                        string path = AssetDatabase.GenerateUniqueAssetPath(System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetOrScenePath(_golem)) + "/New Archetype.asset");
                        AssetDatabase.CreateAsset(_golem.Archetype, path);
                    }
                }
                // else
                {

                    string archetypePath = _golem.Archetype == null ? null : AssetDatabase.GetAssetPath(_golem.Archetype);
                    EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(archetypePath));
                    if (GUILayout.Button("Relocate..."))
                    {
                        string assetName = System.IO.Path.GetFileNameWithoutExtension(archetypePath);
                        string newPath = EditorUtility.SaveFilePanelInProject("Archetype", assetName, "asset", "Move Archetype Asset", archetypePath);
                        if (!string.IsNullOrEmpty(newPath))
                        {
                            string result = AssetDatabase.MoveAsset(archetypePath, newPath);
                            if (!string.IsNullOrEmpty(result))
                                Debug.LogError(result, _golem);
                        }
                    }
                    EditorGUI.EndDisabledGroup();
                }

                EditorGUILayout.EndHorizontal();
                if (_golem.Archetype == null)
                    return;
            }

            EditorGUILayout.LabelField("Archetype Settings", EditorStyles.boldLabel);
            _golem.Archetype.Settings.DoEditorGUILayout(true);

            // Settings Inheritance
            {
                EditorGUILayout.Space();
                _golem.Archetype.InheritSettingsFrom = EditorGUILayout.ObjectField("Inherit From", _golem.Archetype.InheritSettingsFrom, typeof(SettingsAsset), false) as SettingsAsset;
                Settings current = _golem.Archetype.Settings.Parent;
                while (current != null)
                {
                    EditorGUI.BeginChangeCheck();
                    current.DoEditorGUILayout(false);
                    if (EditorGUI.EndChangeCheck() && current.Owner != null)
                    {
                        Undo.RegisterCompleteObjectUndo(current.Owner, current.Owner.name + " Settings");
                        EditorUtility.SetDirty(current.Owner);
                    }
                    current = current.Parent;
                }
            }

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
            EditorGUI.BeginChangeCheck();

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
                            editorAspect.FieldsUsingVariables,
                            _golem
                            );
                    }

                    EditorGUI.indentLevel--;
                }
            }

            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(_golem.Archetype);

            EditorGUI.BeginChangeCheck();

            //-------------------------------------------------
            // Variables
            //-------------------------------------------------
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(new GUIContent("Variables"), EditorStyles.boldLabel);
            if (EditorApplication.isPlaying)
            {
                var variables = _golem.Variables;
                var editorVariables = _golem.Archetype.EditorVariables;

                for (int i = 0; i < editorVariables.Count; ++i)
                {
                    var editorVariable = editorVariables[i];
                    var variable = variables[editorVariable.Name];

                    var labelRect = EditorGUILayout.GetControlRect();
                    var position = new Rect(labelRect);
                    position.xMin += EditorGUIUtility.labelWidth;
                    labelRect.xMax = position.xMin;

                    EditorGUI.LabelField(labelRect, new GUIContent(editorVariable.Name, editorVariable.Tooltip));
                    GolemEditorUtility.EditorGUIField(position, editorVariable.InspectableType, editorVariable.Type, variable);
                }
            }
            else
            {
                var editorVariables = _golem.Archetype.EditorVariables;

                string focusedControlName = GUI.GetNameOfFocusedControl();

                for (int i = 0; i < editorVariables.Count; ++i)
                {
                    var variable = editorVariables[i];

                    var labelRect = EditorGUILayout.GetControlRect();
                    var position = new Rect(labelRect);
                    position.xMin += EditorGUIUtility.labelWidth;
                    labelRect.xMax = position.xMin;


                    string name = variable.Name;
                    string labelControlName = i.ToString("000") + ":var:" + name;
                    GUI.SetNextControlName(labelControlName);
                    bool isSettingFocused = focusedControlName == labelControlName;
                    string newName = EditorGUI.DelayedTextField(labelRect, name, isSettingFocused ? EditorStyles.textField : EditorStyles.label);
                    if (newName != name)
                    {
                        #warning TODO update everything that references a variable when the name changes
                        variable.Name = newName;
                        if (isSettingFocused)
                        {
                            GUI.FocusControl(null);
                        }
                    }

                    variable.InitialValue = GolemEditorUtility.EditorGUIField(position, variable.InspectableType, variable.Type, variable.InitialValue) as Variable;
                }
            }

            //-------------------------------------------------
            // Components
            //-------------------------------------------------
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(new GUIContent("Components"), EditorStyles.boldLabel);
            if (EditorApplication.isPlaying)
            {
                #warning TODO draw cell stats on golem at runtime

                var components = _golem.Archetype.Components;
                for (int i = 0; i < components.Length; ++i)
                {
                    if (components[i] == null)
                    {
                        continue;
                    }
                    if (GUILayout.Button(new GUIContent("Open Editor")))
                    {
                        _golem.Archetype.EditorWindowSelectedComponent = i;
                        GolemEditorWindow.Open(_golem);
                    }
                }
            }
            else
            {
                var components = _golem.Archetype.Components;
                for (int i = 0; i < components.Length; ++i)
                {
                    if (components[i] == null)
                    {
                        _golem.Archetype.DeduplicateComponents();
                        --i;
                    }

                    GUILayout.BeginHorizontal();

                    if (AssetDatabase.IsMainAsset(components[i]))
                    {
                        // this is an on-disk component
                        if (GUILayout.Button(new GUIContent("Ping"), GUILayout.Width(20f)))
                        {
                            EditorGUIUtility.PingObject(components[i]);
                        }
                        if (GUILayout.Button(new GUIContent("Copy to Embedded")))
                        {
                            components[i] = ScriptableObject.Instantiate(components[i]);
                            GUI.changed = true;
                        }
                    }
                    else
                    {
                        // this is a local component
                        if (GUILayout.Button(new GUIContent("Extract to Asset")))
                        {
                            string directory = System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetOrScenePath(_golem));
                            string path = AssetDatabase.GenerateUniqueAssetPath(System.IO.Path.Combine(directory, "Extracted Golem Component.asset"));
                            AssetDatabase.CreateAsset(components[i], path);
                            EditorGUIUtility.PingObject(components[i]);
                            GUI.changed = true;
                        }
                    }


                    if (GUILayout.Button(new GUIContent("Open Editor")))
                    {
                        _golem.Archetype.EditorWindowSelectedComponent = i;
                        GolemEditorWindow.Open(_golem);
                    }
                    if (GUILayout.Button(new GUIContent("X")))
                    {
                        if (_golem.Archetype.EditorWindowSelectedComponent >= i)
                        {
                            --_golem.Archetype.EditorWindowSelectedComponent;
                        }
                        var list = new List<GolemComponent>(components);
                        list.RemoveAt(i);
                        components = list.ToArray();
                        --i;
                        _golem.Archetype.Components = components.ToArray();
                    }
                    GUILayout.EndHorizontal();
                }

                Rect rect = EditorGUILayout.GetControlRect();
                if (EditorGUI.DropdownButton(rect, new GUIContent("Add Component..."), FocusType.Keyboard))
                {
                    GenericMenu menu = new GenericMenu();

                    menu.AddItem(new GUIContent("New Local Component"), false, AddNewLocalComponentMenuFunction);
                    menu.AddSeparator("");

                    string[] assetGuids = AssetDatabase.FindAssets("t:" + typeof(GolemComponent).Name);
                    for (int i = 0; i < assetGuids.Length; ++i)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(assetGuids[i]);
                        menu.AddItem(new GUIContent(path), false, AddAssetComponentMenuFunction, assetGuids[i]);
                    }

                    if (assetGuids.Length == 0)
                    {
                        menu.AddDisabledItem(new GUIContent("No Components in Assets"));
                    }

                    menu.DropDown(rect);
                }
            }

            if (EditorGUI.EndChangeCheck())
            {
                GolemEditorUtility.SetDirty(_golem);
            }

            EditorGUILayout.Space();
            bool advanced = EditorGUILayout.Foldout(EditorPrefs.GetBool("GolemEditorAdvancedFoldout"), "Advanced");
            EditorPrefs.SetBool("GolemEditorAdvancedFoldout", advanced);
            if (advanced)
            {
                EditorGUILayout.LabelField("Golem - Settings Json", EditorStyles.boldLabel);
                EditorGUILayout.TextArea(_golem.Settings.Json);

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Archetype - Json", EditorStyles.boldLabel);
                EditorGUILayout.TextArea(_golem.Archetype.Json);

                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Archetype - Editor Json", EditorStyles.boldLabel);
                EditorGUILayout.TextArea(_golem.Archetype.EditorJson);

                var components = _golem.Archetype.Components;
                for (int i = 0; i < components.Length; ++i)
                {
                    EditorGUILayout.Space();

                    EditorGUILayout.LabelField("["+i+"] - Json", EditorStyles.boldLabel);
                    EditorGUILayout.TextArea(components[i].Json);

                    EditorGUILayout.Space();

                    EditorGUILayout.LabelField("["+i+"] - Editor Json", EditorStyles.boldLabel);
                    EditorGUILayout.TextArea(components[i].EditorJson);

                }
            }


        }

        private void AddNewLocalComponentMenuFunction()
        {
            var components = _golem.Archetype.Components;
            Array.Resize(ref components, components.Length + 1);
            components[components.Length-1] = ScriptableObject.CreateInstance<GolemComponent>();
            _golem.Archetype.Components = components;
        }

        private void AddAssetComponentMenuFunction(object guid)
        {
            var component = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath((string)guid)) as GolemComponent;
            Debug.Assert(component != null);

            var components = _golem.Archetype.Components;
            Array.Resize(ref components, components.Length + 1);
            components[components.Length-1] = component;
            _golem.Archetype.Components = components;
            _golem.Archetype.DeduplicateComponents();
            GolemEditorUtility.SetDirtyArchetype(_golem);

        }


        private void addAspectType(Type type)
        {
            Debug.Assert(typeof(Aspect).IsAssignableFrom(type));
            _golem.Archetype.AddNewAspect(Activator.CreateInstance(type) as Aspect);
            GolemEditorUtility.SetDirtyArchetype(_golem);
        }
    }
}
