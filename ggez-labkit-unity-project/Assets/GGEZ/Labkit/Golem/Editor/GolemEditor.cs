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

        private bool addArchetypeEnabled;

        private SerializedProperty _archetype;
        private SerializedProperty _settings;


        //-----------------------------------------------------
        // OnEnable
        //-----------------------------------------------------
        private void OnEnable()
        {
            _golem = target as Golem;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();
            _golem.Settings.DoEditorGUILayout(true);

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

                EditorGUILayout.EndHorizontal();
                if (_golem.Archetype == null)
                    return;
            }


            int archetypeCount = _golem.Archetype == null ? 0 : 1;
            if (archetypeCount > 0)
                ArchetypeGUI(_golem.Archetype);

            // GrayLine();

            AdvancedGUI();
        }

        private void GrayLine()
        {
            EditorGUILayout.Space();
            Rect rect = GUILayoutUtility.GetLastRect();
            rect.xMin -= 50f;
            rect.xMax += 50f;
            rect.yMin = rect.yMax - 1;
            EditorGUI.DrawRect(rect, Color.gray);
        }

        private void ArchetypeGUI(GolemArchetype archetype)
        {
            GrayLine();

            {
                string key = archetype.GetInstanceID().ToString() + ".Foldout";

                GUIStyle style = new GUIStyle(EditorStyles.foldout);
                style.fontStyle = FontStyle.Bold;

                EditorGUILayout.BeginHorizontal();

                // Foldout
                bool showContents = EditorPrefs.GetBool(key, true);
                showContents = EditorGUILayout.Foldout(showContents, _golem.Archetype.name + " (Archetype)", style);
                EditorPrefs.SetBool(key, showContents);

                GUILayout.FlexibleSpace();

                // Settings button
                if (EditorGUILayout.DropdownButton(GUIContent.none, FocusType.Keyboard, GolemEditorUtility.SettingsButtonStyle))
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Remove Archetype"), false, null);
                    menu.AddItem(
                        new GUIContent("Relocate"),
                        false,
                        (arg) =>
                        {
                            string archetypePath = AssetDatabase.GetAssetPath((UnityObject)arg);
                            string assetName = System.IO.Path.GetFileNameWithoutExtension(archetypePath);
                            string newPath = EditorUtility.SaveFilePanelInProject("Archetype", assetName, "asset", "Move Archetype Asset", archetypePath);
                            if (!string.IsNullOrEmpty(newPath))
                            {
                                string result = AssetDatabase.MoveAsset(archetypePath, newPath);
                                if (!string.IsNullOrEmpty(result))
                                    Debug.LogError(result, _golem);
                            }
                        },
                        _golem.Archetype
                        );

                    // for whatever reason, GetLastRect doesn't return a useful value here so we can't use DropDown
                    menu.ShowAsContext();
                }

                EditorGUILayout.EndHorizontal();

                if (!showContents)
                    return;
            }

            archetype.Settings.DoEditorGUILayout(true);

            archetype.InheritSettingsFrom = EditorGUILayout.ObjectField(new GUIContent(" Inherit From", EditorGUIUtility.FindTexture("FilterByType")), archetype.InheritSettingsFrom, typeof(SettingsAsset), false) as SettingsAsset;

            //-------------------------------------------------
            // Aspects
            //-------------------------------------------------
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal((GUIStyle)"Toolbar");
            EditorGUILayout.LabelField(new GUIContent(" Aspects", EditorGUIUtility.FindTexture("cs Script Icon")));

            GUILayout.FlexibleSpace();

            // Dropdown for adding an aspect of a new type
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
            {
                var dropdownRect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight, EditorStyles.toolbarDropDown, GUILayout.Width(38f));
                if (EditorGUI.DropdownButton(dropdownRect, new GUIContent("New"), FocusType.Passive, EditorStyles.toolbarDropDown))
                {
                    var menu = new GenericMenu();
                    var aspectTypes = Assembly.GetAssembly(typeof(Aspect))
                        .GetTypes()
                        .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Aspect)))
                        .Where(myType => !archetype.EditorAspects.Any(existing => existing.Aspect.GetType() == myType))
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
                            (object _type) => this.AddAspectType((Type)_type),
                            type
                            );
                    }

                    menu.DropDown(dropdownRect);
                }
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();

            //-------------------------------------------------
            // Aspects
            //-------------------------------------------------
            EditorGUI.BeginChangeCheck();

            if (EditorApplication.isPlaying && !_golem.gameObject.IsPrefab())
            {
                var aspects = _golem.Aspects;
                var editorAspects = archetype.EditorAspects;
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
                var editorAspects = archetype.EditorAspects;
                for (int j = editorAspects.Count - 1; j >= 0; --j)
                {
                    EditorAspect editorAspect = editorAspects[j];
                    var inspectableAspectType = InspectableAspectType.GetInspectableAspectType(editorAspect.Aspect.GetType());

                    EditorGUILayout.Space();

                    EditorGUILayout.BeginHorizontal();

                    editorAspect.Foldout = EditorGUILayout.Foldout(editorAspect.Foldout, new GUIContent(" " + inspectableAspectType.Name,  EditorGUIUtility.FindTexture("cs Script Icon")));

                    var settingsRect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight, GolemEditorUtility.SettingsButtonStyle, GUILayout.Width(16f));
                    if (GUI.Button(settingsRect, "", GolemEditorUtility.SettingsButtonStyle))
                    {
                        GenericMenu menu = new GenericMenu();
                        menu.AddItem(new GUIContent("Remove"), false, (arg) => archetype.RemoveAspect((EditorAspect)arg), editorAspect);
                        menu.DropDown(settingsRect);
                    }

                    EditorGUILayout.EndHorizontal();

                    if (!editorAspect.Foldout)
                        continue;

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
                EditorUtility.SetDirty(archetype);

            EditorGUI.BeginChangeCheck();

            //-------------------------------------------------
            // Variables
            //-------------------------------------------------
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal((GUIStyle)"Toolbar");
            EditorGUILayout.LabelField(new GUIContent(" Variables", EditorGUIUtility.FindTexture("CloudConnect")));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            if (EditorApplication.isPlaying)
            {
                var variables = _golem.Variables;

                if (variables != null)
                {
                    foreach (var kvp in variables)
                    {
                        string name = kvp.Key;
                        Variable variable = kvp.Value;

                        var labelRect = EditorGUILayout.GetControlRect();
                        var position = new Rect(labelRect);
                        position.xMin += EditorGUIUtility.labelWidth;
                        labelRect.xMax = position.xMin;

                        EditorGUI.LabelField(labelRect, new GUIContent(name));

                        IUntypedUnaryVariable untypedUnaryVariable = variable as IUntypedUnaryVariable;
                        if (untypedUnaryVariable != null)
                        {
                            untypedUnaryVariable.UntypedValue = GolemEditorUtility.EditorGUIField(position, InspectableTypeExt.GetInspectableTypeOf(untypedUnaryVariable.ValueType), untypedUnaryVariable.ValueType, untypedUnaryVariable.UntypedValue);
                        }
                        else
                        {
                            EditorGUI.LabelField(position, variable.GetType().Name);
                        }
                    }
                }
            }
            else
            {
                var editorVariables = archetype.EditorVariables;

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
            EditorGUILayout.BeginHorizontal((GUIStyle)"Toolbar");
            EditorGUILayout.LabelField(new GUIContent(" Components", EditorGUIUtility.FindTexture("FolderFavorite Icon")));

            GUILayout.FlexibleSpace();

            // Dropdown for adding an aspect of a new type
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
            {
                var dropdownRect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight, EditorStyles.toolbarDropDown, GUILayout.Width(38f));
                if (EditorGUI.DropdownButton(dropdownRect, new GUIContent("New"), FocusType.Passive, EditorStyles.toolbarDropDown))
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(
                        new GUIContent("New Component"),
                        false,
                        AddNewLocalComponentMenuFunction
                        );
                    menu.DropDown(dropdownRect);
                }
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();

            if (EditorApplication.isPlaying)
            {
                #warning TODO draw cell stats on golem at runtime

                var components = archetype.Components;
                for (int i = 0; i < components.Length; ++i)
                {
                    if (components[i] == null)
                    {
                        continue;
                    }
                    if (GUILayout.Button(new GUIContent("Open Editor")))
                    {
                        archetype.EditorWindowSelectedComponent = i;
                        GolemEditorWindow.Open(_golem);
                    }
                }
            }
            else
            {
                var components = archetype.Components;
                for (int i = 0; i < components.Length; ++i)
                {
                    var component = components[i];
                    if (components[i] == null)
                    {
                        archetype.DeduplicateComponents();
                        --i;
                    }

                    EditorGUILayout.Space();

                    EditorGUILayout.BeginHorizontal();

                    bool foldout = archetype.EditorWindowSelectedComponent == i;
                    foldout = EditorGUILayout.Foldout(foldout, new GUIContent(" " + component.name,  EditorGUIUtility.FindTexture("Favorite Icon")));

                    if (GUILayout.Button("Open", EditorStyles.miniButton, GUILayout.MaxWidth(36f)))
                    {
                        foldout = true;
                        GolemEditorWindow.Open(_golem);
                    }

                    if (foldout)
                    {
                        archetype.EditorWindowSelectedComponent = i;
                    }

                    var settingsRect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight, GolemEditorUtility.SettingsButtonStyle, GUILayout.Width(16f));
                    if (GUI.Button(settingsRect, "", GolemEditorUtility.SettingsButtonStyle))
                    {
                        GenericMenu menu = new GenericMenu();
                        menu.AddItem(
                            new GUIContent("Remove"),
                            false,
                            (arg) =>
                            {
                                Undo.RegisterCompleteObjectUndo(archetype, "Remove Component");
                                int index = (int)arg;
                                if (archetype.EditorWindowSelectedComponent >= index)
                                {
                                    --archetype.EditorWindowSelectedComponent;
                                }
                                var list = new List<GolemComponent>(components);
                                list.RemoveAt(index);
                                components = list.ToArray();
                                archetype.Components = components.ToArray();
                            },
                            i);
                        menu.DropDown(settingsRect);
                    }

                    EditorGUILayout.EndHorizontal();

                    if (!foldout)
                        continue;

                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.ObjectField("Component", component, typeof(GolemComponent), false);
                    EditorGUI.EndDisabledGroup();

                }

            }

            if (EditorGUI.EndChangeCheck())
            {
                GolemEditorUtility.SetDirty(_golem);
            }
        }

        private void AddNewLocalComponentMenuFunction()
        {
            var components = _golem.Archetype.Components;
            Array.Resize(ref components, components.Length + 1);
            GolemComponent instance = ScriptableObject.CreateInstance<GolemComponent>();
            components[components.Length-1] = instance;
            string path = AssetDatabase.GenerateUniqueAssetPath(System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(_golem.Archetype)) + "/New Golem Component.asset");
            AssetDatabase.CreateAsset(instance, path);
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


        private void AddAspectType(Type type)
        {
            Debug.Assert(typeof(Aspect).IsAssignableFrom(type));
            _golem.Archetype.AddNewAspect(Activator.CreateInstance(type) as Aspect);
            GolemEditorUtility.SetDirtyArchetype(_golem);
        }


        private void AdvancedGUI()
        {
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
    }
}
