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

    public class GolemManager : MonoBehaviour
    {
        private List<Golem> _livingGolems = new List<Golem>();

        #warning TODO do we need to weak reference the golem pool?
        private Dictionary<int, List<Golem>> _golemPool = new Dictionary<int, List<Golem>>();

        private List<Variable> _changedVariables = new List<Variable>();

        private static CellPriorityQueue _changedCells = new CellPriorityQueue();

        private List<ICollectionRegister> _changedCollectionRegisters = new List<ICollectionRegister>();

        private List<List<Transition>> _activeTransitions = new List<List<Transition>>();

        private List<List<Transition>> _activeTransitionsCache = new List<List<Transition>>();

        private static GolemManager Instance
        {
            get
            {
                Debug.Assert(Application.isPlaying);
                if (s_instance == null)
                {
                    GameObject go = new GameObject("GolemManager");
                    s_instance = go.AddComponent<GolemManager>();
                    GameObject.DontDestroyOnLoad(go);
                }
                return s_instance;
            }
        }
        private static GolemManager s_instance;

        public static Golem AcquireGolem(Golem prefab)
        {
            Debug.Assert(Application.isPlaying);

        TryAgain:
            Golem golem;
            if (Instance._golemPool.MultiPop(prefab.Archetype.GetInstanceID(), out golem))
            {
                // One situation where this can happen is if the golem is added to the pool then a new scene is loaded
                if (golem == null)
                    goto TryAgain;

                // OnAwake(golem);
                // golem.gameObject.SetActive(true);
                Debug.Log("Golem pooling doesn't correctly wake up the old golem with blanked settings. Calling OnAwake isn't good enough because that recreates everything anyway.");
                GameObject.Destroy(golem.gameObject);
                goto TryAgain;
            }
            else
            {
                GameObject gameObject = GameObject.Instantiate(prefab.gameObject);
                golem = gameObject.GetComponent<Golem>();
            }

            return golem;
        }

        public static Golem AcquireGolem(Golem prefab, Settings localSettings)
        {
            throw new System.NotImplementedException();
        }

        public static void ReleaseGolem(Golem golem)
        {
            Debug.Assert(Application.isPlaying);

            var archetype = golem.Archetype;
            Assignment.Assign(golem, archetype.Assignments, null, null, null);
            for (int i = 0; i < archetype.Components.Length; ++i)
            {
                var from = archetype.Components[i];
                var to = golem.Components[i];

                foreach (var kvp in from.ExternalAssignments)
                {
                    Assignment.Assign(golem, kvp.Value, null, to, null);
                }
            }

            golem.gameObject.SetActive(false);
            Instance._golemPool.MultiAdd(archetype.GetInstanceID(), golem);
        }

        public static void OnAwake(Golem golem)
        {
            Debug.Assert(!Instance._livingGolems.Contains(golem));
            Instance._livingGolems.Add(golem);

            GolemArchetype archetype = golem.Archetype;

            // Triggers
            golem.Triggers = new bool[(int)Trigger.__COUNT__];

            // Create aspects
            golem.Aspects = new Aspect[archetype.Aspects.Length];
            for (int i = 0; i < golem.Aspects.Length; ++i)
            {
                golem.Aspects[i] = archetype.Aspects[i].Clone();
            }

            // Create variables
            golem.Variables = new Dictionary<string, Variable>();
            foreach (var kvp in golem.Archetype.Variables)
            {
                Variable clonedVariable = kvp.Value.Clone();
                QueueForVariableRolloverPhase(clonedVariable);
                golem.Variables.Add(kvp.Key, clonedVariable);
            }

            // Assign UnityObjects and settings to aspects
            Assignment.Assign(golem, archetype.Assignments, golem.Variables, null, null);

            // Create data for all the components
            golem.Components = new GolemComponentRuntimeData[archetype.Components.Length];
            for (int i = 0; i < archetype.Components.Length; ++i)
            {
                var from = archetype.Components[i];
                var to = golem.Components[i] = new GolemComponentRuntimeData();

                to.Cells = new Cell[from.Cells.Length];
                for (int j = 0; j < from.Cells.Length; ++j)
                {
                    to.Cells[j] = from.Cells[j].Clone();
                }
                AddChangedCellInputs(to.Cells);

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

                Register[] registers = new Register[from.Registers.Length];
                for (int j = 0; j < from.Registers.Length; ++j)
                {
                    registers[j] = from.Registers[j].Clone();
                }

                // Save registers for live debugging
            #if UNITY_EDITOR
                to.Registers = registers;
            #endif

                // Assign UnityObjects, variables, registers, settings
                Assignment.Assign(golem, from.Assignments, golem.Variables, to, registers);

                // Reset external references
                foreach (var kvp in from.ExternalAssignments)
                {
                    Assignment.Assign(golem, kvp.Value, null, to, null);
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

        }

        /// <summary>Attach external references to the given relationship target</summary>
        public static void SetRelationship(Golem golem, string relationship, Dictionary<string, Variable> variables)
        {
            GolemArchetype archetype = golem.Archetype;
            Assignment[] assignments;

            if (archetype.ExternalAssignments.TryGetValue(relationship, out assignments))
            {
                Assignment.Assign(golem, assignments, variables, null, null);
            }

            for (int i = 0; i < archetype.Components.Length; ++i)
            {
                var component = archetype.Components[i];
                if (component.ExternalAssignments.TryGetValue(relationship, out assignments))
                {
                    Assignment.Assign(golem, assignments, variables, golem.Components[i], null);
                }
            }
        }

        public static void QueueForVariableRolloverPhase(Variable variable)
        {

        #if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
        #endif

            Instance._changedVariables.Add(variable);
        }

        public static void QueueForCollectionRegisterPhase(ICollectionRegister register)
        {

        #if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
        #endif

            Instance._changedCollectionRegisters.Add(register);
        }

        public static void UpdateCellNextFrame(Cell cell)
        {
            Debug.Assert(Application.isPlaying);
            _changedCells.Add(cell);
        }

        public static void AddChangedCellInputs(IList<Cell> cells)
        {

        #if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
        #endif

            if (cells == null)
                return;

            for (int i = 0; i < cells.Count; ++i)
            {
                _changedCells.Add(cells[i]);
            }
        }

        private void Update()
        {
            CircuitPhase();
            ProgramPhase();
            CollectionRegisterPhase();
            VariableRolloverPhase();
        }

        private void CircuitPhase()
        {
            Cell changedCell;
            while (_changedCells.PopNext(out changedCell))
                changedCell.Update();
        }

        private void ProgramPhase()
        {
            const int kMaxTransitionsPerFrame = 10;

            for (int golemIndex = 0; golemIndex < _livingGolems.Count; ++golemIndex)
            {
                Golem golem = _livingGolems[golemIndex];

                bool[] triggers = golem.Triggers;
                GolemArchetype archetype = golem.Archetype;

                //-----------------------------------
                // Transitions
                //-----------------------------------

                #warning TODO: each golem should only be checked for transitions if a trigger has been set

                // TODO: each golem should be able to set a "wants transition" flag
                //       that can shortcut this check if no triggers are set
                //       (and the last frame didn't hit kMaxTransitionsPerFrame)
                // even better: golems only exist in the list at all if they
                //              need to process transitions

                while (_activeTransitionsCache.Count < archetype.Components.Length)
                    _activeTransitionsCache.Add(new List<Transition>());
                _activeTransitions.Clear();
                _activeTransitions.AddRange(_activeTransitionsCache);

                for (int sentinel = 0; sentinel < kMaxTransitionsPerFrame; ++sentinel)
                {
                    for (int componentIndex = 0; componentIndex < archetype.Components.Length; ++componentIndex)
                    {
                        var activeTransitions = _activeTransitions[componentIndex];

                        bool componentIsFinishedTransitioning = activeTransitions == null;
                        if (componentIsFinishedTransitioning)
                            continue;

                        activeTransitions.Clear();

                        var archetypeComponent = archetype.Components[componentIndex];
                        var instanceComponent = golem.Components[componentIndex];

                        Layer[] layers = archetypeComponent.Layers;
                        int layerIndex = 0;
                        while (layerIndex < layers.Length)
                        {
                            var layer = layers[layerIndex];

                            for (int i = 0; i < layer.FromAnyStateTransitions.Length; ++i)
                            {
                                Transition transition = layer.FromAnyStateTransitions[i];
                                if (transition.Evaluate(triggers))
                                {
                                    activeTransitions.Add(transition);
                                    instanceComponent.LayerStates[layerIndex] = transition.ToState;
                                    goto NextLayer;
                                }
                            }

                            Transition[] transitions;
                            if (layer.Transitions.TryGetValue(instanceComponent.LayerStates[layerIndex], out transitions))
                            {
                                for (int transitionIndex = 0; transitionIndex < transitions.Length; ++transitionIndex)
                                {
                                    Transition transition = transitions[transitionIndex];
                                    if (transition.Evaluate(triggers))
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
                            _activeTransitions[componentIndex] = null;
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

                    // Clear all triggers
                    for (int i = 0; i < triggers.Length; ++i)
                    {
                        triggers[i] = false;
                    }

                    int componentsTransitioned = 0;

                    // Enter the new states in each component
                    for (int componentIndex = 0; componentIndex < archetype.Components.Length; ++componentIndex)
                    {
                        var activeTransitions = _activeTransitions[componentIndex];

                        bool componentIsFinishedTransitioning = activeTransitions == null;
                        if (componentIsFinishedTransitioning)
                        {
                            continue;
                        }
                        ++componentsTransitioned;

                        var archetypeComponent = archetype.Components[componentIndex];
                        var instanceComponent = golem.Components[componentIndex];

                        for (int activeTransitionIndex = 0; activeTransitionIndex < activeTransitions.Count; ++activeTransitionIndex)
                        {
                            int toState = (int)activeTransitions[activeTransitionIndex].ToState;
                            if (toState < 0 || toState >= archetypeComponent.States.Length)
                                continue;

                            var indicesOfScriptsToEnter = archetypeComponent.States[toState].Scripts;
                            for (int j = 0; j < indicesOfScriptsToEnter.Length; ++j)
                            {
                                int script = indicesOfScriptsToEnter[j];
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
                    var archetypeComponent = archetype.Components[componentIndex];
                    var instanceComponent = golem.Components[componentIndex];
                    for (int layerIndex = 0; layerIndex < archetypeComponent.Layers.Length; ++layerIndex)
                    {
                        int stateIndex = (int)instanceComponent.LayerStates[layerIndex];
                        if (stateIndex < 0 || stateIndex >= archetypeComponent.States.Length)
                            continue;
                            
                        var indicesOfScriptsInState = archetypeComponent.States[stateIndex].Scripts;
                        for (int i = 0; i < indicesOfScriptsInState.Length; ++i)
                        {
                            int scriptIndex = indicesOfScriptsInState[i];
                            instanceComponent.Scripts[scriptIndex].OnUpdate();
                        }
                    }
                }
            }
        }


        private void CollectionRegisterPhase()
        {
            for (int i = 0; i < _changedCollectionRegisters.Count; ++i)
            {
                _changedCollectionRegisters[i].OnCollectionRegisterPhase();
            }
            _changedCollectionRegisters.Clear();
        }

        private void VariableRolloverPhase()
        {
            for (int i = 0; i < _changedVariables.Count; ++i)
            {
                _changedVariables[i].OnVariableRolloverPhase();
            }
            _changedVariables.Clear();
        }

    }
}
