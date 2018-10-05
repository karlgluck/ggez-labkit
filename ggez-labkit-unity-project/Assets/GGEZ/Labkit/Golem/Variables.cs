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

namespace NewNewLabkit
{
    // ok forget all that, registers and variables are the same thing
    // they point to a single location and are shared

    /*

    the trick is that multiple outputs can listen to a single input, so
    we need some sort of distrubution method for fanout:
        - Store a list of listener pointers and queue them when a variable changes
            - Need to hashset so a listener doesn't get multiple updates
            - ...why not just give cell instances directly and do a pointer-comparison
        - Store a hashset of listener indices and queue them
            - If this is a global list, indices need to be offset by golem instance
            - Kinda cool solution because the order of these can match the fixed update order of cells
            - Can parallelize this by knowing where cell-boundaries are
        - Store a list of pointers to classes that have pointers to arrays of flags and indices to set
        - Limit fanout to a fixed number and do any of the above
            - Means you can use a linear array with stride and a single index
        - Save a dirty flag alongside each value and poll every receiver every frame
        - Poll for delta using comparison


    /*
    rules for this version:
     - Registers store a list of listener indices and queue them when the register changes
            ** every cell needs a unique ID and these IDs should map easily to cell instances **
     - Inputs are always hooked up to something but that thing could just always return default(T)
        - Alternative: inputs are possibly null and you have to null-check them every frame
        - Alternative: inputs are always valid, but those that aren't hooked up can be given an optional
                       bool field
        - ***Alternative: inputs can be optionally set to either a valid default OR be null depending on input flag***
     - Adding or removing references involves reflection
     - The cell is only called if it is dirty, but determining if a particular input is dirty is up to the cell

    goals: no boxing/unboxing, minimize conditional expressions in the main loop
     */

    /*

    OKAY AND in this verion SPECIAL BONUS

    - GolemClass becomes GolemComponent
    - You can add multiple components to a single Golem
        - When it's in a prefab, that's an Archetype!
    - This means that references need to be separable from the class
        - Which means that a Golem holds a list of references keyed by name
        - When a GolemComponent instantiates, it reads Unity Object references by name from the Golem and writes them into fields of aspects/cells/scripts

    WAAAAAIt what if the references were just in the Aspects for the golem? Just like variables!!!
        Inside of a GolemComponent, you get a dropdown for picking a reference to a name based on the field in the aspect
    An aspect declares a field that is of type UnityObject, and all fields with the same name get the same value
        When the golem instantiates, the list of references gets cloned
        the Golem itself maps which fields of which aspects get each UnityObject index

    For debugging purposes, each GolemClass can have a list of the Aspects it depends on
    And each external reference can have a list of the Aspects they require of the target
        These can both be checked at runtime

    the Golem says "I have these settings, these Aspects and these Components"
        - And, transparently, "these object references" as contained in Aspects.

    the Golem contains:
        - A list of GolemComponents it uses
        - A list of settings, a superset of settings used in the GolemComponents
        - A list of aspect types that Golem uses
        - A map of name -> Unity Object references
            - And what aspects to write the references into
        - A list of {variable name / register index / aspect index / field name} for variables

    the GolemComponent:
        - Contains a list of {variable name / [cell/script index] / field name / reftype} for local variables
        - Contains {setting name, [cell/script index], field name } to write values on load
        (contains variables to write to field values on load)
        (contains unity object references to write to field values on load)
     */

    public class Settings
    {
        public object Get(string name, Type type) { return null; }
    }
    public class SettingsAsset { }
    public class Aspect
    {
        public Aspect Clone() { return MemberwiseClone() as Aspect; }
    }
    public class Script
    {
        public Script Clone() { return MemberwiseClone() as Script; }
    }

    public class Golem : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        public UnityEngine.Object[] References;

        public UnityEngine.Object GetReference(string reference)
        {
            Debug.Assert(Archetype.ReferenceNames.Length == References.Length);
            string[] names = Archetype.ReferenceNames;
            for (int i = 0; i < names.Length; ++i)
            {
                if (names[i] == reference)
                {
                    return References[i];
                }
            }
            return null;
        }

        public Aspect GetAspect(Type type)
        {
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

        [SerializeField, HideInInspector]
        public GolemArchetype Archetype;

        #region Runtime
        public Dictionary<string, IVariable> Variables = new Dictionary<string, IVariable>();
        public Dictionary<string, Dictionary<string, IVariable>> Relationships = new Dictionary<string, Dictionary<string, IVariable>>();
        public Aspect[] Aspects;
        public class ComponentData
        {
            public Cell[] Cells;
            public Script[] Scripts;
            public GGEZ.Labkit.StateIndex[] LayerStates;
        }
        public ComponentData[] Components;
        #endregion
    }


    public class GolemArchetype : ScriptableObject, ISerializationCallbackReceiver
    {
        [NonSerialized]
        public Aspect[] Aspects;
        [NonSerialized]
        public GolemComponent[] Components;
        public string[] ReferenceNames;

        public SettingsAsset InheritSettingsFrom;
        [NonSerialized]
        public Settings Settings;

        [NonSerialized]
        public Assignment[] Assignments;
        [NonSerialized]
        public Dictionary<string, Assignment[]> ExternalAssignments;

        public string Json;

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
        }
    }

    public class GolemComponent : ScriptableObject, ISerializationCallbackReceiver
    {
        public string Json;

        [NonSerialized]
        public Cell[] Cells;
        [NonSerialized]
        public Script[] Scripts;
        [NonSerialized]
        public IRegister[] Registers;

        public Assignment[] Assignments;

        public Dictionary<string, Assignment[]> ExternalAssignments;
        [SerializeField]
        private string[] ExternalAssignmentsKeys;
        [SerializeField]
        private Assignment[][] ExternalAssignmentsValues;

        public class Layer
        {
            public GGEZ.Labkit.Transition[] FromAnyStateTransitions;
            public GGEZ.Labkit.Transition[] FromIdleStateTransitions;
            public Dictionary<GGEZ.Labkit.State, GGEZ.Labkit.Transition[]> Transitions;
        }

        public Layer[] Layers;

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
        }
    }


    public enum AssignmentType
    {

        // Used on GolemArchetype:

        AspectSetting,                  // The setting [Name] from the archetype
        AspectUnityObject,              // The UnityObject [Name] from the archetype
        AspectVariable,                 // Find [TargetFieldName] on aspect [TargetIndex] and assign it to variable [Name], creating that variable if necessary
        AspectVariableOrNull,           // The existing variable [Name] or null
        AspectVariableOrDummy,           // The existing variable [Name] or a newly allocated variable

        // Used on GolemComponent:

        CellUnityObject,                // The UnityObject [Name] from the archetype
        ScriptUnityObject,              // The UnityObject [Name] from the archetype

        CellSetting,                    // The setting [Name] from the archetype
        ScriptSetting,                  // The setting [Name] from the archetype

        CellAspect,                     // Aspect of the target field's type or null
        ScriptAspect,                   // Aspect of the target field's type or null

        CellVariable,                   // Find [TargetFieldName] on cell [TargetIndex] and assign it to variable [Name], creating that variable if necessary
        CellVariableOrNull,             // The variable [Name] or null
        CellVariableOrDummy,             // The variable [Name] or a newly allocated variable
        CellRegisterVariable,           // The variable created for register [RegisterIndex]
        CellVariableRegisterOrNull,     // The register for variable [Name] or null
        CellInputVariableRegister,      // The register for variable [Name] or the global read-only register
        CellOutputVariableRegister,     // The register for variable [Name] or the global write-only register
        CellInputRegister,              // The register [RegisterIndex] from this Component
        CellOutputRegister,             // The register [RegisterIndex] from this Component
        CellDummyInputRegister,         // Global read-only register
        CellDummyOutputRegister,        // Global write-only register
        CellDummyVariable,              // Newly allocated variable
        CellDummyInputVariable,         // Global read-only variable
        CellDummyOutputVariable,        // Global write-only variable

        ScriptVariable,                 // Find [TargetFieldName] on script [TargetIndex] and assign it to variable [Name], creating that variable if necessary
        ScriptVariableOrNull,           // The variable [Name] or null
        ScriptVariableOrDummy,           // The variable [Name] or a newly allocated variable
        ScriptRegisterVariable,         // The variable created for register [RegisterIndex]
        ScriptVariableRegisterOrNull,   // The register for variable [Name] or null
        ScriptInputVariableRegister,    // The register for variable [Name] or the global read-only register
        ScriptOutputVariableRegister,   // The register for variable [Name] or the global write-only register
        ScriptInputRegister,            // The register [RegisterIndex] from this Component
        ScriptOutputRegister,           // The register [RegisterIndex] from this Component
        ScriptDummyInputRegister,       // Global read-only register
        ScriptDummyOutputRegister,      // Global write-only register
        ScriptDummyVariable,            // Newly allocated variable
        ScriptDummyInputVariable,       // Global read-only variable
        ScriptDummyOutputVariable,      // Global write-only variable
    }

    public class Assignment
    {

        public enum TargetCategorySelector
        {
            Aspect,
            Cell,
            Script,
        }

        public enum AssignedObjectSelector
        {
            UnityObjectReference,
            Setting,
            Aspect,
            Variable,
            VariableForRegister,
            RegisterOfVariable,
            Register,
        }

        public enum FallbackActionSelector
        {
            Assert,
            Null,
            Dummy,
            Create,
        }

        public TargetCategorySelector TargetCategory;
        public AssignedObjectSelector AssignedObject;
        public FallbackActionSelector FallbackAction;

        public string Name;
        public int RegisterIndex;
        public int TargetIndex;
        public string TargetFieldName;

        public FieldInfo GetFieldInfo(object target)
        {
            return target.GetType().GetField(TargetFieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        }
    }

    public static class GolemManager
    {
        private static Dictionary<string, IVariable> s_emptyVariables = new Dictionary<string, IVariable>();

        public static Golem CreateGolem(Golem prefab)
        {
            GameObject gameObject = GameObject.Instantiate(prefab.gameObject);
            Golem golem = gameObject.GetComponent<Golem>();
            GolemArchetype archetype = golem.Archetype;

            // Create aspects
            golem.Aspects = new Aspect[archetype.Aspects.Length];
            for (int i = 0; i < golem.Aspects.Length; ++i)
            {
                golem.Aspects[i] = archetype.Aspects[i].Clone();
            }

            // Assign UnityObjects and settings to aspects. Create variables from aspects.
            DoAssignments(golem, archetype.Assignments, golem.Variables, null, null);

            // Create data for all the components
            golem.Components = new Golem.ComponentData[archetype.Components.Length];
            for (int i = 0; i < archetype.Components.Length; ++i)
            {
                var from = archetype.Components[i];
                var to = golem.Components[i];


                to.Cells = new Cell[from.Cells.Length];
                for (int j = 0; j < from.Cells.Length; ++j)
                {
                    to.Cells[j] = from.Cells[j].Clone();
                }

                to.Scripts = new Script[from.Scripts.Length];
                for (int j = 0; j < from.Scripts.Length; ++j)
                {
                    to.Scripts[j] = from.Scripts[j].Clone();
                }

                to.LayerStates = new GGEZ.Labkit.StateIndex[from.Layers.Length];
                for (int j = 0; j < to.LayerStates.Length; ++j)
                {
                    to.LayerStates[j] = GGEZ.Labkit.StateIndex.Idle;
                }

                IRegister[] registers = new IRegister[from.Registers.Length];
                for (int j = 0; j < from.Registers.Length; ++j)
                {
                    registers[j] = from.Registers[j].Clone();
                }

                // Assign UnityObjects, variables, registers, settings
                DoAssignments(golem, from.Assignments, golem.Variables, to, registers);

                // Reset external references
                foreach (var kvp in from.ExternalAssignments)
                {
                    DoAssignments(golem, kvp.Value, s_emptyVariables, to, null);
                }
            }

            return golem;
        }

        /// <summary></summary>
        public static void SetRelationship(Golem golem, string relationship, Dictionary<string, IVariable> variables)
        {
            GolemArchetype archetype = golem.Archetype;
            for (int i = 0; i < archetype.Components.Length; ++i)
            {
                var component = archetype.Components[i];
                Assignment[] assignments;
                if (component.ExternalAssignments.TryGetValue(relationship, out assignments))
                {
                    DoAssignments(golem, assignments, variables, golem.Components[i], null);
                }
            }
        }

        private static IVariable GetVariableOrNull(Dictionary<string, IVariable> variables, string name)
        {
            IVariable variable;
            variables.TryGetValue(name, out variable);
            return variable;
        }

        private static IVariable GetOrCreateVariable(Dictionary<string, IVariable> variables, string name, Type type)
        {
            Debug.Assert(typeof(IVariable).IsAssignableFrom(type));
            IVariable variable;
            if (!variables.TryGetValue(name, out variable))
            {
                variable = Activator.CreateInstance(type) as IVariable;
                variables.Add(name, variable);
            }
            Debug.Assert(variable != null);
            return variable;
        }

        private static IVariable GetOrCreateRegisterVariable(int index, IRegister[] registers, ref IVariable[] registerVariables)
        {
            if (registerVariables == null)
            {
                registerVariables = new IVariable[registers.Length];
            }
            IVariable variable = registerVariables[index];
            if (registerVariables[index] == null)
            {
                variable = registers[index].CreateVariable();
                registerVariables[index] = variable;
            }
            return variable;
        }

        private static IVariable GetVariableOrDummy(Dictionary<string, IVariable> variables, string name, Type type)
        {
            Debug.Assert(typeof(IVariable).IsAssignableFrom(type));
            IVariable variable;
            if (!variables.TryGetValue(name, out variable))
            {
                variable = Activator.CreateInstance(type) as IVariable;
            }
            Debug.Assert(variable != null);
            return variable;
        }

        private static void DoAssignments(Golem golem, Assignment[] assignments, Dictionary<string, IVariable> variables, Golem.ComponentData component, IRegister[] registers)
        {
            IVariable[] registerVariables = null;
            for (int assignmentIndex = 0; assignmentIndex < assignments.Length; ++assignmentIndex)
            {
                Assignment assignment = assignments[assignmentIndex];

                object target;
                switch (assignment.TargetCategory)
                {
                default: throw new InvalidProgramException();
                case Assignment.TargetCategorySelector.Aspect:  target = golem.Aspects[assignment.TargetIndex]; break;
                case Assignment.TargetCategorySelector.Cell:    target = component.Cells[assignment.TargetIndex]; break;
                case Assignment.TargetCategorySelector.Script:  target = component.Scripts[assignment.TargetIndex]; break;
                }

                FieldInfo fieldInfo = assignment.GetFieldInfo(target);

                object value;
                switch (assignment.AssignedObject)
                {
                    case Assignment.AssignedObjectSelector.UnityObjectReference: value = golem.GetReference(assignment.Name); break;
                    case Assignment.AssignedObjectSelector.Setting:              value = golem.Archetype.Settings.Get(assignment.Name, fieldInfo.FieldType); break;
                    case Assignment.AssignedObjectSelector.Aspect:               value = golem.GetAspect(fieldInfo.FieldType); break;

                    case Assignment.AssignedObjectSelector.Variable:
                    case Assignment.AssignedObjectSelector.RegisterOfVariable:
                    {
                        Debug.Assert(typeof(IVariable).IsAssignableFrom(fieldInfo.FieldType));
                        IVariable variable;
                        if (!variables.TryGetValue(assignment.Name, out variable))
                        {
                            switch (assignment.FallbackAction)
                            {
                                case Assignment.FallbackActionSelector.Assert:
                                    throw new InvalidOperationException();

                                case Assignment.FallbackActionSelector.Null:
                                    break;

                                case Assignment.FallbackActionSelector.Dummy:
                                    variable = Activator.CreateInstance(fieldInfo.FieldType) as IVariable;
                                    Debug.Assert(variable != null);
                                    break;

                                case Assignment.FallbackActionSelector.Create:
                                    variable = Activator.CreateInstance(fieldInfo.FieldType) as IVariable;
                                    Debug.Assert(variable != null);
                                    variables.Add(assignment.Name, variable);
                                    break;
                            }
                        }
                        if (assignment.AssignedObject == Assignment.AssignedObjectSelector.RegisterOfVariable)
                        {
                            value = variable.GetRegister();
                        }
                        else
                        {
                            value = variable;
                        }
                        break;
                    }

                    case Assignment.AssignedObjectSelector.VariableForRegister:
                    {
                        Debug.Assert(assignment.FallbackAction == Assignment.FallbackActionSelector.Assert);
                        if (registerVariables == null)
                        {
                            registerVariables = new IVariable[registers.Length];
                        }
                        int index = assignment.RegisterIndex;
                        IVariable variable = registerVariables[index];
                        if (registerVariables[index] == null)
                        {
                            variable = registers[index].CreateVariable();
                            registerVariables[index] = variable;
                        }
                        value = variable;
                        break;
                    }

                }

                object target = null;

                switch (assignment.Type)
                {
                    default:
                        throw new InvalidProgramException();

                    case AssignmentType.AspectSetting:
                    case AssignmentType.AspectUnityObject:
                    case AssignmentType.AspectVariable:
                    case AssignmentType.AspectVariableOrNull:
                    case AssignmentType.AspectVariableOrDummy:
                        target = golem.Aspects[assignment.TargetIndex];
                        break;

                    case AssignmentType.CellUnityObject:
                    case AssignmentType.CellSetting:
                    case AssignmentType.CellAspect:
                    case AssignmentType.CellVariable:
                    case AssignmentType.CellVariableOrNull:
                    case AssignmentType.CellVariableOrDummy:
                    case AssignmentType.CellRegisterVariable:
                    case AssignmentType.CellVariableRegisterOrNull:
                    case AssignmentType.CellInputVariableRegister:
                    case AssignmentType.CellOutputVariableRegister:
                    case AssignmentType.CellInputRegister:
                    case AssignmentType.CellOutputRegister:
                    case AssignmentType.CellDummyInputRegister:
                    case AssignmentType.CellDummyOutputRegister:
                    case AssignmentType.CellDummyVariable:
                    case AssignmentType.CellDummyInputVariable:
                    case AssignmentType.CellDummyOutputVariable:
                        target = component.Cells[assignment.TargetIndex];
                        break;

                    case AssignmentType.ScriptUnityObject:
                    case AssignmentType.ScriptSetting:
                    case AssignmentType.ScriptAspect:
                    case AssignmentType.ScriptVariable:
                    case AssignmentType.ScriptVariableOrNull:
                    case AssignmentType.ScriptVariableOrDummy:
                    case AssignmentType.ScriptRegisterVariable:
                    case AssignmentType.ScriptVariableRegisterOrNull:
                    case AssignmentType.ScriptInputVariableRegister:
                    case AssignmentType.ScriptOutputVariableRegister:
                    case AssignmentType.ScriptInputRegister:
                    case AssignmentType.ScriptOutputRegister:
                    case AssignmentType.ScriptDummyInputRegister:
                    case AssignmentType.ScriptDummyOutputRegister:
                    case AssignmentType.ScriptDummyVariable:
                    case AssignmentType.ScriptDummyInputVariable:
                    case AssignmentType.ScriptDummyOutputVariable:
                        target = component.Scripts[assignment.TargetIndex];
                        break;
                }

                FieldInfo fieldInfo = target.GetType().GetField(assignment.TargetFieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);


                switch (assignment.Type)
                {
                    case AssignmentType.AspectUnityObject:
                    case AssignmentType.CellUnityObject:
                    case AssignmentType.ScriptUnityObject:
                        assignment.GetFieldInfo(target).SetValue(target, golem.GetReference(assignment.Name));
                        break;

                    case AssignmentType.AspectSetting:
                    case AssignmentType.CellSetting:
                    case AssignmentType.ScriptSetting:
                        fieldInfo.SetValue(target, golem.Archetype.Settings.Get(assignment.Name, fieldInfo.FieldType));
                        break;

                    case AssignmentType.CellAspect:
                    case AssignmentType.ScriptAspect:
                        fieldInfo = assignment.GetFieldInfo(target);
                        fieldInfo.SetValue(target, golem.GetAspect(fieldInfo.FieldType));
                        break;

                    case AssignmentType.AspectVariable:
                    case AssignmentType.CellVariable:
                    case AssignmentType.ScriptVariable:
                        fieldInfo = assignment.GetFieldInfo(target);
                        fieldInfo.SetValue(target, GetOrCreateVariable(variables, assignment.Name, fieldInfo.FieldType));
                        break;

                    case AssignmentType.AspectVariableOrNull:
                    case AssignmentType.CellVariableOrNull:
                    case AssignmentType.ScriptVariableOrNull:
                        assignment.GetFieldInfo(target).SetValue(target, GetVariableOrNull(variables, assignment.Name));
                        break;

                    case AssignmentType.CellVariableOrDummy:
                    case AssignmentType.ScriptVariableOrDummy:
                    case AssignmentType.AspectVariableOrDummy:
                        fieldInfo = assignment.GetFieldInfo(target);
                        fieldInfo.SetValue(target, GetVariableOrDummy(variables, assignment.Name, fieldInfo.FieldType));
                        break;

                    case AssignmentType.CellRegisterVariable:
                    case AssignmentType.ScriptRegisterVariable:
                        fieldInfo = assignment.GetFieldInfo(target);
                        fieldInfo.SetValue(target, GetOrCreateRegisterVariable(assignment.RegisterIndex, registers, ref registerVariables));
                        break;

                    case AssignmentType.CellVariableRegisterOrNull:
                    case AssignmentType.ScriptVariableRegisterOrNull:
                        {
                            IRegister register = null;
                            IVariable variable;
                            if (variables.TryGetValue(assignment.Name, out variable))
                            {
                                register = variable.GetRegister();
                            }
                            fieldInfo.SetValue(target, register);
                        }
                        break;

                    case AssignmentType.CellInputVariableRegister:
                        {
                            Cell cell = target as Cell;
                            IRegister register = null;
                            IVariable variable;
                            if (variables.TryGetValue(assignment.Name, out variable))
                            {
                                register = variable.GetRegister();
                                register.AddListener(cell);
                            }
                            else
                            {
                                register = CentralPublishing.GetDefaultInputRegister(fieldInfo.FieldType);
                            }
                            fieldInfo.SetValue(target, register);
                        }
                        break;

                    case AssignmentType.ScriptInputVariableRegister:
                        {
                            IRegister register = null;
                            IVariable variable;
                            if (variables.TryGetValue(assignment.Name, out variable))
                            {
                                register = variable.GetRegister();
                            }
                            else
                            {
                                register = CentralPublishing.GetDefaultInputRegister(fieldInfo.FieldType);
                            }
                            fieldInfo.SetValue(target, register);
                        }
                        break;

                    case AssignmentType.CellOutputVariableRegister:
                    case AssignmentType.ScriptOutputVariableRegister:
                        {
                            IRegister register = null;
                            IVariable variable;
                            if (variables.TryGetValue(assignment.Name, out variable))
                            {
                                register = variable.GetRegister();
                            }
                            else
                            {
                                register = CentralPublishing.GetDefaultOutputRegister(fieldInfo.FieldType);
                            }
                            fieldInfo.SetValue(target, register);
                        }
                        break;

                    case AssignmentType.CellInputRegister:
                        {
                            Cell cell = target as Cell;
                            IRegister register = registers[(int)assignment.RegisterIndex];
                            register.AddListener(cell);
                            fieldInfo.SetValue(target, register);
                        }
                        break;

                    case AssignmentType.ScriptInputRegister:
                        fieldInfo.SetValue(target, registers[(int)assignment.RegisterIndex]);
                        break;

                    case AssignmentType.CellOutputRegister:
                    case AssignmentType.ScriptOutputRegister:
                        fieldInfo.SetValue(target, registers[(int)assignment.RegisterIndex]);
                        break;

                    case AssignmentType.CellDummyInputRegister:
                        fieldInfo.SetValue(target, CentralPublishing.GetDefaultInputRegister(fieldInfo.FieldType));
                        break;

                    case AssignmentType.CellDummyOutputRegister:
                        fieldInfo.SetValue(target, CentralPublishing.GetDefaultOutputRegister(fieldInfo.FieldType));
                        break;

                    case AssignmentType.CellDummyVariable:
                        {
                            IVariable instance = Activator.CreateInstance(fieldInfo.FieldType) as IVariable;
                            Debug.Assert(instance != null);
                            Debug.Assert(instance.GetRegister() != null);
                            fieldInfo.SetValue(target, instance);
                        }
                        break;

                    case AssignmentType.CellDummyInputVariable:
                        fieldInfo.SetValue(target, CentralPublishing.GetDefaultInputVariable(fieldInfo.FieldType));
                        break;

                    case AssignmentType.CellDummyOutputVariable:
                        fieldInfo.SetValue(target, CentralPublishing.GetDefaultOutputVariable(fieldInfo.FieldType));
                        break;

                    case AssignmentType.ScriptDummyInputRegister:
                        fieldInfo.SetValue(target, CentralPublishing.GetDefaultInputRegister(fieldInfo.FieldType));
                        break;

                    case AssignmentType.ScriptDummyOutputRegister:
                        fieldInfo.SetValue(target, CentralPublishing.GetDefaultOutputRegister(fieldInfo.FieldType));
                        break;

                    case AssignmentType.ScriptDummyVariable:
                        {
                            IVariable instance = Activator.CreateInstance(fieldInfo.FieldType) as IVariable;
                            Debug.Assert(instance != null);
                            Debug.Assert(instance.GetRegister() != null);
                            fieldInfo.SetValue(target, instance);
                        }
                        break;

                    case AssignmentType.ScriptDummyInputVariable:
                        fieldInfo.SetValue(target, CentralPublishing.GetDefaultInputVariable(fieldInfo.FieldType));
                        break;

                    case AssignmentType.ScriptDummyOutputVariable:
                        fieldInfo.SetValue(target, CentralPublishing.GetDefaultOutputVariable(fieldInfo.FieldType));
                        break;
                }

                // switch (assignment.Type)
                // {
                //     case AssignmentType.AspectSetting:
                //     case AssignmentType.AspectUnityObject:
                //     case AssignmentType.AspectVariable:
                //     case AssignmentType.AspectVariableOrNull:
                //     case AssignmentType.CellUnityObject:
                //     case AssignmentType.ScriptUnityObject:
                //     case AssignmentType.CellSetting:
                //     case AssignmentType.ScriptSetting:
                //     case AssignmentType.CellAspect:
                //     case AssignmentType.ScriptAspect:
                //     case AssignmentType.CellVariable:
                //     case AssignmentType.CellVariableOrNull:
                //     case AssignmentType.CellVariableOrTemp:
                //     case AssignmentType.CellRegisterVariable:
                //     case AssignmentType.CellVariableRegisterOrNull:
                //     case AssignmentType.CellInputVariableRegister:
                //     case AssignmentType.CellOutputVariableRegister:
                //     case AssignmentType.CellInputRegister:
                //     case AssignmentType.CellOutputRegister:
                //     case AssignmentType.CellDefaultInputRegister:
                //     case AssignmentType.CellDefaultOutputRegister:
                //     case AssignmentType.CellDefaultVariable:
                //     case AssignmentType.CellDefaultInputVariable:
                //     case AssignmentType.CellDefaultOutputVariable:

                //     case AssignmentType.ScriptVariable:
                //     case AssignmentType.ScriptVariableOrNull:
                //     case AssignmentType.ScriptVariableOrTemp:
                //     case AssignmentType.ScriptRegisterVariable:
                //     case AssignmentType.ScriptVariableRegisterOrNull:
                //     case AssignmentType.ScriptInputVariableRegister:
                //     case AssignmentType.ScriptOutputVariableRegister:
                //     case AssignmentType.ScriptInputRegister:
                //     case AssignmentType.ScriptOutputRegister:
                //     case AssignmentType.ScriptDefaultInputRegister:
                //     case AssignmentType.ScriptDefaultOutputRegister:
                //     case AssignmentType.ScriptDefaultVariable:
                //     case AssignmentType.ScriptDefaultInputVariable:
                //     case AssignmentType.ScriptDefaultOutputVariable:
                // }
            }
        }
    }

    public interface IVariable
    {
        IRegister GetRegister();
        void OnEndProgramPhase();
    }

    public class Cell : IEquatable<Cell>
    {
        // This is a globally unique sequencing value used to organize cells into a priority queue
        // It is in the order of cells in a single golem, and unique between golems
        /// <remarks>Could use the high bits of this sequencer to distinguish
        /// cells of different golems so that they can be parallelized.</remarks>
        public int Sequencer { get; private set; }

        public virtual void Acquire()
        { }

        public virtual void Update()
        { }

        public Cell Clone(int sequencer)
        {
            Cell cell = MemberwiseClone() as Cell;
            cell.Sequencer = sequencer;
            return cell;
        }

        private static int nextSequencer = 1;
        public Cell Clone()
        {
            Cell cell = MemberwiseClone() as Cell;
            cell.Sequencer = nextSequencer++;
            return cell;
        }

        public bool Equals(Cell other)
        {
            return object.ReferenceEquals(this, other);
        }
    }

    public static class CentralPublishing
    {
        public static void AddChangedVariable(IVariable variable)
        {
        }

        public static void AddChangedCellInputs(List<Cell> cells)
        {
            // add IDs to the priority queue based on their sequencer
        }

        public static void OnEndProgramPhase()
        {
            // go through all changed variables and push their changes so that cells are queued
        }

        public static void OnCircuitPhase()
        {
            // go through the cells in order, pop the front and update them
            // add cells that want an update next frame to the next frame's priority queue
            // NOTE: variables changed during circuit phase don't write registers until
            // the beginning of next frame
        }

        private class Defaults
        {
            public IRegister InputRegister, OutputRegister;
            public IVariable InputVariable, OutputVariable;
        }

        private static Defaults GetDefaultsForType(Type fieldType)
        {
            Defaults value;
            if (!_defaults.TryGetValue(fieldType, out value))
            {
                value = new Defaults();
                if (typeof(IVariable).IsAssignableFrom(fieldType))
                {
                    value.InputVariable = Activator.CreateInstance(fieldType) as IVariable;
                    value.InputRegister = value.InputVariable.GetRegister();
                    value.OutputRegister = value.InputRegister.Clone();
                    value.OutputVariable = value.OutputRegister.CreateVariable();
                }
                else
                {
                    value.InputRegister = Activator.CreateInstance(fieldType) as IRegister;
                    value.OutputRegister = value.InputRegister.Clone();
                    value.InputVariable = value.InputRegister.CreateVariable();
                    value.OutputVariable = value.OutputRegister.CreateVariable();
                }
                _defaults.Add(value.InputRegister.GetType(), value);
                _defaults.Add(value.InputVariable.GetType(), value);
            }
            return value;
        }
        private static Dictionary<Type, Defaults> _defaults = new Dictionary<Type, Defaults>();

        /// <summary></summary>
        public static IRegister GetDefaultInputRegister(Type fieldType)
        {
            return GetDefaultsForType(fieldType).InputRegister;
        }

        /// <summary></summary>
        public static IRegister GetDefaultOutputRegister(Type fieldType)
        {
            return GetDefaultsForType(fieldType).OutputRegister;
        }

        /// <summary></summary>
        public static IVariable GetDefaultInputVariable(Type fieldType)
        {
            return GetDefaultsForType(fieldType).InputVariable;
        }

        /// <summary></summary>
        public static IVariable GetDefaultOutputVariable(Type fieldType)
        {
            return GetDefaultsForType(fieldType).OutputVariable;
        }
    }

    public class Variable<T> : IVariable
    {
        public IRegister GetRegister()
        {
            return Register;
        }

        protected Register<T> Register { get; private set; }

        private T _value; // next frame value
        public T Value
        {
            get
            {
                return Register.Value;
            }
            set
            {
                _value = value;
                CentralPublishing.AddChangedVariable(this);
            }
        }

        public Variable()
        {
            Register = new Register<T>();
        }

        public Variable(Register<T> register)
        {
            Register = register;
        }

        public void OnEndProgramPhase()
        {
            Register.ChangeValue(_value);
        }
    }

    public interface IRegister
    {
        IRegister Clone();
        IVariable CreateVariable();
        void AddListener(Cell cell);
        void RemoveListener(Cell cell);
    }

    public class Register<T> : IRegister
    {
        public IVariable CreateVariable()
        {
            return new Variable<T>(this);
        }

        private T _value;
        public T Value
        {
            get { return _value; }
            set { ChangeValue(value); }
        }

        public bool ChangeValue(T value)
        {
            if (object.Equals(_value, value))
            {
                return false;
            }
            _value = value;
            if (_listeners != null)
            {
                CentralPublishing.AddChangedCellInputs(_listeners);
            }
            return true;
        }

        private List<Cell> _listeners;

        public void AddListener(Cell cell)
        {
            if (_listeners == null)
            {
                _listeners = new List<Cell>();
            }
            _listeners.Add(cell);
        }

        public void RemoveListener(Cell cell)
        {
            if (_listeners != null)
            {
                _listeners.Remove(cell);
            }
        }

        public IRegister Clone ()
        {
            return MemberwiseClone() as IRegister;
        }
    }

    // A cell has fields
    public class Add
    {
        // [In] // Hooked up to read-only input if not attached
        // [InOrNull] // Not hooked up if not attached
        public Register<float> Input;

        // [Out] // Hooked up to write-only output if not attached
        // [OutOrNull] // Not hooked up if not attached
        public Register<float> Output;

        public float Amount;

        public void Update()
        {
            // Output.Value = Input.Value + Amount;
        }
    }


    public class V2Golem
    {

        public Dictionary<string, IVariable> Variables;

        public class Aspect { }
        public Aspect[] Aspects;
        public class Script { }
        public Script[] Scripts;

        public Cell[] Cells;

        public GolemClass Class;

        public Array GetObjectTypeArray(GolemClass.ObjectType type)
        {
            switch (type)
            {
                default: throw new InvalidProgramException();
                case GolemClass.ObjectType.Aspect:  return Aspects;
                case GolemClass.ObjectType.Cell:    return Cells;
                case GolemClass.ObjectType.Script:  return Scripts;
            }
        }

        public void SetReference(string reference, Dictionary<string, IVariable> variables)
        {
            GolemClass.ExternalVariables[] externalVariables;
            if (!Class.ExternalReferences.TryGetValue(reference, out externalVariables))
            {
                return;
            }
            for (int i = 0; i < externalVariables.Length; ++i)
            {
                var externalVariable = externalVariables[i];
                IVariable variable;
                if (!variables.TryGetValue (externalVariable.VariableName, out variable))
                {
                    continue;
                }

                var assignments = externalVariable.RegisterAssignments;
                for (int j = 0; j < assignments.Length; ++j)
                {
                    var assignment = assignments[j];
                    Array objects = GetObjectTypeArray(assignment.ObjectType);
                    objects.GetValue(assignment.TargetIndex);
                }
            }
        }
    }

    public class GolemClass
    {
        static int NextCellID;

        Cell[] Cells;

        public enum ObjectType
        {
            Aspect,
            Cell,
            Script,
        }

        public enum AssignmentType
        {
            InputRegister,
            OutputRegister,
            Variable,
            DefaultInputRegister,
            DefaultOutputRegister,
            DefaultInputVariable,
            DefaultOutputVariable,
        }

        public class ObjectRegisterAssignment
        {
            public ObjectType ObjectType;
            public int TargetIndex;
            public string FieldName;
            public AssignmentType Type;
            public RegisterIndex RegisterIndex;
        }

        public IRegister[] Registers;
        public ObjectRegisterAssignment[] RegisterAssignments;

        public class VariableRegister
        {
            public string VariableName;
            public RegisterIndex RegisterIndex;
        }
        public VariableRegister[] Variables;

        public enum RegisterIndex : int { Invalid = int.MaxValue }


        void Acquire(V2Golem golem)
        {

            golem.Cells = new Cell[Cells.Length];
            for (int i = 0; i < Cells.Length; ++i)
            {
                // Clone the cell
                var cell = Cells[i].Clone(NextCellID++);
                golem.Cells[i] = cell;
            }

            // Clone all the registers for the new golem
            IRegister[] registers = new IRegister[Registers.Length];
            for (int i = 0; i < Registers.Length; ++i)
            {
                registers[i] = Registers[i].Clone();
            }

            // Create all the variables for the new golem
            golem.Variables = new Dictionary<string, IVariable>();
            IVariable[] registerVariables = new IVariable[Registers.Length];
            for (int i = 0; i < Variables.Length; ++i)
            {
                var variableRegister = Variables[i];
                RegisterIndex registerIndex = variableRegister.RegisterIndex;
                var variable = registers[(int)registerIndex].CreateVariable();

                // This reference is used to hook up fields of objects that reference internal variables
                registerVariables[(int)registerIndex] = variable;

                // This is the table made available for external golems to attach themselves
                golem.Variables[variableRegister.VariableName] = variable;
            }

            for (int i = 0; i < RegisterAssignments.Length; ++i)
            {
                var assignment = RegisterAssignments[i];
                Array targetArray = golem.GetObjectTypeArray(assignment.ObjectType);
                object target = targetArray.GetValue(assignment.TargetIndex);
                FieldInfo field = target.GetType().GetField(assignment.FieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                object value;
                switch (assignment.Type)
                {
                default: throw new InvalidProgramException();
                case AssignmentType.InputRegister:
                case AssignmentType.OutputRegister:         value = registers[(int)assignment.RegisterIndex]; break;
                case AssignmentType.Variable:               value = registerVariables[(int)assignment.RegisterIndex]; break;
                case AssignmentType.DefaultInputRegister:   value = CentralPublishing.GetDefaultInputRegister(field.FieldType); break;
                case AssignmentType.DefaultOutputRegister:  value = CentralPublishing.GetDefaultOutputRegister(field.FieldType); break;
                case AssignmentType.DefaultInputVariable:   value = CentralPublishing.GetDefaultInputVariable(field.FieldType); break;
                case AssignmentType.DefaultOutputVariable:  value = CentralPublishing.GetDefaultOutputVariable(field.FieldType); break;
                }
                field.SetValue(target, value);

                // If this is a cell's input register, give it notice it has an input that changes
                if (assignment.ObjectType == ObjectType.Cell && assignment.Type == AssignmentType.InputRegister)
                {
                    (value as IRegister).AddListener(target as Cell);
                }
            }

            // Go through all the things that have direct variable access and assign their variable fields
            // This is: each aspect references a variable using "Variable<T> NameOfVariable;"
            // Each cell and script can refer to a variable using "Variable<T> _someVariableReference;"
            //  - what these refer to is set by the user of Golem in the Golem editor
            //      we can actually hook up regular registers to these inputs... huh
            //      which means variables on scripts give input-side or output-side hookups and once you connect one, the other goes away
            //  - depending on what you wire the variable to, these can be internal references (either to register or to unnamed variable)
            //   or external references. Same rules as for registers where invalid reference
            //    means assign the default.

        }

        public class CellField
        {
            public int Cell;
            public string Field;
        }

        public class ScriptField
        {
            public int Cell;
            public string Field;
        }

        public class ExternalVariables
        {
            public string VariableName;
            public ObjectRegisterAssignment[] RegisterAssignments;
        }
        public Dictionary<string, ExternalVariables[]> ExternalReferences;
    }
}

namespace NewLabkit
{
    public static class EndProgramPhase
    {
        public static List<IVariable> Changed;

        public static void OnEndProgramPhase()
        {
            for (int i = 0; i < Changed.Count; ++i)
            {
                Changed[i].OnEndProgramPhase();
            }
        }
    }

    public interface IVariable
    {
        void Acquire(Dictionary<Type, Array> typeValues, int index, int[] cellsToDirty, bool[] dirtyArray);
        void AddListener(Dictionary<Type, Array> typeValues, int index, int[] cellsToDirty, bool[] dirtyArray);
        void RemoveListener(bool[] dirtyArray);
        void OnEndProgramPhase();
    }

    public class Golem
    {
        public GolemClass GolemClass;
        public Dictionary<string, IVariable> Variables;
        public Dictionary<Type, Array> RegisterValues;
        public bool[] DirtyCells;

        public Dictionary<string, Dictionary<string, IVariable>> References;

        public GGEZ.Labkit.Cell[] Cells;
        public GGEZ.Labkit.Aspect[] Aspects;

        public void Acquire()
        {
            RegisterValues = new Dictionary<Type, Array>();
            for (int i = 0; i < GolemClass.RegisterTypes.Length; ++i)
            {
                Type elementType = GolemClass.RegisterTypes[i].ElementType;
                Array array = Array.CreateInstance(elementType, GolemClass.RegisterTypes[i].Length);
                RegisterValues.Add(elementType, array);
            }

            Cells = new GGEZ.Labkit.Cell[GolemClass.Cells.Length];
            for (int i = 0; i < Cells.Length; ++i)
            {
                Cells[i] = GolemClass.Cells[i].Clone();
            }

            // This function is more of a load than an acquire: we don't have to redo
            // this behavior if the object is pooled. This is one-time setup.

            // Hook up cell inputs to registers
            {
                var fields = GolemClass.CellRegisterFieldsIn;
                for (int fieldIndex = 0; fieldIndex < fields.Length; ++fieldIndex)
                {
                    var cell = Cells[fields[fieldIndex].CellIndex];
                    var fieldInfo = cell.GetType().GetField(fields[fieldIndex].FieldName);
                    object boxedStruct = Activator.CreateInstance(fieldInfo.FieldType, RegisterValues, fields[fieldIndex].Register);
                    fieldInfo.SetValue(cell, boxedStruct);
                }
            }

            // Hook up cell outputs to registers
            {
                var fields = GolemClass.CellRegisterFieldsOut;
                for (int fieldIndex = 0; fieldIndex < fields.Length; ++fieldIndex)
                {
                    var cell = Cells[fields[fieldIndex].CellIndex];
                    var fieldInfo = cell.GetType().GetField(fields[fieldIndex].FieldName);
                    object boxedStruct = Activator.CreateInstance(fieldInfo.FieldType, RegisterValues, fields[fieldIndex].Register, fields[fieldIndex].CellsToDirty, DirtyCells);
                    fieldInfo.SetValue(cell, boxedStruct);
                }
            }

            // TODO: hook all Aspect references in scripts and cells directly to the aspects in this object

            // Hook variables into all of the aspects
            // TODO: hook into all of the scripts and cells as well
            Variables = new Dictionary<string, IVariable>();
            for (int aspectIndex = 0; aspectIndex < Aspects.Length; ++aspectIndex)
            {
                var aspect = Aspects[aspectIndex];
                var type = aspect.GetType();
                var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                for (int fieldIndex = 0; fieldIndex < fields.Length; ++fieldIndex)
                {
                    FieldInfo field = fields[fieldIndex];
                    bool isVariable = field.FieldType.IsAssignableFrom(typeof(IVariable));
                    if (!isVariable)
                    {
                        continue;
                    }
                    IVariable variable;
                    string name = GetVariableFieldName(field);
                    if (!Variables.TryGetValue(name, out variable))
                    {
                        variable = Activator.CreateInstance(field.FieldType) as IVariable;
                        Variables.Add(name, variable);
                    }
                    field.SetValue(aspect, variable);
                }
            }

            // Assign all of the variables to registers
            foreach (var kvp in GolemClass.VariableRegisters)
            {
                IVariable variable;
                if (Variables.TryGetValue(kvp.Key, out variable))
                {
                    variable.Acquire(RegisterValues, kvp.Value.Register, kvp.Value.CellsToDirty, DirtyCells);
                }
            }
        }

        public static string GetVariableFieldName(FieldInfo field)
        {
            return field.Name;
        }

        public void SetReference(string reference, Dictionary<string, IVariable> variables)
        {
            List<GolemClass.ExternalVariableReference> externalVariables;
            if (GolemClass.ExternalVariables.TryGetValue(reference, out externalVariables))
            {

                // Remove listeners from the previous variables on this reference
                Dictionary<string, IVariable> oldVariables = null;
                if (References.TryGetValue(reference, out oldVariables))
                {
                    foreach (GolemClass.ExternalVariableReference externalReference in externalVariables)
                    {
                        IVariable variable;
                        if (oldVariables.TryGetValue(externalReference.Variable, out variable))
                        {
                            variable.RemoveListener(DirtyCells);
                        }
                    }
                }

                // Add listeners to the new variables on this reference
                foreach (GolemClass.ExternalVariableReference externalReference in externalVariables)
                {
                    IVariable variable;
                    if (variables.TryGetValue(externalReference.Variable, out variable))
                    {
                        variable.AddListener(RegisterValues, externalReference.Register, externalReference.CellsToDirty, DirtyCells);
                    }
                }
            }
        }

        public void Release()
        {
            foreach (var kvp in GolemClass.ExternalVariables)
            {
                Dictionary<string, IVariable> variables = null;
                if (References.TryGetValue(kvp.Key, out variables))
                {
                    List<GolemClass.ExternalVariableReference> externalVariables = kvp.Value;
                    for (int i = 0; i < externalVariables.Count; ++i)
                    {
                        IVariable variable;
                        if (variables.TryGetValue(externalVariables[i].Variable, out variable))
                        {
                            variable.RemoveListener(DirtyCells);
                        }
                    }
                }
            }
        }
    }

    public class GolemClass
    {
        // Objects from which cells in golems are cloned
        public GGEZ.Labkit.Cell[] Cells;

        public struct RegisterTypeArray
        {
            public readonly Type ElementType;
            public readonly int Length;
        }

        public RegisterTypeArray[] RegisterTypes;

        // The register index and local-cell dirty data to use for each variable
        public struct VariableRegisterData
        {
            public readonly int Register;
            public readonly int[] CellsToDirty;
        }
        public Dictionary<string, VariableRegisterData> VariableRegisters;

        public struct CellRegisterFieldIn
        {
            public readonly int CellIndex;
            public readonly string FieldName;
            public readonly int Register;
        }

        public CellRegisterFieldIn[] CellRegisterFieldsIn;

        public struct CellRegisterFieldOut
        {
            public readonly int CellIndex;
            public readonly string FieldName;
            public readonly int Register;

            /// <remarks>If multiple cells write the same register, this field will be
            /// duplicated. That shouldn't happen, though, so we don't optimize for it.</remarks>
            public readonly int[] CellsToDirty;
        }

        public CellRegisterFieldOut[] CellRegisterFieldsOut;

        public struct ExternalVariableReference
        {
            public readonly string Variable;
            public readonly int Register;
            public readonly int[] CellsToDirty;
        }

        /// Maps reference string to a list of variables to listen for from that reference
        public Dictionary<string, List<ExternalVariableReference>> ExternalVariables;
    }

    public sealed class ClassVariable<T> : IVariable where T : class
    {
        public void Acquire(Dictionary<Type, Array> typeValues, int index, int[] cellsToDirty, bool[] dirtyArray)
        {
            Array valuesArray;
            typeValues.TryGetValue(typeof(T), out valuesArray);
            _values = valuesArray as T[];
            Debug.Assert(_values != null);
            _index = index;
            _cellsToDirty = cellsToDirty;
            _dirtyArray = dirtyArray;
        }

        public T Get()
        {
            return _currentValue;
        }

        public void Set(T value)
        {
            _values[_index] = value;
            EndProgramPhase.Changed.Add(this);
        }

        private T[] _values;

        private int _index;

        private T _currentValue;

        /// <remarks>One per Golem type. For local listeners.</remarks>
        private int[] _cellsToDirty;

        /// <remarks>One per Golem instance. For local listeners.</remarks>
        private bool[] _dirtyArray;

        /// <remarks>Internal references list updated when this variable changes</remarks>
        private List<VariableListener<T>> _onChanged = new List<VariableListener<T>>();

        public void AddListener(Dictionary<Type, Array> typeValues, int index, int[] cellsToDirty, bool[] dirtyArray)
        {
            var listener = new VariableListener<T>();
            listener.Acquire(typeValues, index, cellsToDirty, dirtyArray);
            _onChanged.Add(listener);
        }

        public void RemoveListener(bool[] dirtyArray)
        {
            for (int i = _onChanged.Count - 1; i >= 0; --i)
            {
                if (_onChanged[i].PointsTo(dirtyArray))
                {
                    _onChanged.RemoveAt(i);
                    return; // only 1 per golem
                }
            }
        }

        public void OnEndProgramPhase()
        {
            // Get the new value that is already stored in the registers array
            T value = _values[_index];
            _currentValue = value;

            // Update local dirty cells
            for (int i = 0; i < _cellsToDirty.Length; ++i)
            {
                _dirtyArray[_cellsToDirty[i]] = true;
            }

            // Update remote value & dirty cells
            for (int i = _onChanged.Count - 1; i >= 0; --i)
            {
                _onChanged[i].OnDidChange(value);
            }
        }
    }

    public interface IVariableListener
    {
        void Acquire(Dictionary<Type, Array> typeValues, int index, int[] cellsToDirty, bool[] dirtyArray);

        /// <remarks>dirtyArray is unique per-golem and already stored in the VariableListener.</remarks>
        bool PointsTo(bool[] dirtyArray);
    }

    public sealed class VariableListener<T> : IVariableListener
    {
        /// Values array in the receiver golem
        T[] _values;

        /// Target in values array in the receiver golem
        int _index;

        /// <remarks>One per Golem type</remarks>
        int[] _cellsToDirty;

        /// <remarks>One per Golem instance</remarks>
        bool[] _dirtyArray;

        public void Acquire(Dictionary<Type, Array> typeValues, int index, int[] cellsToDirty, bool[] dirtyArray)
        {
            Array valuesArray;
            typeValues.TryGetValue(typeof(T), out valuesArray);
            _values = valuesArray as T[];
            Debug.Assert(_values != null);
            _index = index;
            _cellsToDirty = cellsToDirty;
            _dirtyArray = dirtyArray;
        }

        public bool PointsTo(bool[] dirtyArray)
        {
            return object.ReferenceEquals(dirtyArray, _dirtyArray);
        }

        /// <summary>Propagates a change in a variable to a register in a listening golem</summary>
        /// <param name="value">The new value of the variable being listened to</param>
        public void OnDidChange(T value)
        {
            _values[_index] = value;
            for (int i = 0; i < _cellsToDirty.Length; ++i)
            {
                _dirtyArray[_cellsToDirty[i]] = true;
            }
        }
    }

    public struct RegisterIn<T>
    {
        private readonly T[] _values;
        private readonly int _index;

        public RegisterIn(Dictionary<Type, Array> values, int index)
        {
            _values = values[typeof(T)] as T[];
            _index = index;
        }

        public T GetValue()
        {
            return _values == null ? default(T) : _values[_index];
        }

        public T GetValue(T defaultValue)
        {
            return _values == null ? defaultValue : _values[_index];
        }

        public bool TryGetValue(out T value)
        {
            if (_values == null)
            {
                value = default(T);
                return false;
            }
            value = _values[_index];
            return true;
        }

        public bool TryGetValue(out T value, T defaultValue)
        {
            if (_values == null)
            {
                value = defaultValue;
                return false;
            }
            value = _values[_index];
            return true;
        }

        public bool TryUpdateValue(ref T value)
        {
            if (_values == null)
            {
                return false;
            }
            value = _values[_index];
            return true;
        }
    }

    public struct ClassRegisterOut<T> where T : class
    {
        T[] _values;

        int _index;

        /// <remarks>One per Golem type</remarks>
        // might not need this field if cells just check their own dirty flags if they care
        int[] _cellsToDirty;

        /// <remarks>One per Golem instance</remarks>
        bool[] _dirtyArray;

        public ClassRegisterOut(Dictionary<Type, Array> values, int index, int[] cellsToDirty, bool[] dirtyArray)
        {
            _values = values[typeof(T)] as T[];
            _index = index;
            _cellsToDirty = cellsToDirty;
            _dirtyArray = dirtyArray;
        }

        public void SetValue(T value)
        {
            if (_values == null)
            {
                return;
            }
            _values[_index] = value;
            for (int i = 0; i < _cellsToDirty.Length; ++i)
            {
                _dirtyArray[_cellsToDirty[i]] = true;
            }
        }
    }
}


namespace GGEZ.Labkit
{

    public interface IVariable
    {
        int RuntimeVariable { get; set; }
    }

    public interface IMutableVariable
    {
        void EndProgramPhase();
    }

    public enum CellIndex : int { Invalid = int.MaxValue }

    public static class EndProgramPhase
    {
        public delegate void Callback();
        public static event Callback Callbacks;

        public static void Invoke()
        {
            if (Callbacks != null)
            {
                Callbacks.Invoke();
            }
        }

        // These should be priority queues
        public static HashSet<CellIndex> DirtyCells = new HashSet<CellIndex>();
        public static HashSet<CellIndex> UpdatingCells = new HashSet<CellIndex>();


        public static void SetDirty(List<CellIndex> cellsToDirty)
        {
            for (int i = 0; i < cellsToDirty.Count; ++i)
            {
                DirtyCells.Add(cellsToDirty[i]);
            }
        }

        public static void SetDirty(CellIndex[] cellsToDirty, int begin, int end)
        {
            for (int i = begin; i < end; ++i)
            {
                DirtyCells.Add(cellsToDirty[i]);
            }
        }
    }

    public class RuntimeVariables<T>
    {
        /// <remarks>
        /// Value at index [0] in this array is always default(T) so that references to
        /// unassigned values always read something.
        /// </remarks>
        public static List<T> Values = new List<T>();

        /// <remarks>
        /// This class is used for variables and registers. Registers mostly use this
        /// listeners array, but some registers are shortcut to read directly from
        /// variables values via references in the circuit.
        /// </remarks>
        public static List<List<CellIndex>> Listeners = new List<List<CellIndex>>();

        public static List<T> NextValues = new List<T>();
        public static List<int> NextValueIndices = new List<int>();


        static RuntimeVariables()
        {
            Debug.Log("Running static constructor for type " + typeof(T).Name);
            Values.Add(default(T));
            EndProgramPhase.Callbacks +=
                typeof(T).IsValueType
                    ? (EndProgramPhase.Callback)RuntimeVariables<T>.EndProgramPhaseStructType
                    : (EndProgramPhase.Callback)RuntimeVariables<T>.EndProgramPhaseClassType;
        }

        public static void EndProgramPhaseClassType()
        {
            Debug.Assert(NextValues.Count == NextValueIndices.Count);
            for (int i = 0; i < NextValues.Count; ++i)
            {
                int index = NextValueIndices[i];
                T nextValue = NextValues[i];
                Values[index] = nextValue;
                EndProgramPhase.SetDirty(Listeners[index]);
            }
            NextValues.Clear();
            NextValueIndices.Clear();
            Debug.Assert(object.Equals(Values[0], default(T)));
            Values[0] = default(T);
        }

        public static void EndProgramPhaseStructType()
        {
            Debug.Assert(NextValues.Count == NextValueIndices.Count);
            for (int i = 0; i < NextValues.Count; ++i)
            {
                int index = NextValueIndices[i];
                T nextValue = NextValues[i];
                if (!Values[index].Equals(nextValue))
                {
                    Values[index] = NextValues[i];
                    EndProgramPhase.SetDirty(Listeners[index]);
                }
            }
            NextValues.Clear();
            NextValueIndices.Clear();
            Debug.Assert(Values[0].Equals(default(T)));
            Values[0] = default(T);
        }
    }

    public struct Variable<T>
    {
        public readonly int RuntimeVariable;

        public T Value
        {
            get
            {
                return RuntimeVariables<T>.Values[RuntimeVariable];
            }
            set
            {
                RuntimeVariables<T>.NextValues.Add(value);
                RuntimeVariables<T>.NextValueIndices.Add(RuntimeVariable);
            }
        }
    }


    public interface IVariableRef
    {
        // Type GetTargetType();
        void UpdateReference(Golem golem);
        void UpdateReference(Golem golem, CellIndex cellIndex);
    }

    public sealed class VariableRef<T> : IVariableRef
    {
        public T Value
        {
            get
            {
                return RuntimeVariables<T>.Values[_runtimeVariable];
            }
            set
            {
                RuntimeVariables<T>.NextValues.Add(value);
                RuntimeVariables<T>.NextValueIndices.Add(_runtimeVariable);
            }
        }

        public readonly string Reference;
        public readonly string Name;
        private int _runtimeVariable;

        public Type GetTargetType()
        {
            return typeof(T);
        }

        public void UpdateReference(Golem golem)
        {
            Dictionary<string, int> variableMap;
            if (golem.NewRelationships.TryGetValue(Reference, out variableMap))
            {
                variableMap.TryGetValue(Name, out _runtimeVariable);
            }
            else
            {
                _runtimeVariable = 0;
            }
            Debug.Assert(RuntimeVariables<T>.Values.Count > _runtimeVariable);
        }

        public void UpdateReference(Golem golem, CellIndex cellIndex)
        {
            Dictionary<string, int> variableMap;
            int runtimeVariable;
            if (golem.NewRelationships.TryGetValue(Reference, out variableMap))
            {
                variableMap.TryGetValue(Name, out runtimeVariable);
            }
            else
            {
                runtimeVariable = 0;
            }
            Debug.Assert(RuntimeVariables<T>.Values.Count > runtimeVariable);
            if (_runtimeVariable != runtimeVariable)
            {
                if (_runtimeVariable != 0)
                {
                    RuntimeVariables<T>.Listeners[_runtimeVariable].Remove(cellIndex);
                }
                if (runtimeVariable != 0)
                {
                    RuntimeVariables<T>.Listeners[_runtimeVariable].Add(cellIndex);
                }
                _runtimeVariable = runtimeVariable;
            }
        }
    }

    public struct RegisterIn<T>
    {
        public T Value
        {
            get
            {
                return RuntimeVariables<T>.Values[_runtimeVariable];
            }
        }

        public bool IsValid
        {
            get
            {
                return _runtimeVariable > 0;
            }
        }

        public bool TryGetValue(out T value)
        {
            if (_runtimeVariable == 0)
            {
                value = default(T);
                return false;
            }
            value = RuntimeVariables<T>.Values[_runtimeVariable];
            return true;
        }

        public void GetValueIfValid(ref T value)
        {
            if (_runtimeVariable > 0)
            {
                value = RuntimeVariables<T>.Values[_runtimeVariable];
            }
        }

        private readonly int _runtimeVariable;

        public RegisterIn(int runtimeVariable)
        {
            _runtimeVariable = runtimeVariable;
        }
    }

    public struct ClassRegisterOut<T> where T : class
    {
        public T Value
        {
            set
            {
                if (_runtimeVariable == 0)
                {
                    return;
                }
                RuntimeVariables<T>.Values[_runtimeVariable] = value;
                EndProgramPhase.SetDirty(RuntimeVariables<T>.Listeners[_runtimeVariable]);
            }
        }

        private readonly int _runtimeVariable;

        public ClassRegisterOut(int runtimeVariable)
        {
            _runtimeVariable = runtimeVariable;
        }
    }

    public struct StructRegisterOut<T> where T : struct, IEquatable<T>
    {
        public T Value
        {
            set
            {
                if (_runtimeVariable == 0)
                {
                    return;
                }

                T current = RuntimeVariables<T>.Values[_runtimeVariable];
                if (current.Equals(value))
                {
                    return;
                }

                RuntimeVariables<T>.Values[_runtimeVariable] = value;
                EndProgramPhase.SetDirty(RuntimeVariables<T>.Listeners[_runtimeVariable]);
            }
        }

        public int _runtimeVariable;

        public StructRegisterOut(int runtimeVariable)
        {
            _runtimeVariable = runtimeVariable;
        }
    }

    /*

    in the Circuit update, iterate dirty cells and cells that are updating

    while (CentralPublishing.DirtyCells.Count > 0)
    {
        Cells[CentralPublishing.DirtyCells.PopFront()]
    }

     */

    public class Variables
    {
        // This set is for propagating changes in variable values
        public HashSet<string> NextFrameChanged = new HashSet<string>();

        // This set is for waking up state machines that are
        // sleeping on variables in order to transition.
        public HashSet<string> Changed = new HashSet<string>();

        public Dictionary<string, object> Values = new Dictionary<string, object>();
        public Dictionary<string, object> NextFrameValues = new Dictionary<string, object>();

#if UNITY_EDITOR
        // InspectorGet that does not set a default value
        public object InspectorGet(string name, Type type)
        {
            object retval;
            if (name != null && Values.TryGetValue(name, out retval) && type.IsAssignableFrom(retval.GetType()))
            {
                return retval;
            }
            retval = type.IsValueType ? Activator.CreateInstance(type) : null;
            Values[name] = retval;
            NextFrameValues[name] = retval;
            return null;
        }

        public void InspectorSet(string name, Type type, object value)
        {
            Values[name] = value;
            NextFrameValues[name] = value;
        }

        public void EditorGUIInspectVariables()
        {
            foreach (var variable in Values.Keys)
            {

            }
        }
#endif

        /// <summary>
        /// Writes a value into a standard variable slot. The variable is marked
        /// as changed if it modifies the value in this slot or if the variable
        /// is a class type.
        /// </summary>
        /// <remarks>
        /// Object equality being unchecked is not a strict requirement but it
        /// seems to make sense right now.
        /// </remarks>
        public void Set(string name, object value)
        {
            object currentNextFrame;
            bool different =

                // Is there any value at all?
                !Values.TryGetValue(name, out currentNextFrame)

                // Are we changing whether the value is null?
                || (value == null) != (currentNextFrame == null)

                // Is the new or old value an object type?
                || !(value ?? currentNextFrame).GetType().IsValueType

                // Are the value types unequal?
                || !object.Equals(currentNextFrame, value);

            NextFrameValues[name] = value;

            if (different)
            {
                NextFrameChanged.Add(name);
            }
        }

        public object Get(string name)
        {
            object value;
            Values.TryGetValue(name, out value);
            return value;
        }

        public bool Get<T>(string name, ref T value)
        {
            object objValue;
            if (Values.TryGetValue(name, out objValue))
            {
                value = (T)objValue;
                return true;
            }
            return false;
        }

        private HashSetVariable<T> getHashSetVariable<T>(string name)
        {
            object value;
            HashSetVariable<T> retval;
            if (!Values.TryGetValue(name, out value))
            {
                Values[name] = retval = new HashSetVariable<T>();
            }
            else
            {
                Debug.Assert(value is HashSetVariable<T>);
                retval = value as HashSetVariable<T>;
            }
            return retval;
        }

        public void SetKeyAdd<T>(string name, T element)
        {
            if (getHashSetVariable<T>(name).Add(element))
            {
                NextFrameChanged.Add(name);
            }
        }

        public void SetKeyRemove<T>(string name, T element)
        {
            if (getHashSetVariable<T>(name).Remove(element))
            {
                NextFrameChanged.Add(name);
            }
        }

        public HashSet<T> SetKeyGetValues<T>(string name)
        {
            return getHashSetVariable<T>(name).Values;
        }

        public HashSet<T> SetKeyGetAdded<T>(string name)
        {
            return getHashSetVariable<T>(name).Added;
        }

        public HashSet<T> SetKeyGetRemoved<T>(string name)
        {
            return getHashSetVariable<T>(name).Removed;
        }

        /// <summary>
        /// Called by a Golem once processing completes at the
        /// end of a frame to advance variable state based on
        /// changes that were requested this frame.
        /// </summary>
        public void EndFrame()
        {
            foreach (var key in NextFrameChanged)
            {
                var value = NextFrameValues[key];
                var endFrame = value as HasEndFrame;
                if (endFrame != null)
                {
                    endFrame.EndFrame();
                }
                Values[key] = value;
            }

            {
                var swap = Changed;
                Changed = NextFrameChanged;
                swap.Clear();
                NextFrameChanged = swap;
            }
        }
    }

    public interface HasEndFrame
    {
        void EndFrame();
    }

    public class HashSetVariable<T> : HasEndFrame
    {
        public HashSet<T> Values { get { return _values; } }
        public HashSet<T> Added { get { return _added; } }
        public HashSet<T> Removed { get { return _removed; } }

        private HashSet<T> _values = new HashSet<T>();
        private HashSet<T> _added = new HashSet<T>();
        private HashSet<T> _removed = new HashSet<T>();

        // TODO: don't actually need to store NextFrameValues since it is === _values + _nextFrameAdded - _nextFrameRemoved
        private HashSet<T> _nextFrameValues = new HashSet<T>();
        private HashSet<T> _nextFrameAdded = new HashSet<T>();
        private HashSet<T> _nextFrameRemoved = new HashSet<T>();

        public bool Add(T element)
        {
            if (!_nextFrameValues.Add(element))
            {
                return false;
            }

            if (_values.Contains(element))
            {
                Debug.Assert(_nextFrameRemoved.Contains(element));
                _nextFrameRemoved.Remove(element);
            }
            else
            {
                Debug.Assert(!_nextFrameAdded.Contains(element));
                _nextFrameAdded.Add(element);
            }

            return true;
        }

        public void Add(IEnumerable<T> elements)
        {
            foreach (T element in elements)
            {
                Add(element);
            }
        }

        public bool Remove(T element)
        {
            if (!_nextFrameValues.Remove(element))
            {
                return false;
            }

            if (_values.Contains(element))
            {
                Debug.Assert(!_nextFrameRemoved.Contains(element));
                _nextFrameRemoved.Add(element);
            }
            else
            {
                Debug.Assert(_nextFrameAdded.Contains(element));
                _nextFrameAdded.Remove(element);
            }

            return true;
        }

        public void Remove(IEnumerable<T> elements)
        {
            foreach (T element in elements)
            {
                Remove(element);
            }
        }


        public void Set(IEnumerable<T> elements)
        {
            HashSet<T> toRemove = new HashSet<T>(_nextFrameValues);
            toRemove.ExceptWith(elements);
            foreach (T element in toRemove)
            {
                Remove(element);
            }

            foreach (T element in elements)
            {
                Add(element);
            }
        }

        public void EndFrame()
        {
            _values.UnionWith(_nextFrameAdded);
            _values.ExceptWith(_nextFrameRemoved);

            {
                _added.Clear();
                var swap = _added;
                _added = _nextFrameAdded;
                _nextFrameAdded = swap;
            }

            {
                _removed.Clear();
                var swap = _removed;
                _removed = _nextFrameRemoved;
                _nextFrameRemoved = swap;
            }
        }
    }
}
