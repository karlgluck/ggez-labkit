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

    //-----------------------------------------------------------------------------
    // InspectableScriptType
    //-----------------------------------------------------------------------------
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

#endif
