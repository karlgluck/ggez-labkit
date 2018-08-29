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
using System.Collections.Generic;
using UnityObjectList = System.Collections.Generic.List<UnityEngine.Object>;

namespace GGEZ.Omnibus
{
    //-----------------------------------------------------------------------------
    // SettingsAsset
    //-----------------------------------------------------------------------------
    [CreateAssetMenu(menuName = "GGEZ/Omnibus/Settings", fileName = "New Settings.asset")]
    public class SettingsAsset : ScriptableObject, ISerializationCallbackReceiver
    {
        //-----------------------------------------------------------------
        // References
        //-----------------------------------------------------------------
        [Tooltip("Another SettingsAsset to be queried if this one is missing a value")]
        public SettingsAsset InheritFrom;

        //-----------------------------------------------------------------
        // Serialized
        //-----------------------------------------------------------------
        public string Json = "{}";
        public UnityObjectList References = new UnityObjectList();

        //-----------------------------------------------------------------
        // Runtime
        //-----------------------------------------------------------------
        [System.NonSerialized]
        public Settings Settings = new Settings();

        //-----------------------------------------------------
        // OnBeforeSerialize
        //-----------------------------------------------------
        public void OnBeforeSerialize()
        {
            References.Clear();
            var serializer = Serialization.GetSerializer(References);
            fsData data;
            Dictionary<string, object> serialized = new Dictionary<string, object>();
            serialized["Values"] = Settings.Values;
            fsResult result = serializer.TrySerialize(typeof(Dictionary<string, object>), serialized, out data);
            if (result.Failed)
            {
                Debug.LogError(result, this);
                Json = "{}";
            }
            else
            {
                Json = fsJsonPrinter.CompressedJson(data);
            }
        }

        //-----------------------------------------------------
        // OnAfterDeserialize
        //-----------------------------------------------------
        public void OnAfterDeserialize()
        {
            var serializer = Serialization.GetSerializer(References);
            Dictionary<string, object> deserialized = new Dictionary<string, object>();
            fsData data = fsJsonParser.Parse(Json);
            fsResult result = serializer.TryDeserialize(data, ref deserialized);
            if (result.Failed)
            {
                Debug.LogError(result, this);
                Settings = new Settings();
            }
            else
            {
                Settings.Values = new Dictionary<string, object>(deserialized["Values"] as Dictionary<string, object>);
            }
            Settings.InheritFrom = InheritFrom;
        }
    }
}
