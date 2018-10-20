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
    // InspectableDictionaryKeyValuePair
    //-----------------------------------------------------------------------------
    [Obsolete]
    public struct InspectableDictionaryKeyValuePair
    {
        public readonly InspectableType Type;
        public readonly string Key;
        public readonly object Value;

        public InspectableDictionaryKeyValuePair(InspectableType type, string key, object value)
        {
            Type = type;
            Key = key;
            Value = value;
        }

        public static InspectableDictionaryKeyValuePair[] GetDictionaryKeyValuePairs(Dictionary<string, object> target)
        {
            var retval = new InspectableDictionaryKeyValuePair[target.Count];
            int j = 0;
            foreach (var kvp in target)
            {
                object value = kvp.Value;
                Type valueType = value.GetType();
                var inspectableType = InspectableTypeExt.GetInspectableTypeOf(valueType);
                if (inspectableType != InspectableType.Invalid)
                {
                    retval[j++] = new InspectableDictionaryKeyValuePair(inspectableType, kvp.Key, value);
                }
            }
            Array.Resize(ref retval, j);
            return retval;
        }
    }

}

#endif
