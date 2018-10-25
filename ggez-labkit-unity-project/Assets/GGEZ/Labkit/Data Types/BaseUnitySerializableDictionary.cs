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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;


namespace GGEZ.Labkit
{
    public abstract class UntypedBaseUnitySerializableDictionary
    {
    }

    /// <summary>
    ///     Derive a non-generic class from this class to allow Unity to serialize
    //      a dictionary. The key and value types must be natively serializable.
    /// </summary>
    #warning TODO custom editor for BaseUnitySerializableDictionary types
    [Serializable]
    public abstract class BaseUnitySerializableDictionary<TKey, TValue> : UntypedBaseUnitySerializableDictionary, ISerializationCallbackReceiver, IEnumerable, IEnumerable<KeyValuePair<TKey, TValue>>
    {
        private Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();

        private bool _dirty = true;

        private bool _canEverBeClean = true;

        [SerializeField, HideInInspector]
        private TKey[] _keys;

        [SerializeField, HideInInspector]
        private TValue[] _values;

        public void OnBeforeSerialize()
        {
            // The dirty flag refers only to the structural state of the dictionary, not the
            // contents. The contents get reserialized by Unity automatically since either
            // the class's contents are going to get updated (and we have a reference) or
            // a struct's contents are not going to get updated unless the user accesses
            // a setter on this class. Users only access setters if either (a) we expose
            // _dictionary, in which case _dirty is set and can never be cleared, or (b)
            // this dictionary type is only ever used directly, in which case we track all
            // edits to the dictionary.

            if (!_dirty)
                return;

            HashSet<TKey> keys = new HashSet<TKey>(_dictionary.Keys);

            int writePointer = 0;

            if (_keys != null && _values != null)
            {
                int copyLength = _dictionary.Count < _keys.Length ? _dictionary.Count : _keys.Length;
                int readPointer = 0;

                while (readPointer < copyLength)
                {
                    if (_dictionary.ContainsKey(_keys[readPointer]))
                    {
                        TKey key = _keys[readPointer];
                        if (writePointer != readPointer)
                        {
                            _keys[writePointer] = key;
                        }
                        _values[writePointer] = _dictionary[key];
                        ++writePointer;
                        keys.Remove(key);
                    }
                    ++readPointer;
                }
            }

            if (_keys == null)
                _keys = new TKey[_dictionary.Count];

            if (_values == null)
                _values = new TValue[_dictionary.Count];

            if (_keys.Length != _dictionary.Count)
                Array.Resize(ref _keys, _dictionary.Count);

            if (_values.Length != _dictionary.Count)
                Array.Resize(ref _values, _dictionary.Count);

            Debug.Assert(_dictionary.Count - writePointer == keys.Count);

            IEnumerator<TKey> keyEnum = keys.GetEnumerator();
            while (writePointer < _dictionary.Count && keyEnum.MoveNext())
            {
                TKey key = keyEnum.Current;
                _keys[writePointer] = key;
                _values[writePointer] = _dictionary[key];
                ++writePointer;
            }

            int index = 0;
            foreach (KeyValuePair<TKey, TValue> kvp in _dictionary)
            {
                _keys[index] = kvp.Key;
                _values[index] = kvp.Value;
                ++index;
            }

            _dirty = _canEverBeClean;
        }

        public void OnAfterDeserialize()
        {
            if (_keys == null)
                _keys = new TKey[0];
            if (_values == null)
                _values = new TValue[0];
            if (_values.Length > _keys.Length)
                Array.Resize(ref _keys, _values.Length);
            if (_keys.Length > _values.Length)
                Array.Resize(ref _values, _keys.Length);

            if (_dictionary == null || !_canEverBeClean)
                _dictionary = new Dictionary<TKey, TValue>();
            else
                _dictionary.Clear();

            _canEverBeClean = true;
            _dirty = false;

            int index = 0;
            int end = _keys.Length;
            while (index < end)
            {
                _dictionary[_keys[index]] = _values[index];
                ++index;
            }

        }

        public TValue this[TKey key]
        {
            get
            {
                return _dictionary[key];
            }
            set
            {
                _dirty = true;
                _dictionary[key] = value;
            }
        }

        public void Add(TKey key, TValue value)
        {
            _dirty = true;
            _dictionary.Add(key, value);
        }

        public void Clear()
        {
            _dirty = true; _dictionary.Clear();
        }

        public bool Remove(TKey key)
        {
            _dirty = true;
            return _dictionary.Remove(key);
        }

        public bool IsReadOnly { get { return false; } }
        public IEqualityComparer<TKey> Comparer { get { return _dictionary.Comparer; } }
        public ICollection<TKey> Keys { get { return _dictionary.Keys; } }
        public ICollection<TValue> Values { get { return _dictionary.Values; } }
        public int Count { get { return _dictionary.Count; } }
        public bool ContainsKey(TKey key) { return _dictionary.ContainsKey(key); }
        public bool ContainsValue(TValue value) { return _dictionary.ContainsValue(value); }
        IEnumerator IEnumerable.GetEnumerator() { return _dictionary.GetEnumerator(); }
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey,TValue>>.GetEnumerator() { return _dictionary.GetEnumerator(); }
        public bool TryGetValue(TKey key, out TValue value) { return _dictionary.TryGetValue(key, out value); }

        public static implicit operator Dictionary<TKey, TValue>(BaseUnitySerializableDictionary<TKey, TValue> self)
        {
            self._canEverBeClean = false;
            self._dirty = true;
            return self._dictionary;
        }
    }

    public static class BaseUnitySerializableDictionaryExt
    {
        public static void MultiAdd<TKey, TValue>(this BaseUnitySerializableDictionary<TKey,List<TValue>> self, TKey key, TValue item) where TValue : new()
        {
            List<TValue> list;
            if (!self.TryGetValue(key, out list))
            {
                list = new List<TValue>();
                self.Add(key, list);
            }
            list.Add(item);
        }

        public static bool MultiRemove<TKey, TValue>(this BaseUnitySerializableDictionary<TKey,List<TValue>> self, TKey key, TValue item) where TValue : new()
        {
            List<TValue> list;
            if (self.TryGetValue(key, out list))
            {
                if (list.Remove(item))
                {
                    if (list.Count == 0)
                    {
                        self.Remove(key);
                    }
                    return true;
                }
            }
            return false;
        }

        public static Dictionary<TKey, TValue[]> MultiToArray<TKey, TValue>(this BaseUnitySerializableDictionary<TKey,List<TValue>> self)
        {
            var retval = new Dictionary<TKey, TValue[]>(self.Count);
            foreach (var kvp in self)
            {
                retval.Add(kvp.Key, kvp.Value.ToArray());
            }
            return retval;
        }

        public static bool MultiPop<TKey, TValue>(this BaseUnitySerializableDictionary<TKey,List<TValue>> self, TKey key, out TValue item)
        {
            List<TValue> list;
            if (self.TryGetValue(key, out list))
            {
                int index = list.Count - 1;
                if (index >= 0)
                {
                    item = list[index];
                    list.RemoveAt(index);
                    return true;
                }
            }
            item = default(TValue);
            return false;
        }
    }

}
