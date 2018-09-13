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

using System.Collections.Generic;
using System;

namespace GGEZ.Labkit
{
    //-------------------------------------------------------------------------
    // Settings
    //
    // Like Variables except they cannot be changed at runtime. Can inherit
    // values from other Settings objects.
    //-------------------------------------------------------------------------
    public class Settings
    {
        public SettingsAsset InheritFrom;

        /// All of the values in this Settings object. TODO: valuetypes as separate dictionaries.
        public Dictionary<string, object> Values = new Dictionary<string, object>();

        public IEnumerable<string> Keys ()
        {
            foreach (var key in Values.Keys)
            {
                yield return key;
            }
            if (InheritFrom != null)
            {
                foreach (var key in InheritFrom.Settings.Keys())
                {
                    yield return key;
                }
            }
        }

#if UNITY_EDITOR

        // InspectorGet that does not set a default value
        public object InspectorGet(string name, Type type)
        {
            object retval;
            if (Values.TryGetValue(name, out retval) && type.IsAssignableFrom(retval.GetType()))
            {
                return retval;
            }
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }

        // InspectorGet that sets a default value if it doesn't exist
        public object InspectorGet(string name, Type type, object defaultValue)
        {
            object retval;
            if (Values.TryGetValue(name, out retval) && type.IsAssignableFrom(retval.GetType()))
            {
                return retval;
            }
            Values[name] = defaultValue;
            return defaultValue;
        }

        public void InspectorSet(string name, Type type, object value)
        {
            Values[name] = value;
        }

#endif

        public object Get(string name)
        {
            object value;
            if (!Values.TryGetValue(name, out value) && InheritFrom != null)
            {
                return InheritFrom.Settings.Get(name);
            }
            return value;
        }

        public object Get(string name, Type type)
        {
            if (name != null)
            {
                object value;
                if (!Values.TryGetValue(name, out value) && InheritFrom != null)
                {
                    return InheritFrom.Settings.Get(name, type);
                }
            }
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            else
            {
                return null;
            }
        }
    }
}
