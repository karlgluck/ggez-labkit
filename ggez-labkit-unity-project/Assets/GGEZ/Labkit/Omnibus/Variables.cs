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

namespace GGEZ.Omnibus
{
    public class Variables
    {
        // This set is for propagating changes in variable values
        public HashSet<string> Changed = new HashSet<string>();

        // This set is for waking up state machines that are
        // sleeping on variables in order to transition.
        public HashSet<string> ChangedLastFrame = new HashSet<string>();

        public Dictionary<string, object> Values = new Dictionary<string, object>();
        public Dictionary<string, object> NextFrameValues = new Dictionary<string, object>();

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
            NextFrameValues[name] = defaultValue;
            return defaultValue;
        }

        public void InspectorSet(string name, Type type, object value)
        {
            Values[name] = value;
            NextFrameValues[name] = value;
        }
#endif

        public void Set(string name, object value)
        {
            object currentNextFrame;
            bool different =
                !Values.TryGetValue(name, out currentNextFrame)
                || !object.Equals(currentNextFrame, value);
            NextFrameValues[name] = value;
            if (different)
            {
                Changed.Add(name);
            }
        }

        public object Get(string name)
        {
            object value;
            Values.TryGetValue(name, out value);
            return value;
        }

        public bool Get(string name, ref float value)
        {
            object objValue;
            if (Values.TryGetValue(name, out objValue))
            {
                value = (float)objValue;
                return true;
            }
            return false;
        }

        public void EndFrame()
        {
            Changed.Clear();
            foreach (var kvp in NextFrameValues)
            {
                Values[kvp.Key] = kvp.Value;
            }
        }
    }
}
