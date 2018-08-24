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
using System;
using System.Reflection;
using GGEZ.FullSerializer;
using UnityObject = UnityEngine.Object;
using UnityObjectList = System.Collections.Generic.List<UnityEngine.Object>;


namespace GGEZ.Omnibus
{

[AttributeUsage(AttributeTargets.Enum)]
public class fsUseIntValue : Attribute
{
    public static bool IsAppliedTo(Type type)
    {
        var attributes = type.GetCustomAttributes(false);
        for (int i = 0; i < attributes.Length; ++i)
        {
            if (attributes[i] is fsUseIntValue)
            {
                return true;
            }
        }
        return false;
    }
}

public static class Serialization
{
    private static fsSerializer s_serializer;
    private static fsEnumAsIntConverter s_enumAsIntConverter;
    public static fsSerializer GetSerializer (UnityObjectList objectReferences)
    {

        if (s_serializer == null)
        {
            s_serializer = new fsSerializer();
            s_enumAsIntConverter = new fsEnumAsIntConverter();
            s_serializer.AddConverter(s_enumAsIntConverter);
        }

        s_serializer.UnityReferences = objectReferences;
        return s_serializer;
    }

    class fsEnumAsIntConverter : fsConverter
    {
        public override bool CanProcess(Type type)
        {
            return fsUseIntValue.IsAppliedTo(type);
        }
        public override fsResult TryDeserialize(fsData storage, ref object instance, Type storageType)
        {
            if(CheckType(storage, fsDataType.Int64).Failed)
            {
                Debug.LogError("CheckType for enum failed");
                Debug.LogError("data = " + storage.ToString());
                return fsResult.Fail("CheckType for enum failed");
            }
            instance = Enum.ToObject(storageType, storage.AsInt64);
            return fsResult.Success;
        }

        public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
        {
            serialized = new fsData((int)instance);
            return fsResult.Success;
        }

    }
}

}
