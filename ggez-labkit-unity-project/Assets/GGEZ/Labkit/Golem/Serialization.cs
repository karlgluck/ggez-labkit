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
using System.Collections.Generic;

namespace GGEZ.Labkit
{
    public static class Serialization
    {
        private static fsSerializer s_serializer;
        public static fsSerializer GetSerializer(UnityObjectList objectReferences)
        {
            if (s_serializer == null)
            {
                s_serializer = new fsSerializer();
                // would add converters here if we needed to
            }

            s_serializer.UnityReferences = objectReferences;
            return s_serializer;
        }

        public static string SerializeDictionary(Dictionary<string, object> serialized)
        {
            var serializer = Serialization.GetSerializer(null);
            fsData data;
            serializer.TrySerialize(serialized, out data);
            return fsJsonPrinter.PrettyJson(data);
        }
        
        public static Dictionary<string, object> DeserializeDictionary(string json, UnityObjectList objectReferences, UnityObject owner)
        {
            var serializer = Serialization.GetSerializer(objectReferences);
            Dictionary<string, object> deserialized = new Dictionary<string, object>();

            fsData data;
            fsResult result;

            result = fsJsonParser.Parse(json, out data);
            if (result.Failed)
            {
                Debug.LogError(result, owner);
                return deserialized;
            }

            result = serializer.TryDeserialize(data, ref deserialized);
            if (result.Failed)
            {
                Debug.LogError(result, owner);
                return deserialized;
            }

            return deserialized;
        }

        /// <summary>Assigns named field of self to either the value provided (if it exists and
        /// the type matches) or a new instance of the correct type (if possible).</summary>
        /// <remarks>Only throws exceptions for misuse not bad data.</remarks>
        public static void ReadOrCreate(object self, string field, Dictionary<string, object> values)
        {
            if (self == null) throw new ArgumentNullException("self");
            if (field == null) throw new ArgumentNullException("field");
            if (values == null) throw new ArgumentNullException("values");

            var fieldInfo = self.GetType().GetField(field);

            // First, try to read from the set of values
            try
            {
                if (values.ContainsKey(field))
                {
                    fieldInfo.SetValue(self, values[field]);
                    return;
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            // Next, try to assign a default value
            try
            {
                if (fieldInfo.FieldType.IsArray)
                {
                    fieldInfo.SetValue(self, Array.CreateInstance(fieldInfo.FieldType.GetElementType(), 0));
                }
                else
                {
                    fieldInfo.SetValue(self, Activator.CreateInstance(fieldInfo.FieldType));
                }
            }
            catch (Exception e)
            {
                // If we get here, the type probably has no default
                // constructor so just leave it alone.
                Debug.LogException(e);
            }
        }

        public static T Read<T>(string key, Dictionary<string, object> values) where T : class
        {
            object value;
            values.TryGetValue(key, out value);
            return value as T;
        }
    }
}
