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

namespace GGEZ.Omnibus
{


[
Serializable,
AddComponentMenu ("GGEZ/Omnibus/Router/Send Bus on GameObject from Signal to Router (Module)")
]
public class BusRouterModule : Cell
{

public string Pin
    {
    set
        {
        this.pin = value;
        this.inputWire.Connect (this.bus, value);
        }
    }

public Bus Bus
    {
    set
        {
        this.bus = value;
        this.inputWire.Connect (value, this.pin);
        }
    }


[Header ("*:" + Pin.INPUT + "(GameObject)")]

[SerializeField] private Bus bus;
[SerializeField] private string pin;

[Header ("Output")]
[SerializeField] private Router router;

private Wire inputWire = Wire.CELL_INPUT;


public override void OnDidSignal (string pin, object value)
    {
    Debug.Assert (pin == Omnibus.Pin.INPUT);
#if UNITY_EDITOR
    if (value != null && !typeof(GameObject).IsAssignableFrom (value.GetType ()))
        {
        throw new System.InvalidCastException ("`value` should be " + typeof(GameObject).Name);
        }
#endif
	if (value == null)
		{
		this.router.Route (null);
		}
	else
		{
		this.router.Route ((Bus)((GameObject)value).GetComponent (typeof (Bus)));
		}
    }

public override void Route (string port, Bus bus)
    {
    this.Bus = bus;
    }

void OnEnable ()
    {
    this.inputWire.Attach (this, this.bus, this.pin);
    }

void OnDisable ()
    {
    this.inputWire.Detach ();
    }

void OnValidate ()
    {
    this.inputWire.Connect (this.bus, this.pin);
    }

}



}
