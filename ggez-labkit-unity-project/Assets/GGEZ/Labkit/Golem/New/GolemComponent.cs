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

namespace GGEZ.Labkit
{
    public class GolemComponent : ScriptableObject, ISerializationCallbackReceiver
    {

        [NonSerialized]
        public Cell[] Cells;
        
        [NonSerialized]
        public IRegister[] Registers;

        [NonSerialized]
        public Script[] Scripts;

        [NonSerialized]
        public Layer[] Layers;

        [NonSerialized]
        public State[] States;

        [NonSerialized]
        public Assignment[] Assignments;

        [NonSerialized]
        public Dictionary<string, Assignment[]> ExternalAssignments;

        /// <summary>Source of data for all the NonSerialized properties</summary>
        public string Json;

        //---------------------------------------------------------------------
        // Editor Data
        //---------------------------------------------------------------------
    #if UNITY_EDITOR

        // Circuit
        //------------------
        [System.NonSerialized]
        public List<EditorCell> EditorCells = new List<EditorCell>();
        [System.NonSerialized]
        public List<EditorWire> EditorWires = new List<EditorWire>();

        // Program
        //------------------
        [System.NonSerialized]
        public List<EditorState> EditorStates = new List<EditorState>();
        [System.NonSerialized]
        public List<EditorTransition> EditorTransitions = new List<EditorTransition>();


        /// <summary>Source of data for all the NonSerialized editor-only fields</summary>
        public string EditorJson;

    #endif

        public void OnBeforeSerialize()
        {

        #if UNITY_EDITOR

            // Save editor-only data
            //-------------------------
            {
                Dictionary<string, object> serialized = new Dictionary<string, object>();

                serialized["EditorCells"] = EditorCells;
                serialized["EditorWires"] = EditorWires;
                serialized["EditorStates"] = EditorStates;
                serialized["EditorTransitions"] = EditorTransitions;

                EditorJson = Serialization.SerializeDictionary(serialized);
            }

            // Compile runtime data
            //-------------------------
            List<Assignment> assignments = new List<Assignment>();
            Dictionary<string, List<Assignment>> externalAssignments = new Dictionary<string, List<Assignment>>();

            //-------------------------------------------------------------
            // Compile the circuit
            //-------------------------------------------------------------
            {
                // Do some housekeeping
                for (int i = 0; i < EditorCells.Count; ++i)
                {
                    EditorCells[i].Index = (EditorCellIndex)i;
                }

                //-----------------------------------------------------
                // Compute the correct topographically-sorted traversal
                // order from the wire connectivity graph.
                //-----------------------------------------------------
                int[] cellRemap;
                {
                    int[] longestPath = new int[EditorCells.Count];
                    for (int i = 0; i < longestPath.Length; ++i)
                    {
                        longestPath[i] = -1;
                    }

                    //-------------------------------------------------
                    // Prepare a dependency map so we can traverse this
                    // graph. Find all cells without inputs and mark them
                    // as sources.
                    //-------------------------------------------------
                    var hasInput = new bool[EditorCells.Count];
                    var dependencies = new int[EditorCells.Count][];
                    {
                        List<int> cellDependencies = new List<int>();
                        for (int i = 0; i < EditorCells.Count; ++i)
                        {
                            for (int j = 0; j < EditorWires.Count; ++j)
                            {
                                EditorCell readCell = EditorWires[j].ReadCell;
                                if (readCell == null || (int)readCell.Index != i)
                                {
                                    continue;
                                }
                                int writeCell = (int)EditorWires[j].WriteCell.Index;
                                cellDependencies.Add(writeCell);
                                hasInput[writeCell] = true;
                            }
                            dependencies[i] = cellDependencies.ToArray();
                            cellDependencies.Clear();
                        }
                    }

                    //-------------------------------------------------
                    // Iterate through all of the cells and trace distance
                    // to their dependencies. Set the longest path index at
                    // each location to the max of its current value and
                    // the new value. In worst case, we traverse the tree
                    // O(n^2) times. However, in practice, this step should
                    // be run rarely, and on graphs that are flat and small.
                    //-------------------------------------------------
                    var worklist = new int[EditorCells.Count];
                    var depths = new int[EditorCells.Count];
                    int worklistPointer = 0;
                    for (int i = 0; i < hasInput.Length; ++i)
                    {
                        if (!hasInput[i])
                        {
                            worklist[worklistPointer] = i;
                            depths[worklistPointer] = 0;
                            ++worklistPointer;
                        }
                    }
                    if (worklistPointer == 0 && EditorCells.Count > 0)
                    {
                        // We never incremented the worklist pointer so there are no input cells
                        goto CycleExit;
                    }
                    --worklistPointer;
                    while (worklistPointer >= 0)
                    {
                        int cell = worklist[worklistPointer];
                        int depth = depths[worklistPointer];
                        --worklistPointer;

                        // Determine if the path to this cell is longer than the current one
                        if (longestPath[cell] >= depth)
                        {
                            continue;
                        }

                        // Update the longest path to this cell
                        longestPath[cell] = depth;

                        // Put all children on the worklist at +1 depth
                        var children = dependencies[cell];
                        ++depth;
                        for (int j = children.Length - 1; j >= 0; --j)
                        {
                            ++worklistPointer;
                            if (worklistPointer >= worklist.Length)
                            {
                                // This can only happen if there is a cycle, since the longest possible
                                // path to a node must be shorter than the path through every other node
                                goto CycleExit;
                            }
                            if (depth >= worklist.Length)
                            {
                                throw new InvalidProgramException("circuit graph isn't built correctly");
                            }
                            worklist[worklistPointer] = children[j];
                            depths[worklistPointer] = depth;
                        }
                    }
                CycleExit:
                    if (worklistPointer == -1)
                    {
                        // reuse arrays that are already the right size :)
                        cellRemap = worklist;
                        for (int i = 0; i < EditorCells.Count; ++i)
                        {
                            cellRemap[i] = i;
                        }
                        System.Array.Sort(longestPath, cellRemap);
                    }
                    else
                    {
                        Debug.LogError("Cycle in circuit graph detected; circuit will not function", this);
                        cellRemap = new int[0];
                    }
                }

                //---------------------------------------------------------
                // Build mappings required by the wires. Registers store values
                // on cell outputs. These registers then dirty other cells.
                //---------------------------------------------------------
                List<IRegister> registers = new List<IRegister>();
                Dictionary<string, RegisterPtr> cellOutputToRegister = new Dictionary<string, RegisterPtr>();
                Dictionary<VariableRef, RegisterPtr> variablesThatWriteRegister = VariableRef.NewDictionary<RegisterPtr>();


                //---------------------------------------------------------
                // Write out all of the cells in the traversal order
                //---------------------------------------------------------
                Cell[] cells = new Cell[EditorCells.Count];
                for (int cellIndex = 0; cellIndex < cells.Length; ++cellIndex)
                {
                    int editorCellIndex = cellRemap[cellIndex];
                    var editorCell = EditorCells[editorCellIndex];

                    var cell = editorCell.Cell.Clone();
                    cells[cellIndex] = cell;

                    var cellType = cell.GetType();
                    InspectableCellType inspectableCellType = InspectableCellType.GetInspectableCellType(cellType);

                    //-------------------------------------------------
                    // Write assignments for fields referencing:
                    //  * Aspects
                    //  * Settings
                    //  * Unity Objects
                    //  * Variables
                    //-------------------------------------------------
                    {
                        InspectableCellType.Field[] fields = inspectableCellType.Fields;
                        for (int i = 0; i < fields.Length; ++i)
                        {
                            InspectableCellType.Field field = fields[i];
                            string fieldName = field.FieldInfo.Name;

                            switch (field.Type)
                            {
                                
                            //-------------------------------------------------
                            case InspectableType.Golem:
                            //-------------------------------------------------
                                assignments.Add(new Assignment()
                                {
                                    Type = AssignmentType.CellGolem,
                                    TargetIndex = cellIndex,
                                    TargetFieldName = fieldName,
                                });
                                break;

                            //-------------------------------------------------
                            case InspectableType.UnityObject:
                            //-------------------------------------------------
                                string unityObjectName;
                                if (editorCell.UnityObjectFields.TryGetValue(fieldName, out unityObjectName))
                                {
                                    assignments.Add(new Assignment()
                                    {
                                        Type = AssignmentType.CellAspect,
                                        Name = unityObjectName,
                                        TargetIndex = cellIndex,
                                        TargetFieldName = fieldName,
                                    });
                                }
                                else
                                {
                                    Debug.LogWarning("Field " + fieldName + " is not mapped to a UnityObject and will always be null");
                                }
                                break;

                            //-------------------------------------------------
                            case InspectableType.Aspect:
                            //-------------------------------------------------
                                assignments.Add(new Assignment()
                                {
                                    Type = AssignmentType.CellAspect,
                                    TargetIndex = cellIndex,
                                    TargetFieldName = fieldName,
                                });
                                break;

                            //-------------------------------------------------
                            case InspectableType.Variable:
                            //-------------------------------------------------
                                VariableRef variableRef;
                                if (editorCell.FieldsUsingVariables.TryGetValue(fieldName, out variableRef))
                                {
                                    var assignment = new Assignment()
                                    {
                                        Name = variableRef.Name,
                                        TargetIndex = cellIndex,
                                        TargetFieldName = fieldName,
                                    };

                                    if (variableRef.IsExternal)
                                    {
                                        assignment.Type = field.CanBeNull
                                                ? AssignmentType.CellVariableOrNull
                                                : AssignmentType.CellVariableOrDummy;
                                        externalAssignments.MultiAdd(variableRef.Relationship, assignment);
                                    }
                                    else if (variableRef.IsInternalRegister)
                                    {
                                        assignment.Type = AssignmentType.CellRegisterVariable;
                                        assignment.RegisterIndex = variableRef.RegisterIndex;
                                        assignments.Add(assignment);
                                    }
                                    else
                                    {
                                        assignment.Type = AssignmentType.CellVariable;
                                        assignments.Add(assignment);
                                    }
                                }
                                else
                                {
                                    if (!field.CanBeNull)
                                    {
                                        assignments.Add(new Assignment()
                                        {
                                            Type = AssignmentType.CellDummyVariable,
                                            TargetIndex = cellIndex,
                                            TargetFieldName = fieldName,
                                        });
                                    }
                                }
                                break;

                            //-------------------------------------------------
                            default:
                            //-------------------------------------------------
                                string setting;
                                if (editorCell.FieldsUsingSettings.TryGetValue(fieldName, out setting))
                                {
                                    Debug.Assert(InspectableTypeExt.CanUseSetting(field.Type));

                                    assignments.Add(new Assignment()
                                    {
                                        Type = AssignmentType.CellSetting,
                                        Name = setting,
                                        TargetIndex = cellIndex,
                                        TargetFieldName = fieldName,
                                    });
                                }
                                break;
                            }
                        }
                    }

                    //-------------------------------------------------
                    // Assign input registers. Because we traverse in
                    // dependency order, this should never use a
                    // register that hasn't been mapped
                    //-------------------------------------------------
                    {
                        HashSet<string> inputsThatCantBeNull = inspectableCellType.GetUnnullableInputs();
                        List<EditorWire> inputWires = editorCell.Inputs;
                        for (int i = 0; i < inputWires.Count; ++i)
                        {
                            EditorWire wire = inputWires[i];
                            if (wire.ReadCell != null)
                            {
                                Debug.Assert(wire.ReadVariableInputRegister == null);
                                string registerKey = wire.ReadCell.Index + ".{" + wire.ReadField + "}";
                                Debug.Assert(cellOutputToRegister.ContainsKey(registerKey), "A register input was not mapped before it was seen when traversing cells of this circuit. Something is wrong with the dependency sort.", this);
                                RegisterPtr registerPtr = cellOutputToRegister[registerKey];

                                assignments.Add(new Assignment()
                                {
                                    Type = AssignmentType.CellInputRegister,
                                    RegisterIndex = (int)registerPtr,
                                    TargetIndex = cellIndex,
                                    TargetFieldName = wire.WriteField,
                                });
                            }
                            else if (wire.ReadVariableInputRegister != null)
                            {
                                bool fieldCanBeNull = !inputsThatCantBeNull.Contains(wire.WriteField);
                                Assignment assignment = new Assignment()
                                {
                                    Type = fieldCanBeNull
                                            ? AssignmentType.CellInputVariableRegisterOrNull
                                            : AssignmentType.CellInputVariableRegisterOrDummy,
                                    Name = wire.ReadVariableInputRegister.Variable.Name,
                                    TargetIndex = cellIndex,
                                    TargetFieldName = wire.WriteField,
                                };
                                string relationship = wire.ReadVariableInputRegister.Variable.Relationship;
                                if (string.IsNullOrEmpty(relationship))
                                {
                                    assignments.Add(assignment);
                                }
                                else
                                {
                                    externalAssignments.MultiAdd(relationship, assignment);
                                }
                            }
                            else
                            {
                                throw new InvalidProgramException("Wire doesn't read from anything");
                            }
                            inputsThatCantBeNull.Remove(wire.WriteField);
                        }

                        foreach (string input in inputsThatCantBeNull)
                        {
                            assignments.Add(new Assignment()
                            {
                                Type = AssignmentType.CellDummyInputRegister,
                                TargetIndex = cellIndex,
                                TargetFieldName = input,
                            });
                        }
                    }

                    //-------------------------------------------------
                    // Assign output registers. Again, because we do
                    // this in dependency order, we should always be
                    // writing things that don't exist yet.
                    //-------------------------------------------------
                    {
                        HashSet<string> outputsThatCantBeNull = inspectableCellType.GetUnnullableOutputs();
                        List<EditorWire> outputWires = editorCell.Outputs;
                        for (int i = 0; i < outputWires.Count; ++i)
                        {
                            EditorWire wire = outputWires[i];
                            Debug.Assert(wire.ReadVariableInputRegister == null, this);

                            bool fieldCanBeNull = !outputsThatCantBeNull.Contains(wire.ReadField);

                            string registerKey = wire.ReadCell.Index + ".{" + wire.ReadField + "}";

                            // TODO: this assert breaks if you have fanout > 1
                            //       write it better when wires are in dictionaries
                            // Debug.Assert(!cellOutputToRegister.ContainsKey(registerKey), "A register output was already mapped. Are multiple wires assigned to a single input?", this);

                            var readField = cellType.GetField(wire.ReadField);
                            var fieldType = readField.FieldType;
                            Debug.Assert(typeof(IRegister).IsAssignableFrom(fieldType));

                            RegisterPtr registerIndex;
                            if (!cellOutputToRegister.TryGetValue(registerKey, out registerIndex))
                            {
                                registerIndex = (RegisterPtr)registers.Count;
                                cellOutputToRegister.Add(registerKey, registerIndex);
                                IRegister register = Activator.CreateInstance(fieldType) as IRegister;
                                registers.Add(register);
                                wire.Register = registerIndex;
                            }

                            Debug.Assert(registers[(int)registerIndex].GetType().Equals(fieldType));

                            assignments.Add(new Assignment()
                            {
                                Type = AssignmentType.CellOutputRegister,
                                RegisterIndex = (int)registerIndex,
                                TargetIndex = cellIndex,
                                TargetFieldName = wire.ReadField,
                            });

                            outputsThatCantBeNull.Remove(wire.WriteField);
                        }

                        foreach (string input in outputsThatCantBeNull)
                        {
                            assignments.Add(new Assignment()
                            {
                                Type = AssignmentType.CellDummyOutputRegister,
                                TargetIndex = cellIndex,
                                TargetFieldName = input,
                            });
                        }
                    }

                }

                Registers = registers.ToArray();
                Cells = cells;
            }

            //-------------------------------------------------------------
            // Compile the program
            //-------------------------------------------------------------
            {
                // Identify entrypoints and assign them to layers
                List<HashSet<EditorStateIndex>> layersBuilder = new List<HashSet<EditorStateIndex>>();
                for (int i = 0; i < EditorStates.Count; ++i)
                {
                    var editorState = EditorStates[i];
                    Debug.Assert(editorState.Index == (EditorStateIndex)i, "Wrong editor state index; set to " + editorState.Index + " expecting " + i);
                    editorState.Layer = EditorLayerIndex.Invalid;
                    if (editorState.SpecialState == EditorSpecialStateType.LayerEnter)
                    {
                        editorState.Layer = (EditorLayerIndex)layersBuilder.Count;
                        layersBuilder.Add(new HashSet<EditorStateIndex>() { editorState.Index });
                    }
                }

                // Propagate the layer index from entrypoint to all connected states
                int removedLayersAboveIndex = layersBuilder.Count;
                var worklist = new List<EditorStateIndex>();
                for (int layerIndex = layersBuilder.Count - 1; layerIndex >= 0; --layerIndex)
                {
                    var layerStates = layersBuilder[layerIndex];
                    worklist.Add(layerStates.First());
                    while (worklist.Count > 0)
                    {
                        var lastIndex = worklist.Count - 1;
                        var editorState = EditorStates[(int)worklist[lastIndex]];
                        worklist.RemoveAt(lastIndex);
                        var transitions = editorState.TransitionsOut;
                        for (int j = 0; j < transitions.Count; ++j)
                        {
                            var toIndex = EditorTransitions[(int)transitions[j]].To;
                            if (layerStates.Contains(toIndex))
                            {
                                continue;
                            }
                            var toState = EditorStates[(int)toIndex];
                            if (toState.Layer != EditorLayerIndex.Invalid)
                            {
                                Debug.LogError("State " + toState.Name + " is accessible from more than one entrypoint so one of the entries is being disabled");
                                layersBuilder.RemoveAt(layerIndex);
                                removedLayersAboveIndex = layerIndex;
                                goto NextLayer;
                            }
                            toState.Layer = (EditorLayerIndex)layerIndex;
                            layerStates.Add(toIndex);
                            worklist.Add(toIndex);
                        }
                    }
                NextLayer:
                    worklist.Clear();
                }

                // Reassign layer indices to cells if any layers were removed
                for (int layerIndex = removedLayersAboveIndex; layerIndex < layersBuilder.Count; ++layerIndex)
                {
                    foreach (var editorState in layersBuilder[layerIndex])
                    {
                        EditorStates[(int)editorState].Layer = (EditorLayerIndex)layerIndex;
                    }
                }

                // Gather states and scripts into layers
                Layer[] layers = new Layer[layersBuilder.Count];
                List<Script> scripts = new List<Script>();
                List<State> states = new List<State>();
                var exprWorklist = new List<EditorTransitionExpression>();
                for (int layerIndex = 0; layerIndex < layersBuilder.Count; ++layerIndex)
                {
                    var editorStates = layersBuilder[layerIndex];
                    var layerStates = new List<StateIndex>();
                    foreach (var editorStateIndex in editorStates)
                    {
                        var editorState = EditorStates[(int)editorStateIndex];

                        if (editorState.SpecialState != EditorSpecialStateType.Normal)
                        {
                            // if (editorState.SpecialState == EditorSpecialStateType.LayerEnter)
                            // {
                            //     editorState.CompiledIndex = (StateIndex)states.Count;
                            //     states.Add(new State { Scripts = new int[0] });
                            //     layerStates.Add(editorState.CompiledIndex);
                            // }
                            continue;
                        }

                        var editorScripts = editorState.Scripts;
                        int[] stateScripts = new int[editorScripts.Count];
                        for (int j = 0; j < editorScripts.Count; ++j)
                        {
                            int scriptIndex = scripts.Count;
                            stateScripts[j] = scriptIndex;
                            var editorScript = editorScripts[j];
                            var script = editorScript.Script.Clone();
                            ApplyFieldsUsingSettings(script, editorScript.FieldsUsingSettings, Settings);
                            scripts.Add(script);
                        }
                        int stateIndex = states.Count;
                        editorState.CompiledIndex = (StateIndex)states.Count;
                        states.Add(new State { Scripts = stateScripts });
                        layerStates.Add((StateIndex)(int)stateIndex);
                    }
                    layers[layerIndex] = new Layer
                    {
                        States = layerStates.ToArray(),
                    };
                }

                // Compile all the transitions now that we have state indices in place
                for (int layerIndex = 0; layerIndex < layersBuilder.Count; ++layerIndex)
                {
                    var editorStates = layersBuilder[layerIndex];
                    var layerTransitions = new List<Transition>();
                    foreach (var editorStateIndex in editorStates)
                    {
                        var editorState = EditorStates[(int)editorStateIndex];
                        foreach (var editorTransitionIndex in editorState.TransitionsOut)
                        {
                            var editorTransition = EditorTransitions[(int)editorTransitionIndex];
                            var transitionTriggers = new List<Trigger>();
                            var transitionExpression = new List<Transition.Operator>();
                            exprWorklist.Clear();
                            exprWorklist.Add(editorTransition.Expression);
                            while (exprWorklist.Count > 0)
                            {
                                int exprIndex = exprWorklist.Count - 1;
                                var expr = exprWorklist[exprIndex];
                                exprWorklist.RemoveAt(exprIndex);
                                exprWorklist.AddRange(expr.Subexpressions);
                                switch (expr.Type)
                                {
                                    case EditorTransitionExpressionType.False:
                                        transitionExpression.Insert(0, Transition.Operator.False);
                                        break;

                                    case EditorTransitionExpressionType.True:
                                        transitionExpression.Insert(0, Transition.Operator.True);
                                        break;

                                    case EditorTransitionExpressionType.Trigger:
                                        {
                                            transitionTriggers.Insert(0, expr.Trigger);
                                            transitionExpression.Insert(0, Transition.Operator.Push);
                                        }
                                        break;

                                    case EditorTransitionExpressionType.And:
                                        for (int i = 1; i < expr.Subexpressions.Count; ++i)
                                        {
                                            transitionExpression.Insert(0, Transition.Operator.And);
                                        }
                                        break;

                                    case EditorTransitionExpressionType.Or:
                                        for (int i = 1; i < expr.Subexpressions.Count; ++i)
                                        {
                                            transitionExpression.Insert(0, Transition.Operator.Or);
                                        }
                                        break;
                                }
                            }

                            var transition = new Transition
                            {
                                Expression = transitionExpression.ToArray(),
                                Triggers = transitionTriggers.ToArray(),
                            };
                            var fromState = EditorStates[(int)editorTransition.From];
                            switch (fromState.SpecialState)
                            {
                                case EditorSpecialStateType.Normal:
                                    transition.FromState = fromState.CompiledIndex;
                                    break;
                                case EditorSpecialStateType.LayerAny:
                                    transition.FromState = StateIndex.Any;
                                    break;
                                case EditorSpecialStateType.LayerEnter:
                                    transition.FromState = StateIndex.Idle;
                                    break;
                                case EditorSpecialStateType.LayerExit:
                                    Debug.LogError("Can't transition from a state with the Exit type");
                                    transition.FromState = StateIndex.Invalid;
                                    break;
                            }
                            var toState = EditorStates[(int)editorTransition.To];
                            switch (toState.SpecialState)
                            {
                                case EditorSpecialStateType.Normal:
                                    transition.ToState = toState.CompiledIndex;
                                    break;
                                case EditorSpecialStateType.LayerAny:
                                    Debug.LogError("Can't transition to a state with the Any type");
                                    transition.ToState = StateIndex.Invalid;
                                    break;
                                case EditorSpecialStateType.LayerEnter:
                                    Debug.LogError("Can't transition to a state with the Enter type (should be Exit)");
                                    transition.ToState = StateIndex.Invalid;
                                    break;
                                case EditorSpecialStateType.LayerExit:
                                    transition.ToState = StateIndex.Idle;
                                    break;
                            }
                            layerTransitions.Add(transition);
                        }
                    }
                    layers[layerIndex].Transitions = layerTransitions.ToArray();
                }

                serialized["Layers"] = layers;
                serialized["Scripts"] = scripts.ToArray();
                serialized["States"] = states.ToArray();
            }

        #else
            Debug.LogError("GolemArchetype.OnBeforeSerialize should never be called at runtime!", this);
        #endif
        
            // Save runtime data
            //-------------------------
            {
                Dictionary<string, object> serialized = new Dictionary<string, object>();


                Json = Serialization.SerializeDictionary(serialized);
            }
        }

        public void OnAfterDeserialize()
        {

            {
                var deserialized = Serialization.DeserializeDictionary(Json, null, this);

                Serialization.ReadOrCreate(this, "Cells", deserialized);
                Serialization.ReadOrCreate(this, "Scripts", deserialized);
                Serialization.ReadOrCreate(this, "Registers", deserialized);
                Serialization.ReadOrCreate(this, "Assignments", deserialized);
                Serialization.ReadOrCreate(this, "ExternalAssignments", deserialized);
                Serialization.ReadOrCreate(this, "Layers", deserialized);
                Serialization.ReadOrCreate(this, "States", deserialized);
            }

        #if UNITY_EDITOR
            {
                var deserialized = Serialization.DeserializeDictionary(EditorJson, null, this);

                Serialization.ReadOrCreate(this, "EditorCells", deserialized);
                Serialization.ReadOrCreate(this, "EditorWires", deserialized);
                Serialization.ReadOrCreate(this, "EditorStates", deserialized);
                Serialization.ReadOrCreate(this, "EditorTransitions", deserialized);

                for (int i = 0; i < EditorStates.Count; ++i)
                {
                    EditorStates[i].Index = (EditorStateIndex)i;
                }

                for (int i = 0; i < EditorCells.Count; ++i)
                {
                    EditorCells[i].Index = (EditorCellIndex)i;
                }
            }
        #endif
        }
    }
}