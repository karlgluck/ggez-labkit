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

using UnityEngine;
using System.Collections.Generic;

namespace GGEZ.Labkit
{

    public abstract class CollectionVariable<T> : Variable
    {
        protected abstract ICollection<T> VariableAddedCollection { get; }
        protected abstract ICollection<T> VariableRemovedCollection { get; }

        
        /// <summary>Updates the value of the register that backs this variable</summary>
        public override void OnVariableRolloverPhase()
        {
            CollectionRegister<T> register = GetRegister() as CollectionRegister<T>;
            Debug.Assert(register != null);

            ICollection<T> added = VariableAddedCollection;
            ICollection<T> removed = VariableRemovedCollection;

            register.AddRange(added);
            register.RemoveRange(removed);

            added.Clear();
            removed.Clear();
        }

        public void Add(T element)
        {
            QueueForRolloverPhase();

            ICollection<T> removed = VariableRemovedCollection;

            if (removed.Contains(element))
                removed.Remove(element);
            else
                VariableAddedCollection.Add(element);
        }

        public void AddRange(IEnumerable<T> elements)
        {
            QueueForRolloverPhase();
            
            ICollection<T> added = VariableAddedCollection;
            ICollection<T> removed = VariableRemovedCollection;

            foreach (T element in elements)
            {
                if (removed.Contains(element))
                    removed.Remove(element);
                else
                    added.Add(element);
            }
        }

        public void Remove(T element)
        {
            QueueForRolloverPhase();

            ICollection<T> added = VariableAddedCollection;

            if (added.Contains(element))
                added.Remove(element);
            else
                VariableRemovedCollection.Add(element);
        }

        public void RemoveRange(IEnumerable<T> elements)
        {
            QueueForRolloverPhase();
            
            ICollection<T> added = VariableAddedCollection;
            ICollection<T> removed = VariableRemovedCollection;

            foreach (T element in elements)
            {
                if (added.Contains(element))
                    added.Remove(element);
                else
                    removed.Add(element);
            }
        }
    }
}