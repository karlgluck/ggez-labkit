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

    public class EditorVariableInputRegister
    {
        public static readonly Vector2 PortCenterFromTopRight = new Vector2(-GolemEditorUtility.GridSize + GolemEditorUtility.NodeLeftRightMargin, EditorGUIUtility.singleLineHeight * (0.5f + 0));

        public VariableRef Variable;
        public Rect Position;

        public List<EditorWire> OutputWires = new List<EditorWire>();

        public IEnumerable<EditorWire> GetAllOutputWires()
        {
            return OutputWires;
        }
        public bool HasOutputWire()
        {
            return OutputWires.Count > 0;
        }

        public void AddOutputWire(EditorWire outputWire)
        {
            Debug.Log("AddOutputWire " + outputWire.GetHashCode());
            Debug.Assert(outputWire.ReadScript == null);
            Debug.Assert(outputWire.ReadCell == null);
            Debug.Assert(outputWire.ReadVariableInputRegister == this);
            OutputWires.Add(outputWire);
        }

        public void RemoveOutputWire(EditorWire wire)
        {
            OutputWires.Remove(wire);
        }

        public IDraggable DragPosition()
        {
            return new DraggablePosition() { EditorVariableRegister = this, Position = Position.position };
        }
        private class DraggablePosition : IDraggable
        {
            public EditorVariableInputRegister EditorVariableRegister;
            public Vector2 Position;
            public Vector2 Offset
            {
                set { EditorVariableRegister.Position.position = GolemEditorUtility.SnapToGrid(Position + value); }
            }
        }
    }

}

#endif
