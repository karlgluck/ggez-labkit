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
#if UNITY_EDITOR
using System.Linq;
#endif

namespace GGEZ.Labkit
{
    /// <summary>
    /// An Archetype is created for every unique Golem and is shared among all its clones.
    /// The Archetype collects the set of Aspects that the golem contains and the
    /// set of Components that it uses, then ties those together with Settings values,
    /// named References and relationship-based external Variables.
    /// </summary>
    public class GolemArchetype : ScriptableObject, ISerializationCallbackReceiver, IHasSettings
    {
        /// <summary>Functional parts used by the golem</summary>
        public GolemComponent[] Components;

        /// <summary>Groups of functionality used by this archetype</summary>
        [NonSerialized]
        public Aspect[] Aspects;

        /// <summary>Values that get assigned to fields of aspects, cells and scripts</summary>
        public Settings Settings { get; private set; }
        public SettingsAsset InheritSettingsFrom;

        /// <summary>Assignments that map settings and local variables to aspect fields</summary>
        [NonSerialized]
        public Assignment[] Assignments;

        /// <summary>Assignments that map external variables to aspect fields</summary>
        [NonSerialized]
        public Dictionary<string, Assignment[]> ExternalAssignments;

        /// <summary>Default values for all variables</summary>
        [NonSerialized]
        public Dictionary<string, IVariable> Variables;

        /// <summary>Source of data for all the NonSerialized properties</summary>
        public string Json;

        //---------------------------------------------------------------------
        // Editor Data
        //---------------------------------------------------------------------
    #if UNITY_EDITOR

        // Aspects
        //------------------
        [NonSerialized]
        public List<EditorAspect> EditorAspects;

        // Variables
        //------------------
        [NonSerialized]
        public List<GolemVariableEditorData> EditorVariables;

        /// <summary>Source of data for all NonSerialized editor-only properties</summary>
        public string EditorJson;

        /// <summary>In the editor, which component is being displayed in the window</summary>
        public int EditorWindowSelectedComponent;

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
            Dictionary<string, List<Assignment>> externalAssignments = new Dictionary<string, List<Assignment>>();
            {
                // Variables
                //--------------------------
                Variables = new Dictionary<string, IVariable>();
                for (int i = 0; i < EditorVariables.Count; ++i)
                {
                    var editorVariable = EditorVariables[i];
                    #warning TODO: repair InitialValue if needed (if it's null), delete variable if we can't make it
                    Variables.Add(editorVariable.Name, editorVariable.InitialValue.Clone());
                }

                // Aspects
                //--------------------------
                Aspects = new Aspect[EditorAspects.Count];
                for (int aspectIndex = 0; aspectIndex < EditorAspects.Count; ++aspectIndex)
                {
                    EditorAspect editorAspect = EditorAspects[aspectIndex];
                    Aspect aspect = editorAspect.Aspect.Clone();
                    Aspects[aspectIndex] = aspect;

                    var inspectableType = InspectableAspectType.GetInspectableAspectType(aspect.GetType());

                    //-------------------------------------------------
                    // Write assignments for fields referencing:
                    //  * Aspects
                    //  * Settings
                    //  * Unity Objects
                    //  * Variables
                    //-------------------------------------------------
                    {
                        var fields = inspectableType.Fields;
                        for (int fieldIndex = 0; fieldIndex < fields.Length; ++fieldIndex)
                        {
                            InspectableAspectType.Field field = fields[fieldIndex];
                            string fieldName = field.FieldInfo.Name;

                            switch (field.Type)
                            {
                                
                            //-------------------------------------------------
                            case InspectableType.Golem:
                            //-------------------------------------------------
                                assignments.Add(new Assignment()
                                {
                                    Type = AssignmentType.AspectGolem,
                                    TargetIndex = aspectIndex,
                                    TargetFieldName = fieldName,
                                });
                                break;

                            //-------------------------------------------------
                            case InspectableType.Aspect:
                            //-------------------------------------------------
                                assignments.Add(new Assignment()
                                {
                                    Type = AssignmentType.AspectAspect,
                                    TargetIndex = aspectIndex,
                                    TargetFieldName = fieldName,
                                });
                                break;

                            //-------------------------------------------------
                            case InspectableType.Variable:
                            //-------------------------------------------------
                                VariableRef variableRef;
                                if (editorAspect.FieldsUsingVariables.TryGetValue(fieldName, out variableRef) && variableRef.IsValid)
                                {
                                    var assignment = new Assignment()
                                    {
                                        Name = variableRef.Name,
                                        TargetIndex = aspectIndex,
                                        TargetFieldName = fieldName,
                                    };

                                    if (variableRef.IsExternal)
                                    {
                                        assignment.Type = field.CanBeNull
                                                ? AssignmentType.AspectVariableOrNull
                                                : AssignmentType.AspectVariableOrDummy;
                                        externalAssignments.MultiAdd(variableRef.Relationship, assignment);
                                    }
                                    else
                                    {
                                        assignment.Type = AssignmentType.AspectLocalVariable;
                                        assignments.Add(assignment);
                                    }
                                }
                                else
                                {
                                    if (!field.CanBeNull)
                                    {
                                        assignments.Add(new Assignment()
                                        {
                                            Type = AssignmentType.AspectDummyVariable,
                                            TargetIndex = aspectIndex,
                                            TargetFieldName = fieldName,
                                        });
                                    }
                                }
                                break;

                            //-------------------------------------------------
                            default:
                            //-------------------------------------------------
                                string setting;
                                if (editorAspect.FieldsUsingSettings.TryGetValue(fieldName, out setting))
                                {
                                    Debug.Assert(InspectableTypeExt.CanUseSetting(field.Type));

                                    assignments.Add(new Assignment()
                                    {
                                        Type = AssignmentType.AspectSetting,
                                        Name = setting,
                                        TargetIndex = aspectIndex,
                                        TargetFieldName = fieldName,
                                    });
                                }
                                break;
                            }
                        }
                    }

                }

                Assignments = assignments.ToArray();
                ExternalAssignments = externalAssignments.MultiToArray();
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
                serialized["Variables"] = Variables;

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
                Serialization.ReadOrCreate(this, "Variables", deserialized);

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

                #warning handle the case where you change a field or variable or register's type and try to use data that loads the old type
            }

        #endif

        }

    #if UNITY_EDITOR

        public bool ContainsVariable(string name, Type type)
        {
            if (name == null || type == null)
            {
                return false;
            }
            for (int i = 0; i < EditorVariables.Count; ++i)
            {
                if (EditorVariables[i].Name == name)
                {
                    return type.IsAssignableFrom(EditorVariables[i].Type);
                }
            }
            return false;
        }
    
        public void AddNewAspect(Aspect aspect)
        {
            if (aspect == null)
            {
                throw new ArgumentNullException("aspect");
            }
            var aspectType = aspect.GetType();
            
            // Make sure this aspect doesn't already exist
            for (int i = 0; i < Aspects.Length; ++i)
            {
                Debug.Assert(!Aspects[i].GetType().Equals(aspectType));
            }

            var editorAspect = new EditorAspect();
            editorAspect.Aspect = aspect;
            EditorAspects.Add(editorAspect);

            InspectableAspectType inspectableAspectType = InspectableAspectType.GetInspectableAspectType(aspectType);

            // Clean up FieldsUsingSettings to only include existing fields
            {
                foreach (var key in editorAspect.FieldsUsingSettings.Keys.Where(
                    (fieldName) => !inspectableAspectType.Fields.Any((e) => e.FieldInfo.Name == fieldName && InspectableTypeExt.CanUseSetting(e.Type))
                    ).ToArray())
                {
                    editorAspect.FieldsUsingSettings.Remove(key);
                }
            }

        }

        public void RemoveAspect(EditorAspect editorAspect)
        {
            if (editorAspect == null)
            {
                throw new ArgumentNullException("editableAspect");
            }
            var index = EditorAspects.IndexOf(editorAspect);
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("editableAspect");
            }
            EditorAspects.RemoveAt(index);
        }


        public void Reset()
        {
            Components = new GolemComponent[0];
            Aspects = new Aspect[0];
            Settings = new Settings(this);
            InheritSettingsFrom = null;
            Assignments = new Assignment[0];
            ExternalAssignments = new Dictionary<string, Assignment[]>();
            Variables = new Dictionary<string, IVariable>();
            Json = "{}";

            EditorAspects = new List<EditorAspect>();
            EditorVariables = new List<GolemVariableEditorData>();
            EditorJson = "{}";
        }

        void OnValidate()
        {
            bool dirty = false;

            if (Settings == null)
            {
                Settings = new Settings(this);
                dirty = true;
            }
            if (EditorAspects == null)
            {
                EditorAspects = new List<EditorAspect>();
                dirty = true;
            }
            if (EditorVariables == null)
            {
                EditorVariables = new List<GolemVariableEditorData>();
                dirty = true;
            }

            dirty = DeduplicateComponents() || dirty;
            
            for (int i = EditorVariables.Count - 1; i >= 0; --i)
            {
                var variable = EditorVariables[i];
                if (variable.InitialValue == null)
                {
                    dirty = true;
                    variable.InitialValue = Activator.CreateInstance(variable.Type) as IVariable;
                    if (variable.InitialValue == null)
                    {
                        EditorVariables.RemoveAt(i);
                    }
                }
            }

            if (dirty)
            {
                GolemEditorUtility.SetDirty(this);
            }
        }

        public bool DeduplicateComponents()
        {
            bool dirty = false;
            HashSet<int> components = new HashSet<int>();
            for (int i = Components.Length - 1; i >= 0; --i)
            {
                if (Components[i] == null || components.Contains(Components[i].GetInstanceID()))
                {
                    for (int j = i; j + 1 < Components.Length; ++j)
                    {
                        Components[j] = Components[j+1];
                    }
                    Array.Resize(ref Components, Components.Length - 1);
                    dirty = true;
                    // UnityEditor.EditorWindow.focusedWindow.ShowNotification(new GUIContent("Removed Duplicate Component"));
                }
                else
                {
                    components.Add(Components[i].GetInstanceID());
                }
            }
            return dirty;
        }

    #endif

    }
}