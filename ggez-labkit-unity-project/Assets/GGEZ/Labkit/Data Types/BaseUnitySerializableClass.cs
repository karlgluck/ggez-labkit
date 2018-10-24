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
using GGEZ.FullSerializer;

namespace GGEZ.Labkit
{


    /// <summary>
    ///     Derive a non-generic class from this class to allow Unity to serialize
    ///     any class you want. To be able to use the serialized class just like the
    ///     underlying type, have both classes implement the same interface. In the
    ///     serializable version, call SetDirty() any time the underlying class
    ///     performs an action that will require reserialization.
    /// </summary>
    [Serializable]
    public abstract class BaseUnitySerializableClass<T> : ISerializationCallbackReceiver
    {
        private T _class;

        [SerializeField]
        private string _json;

        private static fsSerializer s_serializer;
        private static fsSerializer Serializer
        {
            get
            {
                if (s_serializer == null)
                    s_serializer = new fsSerializer();
                return s_serializer;
            }
        }

        public T Get()
        {
            return _class;
        }

        public void Set(T value)
        {
            _class = value;
        }

        public static implicit operator T(BaseUnitySerializableClass<T> self)
        {
            return self._class;
        }

        public void OnBeforeSerialize()
        {
            fsData data;
            fsResult result;

            result = Serializer.TrySerialize(_class, out data);

            if (result.Failed)
                Debug.LogError(result);

            _json = fsJsonPrinter.CompressedJson(data);
        }

        public void OnAfterDeserialize()
        {
            fsData data;
            fsResult result;

            result = fsJsonParser.Parse(_json, out data);

            if (result.Succeeded)
                result = Serializer.TryDeserialize(data, ref _class);

            if (result.Failed)
                Debug.LogError(result);
        }

    }


}
