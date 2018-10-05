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
using GGEZ.FullSerializer;

namespace GGEZ.Labkit
{

    public class Golem2 : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        public UnityEngine.Object[] References;

        [SerializeField, HideInInspector]
        public GolemArchetype Archetype;

        #region Runtime
        public Dictionary<string, IVariable> Variables;
        public Aspect[] Aspects;
        public class ComponentData
        {
            public Cell[] Cells;
            public Script[] Scripts;
            public GGEZ.Labkit.StateIndex[] LayerStates;
        }
        public ComponentData[] Components;
        public bool[] Triggers;

        #endregion

        /// <summary>Gets a Unity Object reference for this golem by name</summary>
        /// <returns>The matching reference or null if none exists</returns>
        /// <remarks>Used during setup</remarks>
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

        /// <summary>Find an aspect of the given type on this golem</summary>
        /// <returns>The matching aspect or null if none exists</returns>
        /// <remarks>Used during setup</remarks>
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

        void Reset()
        {
            References = new UnityEngine.Object[0];
            Archetype = ScriptableObject.CreateInstance<GolemArchetype>();
            Variables = new Dictionary<string, IVariable>();
            Aspects = new Aspect[0];
            Components = new ComponentData[0];
            Triggers = new bool[(int)Trigger.__COUNT__];
        }
    }








    public class AddFloat : Cell
    {
        // [In] // Hooked up to read-only input if not attached
        // [In, CanBeNull]  // Not hooked up if not attached
        public StructRegister<float> Input;

        // [Out] // Hooked up to write-only output if not attached
        // [OutOrNull] // Not hooked up if not attached
        public StructRegister<float> Output;

        public float Amount;

        public override void Update()
        {
            Output.Value = Input.Value + Amount;
        }
    }

}



namespace GGEZ.Labkit
{

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

}
