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
using System;
using UnityEngine;
using System.Reflection;
using GGEZ;
#warning todo unify namespaces for ggez
using GGEZ.FullSerializer;
using UnityObject = UnityEngine.Object;

namespace GGEZ.Labkit
{

    public sealed class Golem : MonoBehaviour, ISerializationCallbackReceiver
    {
        [SerializeField]
        public GolemArchetype Archetype;

        /// <summary>Golem-local settings used for prefab Unity Object references and individual overrides.</summary>
        public Settings Settings;

        /// <summary>Object references the settings use</summary>
        /// <remarks>Stored on the Golem so that these references are updated by Unity on Instantiate</remarks>
        [SerializeField]
        public List<UnityObject> SettingsObjectReferences;

        /// <summary>Source data for golem-local settings</summary>
        [SerializeField, HideInInspector]
        public string SettingsJson;

    #region Runtime

        /// All named variables for this golem
        [NonSerialized]
        public Dictionary<string, Variable> Variables;

        /// Local copy of each aspect from the archetype
        [NonSerialized]
        public Aspect[] Aspects;

        /// Runtime data needed by each component
        [NonSerialized]
        public GolemComponentRuntimeData[] Components;

        /// Trigger state for this golem
        [NonSerialized]
        public bool[] Triggers;

    #endregion

        void Awake()
        {
            GolemManager.OnAwake(this);
        }

        /// <summary>Find an aspect of the given type on this golem</summary>
        /// <returns>The matching aspect or null if none exists</returns>
        /// <remarks>Used during setup</remarks>
        public Aspect GetAspect(Type type)
        {
            Debug.Assert(Application.isPlaying);
            Debug.Assert(type != null);
            Debug.Assert(typeof(Aspect).IsAssignableFrom(type));
            for (int i = 0; i < Aspects.Length; ++i)
            {
                Aspect aspect = Aspects[i];
                if (aspect.GetType().Equals(type))
                {
                    return aspect;
                }
            }
            return null;
        }

        public void SetTrigger(Trigger trigger)
        {
            Debug.Assert(Application.isPlaying);
            Triggers[(int)trigger] = true;
        }

        public void OnBeforeSerialize()
        {
            GGEZ.ObjectExt.RelocateScriptableObjectField(this, "Archetype", ref Archetype);

            SettingsObjectReferences.Clear();
            var serializer = Serialization.GetSerializer(SettingsObjectReferences);
            fsData data;
            #warning TODO give the settings a dirty flag so we aren't constantly reserializing in OnBeforeSerialize
            serializer.TrySerialize(Settings.Values, out data);
            SettingsJson = fsJsonPrinter.PrettyJson(data);

        }

        public void OnAfterDeserialize()
        {
            var serializer = Serialization.GetSerializer(SettingsObjectReferences);
            fsData data;
            fsResult result;

            List<Settings.Setting> values = null;

            result = fsJsonParser.Parse(SettingsJson, out data);
            if (result.Failed)
            {
                Debug.LogError(result, this);
                goto DeserializedSettings;
            }

            result = serializer.TryDeserialize(data, ref values);
            if (result.Failed)
            {
                Debug.LogError(result, this);
                goto DeserializedSettings;
            }

        DeserializedSettings:
            Settings = new Settings(this, Archetype, values);

        }

        void Reset()
        {
            Debug.Log("Reset", this);
            Archetype = ScriptableObject.CreateInstance<GolemArchetype>();
            Settings = new Settings(this, Archetype);
            SettingsObjectReferences = new List<UnityObject>();
            SettingsJson = "[]";
            Variables = null;
            Aspects = null;
            Components = null;
            Triggers = null;
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            Archetype = Archetype ?? ScriptableObject.CreateInstance<GolemArchetype>();
            Settings = Settings ?? new Settings(this, Archetype);

            #warning do something to make sure runtime variables/etc aren't used until deserialization

            // if (!Application.isPlaying)
            // {
            //     Variables = null;
            //     Aspects = null;
            //     Components = null;
            //     Triggers = null;
            // }
        }
#endif

    }
}
