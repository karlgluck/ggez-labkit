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
using UnityEditor;
using UnityObject = UnityEngine.Object;
using UnityObjectList = System.Collections.Generic.List<UnityEngine.Object>;
using System.Collections.Generic;
using System.Reflection;

#if UNITY_EDITOR

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
        public List<EditorWire> Inputs = new List<EditorWire>();
        #warning // Make things more clear here by making inputs a dictionary and outputs a dictionary-list
        public List<EditorWire> Outputs = new List<EditorWire>();
        public Rect Position;
        public Dictionary<string,string> UnityObjectFields = new Dictionary<string,string>();
        public Dictionary<string,string> FieldsUsingSettings = new Dictionary<string,string>();
        public Dictionary<string,VariableRef> FieldsUsingVariables = new Dictionary<string,VariableRef>();

        public bool HasInputWire(string name)
        {
            for (int i = 0; i < Inputs.Count; ++i)
            {
                if (Inputs[i].WriteField == name)
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasOutputWire(string name)
        {
            for (int i = 0; i < Outputs.Count; ++i)
            {
                if (Outputs[i].ReadField == name)
                {
                    return true;
                }
            }
            return false;
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
