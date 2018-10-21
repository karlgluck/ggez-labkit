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
using System.Collections.Generic;
using Variables = System.Collections.Generic.Dictionary<string, GGEZ.Labkit.Variable>;
using VariablesSet = System.Collections.Generic.HashSet<System.Collections.Generic.Dictionary<string, GGEZ.Labkit.Variable>>;


namespace GGEZ.Labkit
{
    [GGEZ.FullSerializer.fsIgnore]
    public sealed class VariablesSetVariable : CollectionVariable<Variables>
    {
        /// <summary>The backing register for this variable</summary>
        private VariablesSetRegister _register;

        // Values to send to the register
        private VariablesSet _added = new VariablesSet(VariablesSetRegister.EqualityComparer);
        private VariablesSet _removed = new VariablesSet(VariablesSetRegister.EqualityComparer);

        public VariablesSet Values { get { return _register.Values; } }
        public VariablesSet Added { get { return _register.Added; } }
        public VariablesSet Removed { get { return _register.Removed; } }
        
        protected override ICollection<Variables> VariableAddedCollection { get { return _added; } }
        protected override ICollection<Variables> VariableRemovedCollection { get { return _removed; } }

        public VariablesSetVariable() : this(null) { }

        public VariablesSetVariable(VariablesSetRegister register)
        {
            _register = register ?? new VariablesSetRegister();
        }

        /// <summary>The backing register for this variable</summary>
        public override Register GetRegister()
        {
            return _register;
        }

    }
}
