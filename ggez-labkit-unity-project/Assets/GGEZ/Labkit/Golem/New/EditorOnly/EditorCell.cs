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

using System;
using UnityEngine;
using UnityObject = UnityEngine.Object;
using UnityObjectList = System.Collections.Generic.List<UnityEngine.Object>;
using System.Collections.Generic;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;

namespace GGEZ.Labkit
{

    public enum EditorCellIndex : int { Invalid = int.MaxValue }

    //-------------------------------------------------------------------------
    // EditorCell
    //-------------------------------------------------------------------------
    public class EditorCell
    {
        public string Name;
        public EditorCellIndex Index = EditorCellIndex.Invalid;
        public Cell Cell;

        #warning Help decouple references by switching to using EditorCellIndex
        public Dictionary<string,EditorWire> InputWires = new Dictionary<string, EditorWire>();
        public Dictionary<string,List<EditorWire>> OutputWires = new Dictionary<string, List<EditorWire>>();

        public Rect Position;
        public Dictionary<string,string> FieldsUsingSettings = new Dictionary<string,string>();
        public Dictionary<string,VariableRef> FieldsUsingVariables = new Dictionary<string,VariableRef>();

        public IEnumerable<EditorWire> GetAllInputWires()
        {
            return InputWires.Values;
        }

        public IEnumerable<EditorWire> GetAllOutputWires()
        {
            foreach (List<EditorWire> list in OutputWires.Values)
            {
                foreach (EditorWire wire in list)
                {
                    yield return wire;
                }
            }
        }

        public bool HasInputWire(string name)
        {
            return InputWires.ContainsKey(name);
        }

        public bool HasOutputWire(string name)
        {
            return OutputWires.ContainsKey(name);
        }

        public void AddInputWire(EditorWire inputWire)
        {
            Debug.Assert(inputWire.WriteCell == this);
            InputWires.Add(inputWire.WriteField, inputWire);
        }

        public void RemoveInputWire(string name)
        {
            InputWires.Remove(name);
        }

        public void RemoveInputWire(EditorWire editorWire)
        {
            foreach (var kvp in InputWires)
            {
                if (kvp.Value == editorWire)
                {
                    InputWires.Remove(kvp.Key);
                    Debug.Assert(!new HashSet<EditorWire>(InputWires.Values).Contains(editorWire));
                    return;
                }
            }
        }

        public void AddOutputWire(EditorWire outputWire)
        {
            Debug.Assert(outputWire.ReadCell == this);
            OutputWires.MultiAdd(outputWire.ReadField, outputWire);
        }

        public void RemoveOutputWire(EditorWire editorWire)
        {
            foreach (var kvp in OutputWires)
            {
                if (kvp.Value.Remove(editorWire))
                {
                    if (kvp.Value.Count == 0)
                    {
                        OutputWires.Remove(kvp.Key);
                    }
                    return;
                }
            }
        }

        public IDraggable DragPosition()
        {
            return new DraggablePosition() { EditorCell = this, Position = Position.position };
        }
        private class DraggablePosition : IDraggable
        {
            public EditorCell EditorCell;
            public Vector2 Position;
            public Vector2 Offset
            {
                set { EditorCell.Position.position = GolemEditorUtility.SnapToGrid(Position + value); }
            }
        }

        public IDraggable DragSize()
        {
            return new DraggableSize() { EditorCell = this, Size = Position.size };
        }
        private class DraggableSize : IDraggable
        {
            public EditorCell EditorCell;
            public Vector2 Size;
            public Vector2 Offset
            {
                set { EditorCell.Position.size = GolemEditorUtility.SnapToGrid(Size + value); }
            }
        }
    }

}

#endif
