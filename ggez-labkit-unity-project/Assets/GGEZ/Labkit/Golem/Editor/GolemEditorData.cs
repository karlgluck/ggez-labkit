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
using UnityEditor;
using UnityObject = UnityEngine.Object;
using UnityObjectList = System.Collections.Generic.List<UnityEngine.Object>;
using System.Collections.Generic;
using GGEZ.FullSerializer;
using System.Linq;
using System.Reflection;

namespace GGEZ.Labkit
{
    //-------------------------------------------------------------------------
    // GolemAspectEditorData
    //-------------------------------------------------------------------------
    public class GolemAspectEditorData
    {
        public FieldInfo Field;
        public Aspect Aspect;
        public InspectableFieldInfo[] AspectFields;
        public InspectableVariablePropertyInfo[] AspectVariables;

        public Dictionary<string,string> FieldsUsingSettings = new Dictionary<string,string>();

        /// State of the foldout for this aspect's properties in the inspector
        public bool Foldout;
    }

    public class GolemVariableEditorData
    {
        public string Name;
        public string Tooltip;
        public InspectableType InspectableType;
        public Type Type;
        public object InitialValue;

        /// Aspect(s) that declare this variable. Recomputed each load so that
        /// variables removed from aspects (or variables from aspects removed
        /// from golems) will be erased.
        [System.NonSerialized]
        public HashSet<Type> SourceAspects = new HashSet<Type>();
    }

    //-------------------------------------------------------------------------
    // GolemEditorData
    //-------------------------------------------------------------------------
    public class GolemEditorData
    {
        //---------------------------------------------------------------------
        // Load (static)
        //---------------------------------------------------------------------
        public static GolemEditorData Load(Golem golem)
        {
            // // Only load prefab instances
            // bool isPrefab = PrefabUtility.GetPrefabType(entity) == PrefabType.PrefabInstance;
            // if (!isPrefab)
            // {
            //     return null;
            // }

            GolemEditorAsset editorAsset = golem.EditorAsset as GolemEditorAsset;
            if (editorAsset == null)
            {
                // editorAsset = Helper.FindAssetInPrefab<EntityEditorAsset>(entity);
                editorAsset = ScriptableObject.CreateInstance<GolemEditorAsset>();
                Debug.Assert(editorAsset != null);
            }
            editorAsset.hideFlags = HideFlags.DontSaveInBuild;

            var editorData = editorAsset.EditorData;
            if (editorData == null)
            {
                editorData = new GolemEditorData();
            }
            editorData.Golem = golem;
            editorData.EditorAsset = editorAsset;
            golem.EditorAsset = editorAsset;

            golem.Load();
            editorData.Load();

            return editorData;
        }

        public Golem Golem;
        public GolemEditorAsset EditorAsset;

        [System.NonSerialized]
        public Settings Settings;

        // Aspects
        //------------------
        [System.NonSerialized]
        public List<GolemAspectEditorData> EditorAspects = new List<GolemAspectEditorData>();

        // Variables
        //------------------
        [System.NonSerialized]
        public InspectableDictionaryKeyValuePair[] _variables = new InspectableDictionaryKeyValuePair[0];
        [System.NonSerialized]
        public List<GolemVariableEditorData> EditorVariables = new List<GolemVariableEditorData>();

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

        public void AddAspect(Aspect aspect)
        {
            if (aspect == null)
            {
                throw new ArgumentNullException("aspect");
            }
            var aspectType = aspect.GetType();
            var aspectFieldInGolem = typeof(Golem).GetField(aspectType.Name);
            if (aspectFieldInGolem == null || !aspectFieldInGolem.FieldType.IsAssignableFrom(aspectType))
            {
                throw new InvalidProgramException(typeof(Golem).Name + " must contain a field for " + aspectType.Name);
            }
            bool isNew = false;
            var currentFieldValue = aspectFieldInGolem.GetValue(Golem);
            isNew = currentFieldValue == null;
            if (isNew)
            {
                aspectFieldInGolem.SetValue(Golem, aspect);
            }
            else if (!object.ReferenceEquals(currentFieldValue, aspect))
            {
                throw new InvalidOperationException(typeof(Golem).Name + " field " + aspectFieldInGolem.Name + " already contains a value for " + aspectType.Name);
            }
            var editorAspect = new GolemAspectEditorData();
            editorAspect.Field = aspectFieldInGolem;
            editorAspect.Aspect = aspect;
            editorAspect.AspectFields = InspectableFieldInfo.GetFields(aspect);
            editorAspect.AspectVariables = InspectableVariablePropertyInfo.GetVariableProperties(aspect);
            EditorAspects.Add(editorAspect);

            // Mark fields for settings that want them
            if (isNew)
            {
                for (int i = 0; i < editorAspect.AspectFields.Length; ++i)
                {
                    var fieldOfAspect = editorAspect.AspectFields[i];
                    if (fieldOfAspect.WantsSetting)
                    {
                        editorAspect.FieldsUsingSettings.Add(fieldOfAspect.FieldInfo.Name, null);
                    }
                }
            }

            // Clean up FieldsUsingSettings to only include existing fields
            {
                foreach (var key in editorAspect.FieldsUsingSettings.Keys.Where(
                    (fieldName) => !editorAspect.AspectFields.Any((e) => e.FieldInfo.Name == fieldName)
                    ).ToArray())
                {
                    editorAspect.FieldsUsingSettings.Remove(key);
                }
            }

            ApplyFieldsUsingSettings(aspect, editorAspect.FieldsUsingSettings, Settings);

            // Add all of the aspect's declared variables
            for (int i = 0; i < editorAspect.AspectVariables.Length; ++i)
            {
                InspectableVariablePropertyInfo aspectVariable = editorAspect.AspectVariables[i];
                string variableName = aspectVariable.VariableAttribute.Name;
                string variableTooltip = aspectVariable.VariableAttribute.Tooltip;
                bool notFound = true;
                for (int j = 0; notFound && j < EditorVariables.Count; ++j)
                {
                    if (EditorVariables[j].Name == variableName)
                    {
                        EditorVariables[j].SourceAspects.Add(aspectType);
                        if (!string.IsNullOrEmpty(variableTooltip))
                        {
                            EditorVariables[j].Tooltip = variableTooltip;
                        }
                        notFound = false;
                    }
                }
                if (notFound)
                {
                    Type type = aspectVariable.PropertyInfo.PropertyType;
                    var variableEditorData = new GolemVariableEditorData()
                    {
                        Name = variableName,
                        Tooltip = variableTooltip,
                        SourceAspects = new HashSet<Type>(){ aspectType },
                        InspectableType = aspectVariable.Type,
                        Type = aspectVariable.PropertyInfo.PropertyType,
                        InitialValue = type.IsValueType ? Activator.CreateInstance(type) : null,
                    };
                    EditorVariables.Add(variableEditorData);
                }
            }
        }

        public void RemoveAspect(GolemAspectEditorData editableAspect)
        {
            if (editableAspect == null)
            {
                throw new ArgumentNullException("editableAspect");
            }
            var index = EditorAspects.IndexOf(editableAspect);
            if (index < 0)
            {
                throw new ArgumentNullException("editableAspect");
            }
            EditorAspects.RemoveAt(index);
            var aspect = editableAspect.Aspect;
            var aspectType = aspect.GetType();
            var aspectFieldInGolem = typeof(Golem).GetField(aspectType.Name);
            if (aspectFieldInGolem == null || !aspectFieldInGolem.FieldType.IsAssignableFrom(aspectType))
            {
                throw new InvalidProgramException(typeof(Golem).Name + " must contain a field for " + aspectType.Name);
            }
            var currentFieldValue = aspectFieldInGolem.GetValue(Golem);
            if (currentFieldValue == null)
            {
                throw new InvalidOperationException("Aspect type " + aspectType.GetType() + " doesn't exist in golem");
            }
            if (!object.ReferenceEquals(currentFieldValue, aspect))
            {
                throw new InvalidOperationException("Aspect type " + aspectType.GetType() + " doesn't match the one currently in the golem");
            }
            aspectFieldInGolem.SetValue(Golem, null);
            for (int i = 0; i < EditorVariables.Count; ++i)
            {
                EditorVariables[i].SourceAspects.Remove(aspectType);
            }
            removeEditorVariablesWithoutSourceAspects();
        }

        private void removeEditorVariablesWithoutSourceAspects()
        {
            for (int i = EditorVariables.Count - 1; i >= 0; --i)
            {
                if (EditorVariables[i].SourceAspects.Count == 0)
                {
                    EditorVariables.RemoveAt(i);
                }
            }
        }

        //---------------------------------------------------------------------
        // Load
        //---------------------------------------------------------------------
        public void Load()
        {
            Debug.Assert(Golem != null);
            Debug.Assert(EditorAsset != null);
            Debug.Assert(EditorAsset == Golem.EditorAsset);
            // Debug.Log("References = " + Golem.References.Count());

            //-------------------------------------------------
            // Deserialize
            //-------------------------------------------------
            var serializer = Serialization.GetSerializer(Golem.References);
            Dictionary<string, object> deserialized = new Dictionary<string, object>();
            {
                fsData data;
                fsResult result;

                if (EditorAsset.Data != null)
                {
                    data = EditorAsset.Data;
                }
                else
                {
                    result = fsJsonParser.Parse(EditorAsset.Json, out data);
                    if (result.Failed)
                    {
                        Debug.LogError(result, Golem);
                        return;
                    }
                    EditorAsset.Data = data;
                }

                result = serializer.TryDeserialize(data, ref deserialized);
                if (result.Failed)
                {
                    Debug.LogError(result, Golem);
                    return;
                }
            }

            //-------------------------------------------------
            // Settings
            //-------------------------------------------------
            if (deserialized.ContainsKey("Settings"))
            {
                Settings = new Settings(Golem, EditorAsset.InheritSettingsFrom, deserialized["Settings"] as List<Settings.Setting>);
            }
            else
            {
                Settings = new Settings(Golem, EditorAsset.InheritSettingsFrom);
            }

            //-------------------------------------------------
            // Variables
            //-------------------------------------------------
            if (deserialized.ContainsKey("EditorVariables"))
            {
                EditorVariables = deserialized["EditorVariables"] as List<GolemVariableEditorData>;
            }

            //-------------------------------------------------
            // Aspects
            //-------------------------------------------------
            {
                EditorAspects.Clear();
                var aspectFields =
                        Golem.GetType()
                        .GetFields(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public)
                        .Where((f) => f.FieldType.IsSubclassOf(typeof(Aspect)));
                foreach (var aspectField in aspectFields)
                {
                    var aspect = aspectField.GetValue(Golem) as Aspect;
                    if (aspect == null)
                    {
                        continue;
                    }
                    AddAspect(aspect);
                }
            }

            removeEditorVariablesWithoutSourceAspects();

            //-------------------------------------------------
            // Circuit
            //-------------------------------------------------
            if (deserialized.ContainsKey("EditorCells"))
            {
                EditorCells = deserialized["EditorCells"] as List<EditorCell>;
                for (int i = 0; i < EditorCells.Count; ++i)
                {
                    EditorCells[i].Index = (EditorCellIndex)i;
                }
            }
            else
            {
                EditorCells = new List<EditorCell>();
            }

            if (deserialized.ContainsKey("EditorWires"))
            {
                EditorWires = deserialized["EditorWires"] as List<EditorWire>;
            }
            else
            {
                EditorWires = new List<EditorWire>();
            }

            //-------------------------------------------------
            // Program
            //-------------------------------------------------
            if (deserialized.ContainsKey("EditorStates"))
            {
                EditorStates = deserialized["EditorStates"] as List<EditorState>;
                for (int i = 0; i < EditorStates.Count; ++i)
                {
                    EditorStates[i].Index = (EditorStateIndex)i;
                }
            }
            else
            {
                EditorStates = new List<EditorState>();
            }

            if (deserialized.ContainsKey("EditorTransitions"))
            {
                EditorTransitions = deserialized["EditorTransitions"] as List<EditorTransition>;
            }
            else
            {
                EditorTransitions = new List<EditorTransition>();
            }
        }

        //---------------------------------------------------------------------
        // Save
        //---------------------------------------------------------------------
        public void Save()
        {
            // // Only write if this is a prefab instance
            // bool isPrefab = PrefabUtility.GetPrefabType(Entity) == PrefabType.PrefabInstance;
            // bool isRoot = PrefabUtility.FindRootGameObjectWithSameParentPrefab(Entity.gameObject) == Entity.gameObject;
            // if (!isPrefab || !isRoot)
            // {
            //     // TODO: allow non-root to premit template GameObject style instancing?
            //     return;
            // }

            // Holds data to be written to the entity asset
            Dictionary<string, object> serialized = new Dictionary<string, object>();

            //-------------------------------------------------------------
            // Write Variables
            //-------------------------------------------------------------
            {
                var variables = new Dictionary<string, object>();
                for (int i = 0; i < EditorVariables.Count; ++i)
                {
                    object initialValue = EditorVariables[i].InitialValue;
                    Type type = EditorVariables[i].Type;
                    Debug.Assert(type.IsValueType ? initialValue.GetType() == type : (initialValue == null || type.IsAssignableFrom(initialValue.GetType())));
                    variables.Add(EditorVariables[i].Name, EditorVariables[i].InitialValue);
                }
                serialized["Variables"] = variables;
            }

            //-------------------------------------------------------------
            // Write Aspects
            //-------------------------------------------------------------
            {
                var aspects = new Dictionary<string, Aspect>();
                for (int i = 0; i < EditorAspects.Count; ++i)
                {
                    var editorAspect = EditorAspects[i];
                    var aspect = editorAspect.Aspect;
                    ApplyFieldsUsingSettings(aspect, editorAspect.FieldsUsingSettings, Settings);
                    aspects[editorAspect.Field.Name] = aspect;
                }
                serialized["Aspects"] = aspects;
            }

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
                                if ((int)EditorWires[j].ReadCell.Index != i)
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
                    // be run rarely on graphs that are flat and small.
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
                        var originalCellIndex = depths;
                        for (int i = 0; i < EditorCells.Count; ++i)
                        {
                            originalCellIndex[i] = i;
                        }
                        System.Array.Sort(longestPath, originalCellIndex);
                        for (int i = 0; i < EditorCells.Count; ++i)
                        {
                            cellRemap[originalCellIndex[i]] = i;
                        }
                    }
                    else
                    {
                        Debug.LogError("Cycle in circuit graph detected; circuit will not function");
                        cellRemap = new int[0];
                    }
                }

                //---------------------------------------------------------
                // Build mappings required by the wires. Registers store values
                // on cell outputs. These registers then dirty other cells.
                //---------------------------------------------------------
                Debug.Log("TODO: map registers in an order that better matches the cell update order");
                List<object> registers = new List<object>();
                Dictionary<string, int> cellOutputToRegister = new Dictionary<string, int>();
                List<HashSet<EditorCellIndex>> cellsThatReadRegister = new List<HashSet<EditorCellIndex>>();
                for (int i = 0; i < EditorWires.Count; ++i)
                {
                    var wire = EditorWires[i];
                    string registerKey = wire.ReadCell.Index + ".{" + wire.ReadField + "}";
                    int register;
                    if (cellOutputToRegister.TryGetValue(registerKey, out register))
                    {
                        wire.Register = register;
                        cellsThatReadRegister[register].Add(wire.WriteCell.Index);
                    }
                    else
                    {
                        var readCellType = wire.ReadCell.Cell.GetType();
                        var readField = readCellType.GetField(wire.ReadField);
                        var fieldType = readField.FieldType;

                        if (!fieldType.IsValueType || !fieldType.Name.EndsWith("Ptr"))
                        {
                            Debug.LogError("Output field is not a *Ptr type. This is probably wrong!");
                        }

                        bool foundAttribute = false;
                        object value = null;
                        var attributes = fieldType.GetCustomAttributes(typeof(PointerTypeAttribute), false);
                        for (int j = 0; j < attributes.Length; ++j)
                        {
                            var pointerTypeAttribute = attributes[j] as PointerTypeAttribute;
                            if (pointerTypeAttribute != null)
                            {
                                if (pointerTypeAttribute.Type.IsValueType)
                                {
                                    foundAttribute = true;
                                    value = Activator.CreateInstance(pointerTypeAttribute.Type);
                                }
                                break;
                            }
                        }
                        if (!foundAttribute)
                        {
                            Debug.LogError("Add a [PointerType] attribute to '" + fieldType.Name + "'");
                            value = null;
                        }
                        register = registers.Count;
                        cellOutputToRegister[registerKey] = register;
                        registers.Add(value);
                        wire.Register = register;
                        cellsThatReadRegister.Add(new HashSet<EditorCellIndex>() { wire.WriteCell.Index });
                    }
                }
                int[][] cellsThatReadRegisterOutput = new int[cellsThatReadRegister.Count][];
                for (int i = 0; i < cellsThatReadRegister.Count; ++i)
                {
                    var editorCellIndicesThatReadThisRegister = cellsThatReadRegister[i];
                    var outputArray = new int[editorCellIndicesThatReadThisRegister.Count];
                    cellsThatReadRegisterOutput[i] = outputArray;
                    int j = 0;
                    foreach (var cellIndex in editorCellIndicesThatReadThisRegister)
                    {
                        outputArray[j++] = cellRemap[(int)cellIndex];
                    }
                }

                //---------------------------------------------------------
                // Write out all of the cells in the traversal order
                //---------------------------------------------------------
                Cell[] cells = new Cell[EditorCells.Count];
                for (int i = 0; i < EditorCells.Count; ++i)
                {
                    var editorCell = EditorCells[i];
                    var cell = editorCell.Cell.Clone();
                    cells[cellRemap[i]] = cell;
                    var cellType = cell.GetType();

                    //-------------------------------------------------
                    // Default all registers on the cell to invalid
                    //-------------------------------------------------
                    foreach (var inputField in cellType.GetFields(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public).Where((f) => f.IsDefined(typeof(InAttribute), false)))
                    {
                        inputField.SetValue(cell, int.MaxValue);
                    }
                    foreach (var outputField in cellType.GetFields(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public).Where((f) => f.IsDefined(typeof(OutAttribute), false)))
                    {
                        outputField.SetValue(cell, int.MaxValue);
                    }

                    //-------------------------------------------------
                    // Set the register indices for every active I/O
                    //-------------------------------------------------
                    var inputs = editorCell.Inputs;
                    for (int j = 0; j < inputs.Count; ++j)
                    {
                        string registerKey = inputs[j].ReadCell.Index + ".{" + inputs[j].ReadField + "}";
                        cellType.GetField(inputs[j].WriteField).SetValue(cell, cellOutputToRegister[registerKey]);
                    }
                    var outputs = editorCell.Outputs;
                    for (int j = 0; j < outputs.Count; ++j)
                    {
                        string registerKey = outputs[j].ReadCell.Index + ".{" + outputs[j].ReadField + "}";
                        cellType.GetField(outputs[j].ReadField).SetValue(cell, cellOutputToRegister[registerKey]);
                    }

                    //-------------------------------------------------
                    // Apply settings values
                    //-------------------------------------------------
                    ApplyFieldsUsingSettings(cell, editorCell.FieldsUsingSettings, Settings);
                }

                serialized["Registers"] = registers.ToArray();
                serialized["CellsThatReadRegister"] = cellsThatReadRegisterOutput;
                serialized["Cells"] = cells;
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
                    Debug.Assert(editorState.Index == (EditorStateIndex)i);
                    if (editorState.Index != (EditorStateIndex)i)
                    {
                        Debug.Log("Wrong editor state index; set to " + editorState.Index + " expecting " + i);
                    }
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

            //-----------------------------------------------------------------
            // Write the JSON for the golem into the golem asset
            //-----------------------------------------------------------------
            {
                Golem.References.Clear();
                var serializer = Serialization.GetSerializer(Golem.References);
                fsData data;
                serializer.TrySerialize(serialized, out data);
                Golem.Asset.Json = fsJsonPrinter.PrettyJson(data);
                Golem.Asset.Data = data;
                EditorUtility.SetDirty(Golem.Asset);
                // Debug.Log("References: " + Entity.References.Count + " @ " + Entity.References.GetHashCode());
            }

            //-----------------------------------------------------------------
            // Write the editor JSON
            //
            // This is done second so that register assignments get saved into
            // the wires. We can use this to inspect a running circuit!
            //-----------------------------------------------------------------
            {
                // By not clearing, the serializer will reuse the previous set of references
                // _circuitContainer.References.Clear();

                var serializer = Serialization.GetSerializer(Golem.References);
                serialized.Clear();
                serialized["EditorCells"] = EditorCells;
                serialized["EditorWires"] = EditorWires;
                serialized["EditorStates"] = EditorStates;
                serialized["EditorTransitions"] = EditorTransitions;
                serialized["EditorVariables"] = EditorVariables;
                serialized["Settings"] = Settings.Values;

                fsData data;
                serializer.TrySerialize(serialized, out data);
                EditorAsset.Json = fsJsonPrinter.PrettyJson(data);
                EditorAsset.Data = data;
                EditorUtility.SetDirty(EditorAsset);
                // Debug.Log("References: " + Golem.References.Count + " @ " + Golem.References.GetHashCode());
            }

            //-----------------------------------------------------------------
            // Mark everything we changed as needing to be saved
            //-----------------------------------------------------------------
            // var prefab = PrefabUtility.GetCorrespondingObjectFromSource(Entity);
            // EditorUtility.SetDirty(prefab);
            EditorUtility.SetDirty(Golem.gameObject);
        }

        private static HashSet<string> s_fieldNamesToRemove = new HashSet<string>();
        private static void ApplyFieldsUsingSettings(object target, Dictionary<string, string> fieldsUsingSettings, Settings settings)
        {
            s_fieldNamesToRemove.Clear();
            Type type = target.GetType();
            foreach (var keyValuePair in fieldsUsingSettings)
            {
                Debug.Assert(keyValuePair.Key != null);
                FieldInfo fieldInfo = type.GetField(keyValuePair.Key);
                if (fieldInfo == null)
                {
                    s_fieldNamesToRemove.Add(keyValuePair.Key);
                }
                else
                {
                    object valueFromSettings = settings.Get(keyValuePair.Value, fieldInfo.FieldType);
                    fieldInfo.SetValue(target, valueFromSettings);
                }
            }
            foreach (string name in s_fieldNamesToRemove)
            {
                fieldsUsingSettings.Remove(name);
            }
        }
    }
}
