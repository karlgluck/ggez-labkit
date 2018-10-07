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
using UnityObject = UnityEngine.Object;

namespace GGEZ.Labkit
{

    public class Golem2 : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        public UnityObject[] References;

        [SerializeField, HideInInspector]
        public GolemArchetype Archetype;

    #region Runtime

        /// All named variables for this golem
        [NonSerialized]
        public Dictionary<string, IVariable> Variables;

        /// Local copy of each aspect from the archetype
        [NonSerialized]
        public Aspect[] Aspects;

        /// Runtime data needed by each component
        [NonSerialized]
        public GolemComponentRuntimeData[] Components;

        /// Trigger state for this golem
        [NonSerialized]
        public bool[] Triggers;

    #endregion

        /// <summary>Gets a Unity Object reference for this golem by name</summary>
        /// <returns>The matching reference or null if none exists</returns>
        /// <remarks>Used during setup</remarks>
        public UnityEngine.Object GetUnityObjectReference(string reference)
        {
            Debug.Assert(Application.isPlaying);
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
            Debug.Assert(Application.isPlaying);
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

        public void SetTrigger(Trigger trigger)
        {
            Debug.Assert(Application.isPlaying);
            Triggers[(int)trigger] = true;
        }

        void Reset()
        {
            References = new UnityEngine.Object[0];
            Archetype = ScriptableObject.CreateInstance<GolemArchetype>();
            Variables = null;
            Aspects = null;
            Components = null;
            Triggers = null;
        }

        void OnValidate()
        {
            if (Archetype == null)
            {
                Archetype = ScriptableObject.CreateInstance<GolemArchetype>();
            }

            if (!Application.isPlaying)
            {
                Variables = null;
                Aspects = null;
                Components = null;
                Triggers = null;
            }
        }
    }
}