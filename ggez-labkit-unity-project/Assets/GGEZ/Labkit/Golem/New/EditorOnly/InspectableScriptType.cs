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
        public readonly Output[] Outputs;

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

        public struct Output
        {
            public string Name { get { return Field.Name; } }
            public Type Type { get { return Field.FieldType; } }
            public readonly FieldInfo Field;
            public readonly Vector2 PortCenterFromTopRight;
            public readonly bool CanBeNull;

            public Output(FieldInfo field, Vector2 portCenterFromTopRight, bool canBeNull)
            {
                Field = field;
                PortCenterFromTopRight = portCenterFromTopRight;
                CanBeNull = canBeNull;
            }
        }

        public InspectableScriptType(string name, Field[] fields, Output[] outputs)
        {
            Name = name;
            Fields = fields;
            Outputs = outputs;
        }

        private static Dictionary<Type, InspectableScriptType> s_typeToInspectableType = new Dictionary<Type, InspectableScriptType>();
        public static InspectableScriptType GetInspectableScriptType(Type scriptType)
        {
            Debug.Assert(typeof(Script).IsAssignableFrom(scriptType));

            InspectableScriptType retval;
            if (s_typeToInspectableType.TryGetValue(scriptType, out retval))
            {
                return retval;
            }

            FieldInfo[] fields = scriptType.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
            var returnedFields = new Field[fields.Length];
            int j = 0;
            List<Output> outputs = new List<Output>();
            for (int i = 0; i < fields.Length; ++i)
            {
                #warning these really shouldn't be assertions since the coder can mess this up and it doesn't really matter
                Debug.Assert(!fields[i].IsDefined(typeof(InAttribute), true));

                InspectableType inspectableType = InspectableTypeExt.GetInspectableTypeOf(fields[i].FieldType);
                if (inspectableType == InspectableType.Invalid)
                {
                    continue;
                }

                bool canBeNull = fields[i].IsDefined(typeof(CanBeNullAttribute), true);

                if (fields[i].IsDefined(typeof(OutAttribute), true))
                {
                    Debug.Assert(typeof(IVariable).IsAssignableFrom(fields[i].FieldType));
                    var portCenter = new Vector2(-GolemEditorUtility.GridSize + GolemEditorUtility.NodeLeftRightMargin, EditorGUIUtility.singleLineHeight * (0.5f + i));
                    #warning make this into an array
                    outputs.Add(new Output(fields[i], portCenter, canBeNull));
                }

                bool wantsSetting = fields[i].IsDefined(typeof(SettingAttribute), true);
                Type specificType = InspectableTypeExt.GetSpecificType(inspectableType, fields[i]);
                returnedFields[j++] = new Field(inspectableType, specificType, fields[i], wantsSetting, canBeNull);
            }

            retval = new InspectableScriptType(scriptType.Name, returnedFields, outputs.ToArray());
            s_typeToInspectableType.Add(scriptType, retval);
            return retval;
        }
    }

}

#endif
