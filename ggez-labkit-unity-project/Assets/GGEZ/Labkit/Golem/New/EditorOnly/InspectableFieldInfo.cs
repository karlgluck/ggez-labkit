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
    // InspectableFieldInfo
    //-----------------------------------------------------------------------------
    public struct InspectableFieldInfo
    {
        public readonly InspectableType InspectableType;
        public readonly Type SpecificType;
        public readonly FieldInfo FieldInfo;
        public readonly bool WantsSetting;

        public InspectableFieldInfo(InspectableType inspectableType, Type specificType, FieldInfo fieldInfo, bool wantsSetting)
        {
            InspectableType = inspectableType;
            SpecificType = specificType;
            FieldInfo = fieldInfo;
            WantsSetting = wantsSetting;
        }

        public static InspectableFieldInfo[] GetFields(object target)
        {
            FieldInfo[] fields = target.GetType().GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
            var retval = new InspectableFieldInfo[fields.Length];
            int j = 0;
            for (int i = 0; i < fields.Length; ++i)
            {
                InspectableType inspectableType = InspectableTypeExt.GetInspectableTypeOf(fields[i].FieldType);
                if (inspectableType == InspectableType.Invalid)
                {
                    continue;
                }
                bool wantsSetting = SettingAttribute.IsDeclaredOn(fields[i]);
                Type specificType = InspectableTypeExt.GetSpecificType(inspectableType, fields[i]);
                retval[j++] = new InspectableFieldInfo(inspectableType, specificType, fields[i], wantsSetting);
            }
            Array.Resize(ref retval, j);
            return retval;
        }
    }

}

#endif
