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

namespace GGEZ.Omnibus
{

public class OmnibusEditorWindow : EditorWindow
{
    public static void Open (EntityEditorData editable)
    {
        var window = EditorWindow.GetWindow<OmnibusEditorWindow>("Circuit Editor");
        window.Initialize (editable);
        window.Show ();
    }

    public static void OpenEmpty()
    {
        Open (null);
    }

    public void Initialize (EntityEditorData editable)
    {
        _selection = null;
        _editable = editable;
        _scrollPosition = Vector2.zero;
        _scrollAnchor = Vector2.zero;
        _scrollSize = this.position.size;

        Read ();
    }

    protected EntityEditorData _editable { get; private set; }


    void OnEnable()
    {
        if (_editable == null) Initialize(null);
        Undo.undoRedoPerformed += OnUndo;
    }

    void OnDisable()
    {
        Undo.undoRedoPerformed -= OnUndo;
    }

    void OnUndo()
    {
        _dirty = true;
        this.Repaint();
    }

    void OnFocus()
    {
        _dirty |= _hierarchyChanged;
    }

    void OnLostFocus()
    {
        _hierarchyChanged = false;
    }

    void OnHierarchyChange()
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

    private List<EditorCell> _cells { get { return _editable.EditorCells; } }
    private List<EditorWire> _wires { get { return _editable.EditorWires; } }
    private bool _dirty = false;

    private List<EditorState> _states { get { return _editable.EditorStates; } }
    private List<EditorTransition> _transitions { get { return _editable.EditorTransitions; } }

    void Read ()
    {
        if (_editable == null)
        {
            return;
        }

        // Make sure all references are reset so there are no dangling objects
        IsCreatingWire = false;
        CreatingWireStartCell = null;
        creatingWireEndEditorCell = null;
        IsCreatingTransition = false;
        CreatingTransitionStartState = null;
        CreatingTransitionEndState = null;

        // Reload the object
        _editable.Load();
        _dirty = false;
    }

    void Write()
    {
        if (_editable == null)
        {
            return;
        }
        _editable.Save();
    }


    EditorCell pickEditorCell (Vector2 graphPosition)
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

    EditorState pickEditorState (Vector2 graphPosition)
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

    EditorWire pickEditorWire (Vector2 graphPosition)
    {
        const float toleranceSquared = 10f*10f;
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

    EditorTransition pickEditorTransition (Vector2 graphPosition)
    {
        const float toleranceSquared = 10f*10f;
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

    bool pickEditorCellOutputPort (Vector2 graphPosition, out EditorCell editorCell, out InspectableCellType inspectableType, out int outputPort)
    {
        editorCell = pickEditorCell(graphPosition);
        inspectableType = null;
        outputPort = -1;
        if (editorCell == null || object.ReferenceEquals(editorCell, CreatingWireStartCell))
        {
            return false;
        }
        inspectableType = GetInspectableCellType(editorCell.Cell.GetType());
        var outputs = inspectableType.Outputs;
        for (int i = 0; i < outputs.Length; ++i)
        {
            var portRect = OmnibusEditorUtility.GetNodeOutputPortRect (editorCell.Position, outputs[i].PortCenterFromTopRight);
            if (portRect.Contains(graphPosition))
            {
                outputPort = i;
                return true;
            }
        }
        return false;
    }

    bool pickEditorCellInputPort (Vector2 graphPosition, out EditorCell editorCell, out InspectableCellType inspectableType, out int inputPort)
    {
        editorCell = pickEditorCell(graphPosition);
        inspectableType = null;
        inputPort = -1;
        if (editorCell == null || object.ReferenceEquals(editorCell, CreatingWireStartCell))
        {
            return false;
        }
        inspectableType = GetInspectableCellType(editorCell.Cell.GetType());
        var inputs = inspectableType.Inputs;
        for (int i = 0; i < inputs.Length; ++i)
        {
            var portRect = OmnibusEditorUtility.GetNodeInputPortRect (editorCell.Position, inputs[i].PortCenterFromTopLeft);
            if (portRect.Contains(graphPosition))
            {
                inputPort = i;
                return true;
            }
        }
        return false;
    }

    EditorTransition pickEditorTransitionExpressionHitbox(Vector2 graphPosition)
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

    struct PickEditorTransitionExpressionWorklistItem
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

    class PickedEditorTransitionExpression
    {
        public EditorTransition Transition;
        public EditorTransitionExpression Expression;
        public EditorTransitionExpression Parent;
        public int IndexInParent;
    }

    PickedEditorTransitionExpression pickEditorTransitionExpression(
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
            int index = worklist.Count-1;
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

    void getEditorWirePoints(EditorWire editorWire, out Vector2 from, out Vector2 to)
    {
        from = editorWire.ReadCell.Position.center;
        to = editorWire.WriteCell.Position.center;

        var fromType = GetInspectableCellType(editorWire.ReadCell.Cell.GetType());
        var outputs = fromType.Outputs;
        for (int i = 0; i < outputs.Length; ++i)
        {
            if (outputs[i].Name.Equals(editorWire.ReadField))
            {
                from = OmnibusEditorUtility.GetNodeOutputPortRect(editorWire.ReadCell.Position, outputs[i].PortCenterFromTopRight).center;
                break;
            }
        }
        var toType = GetInspectableCellType(editorWire.WriteCell.Cell.GetType());
        var inputs = toType.Inputs;
        for (int i = 0; i < inputs.Length; ++i)
        {
            if (inputs[i].Name.Equals(editorWire.WriteField))
            {
                to = OmnibusEditorUtility.GetNodeInputPortRect(editorWire.WriteCell.Position, inputs[i].PortCenterFromTopLeft).center;
                break;
            }
        }
    }

    bool editorTransitionHasReverse(EditorTransition editorTransition)
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

    void getEditorTransitionPoints(EditorTransition editorTransition, out Vector2 from, out Vector2 to)
    {
        var fromState = _states[(int)editorTransition.From];
        var toState = _states[(int)editorTransition.To];

        bool hasReverse = editorTransitionHasReverse(editorTransition);

        OmnibusEditorUtility.GetEditorTransitionPoints(
            fromState,
            toState,
            hasReverse,
            out from,
            out to
        );
    }

    object _selection = null;

    bool _shouldDrag = false;
    IDraggable _draggable = null;


    private Dictionary<Type, InspectableCellType> _cellTypeInspectors = new Dictionary<Type, InspectableCellType>();
    private InspectableCellType GetInspectableCellType(Type cellType)
    {
        InspectableCellType retval;
        if (_cellTypeInspectors.TryGetValue(cellType, out retval))
        {
            return retval;
        }
        retval = InspectableCellType.GetInspectableCellType(cellType);
        _cellTypeInspectors.Add(cellType, retval);
        return retval;
    }

    private Dictionary<Type, InspectableScriptType> _scriptTypeInspectors = new Dictionary<Type, InspectableScriptType>();
    private InspectableScriptType GetInspectableScriptType(Type scriptType)
    {
        InspectableScriptType retval;
        if (_scriptTypeInspectors.TryGetValue(scriptType, out retval))
        {
            return retval;
        }
        retval = InspectableScriptType.GetInspectableScriptType(scriptType);
        _scriptTypeInspectors.Add(scriptType, retval);
        return retval;
    }

    public IDraggable DragWire (EditorCell editorCell, bool isInput, string startPortName, Vector2 startPoint)
    {
        IsCreatingWire = true;
        IsCreatingInputWire = isInput;
        CreatingWireStartCell = editorCell;
        CreatingWireStartPortName = startPortName;
        CreatingWireStartPoint = startPoint;
        CreatingWireEndPoint = startPoint;
        return new DraggableWire() { Window = this };
    }

    internal class DraggableWire : IDraggable
    {
        public OmnibusEditorWindow Window;
        public Vector2 Offset
        {
            set { Window.CreatingWireEndPoint = Window.CreatingWireStartPoint + value; }
        }
    }

    public bool IsCreatingWire;
    public bool IsCreatingInputWire;
    private EditorCell CreatingWireStartCell;
    private string CreatingWireStartPortName;
    private Vector2 CreatingWireStartPoint;
    public bool CreatingWireHoveringEndPoint;
    public bool CreatingWireHoveringInvalidEndPoint;
    private Vector2 CreatingWireEndPoint;
    EditorCell creatingWireEndEditorCell;
    InspectableCellType creatingWireEndInspectableType;
    int creatingWireEndPort;

    public IDraggable DragTransition (EditorState editorState)
    {
        IsCreatingTransition = true;
        CreatingTransitionStartState = editorState;
        CreatingTransitionStartPoint = editorState.Position.center;
        CreatingTransitionEndPoint = _mouseDownPosition;
        return new DraggableTransition() { Window = this };
    }

    internal class DraggableTransition : IDraggable
    {
        public OmnibusEditorWindow Window;
        public Vector2 Offset
        {
            set { Window.CreatingTransitionEndPoint = Window._mouseDownPosition + value; }
        }
    }

    public bool IsCreatingTransition;
    private EditorState CreatingTransitionStartState;
    private Vector2 CreatingTransitionStartPoint;
    private bool CreatingTransitionHoveringEndPoint;
    private Vector2 CreatingTransitionEndPoint;
    private EditorState CreatingTransitionEndState;

    float _graphScale = 1f;

    //-----------------------------------------------------
    // OnGUI
    //-----------------------------------------------------
    void OnGUI()
    {
        if (_editable != null && _editable.Entity == null)
        {
            _editable = null;
        }

        //-------------------------------------------------
        // Draw some placeholder text if no circuit is active
        //-------------------------------------------------
        if (_editable == null)
        {
            if (Selection.activeGameObject != null && Event.current.type == EventType.Layout)
            {
                var entity = Selection.activeGameObject.GetComponent<EntityContainer>();
                if (entity != null)
                {
                    _editable = entity.EditorData as EntityEditorData;
                }
            }
            if (_editable != null)
            {
                _dirty = true;
                Repaint ();
            }
            else
            {
                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("No entity is selected for editing", EditorStyles.largeLabel);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("Select an entity then press 'Open Editor' from the inspector.", EditorStyles.miniLabel);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();

                return;
            }
        }

        if (_dirty && Event.current.type == EventType.Layout)
        {
            Read();
        }

        if (Event.current.type == EventType.ScrollWheel)
        {
            var mousePosition = Event.current.mousePosition;
            float oldScale = _graphScale;

            // Scroll wheel adjusts zoom
            _graphScale = Mathf.Clamp (_graphScale + Event.current.delta.y / 100f, 0.05f, 1f);

            // Move the window so that zooming occurs on the mouse target
            _scrollPosition -= mousePosition / oldScale - mousePosition / _graphScale;

            // Update visible regions
            AddWindowRectToScrollArea ();
            Repaint();
        }

        // We need to do some black magic here to work around Unity,
        // otherwise the scaling will apply to the clipping area!
        // http://martinecker.com/martincodes/unity-editor-window-zooming/
        Matrix4x4 previousGuiMatrix = Matrix4x4.identity;
        bool isRescaling = !Mathf.Approximately (_graphScale, 1f);
        if (isRescaling)
        {
            GUI.EndGroup();
            GUI.BeginGroup(new Rect(new Vector2(0f, OmnibusEditorUtility.editorWindowTabHeight / _graphScale), position.size / _graphScale));
            previousGuiMatrix = GUI.matrix;
            GUI.matrix = Matrix4x4.Scale(Vector3.one * _graphScale);
        }

        //-------------------------------------------------
        // Draw the background grid texture
        //-------------------------------------------------
        {
            var rect = new Rect (Vector2.zero, this.position.size / _graphScale);
            Texture2D gridTexture = OmnibusEditorUtility.GridTexture;
            Vector2 offset = _scrollPosition + _scrollAnchor;
            float aligningOffset = rect.height - (int)(1 + rect.height / gridTexture.height) * gridTexture.height;
            Vector2 tileOffset = new Vector2 (-offset.x / gridTexture.width, (offset.y - aligningOffset) / gridTexture.height);
            Vector2 tileAmount = new Vector2 (rect.width / gridTexture.width, rect.height / gridTexture.height);
            GUI.DrawTextureWithTexCoords (rect, gridTexture, new Rect (tileOffset, tileAmount), false);
        }

        // Compute and store this at the window level because entering a layout context changes the value of mousePosition!
        var mouseGraphPosition = WindowToGraphPosition (Event.current.mousePosition);

        bool shouldWrite = false;

        if (IsCreatingTransition)
        {   
            bool cancelTransitionCreation = false;// Event.current.type == EventType.MouseDown && Event.current.button == 1;
            bool tryCreatingTransition = _shouldDrag && Event.current.button == 0 && (Event.current.type == EventType.MouseUp || Event.current.type == EventType.MouseDown);

            if ((Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseMove) || tryCreatingTransition)
            {
                var endState = pickEditorState(mouseGraphPosition);
                if (endState != CreatingTransitionEndState)
                {
                    CreatingTransitionEndState = endState;
                    CreatingTransitionHoveringEndPoint = endState != null;
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
                            From = CreatingTransitionStartState.Index,
                            To = CreatingTransitionEndState.Index,
                        };
                        CreatingTransitionStartState.TransitionsOut.Add(transition.Index);
                        CreatingTransitionEndState.TransitionsIn.Add(transition.Index);
                        {
                            Vector2 fromPosition, toPosition;
                            getEditorTransitionPoints(transition, out fromPosition, out toPosition);
                            transition.ExpressionAnchor = (fromPosition + toPosition) * 0.5f;
                        }
                        _transitions.Add (transition);
                        IsCreatingTransition = false;
                        shouldWrite = true;
                    }
                    else
                    {
                        CreatingTransitionEndPoint = endState.Position.center;
                    }
                }
            }

            Debug.Log("Cancel = " + cancelTransitionCreation);
            Debug.Log("Try = " + tryCreatingTransition);

            if (cancelTransitionCreation || tryCreatingTransition)
            {
                wantsMouseMove = false;
                IsCreatingTransition = false;
                _draggable = null;
                Repaint();
            }
        }

        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Delete)
        {
            var editorWireSelection = _selection as EditorWire;
            if (editorWireSelection != null)
            {
                editorWireSelection.ReadCell.Outputs.Remove(editorWireSelection);
                editorWireSelection.WriteCell.Inputs.Remove(editorWireSelection);
                _wires.Remove(editorWireSelection);
                _selection = null;
                Repaint();
                shouldWrite = true;
            }
            
            var editorTransitionSelection = _selection as EditorTransition;
            if (editorTransitionSelection != null)
            {
                removeTransitionAt(editorTransitionSelection.Index);
                _selection = null;
                Repaint();
                shouldWrite = true;
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
                    Repaint ();
                }
                _selection = editorCell;
                _draggable = null;
                if (OmnibusEditorUtility.GetNodeTitleRect(editorCell.Position).Contains(mouseGraphPosition))
                {
                    _draggable = editorCell.DragPosition();
                }
                else if (OmnibusEditorUtility.GetNodeResizeRect(editorCell.Position).Contains(mouseGraphPosition))
                {
                    _draggable = editorCell.DragSize();
                }
                else
                {
                    var inspectableType = GetInspectableCellType(editorCell.Cell.GetType());
                    var inputs = inspectableType.Inputs;
                    for (int i = 0; i < inputs.Length; ++i)
                    {
                        var portRect = OmnibusEditorUtility.GetNodeInputPortRect (editorCell.Position, inputs[i].PortCenterFromTopLeft);
                        if (portRect.Contains(mouseGraphPosition))
                        {
                            _draggable = DragWire(editorCell, true, inputs[i].Name, portRect.center);
                        }
                    }
                    var outputs = inspectableType.Outputs;
                    for (int i = 0; i < outputs.Length; ++i)
                    {
                        var portRect = OmnibusEditorUtility.GetNodeOutputPortRect (editorCell.Position, outputs[i].PortCenterFromTopRight);
                        if (portRect.Contains(mouseGraphPosition))
                        {
                            _draggable = DragWire(editorCell, false, outputs[i].Name, portRect.center);
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
                    Repaint ();
                }
                _selection = editorState;
                _draggable = null;
                if (OmnibusEditorUtility.GetNodeTitleRect(editorState.Position).Contains(mouseGraphPosition))
                {
                    _draggable = editorState.DragPosition();
                }
                else if (OmnibusEditorUtility.GetNodeResizeRect(editorState.Position).Contains(mouseGraphPosition))
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
                _draggable = null;
                _shouldDrag = false;
            }
        }

        if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
        {
            _shouldScroll = false;
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
            AddWindowRectToScrollArea ();
        }

        if (_shouldScroll)
        {
            if (Event.current.type == EventType.MouseDrag && Event.current.button == 0)
            {
                _scrollPosition = _scrollPositionOnMouseDown + (mouseGraphPosition - _mouseDownPosition);
                Repaint ();
            }
        }
        if (_shouldDrag)
        {
            if ((Event.current.type == EventType.MouseDrag && Event.current.button == 0) || Event.current.type == EventType.MouseMove)
            {
                _draggable.Offset = mouseGraphPosition - _mouseDownPosition;
                Repaint ();
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
                Read ();
                return;
            }

            bool selected = object.ReferenceEquals(_selection, editorCell);
            var nodeRect = OmnibusEditorUtility.BeginNode(editorCell.Name, editorCell.Position, selected, _scrollPosition + _scrollAnchor);
            if (!IsCreatingWire)
            {
                EditorGUIUtility.AddCursorRect(GraphToWindowPosition(OmnibusEditorUtility.GetNodeResizeRect(nodeRect)), MouseCursor.ResizeUpLeft);
                EditorGUIUtility.AddCursorRect(GraphToWindowPosition(OmnibusEditorUtility.GetNodeTitleRect(nodeRect)), MouseCursor.MoveArrow);
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
                var position = new Rect (ioRect.position + new Vector2(OmnibusEditorUtility.PortLayoutWidth, EditorGUIUtility.singleLineHeight * i), new Vector2(ioRect.width * 0.75f, EditorGUIUtility.singleLineHeight));
                EditorGUI.LabelField(position, new GUIContent(inputs[i].Name, inputs[i].Field.FieldType.Name));
                var portCenter = inputs[i].PortCenterFromTopLeft;
                OmnibusEditorUtility.DrawPort(portCenter, editorCell.HasInputWire(inputs[i].Name), false);
                var rect = OmnibusEditorUtility.GetPortRect(portCenter);
                if (!IsCreatingWire)
                {
                    OmnibusEditorUtility.AddScaledCursorRect(_graphScale, rect, MouseCursor.ArrowPlus);
                }
            }

            //-------------------------------
            // Cell Outputs
            //-------------------------------
            var topRight = new Vector2(ioRect.width, 0);
            for (int i = 0; i < outputs.Length; ++i)
            {
                var position = new Rect (ioRect.position + new Vector2(ioRect.width * 0.25f, EditorGUIUtility.singleLineHeight * i), new Vector2(ioRect.width * 0.75f - OmnibusEditorUtility.PortLayoutWidth, EditorGUIUtility.singleLineHeight));
                EditorGUI.LabelField(position, new GUIContent(outputs[i].Name, outputs[i].Field.FieldType.Name), OmnibusEditorUtility.outputLabelStyle);
                var portCenter = topRight + outputs[i].PortCenterFromTopRight;
                OmnibusEditorUtility.DrawPort(portCenter, editorCell.HasOutputWire(outputs[i].Name), false);
                var rect = OmnibusEditorUtility.GetPortRect(portCenter);
                if (!IsCreatingWire)
                {
                    OmnibusEditorUtility.AddScaledCursorRect(_graphScale, rect, MouseCursor.ArrowPlus);
                }
            }


            //-------------------------------
            // Cell Fields
            //-------------------------------
            var fields = inspectableCellType.Fields;
            for (int i = 0; i < fields.Length; ++i)
            {
                var labelRect = EditorGUILayout.GetControlRect();
                var position = new Rect (labelRect);
                position.xMin = position.xMin + Mathf.Min (EditorGUIUtility.labelWidth, position.width/2f);
                labelRect.xMax = position.xMin;
                EditorGUI.LabelField(labelRect, fields[i].FieldInfo.Name);
                var fieldInfo = fields[i];
                fieldInfo.FieldInfo.SetValue(editorCell.Cell, OmnibusEditorUtility.EditorGUIField(position, fieldInfo.Type, fieldInfo.FieldInfo.FieldType, fieldInfo.FieldInfo.GetValue(editorCell.Cell)));
            }

            OmnibusEditorUtility.EndNode();
        }

        shouldWrite = shouldWrite || EditorGUI.EndChangeCheck();


        // Draw transition we are creating
        //-------------------------------------------------
        if (IsCreatingTransition)
        {
            OmnibusEditorUtility.DrawBezier(_scrollPosition + _scrollAnchor + CreatingTransitionStartPoint, _scrollPosition + _scrollAnchor + CreatingTransitionEndPoint, Vector2.zero, Vector2.zero, false);
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

            OmnibusEditorUtility.DrawTransition(editorTransition, fromState, toState, hasReverse, selected, _scrollPosition + _scrollAnchor);
        }

        //-------------------------------------------------
        // Draw the states
        //-------------------------------------------------

        EditorGUI.BeginChangeCheck();

        foreach (EditorState editorState in _states)
        {
            bool selected = object.ReferenceEquals(_selection, editorState);
            OmnibusEditorUtility.BeginNode(editorState.Name, editorState.Position, selected, _scrollPosition + _scrollAnchor, editorState.NodeColor);

            //-------------------------------
            // State Scripts
            //-------------------------------
            var editorScripts = editorState.Scripts;
            for (int i = 0; i < editorScripts.Count; ++i)
            {
                var editorScript = editorScripts[i];
                var inspectableType = GetInspectableScriptType(editorScript.Script.GetType());

                EditorGUILayout.LabelField (inspectableType.Name, EditorStyles.boldLabel);

                //-------------------------------
                // Script Fields
                //-------------------------------
                var fields = inspectableType.Fields;
                for (int j = 0; j < fields.Length; ++j)
                {
                    var labelRect = EditorGUILayout.GetControlRect();
                    var position = new Rect (labelRect);
                    position.xMin = position.xMin + Mathf.Min (EditorGUIUtility.labelWidth, position.width/2f);
                    labelRect.xMax = position.xMin;
                    EditorGUI.LabelField(labelRect, fields[j].FieldInfo.Name);
                    var fieldInfo = fields[j];
                    fieldInfo.FieldInfo.SetValue(editorScript.Script, OmnibusEditorUtility.EditorGUIField(position, fieldInfo.Type, fieldInfo.FieldInfo.FieldType, fieldInfo.FieldInfo.GetValue(editorScript.Script)));
                }
            }


            OmnibusEditorUtility.EndNode();
        }

        shouldWrite = shouldWrite || EditorGUI.EndChangeCheck();

        //-------------------------------------------------
        // Draw a wire we are creating
        //-------------------------------------------------
        if (IsCreatingWire)
        {
            OmnibusEditorUtility.DrawBezier(_scrollPosition + _scrollAnchor + CreatingWireStartPoint, _scrollPosition + _scrollAnchor + CreatingWireEndPoint, Vector2.zero, Vector2.zero, false);
            OmnibusEditorUtility.DrawPort(_scrollPosition + _scrollAnchor + CreatingWireStartPoint, true, false);
            if (CreatingWireHoveringEndPoint)
            {
                OmnibusEditorUtility.DrawPort(_scrollPosition + _scrollAnchor + CreatingWireEndPoint, true, false);
            }

            if (Event.current.type == EventType.MouseDrag || (Event.current.type == EventType.MouseUp && Event.current.button == 0))
            {
                if (IsCreatingInputWire && pickEditorCellOutputPort(mouseGraphPosition, out creatingWireEndEditorCell, out creatingWireEndInspectableType, out creatingWireEndPort))
                {
                    Debug.LogWarning("todo: make sure the port types match");
                    if (Event.current.type == EventType.MouseDrag)
                    {
                        var endPoint = OmnibusEditorUtility.GetNodeOutputPortRect(creatingWireEndEditorCell.Position, creatingWireEndInspectableType.Outputs[creatingWireEndPort].PortCenterFromTopRight).center;
                        if (!CreatingWireHoveringEndPoint || CreatingWireEndPoint != endPoint)
                        {
                            CreatingWireHoveringEndPoint = true;
                            CreatingWireEndPoint = endPoint;
                            Repaint();
                        }
                    }
                    else
                    {
                        var wire = new EditorWire
                        {
                            Register = -1,
                            ReadCell = creatingWireEndEditorCell,
                            ReadField = creatingWireEndInspectableType.Outputs[creatingWireEndPort].Name,
                            WriteCell = CreatingWireStartCell,
                            WriteField = CreatingWireStartPortName,
                        };
                        creatingWireEndEditorCell.Outputs.Add(wire);
                        CreatingWireStartCell.Inputs.Add(wire);
                        _wires.Add (wire);
                        IsCreatingWire = false;
                        shouldWrite = true;
                    }
                }
                else if (!IsCreatingInputWire && pickEditorCellInputPort(mouseGraphPosition, out creatingWireEndEditorCell, out creatingWireEndInspectableType, out creatingWireEndPort))
                {
                    Debug.LogWarning("todo: make sure the port types match");
                    if (Event.current.type == EventType.MouseDrag)
                    {
                        var endPoint = OmnibusEditorUtility.GetNodeInputPortRect(creatingWireEndEditorCell.Position, creatingWireEndInspectableType.Inputs[creatingWireEndPort].PortCenterFromTopLeft).center;
                        if (!CreatingWireHoveringEndPoint || CreatingWireEndPoint != endPoint)
                        {
                            CreatingWireHoveringEndPoint = true;
                            CreatingWireEndPoint = endPoint;
                            Repaint();
                        }
                    }
                    else
                    {
                        var wire = new EditorWire
                        {
                            Register = -1,
                            ReadCell = CreatingWireStartCell,
                            ReadField = CreatingWireStartPortName,
                            WriteCell = creatingWireEndEditorCell,
                            WriteField = creatingWireEndInspectableType.Inputs[creatingWireEndPort].Name,
                        };
                        CreatingWireStartCell.Outputs.Add(wire);
                        creatingWireEndEditorCell.Inputs.Add(wire);
                        _wires.Add (wire);
                        IsCreatingWire = false;
                        shouldWrite = true;
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
            OmnibusEditorUtility.DrawEdge(
                new ControlPoint {Point = from},
                new ControlPoint {Point = to},
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
            GUI.BeginGroup(new Rect(0f, OmnibusEditorUtility.editorWindowTabHeight, Screen.width, Screen.height));
        }

        //-------------------------------------------------
        // Context menu
        // Do this outside of the rescaled context
        // otherwise positioning doesn't work correctly.
        //-------------------------------------------------
        if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
        {
            Event.current.Use();
            var menu = new GenericMenu ();

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
                    menu.AddItem (new GUIContent ("New Cell/"+type.Name), false, CreateCellMenuFunction, type);
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
                            menu.AddItem (new GUIContent ("Add/"+type.Name), false, AddScriptMenuFunction, new object[]{editorState, type});
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
                            menu.AddItem (new GUIContent ("Remove/"+type.Name), false, RemoveScriptMenuFunction, new object[]{editorState, type});
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
                menu.AddItem(new GUIContent("And &&&&"), currentType == EditorTransitionExpressionType.And, SetTransitionExpressionType, new object[]{pickedEditorTransitionExpression, EditorTransitionExpressionType.And});
                menu.AddItem(new GUIContent("Or ||"), currentType == EditorTransitionExpressionType.Or, SetTransitionExpressionType, new object[]{pickedEditorTransitionExpression, EditorTransitionExpressionType.Or});
                menu.AddItem(new GUIContent("True"), currentType == EditorTransitionExpressionType.True, SetTransitionExpressionType, new object[]{pickedEditorTransitionExpression, EditorTransitionExpressionType.True});
                menu.AddItem(new GUIContent("False"), currentType == EditorTransitionExpressionType.False, SetTransitionExpressionType, new object[]{pickedEditorTransitionExpression, EditorTransitionExpressionType.False});
                menu.AddSeparator("");
                for (int i = 0; i < triggers.Length; ++i)
                {
                    var value = (Trigger)triggers.GetValue(i);
                    if ((int)value >= (int)Trigger.__COUNT__)
                    {
                        continue;
                    }
                    menu.AddItem(new GUIContent(value.ToString().Replace('/', '_')), currentType == EditorTransitionExpressionType.Trigger && value == pickedEditorTransitionExpression.Expression.Trigger, SetTransitionExpressionType, new object[]{pickedEditorTransitionExpression, EditorTransitionExpressionType.Trigger, value});
                }
            }
            else if (OmnibusEditorUtility.GetNodeTitleRect(editorCell.Position).Contains(mouseGraphPosition))
            {
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Delete Cell"), false, DeleteEditorCellMenuFunction, editorCell);
            }
            menu.ShowAsContext ();
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

        if (shouldWrite)
        {
            Write();
            Repaint();
        }
    }

    void removeTransitionAt(EditorTransitionIndex indexToRemove)
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

    void removeTransitionsAt(List<EditorTransitionIndex> indicesToRemove)
    {
        for (int i = 0; i < indicesToRemove.Count; ++i)
        {
            removeTransitionAt(indicesToRemove[i]);
        }
    }

    private Vector2 _menuFunctionPosition;
    void CreateCellMenuFunction (object typeParam)
    {
        Type type = typeParam as Type;
        var editorCell = new EditorCell();
        editorCell.Name = type.Name;
        editorCell.Cell = Activator.CreateInstance(type) as Cell;
        editorCell.Position = OmnibusEditorUtility.SnapToGrid(new Rect(_menuFunctionPosition, new Vector2 (EditorGUIUtility.labelWidth * 2 + EditorGUIUtility.fieldWidth, 100)));
        editorCell.Index = (EditorCellIndex)_cells.Count;
        _cells.Add(editorCell);
        Write();
    }

    void DeleteEditorCellMenuFunction(object editorCellParam)
    {
        var editorCell = editorCellParam as EditorCell;
        foreach (var input in editorCell.Inputs)
        {
            input.ReadCell.Outputs.Remove(input);
            _wires.Remove(input);
        }
        foreach (var output in editorCell.Outputs)
        {
            output.WriteCell.Inputs.Remove(output);
            _wires.Remove(output);
        }
        _cells.Remove(editorCell);
        Write();
    }

    void SetTransitionExpressionType(object exprTypeTrigger)
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

        Write();
    }

    void AddSubexpressionToTransitionExpression(object pickedEditorTransitionExpressionParam)
    {
        var pickedEditorTransitionExpression = pickedEditorTransitionExpressionParam as PickedEditorTransitionExpression;
        Debug.Assert(HasSubexpressionsAttribute.IsFoundOn(pickedEditorTransitionExpression.Expression.Type));
        pickedEditorTransitionExpression.Expression.Subexpressions.Add(new EditorTransitionExpression()
        {
            Type = EditorTransitionExpressionType.True,
        });
        Write();
    }

    void DeleteTransitionExpression(object pickedEditorTransitionExpressionParam)
    {
        var pickedEditorTransitionExpression = pickedEditorTransitionExpressionParam as PickedEditorTransitionExpression;
        pickedEditorTransitionExpression.Parent.Subexpressions.RemoveAt(pickedEditorTransitionExpression.IndexInParent);
        Write();
    }

    void CreateStateMenuFunction (object param)
    {
        var type = (EditorSpecialStateType)param;
        var editorState = new EditorState();
        switch(type)
        {
            case EditorSpecialStateType.Normal:     editorState.Name = "State"; break;
            case EditorSpecialStateType.LayerEnter: editorState.Name = "Enter"; break;
            case EditorSpecialStateType.LayerExit:  editorState.Name = "Exit"; break;
            case EditorSpecialStateType.LayerAny:   editorState.Name = "Any"; break;
        }
        editorState.SpecialState = type;
        editorState.Position = OmnibusEditorUtility.SnapToGrid(new Rect(_menuFunctionPosition, new Vector2 (EditorGUIUtility.labelWidth * 2 + EditorGUIUtility.fieldWidth, 100)));
        editorState.Index = (EditorStateIndex)_states.Count;
        _states.Add(editorState);
        Write();
    }

    void DeleteEditorStateMenuFunction(object editorStateParam)
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


        Write();
    }

    void CreateTransitionMenuFunction(object editorStateParam)
    {
        var editorState = (EditorState)editorStateParam;
        _draggable = DragTransition(editorState);
        _shouldDrag = true;
        wantsMouseMove = true;
    }

    void AddScriptMenuFunction (object paramsArray)
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
        Write();
    }

    void RemoveScriptMenuFunction (object paramsArray)
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
                Write();
                return;
            }
        }
        Debug.LogError("trying to remove a script type that doesn't exist");
    }


    void AddWindowRectToScrollArea ()
    {
        Vector2 extraMin = Vector2.Max (Vector2.zero, _scrollPosition);
        Vector2 extraMax = Vector2.Max (Vector2.zero, this.position.size - (_scrollPosition + _scrollSize));
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

    Vector2 WindowToGraphPosition (Vector2 windowPosition)
    {
        return windowPosition - _scrollPositionOnMouseDown - _scrollAnchor;
    }

    Rect GraphToWindowPosition (Rect rect)
    {
        // this is wrong but I don't know how to fix right now for drawing cursor rects, it seems like the cursor thingy
        // automatically compensates for the gui matrix in some way
        return rect;
    }
}


}