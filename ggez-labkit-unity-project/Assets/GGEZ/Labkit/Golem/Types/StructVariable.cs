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

namespace GGEZ.Labkit
{

    [GGEZ.FullSerializer.fsIgnore]
    public sealed class StructVariable<T> : SingleValueVariable<T> where T : struct, IEquatable<T>
    {
        /// <summary>The backing register for this variable</summary>
        private StructRegister<T> _register;

        /// <summary>Next frame value</summary>
        private T _value;

        /// <summary>Returns the value of this variable at the end of the last frame</summary>
        public override T Value
        {
            get
            {
                return _register.Value;
            }
            set
            {
                _value = value;
                QueueForRolloverPhase();
            }
        }

        /// <summary>Create a variable with a new backing register</summary>
        public StructVariable()
        {
            _register = new StructRegister<T>();
        }

        /// <summary>Create a variable for the given backing register</summary>
        public StructVariable(StructRegister<T> register)
        {
            _register = register;
        }
        
        /// <summary>The backing register for this variable</summary>
        public override Register GetRegister()
        {
            return _register;
        }

        /// <summary>Updates the value of the register that backs this variable</summary>
        public override void OnVariableRolloverPhase()
        {
            _register.Value = _value;
        }

    }
}
