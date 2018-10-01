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
