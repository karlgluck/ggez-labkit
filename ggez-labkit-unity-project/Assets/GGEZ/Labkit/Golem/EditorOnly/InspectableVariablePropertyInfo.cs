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
    // InspectableVariablePropertyInfo
    //-----------------------------------------------------------------------------
    [Obsolete]
    public struct InspectableVariablePropertyInfo
    {
        public readonly InspectableType Type;
        public readonly PropertyInfo PropertyInfo;
        public readonly VariableAttribute VariableAttribute;

        public InspectableVariablePropertyInfo(InspectableType type, PropertyInfo propertyInfo, VariableAttribute variableAttribute)
        {
            Type = type;
            PropertyInfo = propertyInfo;
            VariableAttribute = variableAttribute;
        }

        public static InspectableVariablePropertyInfo[] GetVariableProperties(object target)
        {
            var properties = target.GetType().GetProperties();
            var retval = new InspectableVariablePropertyInfo[properties.Length];
            int j = 0;
            for (int i = 0; i < properties.Length; ++i)
            {
                var attributes = properties[i].GetCustomAttributes(typeof(VariableAttribute), false);
                VariableAttribute variableAttribute = null;
                for (int k = 0; k < attributes.Length && variableAttribute == null; ++k)
                {
                    variableAttribute = attributes[k] as VariableAttribute;
                }
                if (variableAttribute == null)
                {
                    continue;
                }
                var inspectableType = InspectableTypeExt.GetInspectableTypeOf(properties[i].PropertyType);
                if (inspectableType != InspectableType.Invalid)
                {
                    retval[j++] = new InspectableVariablePropertyInfo(inspectableType, properties[i], attributes[0] as VariableAttribute);
                }
            }
            Array.Resize(ref retval, j);
            return retval;
        }
    }

}

#endif
