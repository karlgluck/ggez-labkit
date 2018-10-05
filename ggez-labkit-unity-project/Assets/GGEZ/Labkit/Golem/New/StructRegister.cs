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

namespace GGEZ.Labkit
{
    public class StructRegister<T> : IRegister where T : struct, IEquatable<T>
    {
        private T _value;

        private List<Cell> _listeners;

        public T Value
        {
            get { return _value; }
            set { ChangeValue(value); }
        }

        public IRegister Clone ()
        {
            return MemberwiseClone() as IRegister;
        }

        public IVariable CreateVariable()
        {
            return new StructVariable<T>(this);
        }

        public bool ChangeValue(T value)
        {
            if (_value.Equals(value))
            {
                return false;
            }
            _value = value;
            if (_listeners != null)
            {
                GolemManager.AddChangedCellInputs(_listeners);
            }
            return true;
        }


        public void AddListener(Cell cell)
        {
            if (_listeners == null)
            {
                _listeners = new List<Cell>();
            }
            _listeners.Add(cell);
        }

        public void RemoveListener(Cell cell)
        {
            if (_listeners != null)
            {
                _listeners.Remove(cell);
            }
        }
    }
}
