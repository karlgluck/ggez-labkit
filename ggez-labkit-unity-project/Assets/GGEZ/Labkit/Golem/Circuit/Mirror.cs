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
using GGEZ.FullSerializer;
using Variables = System.Collections.Generic.Dictionary<string, GGEZ.Labkit.Variable>;
using VariablesSet = System.Collections.Generic.HashSet<System.Collections.Generic.Dictionary<string, GGEZ.Labkit.Variable>>;


using System.Linq;

namespace GGEZ.Labkit
{

    public class MirrorVariablesAsGolems : Cell
    {
        public VariablesSetRegister Input;
        private VariablesSetRegister _lastInput;

        [GGEZ.FullSerializer.fsIgnore]
        public GameObject GolemPrefab;

        [GGEZ.FullSerializer.fsIgnore]
        public Transform Parent;

        public string InputReferenceName;
        public string OwnerReferenceName;

        private Dictionary<Variables, Golem> Spawned = new Dictionary<Variables, Golem>(VariablesSetRegister.EqualityComparer);

        public override void Release()
        {
            foreach (Golem golem in Spawned.Values)
                GolemManager.ReleaseGolem(golem);

            Spawned.Clear();
        }

        public override void Update()
        {
            if (object.ReferenceEquals(_lastInput, Input))
            {
                AcquireAll(Input.Added);
                ReleaseAll(Input.Removed);
            }
            else
            {
                HashSet<Variables> toAdd = Input == null ? null : Input.Values;
                if (_lastInput != null)
                {
                    // Release all previous values including those that were lost since last frame
                    HashSet<Variables> toRelease = new HashSet<Variables>(_lastInput.Values, VariablesSetRegister.EqualityComparer);
                    toRelease.UnionWith(_lastInput.Removed);

                    // Don't release variables present in the new set, if it exists
                    if (Input != null)
                    {
                        toRelease.ExceptWith(Input.Values);
                        toAdd = new HashSet<Variables>(toAdd, VariablesSetRegister.EqualityComparer);
                        toAdd.ExceptWith(toRelease);
                    }

                    ReleaseAll(toRelease);
                }

                if (toAdd != null)
                {
                    AcquireAll(toAdd);
                }

                _lastInput = Input;
            }
        }

        private void AcquireAll(HashSet<Variables> variables)
        {
            Golem golemPrefab = GolemPrefab.GetComponent<Golem>();
            foreach (Variables key in variables)
            {
                Golem golem = GolemManager.AcquireGolem(golemPrefab);
                golem.transform.SetParent(Parent, false);
                Spawned.Add(key, golem);
            }
        }

        private void ReleaseAll(HashSet<Variables> variables)
        {
            foreach (Variables key in variables)
            {
                Golem golem;
                if (Spawned.TryGetValue(key, out golem))
                {
                    GolemManager.ReleaseGolem(golem);
                    Spawned.Remove(key);
                }
            }
        }
    }

}