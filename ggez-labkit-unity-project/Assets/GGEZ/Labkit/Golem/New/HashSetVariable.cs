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
using System.Collections.Generic;

namespace GGEZ.Labkit
{
    [GGEZ.FullSerializer.fsIgnore]
    public class HashSetVariable<T> : IVariable
    {
        /// <summary>The backing register for this variable</summary>
        private HashSetRegister<T> _register;

        // Values to roll into the register
        private HashSet<T> _added = new HashSet<T>();
        private HashSet<T> _removed = new HashSet<T>();

        /// <summary>Values contained in the collection in the last program phase or the current circuit phase</summary>
        public HashSet<T> Values { get { return _register.Values; } }

        /// <summary>Values added to the collection in the last program phase or the current circuit phase</summary>
        public HashSet<T> Added { get { return _register.Added; } }

        /// <summary>Values removed from the collection in the last program phase or the current circuit phase</summary>
        public HashSet<T> Removed { get { return _register.Removed; } }

        /// <summary>Create a variable with a new backing register</summary>
        public HashSetVariable()
        {
            _register = new HashSetRegister<T>();
        }

        /// <summary>Create a variable for the given backing register</summary>
        public HashSetVariable(HashSetRegister<T> register)
        {
            _register = register;
        }

        /// <summary>The backing register for this variable</summary>
        public IRegister GetRegister()
        {
            return _register;
        }

        /// <summary>Updates the value of the register that backs this variable</summary>
        public void OnEndProgramPhase()
        {
            _register.Add(_added);
            _register.Remove(_removed);
            _added.Clear();
            _removed.Clear();
        }

        public bool Add(T element)
        {
            _removed.Remove(element);
            return _added.Add(element);
        }

        public void Add(IEnumerable<T> elements)
        {
            foreach (T element in elements)
            {
                Add(element);
            }
        }

        public bool Remove(T element)
        {
            _added.Remove(element);
            return _removed.Add(element);
        }

        public void Remove(IEnumerable<T> elements)
        {
            foreach (T element in elements)
            {
                Remove(element);
            }
        }
    }
}
