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


namespace GGEZ.Labkit
{

    public static class GolemManager
    {
        private static Dictionary<string, IVariable> s_emptyVariables = new Dictionary<string, IVariable>();

        private static List<Golem2> s_livingGolems = new List<Golem2>();

        public static Golem2 CreateGolem(Golem2 prefab)
        {
            GameObject gameObject = GameObject.Instantiate(prefab.gameObject);
            Golem2 golem = gameObject.GetComponent<Golem2>();
            s_livingGolems.Add(golem);
            GolemArchetype archetype = golem.Archetype;

            // Triggers
            golem.Triggers = new bool[(int)Trigger.__COUNT__];

            // Create aspects
            golem.Aspects = new Aspect[archetype.Aspects.Length];
            for (int i = 0; i < golem.Aspects.Length; ++i)
            {
                golem.Aspects[i] = archetype.Aspects[i].Clone();
            }

            // Assign UnityObjects and settings to aspects. Create variables from aspects.
            DoAssignments(golem, archetype.Assignments, golem.Variables, null, null);

            // Create data for all the components
            golem.Components = new Golem2.ComponentData[archetype.Components.Length];
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

                // Initialize the cells
                for (int j = 0; j < to.Cells.Length; ++j)
                {
                    to.Cells[j].Acquire();
                }

                // Initialize the scripts
                for (int j = 0; j < to.Scripts.Length; ++j)
                {
                    to.Scripts[j].Acquire();
                }
            }

            return golem;
        }

        /// <summary>Attach external references to the given relationship target</summary>
        public static void SetRelationship(Golem2 golem, string relationship, Dictionary<string, IVariable> variables)
        {
            GolemArchetype archetype = golem.Archetype;
            Assignment[] assignments;

            if (archetype.ExternalAssignments.TryGetValue(relationship, out assignments))
            {
                DoAssignments(golem, assignments, variables, null, null);
            }

            for (int i = 0; i < archetype.Components.Length; ++i)
            {
                var component = archetype.Components[i];
                if (component.ExternalAssignments.TryGetValue(relationship, out assignments))
                {
                    DoAssignments(golem, assignments, variables, golem.Components[i], null);
                }
            }
        }

        private static void DoAssignments(Golem2 golem, Assignment[] assignments, Dictionary<string, IVariable> variables, Golem2.ComponentData component, IRegister[] registers)
        {
            IVariable[] registerVariables = null;
            for (int assignmentIndex = 0; assignmentIndex < assignments.Length; ++assignmentIndex)
            {
                Assignment assignment = assignments[assignmentIndex];

                object target;
                FieldInfo fieldInfo;

                switch (assignment.Type)
                {
                    case AssignmentType.AspectGolem:  assignment.GetObjectFieldInfo(golem.Aspects,     out target, out fieldInfo).SetValue(target, golem); break;
                    case AssignmentType.CellGolem:    assignment.GetObjectFieldInfo(component.Cells,   out target, out fieldInfo).SetValue(target, golem); break;
                    case AssignmentType.ScriptGolem:  assignment.GetObjectFieldInfo(component.Scripts, out target, out fieldInfo).SetValue(target, golem); break;

                    
                    case AssignmentType.AspectUnityObject:  assignment.GetObjectFieldInfo(golem.Aspects,     out target, out fieldInfo).SetValue(target, golem.GetReference(assignment.Name)); break;
                    case AssignmentType.CellUnityObject:    assignment.GetObjectFieldInfo(component.Cells,   out target, out fieldInfo).SetValue(target, golem.GetReference(assignment.Name)); break;
                    case AssignmentType.ScriptUnityObject:  assignment.GetObjectFieldInfo(component.Scripts, out target, out fieldInfo).SetValue(target, golem.GetReference(assignment.Name)); break;

                    case AssignmentType.AspectSetting:  assignment.GetObjectFieldInfo(golem.Aspects,     out target, out fieldInfo).SetValue(target, golem.Archetype.Settings.Get(assignment.Name, fieldInfo.FieldType)); break;
                    case AssignmentType.CellSetting:    assignment.GetObjectFieldInfo(component.Cells,   out target, out fieldInfo).SetValue(target, golem.Archetype.Settings.Get(assignment.Name, fieldInfo.FieldType)); break;
                    case AssignmentType.ScriptSetting:  assignment.GetObjectFieldInfo(component.Scripts, out target, out fieldInfo).SetValue(target, golem.Archetype.Settings.Get(assignment.Name, fieldInfo.FieldType)); break;

                    case AssignmentType.CellAspect:     assignment.GetObjectFieldInfo(component.Cells,   out target, out fieldInfo).SetValue(target, golem.GetAspect(fieldInfo.FieldType)); break;
                    case AssignmentType.ScriptAspect:   assignment.GetObjectFieldInfo(component.Scripts, out target, out fieldInfo).SetValue(target, golem.GetAspect(fieldInfo.FieldType)); break;

                    case AssignmentType.AspectVariable: assignment.GetObjectFieldInfo(golem.Aspects,     out target, out fieldInfo).SetValue(target, GetOrCreateVariable(variables, assignment.Name, fieldInfo.FieldType)); break;
                    case AssignmentType.CellVariable:   assignment.GetObjectFieldInfo(component.Cells,   out target, out fieldInfo).SetValue(target, GetOrCreateVariable(variables, assignment.Name, fieldInfo.FieldType)); break;
                    case AssignmentType.ScriptVariable: assignment.GetObjectFieldInfo(component.Scripts, out target, out fieldInfo).SetValue(target, GetOrCreateVariable(variables, assignment.Name, fieldInfo.FieldType)); break;

                    case AssignmentType.AspectVariableOrNull:   assignment.GetObjectFieldInfo(golem.Aspects,     out target, out fieldInfo).SetValue(target, GetVariableOrNull(variables, assignment.Name)); break;
                    case AssignmentType.CellVariableOrNull:     assignment.GetObjectFieldInfo(component.Cells,   out target, out fieldInfo).SetValue(target, GetVariableOrNull(variables, assignment.Name)); break;
                    case AssignmentType.ScriptVariableOrNull:   assignment.GetObjectFieldInfo(component.Scripts, out target, out fieldInfo).SetValue(target, GetVariableOrNull(variables, assignment.Name)); break;

                    case AssignmentType.AspectVariableOrDummy:   assignment.GetObjectFieldInfo(golem.Aspects,     out target, out fieldInfo).SetValue(target, GetVariableOrDummy(variables, assignment.Name, fieldInfo.FieldType)); break;
                    case AssignmentType.CellVariableOrDummy:     assignment.GetObjectFieldInfo(component.Cells,   out target, out fieldInfo).SetValue(target, GetVariableOrDummy(variables, assignment.Name, fieldInfo.FieldType)); break;
                    case AssignmentType.ScriptVariableOrDummy:   assignment.GetObjectFieldInfo(component.Scripts, out target, out fieldInfo).SetValue(target, GetVariableOrDummy(variables, assignment.Name, fieldInfo.FieldType)); break;

                    case AssignmentType.CellRegisterVariable:     assignment.GetObjectFieldInfo(component.Cells,   out target, out fieldInfo).SetValue(target, GetOrCreateRegisterVariable(assignment.RegisterIndex, registers, ref registerVariables)); break;
                    case AssignmentType.ScriptRegisterVariable:   assignment.GetObjectFieldInfo(component.Scripts, out target, out fieldInfo).SetValue(target, GetOrCreateRegisterVariable(assignment.RegisterIndex, registers, ref registerVariables)); break;

                    case AssignmentType.CellVariableRegisterOrNull:     assignment.GetObjectFieldInfo(component.Cells,   out target, out fieldInfo).SetValue(target, GetVariableRegisterOrNull(variables, assignment.Name)); break;
                    case AssignmentType.ScriptVariableRegisterOrNull:   assignment.GetObjectFieldInfo(component.Scripts, out target, out fieldInfo).SetValue(target, GetVariableRegisterOrNull(variables, assignment.Name)); break;

                    case AssignmentType.CellInputVariableRegister:
                        {
                            Cell targetCell = component.Cells[assignment.TargetIndex];
                            fieldInfo = assignment.GetFieldInfo(targetCell);
                            IRegister register = GetVariableRegisterOrNull(variables, assignment.Name);
                            if (register == null)
                            {
                                register = GetReadonlyRegister(fieldInfo.FieldType);
                            }
                            else
                            {
                                register.AddListener(targetCell);
                            }
                            fieldInfo.SetValue(targetCell, register);
                        }
                        break;

                    case AssignmentType.ScriptInputVariableRegister:  assignment.GetObjectFieldInfo(component.Scripts, out target, out fieldInfo).SetValue(target, GetVariableRegisterOrNull(variables, assignment.Name) ?? GetReadonlyRegister(fieldInfo.FieldType)); break;

                    case AssignmentType.CellOutputVariableRegister:   assignment.GetObjectFieldInfo(component.Cells,   out target, out fieldInfo).SetValue(target, GetVariableRegisterOrNull(variables, assignment.Name) ?? GetWriteonlyRegister(fieldInfo.FieldType)); break;
                    case AssignmentType.ScriptOutputVariableRegister: assignment.GetObjectFieldInfo(component.Scripts, out target, out fieldInfo).SetValue(target, GetVariableRegisterOrNull(variables, assignment.Name) ?? GetWriteonlyRegister(fieldInfo.FieldType)); break;

                    case AssignmentType.CellInputRegister:
                        {
                            Cell targetCell = component.Cells[assignment.TargetIndex];
                            IRegister register = registers[assignment.RegisterIndex];
                            register.AddListener(targetCell);
                            assignment.GetFieldInfo(targetCell).SetValue(targetCell, register);
                        }
                        break;

                    case AssignmentType.CellOutputRegister:         assignment.GetObjectFieldInfo(component.Cells,   out target, out fieldInfo).SetValue(target, registers[assignment.RegisterIndex]); break;
                    case AssignmentType.ScriptRegister:             assignment.GetObjectFieldInfo(component.Scripts, out target, out fieldInfo).SetValue(target, registers[assignment.RegisterIndex]); break;
                    case AssignmentType.CellDummyInputRegister:     assignment.GetObjectFieldInfo(component.Cells, out target, out fieldInfo).SetValue(target, GetReadonlyRegister(fieldInfo.FieldType)); break;
                    case AssignmentType.CellDummyOutputRegister:    assignment.GetObjectFieldInfo(component.Cells, out target, out fieldInfo).SetValue(target, GetWriteonlyRegister(fieldInfo.FieldType)); break;
                    case AssignmentType.CellDummyVariable:          assignment.GetObjectFieldInfo(component.Cells, out target, out fieldInfo).SetValue(target, GetWriteonlyRegister(fieldInfo.FieldType)); break;
                    case AssignmentType.CellDummyInputVariable:     assignment.GetObjectFieldInfo(component.Cells, out target, out fieldInfo).SetValue(target, GetReadonlyVariable(fieldInfo.FieldType)); break;
                    case AssignmentType.CellDummyOutputVariable:    assignment.GetObjectFieldInfo(component.Cells, out target, out fieldInfo).SetValue(target, GetWriteonlyVariable(fieldInfo.FieldType)); break;
                    case AssignmentType.ScriptDummyInputRegister:   assignment.GetObjectFieldInfo(component.Scripts, out target, out fieldInfo).SetValue(target, GetReadonlyRegister(fieldInfo.FieldType)); break;
                    case AssignmentType.ScriptDummyOutputRegister:  assignment.GetObjectFieldInfo(component.Scripts, out target, out fieldInfo).SetValue(target, GetWriteonlyRegister(fieldInfo.FieldType)); break;
                    case AssignmentType.ScriptDummyVariable:        assignment.GetObjectFieldInfo(component.Scripts, out target, out fieldInfo).SetValue(target, GetDummyVariable(fieldInfo.FieldType)); break;
                    case AssignmentType.ScriptDummyInputVariable:   assignment.GetObjectFieldInfo(component.Scripts, out target, out fieldInfo).SetValue(target, GetReadonlyVariable(fieldInfo.FieldType)); break;
                    case AssignmentType.ScriptDummyOutputVariable:  assignment.GetObjectFieldInfo(component.Scripts, out target, out fieldInfo).SetValue(target, GetWriteonlyVariable(fieldInfo.FieldType)); break;
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
            Debug.Assert(registers != null);
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

        private static IRegister GetVariableRegisterOrNull(Dictionary<string, IVariable> variables, string name)
        {
            IVariable variable;

            if (variables.TryGetValue(name, out variable))
            {
                return variable.GetRegister();
            }
            
            return null;
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
        private static IRegister GetReadonlyRegister(Type fieldType)
        {
            Debug.Assert(typeof(IRegister).IsAssignableFrom(fieldType));
            return GetDefaultsForType(fieldType).InputRegister;
        }

        /// <summary></summary>
        private static IRegister GetWriteonlyRegister(Type fieldType)
        {
            Debug.Assert(typeof(IRegister).IsAssignableFrom(fieldType));
            return GetDefaultsForType(fieldType).OutputRegister;
        }

        /// <summary></summary>
        private static IVariable GetReadonlyVariable(Type fieldType)
        {
            Debug.Assert(typeof(IVariable).IsAssignableFrom(fieldType));
            return GetDefaultsForType(fieldType).InputVariable;
        }

        /// <summary></summary>
        private static IVariable GetWriteonlyVariable(Type fieldType)
        {
            Debug.Assert(typeof(IVariable).IsAssignableFrom(fieldType));
            return GetDefaultsForType(fieldType).OutputVariable;
        }

        private static IVariable GetDummyVariable(Type fieldType)
        {
            Debug.Assert(typeof(IVariable).IsAssignableFrom(fieldType));
            IVariable instance = Activator.CreateInstance(fieldType) as IVariable;
            Debug.Assert(instance != null);
            Debug.Assert(instance.GetRegister() != null);
            return instance;
        }
        
        private static List<IVariable> s_changedVariables = new List<IVariable>();
        public static void AddChangedVariable(IVariable variable)
        {
            s_changedVariables.Add(variable);
        }

        private static List<Cell> s_changedCells = new List<Cell>();
        public static void AddChangedCellInputs(List<Cell> cells)
        {
            // add IDs to the priority queue based on their sequencer
            s_changedCells.AddRange(cells);
        }

        public static void OnVariableRollover()
        {
            // go through all changed variables and push their changes so that cells are queued
            for (int i = 0; i < s_changedVariables.Count; ++i)
            {
                s_changedVariables[i].OnEndProgramPhase();
            }
        }

        public static void OnCircuitPhase()
        {
            // go through the cells in order, pop the front and update them
            // add cells that want an update next frame to the next frame's priority queue
            // NOTE: variables changed during circuit phase don't write registers until
            // the end of the frame

            Debug.LogErrorFormat("OnCircuitPhase is REALLY REALLY bad, it sorts the list of cells every cell! Make this into a priority queue!");

            int lastSequencer = -1;
            while (s_changedCells.Count > 0)
            {
                s_changedCells.Sort((Cell lhs, Cell rhs) => lhs.Sequencer - rhs.Sequencer);
                Debug.Assert(s_changedCells[0].Sequencer >= lastSequencer);
                if (s_changedCells[0].Sequencer > lastSequencer)
                {
                    s_changedCells[0].Update();
                }
                s_changedCells.RemoveAt(0);
            }
        }

        public static void OnProgramPhase()
        {
            // go through all the golems and run their programs
            for (int i = 0; i < s_livingGolems.Count; ++i)
            {
                OnProgramPhase(s_livingGolems[i]);
            }
        }

        private static Stack<bool> _stackForProcessingTransitions = new Stack<bool>();
        private static bool EvaluateTransitionExpression(Transition transition, bool[] triggers)
        {
            int triggerPtr = 0;
            Stack<bool> evaluation = _stackForProcessingTransitions;
            evaluation.Clear();
            var expression = transition.Expression;
            for (int k = 0; k < expression.Length; ++k)
            {
                switch (expression[k])
                {
                    case Transition.Operator.And: evaluation.Push(evaluation.Pop() & evaluation.Pop()); break;
                    case Transition.Operator.Or: evaluation.Push(evaluation.Pop() | evaluation.Pop()); break;
                    case Transition.Operator.Push: evaluation.Push(triggers[(int)transition.Triggers[triggerPtr++]]); break;
                    case Transition.Operator.True: evaluation.Push(true); break;
                    case Transition.Operator.False: evaluation.Push(false); break;
                }
            }
            Debug.Assert(evaluation.Count == 1);
            bool shouldTransition = evaluation.Pop();
            return shouldTransition;
        }

        private static List<List<Transition>> s_activeTransitions = new List<List<Transition>>();
        private static List<List<Transition>> s_activeTransitionsCache = new List<List<Transition>>();

        private static void OnProgramPhase(Golem2 golem)
        {
            const int kMaxTransitionsPerFrame = 10;

            bool[] triggers = golem.Triggers;
            GolemArchetype archetype = golem.Archetype;

            //-----------------------------------
            // Transitions
            //-----------------------------------

            while (s_activeTransitionsCache.Count < archetype.Components.Length)
            {
                s_activeTransitionsCache.Add(new List<Transition>());
            }
            s_activeTransitions.Clear();
            s_activeTransitions.AddRange(s_activeTransitionsCache);

            for (int sentinel = 0; sentinel < kMaxTransitionsPerFrame; ++sentinel)
            {
                for (int componentIndex = 0; componentIndex < archetype.Components.Length; ++componentIndex)
                {
                    var activeTransitions = s_activeTransitions[componentIndex];

                    bool componentIsFinishedTransitioning = activeTransitions == null;
                    if (componentIsFinishedTransitioning)
                    {
                        continue;
                    }
                    activeTransitions.Clear();

                    var archetypeComponent = archetype.Components[componentIndex];
                    var instanceComponent = golem.Components[componentIndex];

                    GolemComponent.Layer[] layers = archetypeComponent.Layers;
                    int layerIndex = 0;
                    while (layerIndex < layers.Length)
                    {
                        var layer = layers[layerIndex];
                        
                        for (int i = 0; i < layer.FromAnyStateTransitions.Length; ++i)
                        {
                            Transition transition = layer.FromAnyStateTransitions[i];
                            if (EvaluateTransitionExpression(transition, triggers))
                            {
                                activeTransitions.Add(transition);
                                instanceComponent.LayerStates[layerIndex] = transition.ToState;
                                goto NextLayer;
                            }
                        }

                        Transition[] transitions;
                        if (layer.Transitions.TryGetValue(instanceComponent.LayerStates[layerIndex], out transitions))
                        {
                            for (int i = 0; i < transitions.Length; ++i)
                            {
                                Transition transition = transitions[i];
                                if (EvaluateTransitionExpression(transition, triggers))
                                {
                                    activeTransitions.Add(transition);
                                    instanceComponent.LayerStates[layerIndex] = transition.ToState;
                                    goto NextLayer;
                                }
                            }
                        }

                    NextLayer:
                        ++layerIndex;
                    }

                    componentIsFinishedTransitioning = activeTransitions.Count == 0;
                    if (componentIsFinishedTransitioning)
                    {
                        s_activeTransitions[componentIndex] = null;
                        break;
                    }

                    for (int i = 0; i < activeTransitions.Count; ++i)
                    {
                        var fromState = (int)activeTransitions[i].FromState;
                        if (fromState < 0 || fromState >= archetypeComponent.States.Length)
                        {
                            continue;
                        }
                        var scriptsToTransitionFrom = archetypeComponent.States[fromState].Scripts;
                        for (int j = 0; j < scriptsToTransitionFrom.Length; ++j)
                        {
                            int script = scriptsToTransitionFrom[j];
                            instanceComponent.Scripts[script].OnExit();
                        }
                    }
                }

                for (int i = 0; i < triggers.Length; ++i)
                {
                    triggers[i] = false;
                }

                int componentsTransitioned = 0;

                for (int componentIndex = 0; componentIndex < archetype.Components.Length; ++componentIndex)
                {
                    var activeTransitions = s_activeTransitions[componentIndex];

                    bool componentIsFinishedTransitioning = activeTransitions == null;
                    if (componentIsFinishedTransitioning)
                    {
                        continue;
                    }
                    ++componentsTransitioned;

                    var archetypeComponent = archetype.Components[componentIndex];
                    var instanceComponent = golem.Components[componentIndex];

                    for (int i = 0; i < activeTransitions.Count; ++i)
                    {
                        var toState = (int)activeTransitions[i].ToState;
                        if (toState < 0 || toState >= archetypeComponent.States.Length)
                        {
                            continue;
                        }
                        var scriptsToTransitionTo = archetypeComponent.States[toState].Scripts;
                        for (int j = 0; j < scriptsToTransitionTo.Length; ++j)
                        {
                            int script = scriptsToTransitionTo[j];
                            instanceComponent.Scripts[script].OnEnter();
                        }
                    }
                }

                if (componentsTransitioned == 0)
                {
                    break;
                }

                Debug.Assert(sentinel + 1 < kMaxTransitionsPerFrame);
            }

            //-----------------------------------
            // States
            //-----------------------------------

            for (int componentIndex = 0; componentIndex < archetype.Components.Length; ++componentIndex)
            {
                var activeTransitions = s_activeTransitions[componentIndex];
                var archetypeComponent = archetype.Components[componentIndex];
                var instanceComponent = golem.Components[componentIndex];
                for (int i = 0; i < archetypeComponent.Layers.Length; ++i)
                {
                    int state = (int)instanceComponent.LayerStates[i];
                    if (state < 0 || state >= archetypeComponent.States.Length)
                    {
                        continue;
                    }
                    var scriptsToUpdate = archetypeComponent.States[state].Scripts;
                    for (int j = 0; j < scriptsToUpdate.Length; ++j)
                    {
                        int script = scriptsToUpdate[j];
                        instanceComponent.Scripts[script].OnUpdate();
                    }
                }
            }
        }
    }
}