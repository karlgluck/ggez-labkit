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
        private Vector2 _scrollSize;

        private bool _shouldScroll = true;
        private Vector2 _mouseDownPosition;
        private Vector2 _scrollPositionOnMouseDown;

        private List<EditorCell> _cells { get { return _golem.Archetype.Components[_golem.Archetype.EditorWindowSelectedComponent].EditorCells; } }
        private List<EditorWire> _wires { get { return _golem.Archetype.Components[_golem.Archetype.EditorWindowSelectedComponent].EditorWires; } }
        private bool _shouldRead = false;

        private List<EditorState> _states { get { return _golem.Archetype.Components[_golem.Archetype.EditorWindowSelectedComponent].EditorStates; } }
        private List<EditorTransition> _transitions { get { return _golem.Archetype.Components[_golem.Archetype.EditorWindowSelectedComponent].EditorTransitions; } }

        private void Read()
        {
            if (_golem == null)
            {
                return;
            }

            // Make sure all references are reset so there are no dangling objects
            IsCreatingWire = false;
            _creatingWireStartCell = null;
            _creatingWireEndEditorCell = null;
            IsCreatingTransition = false;
            _creatingTransitionStartState = null;
            _creatingTransitionEndState = null;

            // Reload the object
            _shouldRead = false;
        }


        private EditorCell pickEditorCell(Vector2 graphPosition)
        {
            for (int i = 0; i < _cells.Count; ++i)
            {
                if (_cells[i].Position.Contains(graphPosition))
                {
                    return _cells[i];
                }
            }
            return null;
        }

        private EditorState pickEditorState(Vector2 graphPosition)
        {
            for (int i = 0; i < _states.Count; ++i)
            {
                if (_states[i].Position.Contains(graphPosition))
                {
                    return _states[i];
                }
            }
            return null;
        }

        private EditorWire pickEditorWire(Vector2 graphPosition)
        {
            const float toleranceSquared = 10f * 10f;
            for (int i = 0; i < _wires.Count; ++i)
            {
                Vector2 from, to;
                var editorWire = _wires[i];
                getEditorWirePoints(editorWire, out from, out to);
                if (Vector2Ext.DistanceToLineSegmentSquared(graphPosition, from, to) < toleranceSquared)
                {
                    return _wires[i];
                }
            }
            return null;
        }

        private EditorTransition pickEditorTransition(Vector2 graphPosition)
        {
            const float toleranceSquared = 10f * 10f;
            for (int i = 0; i < _transitions.Count; ++i)
            {
                Vector2 from, to;
                var transition = _transitions[i];
                getEditorTransitionPoints(transition, out from, out to);
                if (Vector2Ext.DistanceToLineSegmentSquared(graphPosition, from, to) < toleranceSquared)
                {
                    return transition;
                }
            }
            return null;
        }

        private bool pickEditorCellOutputPort(Vector2 graphPosition, out EditorCell editorCell, out InspectableCellType inspectableType, out int outputPort)
        {
            editorCell = pickEditorCell(graphPosition);
            inspectableType = null;
            outputPort = -1;
            if (editorCell == null || object.ReferenceEquals(editorCell, _creatingWireStartCell))
            {
                return false;
            }
            inspectableType = GetInspectableCellType(editorCell.Cell.GetType());
            var outputs = inspectableType.Outputs;
            for (int i = 0; i < outputs.Length; ++i)
            {
                var portRect = GolemEditorUtility.GetNodeOutputPortRect(editorCell.Position, outputs[i].PortCenterFromTopRight);
                if (portRect.Contains(graphPosition))
                {
                    outputPort = i;
                    return true;
                }
            }
            return false;
        }

        private bool pickEditorCellInputPort(Vector2 graphPosition, out EditorCell editorCell, out InspectableCellType inspectableType, out int inputPort)
        {
            editorCell = pickEditorCell(graphPosition);
            inspectableType = null;
            inputPort = -1;
            if (editorCell == null || object.ReferenceEquals(editorCell, _creatingWireStartCell))
            {
                return false;
            }
            inspectableType = GetInspectableCellType(editorCell.Cell.GetType());
            var inputs = inspectableType.Inputs;
            for (int i = 0; i < inputs.Length; ++i)
            {
                var portRect = GolemEditorUtility.GetNodeInputPortRect(editorCell.Position, inputs[i].PortCenterFromTopLeft);
                if (portRect.Contains(graphPosition))
                {
                    inputPort = i;
                    return true;
                }
            }
            return false;
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

        private PickedEditorTransitionExpression pickEditorTransitionExpression(
                Vector2 graphPosition
                )
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

        private void getEditorWirePoints(EditorWire editorWire, out Vector2 from, out Vector2 to)
        {
            var readCell = editorWire.ReadCell;
            var writeCell = editorWire.WriteCell;

            from = readCell.Position.center;
            to = writeCell.Position.center;

            var fromType = GetInspectableCellType(readCell.Cell.GetType());
            var outputs = fromType.Outputs;
            for (int i = 0; i < outputs.Length; ++i)
            {
                if (outputs[i].Name.Equals(editorWire.ReadField))
                {
                    from = GolemEditorUtility.GetNodeOutputPortRect(readCell.Position, outputs[i].PortCenterFromTopRight).center;
                    break;
                }
            }
            var toType = GetInspectableCellType(writeCell.Cell.GetType());
            var inputs = toType.Inputs;
            for (int i = 0; i < inputs.Length; ++i)
            {
                if (inputs[i].Name.Equals(editorWire.WriteField))
                {
                    to = GolemEditorUtility.GetNodeInputPortRect(writeCell.Position, inputs[i].PortCenterFromTopLeft).center;
                    break;
                }
            }
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

        private bool _shouldDrag = false;
        private IDraggable _draggable = null;


        private InspectableCellType GetInspectableCellType(Type cellType)
        {
            return InspectableCellType.GetInspectableCellType(cellType);
        }

        private InspectableScriptType GetInspectableScriptType(Type scriptType)
        {
            return InspectableScriptType.GetInspectableScriptType(scriptType);
        }

        public IDraggable DragWire(EditorCell editorCell, bool isInput, string startPortName, int portIndex, Vector2 startPoint)
        {
            IsCreatingWire = true;
            IsCreatingInputWire = isInput;
            _creatingWireStartCell = editorCell;
            _creatingWireStartPortName = startPortName;
            _creatingWireStartPort = portIndex;
            _creatingWireStartInspectableType = InspectableCellType.GetInspectableCellType(editorCell.Cell.GetType());
            _creatingWireStartPoint = startPoint;
            _creatingWireEndPoint = startPoint;
            return new DraggableWire() { Window = this };
        }

        internal class DraggableWire : IDraggable
        {
            public GolemEditorWindow Window;
            public Vector2 Offset
            {
                set { Window._creatingWireEndPoint = Window._creatingWireStartPoint + value; }
            }
        }

        public bool IsCreatingWire;
        public bool IsCreatingInputWire;
        private EditorCell _creatingWireStartCell;
        private InspectableCellType _creatingWireStartInspectableType;
        private string _creatingWireStartPortName;
        private int _creatingWireStartPort;
        private Vector2 _creatingWireStartPoint;
        public bool CreatingWireHoveringEndPoint;
        public bool CreatingWireHoveringInvalidEndPoint;
        private Vector2 _creatingWireEndPoint;
        private EditorCell _creatingWireEndEditorCell;
        private InspectableCellType _creatingWireEndInspectableType;
        private int _creatingWireEndPort;

        public IDraggable DragTransition(EditorState editorState)
        {
            IsCreatingTransition = true;
            _creatingTransitionStartState = editorState;
            _creatingTransitionStartPoint = editorState.Position.center;
            _creatingTransitionEndPoint = _mouseDownPosition;
            return new DraggableTransition() { Window = this };
        }

        internal class DraggableTransition : IDraggable
        {
            public GolemEditorWindow Window;
            public Vector2 Offset
            {
                set { Window._creatingTransitionEndPoint = Window._mouseDownPosition + value; }
            }
        }

        public bool IsCreatingTransition;
        private EditorState _creatingTransitionStartState;
        private Vector2 _creatingTransitionStartPoint;
        private bool _creatingTransitionHoveringEndPoint;
        private Vector2 _creatingTransitionEndPoint;
        private EditorState _creatingTransitionEndState;

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
                    GUILayout.Label("No golem is selected for editing", EditorStyles.largeLabel);
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    #warning this text is wrong
                    GUILayout.Label("Select an golem then press 'Open Editor' from the inspector.", EditorStyles.miniLabel);
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

            // We need to do some black magic here to work around Unity,
            // otherwise the scaling will apply to the clipping area!
            // http://martinecker.com/martincodes/unity-editor-window-zooming/
            Matrix4x4 previousGuiMatrix = Matrix4x4.identity;
            bool isRescaling = !Mathf.Approximately(_graphScale, 1f);
            if (isRescaling)
            {
                GUI.EndGroup();
                GUI.BeginGroup(new Rect(new Vector2(0f, GolemEditorUtility.editorWindowTabHeight / _graphScale), position.size / _graphScale));
                previousGuiMatrix = GUI.matrix;
                GUI.matrix = Matrix4x4.Scale(Vector3.one * _graphScale);
            }

            //-------------------------------------------------
            // Draw the background grid texture
            //-------------------------------------------------
            {
                var rect = new Rect(Vector2.zero, this.position.size / _graphScale);
                Texture2D gridTexture = GolemEditorUtility.GridTexture;
                Vector2 offset = _scrollPosition + _scrollAnchor;
                float aligningOffset = rect.height - (int)(1 + rect.height / gridTexture.height) * gridTexture.height;
                Vector2 tileOffset = new Vector2(-offset.x / gridTexture.width, (offset.y - aligningOffset) / gridTexture.height);
                Vector2 tileAmount = new Vector2(rect.width / gridTexture.width, rect.height / gridTexture.height);
                GUI.DrawTextureWithTexCoords(rect, gridTexture, new Rect(tileOffset, tileAmount), false);
            }

            // Compute and store this at the window level because entering a layout context changes the value of mousePosition!
            var mouseGraphPosition = WindowToGraphPosition(Event.current.mousePosition);

            if (IsCreatingTransition)
            {
                bool cancelTransitionCreation = false;// Event.current.type == EventType.MouseDown && Event.current.button == 1;
                bool tryCreatingTransition = _shouldDrag && Event.current.button == 0 && (Event.current.type == EventType.MouseUp || Event.current.type == EventType.MouseDown);

                if ((Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseMove) || tryCreatingTransition)
                {
                    var endState = pickEditorState(mouseGraphPosition);
                    if (endState != _creatingTransitionEndState)
                    {
                        _creatingTransitionEndState = endState;
                        _creatingTransitionHoveringEndPoint = endState != null;
                        Repaint();
                    }
                    if (endState != null)
                    {
                        if (tryCreatingTransition)
                        {
                            var transition = new EditorTransition
                            {
                                Name = "Transition",
                                Index = (EditorTransitionIndex)_transitions.Count,
                                Expression = new EditorTransitionExpression
                                {
                                    Type = EditorTransitionExpressionType.True,
                                },
                                From = _creatingTransitionStartState.Index,
                                To = _creatingTransitionEndState.Index,
                            };
                            _creatingTransitionStartState.TransitionsOut.Add(transition.Index);
                            _creatingTransitionEndState.TransitionsIn.Add(transition.Index);
                            {
                                Vector2 fromPosition, toPosition;
                                getEditorTransitionPoints(transition, out fromPosition, out toPosition);
                                transition.ExpressionAnchor = (fromPosition + toPosition) * 0.5f;
                            }
                            _transitions.Add(transition);
                            IsCreatingTransition = false;
                            _shouldWrite = true;
                        }
                        else
                        {
                            _creatingTransitionEndPoint = endState.Position.center;
                        }
                    }
                }

                if (cancelTransitionCreation || tryCreatingTransition)
                {
                    wantsMouseMove = false;
                    IsCreatingTransition = false;
                    _draggable = null;
                    _shouldWrite = true;
                    Repaint();
                }
            }

            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Delete)
            {
                var editorWireSelection = _selection as EditorWire;
                if (editorWireSelection != null)
                {
                    editorWireSelection.ReadCell.RemoveOutputWire(editorWireSelection);
                    editorWireSelection.WriteCell.RemoveInputWire(editorWireSelection);
                    _wires.Remove(editorWireSelection);
                    _selection = null;
                    Repaint();
                    _shouldWrite = true;
                }

                var editorTransitionSelection = _selection as EditorTransition;
                if (editorTransitionSelection != null)
                {
                    removeTransitionAt(editorTransitionSelection.Index);
                    _selection = null;
                    Repaint();
                    _shouldWrite = true;
                }
            }

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                _scrollPositionOnMouseDown = _scrollPosition;
                mouseGraphPosition = WindowToGraphPosition(Event.current.mousePosition);
                _mouseDownPosition = mouseGraphPosition;

                var editorCell = this.pickEditorCell(mouseGraphPosition);
                var editorState = this.pickEditorState(mouseGraphPosition);
                var editorWire = this.pickEditorWire(mouseGraphPosition);
                var editorTransition = this.pickEditorTransition(mouseGraphPosition);
                var pickedEditorTransitionExpression = this.pickEditorTransitionExpression(mouseGraphPosition);

                _shouldScroll = editorCell == null && editorState == null && editorWire == null && editorTransition == null && pickedEditorTransitionExpression == null;
                if (editorCell != null)
                {
                    if (_selection != editorCell)
                    {
                        Repaint();
                    }
                    _selection = editorCell;
                    _draggable = null;
                    if (GolemEditorUtility.GetNodeTitleRect(editorCell.Position).Contains(mouseGraphPosition))
                    {
                        _draggable = editorCell.DragPosition();
                    }
                    else if (GolemEditorUtility.GetNodeResizeRect(editorCell.Position).Contains(mouseGraphPosition))
                    {
                        _draggable = editorCell.DragSize();
                    }
                    else
                    {
                        var inspectableType = GetInspectableCellType(editorCell.Cell.GetType());
                        var inputs = inspectableType.Inputs;
                        for (int i = 0; i < inputs.Length; ++i)
                        {
                            var portRect = GolemEditorUtility.GetNodeInputPortRect(editorCell.Position, inputs[i].PortCenterFromTopLeft);
                            if (portRect.Contains(mouseGraphPosition))
                            {
                                _draggable = DragWire(editorCell, true, inputs[i].Name, i, portRect.center);
                            }
                        }
                        var outputs = inspectableType.Outputs;
                        for (int i = 0; i < outputs.Length; ++i)
                        {
                            var portRect = GolemEditorUtility.GetNodeOutputPortRect(editorCell.Position, outputs[i].PortCenterFromTopRight);
                            if (portRect.Contains(mouseGraphPosition))
                            {
                                _draggable = DragWire(editorCell, false, outputs[i].Name, i, portRect.center);
                            }
                        }
                    }
                    _shouldDrag = _draggable != null;
                    if (_shouldDrag)
                    {
                        Repaint();
                    }
                }
                else if (editorState != null)
                {
                    if (_selection != editorState)
                    {
                        Repaint();
                    }
                    _selection = editorState;
                    _draggable = null;
                    if (GolemEditorUtility.GetNodeTitleRect(editorState.Position).Contains(mouseGraphPosition))
                    {
                        _draggable = editorState.DragPosition();
                    }
                    else if (GolemEditorUtility.GetNodeResizeRect(editorState.Position).Contains(mouseGraphPosition))
                    {
                        _draggable = editorState.DragSize();
                    }
                    _shouldDrag = _draggable != null;
                    if (_shouldDrag)
                    {
                        Repaint();
                    }
                }
                else if (editorWire != null)
                {
                    if (_selection != editorWire)
                    {
                        Repaint();
                    }
                    _selection = editorWire;
                }
                else if (pickedEditorTransitionExpression != null)
                {
                    _draggable = pickedEditorTransitionExpression.Transition.DragExpressionAnchor();
                    _shouldDrag = true;
                    Repaint();
                    _selection = null;
                }
                else if (editorTransition != null)
                {
                    if (_selection != editorTransition)
                    {
                        Repaint();
                    }
                    _selection = editorTransition;
                }
                else
                {
                    if (_selection != null)
                    {
                        _selection = null;
                        Repaint();
                    }
                    if (_draggable != null)
                    {
                        _shouldWrite = true;
                    }
                    _draggable = null;
                    _shouldDrag = false;
                }
            }

            if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                _shouldScroll = false;
                if (_draggable != null)
                {
                    _shouldWrite = true;
                    _draggable = null;
                }
                // this doesn't work all that well
                // if (_draggable != null)
                // {
                //     bool allowDragWithMouseUp = (mouseGraphPosition - _mouseDownPosition).magnitude < 10f;
                //     allowDragWithMouseUp = false;
                //     Debug.Log("mouseup " + allowDragWithMouseUp + " " + mouseGraphPosition + " " + _mouseDownPosition + " " + mouseGraphPosition);
                //     if (allowDragWithMouseUp)
                //     {
                //         wantsMouseMove = true;
                //     }
                //     else
                //     {
                //         Debug.Log("Cancel drag");
                //         _shouldDrag = false;
                //         shouldWrite = true;
                //     }
                // }
                AddWindowRectToScrollArea();
            }

            if (_shouldScroll)
            {
                if (Event.current.type == EventType.MouseDrag && Event.current.button == 0)
                {
                    _scrollPosition = _scrollPositionOnMouseDown + (mouseGraphPosition - _mouseDownPosition);
                    Repaint();
                }
            }
            if (_shouldDrag)
            {
                if ((Event.current.type == EventType.MouseDrag && Event.current.button == 0) || Event.current.type == EventType.MouseMove)
                {
                    _draggable.Offset = mouseGraphPosition - _mouseDownPosition;
                    Repaint();
                }
            }

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

                bool selected = object.ReferenceEquals(_selection, editorCell);
                var nodeRect = GolemEditorUtility.BeginNode(editorCell.Name, editorCell.Position, selected, _scrollPosition + _scrollAnchor);
                if (!IsCreatingWire)
                {
                    EditorGUIUtility.AddCursorRect(GraphToWindowPosition(GolemEditorUtility.GetNodeResizeRect(nodeRect)), MouseCursor.ResizeUpLeft);
                    EditorGUIUtility.AddCursorRect(GraphToWindowPosition(GolemEditorUtility.GetNodeTitleRect(nodeRect)), MouseCursor.MoveArrow);
                }

                var inspectableCellType = GetInspectableCellType(editorCell.Cell.GetType());
                var inputs = inspectableCellType.Inputs;
                var outputs = inspectableCellType.Outputs;
                var ioRect = EditorGUILayout.GetControlRect(false, Mathf.Max(inputs.Length, outputs.Length) * EditorGUIUtility.singleLineHeight);

                //-------------------------------
                // Cell Inputs
                //-------------------------------
                for (int i = 0; i < inputs.Length; ++i)
                {
                    var position = new Rect(ioRect.position + new Vector2(GolemEditorUtility.PortLayoutWidth, EditorGUIUtility.singleLineHeight * i), new Vector2(ioRect.width * 0.75f, EditorGUIUtility.singleLineHeight));
                    EditorGUI.LabelField(position, new GUIContent(inputs[i].Name, inputs[i].Field.FieldType.Name));
                    var portCenter = inputs[i].PortCenterFromTopLeft;
                    GolemEditorUtility.DrawPort(portCenter, editorCell.HasInputWire(inputs[i].Name), false);
                    var rect = GolemEditorUtility.GetPortRect(portCenter);
                    if (!IsCreatingWire)
                    {
                        GolemEditorUtility.AddScaledCursorRect(_graphScale, rect, MouseCursor.ArrowPlus);
                    }
                }

                //-------------------------------
                // Cell Outputs
                //-------------------------------
                var topRight = new Vector2(ioRect.width, 0);
                for (int i = 0; i < outputs.Length; ++i)
                {
                    var position = new Rect(ioRect.position + new Vector2(ioRect.width * 0.25f, EditorGUIUtility.singleLineHeight * i), new Vector2(ioRect.width * 0.75f - GolemEditorUtility.PortLayoutWidth, EditorGUIUtility.singleLineHeight));
                    EditorGUI.LabelField(position, new GUIContent(outputs[i].Name, outputs[i].Field.FieldType.Name), GolemEditorUtility.outputLabelStyle);
                    var portCenter = topRight + outputs[i].PortCenterFromTopRight;
                    GolemEditorUtility.DrawPort(portCenter, editorCell.HasOutputWire(outputs[i].Name), false);
                    var rect = GolemEditorUtility.GetPortRect(portCenter);
                    if (!IsCreatingWire)
                    {
                        GolemEditorUtility.AddScaledCursorRect(_graphScale, rect, MouseCursor.ArrowPlus);
                    }
                }


                //-------------------------------
                // Cell Fields
                //-------------------------------
                var fields = inspectableCellType.Fields;
                for (int i = 0; i < fields.Length; ++i)
                {
                    var fieldInfo = fields[i];
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

                GolemEditorUtility.EndNode();
            }

            _shouldWrite = _shouldWrite || EditorGUI.EndChangeCheck();


            // Draw transition we are creating
            //-------------------------------------------------
            if (IsCreatingTransition)
            {
                GolemEditorUtility.DrawBezier(_scrollPosition + _scrollAnchor + _creatingTransitionStartPoint, _scrollPosition + _scrollAnchor + _creatingTransitionEndPoint, Vector2.zero, Vector2.zero, _creatingTransitionHoveringEndPoint);
            }


            //-------------------------------------------------
            // Draw transitions
            //-------------------------------------------------
            foreach (EditorTransition editorTransition in _transitions)
            {
                var fromState = _states[(int)editorTransition.From];
                var toState = _states[(int)editorTransition.To];
                bool selected = object.ReferenceEquals(_selection, editorTransition);
                bool hasReverse = editorTransitionHasReverse(editorTransition);

                GolemEditorUtility.DrawTransition(editorTransition, fromState, toState, hasReverse, selected, _scrollPosition + _scrollAnchor);
            }

            //-------------------------------------------------
            // Draw the states
            //-------------------------------------------------

            EditorGUI.BeginChangeCheck();

            foreach (EditorState editorState in _states)
            {
                bool selected = object.ReferenceEquals(_selection, editorState);
                GolemEditorUtility.BeginNode(editorState.Name, editorState.Position, selected, _scrollPosition + _scrollAnchor, editorState.NodeColor);

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
                    var fields = inspectableType.Fields;
                    for (int j = 0; j < fields.Length; ++j)
                    {
                        var fieldInfo = fields[j];
                        GolemEditorUtility.EditorGUILayoutGolemField(
                                fieldInfo.Type,
                                fieldInfo.SpecificType,
                                fieldInfo.FieldInfo,
                                editorScript.Script,
                                editorScript.FieldsUsingSettings,
                                editorScript.FieldsUsingVariables,
                                _golem
                                );
                    }
                }
                GolemEditorUtility.EndNode();
            }

            _shouldWrite = _shouldWrite || EditorGUI.EndChangeCheck();

            //-------------------------------------------------
            // Draw a wire we are creating
            //-------------------------------------------------
            if (IsCreatingWire)
            {
                GolemEditorUtility.DrawBezier(_scrollPosition + _scrollAnchor + _creatingWireStartPoint, _scrollPosition + _scrollAnchor + _creatingWireEndPoint, Vector2.zero, Vector2.zero, false);
                GolemEditorUtility.DrawPort(_scrollPosition + _scrollAnchor + _creatingWireStartPoint, true, false);
                if (CreatingWireHoveringEndPoint)
                {
                    GolemEditorUtility.DrawPort(_scrollPosition + _scrollAnchor + _creatingWireEndPoint, true, false);
                }

                if (Event.current.type == EventType.MouseDrag || (Event.current.type == EventType.MouseUp && Event.current.button == 0))
                {
                    if (IsCreatingInputWire && pickEditorCellOutputPort(mouseGraphPosition, out _creatingWireEndEditorCell, out _creatingWireEndInspectableType, out _creatingWireEndPort))
                    {
                        if (Event.current.type == EventType.MouseDrag)
                        {
                            var endPoint = GolemEditorUtility.GetNodeOutputPortRect(_creatingWireEndEditorCell.Position, _creatingWireEndInspectableType.Outputs[_creatingWireEndPort].PortCenterFromTopRight).center;
                            if (!CreatingWireHoveringEndPoint || _creatingWireEndPoint != endPoint)
                            {
                                CreatingWireHoveringEndPoint = true;
                                _creatingWireEndPoint = endPoint;
                                Repaint();
                            }
                        }
                        else if (InspectableCellType.CanConnect(_creatingWireEndInspectableType, _creatingWireEndPort, _creatingWireStartInspectableType, _creatingWireStartPort))
                        {
                            var wire = new EditorWire
                            {
                                Register = RegisterPtr.Invalid,
                                ReadCell = _creatingWireEndEditorCell,
                                ReadField = _creatingWireEndInspectableType.Outputs[_creatingWireEndPort].Name,
                                WriteCell = _creatingWireStartCell,
                                WriteField = _creatingWireStartPortName,
                            };
                            _creatingWireEndEditorCell.AddOutputWire(wire);
                            _creatingWireStartCell.AddInputWire(wire);
                            _wires.Add(wire);
                            IsCreatingWire = false;
                            _shouldWrite = true;
                        }
                    }
                    else if (!IsCreatingInputWire && pickEditorCellInputPort(mouseGraphPosition, out _creatingWireEndEditorCell, out _creatingWireEndInspectableType, out _creatingWireEndPort))
                    {
                        if (Event.current.type == EventType.MouseDrag)
                        {
                            var endPoint = GolemEditorUtility.GetNodeInputPortRect(_creatingWireEndEditorCell.Position, _creatingWireEndInspectableType.Inputs[_creatingWireEndPort].PortCenterFromTopLeft).center;
                            if (!CreatingWireHoveringEndPoint || _creatingWireEndPoint != endPoint)
                            {
                                CreatingWireHoveringEndPoint = true;
                                _creatingWireEndPoint = endPoint;
                                Repaint();
                            }
                        }
                        else if (InspectableCellType.CanConnect(_creatingWireStartInspectableType, _creatingWireStartPort, _creatingWireEndInspectableType, _creatingWireEndPort))
                        {
                            var wire = new EditorWire
                            {
                                Register = RegisterPtr.Invalid,
                                ReadCell = _creatingWireStartCell,
                                ReadField = _creatingWireStartPortName,
                                WriteCell = _creatingWireEndEditorCell,
                                WriteField = _creatingWireEndInspectableType.Inputs[_creatingWireEndPort].Name,
                            };
                            _creatingWireStartCell.AddOutputWire(wire);
                            _creatingWireEndEditorCell.AddInputWire(wire);
                            _wires.Add(wire);
                            IsCreatingWire = false;
                            _shouldWrite = true;
                        }
                    }
                    else
                    {
                        if (CreatingWireHoveringEndPoint)
                        {
                            CreatingWireHoveringEndPoint = false;
                            Repaint();
                        }
                    }
                }
                if (Event.current.type == EventType.MouseUp)
                {
                    IsCreatingWire = false;
                    Repaint();
                }
            }

            //-------------------------------------------------
            // Draw wires in the graph
            //-------------------------------------------------
            foreach (EditorWire editorWire in _wires)
            {
                bool selected = object.ReferenceEquals(_selection, editorWire);
                Vector2 from, to;
                getEditorWirePoints(editorWire, out from, out to);
                GolemEditorUtility.DrawEdge(
                    new ControlPoint { Point = from },
                    new ControlPoint { Point = to },
                    null,
                    selected,
                    _scrollPosition + _scrollAnchor
                    );
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
            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                Event.current.Use();
                var menu = new GenericMenu();

                _scrollPositionOnMouseDown = _scrollPosition;
                _menuFunctionPosition = WindowToGraphPosition(Event.current.mousePosition);
                _mouseDownPosition = mouseGraphPosition;
                _shouldScroll = false;

                var editorCell = pickEditorCell(mouseGraphPosition);
                var editorState = pickEditorState(mouseGraphPosition);
                var pickedEditorTransitionExpression = pickEditorTransitionExpression(mouseGraphPosition);
                if (editorCell == null
                        && editorState == null
                        && pickedEditorTransitionExpression == null)
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
                }
                else if (editorState != null)
                {
                    menu.AddItem(new GUIContent("Create Transition"), false, CreateTransitionMenuFunction, editorState);
                    menu.AddSeparator("");
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
                else if (GolemEditorUtility.GetNodeTitleRect(editorCell.Position).Contains(mouseGraphPosition))
                {
                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent("Delete Cell"), false, DeleteEditorCellMenuFunction, editorCell);
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
            GUILayout.Label("_scrollPosition: " + _scrollPosition.ToString() + " - _scrollAnchor: " + _scrollAnchor.ToString() + " - _scrollSize: " + _scrollSize.ToString() + " - mouseDown: " + _mouseDownPosition.ToString() + " - graphPos: " + mouseGraphPosition.ToString());
            GUILayout.EndArea();

            if (_shouldWrite)
            {
                var component = _golem.Archetype.Components[_golem.Archetype.EditorWindowSelectedComponent];
                EditorUtility.SetDirty(component);
                if (!AssetDatabase.IsMainAsset(component))
                {
                    // EditorUtility.SetDirty(_golem);
                    // EditorUtility.SetDirty(_golem.Archetype);
                    GolemEditorUtility.SetSceneDirty(_golem.gameObject);
                }
                Repaint();
                _shouldWrite = false;
            }
        }

        private void removeTransitionAt(EditorTransitionIndex indexToRemove)
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
                removeTransitionAt(indicesToRemove[i]);
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

        private void DeleteEditorCellMenuFunction(object editorCellParam)
        {
            var editorCell = editorCellParam as EditorCell;
            foreach (var input in editorCell.GetAllInputWires())
            {
                input.ReadCell.RemoveOutputWire(input);
                _wires.Remove(input);
            }
            foreach (var output in editorCell.GetAllOutputWires())
            {
                output.WriteCell.RemoveInputWire(output);
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
        }

        private void DeleteEditorStateMenuFunction(object editorStateParam)
        {
            var editorStateBeingRemoved = editorStateParam as EditorState;
            EditorStateIndex indexToRemove = editorStateBeingRemoved.Index;
            Debug.Assert(editorStateBeingRemoved.Index == indexToRemove);
            removeTransitionsAt(editorStateBeingRemoved.TransitionsIn);
            removeTransitionsAt(editorStateBeingRemoved.TransitionsOut);
#if UNITY_EDITOR
            foreach (var transitionIndex in editorStateBeingRemoved.TransitionsIn)
            {
                Debug.Assert(transitionIndex == EditorTransitionIndex.Invalid);
            }
            foreach (var transitionIndex in editorStateBeingRemoved.TransitionsOut)
            {
                Debug.Assert(transitionIndex == EditorTransitionIndex.Invalid);
            }
#endif
            _states.RemoveAt((int)indexToRemove);


            // If anything uses EditorStateIndex other than the transitions that are into or out of this state,
            // we need to update those references here.

            _shouldWrite = true;
        }

        private void CreateTransitionMenuFunction(object editorStateParam)
        {
            var editorState = (EditorState)editorStateParam;
            _draggable = DragTransition(editorState);
            _shouldDrag = true;
            wantsMouseMove = true;
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
    }
}
