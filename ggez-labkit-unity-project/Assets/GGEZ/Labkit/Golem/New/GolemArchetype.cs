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
    /// <summary>
    /// An Archetype is created for every unique Golem and is shared among all its clones.
    /// The Archetype collects the set of Aspects that the golem contains and the
    /// set of Components that it uses, then ties those together with Settings values,
    /// named References and relationship-based external Variables.
    /// </summary>
    public class GolemArchetype : ScriptableObject, ISerializationCallbackReceiver
    {
        /// <summary>Pairs with the References field in each golem instance</summary>
        public string[] ReferenceNames;

        /// <summary>Functional parts used by the golem</summary>
        public GolemComponent[] Components;

        [NonSerialized]
        public Aspect[] Aspects;

        public SettingsAsset InheritSettingsFrom;
        [NonSerialized]
        public Settings Settings;

        public Assignment[] Assignments;
        [NonSerialized]
        public Dictionary<string, Assignment[]> ExternalAssignments;

        /// <summary>Source of data for all the NonSerialized properties</summary>
        public string Json;

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
        }
    }
}