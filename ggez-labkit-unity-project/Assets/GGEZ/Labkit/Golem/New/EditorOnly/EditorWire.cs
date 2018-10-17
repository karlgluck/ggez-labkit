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
    public enum EditorWireIndex : int { Invalid = int.MaxValue }

    //-------------------------------------------------------------------------
    // EditorWire
    //-------------------------------------------------------------------------
    public class EditorWire
    {
        public EditorWireIndex Index = EditorWireIndex.Invalid;

        public RegisterPtr Register; // needed for live debug view

        public EditorCell ReadCell;
        public EditorScript ReadScript;
        public string ReadField;
        public EditorCell WriteCell;
        public string WriteField;

        public IGraphObjectWithOutputs ReadObject;
        public IGraphObjectWithInputs WriteObject;

        public Vector2 ReadPoint, WritePoint;

        // Other than cells, a wire can also read a register that is
        // filled with a variable's value.
        public EditorVariableInputRegister ReadVariableInputRegister;
    }

}

#endif
