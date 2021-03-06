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
using System.Collections.Generic;
using GGEZ.FullSerializer;

namespace GGEZ.Labkit
{
    public partial class Golem : MonoBehaviour
    {
        // The entity that controls the lifespan of this entity
        public Golem Owner;

        // The entity for which this entity was created
        public Golem Subject;

        // The entities most recently found by a search this entity did
        public Golem[] Targets;

        // Contains the template used to create each instance of this entity
        public GolemAsset Asset;

#if UNITY_EDITOR

        // When running in the editor, these values track additional data that
        // is used to construct and edit this entity. For example, the runtime
        // version doesn't care about how wires are displayed visually so all
        // that data is stored here.
        public object EditorData;
        public ScriptableObject EditorAsset;

#endif

        // Automatically-generated references for the JSON written in assets.
        // This list allows us to leverage Unity's automatic reference updates
        // while still doing all of our own serialization.
        public UnityObjectList References = new UnityObjectList();


        //---------------------------------------------------------------------
        // Variables
        //---------------------------------------------------------------------
        [System.NonSerialized]
        public Variables Variables = new Variables();

        //---------------------------------------------------------------------
        // Awake (Unity)
        //---------------------------------------------------------------------
        private void Awake()
        {
            Load();
        }

        private Stack<bool> _stackForProcessingTransitions = new Stack<bool>();

        //---------------------------------------------------------------------
        // Update (Unity)
        //---------------------------------------------------------------------
        private void Update()
        {
            const int kMaxTransitionsPerFrame = 10;

            //-----------------------------------
            // Transitions
            //-----------------------------------

            List<Transition> activeTransitions = new List<Transition>(Layers.Length);
            for (int sentinel = 0; sentinel < kMaxTransitionsPerFrame; ++sentinel)
            {
                for (int i = 0; i < Layers.Length; ++i)
                {
                    var layer = Layers[i];
                    var transitions = layer.Transitions;
                    for (int j = 0; j < transitions.Length; ++j)
                    {
                        var transition = transitions[j];
                        if (transition.FromState != StateIndex.Any && transition.FromState != LayerState[i])
                        {
                            continue;
                        }

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
                                case Transition.Operator.Push: evaluation.Push(Triggers[(int)transition.Triggers[triggerPtr++]]); break;
                                case Transition.Operator.True: evaluation.Push(true); break;
                                case Transition.Operator.False: evaluation.Push(false); break;
                            }
                        }

                        Debug.Assert(evaluation.Count == 1);
                        bool shouldTransition = evaluation.Pop();
                        if (shouldTransition)
                        {
                            activeTransitions.Add(transition);
                            this.LayerState[i] = transition.ToState;
                            break;
                        }
                    }
                }

                if (activeTransitions.Count == 0)
                {
                    break;
                }

                for (int i = 0; i < activeTransitions.Count; ++i)
                {
                    var fromState = (int)activeTransitions[i].FromState;
                    if (fromState < 0 || fromState >= States.Length)
                    {
                        continue;
                    }
                    var scriptsToTransitionFrom = States[fromState].Scripts;
                    for (int j = 0; j < scriptsToTransitionFrom.Length; ++j)
                    {
                        int script = scriptsToTransitionFrom[j];
                        Scripts[script].OnExit(this);
                    }
                }

                for (int i = 0; i < Triggers.Length; ++i)
                {
                    Triggers[i] = false;
                }

                for (int i = 0; i < activeTransitions.Count; ++i)
                {
                    var toState = (int)activeTransitions[i].ToState;
                    if (toState < 0 || toState >= States.Length)
                    {
                        continue;
                    }
                    var scriptsToTransitionTo = States[toState].Scripts;
                    for (int j = 0; j < scriptsToTransitionTo.Length; ++j)
                    {
                        int script = scriptsToTransitionTo[j];
                        Scripts[script].OnEnter(this);
                    }
                }

                activeTransitions.Clear();
            }

            //-----------------------------------
            // States
            //-----------------------------------

            for (int i = 0; i < Layers.Length; ++i)
            {
                int state = (int)LayerState[i];
                if (state < 0 || state >= States.Length)
                {
                    continue;
                }
                var scriptsToUpdate = States[state].Scripts;
                for (int j = 0; j < scriptsToUpdate.Length; ++j)
                {
                    int script = scriptsToUpdate[j];
                    Scripts[script].OnUpdate(this);
                }
            }

            //-----------------------------------
            // Circuit
            //-----------------------------------

            // Input variables

            foreach (string variable in Variables.ChangedLastFrame)
            {
                object value = Variables.Get(variable);
                int[] registersToWrite;
                if (!_variablesThatWriteRegister.TryGetValue(variable, out registersToWrite))
                {
                    // TODO: for now (9/16/18), this will always be the case
                    // since the source array is never saved by the editor
                    continue;
                }
                for (int i = 0; i < registersToWrite.Length; ++i)
                {
                    _registers[i] = value;
                    int[] cellsToDirty = _cellsThatReadRegister[i];
                    for (int j = 0; j < cellsToDirty.Length; ++j)
                    {
                        _dirty[cellsToDirty[j]] = true;
                    }
                }
            }

            // Timers
            //
            // while (head of timer queue expired):
            //      pop timer
            //      set dirty flag for that timer

            // Update

            for (int i = 0; i < _dirty.Length; ++i)
            {
                bool dirty = _dirty[i];
                bool running = _running[i];
                if (dirty || running)
                {
                    _dirty[i] = false;
                    Cells[i].Update(this, dirty, ref running);
                    _running[i] = running;
                }
            }

            //-----------------------------------
            // Variables
            //-----------------------------------

            // By updating variables here and using a second ChangedLastFrame
            // set inside of Variables, we cause the Circuit not to get an
            // update notification until next frame.
            //
            // This means that variables written by cells will overwrite
            // changes to a variable by scripts, and the cells will never
            // have seen the original script variable. This can cause a
            // dirty to be passed to a cell even if its circuit-measured
            // value doesn't change, which is a behavior the circuit must
            // tolerate since it has no memory.
            Variables.EndFrame();

        }

        //---------------------------------------------------------------------
        // Load
        //---------------------------------------------------------------------
        public void Load()
        {
            //-------------------------------------------------
            // Assets
            //-------------------------------------------------
#if UNITY_EDITOR
            if (Asset == null)
            {
                Asset = ScriptableObject.CreateInstance<GolemAsset>();
            }
#endif
            if (Asset == null)
            {
                gameObject.SetActive(false);
                throw new InvalidProgramException("entity has no data");
            }

            //-------------------------------------------------
            // Deserialize
            //-------------------------------------------------
            var serializer = Serialization.GetSerializer(References);
            Dictionary<string, object> deserialized = new Dictionary<string, object>();
            {
                fsData data;
                fsResult result;

                if (Asset.Data != null)
                {
                    data = Asset.Data;
                }
                else
                {
                    result = fsJsonParser.Parse(Asset.Json, out data);
                    if (result.Failed)
                    {
                        Debug.LogError(result, this);
                        return;
                    }
                    Asset.Data = data;
                }

                result = serializer.TryDeserialize(data, ref deserialized);
                if (result.Failed)
                {
                    Debug.LogError(result, this);
                    return;
                }
            }

            //-------------------------------------------------
            // Variables
            //-------------------------------------------------
            if (deserialized.ContainsKey("Variables"))
            {
                Variables.NextFrameValues = deserialized["Variables"] as Dictionary<string, object>;
                Variables.Values = new Dictionary<string, object>(Variables.NextFrameValues);
            }

            //-------------------------------------------------
            // Aspects
            //-------------------------------------------------
            if (deserialized.ContainsKey("Aspects"))
            {
                var thisType = GetType();
                var aspects = deserialized["Aspects"] as Dictionary<string, Aspect>;
                foreach (var kvp in aspects)
                {
                    var aspect = kvp.Value;
                    aspect.Golem = this;
                    aspect.Variables = Variables;
                    var field = thisType.GetField(kvp.Key);
                    if (field == null)
                    {
                        Debug.LogError("No field available for aspect type " + kvp.Key, this);
                        continue;
                    }
                    field.SetValue(this, aspect);
                }
            }

            //-------------------------------------------------
            // Circuit
            //-------------------------------------------------
            if (deserialized.ContainsKey("Registers"))
            {
                _registers = deserialized["Registers"] as object[];
            }
            else
            {
                _registers = new object[0];
            }

            if (deserialized.ContainsKey("VariablesThatWriteRegister"))
            {
                _variablesThatWriteRegister = deserialized["VariablesThatWriteRegister"] as Dictionary<string, int[]>;
            }
            else
            {
                _variablesThatWriteRegister = new Dictionary<string,int[]>();
            }

            if (deserialized.ContainsKey("CellsThatReadRegister"))
            {
                _cellsThatReadRegister = deserialized["CellsThatReadRegister"] as int[][];
            }
            else
            {
                _cellsThatReadRegister = new int[0][];
            }

            if (deserialized.ContainsKey("Cells"))
            {
                Cells = deserialized["Cells"] as Cell[];
            }
            else
            {
                Cells = new Cell[0];
            }

            foreach (var kvp in _variablesThatWriteRegister)
            {
                object value = Variables.Get(kvp.Key);
                int[] registersToWrite = kvp.Value;
                for (int i = 0; i < registersToWrite.Length; ++i)
                {
                    _registers[i] = value;
                }
            }

            _dirty = new bool[Cells.Length];
            _running = new bool[Cells.Length];
            for (int i = 0; i < _dirty.Length; ++i)
            {
                Cells[i].Acquire(this, ref _running[i]);
            }

            //-------------------------------------------------
            // Program
            //-------------------------------------------------
            if (deserialized.ContainsKey("Scripts"))
            {
                Layers = deserialized["Layers"] as Layer[];
                States = deserialized["States"] as State[];
                Scripts = deserialized["Scripts"] as Script[];
            }
            else
            {
                Layers = new Layer[0];
                States = new State[0];
                Scripts = new Script[0];
            }
            Triggers = new bool[(int)Trigger.__COUNT__];
            LayerState = new StateIndex[Layers.Length];
            for (int i = 0; i < LayerState.Length; ++i)
            {
                LayerState[i] = StateIndex.Idle;
            }
            for (int i = 0; i < Scripts.Length; ++i)
            {
                Scripts[i].Acquire(this);
            }
        }

        //---------------------------------------------------------------------
        // Circuit
        //---------------------------------------------------------------------
        private object[] _registers;
        private Dictionary<string, int[]> _variablesThatWriteRegister;
        private int[][] _cellsThatReadRegister;
        private bool[] _dirty;
        private bool[] _running;
        public Cell[] Cells;

        //-----------------------------------------------------
        // Variable read/writes used by Cells
        //-----------------------------------------------------
        public object Read(VariableRef variable)
        {
            Variables variables = getVariables(variable.Relationship);
            return (variables == null) ? null : variables.Get(variable.Name);
        }

        public bool Read(VariableRef variable, ref float value)
        {
            Variables variables = getVariables(variable.Relationship);
            return (variables == null) ? false : variables.Get(variable.Name, ref value);
        }

        public void Write(VariableRef variable, object value)
        {
            Variables variables = getVariables(variable.Relationship);
            if (variables != null)
            {
                variables.Set(variable.Name, value);
            }
        }

        private Variables getVariables(EntityRelationship relationship)
        {
            switch (relationship)
            {
                case EntityRelationship.Self: return Variables;
                case EntityRelationship.Owner: return Owner != null ? Owner.Variables : null;
                case EntityRelationship.Subject: return Subject != null ? Subject.Variables : null;
                case EntityRelationship.Target: return (Targets.Length > 0) ? Targets[0].Variables : null;
                default: throw new InvalidProgramException("EntityRelationship " + relationship + " not handled");
            }
        }

        //-----------------------------------------------------
        // Register getters and setters for use by Cells
        //-----------------------------------------------------

        public T Get<T>(RegisterPtr pointer) { return pointer == RegisterPtr.Invalid ? default(T) : (T)_registers[(int)pointer]; }
        public bool Get<T>(RegisterPtr pointer, out T v) { bool b = pointer != RegisterPtr.Invalid; v = b ? (T)_registers[(int)pointer] : default(T); return b; }
        public bool TryGet<T>(RegisterPtr pointer, ref T v) { bool b = pointer != RegisterPtr.Invalid; if(b) v = (T)_registers[(int)pointer]; return b; }
        public void Set<T>(RegisterPtr pointer, T value) { setRegister((int)pointer, (object)value); }

        //-----------------------------------------------------
        // Sets the register to the new value and updates
        // the dirty mask for cells.
        //-----------------------------------------------------
        private void setRegister(int pointer, object value)
        {
            // Skip unused pointers
            if (pointer == int.MaxValue)
            {
                return;
            }

#if UNITY_EDITOR

            // Make sure the pointer is valid
            if (pointer < 0 || pointer >= _registers.Length)
            {
                throw new ArgumentOutOfRangeException("pointer");
            }

            // Typecheck the register vs. the incoming type
            // TODO: It would be better to do this by looking into the editor
            // data associated with this golem and checking vs. the original
            // type because type mutations can occur this way.
            object currentValue = _registers[pointer];
            if (value != null && currentValue == null)
            {
                Debug.Assert(!value.GetType().IsValueType);
            }
            else if (value == null && currentValue != null)
            {
                Debug.Assert(!currentValue.GetType().IsValueType);
            }
            else if (value != null && currentValue != null)
            {
                Debug.Assert(currentValue.GetType().IsAssignableFrom(value.GetType()));
            }

#endif

            // Don't dirty a register when it gets set to its current value
            if (object.Equals(_registers[pointer], value))
            {
                return;
            }

            // Save the new value
            _registers[pointer] = value;

            // Dirty all the cells that read this register
            int[] outputs = _cellsThatReadRegister[pointer];
            for (int i = 0; i < outputs.Length; ++i)
            {
                _dirty[outputs[i]] = true;
            }
        }

        //---------------------------------------------------------------------
        // Program
        //---------------------------------------------------------------------
        public Layer[] Layers;
        public StateIndex[] LayerState;
        public bool[] Triggers;
        public State[] States;
        public Script[] Scripts;

        public void SetTrigger(Trigger trigger)
        {
            Triggers[(int)trigger] = true;
        }
    }
}
