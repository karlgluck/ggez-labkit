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
using Variables = System.Collections.Generic.Dictionary<string, GGEZ.Labkit.Variable>;
using VariablesSet = System.Collections.Generic.HashSet<System.Collections.Generic.Dictionary<string, GGEZ.Labkit.Variable>>;

namespace GGEZ.Labkit
{
    [GGEZ.FullSerializer.fsIgnore]
    public sealed class VariablesSetRegister : CollectionRegister<Variables>
    {
        private VariablesSet _values = new VariablesSet(EqualityComparer);
        private VariablesSet _added = new VariablesSet(EqualityComparer);
        private VariablesSet _removed = new VariablesSet(EqualityComparer);

        public VariablesSet Values { get { return _values; } }
        public VariablesSet Added { get { return _added; } }
        public VariablesSet Removed { get { return _removed; } }

        protected override ICollection<Variables> ValuesCollection { get { return _values; } }
        protected override ICollection<Variables> AddedCollection { get { return _added; } }
        protected override ICollection<Variables> RemovedCollection { get { return _removed; } }

        public override Variable CreateVariable()
        {
            #warning TODO make sure only up to 1 variable is ever created for each register
            return new VariablesSetVariable(this);
        }
        
        public static readonly IEqualityComparer<Variables> EqualityComparer = new VariablesSetEqualityComparer();
        private sealed class VariablesSetEqualityComparer : IEqualityComparer<Variables>
        {
            public bool Equals(Variables a, Variables b)
            {
                return object.ReferenceEquals(a, b);
            }

            public int GetHashCode(Variables obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
