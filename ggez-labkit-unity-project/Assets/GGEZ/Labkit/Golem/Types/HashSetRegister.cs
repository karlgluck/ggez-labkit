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
using System.Collections.Generic;

namespace GGEZ.Labkit
{
    [GGEZ.FullSerializer.fsIgnore]
    public sealed class HashSetRegister<T> : CollectionRegister<T>
    {
        private HashSet<T> _values = new HashSet<T>();
        private HashSet<T> _added = new HashSet<T>();
        private HashSet<T> _removed = new HashSet<T>();

        public HashSet<T> Values { get { return _values; } }
        public HashSet<T> Added { get { return _added; } }
        public HashSet<T> Removed { get { return _removed; } }

        protected override ICollection<T> ValuesCollection { get { return _values; } }
        protected override ICollection<T> AddedCollection { get { return _added; } }
        protected override ICollection<T> RemovedCollection { get { return _removed; } }

        public override Variable CreateVariable()
        {
            #warning TODO make sure only up to 1 variable is ever created for each register
            return new HashSetVariable<T>(this);
        }

        public override string ToString()
        {
            string retval = "HashSet {";
            string separator = "";
            foreach (T value in _values)
            {
                retval += separator + value.ToString();
                separator = ", ";
            }
            retval += "} = <prev> + {";
            separator = "";
            foreach (T value in _added)
            {
                retval += separator + value.ToString();
                separator = ", ";
            }
            retval += "} - {";
            separator = "";
            foreach (T value in _removed)
            {
                retval += separator + value.ToString();
                separator = ", ";
            }
            retval += "}";
            return retval;
        }

    }
}
