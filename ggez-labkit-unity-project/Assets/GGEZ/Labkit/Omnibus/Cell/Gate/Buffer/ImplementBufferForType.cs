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
using UnityEngine.Events;
using System.Collections.Generic;

namespace GGEZ.Omnibus
{


public abstract class ImplementBufferForType<T, D> : Cell where D : OneInputTerminal<T>
{

#region Programming Interface

public string Pin
    {
    set
        {
        this.pin = value;
        this.wireIn.Connect (this.bus, value);
        }
    }

public Bus Bus
    {
    set
        {
        this.bus = value;
        this.wireIn.Connect (value, this.pin);
        }
    }

#endregion




private Wire wireIn = Wire.CELL_INPUT;

[Header ("*:" + Pin.INPUT + " (type)")]
[SerializeField] private Bus bus;
[SerializeField] private string pin;

[Space]
[SerializeField] private D terminal;

void OnEnable ()
    {
    this.wireIn.Attach (this, this.bus, this.pin);
    }

void OnDisable ()
    {
    this.wireIn.Detach ();
    }

void OnValidate ()
    {
    this.wireIn.Connect (this.bus, this.pin);
    OneInputTerminal<T>.FindTerminal (this, ref this.terminal);
    }


public override void Route (string port, Bus bus)
    {
    this.Bus = bus;
    }


public override void OnDidSignal (string pin, object value)
    {
#if UNITY_EDITOR
    if (value != null && !typeof(T).IsAssignableFrom (value.GetType ()))
        {
        throw new System.InvalidCastException ("`value` should be " + typeof(T).Name);
        }
#endif
    this.terminal.Signal ((T)value);
    }
}





}
