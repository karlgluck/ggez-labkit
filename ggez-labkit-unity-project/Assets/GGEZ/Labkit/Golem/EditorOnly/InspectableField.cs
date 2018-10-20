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

    //-----------------------------------------------------------------------------
    // InspectableField
    //-----------------------------------------------------------------------------
    public struct InspectableField
    {
        public readonly InspectableType InspectableType;
        public readonly Type SpecificType;
        public readonly FieldInfo FieldInfo;
        public readonly bool WantsSetting;
        public readonly bool CanBeNull;

        public InspectableField(InspectableType inspectableType, Type specificType, FieldInfo fieldInfo, bool wantsSetting, bool canBeNull)
        {
            InspectableType = inspectableType;
            SpecificType = specificType;
            FieldInfo = fieldInfo;
            WantsSetting = wantsSetting;
            CanBeNull = canBeNull;
        }

        // private static Dictionary<Type, InspectableField[]> s_typeToInspectableFields = new Dictionary<Type, InspectableField[]>();
        public static InspectableField[] GetInspectableFields(Type targetType)
        {
            InspectableField[] retval;
            // if (s_typeToInspectableFields.TryGetValue(targetType, out retval))
            // {
            //     return retval;
            // }

            FieldInfo[] fields = targetType.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
            retval = new InspectableField[fields.Length];
            int j = 0;
            for (int i = 0; i < fields.Length; ++i)
            {
                bool isInputOrOutput = fields[i].IsDefined(typeof(InAttribute), true) || fields[i].IsDefined(typeof(OutAttribute), true);
                if (isInputOrOutput)
                {
                    continue;
                }

                InspectableType inspectableType = InspectableTypeExt.GetInspectableTypeOf(fields[i].FieldType);
                if (inspectableType == InspectableType.Invalid)
                {
                    continue;
                }

                bool wantsSetting = fields[i].IsDefined(typeof(SettingAttribute), true);
                bool canBeNull = fields[i].IsDefined(typeof(CanBeNullAttribute), true);
                Type specificType = InspectableTypeExt.GetSpecificType(inspectableType, fields[i]);
                retval[j++] = new InspectableField(inspectableType, specificType, fields[i], wantsSetting, canBeNull);
            }
            Array.Resize(ref retval, j);
            // s_typeToInspectableFields.Add(targetType, retval);
            return retval;
        }
    }

}

#endif
