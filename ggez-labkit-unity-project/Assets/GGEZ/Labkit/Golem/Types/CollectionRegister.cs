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

namespace GGEZ.Labkit
{
    public interface ICollectionRegister
    {
        void OnCollectionRegisterPhase();
    }

    public abstract class CollectionRegister<T> : Register, ICollectionRegister
    {
        protected abstract ICollection<T> ValuesCollection { get; }
        protected abstract ICollection<T> AddedCollection { get; }
        protected abstract ICollection<T> RemovedCollection { get; }

        public void OnCollectionRegisterPhase()
        {
            AddedCollection.Clear();
            RemovedCollection.Clear();
        }

        protected void NotifyListenersAndQueueForCollectionRegisterPhase()
        {
            GolemManager.QueueForCollectionRegisterPhase(this);
            NotifyListeners();
        }

        public void Add(T element)
        {
            ValuesCollection.Add(element);

            ICollection<T> removed = RemovedCollection;
            if (removed.Contains(element))
                removed.Remove(element);
            else
                AddedCollection.Add(element);
                
            NotifyListenersAndQueueForCollectionRegisterPhase();
        }

        public void AddRange(IEnumerable<T> elements)
        {
            ICollection<T> values = ValuesCollection;
            ICollection<T> added = AddedCollection;
            ICollection<T> removed = RemovedCollection;

            foreach (T element in elements)
            {
                values.Add(element);

                if (removed.Contains(element))
                    removed.Remove(element);
                else
                    added.Add(element);
            }

            NotifyListenersAndQueueForCollectionRegisterPhase();
        }

        public void Clear()
        {
            if (ValuesCollection.Count == 0)
                return;

            ICollection<T> values = ValuesCollection;
            ICollection<T> added = AddedCollection;
            ICollection<T> removed = RemovedCollection;

            foreach (T element in values)
            {
                if (added.Contains(element))
                    added.Remove(element);
                else
                    removed.Add(element);
            }

            values.Clear();

            NotifyListenersAndQueueForCollectionRegisterPhase();
        }

        public bool Contains(T element)
        {
            return ValuesCollection.Contains(element);
        }

        public void Remove(T element)
        {
            ValuesCollection.Remove(element);

            ICollection<T> added = AddedCollection;
            if (added.Contains(element))
                added.Remove(element);
            else
                RemovedCollection.Add(element);

            NotifyListenersAndQueueForCollectionRegisterPhase();
        }

        public void RemoveRange(IEnumerable<T> elements)
        {
            ICollection<T> values = ValuesCollection;
            ICollection<T> added = AddedCollection;
            ICollection<T> removed = RemovedCollection;

            foreach (T element in elements)
            {
                values.Remove(element);

                if (added.Contains(element))
                    added.Remove(element);
                else
                    removed.Add(element);
            }

            NotifyListenersAndQueueForCollectionRegisterPhase();
        }
    }
}
