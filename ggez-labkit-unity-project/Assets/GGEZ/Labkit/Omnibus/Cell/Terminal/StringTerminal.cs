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

namespace GGEZ
{
namespace Omnibus
{

[Serializable] public sealed class UnityEventForStringTerminal : UnityEngine.Events.UnityEvent<string> { }

[
Serializable,
AddComponentMenu ("GGEZ/Omnibus/Terminal/String Terminal")
]
public class StringTerminal : Cell
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

public void AddCallback (UnityAction<string> action)
    {
    this.didSignal.AddListener (action);
    }

public void RemoveCallback (UnityAction<string> action)
    {
    this.didSignal.RemoveListener (action);
    }

public void RemoveAllCallbacks ()
    {
    this.didSignal.RemoveAllListeners ();
    }


#endregion

[Header ("*:" + Pin.INPUT + " (string)")]

[SerializeField] private Bus bus;
[SerializeField] private string pin;

[Space]
[SerializeField] private UnityEventForStringTerminal didSignal = new UnityEventForStringTerminal ();

private Wire wireIn = Wire.CELL_INPUT;

public override void OnDidSignal (string pin, object value)
    {
    Debug.Assert (pin == Omnibus.Pin.INPUT);
    string stringValue = value == null ? null : value.ToString ();
    this.didSignal.Invoke (stringValue);
    }

public override void Route (string port, Bus bus)
    {
    this.Bus = bus;
    }

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
    }

}

}
}
