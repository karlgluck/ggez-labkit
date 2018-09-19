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
using UnityObject = UnityEngine.Object;
using System.Collections.Generic;
using System.Linq;

namespace GGEZ.Labkit
{
    [System.Serializable]
    public struct ControlPoint
    {
        public Vector2 Point;
        public Vector2 Tangent;
    }

    //-----------------------------------------------------------------------------
    // GolemEditorUtility
    //-----------------------------------------------------------------------------
    public static class GolemEditorUtility
    {
        public const float editorWindowTabHeight = 21f;

        public static void AddScaledCursorRect(float scale, Rect rect, MouseCursor cursor)
        {
            rect = new Rect(rect.position, rect.size);
            EditorGUIUtility.AddCursorRect(rect, cursor);
        }

        public static void SetDirty(Golem golem)
        {
            var scene = golem.gameObject.scene;
            if (scene.IsValid())
            {
                if (!EditorApplication.isPlaying)
                {
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(scene);
                }
            }
            else
            {
                EditorUtility.SetDirty(golem.gameObject);
            }
            EditorUtility.SetDirty(golem.Asset);
            EditorUtility.SetDirty(golem.EditorAsset);
        }

        //-----------------------------------------------------
        // boldFoldoutStyle
        //-----------------------------------------------------
        private static GUIStyle s_boldFoldoutStyle;
        public static GUIStyle boldFoldoutStyle
        {
            get
            {
                if (s_boldFoldoutStyle == null)
                {
                    s_boldFoldoutStyle = new GUIStyle(EditorStyles.foldout);
                    s_boldFoldoutStyle.fontStyle = FontStyle.Bold;
                }
                return s_boldFoldoutStyle;
            }
        }

        //-----------------------------------------------------
        // outputLabelStyle
        //-----------------------------------------------------
        private static GUIStyle s_outputLabelStyle;
        public static GUIStyle outputLabelStyle
        {
            get
            {
                if (s_outputLabelStyle == null)
                {
                    s_outputLabelStyle = new GUIStyle(EditorStyles.label);
                    s_outputLabelStyle.alignment = TextAnchor.MiddleRight;
                }
                return s_outputLabelStyle;
            }
        }


        //-----------------------------------------------------
        // cellNameStyle
        //-----------------------------------------------------
        private static GUIStyle s_cellNameStyle;
        public static GUIStyle cellNameStyle
        {
            get
            {
                if (s_cellNameStyle == null)
                {
                    s_cellNameStyle = new GUIStyle(EditorStyles.label);
                    s_cellNameStyle.alignment = TextAnchor.MiddleCenter;
                    s_cellNameStyle.fontStyle = FontStyle.Bold;
                }
                return s_cellNameStyle;
            }
        }

        private static bool s_hasActiveDropdown;
        private static int s_activeDropdownControlId;
        private static object s_activeDropdownControlValue;
        private static bool s_hasDropdownControlValueChanged;
        private static int s_activeDropdownHandle;

        /// <summary>
        ///     Works like EditorGUI.DropdownButton, only this function assists the caller in
        ///     directly returning a new value in the same pattern as other GUI functions.
        /// </summary>
        /// <param name="position">Where to draw the button for the dropdown button.</param>
        /// <param name="content">Content for the button.</param>
        /// <param name="value">Current value. Returned unless changed by the menu.</param>
        /// <returns>True if the button was clicked</returns>
        public static bool DropdownField<T>(Rect position, GUIContent content, ref T value)
        {
            object objectValue = value;
            bool retval = DropdownField(position, content, ref objectValue);
            value = (T)objectValue;
            return retval;
        }

        public static bool DropdownField(Rect position, GUIContent content, ref object value)
        {
            var controlId = GUIUtility.GetControlID(FocusType.Keyboard);
            if (EditorGUI.DropdownButton(position, content, FocusType.Keyboard, EditorStyles.popup))
            {
                ++s_activeDropdownHandle;
                s_hasActiveDropdown = true;
                s_activeDropdownControlId = controlId;
                s_activeDropdownControlValue = value;
                s_hasDropdownControlValueChanged = false;
                return true;
            }
            if (s_hasActiveDropdown && s_activeDropdownControlId == controlId)
            {
                if (s_hasDropdownControlValueChanged)
                {
                    s_hasActiveDropdown = false;
                    value = s_activeDropdownControlValue;
                    s_hasDropdownControlValueChanged = false;
                    GUI.changed = true;
                }
            }
            return false;
        }

        public enum ActiveDropdownFieldHandle : int { Invalid = int.MaxValue }

        /// <summary>
        ///     If DropdownField returns true, use this method to get a handle that can
        ///     be later passed to SetDropdownFieldValue to set the value the original
        ///     DropdownField will return.
        /// </summary>
        /// <seealso cref="SetDropdownFieldValue"></seealso>
        public static ActiveDropdownFieldHandle GetActiveDropdownFieldHandle()
        {
            return s_hasActiveDropdown ? (ActiveDropdownFieldHandle)s_activeDropdownHandle : ActiveDropdownFieldHandle.Invalid;
        }

        /// <summary>
        ///     In response to DropdownField returning true, use this method to set the new value
        ///     that the field should return.
        /// </summary>
        /// <seealso cref="GetActiveDropdownFieldHandle"></seealso>
        public static void SetDropdownFieldValue(ActiveDropdownFieldHandle handle, object value)
        {
            if ((int)handle == s_activeDropdownHandle && s_hasActiveDropdown)
            {
                s_activeDropdownControlValue = value;
                s_hasDropdownControlValueChanged = true;
            }
        }

        /// <summary>
        ///     A helper function for GenericMenu.AddItem that calls
        ///     SetDropdownFieldValue.
        /// </summary>
        /// <param name="contextParam">= new object[] { ActiveDropdownFieldHandle, object }</param>
        /// <seealso cref="SetDropdownFieldValue"></seealso>
        /// <seealso cref="GetActiveDropdownFieldHandle"></seealso>
        public static void SetDropdownFieldValueMenuFunction2(object contextParam)
        {
            var context = contextParam as object[];
            var handle = (GolemEditorUtility.ActiveDropdownFieldHandle)context[0];
            GolemEditorUtility.SetDropdownFieldValue(handle, context[1]);
        }

        //-----------------------------------------------------
        // EditorGUIField
        //-----------------------------------------------------
        public static object EditorGUIField(Rect position, InspectableType type, Type actualType, object value)
        {
            switch (type)
            {
                case InspectableType.Int: return EditorGUI.IntField(position, (int)value);
                case InspectableType.Float: return EditorGUI.FloatField(position, (float)value);
                case InspectableType.UnityObject: return EditorGUI.ObjectField(position, (UnityObject)value, actualType, true);
                case InspectableType.Bool: return EditorGUI.Toggle(position, (bool)value);
                case InspectableType.String: return EditorGUI.TextField(position, (string)value);
                case InspectableType.Rect: return EditorGUI.RectField(position, (Rect)value);
                case InspectableType.Color: return EditorGUI.ColorField(position, (Color)value);
                case InspectableType.VariableRef:
                    {
                        throw new InvalidOperationException("Use EditorGUIGolemField to edit a VariableRef");
                    }
                case InspectableType.Vector2:
                    {
                        var vector2 = (Vector2)value;
                        float[] values = new float[] { vector2.x, vector2.y };
                        EditorGUI.MultiFloatField(position, new GUIContent[] { new GUIContent("x"), new GUIContent("y") }, values);
                        return new Vector2(values[0], values[1]);
                    }
                case InspectableType.Vector3:
                    {
                        var vector3 = (Vector3)value;
                        float[] values = new float[] { vector3.x, vector3.y, vector3.z };
                        EditorGUI.MultiFloatField(position, new GUIContent[] { new GUIContent("x"), new GUIContent("y"), new GUIContent("z") }, values);
                        return new Vector3(values[0], values[1], values[2]);
                    }
                case InspectableType.Enum:
                    {
                        return EditorGUI.EnumPopup(position, (Enum)value);
                    }
                // TODO: bounds, boundsint, AnimationCurve, double, Texture, Sprite, long, mask, object? RectInt, vector2int, vector4,
                case InspectableType.KeyCode:
                    {
                        const float widthInput = 18;
                        position.width = position.width - widthInput;

                        GUI.color = Color.white;
                        KeyCode enumValue = (KeyCode)value;
                        enumValue = (KeyCode)EditorGUI.EnumPopup(position, enumValue);
                        position.x += position.width;
                        position.width = widthInput;

                        int id = EditorGUIUtility.GetControlID((int)position.x, FocusType.Keyboard, position);
                        if (Event.current.type == EventType.MouseDown && position.Contains(Event.current.mousePosition))
                        {
                            EditorGUIUtility.keyboardControl = id;
                            Event.current.Use();
                        }

                        GUI.color = EditorGUIUtility.keyboardControl == id ? Color.green : Color.white;
                        position.y -= 2;
                        position.x += 1;
                        GUI.Label(position, GUIContent.none, (GUIStyle)"IN ObjectField");
                        if (EditorGUIUtility.keyboardControl == id && Event.current.type == EventType.KeyUp)
                        {
                            if (Event.current.keyCode != KeyCode.Escape)
                            {
                                enumValue = Event.current.keyCode;
                            }
                            Event.current.Use();
                            EditorGUIUtility.keyboardControl = -1;
                        }
                        else if (EditorGUIUtility.keyboardControl == id && Event.current.isKey)
                        {
                            Event.current.Use();
                        }
                        GUI.color = Color.white;
                        return enumValue;
                    }

                case InspectableType.TriggerRef:
                    {
                        Trigger trigger = (Trigger)value;
                        var enumValues = Enum.GetNames(typeof(Trigger));
                        Array.Resize(ref enumValues, (int)Trigger.__COUNT__);
                        trigger = (Trigger)EditorGUI.Popup(position, (int)trigger, enumValues);
                        return trigger;
                    }

                default: return value;
            }
        }

        public static void EditorGUILayoutGolemField(
                InspectableType inspectableType,
                Type specificType,
                FieldInfo fieldInfo,
                object target,
                Dictionary<string,string> fieldsUsingSettings,
                GolemEditorData golemEditorData
                )
        {
            Rect position = EditorGUILayout.GetControlRect();
            EditorGUIGolemField(position, inspectableType, specificType, fieldInfo, target, fieldsUsingSettings, golemEditorData);
        }
        
        public static void EditorGUIGolemField(
                Rect position,
                InspectableType inspectableType,
                Type specificType,
                FieldInfo fieldInfo,
                object target,
                Dictionary<string,string> fieldsUsingSettings,
                GolemEditorData golemEditorData
                )
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            if (fieldsUsingSettings == null)
            {
                throw new ArgumentNullException("fieldsUsingSettings");
            }
            if (golemEditorData == null)
            {
                throw new ArgumentNullException("golemEditorData");
            }
            string fieldName = fieldInfo.Name;
            Rect labelRect = new Rect(position);
            Rect valueRect = new Rect(position);
            valueRect.xMin = valueRect.xMin + Mathf.Min(EditorGUIUtility.labelWidth, valueRect.width / 2f);
            labelRect.xMax = valueRect.xMin;
            bool hasSetting = false;
            string setting = null;
            if (InspectableTypeExt.CanUseSetting(inspectableType))
            {
                hasSetting = fieldsUsingSettings.TryGetValue(fieldName, out setting);
                if (hasSetting != EditorGUI.ToggleLeft(labelRect, fieldName, hasSetting))
                {
                    if (hasSetting)
                    {
                        fieldsUsingSettings.Remove(fieldName);
                    }
                    hasSetting = !hasSetting;
                }
            }
            else
            {
                EditorGUI.LabelField(labelRect, fieldName);
            }
            object value;
            if (hasSetting)
            {
                if (!golemEditorData.Settings.Contains(setting, specificType))
                {
                    setting = null;
                }
                if (specificType == null)
                {
                    EditorGUI.DropdownButton(valueRect, TypeNotSetErrorGUIContent, FocusType.Keyboard);
                }
                else if (GolemEditorUtility.DropdownField(valueRect, setting == null ? GolemEditorUtility.NoSettingGUIContent : new GUIContent(setting), ref setting))
                {
                    GenericMenu menu = new GenericMenu();

                    string newSettingString = "New " + specificType.Name + " Setting...";

                    HashSet<string> encounteredSettings = new HashSet<string>();
                    object handle = GolemEditorUtility.GetActiveDropdownFieldHandle();

                    Settings current = golemEditorData.Settings;
                    bool needsRootSeparator = false;
                    bool isSelf = true;
                    while (current != null)
                    {
                        if (needsRootSeparator)
                        {
                            menu.AddSeparator("");
                            needsRootSeparator = false;
                        }

                        string prefix = isSelf ? "" : (current.Name + "/");
                        menu.AddItem(
                                new GUIContent(prefix + newSettingString),
                                false,
                                SetDropdownFieldValueToNewSetting,
                                new object[]{handle, current, specificType}
                                );

                        var enumerator = current.Values.GetEnumerator();
                        if (enumerator.MoveNext())
                        {
                            menu.AddSeparator(prefix);
                            do
                            {
                                string name = enumerator.Current.Name;
                                if (specificType.IsAssignableFrom(enumerator.Current.Type))
                                {
                                    if (encounteredSettings.Contains(name))
                                    {
                                        menu.AddDisabledItem(new GUIContent(prefix + name + " (overridden)"));
                                    }
                                    else
                                    {
                                        menu.AddItem(
                                            new GUIContent(prefix + name),
                                            setting == name,
                                            GolemEditorUtility.SetDropdownFieldValueMenuFunction2,
                                            new object[]{ handle, name }
                                            );
                                    }
                                }
                                encounteredSettings.Add(name);
                            } while (enumerator.MoveNext());
                        }

                        current = current.InheritFrom == null ? null : current.InheritFrom.Settings;
                        needsRootSeparator = needsRootSeparator || isSelf;
                        isSelf = false;
                    }
                    menu.DropDown(valueRect);
                }
                fieldsUsingSettings[fieldName] = setting;
                value = golemEditorData.Settings.Get(setting, specificType);
            }
            else
            {
                value = fieldInfo.GetValue(target);
                switch (inspectableType)
                {
                    default:
                        value = EditorGUIField(valueRect, inspectableType, specificType, value);
                        break;

                    case InspectableType.VariableRef:
                    {
                        VariableRef reference = (VariableRef)value;

                        Rect left = valueRect, right = valueRect;
                        float leftSize = valueRect.width * 0.3f;
                        left.xMax = left.xMin + leftSize;
                        right.xMin = left.xMax;
                        reference.Relationship = (EntityRelationship)EditorGUI.EnumPopup(left, reference.Relationship);

                        string variableName = golemEditorData.ContainsVariable(reference.Name, specificType) ? reference.Name : null;

                        GUIContent content;
                        bool typeNotSet = specificType == null;
                        EditorGUI.BeginDisabledGroup(typeNotSet);
                        if (typeNotSet)
                        {
                            content = GolemEditorUtility.TypeNotSetErrorGUIContent;
                        }
                        else if (variableName == null)
                        {
                            content = GolemEditorUtility.NoVariableErrorGUIContent;
                        }
                        else
                        {
                            content = new GUIContent(reference.Name);
                        }

                        if (DropdownField(right, content, ref variableName))
                        {
                            GenericMenu menu = new GenericMenu();
                            var variables = golemEditorData.EditorVariables;
                            int variablesAdded = 0;
                            ActiveDropdownFieldHandle handle = GetActiveDropdownFieldHandle();

                            for (int i = 0; i < variables.Count; ++i)
                            {
                                var variable = variables[i];

                                if (!specificType.IsAssignableFrom(variable.Type))
                                {
                                    continue;
                                }

                                foreach (Type aspectType in variable.SourceAspects)
                                {
                                    ++variablesAdded;
                                    menu.AddItem(
                                        new GUIContent(aspectType.Name + "/" + variable.Name),
                                        variable.Name == variableName,
                                        SetDropdownFieldValueMenuFunction2,
                                        new object[]{ handle, variable.Name }
                                        );
                                }
                            }

                            if (variablesAdded == 0)
                            {
                                menu.AddDisabledItem(new GUIContent("Add an Aspect to create variables"));
                            }

                            menu.DropDown(right);
                        }
                        EditorGUI.EndDisabledGroup();
                        reference.Name = variableName;
                        value = reference;
                        break;
                    }
                }
            }
            fieldInfo.SetValue(target, value);
        }

        private static void SetDropdownFieldValueToNewSetting(object contextParam)
        {
            var context = contextParam as object[];
            var handle = (GolemEditorUtility.ActiveDropdownFieldHandle)context[0];
            var settings = context[1] as Settings;
            var settingType = context[2] as Type;

            string newSettingName = Guid.NewGuid().ToString();
            settings.Add(newSettingName, settingType);
            GolemEditorUtility.SetDropdownFieldValue(handle, newSettingName);
        }

        //-----------------------------------------------------
        // GenerateGridTexture
        //-----------------------------------------------------
        public static Texture2D GenerateGridTexture(int size, int minorLine, Color background, Color line)
        {
            Texture2D tex = new Texture2D(size, size);
            Color[] cols = new Color[size * size];
            for (int y = 0; y < size; ++y)
            {
                for (int x = 0; x < size; ++x)
                {
                    Color col = background;
                    if (y % minorLine == 0 || x % minorLine == 0) col = Color.Lerp(line, background, 0.65f);
                    if (y + 1 == size || x + 1 == size) col = Color.Lerp(line, background, 0.35f);
                    cols[y * size + x] = col;
                }
            }
            tex.SetPixels(cols);
            tex.wrapMode = TextureWrapMode.Repeat;
            tex.filterMode = FilterMode.Bilinear;
            tex.name = "Grid";
            tex.Apply();
            return tex;
        }

        //-----------------------------------------------------
        // GenerateSolidColorTexture
        //-----------------------------------------------------
        public static Texture2D GenerateSolidColorTexture(int size, Color color)
        {
            Texture2D tex = new Texture2D(size, size);
            Color[] cols = new Color[size * size];
            for (int y = 0; y < size; ++y)
            {
                for (int x = 0; x < size; ++x)
                {
                    cols[y * size + x] = color;
                }
            }
            tex.SetPixels(cols);
            tex.wrapMode = TextureWrapMode.Repeat;
            tex.filterMode = FilterMode.Bilinear;
            tex.name = "Solid";
            tex.Apply();
            return tex;
        }


        public static int GridSize { get { return (int)EditorGUIUtility.singleLineHeight; } }

        //-----------------------------------------------------
        // Node Styles
        //-----------------------------------------------------

        public enum NodeStyleColor
        {
            Gray,
            Blue,
            BlueGreen,
            Green,
            Yellow,
            Orange,
            Red
        }

        private static GUIStyle[] s_nodeStyles;
        public static GUIStyle[] NodeStyles
        {
            get
            {
                if (s_nodeStyles == null)
                {
                    s_nodeStyles = new GUIStyle[7];
                    for (int i = 0; i < s_nodeStyles.Length; ++i)
                    {
                        var style = new GUIStyle("flow node " + i);
                        style.padding = new RectOffset(0, 0, 0, 0);
                        style.margin = new RectOffset(0, 0, 0, 0);
                        s_nodeStyles[i] = style;
                    }
                }
                return s_nodeStyles;
            }
        }

        private static GUIStyle[] s_nodeStylesSelected;
        public static GUIStyle[] NodeStylesSelected
        {
            get
            {
                if (s_nodeStylesSelected == null)
                {
                    s_nodeStylesSelected = new GUIStyle[7];
                    for (int i = 0; i < s_nodeStylesSelected.Length; ++i)
                    {
                        var style = new GUIStyle("flow node " + i + " on");
                        style.padding = new RectOffset(0, 0, 0, 0);
                        style.margin = new RectOffset(0, 0, 0, 0);
                        s_nodeStylesSelected[i] = style;
                    }
                }
                return s_nodeStylesSelected;
            }
        }

        private static GUIStyle s_settingToggleStyle;

        public static GUIStyle SettingToggleStyle
        {
            get
            {
                if (s_settingToggleStyle == null)
                {
                    s_settingToggleStyle = new GUIStyle(EditorStyles.toggle);
                }
                return s_settingToggleStyle;
            }
        }

        //-----------------------------------------------------
        // GridTexture
        //-----------------------------------------------------
        private static Texture2D s_gridTexture;
        public static Texture2D GridTexture
        {
            get
            {
                if (s_gridTexture == null)
                {
                    s_gridTexture = GolemEditorUtility.GenerateGridTexture(GridSize * 10, GridSize, new Color(0.9f, 0.9f, 0.9f), new Color(0.8f, 0.8f, 0.8f));
                }
                return s_gridTexture;
            }
        }

        //-----------------------------------------------------
        // NoSettingGUIContent
        //-----------------------------------------------------
        private static GUIContent s_noSettingGUIContent;
        public static GUIContent NoSettingGUIContent
        {
            get
            {
                if (s_noSettingGUIContent == null)
                {
                    s_noSettingGUIContent = new GUIContent(" No Setting", EditorGUIUtility.FindTexture("d_console.erroricon.sml"));
                }
                return s_noSettingGUIContent;
            }
        }

        //-----------------------------------------------------
        // NoVariableGUIContent
        //-----------------------------------------------------
        private static GUIContent s_noVariableErrorGUIContent;
        public static GUIContent NoVariableErrorGUIContent
        {
            get
            {
                if (s_noVariableErrorGUIContent == null)
                {
                    s_noVariableErrorGUIContent = new GUIContent(" No Variable", EditorGUIUtility.FindTexture("d_console.erroricon.sml"));
                }
                return s_noVariableErrorGUIContent;
            }
        }

        private static GUIContent s_typeNotSetErrorGUIContent;
        public static GUIContent TypeNotSetErrorGUIContent
        {
            get
            {
                if (s_typeNotSetErrorGUIContent == null)
                {
                    s_typeNotSetErrorGUIContent = new GUIContent(" Type Not Set", EditorGUIUtility.FindTexture("d_console.erroricon.sml"));
                }
                return s_typeNotSetErrorGUIContent;
            }
        }

        //-----------------------------------------------------
        // SelectedColor
        //-----------------------------------------------------
        public static readonly Color SelectedColor = new Color(116f / 255f, 184f / 255f, 242f / 255f);
        public static readonly Color WireColor = new Color(0.3f, 0.3f, 0.3f);
        public const float SelectedWireWidth = 6f;
        public const float WireWidth = 4f;

        //-----------------------------------------------------
        // SelectedBackgroundTexture
        //-----------------------------------------------------
        private static Texture2D s_selectedBackgroundTexture;
        public static Texture2D SelectedBackgroundTexture
        {
            get
            {
                if (s_selectedBackgroundTexture == null)
                {
                    s_selectedBackgroundTexture = GolemEditorUtility.GenerateSolidColorTexture(64, SelectedColor);
                }
                return s_selectedBackgroundTexture;
            }
        }

        //-----------------------------------------------------
        // SnapToGrid
        //-----------------------------------------------------
        public static Vector2 SnapToGrid(Vector2 v)
        {
            var offset = new Vector2(v.x - (int)(v.x / GridSize) * GridSize, v.y - (int)(v.y / GridSize) * GridSize);
            if (offset.x < 0)
            {
                offset.x += GridSize;
            }
            if (offset.y < 0)
            {
                offset.y += GridSize;
            }
            return v - offset;
        }

        //-----------------------------------------------------
        // SnapToGrid
        //-----------------------------------------------------
        public static Rect SnapToGrid(Rect r)
        {
            float xMin = r.xMin - (int)(r.xMin / GridSize) * GridSize;
            float yMin = r.yMin - (int)(r.yMin / GridSize) * GridSize;
            if (xMin < 0)
            {
                xMin += GridSize;
            }
            if (yMin < 0)
            {
                yMin += GridSize;
            }
            float xMax = r.xMax - (int)(1 + r.xMax / GridSize) * GridSize;
            float yMax = r.yMax - (int)(1 + r.yMax / GridSize) * GridSize;
            if (xMax < 0)
            {
                xMax += GridSize;
            }
            if (yMax < 0)
            {
                yMax += GridSize;
            }
            return Rect.MinMaxRect(r.xMin - xMin, r.yMin - yMin, r.xMax - xMax, r.yMax - yMax);
        }

        //-----------------------------------------------------
        // BeginNode
        //-----------------------------------------------------
        public const float NodeLeftRightMargin = 5f;
        public static readonly float NodeTopMargin = EditorGUIUtility.singleLineHeight * 1.5f;
        public static readonly float NodeBottomMargin = EditorGUIUtility.singleLineHeight * 0.5f;
        public static Rect BeginNode(string name, Rect position, bool selected, Vector2 anchor, NodeStyleColor color = NodeStyleColor.Gray)
        {
            // Compute the on-screen position and round the value because it significantly improves rendering quality
            var rect = new Rect(anchor + position.position, position.size);
            rect.xMin = Mathf.Round(rect.xMin); rect.yMin = Mathf.Round(rect.yMin);
            rect.xMax = Mathf.Round(rect.xMax); rect.yMax = Mathf.Round(rect.yMax);

            var titleRect = GetNodeTitleRect(rect);
            GUIStyle labelColor = new GUIStyle(EditorStyles.label);
            if (selected)
            {
                labelColor.normal.textColor = Color.white;
            }
            var nodeStyle = (selected ? NodeStylesSelected : NodeStyles)[(int)color];
            EditorGUI.LabelField(titleRect, name, GolemEditorUtility.cellNameStyle);
            GUILayout.BeginArea(new Rect(rect.position + new Vector2(0f, EditorGUIUtility.singleLineHeight), rect.size - new Vector2(0f, EditorGUIUtility.singleLineHeight)), nodeStyle);
            GUILayout.EndArea();
            GUILayout.BeginArea(new Rect(rect.position + new Vector2(NodeLeftRightMargin, NodeTopMargin), rect.size - new Vector2(NodeLeftRightMargin * 2f, NodeTopMargin + NodeBottomMargin)));
            GUILayout.BeginVertical();
            return rect;
        }

        //-----------------------------------------------------
        // GetNodeTitleRect
        //-----------------------------------------------------
        public static Rect GetNodeTitleRect(Rect position)
        {
            return new Rect(position.position, new Vector2(position.size.x, NodeTitleRectHeight));
        }

        public static float NodeTitleRectHeight { get { return EditorGUIUtility.singleLineHeight; } }

        //-----------------------------------------------------
        // GetNodeResizeRect
        //-----------------------------------------------------
        public static Rect GetNodeResizeRect(Rect position)
        {
            var size = new Vector2(EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);
            return new Rect(position.position + position.size - size, size);
        }

        //-----------------------------------------------------
        // GetNodeInputPortRect
        //-----------------------------------------------------
        public static Rect GetNodeInputPortRect(Rect position, Vector2 portCenterFromTopLeft)
        {
            var portCenter = new Vector2(position.position.x + NodeLeftRightMargin + portCenterFromTopLeft.x,
                                          position.position.y + NodeTopMargin + portCenterFromTopLeft.y);
            return GetPortRect(portCenter);
        }

        //-----------------------------------------------------
        // GetNodeOutputPortRect
        //-----------------------------------------------------
        public static Rect GetNodeOutputPortRect(Rect position, Vector2 portCenterFromTopRight)
        {
            var portCenter = new Vector2(position.position.x + position.width - NodeLeftRightMargin + portCenterFromTopRight.x,
                                          position.position.y + NodeTopMargin + portCenterFromTopRight.y);
            return GetPortRect(portCenter);
        }

        //-----------------------------------------------------
        // EndNode
        //-----------------------------------------------------
        public static void EndNode()
        {
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        //-----------------------------------------------------
        // DrawPort
        //-----------------------------------------------------
        public static readonly float PortLayoutWidth = GridSize + NodeLeftRightMargin;
        public static void DrawPort(Vector2 position, bool filled, bool selected)
        {
            // var texture = (filled ? EditorStyles.radioButton.active : EditorStyles.radioButton.normal).background;
            // var portSize = new Vector2(texture.width, texture.height);
            // GUI.DrawTextureWithTexCoords (new Rect(position - portSize * 0.5f, portSize), texture, new Rect (0, 0, 1, 1), true);

            var oldColor = Handles.color;
            Handles.color = selected ? GolemEditorUtility.SelectedColor : GolemEditorUtility.WireColor;
            Handles.DrawWireDisc(position, Vector3.forward, EditorGUIUtility.singleLineHeight * 0.4f);
            if (filled)
            {
                Handles.color = selected ? GolemEditorUtility.SelectedColor : GolemEditorUtility.WireColor;
                Handles.DrawSolidDisc(position, Vector3.forward, EditorGUIUtility.singleLineHeight * 0.25f);
            }
            Handles.color = oldColor;
        }

        //-----------------------------------------------------
        // GetPortRect
        //-----------------------------------------------------
        public static Rect GetPortRect(Vector2 position)
        {
            float halfSize = EditorGUIUtility.singleLineHeight * 0.4f;
            float size = 2 * halfSize;
            return new Rect(position.x - halfSize, position.y - halfSize, size, size);
        }

        //-----------------------------------------------------
        // DrawBezier
        //-----------------------------------------------------
        public static void DrawBezier(Vector2 from, Vector2 to, Vector2 fromTangent, Vector2 toTangent, bool selected)
        {
            Handles.DrawBezier(
                    from,
                    to,
                    from + fromTangent,
                    to + toTangent,
                    selected ? SelectedColor : WireColor,
                    null,
                    selected ? SelectedWireWidth : WireWidth
                    );
        }

        public static void DrawExpressionSquiggle(Vector2 from, Vector2 to, Vector2 expressionAnchor, bool selected, bool flip, Vector2 anchor)
        {
            const float tangentSize = 50f;
            from += anchor;
            to += anchor;
            expressionAnchor += anchor;
            var middle = (to + from) * 0.5f;
            Handles.DrawBezier(
                middle,
                expressionAnchor,
                middle + (from - to).normalized * tangentSize,
                expressionAnchor + (flip ? Vector2.right : Vector2.left) * tangentSize,
                selected ? SelectedColor : WireColor,
                null,
                2f
            );
        }

        //-----------------------------------------------------
        // DrawEdge
        //-----------------------------------------------------
        public static void DrawEdge(ControlPoint from, ControlPoint to, ControlPoint[] controlPoints, bool selected, Vector2 anchor)
        {
            if (controlPoints == null || controlPoints.Length == 0)
            {
                DrawBezier(
                        anchor + from.Point,
                        anchor + to.Point,
                        from.Tangent,
                        to.Tangent,
                        selected
                        );
                return;
            }

            var secondPoint = controlPoints[0];
            DrawBezier(
                    anchor + from.Point,
                    anchor + secondPoint.Point,
                    from.Tangent,
                    -secondPoint.Tangent,
                    selected
                    );

            var secondFromLastPoint = controlPoints[controlPoints.Length - 1];
            DrawBezier(
                    anchor + secondFromLastPoint.Point,
                    anchor + to.Point,
                    secondFromLastPoint.Tangent,
                    to.Tangent,
                    selected
                    );

            for (int i = controlPoints.Length - 2; i >= 0; --i)
            {
                var start = controlPoints[i];
                var end = controlPoints[i + 1];
                DrawBezier(
                        anchor + start.Point,
                        anchor + end.Point,
                        start.Tangent,
                        -end.Tangent,
                        selected
                        );
            }
        }


        public static void GetEditorTransitionPoints(EditorState fromState, EditorState toState, bool hasReverse, out Vector2 from, out Vector2 to)
        {
            var fromCenter = fromState.Position.center - Vector2.down * GolemEditorUtility.NodeTitleRectHeight * 0.5f;
            var toCenter = toState.Position.center - Vector2.down * GolemEditorUtility.NodeTitleRectHeight * 0.5f;

            // fromCenter = OmnibusEditorUtility.SnapToGrid(fromCenter);
            // toCenter = OmnibusEditorUtility.SnapToGrid(toCenter);

            if (hasReverse)
            {
                const float halfSeparation = 8f;
                var bumpV3 = Vector3.Cross(Vector3.back, toCenter - fromCenter);
                var bump = new Vector2(bumpV3.x, bumpV3.y).normalized * halfSeparation;
                from = fromCenter + bump;
                to = toCenter + bump;
            }
            else
            {
                from = fromCenter;
                to = toCenter;
            }
        }

        //-----------------------------------------------------
        // DrawTransition
        //-----------------------------------------------------
        public static void DrawTransition(EditorTransition editorTransition, EditorState fromState, EditorState toState, bool hasReverse, bool selected, Vector2 anchor)
        {
            Vector2 from, to;
            GolemEditorUtility.GetEditorTransitionPoints(
                fromState,
                toState,
                hasReverse,
                out from,
                out to
            );

            GolemEditorUtility.DrawEdge(
                new ControlPoint { Point = from },
                new ControlPoint { Point = to },
                null,
                selected,
                anchor
                );

            // Draw the triangle
            var middle = anchor + (from + to) * 0.5f;
            var parallel = (to - from).normalized;
            var bumpVector3 = Vector3.Cross(Vector3.forward, parallel);
            const float arrowHalfWidth = 7f;
            const float arrowForward = 7f;
            const float arrowBackward = 6f;
            var bump = new Vector2(bumpVector3.x * arrowHalfWidth, bumpVector3.y * arrowHalfWidth);
            var oldColor = Handles.color;
            Handles.color = selected ? SelectedColor : WireColor;
            Handles.DrawAAConvexPolygon(middle + parallel * arrowForward, middle + bump - parallel * arrowBackward, middle - bump - parallel * arrowBackward);

            var deltaA = editorTransition.ExpressionAnchor - from;
            var deltaB = to - from;
            float side = deltaA.x * deltaB.y - deltaA.y * deltaB.x;
            bool flipTreeSide;

            if (Mathf.Abs(deltaB.x) > Mathf.Abs(deltaB.y))
            {
                flipTreeSide = deltaB.x > 0;
            }
            else
            {
                flipTreeSide = side < 0f == deltaB.y > 0f;
            }


            GolemEditorUtility.DrawExpressionSquiggle(
                from,
                to,
                editorTransition.ExpressionAnchor,
                selected,
                flipTreeSide,
                anchor
                );

            GolemEditorUtility.UpdateExpressionPositions(editorTransition, flipTreeSide);
            GolemEditorUtility.DrawTransitionExpression(anchor, editorTransition.Expression, flipTreeSide);

            Handles.color = oldColor;
        }

        //-----------------------------------------------------
        // DrawTransitionExpression
        //-----------------------------------------------------
        private static Vector2 leftCenter(Rect rect)
        {
            return new Vector2(rect.xMin, (rect.yMin + rect.yMax) * 0.5f);
        }
        private static Vector2 rightCenter(Rect rect)
        {
            return new Vector2(rect.xMax, (rect.yMin + rect.yMax) * 0.5f);
        }
        public static void DrawTransitionExpression(Vector2 offset, EditorTransitionExpression expression, bool flipLeftRight)
        {
            var position = expression.Position;
            var rect = new Rect(offset.x + position.x, offset.y + position.y, position.width, position.height);

            switch (expression.Type)
            {
                case EditorTransitionExpressionType.False:
                    EditorGUI.LabelField(rect, "false", new GUIStyle("sv_label_0"));
                    break;

                case EditorTransitionExpressionType.True:
                    EditorGUI.LabelField(rect, "true", new GUIStyle("sv_label_0"));
                    break;

                case EditorTransitionExpressionType.Trigger:
                    EditorGUI.LabelField(rect, expression.Trigger.ToString(), new GUIStyle("sv_label_0"));
                    break;

                case EditorTransitionExpressionType.And:
                    EditorGUI.LabelField(rect, "and", new GUIStyle("sv_label_0"));
                    break;

                case EditorTransitionExpressionType.Or:
                    EditorGUI.LabelField(rect, "or", new GUIStyle("sv_label_0"));
                    break;

                default:
                    throw new System.NotImplementedException();
            }

            if (HasSubexpressionsAttribute.IsFoundOn(expression.Type))
            {
                foreach (var subexpression in expression.Subexpressions)
                {
                    Vector2 from, to, fromTangent, toTangent;
                    if (flipLeftRight)
                    {
                        from = leftCenter(rect) + Vector2.right * 2f;
                        to = offset + rightCenter(subexpression.Position);
                        fromTangent = Vector2.left * 10f;
                        toTangent = Vector2.right * 10f;
                    }
                    else
                    {
                        from = rightCenter(rect) + Vector2.left * 2f;
                        to = offset + leftCenter(subexpression.Position);
                        fromTangent = Vector2.right * 10f;
                        toTangent = Vector2.left * 10f;
                    }
                    Handles.DrawBezier(
                            from,
                            to,
                            from + fromTangent,
                            to + toTangent,
                            Handles.color,
                            null,
                            2f
                            );
                    DrawTransitionExpression(offset, subexpression, flipLeftRight);
                }
            }
        }

        private const float typeLabelWidth = 65f;
        private static float typeLabelHeight { get { return EditorGUIUtility.singleLineHeight; } }
        private const float spaceBetweenLabelAndSubexpressions = 10f;

        public static void UpdateExpressionPositions(EditorTransition editorTransition, bool flipLeftRight)
        {
            computeTransitionExpressionSizes(editorTransition.Expression);
            editorTransition.Position = new Rect(
                editorTransition.ExpressionAnchor.x,
                editorTransition.ExpressionAnchor.y - editorTransition.Expression.Position.height * 0.5f,
                editorTransition.Expression.Position.width,
                editorTransition.Expression.Position.height
                );
            if (flipLeftRight)
            {
                editorTransition.Position.x -= editorTransition.Position.width;
            }
            computeTransitionExpressionPositions(editorTransition.ExpressionAnchor, editorTransition.Expression, editorTransition.Expression.Position.position.x, flipLeftRight);
        }

        private static Rect computeTransitionExpressionSizes(EditorTransitionExpression expression)
        {
            switch (expression.Type)
            {
                case EditorTransitionExpressionType.False:
                case EditorTransitionExpressionType.True:
                    expression.Position = new Rect(typeLabelWidth, typeLabelHeight, typeLabelWidth, typeLabelHeight);
                    break;


                case EditorTransitionExpressionType.Trigger:
                    expression.Position = new Rect(typeLabelWidth * 2f, typeLabelHeight, typeLabelWidth * 2f, typeLabelHeight);
                    break;

                case EditorTransitionExpressionType.And:
                case EditorTransitionExpressionType.Or:
                    var widestLabel = 0f;
                    foreach (var subexpression in expression.Subexpressions)
                    {
                        var subSize = computeTransitionExpressionSizes(subexpression);
                        widestLabel = Mathf.Max(subSize.x, widestLabel);
                    }
                    float height = 0f, width = 0f;
                    foreach (var subexpression in expression.Subexpressions)
                    {
                        var subSize = subexpression.Position;
                        var additionalWidthFromSiblings = widestLabel - subSize.x;
                        subSize.width += additionalWidthFromSiblings;
                        width = Mathf.Max(subSize.width, width);
                        height += subSize.height;
                    }
                    expression.Position = new Rect(typeLabelWidth, typeLabelHeight, width + typeLabelWidth + spaceBetweenLabelAndSubexpressions, Mathf.Max(height, typeLabelHeight));
                    break;

                default:
                    throw new System.NotImplementedException();
            }

            return expression.Position;
        }

        private static void computeTransitionExpressionPositions(Vector2 offset, EditorTransitionExpression expression, float siblingMaxLabelWidth, bool flipLeftRight)
        {
            var labelSize = expression.Position.position;
            var totalSize = expression.Position.size;
            expression.Position = new Rect(offset.x, offset.y - labelSize.y * 0.5f, labelSize.x, labelSize.y);

            float heightSoFar = -totalSize.y * 0.5f;
            float suboffsetX;

            if (flipLeftRight)
            {
                expression.Position.x -= labelSize.x;
                suboffsetX = offset.x - siblingMaxLabelWidth - spaceBetweenLabelAndSubexpressions;
            }
            else
            {
                suboffsetX = offset.x + siblingMaxLabelWidth + spaceBetweenLabelAndSubexpressions;
            }

            float childrenMaxLabelWidth = typeLabelWidth;
            foreach (var subexpression in expression.Subexpressions)
            {
                var subLabelSize = subexpression.Position.position;
                childrenMaxLabelWidth = Mathf.Max(subLabelSize.x, childrenMaxLabelWidth);
            }

            Debug.Assert((expression.Subexpressions.Count == 0) || HasSubexpressionsAttribute.IsFoundOn(expression.Type));
            foreach (var subexpression in expression.Subexpressions)
            {
                var subLabelSize = subexpression.Position.position;
                var subTotalSize = subexpression.Position.size;
                computeTransitionExpressionPositions(new Vector2(suboffsetX, offset.y + heightSoFar + subTotalSize.y * 0.5f), subexpression, childrenMaxLabelWidth, flipLeftRight);
                heightSoFar += subTotalSize.y;
            }
        }
    }


    //-----------------------------------------------------------------------------
    //-----------------------------------------------------------------------------
    public class RepresentsAttribute : Attribute
    {
        public readonly Type Type;
        public readonly bool CanUseSettings;
        public RepresentsAttribute(Type type, bool canUseSettings = true)
        {
            Type = type;
            CanUseSettings = canUseSettings;
        }
    }

    //-----------------------------------------------------------------------------
    //-----------------------------------------------------------------------------
    public enum InspectableType
    {
        [Represents(typeof(float))] Float,
        [Represents(typeof(int))] Int,
        [Represents(typeof(bool))] Bool,
        [Represents(typeof(string))] String,
        [Represents(typeof(Rect))] Rect,
        [Represents(typeof(Color))] Color,
        [Represents(typeof(Vector2))] Vector2,
        [Represents(typeof(Vector3))] Vector3,
        [Represents(typeof(UnityEngine.Object))] UnityObject,
        [Represents(typeof(VariableRef), false)] VariableRef,
        [Represents(typeof(Enum))] Enum,
        [Represents(typeof(KeyCode))] KeyCode,
        [Represents(typeof(Trigger), false)] TriggerRef,

        Invalid = int.MaxValue,
    }

    //-----------------------------------------------------------------------------
    //-----------------------------------------------------------------------------
    public static partial class InspectableTypeExt
    {
        private static Dictionary<Type, InspectableType> s_typeToInspectableType;
        private static HashSet<InspectableType> s_typesThatCanUseSettings = new HashSet<InspectableType>();
        private static Type[] s_representedType;
        public static InspectableType GetInspectableTypeOf(Type type)
        {
            // If we haven't built our type-map for the enum yet, do it now
            if (s_typeToInspectableType == null)
            {
                s_typeToInspectableType = new Dictionary<Type, InspectableType>();
                var enumType = typeof(InspectableType);
                var enumValues = Enum.GetValues(enumType);
                var enumNames = Enum.GetNames(enumType);
                s_representedType = new Type[enumValues.Length];
                for (int i = 0; i < enumValues.Length; ++i)
                {
                    var value = (InspectableType)enumValues.GetValue(i);
                    var member = enumType.GetMember(enumNames[i]);
                    var attributes = member[0].GetCustomAttributes(typeof(RepresentsAttribute), false);
                    foreach (RepresentsAttribute attribute in attributes)
                    {
                        s_representedType[(int)value] = attribute.Type;
                        s_typeToInspectableType.Add(attribute.Type, value);
                        if (attribute.CanUseSettings)
                        {
                            s_typesThatCanUseSettings.Add(value);
                        }
                    }
                }
            }

            // Try a direct lookup
            InspectableType retval;
            if (s_typeToInspectableType.TryGetValue(type, out retval))
            {
                return retval;
            }

            // Try inheritance
            foreach (var kvp in s_typeToInspectableType)
            {
                if (kvp.Key.IsAssignableFrom(type))
                {
                    return kvp.Value;
                }
            }

            Debug.LogWarning(type.Name + " is not an InspectableType");
            return InspectableType.Invalid;
        }

        public static Type GetSpecificType(InspectableType inspectableType, FieldInfo fieldInfo)
        {
            if (inspectableType == InspectableType.VariableRef)
            {
                var targetTypeAttributes = fieldInfo.GetCustomAttributes(typeof(VariableTypeAttribute), false);
                if (targetTypeAttributes.Length > 0)
                {
                    var typeAttribute = targetTypeAttributes[0] as VariableTypeAttribute;
                    return typeAttribute.Type;
                }
                Debug.LogError("Field " + fieldInfo.Name + " in " + fieldInfo.DeclaringType.Name + " is not annotated with a variable type");
                return null;
            }
            else
            {
                return fieldInfo.FieldType;
            }
        }

        public static Type GetRepresentedType(InspectableType inspectableType)
        {
            int index = (int)inspectableType;
            if (index < 0 || index >= s_representedType.Length)
            {
                return null;
            }
            return s_representedType[index];
        }

        public static bool CanUseSetting(InspectableType inspectableType)
        {
            return s_typesThatCanUseSettings.Contains(inspectableType);
        }


    }

    //-----------------------------------------------------------------------------
    //-----------------------------------------------------------------------------
    public struct InspectableFieldInfo
    {
        public readonly InspectableType InspectableType;
        public readonly Type SpecificType;
        public readonly FieldInfo FieldInfo;
        public readonly bool WantsSetting;

        public InspectableFieldInfo(InspectableType inspectableType, Type specificType, FieldInfo fieldInfo, bool wantsSetting)
        {
            InspectableType = inspectableType;
            SpecificType = specificType;
            FieldInfo = fieldInfo;
            WantsSetting = wantsSetting;
        }

        public static InspectableFieldInfo[] GetFields(object target)
        {
            FieldInfo[] fields = target.GetType().GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
            var retval = new InspectableFieldInfo[fields.Length];
            int j = 0;
            for (int i = 0; i < fields.Length; ++i)
            {
                InspectableType inspectableType = InspectableTypeExt.GetInspectableTypeOf(fields[i].FieldType);
                if (inspectableType == InspectableType.Invalid)
                {
                    continue;
                }
                bool wantsSetting = SettingAttribute.IsDeclaredOn(fields[i]);
                Type specificType = InspectableTypeExt.GetSpecificType(inspectableType, fields[i]);
                retval[j++] = new InspectableFieldInfo(inspectableType, specificType, fields[i], wantsSetting);
            }
            Array.Resize(ref retval, j);
            return retval;
        }
    }

    //-----------------------------------------------------------------------------
    //-----------------------------------------------------------------------------
    public struct InspectableVariablePropertyInfo
    {
        public readonly InspectableType Type;
        public readonly PropertyInfo PropertyInfo;
        public readonly VariableAttribute VariableAttribute;

        public InspectableVariablePropertyInfo(InspectableType type, PropertyInfo propertyInfo, VariableAttribute variableAttribute)
        {
            Type = type;
            PropertyInfo = propertyInfo;
            VariableAttribute = variableAttribute;
        }

        public static InspectableVariablePropertyInfo[] GetVariableProperties(object target)
        {
            var properties = target.GetType().GetProperties();
            var retval = new InspectableVariablePropertyInfo[properties.Length];
            int j = 0;
            for (int i = 0; i < properties.Length; ++i)
            {
                var attributes = properties[i].GetCustomAttributes(typeof(VariableAttribute), false);
                VariableAttribute variableAttribute = null;
                for (int k = 0; k < attributes.Length && variableAttribute == null; ++k)
                {
                    variableAttribute = attributes[k] as VariableAttribute;
                }
                if (variableAttribute == null)
                {
                    continue;
                }
                var inspectableType = InspectableTypeExt.GetInspectableTypeOf(properties[i].PropertyType);
                if (inspectableType != InspectableType.Invalid)
                {
                    retval[j++] = new InspectableVariablePropertyInfo(inspectableType, properties[i], attributes[0] as VariableAttribute);
                }
            }
            Array.Resize(ref retval, j);
            return retval;
        }
    }



    //-----------------------------------------------------------------------------
    //-----------------------------------------------------------------------------
    public struct InspectableDictionaryKeyValuePair
    {
        public readonly InspectableType Type;
        public readonly string Key;
        public readonly object Value;

        public InspectableDictionaryKeyValuePair(InspectableType type, string key, object value)
        {
            Type = type;
            Key = key;
            Value = value;
        }

        public static InspectableDictionaryKeyValuePair[] GetDictionaryKeyValuePairs(Dictionary<string, object> target)
        {
            var retval = new InspectableDictionaryKeyValuePair[target.Count];
            int j = 0;
            foreach (var kvp in target)
            {
                object value = kvp.Value;
                Type valueType = value.GetType();
                var inspectableType = InspectableTypeExt.GetInspectableTypeOf(valueType);
                if (inspectableType != InspectableType.Invalid)
                {
                    retval[j++] = new InspectableDictionaryKeyValuePair(inspectableType, kvp.Key, value);
                }
            }
            Array.Resize(ref retval, j);
            return retval;
        }
    }


    public class InspectableCellType
    {
        public struct Input
        {
            public readonly string Name;
            public readonly Type Type;
            public readonly FieldInfo Field;
            public readonly Vector2 PortCenterFromTopLeft;
            public Input(string name, Type type, FieldInfo field, Vector2 portCenterFromTopLeft)
            {
                Name = name;
                Type = type;
                Field = field;
                PortCenterFromTopLeft = portCenterFromTopLeft;
            }
        }

        public struct Output
        {
            public readonly string Name;
            public readonly Type Type;
            public readonly FieldInfo Field;
            public readonly Vector2 PortCenterFromTopRight;
            public Output(string name, Type type, FieldInfo field, Vector2 portCenterFromTopRight)
            {
                Name = name;
                Type = type;
                Field = field;
                PortCenterFromTopRight = portCenterFromTopRight;
            }
        }

        public struct Field
        {
            public readonly InspectableType Type;
            public readonly Type SpecificType;
            public readonly FieldInfo FieldInfo;

            public Field(InspectableType type, Type targetType, FieldInfo fieldInfo)
            {
                Type = type;
                SpecificType = targetType;
                FieldInfo = fieldInfo;
            }
        }

        public InspectableCellType(Input[] inputs, Output[] outputs, Field[] fields)
        {
            Inputs = inputs;
            Outputs = outputs;
            Fields = fields;
        }

        public readonly Input[] Inputs;
        public readonly Output[] Outputs;
        public readonly Field[] Fields;

        private static Dictionary<Type, InspectableCellType> s_typeToInspectableType = new Dictionary<Type, InspectableCellType>();
        public static InspectableCellType GetInspectableCellType(Type cellType)
        {
            InspectableCellType retval;
            if (s_typeToInspectableType.TryGetValue(cellType, out retval))
            {
                return retval;
            }

            var inputs = cellType.GetFields(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public).Where((f) => f.IsDefined(typeof(InAttribute), false)).ToArray();
            var returnedInputs = new Input[inputs.Length];
            for (int i = 0; i < inputs.Length; ++i)
            {
                var inAttribute = inputs[i].GetCustomAttributes(typeof(InAttribute), false)[0] as InAttribute;
                var portCenter = new Vector2(GolemEditorUtility.GridSize - GolemEditorUtility.NodeLeftRightMargin, EditorGUIUtility.singleLineHeight * (0.5f + i));
                returnedInputs[i] = new Input(inputs[i].Name, inAttribute.Type, inputs[i], portCenter);
            }

            var outputs = cellType.GetFields(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public).Where((f) => f.IsDefined(typeof(OutAttribute), false)).ToArray();
            var returnedOutputs = new Output[outputs.Length];
            for (int i = 0; i < outputs.Length; ++i)
            {
                var outAttribute = outputs[i].GetCustomAttributes(typeof(OutAttribute), false)[0] as OutAttribute;
                var portCenter = new Vector2(-GolemEditorUtility.GridSize + GolemEditorUtility.NodeLeftRightMargin, EditorGUIUtility.singleLineHeight * (0.5f + i));
                returnedOutputs[i] = new Output(outputs[i].Name, outAttribute.Type, outputs[i], portCenter);
            }

            var fields = cellType.GetFields(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public).Where((f) => !f.IsDefined(typeof(InAttribute), false) && !f.IsDefined(typeof(OutAttribute), false)).ToArray();
            var returnedFields = new Field[fields.Length];
            {
                int j = 0;
                for (int i = 0; i < fields.Length; ++i)
                {
                    var inspectableType = InspectableTypeExt.GetInspectableTypeOf(fields[i].FieldType);
                    if (inspectableType != InspectableType.Invalid)
                    {
                        var targetType = InspectableTypeExt.GetSpecificType(inspectableType, fields[i]);
                        returnedFields[j++] = new Field(inspectableType, targetType, fields[i]);
                    }
                }
                Array.Resize(ref returnedFields, j);
            }

            retval = new InspectableCellType(returnedInputs, returnedOutputs, returnedFields);
            s_typeToInspectableType.Add(cellType, retval);
            return retval;
        }

        public static bool CanConnect(InspectableCellType readCell, int outputIndex, InspectableCellType writeCell, int inputIndex)
        {
            Type writeType = writeCell.Inputs[inputIndex].Type;
            Type readType = readCell.Outputs[outputIndex].Type;

            // If the output wants an object, send 'em anything
            if (writeType.Equals(typeof(object)))
            {
                return true;
            }

            // If the output is a value type we can't do type conversion so the types must match
            if (readType.IsValueType || writeType.IsValueType)
            {
                return readType.Equals(writeType);
            }

            // An object is being passed to something that wants an object: make sure the assignment is possible
            return writeType.IsAssignableFrom(readType);
        }
    }

    public class InspectableScriptType
    {
        public readonly string Name;
        public readonly Field[] Fields;

        public struct Field
        {
            public readonly InspectableType Type;
            public readonly Type SpecificType;
            public readonly FieldInfo FieldInfo;

            public Field(InspectableType type, Type specificType, FieldInfo fieldInfo)
            {
                Type = type;
                SpecificType = specificType;
                FieldInfo = fieldInfo;
            }
        }

        public InspectableScriptType(string name, Field[] fields)
        {
            Name = name;
            Fields = fields;
        }

        private static Dictionary<Type, InspectableScriptType> s_typeToInspectableType = new Dictionary<Type, InspectableScriptType>();
        public static InspectableScriptType GetInspectableScriptType(Type scriptType)
        {
            InspectableScriptType retval;
            if (s_typeToInspectableType.TryGetValue(scriptType, out retval))
            {
                return retval;
            }

            var fields = scriptType.GetFields(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public).Where((f) => !f.IsDefined(typeof(InAttribute), false) && !f.IsDefined(typeof(OutAttribute), false)).ToArray();
            var returnedFields = new Field[fields.Length];
            {
                int j = 0;
                for (int i = 0; i < fields.Length; ++i)
                {
                    var inspectableType = InspectableTypeExt.GetInspectableTypeOf(fields[i].FieldType);
                    if (inspectableType != InspectableType.Invalid)
                    {
                        var specificType = InspectableTypeExt.GetSpecificType(inspectableType, fields[i]);
                        returnedFields[j++] = new Field(inspectableType, specificType, fields[i]);
                    }
                }
                Array.Resize(ref returnedFields, j);
            }

            retval = new InspectableScriptType(scriptType.Name, returnedFields);
            s_typeToInspectableType.Add(scriptType, retval);
            return retval;
        }
    }
}
