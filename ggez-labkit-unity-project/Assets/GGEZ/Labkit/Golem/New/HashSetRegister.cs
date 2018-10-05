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
    // public class HashSetRegister<T> : IRegister
    // {
    //     private HashSet<T> _values = new HashSet<T>();
    //     private HashSet<T> _added = new HashSet<T>();
    //     private HashSet<T> _removed = new HashSet<T>();

    //     private List<Cell> _listeners;

    //     public HashSet<T> Values { get { return _values; } }
    //     public HashSet<T> Added { get { return _added; } }
    //     public HashSet<T> Removed { get { return _removed; } }

    //     public IRegister Clone ()
    //     {
    //         return MemberwiseClone() as IRegister;
    //     }

    //     public IVariable CreateVariable()
    //     {
    //         return new HashSetVariable<T>(this);
    //     }

    //     public void AddListener(Cell cell)
    //     {
    //         if (_listeners == null)
    //         {
    //             _listeners = new List<Cell>();
    //         }
    //         _listeners.Add(cell);
    //     }

    //     public void RemoveListener(Cell cell)
    //     {
    //         if (_listeners != null)
    //         {
    //             _listeners.Remove(cell);
    //         }
    //     }
    // }
}

            // if (_listeners != null)
            // {
            //     GolemManager.AddChangedCellInputs(_listeners);
            // }

    // does this only live in variable space?

    // public class HashSetVariable<T> : HasEndFrame
    // {
    //     public HashSet<T> Values { get { return _values; } }
    //     public HashSet<T> Added { get { return _added; } }
    //     public HashSet<T> Removed { get { return _removed; } }

    //     private HashSet<T> _values = new HashSet<T>();
    //     private HashSet<T> _added = new HashSet<T>();
    //     private HashSet<T> _removed = new HashSet<T>();

    //     // TODO: don't actually need to store NextFrameValues since it is === _values + _nextFrameAdded - _nextFrameRemoved
    //     private HashSet<T> _nextFrameValues = new HashSet<T>();
    //     private HashSet<T> _nextFrameAdded = new HashSet<T>();
    //     private HashSet<T> _nextFrameRemoved = new HashSet<T>();

    //     public bool Add(T element)
    //     {
    //         if (!_nextFrameValues.Add(element))
    //         {
    //             return false;
    //         }

    //         if (_values.Contains(element))
    //         {
    //             Debug.Assert(_nextFrameRemoved.Contains(element));
    //             _nextFrameRemoved.Remove(element);
    //         }
    //         else
    //         {
    //             Debug.Assert(!_nextFrameAdded.Contains(element));
    //             _nextFrameAdded.Add(element);
    //         }

    //         return true;
    //     }

    //     public void Add(IEnumerable<T> elements)
    //     {
    //         foreach (T element in elements)
    //         {
    //             Add(element);
    //         }
    //     }

    //     public bool Remove(T element)
    //     {
    //         if (!_nextFrameValues.Remove(element))
    //         {
    //             return false;
    //         }

    //         if (_values.Contains(element))
    //         {
    //             Debug.Assert(!_nextFrameRemoved.Contains(element));
    //             _nextFrameRemoved.Add(element);
    //         }
    //         else
    //         {
    //             Debug.Assert(_nextFrameAdded.Contains(element));
    //             _nextFrameAdded.Remove(element);
    //         }

    //         return true;
    //     }

    //     public void Remove(IEnumerable<T> elements)
    //     {
    //         foreach (T element in elements)
    //         {
    //             Remove(element);
    //         }
    //     }


    //     public void Set(IEnumerable<T> elements)
    //     {
    //         HashSet<T> toRemove = new HashSet<T>(_nextFrameValues);
    //         toRemove.ExceptWith(elements);
    //         foreach (T element in toRemove)
    //         {
    //             Remove(element);
    //         }

    //         foreach (T element in elements)
    //         {
    //             Add(element);
    //         }
    //     }

    //     public void EndFrame()
    //     {
    //         _values.UnionWith(_nextFrameAdded);
    //         _values.ExceptWith(_nextFrameRemoved);

    //         {
    //             _added.Clear();
    //             var swap = _added;
    //             _added = _nextFrameAdded;
    //             _nextFrameAdded = swap;
    //         }

    //         {
    //             _removed.Clear();
    //             var swap = _removed;
    //             _removed = _nextFrameRemoved;
    //             _nextFrameRemoved = swap;
    //         }
    //     }
    // }