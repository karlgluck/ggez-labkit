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
using System.Linq;

#if UNITY_EDITOR

namespace GGEZ.Labkit
{

    //-----------------------------------------------------------------------------
    // InspectableCellType
    //-----------------------------------------------------------------------------
    public class InspectableCellType
    {
        public struct Input
        {
            public readonly string Name;
            public readonly Type Type;
            public readonly FieldInfo Field;
            public readonly Vector2 PortCenterFromTopLeft;
            public readonly bool CanBeNull;

            public Input(string name, Type type, FieldInfo field, Vector2 portCenterFromTopLeft, bool canBeNull)
            {
                Name = name;
                Type = type;
                Field = field;
                PortCenterFromTopLeft = portCenterFromTopLeft;
                CanBeNull = canBeNull;
            }
        }

        public struct Output
        {
            public readonly string Name;
            public readonly Type Type;
            public readonly FieldInfo Field;
            public readonly Vector2 PortCenterFromTopRight;
            public readonly bool CanBeNull;

            public Output(string name, Type type, FieldInfo field, Vector2 portCenterFromTopRight, bool canBeNull)
            {
                Name = name;
                Type = type;
                Field = field;
                PortCenterFromTopRight = portCenterFromTopRight;
                CanBeNull = canBeNull;
            }
        }

        public struct Field
        {
            public readonly InspectableType Type;
            public readonly Type SpecificType;
            public readonly FieldInfo FieldInfo;
            public readonly bool CanBeNull;

            public Field(InspectableType type, Type targetType, FieldInfo fieldInfo, bool canBeNull)
            {
                Type = type;
                SpecificType = targetType;
                FieldInfo = fieldInfo;
                CanBeNull = canBeNull;
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

        public HashSet<string> GetUnnullableInputs()
        {
            throw new NotImplementedException();
        }

        public HashSet<string> GetUnnullableOutputs()
        {
            throw new NotImplementedException();
        }

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
                bool canBeNull = CanBeNullAttribute.IsAppliedTo(inputs[i]);
                returnedInputs[i] = new Input(inputs[i].Name, inAttribute.Type, inputs[i], portCenter, canBeNull);
            }

            var outputs = cellType.GetFields(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public).Where((f) => f.IsDefined(typeof(OutAttribute), false)).ToArray();
            var returnedOutputs = new Output[outputs.Length];
            for (int i = 0; i < outputs.Length; ++i)
            {
                var outAttribute = outputs[i].GetCustomAttributes(typeof(OutAttribute), false)[0] as OutAttribute;
                var portCenter = new Vector2(-GolemEditorUtility.GridSize + GolemEditorUtility.NodeLeftRightMargin, EditorGUIUtility.singleLineHeight * (0.5f + i));
                bool canBeNull = CanBeNullAttribute.IsAppliedTo(outputs[i]);
                returnedOutputs[i] = new Output(outputs[i].Name, outAttribute.Type, outputs[i], portCenter, canBeNull);
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
                        bool canBeNull = CanBeNullAttribute.IsAppliedTo(fields[i]);
                        returnedFields[j++] = new Field(inspectableType, targetType, fields[i], canBeNull);
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

}

#endif
