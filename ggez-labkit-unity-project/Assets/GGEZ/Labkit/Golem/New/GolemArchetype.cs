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
using System.Collections.Generic;
using UnityObject = UnityEngine.Object;
using UnityObjectList = System.Collections.Generic.List<UnityEngine.Object>;
using GGEZ.FullSerializer;

namespace GGEZ.Labkit
{
    /// <summary>
    /// An Archetype is created for every unique Golem and is shared among all its clones.
    /// The Archetype collects the set of Aspects that the golem contains and the
    /// set of Components that it uses, then ties those together with Settings values,
    /// named References and relationship-based external Variables.
    /// </summary>
    public class GolemArchetype : ScriptableObject, ISerializationCallbackReceiver
    {
        /// <summary>Pairs with the References field in each golem instance</summary>
        public string[] ReferenceNames;

        /// <summary>Functional parts used by the golem</summary>
        public GolemComponent[] Components;

        /// Groups of functionality used by this archetype
        [NonSerialized]
        public Aspect[] Aspects;

        /// Values that get assigned to fields of aspects, cells and scripts
        [NonSerialized]
        public Settings Settings;
        public SettingsAsset InheritSettingsFrom;

        /// Assignments that map settings and local variables to aspect fields
        [NonSerialized]
        public Assignment[] Assignments;

        /// Assignments that map external variables to aspect fields
        [NonSerialized]
        public Dictionary<string, Assignment[]> ExternalAssignments;

        /// <summary>Source of data for all the NonSerialized properties</summary>
        public string Json;

        //---------------------------------------------------------------------
        // Editor Data
        //---------------------------------------------------------------------
    #if UNITY_EDITOR

        // Aspects
        //------------------
        [NonSerialized]
        public List<GolemAspectEditorData> EditorAspects = new List<GolemAspectEditorData>();

        // Variables
        //------------------
        [NonSerialized]
        public List<GolemVariableEditorData> EditorVariables = new List<GolemVariableEditorData>();

        /// <summary>Source of data for all NonSerialized editor-only properties</summary>
        public string EditorJson;

    #endif

        public void OnBeforeSerialize()
        {

        #if UNITY_EDITOR

            // Save editor-only data
            //-------------------------
            {
                Dictionary<string, object> serialized = new Dictionary<string, object>();

                serialized["EditorAspects"] = EditorAspects;
                serialized["EditorVariables"] = EditorVariables;

                EditorJson = Serialization.SerializeDictionary(serialized);
            }

            // Compile runtime data
            //-------------------------
            List<Assignment> assignments = new List<Assignment>();
            Dictionary<string, List<Assignment>> externalAssignments = new Dictionary<string, List<Assignment>> externalAssignments;
            {
                Aspects = new Aspect[EditorAspects.Count];
                for (int i = 0; i < EditorAspects.Count; ++i)
                {
                    Aspect aspect = EditorAspects[i].Aspect.Clone();
                    Aspects[i] = aspect;

                    var inspectableType = InspectableAspectType.GetInspectableAspectType(aspect.GetType());

                    var fields = inspectableType.Fields;
                    for (int j = 0; j < fields.Length; ++j)
                    {
                        fields[j].FieldInfo.Name
                    }
                }
            }

        #else
            Debug.LogError("GolemArchetype.OnBeforeSerialize should never be called at runtime!", this);
        #endif
        
            // Save runtime data
            //-------------------------
            {
                Dictionary<string, object> serialized = new Dictionary<string, object>();

                serialized["Aspects"] = Aspects;
                serialized["Assignments"] = Assignments;
                serialized["ExternalAssignments"] = ExternalAssignments;
                serialized["Settings"] = Settings.Values;

                Json = Serialization.SerializeDictionary(serialized);
            }

        }

        public void OnAfterDeserialize()
        {

            {
                var deserialized = Serialization.DeserializeDictionary(Json, null, this);

                Serialization.ReadOrCreate(this, "Aspects", deserialized);
                Serialization.ReadOrCreate(this, "Assignments", deserialized);
                Serialization.ReadOrCreate(this, "ExternalAssignments", deserialized);

                Settings = new Settings(
                        this,
                        InheritSettingsFrom,
                        Serialization.Read<List<Settings.Setting>>("Settings", deserialized)
                        );
            }

        #if UNITY_EDITOR
            {
                var deserialized = Serialization.DeserializeDictionary(EditorJson, null, this);

                Serialization.ReadOrCreate(this, "EditorAspects", deserialized);
                Serialization.ReadOrCreate(this, "EditorVariables", deserialized);

                this.RemoveEditorVariablesWithoutSourceAspects();
            }
        #endif

        } 
    }
}