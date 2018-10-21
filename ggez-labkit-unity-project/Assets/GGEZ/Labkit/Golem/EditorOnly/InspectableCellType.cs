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
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;

namespace GGEZ.Labkit
{

    //-----------------------------------------------------------------------------
    // InspectableCellType
    //-----------------------------------------------------------------------------
    public class InspectableCellType
    {
        public struct Input
        {
            public string Name { get { return Property.Name; } }
            public Type Type { get { return Property.PropertyType; } }
            public readonly PropertyInfo Property;
            public readonly Vector2 PortCenterFromTopLeft;
            public readonly bool CanBeNull;

            public Input(PropertyInfo property, Vector2 portCenterFromTopLeft, bool canBeNull)
            {
                Property = property;
                PortCenterFromTopLeft = portCenterFromTopLeft;
                CanBeNull = canBeNull;
            }
        }

        public struct Output
        {
            public string Name { get { return Property.Name; } }
            public Type Type { get { return Property.PropertyType; } }
            public readonly PropertyInfo Property;
            public readonly Vector2 PortCenterFromTopRight;
            public readonly bool CanBeNull;

            public Output(PropertyInfo property, Vector2 portCenterFromTopRight, bool canBeNull)
            {
                Property = property;
                PortCenterFromTopRight = portCenterFromTopRight;
                CanBeNull = canBeNull;
            }
        }

        public struct Field
        {
            public readonly InspectableType Type;
            public readonly Type SpecificType;
            public readonly FieldInfo FieldInfo;
            public readonly bool WantSetting;
            public readonly bool CanBeNull;

            public Field(InspectableType type, Type targetType, FieldInfo fieldInfo, bool wantsSetting, bool canBeNull)
            {
                Type = type;
                SpecificType = targetType;
                FieldInfo = fieldInfo;
                WantSetting = wantsSetting;
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

        public Type GetInputType(string name)
        {
            for (int i = 0; i < Inputs.Length; ++i)
            {
                if (Inputs[i].Name.Equals(name))
                {
                    return Inputs[i].Type;
                }
            }
            Debug.Assert(false, "Tried to get an input that doesn't exist");
            return null;
        }

        public bool GetInputCanBeNull(string name)
        {
            for (int i = 0; i < Inputs.Length; ++i)
            {
                if (Inputs[i].Name.Equals(name))
                {
                    return Inputs[i].CanBeNull;
                }
            }
            Debug.Assert(false, "Tried to get an input that doesn't exist");
            return true;
        }

        private static Dictionary<Type, InspectableCellType> s_typeToInspectableType = new Dictionary<Type, InspectableCellType>();
        public static InspectableCellType GetInspectableCellType(Type cellType)
        {
            Debug.Assert(typeof(Cell).IsAssignableFrom(cellType));

            InspectableCellType retval;
            if (s_typeToInspectableType.TryGetValue(cellType, out retval))
            {
                return retval;
            }

            var inputs = cellType.GetProperties(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public)
                                 .Where((f) => f.Name.Equals("Input") || f.Name.EndsWith("In")).ToArray();
            var returnedInputs = new Input[inputs.Length];
            for (int i = 0; i < inputs.Length; ++i)
            {
                var portCenter = new Vector2(GolemEditorUtility.GridSize - GolemEditorUtility.NodeLeftRightMargin, EditorGUIUtility.singleLineHeight * (0.5f + i));
                bool canBeNull = inputs[i].IsDefined(typeof(CanBeNullAttribute), true);
                returnedInputs[i] = new Input(inputs[i], portCenter, canBeNull);
            }

            var outputs = cellType.GetProperties(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public)
                                  .Where((f) => f.Name.Equals("Output") || f.Name.EndsWith("Out")).ToArray();
            var returnedOutputs = new Output[outputs.Length];
            for (int i = 0; i < outputs.Length; ++i)
            {
                var portCenter = new Vector2(-GolemEditorUtility.GridSize + GolemEditorUtility.NodeLeftRightMargin, EditorGUIUtility.singleLineHeight * (0.5f + i));
                bool canBeNull = outputs[i].IsDefined(typeof(CanBeNullAttribute), true);
                returnedOutputs[i] = new Output(outputs[i], portCenter, canBeNull);
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
                        bool wantsSetting = fields[i].IsDefined(typeof(SettingAttribute), true);
                        bool canBeNull = fields[i].IsDefined(typeof(CanBeNullAttribute), true);
                        returnedFields[j++] = new Field(inspectableType, targetType, fields[i], wantsSetting, canBeNull);
                    }
                }
                Array.Resize(ref returnedFields, j);
            }

            retval = new InspectableCellType(returnedInputs, returnedOutputs, returnedFields);
            s_typeToInspectableType.Add(cellType, retval);
            return retval;
        }

    }

}

#endif
