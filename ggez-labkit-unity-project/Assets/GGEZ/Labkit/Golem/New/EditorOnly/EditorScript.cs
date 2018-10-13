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
using GGEZ.FullSerializer;

#if UNITY_EDITOR
using UnityEditor;

namespace GGEZ.Labkit
{
    [fsSerializeEnumAsInteger]
    public enum ScriptIndex : int { Invalid = int.MaxValue }

    //-------------------------------------------------------------------------
    // EditorScript
    //-------------------------------------------------------------------------
    public class EditorScript
    {
        #warning Enabled flag isn't exposed in the editor
        public bool Enabled = true;
        public Script Script;
        /// <summary>The compiled index of the script</summary>
        public ScriptIndex CompiledIndex;
        public Dictionary<string, string> FieldsUsingSettings = new Dictionary<string,string>();
        public Dictionary<string, VariableRef> FieldsUsingVariables = new Dictionary<string,VariableRef>();
        public Dictionary<string, List<EditorWire>> OutputWires = new Dictionary<string, List<EditorWire>>();

        /// <summary>Which state contains this script</summary>
        public EditorState State;

        /// <remarks>Position in graph, not within State</remarks>
        public Rect Position;

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
        public bool HasOutputWire(string name)
        {
            return OutputWires.ContainsKey(name);
        }

        public void AddOutputWire(EditorWire outputWire)
        {
            Debug.Assert(outputWire.ReadScript == this);
            Debug.Assert(outputWire.ReadCell == null);
            Debug.Assert(outputWire.ReadVariableInputRegister == null);
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

    }

}

#endif
