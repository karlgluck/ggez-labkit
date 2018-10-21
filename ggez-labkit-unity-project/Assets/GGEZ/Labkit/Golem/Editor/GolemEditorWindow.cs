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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GGEZ.FullSerializer;
using System;
using System.Reflection;
using System.Linq;
using GGEZ;

namespace GGEZ.Labkit
{
    public class GolemEditorWindow : EditorWindow
    {
        public static void Open(Golem golem)
        {
            var window = EditorWindow.GetWindow<GolemEditorWindow>("Golem Editor");
            window.Initialize(golem);
            window.Show();
        }

        [MenuItem("Window/Golem Editor")]
        public static void OpenEmpty()
        {
            Open(null);
        }

        public void Initialize(Golem golem)
        {
            _selection = null;
            _golem = golem;
            _scrollPosition = Vector2.zero;
            _scrollAnchor = Vector2.zero;
            _scrollSize = this.position.size;

            Read();
        }
        private Golem _golem;

        private void OnEnable()
        {
            if (_golem == null)
            {
                Initialize(null);
            }

            Undo.undoRedoPerformed += OnUndo;
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndo;
        }

        private void OnUndo()
        {
            _shouldRead = true;
            this.Repaint();
        }

        private void OnFocus()
        {
            _shouldRead |= _hierarchyChanged;
        }

        private void OnLostFocus()
        {
            _hierarchyChanged = false;
        }

        private void OnHierarchyChange()
        {
            _hierarchyChanged = true;
        }

        private bool _hierarchyChanged = false;

        private Vector2 _scrollPosition;
        private Vector2 _scrollAnchor;
        private Vector2 _anchor { get { return _scrollPosition + _scrollAnchor; } }
        private Vector2 _scrollSize;

        private bool _shouldScroll = true;
        private Vector2 _mouseDownPosition;
        private Vector2 _scrollPositionOnMouseDown;

        private List<EditorCell> _cells { get { return _golem.Archetype.Components[_golem.Archetype.EditorWindowSelectedComponent].EditorCells; } }
        private List<EditorWire> _wires { get { return _golem.Archetype.Components[_golem.Archetype.EditorWindowSelectedComponent].EditorWires; } }
        private bool _shouldRead = false;

        private List<EditorState> _states { get { return _golem.Archetype.Components[_golem.Archetype.EditorWindowSelectedComponent].EditorStates; } }
        private List<EditorTransition> _transitions { get { return _golem.Archetype.Components[_golem.Archetype.EditorWindowSelectedComponent].EditorTransitions; } }
        private List<EditorVariableInputRegister> _variableInputRegisters { get { return _golem.Archetype.Components[_golem.Archetype.EditorWindowSelectedComponent].EditorVariableInputRegisters; } }

        private bool _rightClickIsContextMenu;

        private void Read()
        {
            if (_golem == null)
            {
                return;
            }

            _createTransition.Enabled = false;
            _createWire.Enabled = false;

            // Reload the object
            _shouldRead = false;
        }


        private EditorTransition pickEditorTransitionExpressionHitbox(Vector2 graphPosition)
        {
            foreach (var transitionToInspect in _transitions)
            {
                if (transitionToInspect.Position.Contains(graphPosition))
                {
                    return transitionToInspect;
                }
            }
            return null;
        }

        private struct PickEditorTransitionExpressionWorklistItem
        {
            public readonly EditorTransitionExpression Expression;
            public readonly EditorTransitionExpression Parent;
            public readonly int Index;

            public PickEditorTransitionExpressionWorklistItem(
                EditorTransitionExpression expression,
                EditorTransitionExpression parent,
                int index
                )
            {
                Expression = expression;
                Parent = parent;
                Index = index;
            }
        }

        private class PickedEditorTransitionExpression
        {
            public EditorTransition Transition;
            public EditorTransitionExpression Expression;
            public EditorTransitionExpression Parent;
            public int IndexInParent;
        }

        private PickedEditorTransitionExpression pickEditorTransitionExpression(Vector2 graphPosition)
        {
            var transition = pickEditorTransitionExpressionHitbox(graphPosition);
            if (transition == null)
            {
                return null;
            }

            List<PickEditorTransitionExpressionWorklistItem> worklist = new List<PickEditorTransitionExpressionWorklistItem>();
            worklist.Add(new PickEditorTransitionExpressionWorklistItem(
                transition.Expression,
                null,
                -1
            ));

            int sentinel = 9999999;
            while (worklist.Count > 0 && --sentinel > 0)
            {
                int index = worklist.Count - 1;
                var item = worklist[index];
                worklist.RemoveAt(index);
                var expression = item.Expression;

                if (expression.Position.Contains(graphPosition))
                {
                    return new PickedEditorTransitionExpression()
                    {
                        Parent = item.Parent,
                        IndexInParent = item.Index,
                        Expression = expression,
                        Transition = transition,
                    };
                }

                var subexpressions = expression.Subexpressions;
                Debug.Assert((subexpressions.Count == 0) || HasSubexpressionsAttribute.IsFoundOn(expression.Type));
                for (int j = 0; j < subexpressions.Count; ++j)
                {
                    var subexpression = subexpressions[j];

                    worklist.Add(new PickEditorTransitionExpressionWorklistItem(
                        subexpression,
                        expression,
                        j
                    ));
                }
            }
            if (sentinel <= 0)
            {
                Debug.LogError("Infinite recursion in pickEditorTransitionExpression");
            }
            if (worklist.Count > 0)
            {
                Debug.LogError("Worklist wasn't empty when terminated");
                worklist.Clear();
            }

            return null;
        }

        private bool editorTransitionHasReverse(EditorTransition editorTransition)
        {
            var fromState = _states[(int)editorTransition.From];
            var toState = _states[(int)editorTransition.To];
            bool hasReverse = false;
            var fromStateIndex = fromState.Index;
            var possiblyReversedTransitions = toState.TransitionsOut;
            for (int i = 0; !hasReverse && i < possiblyReversedTransitions.Count; ++i)
            {
                hasReverse = _transitions[(int)possiblyReversedTransitions[i]].To == fromStateIndex;
            }
            return hasReverse;
        }

        private void getEditorTransitionPoints(EditorTransition editorTransition, out Vector2 from, out Vector2 to)
        {
            var fromState = _states[(int)editorTransition.From];
            var toState = _states[(int)editorTransition.To];

            bool hasReverse = editorTransitionHasReverse(editorTransition);

            GolemEditorUtility.GetEditorTransitionPoints(
                fromState,
                toState,
                hasReverse,
                out from,
                out to
            );
        }

        private object _selection = null;

        private IDraggable _draggable = null;


        private InspectableCellType GetInspectableCellType(Type cellType)
        {
            return InspectableCellType.GetInspectableCellType(cellType);
        }

        private InspectableScriptType GetInspectableScriptType(Type scriptType)
        {
            return InspectableScriptType.GetInspectableScriptType(scriptType);
        }


        private float _graphScale = 1f;

        bool _shouldWrite = false;



        //-----------------------------------------------------
        // OnGUI
        //-----------------------------------------------------
        private void OnGUI()
        {
            //-------------------------------------------------
            // Draw some placeholder text if no circuit is active
            //-------------------------------------------------
            if (_golem == null)
            {
                if (Selection.activeGameObject != null && Event.current.type == EventType.Layout)
                {
                    _golem = Selection.activeGameObject.GetComponent<Golem>();
                }

                if (_golem != null)
                {
                    _shouldRead = true;
                    Repaint();
                }
                else
                {
                    GUILayout.BeginVertical();
                    GUILayout.FlexibleSpace();

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("Golem Component Editor", EditorStyles.largeLabel);
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("Select a golem, pick a Component, and press 'Open Editor'", EditorStyles.miniLabel);
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.FlexibleSpace();
                    GUILayout.EndVertical();

                    return;
                }
            }

            if (_shouldRead && Event.current.type == EventType.Layout)
            {
                Read();
            }

            if (Event.current.type == EventType.ScrollWheel)
            {
                var mousePosition = Event.current.mousePosition;
                float oldScale = _graphScale;

                // Scroll wheel adjusts zoom
                _graphScale = Mathf.Clamp(_graphScale + Event.current.delta.y / 100f, 0.05f, 1f);

                // Move the window so that zooming occurs on the mouse target
                _scrollPosition -= mousePosition / oldScale - mousePosition / _graphScale;

                // Update visible regions
                AddWindowRectToScrollArea();
                Repaint();
            }

            if (Event.current.type == EventType.MouseDown)
            {
                bool shouldCancelScroll = Event.current.button == 0 && _shouldScroll;
                bool shouldCancelDrag = false;//Event.current.button == 0 && _draggable != null;
                if (shouldCancelScroll)
                {
                    _scrollPosition = _scrollPositionOnMouseDown;
                    _shouldScroll = false;
                    Repaint();
                }
                if (shouldCancelDrag)
                {
                    _draggable.Offset = Vector2.zero;
                    _draggable = null;
                    Repaint();
                }
                else if (Event.current.button == 1)
                {
                    _rightClickIsContextMenu = true;
                }
            }

            if (_draggable != null || _shouldScroll)
            {
                EditorGUIUtility.AddCursorRect(new Rect(Vector2.zero, position.size), MouseCursor.MoveArrow);
            }

            // We need to do some black magic here to work around Unity,
            // otherwise the scaling will apply to the clipping area!
            // http://martinecker.com/martincodes/unity-editor-window-zooming/
            Matrix4x4 previousGuiMatrix = Matrix4x4.identity;
            bool isRescaling = !Mathf.Approximately(_graphScale, 1f);
            if (isRescaling)
            {
                GUI.EndGroup();
                GUI.BeginGroup(new Rect(new Vector2(0f, 0f), position.size / _graphScale));

                previousGuiMatrix = GUI.matrix;
                GUI.matrix = Matrix4x4.TRS(new Vector2(0f, GolemEditorUtility.editorWindowTabHeight), Quaternion.identity, Vector3.one * _graphScale);
            }

            //-------------------------------------------------
            // Draw the background grid texture
            //-------------------------------------------------
            {
                var rect = new Rect(Vector2.zero, this.position.size / _graphScale);
                Texture2D gridTexture = GolemEditorUtility.GridTexture;
                float aligningOffset = rect.height - (int)(1 + rect.height / gridTexture.height) * gridTexture.height;
                Vector2 tileOffset = new Vector2(-_anchor.x / gridTexture.width, (_anchor.y - aligningOffset) / gridTexture.height);
                Vector2 tileAmount = new Vector2(rect.width / gridTexture.width, rect.height / gridTexture.height);
                GUI.DrawTextureWithTexCoords(rect, gridTexture, new Rect(tileOffset, tileAmount), false);
            }

            // Compute and store this at the window level because entering a layout context changes the value of mousePosition!
            var mouseGraphPosition = WindowToGraphPosition(Event.current.mousePosition);

            if (_selection != null)
            {
                // The key labeled "Delete" on OSX is actually backspace
                bool pressedDelete =
                #if UNITY_EDITOR_OSX
                    Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Backspace;
                #else
                    Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Delete;
                #endif

                if (pressedDelete)
                {
                    var editorWireSelection = _selection as EditorWire;
                    if (editorWireSelection != null)
                    {
                        DeleteEditorWire(editorWireSelection);
                    }

                    var editorTransitionSelection = _selection as EditorTransition;
                    if (editorTransitionSelection != null)
                    {
                        DeleteTransition(editorTransitionSelection.Index);
                    }

                    var editorStateSelection = _selection as EditorState;
                    if (editorStateSelection != null)
                    {
                        DeleteEditorState(editorStateSelection);
                    }

                    Repaint();
                    _shouldWrite = true;
                    _selection = null;
                }
            }

            if (Event.current.type == EventType.MouseDown)
            {
                _scrollPositionOnMouseDown = _scrollPosition;
                mouseGraphPosition = WindowToGraphPosition(Event.current.mousePosition);
                _mouseDownPosition = mouseGraphPosition;

                if (Event.current.button == 0)
                {
                    var pickedEditorTransitionExpression = this.pickEditorTransitionExpression(mouseGraphPosition);

                    if (pickedEditorTransitionExpression != null)
                    {
                        _draggable = pickedEditorTransitionExpression.Transition.DragExpressionAnchor();
                        Repaint();
                        _selection = null;
                    }
                }
                else if (Event.current.button == 1)
                {
                    _shouldScroll = true;

                }
            }

            if (Event.current.type == EventType.MouseUp)
            {
                _shouldScroll = false;
                if (_draggable != null)
                {
                    _shouldWrite = true;
                    _draggable = null;
                }
                AddWindowRectToScrollArea();
            }

            if (_shouldScroll)
            {
                if (Event.current.type == EventType.MouseDrag && Event.current.button == 1)
                {
                    _rightClickIsContextMenu = false;
                    _scrollPosition = _scrollPositionOnMouseDown + (mouseGraphPosition - _mouseDownPosition);
                    Repaint();
                }
            }
            if (_draggable != null)
            {
                if (Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseMove)
                {
                    _rightClickIsContextMenu = false;
                    _draggable.Offset = mouseGraphPosition - _mouseDownPosition;
                    Repaint();
                }
            }

            DrawCreateWire(_anchor);

            //-------------------------------------------------
            // Draw the circuit to the graph
            //-------------------------------------------------

            EditorGUI.BeginChangeCheck();

            foreach (EditorCell editorCell in _cells)
            {
                if (editorCell.Cell == null)
                {
                    Read();
                    return;
                }

                var inspectableCellType = GetInspectableCellType(editorCell.Cell.GetType());
                bool selected = object.ReferenceEquals(_selection, editorCell);

                Rect cellPosition = editorCell.Position.MovedBy(_anchor);
                if (Event.current.type == EventType.Repaint)
                {
                    GolemEditorSkin.Current.CellStyle.Draw(cellPosition, new GUIContent(editorCell.Name), false, false, false, selected);
                }

                Rect clientPosition = GolemEditorSkin.Current.CellStyle.padding.Remove(cellPosition);
                if (Event.current.type == EventType.Repaint)
                {
                    GolemEditorSkin.Current.CellBodyStyle.Draw(clientPosition, false, false, false, false);
                }

                //-------------------------------
                // Cell Inputs
                //-------------------------------
                Rect clientInputsRect;
                {
                    var inputs = inspectableCellType.Inputs;
                    clientInputsRect = new Rect(
                        clientPosition.x,
                        clientPosition.y + EditorGUIUtility.singleLineHeight * 0.5f,
                        clientPosition.width,
                        inputs.Length * EditorGUIUtility.singleLineHeight
                        );
                    for (int i = 0; i < inputs.Length; ++i)
                    {
                        Rect labelPosition = clientInputsRect.GetRow(i, EditorGUIUtility.singleLineHeight);

                        Rect portPosition = labelPosition.GetCenteredLeft(-EditorGUIUtility.singleLineHeight, GolemEditorSkin.Current.PortStyle.fixedWidth, GolemEditorSkin.Current.PortStyle.fixedHeight);
                        int id = GUIUtility.GetControlID(FocusType.Passive);
                        bool focused = GUIUtility.hotControl == id || GUI.GetNameOfFocusedControl() == id.ToString();
                        bool on = editorCell.HasInputWire(inputs[i].Name);

                        if (_createWire.Disabled || _createWire.StartIsOutput)
                        {
                            EditorGUIUtility.AddCursorRect(portPosition, MouseCursor.Link);
                        }

                        switch (Event.current.type)
                        {
                            case EventType.MouseDown:
                                if (labelPosition.ContainsInLeftHalf(Event.current.mousePosition))
                                {
                                    Event.current.Use();
                                    Repaint();
                                    GUI.FocusControl(id.ToString());
                                }
                                if (portPosition.Contains(Event.current.mousePosition))
                                {
                                    GUIUtility.hotControl = id;
                                    GUI.FocusControl(id.ToString());
                                    Event.current.Use();
                                    _shouldScroll = false;

                                    _createWire = CreateWire.Input(editorCell, inputs[i].Property, portPosition.center);

                                    Repaint();
                                }
                                break;

                            case EventType.MouseMove:
                            case EventType.MouseDrag:
                                if (_createWire.Enabled && portPosition.Contains(Event.current.mousePosition))
                                {
                                    _createWire.HoverEndInput(editorCell, inputs[i].Property, portPosition.center);
                                    GUIUtility.hotControl = id;
                                }
                                break;

                            case EventType.Repaint:
                                GUI.SetNextControlName(id.ToString());
                                GolemEditorSkin.Current.InputLabelStyle.Draw(labelPosition, new GUIContent(inputs[i].Name, inputs[i].Type.Name), false, false, on, focused);
                                GolemEditorSkin.Current.PortStyle.Draw(portPosition, false, false, on, focused);
                                GolemEditorUtility.SetWireWritePoints(inputs[i].Name, editorCell.InputWires, portPosition.center);
                                break;
                        }
                    }
                }

                //-------------------------------
                // Cell Outputs
                //-------------------------------
                Rect clientOutputsRect;
                {
                    var outputs = inspectableCellType.Outputs;
                    clientOutputsRect = new Rect(
                        clientPosition.x,
                        clientPosition.y + EditorGUIUtility.singleLineHeight * 0.5f,
                        clientPosition.width,
                        outputs.Length * EditorGUIUtility.singleLineHeight
                        );
                    for (int i = 0; i < outputs.Length; ++i)
                    {
                        Rect labelPosition = clientOutputsRect.GetRow(i, EditorGUIUtility.singleLineHeight);
                        Rect portPosition = labelPosition.GetCenteredRight(EditorGUIUtility.singleLineHeight, GolemEditorSkin.Current.PortStyle.fixedWidth, GolemEditorSkin.Current.PortStyle.fixedHeight);
                        int id = GUIUtility.GetControlID(FocusType.Passive);
                        bool focused = GUIUtility.hotControl == id || GUI.GetNameOfFocusedControl() == id.ToString();
                        bool on = editorCell.HasOutputWire(outputs[i].Name);

                        if (_createWire.Disabled || _createWire.StartIsInput)
                        {
                            EditorGUIUtility.AddCursorRect(portPosition, MouseCursor.Link);
                        }

                        switch (Event.current.type)
                        {
                            case EventType.MouseDown:
                                if (labelPosition.ContainsInRightHalf(Event.current.mousePosition))
                                {
                                    Event.current.Use();
                                    Repaint();
                                    GUI.FocusControl(id.ToString());
                                }
                                if (portPosition.Contains(Event.current.mousePosition))
                                {
                                    GUIUtility.hotControl = id;
                                    GUI.FocusControl(id.ToString());
                                    Event.current.Use();
                                    _shouldScroll = false;

                                    _createWire = CreateWire.Output(editorCell, outputs[i].Property, portPosition.center);

                                    Repaint();
                                }
                                break;

                            case EventType.MouseMove:
                            case EventType.MouseDrag:
                                if (_createWire.Enabled && portPosition.Contains(Event.current.mousePosition))
                                {
                                    _createWire.HoverEndOutput(editorCell, outputs[i].Property, portPosition.center);
                                    GUIUtility.hotControl = id;
                                }
                                break;

                            case EventType.MouseUp:
                                break;

                            case EventType.Repaint:
                                GUI.SetNextControlName(id.ToString());
                                GolemEditorSkin.Current.OutputLabelStyle.Draw(labelPosition, new GUIContent(outputs[i].Name, outputs[i].Type.Name), false, false, on, focused);
                                GolemEditorSkin.Current.PortStyle.Draw(portPosition, false, false, on, focused);
                                GolemEditorUtility.SetWireReadPoints(outputs[i].Name, editorCell.OutputWires, portPosition.center);
                                break;
                        }
                    }
                }

                //-------------------------------
                // Cell Fields
                //-------------------------------
                {
                    Rect clientFieldsRect = GolemEditorSkin.Current.CellBodyStyle.padding.Remove(Rect.MinMaxRect(
                        clientPosition.xMin,
                        Mathf.Max(clientInputsRect.yMax, clientOutputsRect.yMax),
                        clientPosition.xMax,
                        clientPosition.yMax
                        ));
                    GUILayout.BeginArea(clientFieldsRect);
                    GUILayout.BeginVertical();

                    for (int i = 0; i < inspectableCellType.Fields.Length; ++i)
                    {
                        var fieldInfo = inspectableCellType.Fields[i];
                        GolemEditorUtility.EditorGUILayoutGolemField(
                            fieldInfo.Type,
                            fieldInfo.SpecificType,
                            fieldInfo.FieldInfo,
                            editorCell.Cell,
                            editorCell.FieldsUsingSettings,
                            editorCell.FieldsUsingVariables,
                            _golem
                            );
                    }

                    GUILayout.EndVertical();
                    if (Event.current.type == EventType.Repaint)
                    {
                        clientPosition.yMax = clientFieldsRect.yMin + GolemEditorSkin.Current.CellBodyStyle.padding.Add(GUILayoutUtility.GetLastRect()).height;
                        editorCell.Position.size = GolemEditorSkin.Current.CellStyle.padding.Add(clientPosition).size;
                    }
                    GUILayout.EndArea();
                }

                bool isMouseInside = Event.current.isMouse && GolemEditorSkin.Current.CellStyle.overflow.Add(editorCell.Position.MovedBy(_anchor)).Contains(Event.current.mousePosition);
                if (isMouseInside)
                {
                    switch (Event.current.type)
                    {
                        case EventType.MouseDown:
                            _shouldScroll = false;
                            _selection = editorCell;
                            GUI.FocusControl(null);
                            GUIUtility.hotControl = 0;
                            Repaint();
                            if (Event.current.button == 0)
                            {
                                _draggable = editorCell.DragPosition();
                            }
                            break;

                        case EventType.MouseUp:
                            if (Event.current.button == 1 && _rightClickIsContextMenu)
                            {
                                GenericMenu menu = new GenericMenu();
                                menu.AddItem(new GUIContent("Delete Cell"), false, DeleteEditorCellMenuFunction, editorCell);
                                #warning TODO: better dropdown that works when scaling
                                menu.DropDown(new Rect(Event.current.mousePosition, Vector2.zero));
                            }
                            break;
                    }
                    Event.current.Use();
                }

            }

            _shouldWrite = _shouldWrite || EditorGUI.EndChangeCheck();


            // Draw transition we are creating
            //-------------------------------------------------
            DrawCreateTransition(_anchor);

            //-------------------------------------------------
            // Draw transitions
            //-------------------------------------------------
            {
                EditorTransition pickedTransition = null;
                foreach (EditorTransition editorTransition in _transitions)
                {
                    var fromState = _states[(int)editorTransition.From];
                    var toState = _states[(int)editorTransition.To];
                    bool selected = object.ReferenceEquals(_selection, editorTransition);
                    bool hasReverse = editorTransitionHasReverse(editorTransition);

                    if (Event.current.type == EventType.MouseDown)
                    {
                        #warning this code is duplicated with DrawTransition
                        const float toleranceSquared = 10f * 10f;
                        Vector2 from, to;
                        getEditorTransitionPoints(editorTransition, out from, out to);
                        if (Vector2Ext.DistanceToLineSegmentSquared(Event.current.mousePosition - _anchor, from, to) < toleranceSquared)
                        {
                            pickedTransition = editorTransition;
                        }
                    }

                    // Draw the transition
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
                            _anchor
                            );

                        // Draw the triangle
                        var middle = _anchor + (from + to) * 0.5f;
                        var parallel = (to - from).normalized;
                        var bumpVector3 = Vector3.Cross(Vector3.forward, parallel);
                        const float arrowHalfWidth = 7f;
                        const float arrowForward = 7f;
                        const float arrowBackward = 6f;
                        var bump = new Vector2(bumpVector3.x * arrowHalfWidth, bumpVector3.y * arrowHalfWidth);
                        var oldColor = Handles.color;
                        Handles.color = selected ? GolemEditorUtility.SelectedColor : GolemEditorUtility.WireColor;
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
                            _anchor
                            );

                        GolemEditorUtility.UpdateExpressionPositions(editorTransition, flipTreeSide);
                        GolemEditorUtility.DrawTransitionExpression(_anchor, editorTransition.Expression, flipTreeSide);

                        Handles.color = oldColor;
                    }
                }
                if (pickedTransition != null)
                {
                    if (Event.current.button == 1)
                    {
                        _rightClickIsContextMenu = false;
                        DeleteTransition(pickedTransition);
                    }
                    else
                    {
                        _selection = pickedTransition;
                    }
                    GUI.FocusControl("");
                    GUIUtility.hotControl = 0;
                    Repaint();
                    Event.current.Use();
                }
            }

            //-------------------------------------------------
            // Draw states to the graph
            //-------------------------------------------------

            EditorGUI.BeginChangeCheck();

            foreach (EditorState editorState in _states)
            {
                bool selected = object.ReferenceEquals(_selection, editorState);
                bool on = Application.isPlaying && _golem.Components[_golem.Archetype.EditorWindowSelectedComponent].LayerStates[(int)editorState.Layer] == editorState.CompiledIndex;

                Rect cellPosition = editorState.Position.MovedBy(_anchor);
                if (Event.current.type == EventType.Repaint)
                {
                    GolemEditorSkin.Current.CellStyle.Draw(cellPosition, new GUIContent(editorState.Name), false, false, on, selected);
                }

                Rect clientPosition = GolemEditorSkin.Current.CellStyle.padding.Remove(cellPosition);
                if (Event.current.type == EventType.Repaint)
                {
                    GolemEditorSkin.Current.CellBodyStyle.Draw(clientPosition, false, false, false, false);
                }

                Rect clientRect = GolemEditorSkin.Current.CellBodyStyle.padding.Remove(clientPosition);
                GUILayout.BeginArea(clientRect);
                GUI.EndClip();
                GUI.BeginClip(clientRect.ExpandedBy(0, EditorGUIUtility.singleLineHeight + GolemEditorSkin.Current.PortStyle.fixedWidth, 0f, 0f));
                GUILayout.BeginVertical();

                //-------------------------------
                // State Scripts
                //-------------------------------
                var editorScripts = editorState.Scripts;
                for (int i = 0; i < editorScripts.Count; ++i)
                {
                    var editorScript = editorScripts[i];
                    var inspectableType = GetInspectableScriptType(editorScript.Script.GetType());

                    EditorGUILayout.LabelField(inspectableType.Name, EditorStyles.boldLabel);

                    //-------------------------------
                    // Script Fields
                    //-------------------------------
                    var fields = inspectableType.Members;
                    for (int j = 0; j < fields.Length; ++j)
                    {
                        var field = fields[j];

                        if (fields[j].IsVariable)
                        {
                            Debug.Assert(fields[j].FieldInfo == null);
                            Debug.Assert(fields[j].PropertyInfo != null);

                            int id = GUIUtility.GetControlID(FocusType.Passive);
                            GUI.SetNextControlName(id.ToString());
                            Rect labelPosition = EditorGUILayout.GetControlRect();
                            bool fieldFocused = GUIUtility.hotControl == id || GUI.GetNameOfFocusedControl() == id.ToString();
                            bool fieldOn = editorScript.HasOutputWire(field.FieldInfo.Name);
                            GolemEditorUtility.EditorGUIGolemField(
                                    labelPosition,
                                    field.Type,
                                    field.SpecificType,
                                    field.FieldInfo,
                                    editorScript.Script,
                                    editorScript.FieldsUsingSettings,
                                    editorScript.FieldsUsingVariables,
                                    _golem,
                                    fieldOn
                                    );

                            Rect portPosition = labelPosition.GetCenteredRight(EditorGUIUtility.singleLineHeight + GolemEditorSkin.Current.CellBodyStyle.padding.right, GolemEditorSkin.Current.PortStyle.fixedWidth, GolemEditorSkin.Current.PortStyle.fixedHeight);

                            if (_createWire.Disabled || _createWire.StartIsInput)
                            {
                                EditorGUIUtility.AddCursorRect(portPosition, MouseCursor.Link);
                            }

                            // Calculate the center of the port when the screen is being scaled
                            Vector2 portCenter = portPosition.center - (1 - _graphScale) / _graphScale * clientRect.position;

                            switch (Event.current.type)
                            {
                                case EventType.MouseDown:
                                    if (labelPosition.Contains(Event.current.mousePosition))
                                    {
                                        Event.current.Use();
                                        Repaint();
                                        GUI.FocusControl(id.ToString());
                                    }
                                    if (portPosition.Contains(Event.current.mousePosition))
                                    {
                                        GUIUtility.hotControl = id;
                                        GUI.FocusControl(id.ToString());
                                        Event.current.Use();
                                        _shouldScroll = false;
                                        _createWire = CreateWire.Output(editorScript, field.PropertyInfo, portCenter);
                                        Repaint();
                                    }
                                    break;

                                case EventType.MouseMove:
                                case EventType.MouseDrag:
                                    if (_createWire.Enabled && portPosition.Contains(Event.current.mousePosition))
                                    {
                                        _createWire.HoverEndOutput(editorScript, field.PropertyInfo, portCenter);
                                        GUIUtility.hotControl = id;
                                    }
                                    break;

                                case EventType.MouseUp:
                                    break;

                                case EventType.Repaint:
                                    GolemEditorSkin.Current.PortStyle.Draw(portPosition, false, false, fieldOn, fieldFocused);
                                    GolemEditorUtility.SetWireReadPoints(field.PropertyInfo.Name, editorScript.OutputWires, portCenter);
                                    break;
                            }
                        }
                        else
                        {
                            GolemEditorUtility.EditorGUILayoutGolemField(
                                    field.Type,
                                    field.SpecificType,
                                    field.FieldInfo,
                                    editorScript.Script,
                                    editorScript.FieldsUsingSettings,
                                    editorScript.FieldsUsingVariables,
                                    _golem
                                    );
                        }
                    }
                }

                GUILayout.EndVertical();
                if (Event.current.type == EventType.Repaint)
                {
                    clientPosition.yMax = clientRect.yMin + GolemEditorSkin.Current.CellBodyStyle.padding.Add(GUILayoutUtility.GetLastRect()).height;
                    editorState.Position.size = GolemEditorSkin.Current.CellStyle.padding.Add(clientPosition).size;
                }

                GUILayout.EndArea();

                bool mouseIsInside = GolemEditorSkin.Current.CellStyle.overflow.Add(editorState.Position.MovedBy(_anchor)).Contains(Event.current.mousePosition);
                if (mouseIsInside)
                {
                    switch (Event.current.type)
                    {
                        case EventType.MouseDown:

                            Event.current.Use();
                            Repaint();

                            _shouldScroll = false;
                            _selection = editorState;
                            GUI.FocusControl(null);
                            GUIUtility.hotControl = 0;

                            if (Event.current.button == 0)
                            {
                                _draggable = editorState.DragPosition();
                            }
                            else
                            {
                                _createTransition = CreateTransition.Create(editorState, editorState.Position.MovedBy(_anchor).center);
                            }

                            break;

                        
                        case EventType.MouseUp:
                            if (Event.current.button == 1 && _rightClickIsContextMenu)
                            {
                                GenericMenu menu = new GenericMenu();
                                var scriptTypes = Assembly.GetAssembly(typeof(Script))
                                    .GetTypes()
                                    .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Script)))
                                    .ToList();
                                scriptTypes.Sort((a, b) => a.Name.CompareTo(b.Name));
                                {
                                    var removeable = scriptTypes.Where((type) => editorState.Scripts.Any((editorScript) => editorScript.Script.GetType().Equals(type))).ToList();
                                    var addable = scriptTypes.Except(removeable);
                                    if (addable.Any((type) => true))
                                    {
                                        foreach (var type in addable)
                                        {
                                            menu.AddItem(new GUIContent("Add/" + type.Name), false, AddScriptMenuFunction, new object[] { editorState, type });
                                        }
                                    }
                                    else
                                    {
                                        menu.AddDisabledItem(new GUIContent("Add/(empty)"));
                                    }
                                    if (removeable.Any((type) => true))
                                    {
                                        foreach (var type in removeable)
                                        {
                                            menu.AddItem(new GUIContent("Remove/" + type.Name), false, RemoveScriptMenuFunction, new object[] { editorState, type });
                                        }
                                    }
                                    else
                                    {
                                        menu.AddDisabledItem(new GUIContent("Remove/(empty)"));
                                    }
                                    menu.AddSeparator("");
                                    menu.AddItem(new GUIContent("Delete State"), false, DeleteEditorStateMenuFunction, editorState);
                                }
                                menu.ShowAsContext();
                            }
                            Event.current.Use();
                            break;
                        
                        case EventType.MouseMove:
                        case EventType.MouseDrag:
                            if (_createTransition.Enabled)
                            {
                                _createTransition.HoverEnd(editorState, editorState.Position.MovedBy(_anchor).center);
                            }
                            break;
                    }
                }
            }


            _shouldWrite = _shouldWrite || EditorGUI.EndChangeCheck();

            //-------------------------------------------------
            // Draw variable input registers
            //-------------------------------------------------
            EditorGUI.BeginChangeCheck();
            foreach (EditorVariableInputRegister variableInputRegister in _variableInputRegisters)
            {
                bool selected = object.ReferenceEquals(_selection, variableInputRegister);

                Rect cellPosition = variableInputRegister.Position.MovedBy(_anchor);
                if (Event.current.type == EventType.Repaint)
                {
                    GolemEditorSkin.Current.CellStyle.Draw(cellPosition, new GUIContent("Read Variable"), false, false, false, selected);
                }

                Rect clientPosition = GolemEditorSkin.Current.CellStyle.padding.Remove(cellPosition);
                if (Event.current.type == EventType.Repaint)
                {
                    GolemEditorSkin.Current.CellBodyStyle.Draw(clientPosition, false, false, false, false);
                }

                Rect clientOutputsRect = new Rect(
                    clientPosition.x,
                    clientPosition.y + EditorGUIUtility.singleLineHeight * 0.5f,
                    clientPosition.width,
                    EditorGUIUtility.singleLineHeight
                    );

                Rect labelPosition = clientOutputsRect;
                Rect portPosition = labelPosition.GetCenteredRight(EditorGUIUtility.singleLineHeight, GolemEditorSkin.Current.PortStyle.fixedWidth, GolemEditorSkin.Current.PortStyle.fixedHeight);
                int id = GUIUtility.GetControlID(FocusType.Passive);
                bool focused = GUIUtility.hotControl == id || GUI.GetNameOfFocusedControl() == id.ToString();
                bool on = variableInputRegister.HasOutputWire();

                if (_createWire.Disabled || _createWire.StartIsInput)
                {
                    EditorGUIUtility.AddCursorRect(portPosition, MouseCursor.Link);
                }

                #warning slow!!
                FieldInfo fieldInfo = variableInputRegister.GetType().GetField("Variable");

                GUI.SetNextControlName(id.ToString());
                GolemEditorUtility.EditorGUIGolemField(
                    labelPosition,
                    InspectableType.VariableRef,
                    typeof(VariableRef),
                    fieldInfo,
                    variableInputRegister,
                    null,
                    null,
                    _golem,
                    variableInputRegister.HasOutputWire()
                    );

                switch (Event.current.type)
                {
                    case EventType.MouseDown:
                        if (portPosition.Contains(Event.current.mousePosition))
                        {
                            GUIUtility.hotControl = id;
                            GUI.FocusControl(id.ToString());
                            Event.current.Use();
                            _shouldScroll = false;

                            _createWire = CreateWire.Output(variableInputRegister, null, portPosition.center);

                            Repaint();
                        }
                        if (labelPosition.Contains(Event.current.mousePosition))
                        {
                            Event.current.Use();
                            Repaint();
                            GUI.FocusControl(id.ToString());
                        }
                        break;

                    case EventType.MouseMove:
                    case EventType.MouseDrag:
                        if (_createWire.Enabled && portPosition.Contains(Event.current.mousePosition))
                        {
                            _createWire.HoverEndOutput(variableInputRegister, null, portPosition.center);
                            GUIUtility.hotControl = id;
                        }
                        break;

                    case EventType.MouseUp:
                        break;

                    case EventType.Repaint:
                        GolemEditorSkin.Current.PortStyle.Draw(portPosition, false, false, on, focused);
                        GolemEditorUtility.SetWireReadPoints(variableInputRegister.OutputWires, portPosition.center);
                        break;
                }

                if (Event.current.type == EventType.Repaint)
                {
                    clientPosition.yMax = clientOutputsRect.yMin + GolemEditorSkin.Current.CellBodyStyle.padding.Add(clientOutputsRect).height;
                    variableInputRegister.Position.size = GolemEditorSkin.Current.CellStyle.padding.Add(clientPosition).size;
                }

                bool mouseIsInside = Event.current.isMouse && GolemEditorSkin.Current.CellStyle.overflow.Add(variableInputRegister.Position.MovedBy(_anchor)).Contains(Event.current.mousePosition);
                if (mouseIsInside)
                {
                    switch (Event.current.type)
                    {
                        case EventType.MouseDown:
                            _selection = variableInputRegister;
                            GUI.FocusControl(null);
                            GUIUtility.hotControl = 0;
                            _shouldScroll = false;
                            Repaint();
                            if (Event.current.button == 0)
                            {
                                _draggable = variableInputRegister.DragPosition();
                            }
                            break;

                        case EventType.MouseUp:
                            if (Event.current.button == 1 && _rightClickIsContextMenu)
                            {
                                GenericMenu menu = new GenericMenu();
                                menu.AddItem(new GUIContent("Delete"), false, DeleteVariableInputRegister, variableInputRegister);
                                menu.ShowAsContext();
                            }
                            break;
                    }
                    Event.current.Use();
                }
            }

            _shouldWrite = _shouldWrite || EditorGUI.EndChangeCheck();

            //-------------------------------------------------
            // Draw wires in the graph
            //-------------------------------------------------
            switch (Event.current.type)
            {
                case EventType.Repaint:
                    foreach (EditorWire editorWire in _wires)
                    {
                        bool selected = object.ReferenceEquals(_selection, editorWire);
                        GolemEditorUtility.DrawEdge(
                            new ControlPoint { Point = EditorGUIUtility.ScreenToGUIPoint(editorWire.ReadPoint) },
                            new ControlPoint { Point = EditorGUIUtility.ScreenToGUIPoint(editorWire.WritePoint) },
                            null,
                            selected,
                            Vector2.zero
                            );
                    }
                    break;

                case EventType.MouseDown:
                    EditorWire pickedWire = null;
                    foreach (EditorWire editorWire in _wires)
                    {
                        const float toleranceSquared = 10f * 10f;
                        if (Vector2Ext.DistanceToLineSegmentSquared(Event.current.mousePosition, EditorGUIUtility.ScreenToGUIPoint(editorWire.ReadPoint), EditorGUIUtility.ScreenToGUIPoint(editorWire.WritePoint)) < toleranceSquared)
                        {
                            pickedWire = editorWire;
                        }
                    }
                    if (pickedWire != null)
                    {
                        if (Event.current.button == 1)
                        {
                            _rightClickIsContextMenu = false;
                            DeleteEditorWire(pickedWire);
                        }
                        else
                        {
                            _selection = pickedWire;
                        }
                        GUI.FocusControl("");
                        GUIUtility.hotControl = 0;
                        Repaint();
                        Event.current.Use();
                    }
                    break;
            }

            //-------------------------------------------------
            // Bring back Unity's window rendering context
            //-------------------------------------------------
            if (isRescaling)
            {
                GUI.matrix = previousGuiMatrix;
                GUI.EndGroup();
                GUI.BeginGroup(new Rect(0f, GolemEditorUtility.editorWindowTabHeight, Screen.width, Screen.height));
            }

            //-------------------------------------------------
            // Context menu
            // Do this outside of the rescaled context
            // otherwise positioning doesn't work correctly.
            //-------------------------------------------------
            if (Event.current.type == EventType.MouseUp && Event.current.button == 1 && _rightClickIsContextMenu)
            {
                Event.current.Use();
                var menu = new GenericMenu();

                _scrollPositionOnMouseDown = _scrollPosition;
                _menuFunctionPosition = WindowToGraphPosition(Event.current.mousePosition);
                _mouseDownPosition = mouseGraphPosition;
                _shouldScroll = false;

                var pickedEditorTransitionExpression = pickEditorTransitionExpression(mouseGraphPosition);
                if (pickedEditorTransitionExpression == null)
                {
                    menu.AddItem(new GUIContent("New State/Normal"), false, CreateStateMenuFunction, EditorSpecialStateType.Normal);
                    menu.AddSeparator("New State");
                    menu.AddItem(new GUIContent("New State/Enter"), false, CreateStateMenuFunction, EditorSpecialStateType.LayerEnter);
                    menu.AddItem(new GUIContent("New State/Exit"), false, CreateStateMenuFunction, EditorSpecialStateType.LayerExit);
                    menu.AddItem(new GUIContent("New State/Any"), false, CreateStateMenuFunction, EditorSpecialStateType.LayerAny);
                    var cellTypes = Assembly.GetAssembly(typeof(Cell))
                        .GetTypes()
                        .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Cell)))
                        .ToList();
                    cellTypes.Sort((a, b) => a.Name.CompareTo(b.Name));
                    if (cellTypes.Count == 0)
                    {
                        menu.AddDisabledItem(new GUIContent("New Cell/(empty)"));
                    }
                    foreach (var type in cellTypes)
                    {
                        menu.AddItem(new GUIContent("New Cell/" + type.Name), false, CreateCellMenuFunction, type);
                    }
                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent("New Variable Input"), false, CreateVariableInputMenuFunction, null);
                }
                else if (pickedEditorTransitionExpression != null)
                {
                    var currentType = pickedEditorTransitionExpression.Expression.Type;

                    if (HasSubexpressionsAttribute.IsFoundOn(currentType))
                    {
                        menu.AddItem(new GUIContent("Add Child"), false, AddSubexpressionToTransitionExpression, pickedEditorTransitionExpression);
                    }
                    else
                    {
                        menu.AddDisabledItem(new GUIContent("Add Child"));
                    }
                    if (pickedEditorTransitionExpression.Parent == null)
                    {
                        menu.AddDisabledItem(new GUIContent("Delete"));
                    }
                    else
                    {
                        menu.AddItem(new GUIContent("Delete"), false, DeleteTransitionExpression, pickedEditorTransitionExpression);
                    }
                    menu.AddSeparator("");
                    var triggers = Enum.GetValues(typeof(Trigger));
                    menu.AddItem(new GUIContent("And &&&&"), currentType == EditorTransitionExpressionType.And, SetTransitionExpressionType, new object[] { pickedEditorTransitionExpression, EditorTransitionExpressionType.And });
                    menu.AddItem(new GUIContent("Or ||"), currentType == EditorTransitionExpressionType.Or, SetTransitionExpressionType, new object[] { pickedEditorTransitionExpression, EditorTransitionExpressionType.Or });
                    menu.AddItem(new GUIContent("True"), currentType == EditorTransitionExpressionType.True, SetTransitionExpressionType, new object[] { pickedEditorTransitionExpression, EditorTransitionExpressionType.True });
                    menu.AddItem(new GUIContent("False"), currentType == EditorTransitionExpressionType.False, SetTransitionExpressionType, new object[] { pickedEditorTransitionExpression, EditorTransitionExpressionType.False });
                    menu.AddSeparator("");
                    for (int i = 0; i < triggers.Length; ++i)
                    {
                        var value = (Trigger)triggers.GetValue(i);
                        if ((int)value >= (int)Trigger.__COUNT__)
                        {
                            continue;
                        }
                        menu.AddItem(new GUIContent(value.ToString().Replace('/', '_')), currentType == EditorTransitionExpressionType.Trigger && value == pickedEditorTransitionExpression.Expression.Trigger, SetTransitionExpressionType, new object[] { pickedEditorTransitionExpression, EditorTransitionExpressionType.Trigger, value });
                    }
                }
                menu.ShowAsContext();
            }

            //-------------------------------------------------
            // Status bar
            //-------------------------------------------------
            const float kStatusBarHeight = 17;
            var height = this.position.height;
            var width = this.position.width;
            GUILayout.BeginArea(new Rect(0, height - kStatusBarHeight, width, kStatusBarHeight));
            if (Application.isPlaying)
            {
                EditorWire wire = _selection as EditorWire;
                if (wire != null)
                {
                    GolemComponentRuntimeData runtimeComponent = _golem.Components[_golem.Archetype.EditorWindowSelectedComponent];
                    GUILayout.Label(runtimeComponent.Registers[(int)wire.Register].ToString());
                }
                Repaint(); // constant repaint
            }
            else
            {
                GUILayout.Label("_scrollPosition: " + _scrollPosition.ToString() + " - _scrollAnchor: " + _scrollAnchor.ToString() + " - _scrollSize: " + _scrollSize.ToString() + " - mouseDown: " + _mouseDownPosition.ToString() + " - graphPos: " + mouseGraphPosition.ToString());
            }
            GUILayout.EndArea();

            switch (Event.current.type)
            {

                case EventType.KeyDown:
                    switch (Event.current.keyCode)
                    {
                        case KeyCode.LeftArrow:
                        case KeyCode.A:
                            _scrollPosition += Vector2.right * 100f;
                            AddWindowRectToScrollArea();
                            Repaint();
                            Event.current.Use();
                            break;

                        case KeyCode.RightArrow:
                        case KeyCode.D:
                            _scrollPosition += Vector2.left * 100f;
                            AddWindowRectToScrollArea();
                            Repaint();
                            Event.current.Use();
                            break;

                        case KeyCode.UpArrow:
                        case KeyCode.W:
                            _scrollPosition += Vector2.up * 100f;
                            AddWindowRectToScrollArea();
                            Repaint();
                            Event.current.Use();
                            break;

                        case KeyCode.DownArrow:
                        case KeyCode.S:
                            _scrollPosition += Vector2.down * 100f;
                            AddWindowRectToScrollArea();
                            Repaint();
                            Event.current.Use();
                            break;
                    }
                    break;
            }

            if (_shouldWrite)
            {
                GolemEditorUtility.SetDirty(_golem, _golem.Archetype.Components[_golem.Archetype.EditorWindowSelectedComponent]);
                Repaint();
                _shouldWrite = false;
            }
        }
        private void DeleteTransition(EditorTransition transitionToRemove)
        {
            DeleteTransition(transitionToRemove.Index);
        }

        private void DeleteTransition(EditorTransitionIndex indexToRemove)
        {
            var transitionBeingRemoved = _transitions[(int)indexToRemove];
            Debug.Assert(object.ReferenceEquals(_transitions[(int)transitionBeingRemoved.Index], transitionBeingRemoved));
            _states[(int)transitionBeingRemoved.From].TransitionsOut.Remove(transitionBeingRemoved.Index);
            _states[(int)transitionBeingRemoved.To].TransitionsIn.Remove(transitionBeingRemoved.Index);
            _transitions.RemoveAt((int)indexToRemove);

            for (int i = (int)indexToRemove; i < _transitions.Count; ++i)
            {
                var transition = _transitions[i];
                EditorTransitionIndex oldTransitionIndex = transition.Index;
                EditorTransitionIndex newTransitionIndex = (EditorTransitionIndex)i;
                Debug.Assert((int)transition.Index == i + 1);
                transition.Index = newTransitionIndex;

                // Update references in 'from'
                {
                    var indexList = _states[(int)transition.From].TransitionsOut;
                    for (int j = 0; j < indexList.Count; ++j)
                    {
                        if (indexList[j] == indexToRemove)
                        {
                            indexList[j] = EditorTransitionIndex.Invalid;
                        }
                        else if (indexList[j] == oldTransitionIndex)
                        {
                            indexList[j] = newTransitionIndex;
                        }
                    }
                }

                // Update references in 'to'
                {
                    var indexList = _states[(int)transition.To].TransitionsIn;
                    for (int j = 0; j < indexList.Count; ++j)
                    {
                        if (indexList[j] == indexToRemove)
                        {
                            indexList[j] = EditorTransitionIndex.Invalid;
                        }
                        else if (indexList[j] == oldTransitionIndex)
                        {
                            indexList[j] = newTransitionIndex;
                        }
                    }
                }
            }
        }

        private void removeTransitionsAt(List<EditorTransitionIndex> indicesToRemove)
        {
            for (int i = 0; i < indicesToRemove.Count; ++i)
            {
                DeleteTransition(indicesToRemove[i]);
            }
        }

        private Vector2 _menuFunctionPosition;
        private void CreateCellMenuFunction(object typeParam)
        {
            Type type = typeParam as Type;
            var editorCell = new EditorCell();
            editorCell.Name = type.Name;
            editorCell.Cell = Activator.CreateInstance(type) as Cell;
            editorCell.Position = GolemEditorUtility.SnapToGrid(new Rect(_menuFunctionPosition, new Vector2(EditorGUIUtility.labelWidth * 2 + EditorGUIUtility.fieldWidth, 100)));
            editorCell.Index = (EditorCellIndex)_cells.Count;
            _cells.Add(editorCell);
            _shouldWrite = true;
        }

        private void CreateVariableInputMenuFunction(object nullParam)
        {
            var editorVariableInputRegister = new EditorVariableInputRegister();
            editorVariableInputRegister.Position = GolemEditorUtility.SnapToGrid(new Rect(_menuFunctionPosition, new Vector2(EditorGUIUtility.labelWidth * 2 + EditorGUIUtility.fieldWidth, 50)));
            editorVariableInputRegister.Variable = new VariableRef(null, null);
            _variableInputRegisters.Add(editorVariableInputRegister);
            _shouldWrite = true;
        }

        private void DeleteEditorCellMenuFunction(object editorCellParam)
        {
            var editorCell = editorCellParam as EditorCell;
            foreach (var input in editorCell.GetAllInputWires())
            {
                input.ReadObject.RemoveOutputWire(input);
                _wires.Remove(input);
            }
            foreach (var output in editorCell.GetAllOutputWires())
            {
                output.WriteObject.RemoveInputWire(output);
                _wires.Remove(output);
            }
            _cells.Remove(editorCell);
            _shouldWrite = true;
        }

        private void SetTransitionExpressionType(object exprTypeTrigger)
        {
            var exprTypeTriggerArray = exprTypeTrigger as object[];
            var pickedEditorTransitionExpression = exprTypeTriggerArray[0] as PickedEditorTransitionExpression;
            var expression = pickedEditorTransitionExpression.Expression;
            var type = (EditorTransitionExpressionType)exprTypeTriggerArray[1];

            if (!HasSubexpressionsAttribute.IsFoundOn(type))
            {
                expression.Subexpressions.Clear();
            }
            expression.Type = type;

            if (type == EditorTransitionExpressionType.Trigger)
            {
                expression.Trigger = (Trigger)exprTypeTriggerArray[2];
            }
            else
            {
                expression.Trigger = Trigger.Invalid;
            }

            _shouldWrite = true;
        }

        private void AddSubexpressionToTransitionExpression(object pickedEditorTransitionExpressionParam)
        {
            var pickedEditorTransitionExpression = pickedEditorTransitionExpressionParam as PickedEditorTransitionExpression;
            Debug.Assert(HasSubexpressionsAttribute.IsFoundOn(pickedEditorTransitionExpression.Expression.Type));
            pickedEditorTransitionExpression.Expression.Subexpressions.Add(new EditorTransitionExpression()
            {
                Type = EditorTransitionExpressionType.True,
            });
            _shouldWrite = true;
        }

        private void DeleteTransitionExpression(object pickedEditorTransitionExpressionParam)
        {
            var pickedEditorTransitionExpression = pickedEditorTransitionExpressionParam as PickedEditorTransitionExpression;
            pickedEditorTransitionExpression.Parent.Subexpressions.RemoveAt(pickedEditorTransitionExpression.IndexInParent);
            _shouldWrite = true;
        }

        private void DeleteVariableInputRegister(object pickedEditorVariableInputRegisterParam)
        {
            EditorVariableInputRegister obj = pickedEditorVariableInputRegisterParam as EditorVariableInputRegister;
            for (int i = 0; i < obj.OutputWires.Count; ++i)
            {
                var wire = obj.OutputWires[i];
                wire.WriteCell.RemoveInputWire(wire);
                _wires.Remove(wire);
            }
            _variableInputRegisters.Remove(obj);
            _shouldWrite = true;
        }

        private void CreateStateMenuFunction(object param)
        {
            var type = (EditorSpecialStateType)param;
            var editorState = new EditorState();
            switch (type)
            {
                case EditorSpecialStateType.Normal: editorState.Name = "State"; break;
                case EditorSpecialStateType.LayerEnter: editorState.Name = "Enter"; break;
                case EditorSpecialStateType.LayerExit: editorState.Name = "Exit"; break;
                case EditorSpecialStateType.LayerAny: editorState.Name = "Any"; break;
            }
            editorState.SpecialState = type;
            editorState.Position = GolemEditorUtility.SnapToGrid(new Rect(_menuFunctionPosition, new Vector2(EditorGUIUtility.labelWidth * 2 + EditorGUIUtility.fieldWidth, 100)));
            editorState.Index = (EditorStateIndex)_states.Count;
            _states.Add(editorState);
            _shouldWrite = true;
            Repaint();
        }

        private void DeleteEditorWire(EditorWire editorWire)
        {
            editorWire.ReadObject.RemoveOutputWire(editorWire);
            editorWire.WriteObject.RemoveInputWire(editorWire);
            _wires.Remove(editorWire);
            if (object.ReferenceEquals(editorWire, _selection))
            {
                _selection = null;
            }
            Repaint();
        }

        private void DeleteEditorState(EditorState editorState)
        {
            EditorStateIndex indexToRemove = editorState.Index;
            Debug.Assert(editorState.Index == indexToRemove);
            removeTransitionsAt(editorState.TransitionsIn);
            removeTransitionsAt(editorState.TransitionsOut);
            foreach (EditorScript script in editorState.Scripts)
            {
                foreach (EditorWire output in script.GetAllOutputWires())
                {
                    output.WriteObject.RemoveInputWire(output);
                    _wires.Remove(output);
                }
            }
#if UNITY_EDITOR
            foreach (var transitionIndex in editorState.TransitionsIn)
            {
                Debug.Assert(transitionIndex == EditorTransitionIndex.Invalid);
            }
            foreach (var transitionIndex in editorState.TransitionsOut)
            {
                Debug.Assert(transitionIndex == EditorTransitionIndex.Invalid);
            }
#endif

            _states.RemoveAt((int)indexToRemove);

            if (object.ReferenceEquals(editorState, _selection))
            {
                _selection = null;
            }

            // If anything uses EditorStateIndex other than the transitions that are into or out of this state,
            // we need to update those references here.

            _shouldWrite = true;
            Repaint();
        }

        private void DeleteEditorStateMenuFunction(object editorStateParam)
        {
            DeleteEditorState(editorStateParam as EditorState);
        }

        private void AddScriptMenuFunction(object paramsArray)
        {
            var tuple = paramsArray as object[];
            var editorState = tuple[0] as EditorState;
            var type = tuple[1] as Type;
            var editorScript = new EditorScript();
            editorScript.Script = Activator.CreateInstance(type) as Script;
            if (editorScript.Script == null)
            {
                throw new InvalidProgramException("trying to add a script that is not a Script");
            }
            editorState.Scripts.Add(editorScript);
            _shouldWrite = true;
        }

        private void RemoveScriptMenuFunction(object paramsArray)
        {
            var tuple = paramsArray as object[];
            var editorState = tuple[0] as EditorState;
            var type = tuple[1] as Type;
            var scripts = editorState.Scripts;
            for (int i = 0; i < scripts.Count; ++i)
            {
                if (scripts[i].Script.GetType().Equals(type))
                {
                    foreach (var output in scripts[i].GetAllOutputWires())
                    {
                        output.WriteCell.RemoveInputWire(output);
                        _wires.Remove(output);
                    }
                    scripts.RemoveAt(i);
                    _shouldWrite = true;
                    return;
                }
            }
            Debug.LogError("trying to remove a script type that doesn't exist");
        }


        private void AddWindowRectToScrollArea()
        {
            Vector2 extraMin = Vector2.Max(Vector2.zero, _scrollPosition);
            Vector2 extraMax = Vector2.Max(Vector2.zero, this.position.size - (_scrollPosition + _scrollSize));
            _scrollSize += extraMax + extraMin;
            _scrollAnchor += extraMin;
            _scrollPosition -= extraMin;
            bool changedAnything = extraMin.x > 0 || extraMin.y > 0 || extraMax.x > 0 || extraMax.y > 0;
            if (changedAnything)
            {
                _scrollPositionOnMouseDown = _scrollPosition;
                Repaint();
            }
        }

        private Vector2 WindowToGraphPosition(Vector2 windowPosition)
        {
            return windowPosition - _scrollPositionOnMouseDown - _scrollAnchor;
        }

        private Rect GraphToWindowPosition(Rect rect)
        {
            // this is wrong but I don't know how to fix right now for drawing cursor rects, it seems like the cursor thingy
            // automatically compensates for the gui matrix in some way
            return rect;
        }


        internal struct CreateWire
        {
            public readonly object StartObject;
            public readonly PropertyInfo StartProperty;
            public readonly bool StartIsInput;
            public bool StartIsOutput { get { return !StartIsInput; } }
            public Vector2 StartPoint;

            public bool Enabled;
            public bool Disabled { get { return !Enabled; } }
            public bool HasGraphPosition;

            public bool HasEnd;
            public bool Valid;

            public object EndObject;
            public PropertyInfo EndProperty;
            public Vector2 EndPoint;

            private CreateWire(object startObject, PropertyInfo startProperty, Vector2 startPoint, bool input)
            {
                StartObject = startObject;
                StartProperty = startProperty;
                StartPoint = startPoint;
                StartIsInput = input;

                Enabled = true;
                HasGraphPosition = false;
                HasEnd = false;
                Valid = false;

                EndObject = null;
                EndProperty = null;
                EndPoint = Vector2.zero;
            }

            public static CreateWire Input(IGraphObjectWithInputs startObject, PropertyInfo startProperty, Vector2 startPoint)
            {
                #warning make sure that there isn't an input on this field already
                return new CreateWire(startObject, startProperty, GUIUtility.GUIToScreenPoint(startPoint), true);
            }

            public static CreateWire Output(IGraphObjectWithOutputs startObject, PropertyInfo startProperty, Vector2 startPoint)
            {
                return new CreateWire(startObject, startProperty, GUIUtility.GUIToScreenPoint(startPoint), false);
            }

            public void HoverEndInput(IGraphObjectWithInputs endObject, PropertyInfo endProperty, Vector2 endPoint)
            {
                HasEnd = true;
                Valid =
                    StartIsOutput
                && !object.ReferenceEquals(StartObject, endObject)
                #warning check types in a better way
                && (endProperty == null || StartProperty == null || object.Equals(endProperty.PropertyType.GetGenericArguments()[0], StartProperty.PropertyType.GetGenericArguments()[0]));
                #warning make sure that no wire already exists

                EndObject = endObject;
                EndProperty = endProperty;
                EndPoint = GUIUtility.GUIToScreenPoint(endPoint);
            }

            public void HoverEndOutput(IGraphObjectWithOutputs endObject, PropertyInfo endProperty, Vector2 endPoint)
            {
                HasEnd = true;
                Valid =
                    StartIsInput
                && !object.ReferenceEquals(StartObject, endObject)
                && (endProperty == null || StartProperty == null || object.Equals(endProperty.PropertyType.GetGenericArguments()[0], StartProperty.PropertyType.GetGenericArguments()[0]));
                #warning make sure that no wire already exists

                EndObject = endObject;
                EndProperty = endProperty;
                EndPoint = GUIUtility.GUIToScreenPoint(endPoint);
            }

            public EditorWire DropWire()
            {
                Enabled = false;
                if (!HasEnd || !Valid)
                {
                    return null;
                }

                EditorWire wire;
                if (StartIsInput)
                {
                    wire = new EditorWire()
                    {
                        ReadObject = EndObject as IGraphObjectWithOutputs,
                        ReadField = EndProperty == null ? null : EndProperty.Name,
                        WriteObject = StartObject as IGraphObjectWithInputs,
                        WriteField = StartProperty == null ? null : StartProperty.Name,
                    };
                }
                else
                {
                    wire = new EditorWire()
                    {
                        ReadObject = StartObject as IGraphObjectWithOutputs,
                        ReadField = StartProperty == null ? null : StartProperty.Name,
                        WriteObject = EndObject as IGraphObjectWithInputs,
                        WriteField = EndProperty == null ? null : EndProperty.Name,
                    };
                }
                return wire;
            }
        }

        private static CreateWire _createWire = new CreateWire();

        public void DrawCreateWire(Vector2 canvasOffset)
        {
            if (!_createWire.Enabled)
            {
                return;
            }

            if (!_createWire.HasGraphPosition)
            {
                _createWire.StartPoint = GUIUtility.ScreenToGUIPoint(_createWire.StartPoint) - canvasOffset;
                _createWire.HasGraphPosition = true;
            }

            switch (Event.current.type)
            {
                case EventType.Repaint:
                    if (!_createWire.HasEnd || _createWire.Valid)
                    {
                        GolemEditorUtility.DrawBezier(
                            _createWire.StartPoint + canvasOffset,
                            _createWire.HasEnd ? GUIUtility.ScreenToGUIPoint(_createWire.EndPoint) : Event.current.mousePosition,
                            Vector2.zero,
                            Vector2.zero,
                            true
                            );
                    }
                    break;

                case EventType.MouseMove:
                case EventType.MouseDrag:
                    _createWire.HasEnd = false;
                    Repaint();
                    break;

                case EventType.MouseUp:

                    EditorWire wire = _createWire.DropWire();
                    if (wire != null)
                    {
                        wire.ReadObject.AddOutputWire(wire);
                        wire.WriteObject.AddInputWire(wire);
                        _wires.Add(wire);
                    }
                    Repaint();

                    break;

                case EventType.KeyDown:
                    switch (Event.current.keyCode)
                    {
                        case KeyCode.Escape:
                            _createWire.Enabled = false;
                            Repaint();
                            Event.current.Use();
                            break;
                    }
                    break;
            }
        }


        internal struct CreateTransition
        {
            public readonly object StartObject;
            public Vector2 StartPoint;

            public bool Enabled;
            public bool Disabled { get { return !Enabled; } }
            public bool HasGraphPosition;

            public bool HasEnd;
            public bool Valid;

            public object EndObject;
            public Vector2 EndPoint;

            public CreateTransition(object startObject, Vector2 startPoint)
            {
                StartObject = startObject;
                StartPoint = startPoint;
                Enabled = true;
                HasGraphPosition = false;
                HasEnd = false;
                Valid = false;
                EndObject = null;
                EndPoint = startPoint;
            }

            public static CreateTransition Create(object startObject, Vector2 startPoint)
            {
                return new CreateTransition(startObject, GUIUtility.GUIToScreenPoint(startPoint));
            }

            public void HoverEnd(object endObject, Vector2 endPoint)
            {
                HasEnd = true;
                Valid = !object.ReferenceEquals(StartObject, endObject);
                #warning also check that no transition in the same direction already exists between these

                EndObject = endObject;
                EndPoint = GUIUtility.GUIToScreenPoint(endPoint);
            }

            public EditorTransition DropTransition()
            {
                Enabled = false;
                if (!HasEnd || !Valid)
                {
                    return null;
                }

                EditorTransition transition = new EditorTransition
                {
                    Name = "Transition",
                    Expression = new EditorTransitionExpression
                    {
                        Type = EditorTransitionExpressionType.True,
                    },
                    From = (StartObject as EditorState).Index,
                    To = (EndObject as EditorState).Index,
                    ExpressionAnchor = (StartPoint + GUIUtility.ScreenToGUIPoint(EndPoint)) * 0.5f
                };
                return transition;
            }

        }

        private CreateTransition _createTransition;

        public void DrawCreateTransition(Vector2 canvasOffset)
        {
            if (!_createTransition.Enabled)
            {
                return;
            }

            if (!_createTransition.HasGraphPosition)
            {
                _createTransition.StartPoint = GUIUtility.ScreenToGUIPoint(_createTransition.StartPoint) - canvasOffset;
                _createTransition.HasGraphPosition = true;
            }

            switch (Event.current.type)
            {
                case EventType.Repaint:
                    if (!_createTransition.HasEnd || _createTransition.Valid)
                    {
                        GolemEditorUtility.DrawBezier(
                            _createTransition.StartPoint + canvasOffset,
                            _createTransition.HasEnd ? GUIUtility.ScreenToGUIPoint(_createTransition.EndPoint) : Event.current.mousePosition,
                            Vector2.zero,
                            Vector2.zero,
                            true
                            );
                    }
                    break;

                case EventType.MouseMove:
                case EventType.MouseDrag:
                    _createTransition.HasEnd = false;
                    _rightClickIsContextMenu = false;
                    Repaint();
                    break;

                case EventType.MouseUp:

                    EditorTransition transition = _createTransition.DropTransition();
                    if (transition != null)
                    {
                        transition.Index = (EditorTransitionIndex)_transitions.Count;
                        _states[(int)transition.From].TransitionsOut.Add(transition.Index);
                        _states[(int)transition.To].TransitionsIn.Add(transition.Index);
                        _transitions.Add(transition);
                    }
                    Repaint();

                    break;

                case EventType.KeyDown:
                    switch (Event.current.keyCode)
                    {
                        case KeyCode.Escape:
                            _createTransition.Enabled = false;
                            Repaint();
                            Event.current.Use();
                            break;
                    }
                    break;
            }
        }
    }
}
