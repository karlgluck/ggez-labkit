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
    // InspectableAspectType
    //-----------------------------------------------------------------------------
    public class InspectableAspectType
    {
        public readonly string Name;
        public readonly Field[] Fields;
        public readonly Variable[] Variables;

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

        public struct Variable
        {
            public readonly FieldInfo FieldInfo;

            public Variable(FieldInfo fieldInfo)
            {
                FieldInfo = fieldInfo;
            }
        }

        public InspectableAspectType(string name, Field[] fields, Variable[] variables)
        {
            Name = name;
            Fields = fields;
            Variables = variables;
        }

        private static Dictionary<Type, InspectableAspectType> s_typeToInspectableType = new Dictionary<Type, InspectableAspectType>();
        public static InspectableAspectType GetInspectableAspectType(Type aspectType)
        {
            Debug.Assert(typeof(Aspect).IsAssignableFrom(aspectType));

            InspectableAspectType retval;
            if (s_typeToInspectableType.TryGetValue(aspectType, out retval))
            {
                return retval;
            }

            var fields = aspectType.GetFields(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public).Where(f => !typeof(IVariable).IsAssignableFrom(f.FieldType)).ToArray();
            var returnedFields = new Field[fields.Length];
            {
                int j = 0;
                for (int i = 0; i < fields.Length; ++i)
                {
                    Debug.Assert(!fields[i].FieldType.IsDefined(typeof(InAttribute), false));
                    Debug.Assert(!fields[i].FieldType.IsDefined(typeof(OutAttribute), false));

                    var inspectableType = InspectableTypeExt.GetInspectableTypeOf(fields[i].FieldType);
                    if (inspectableType != InspectableType.Invalid)
                    {
                        var specificType = InspectableTypeExt.GetSpecificType(inspectableType, fields[i]);
                        bool wantsSetting = fields[i].IsDefined(typeof(SettingAttribute), true);
                        bool canBeNull = fields[i].IsDefined(typeof(CanBeNullAttribute), true);
                        returnedFields[j++] = new Field(inspectableType, specificType, fields[i], wantsSetting, canBeNull);
                    }
                }
                Array.Resize(ref returnedFields, j);
            }

            var variables = aspectType.GetFields(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public).Where(f => typeof(IVariable).IsAssignableFrom(f.FieldType)).ToArray();
            var returnedVariables = new Variable[variables.Length];
            {
                int j = 0;
                for (int i = 0; i < variables.Length; ++i)
                {
                    returnedVariables[j++] = new Variable(variables[i]);
                }
                Array.Resize(ref returnedVariables, j);
            }

            retval = new InspectableAspectType(aspectType.Name, returnedFields, returnedVariables);
            s_typeToInspectableType.Add(aspectType, retval);
            return retval;
        }
    }

}

#endif
