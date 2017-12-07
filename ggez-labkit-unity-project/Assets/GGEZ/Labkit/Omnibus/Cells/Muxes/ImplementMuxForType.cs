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



public class ImplementMuxForType<T, D> : Cell where D : UnityEvent<T>, new ()
{


#region Programming Interface

public Bus DataBus
    {
    get { return this.dataBus; }
    set
        {
        this.dataBus = value;
        this.refresh ();
        }
    }

public Bus StringBus
    {
    get { return this.stringBus; }
    set
        {
        this.stringBus = value;
        this.refresh ();
        }
    }

public string StringPin
    {
    get { return this.stringPin; }
    set
        {
        this.stringPin = value;
        this.refresh ();
        }
    }

#endregion



[SerializeField] private Bus dataBus;
[SerializeField] private string inputPin;
[SerializeField] private Bus stringBus;
[SerializeField] private string stringPin;



public override void OnDidSignal (string pin, object value)
    {
    switch (pin)
        {
        case Pin.IN:
            this.dataInput.Connect (value == null ? null : value.ToString ());
            break;
        case Pin.DATA:
#if UNITY_EDITOR
            if (value == null ? typeof(T).IsValueType : !typeof(T).IsAssignableFrom (value.GetType ()))
                {
                throw new System.InvalidCastException ("`value` should be " + typeof(T).Name);
                }
#endif
            this.didChange.Invoke ((T)value);
            break;
        }
    Debug.Assert (this.stringBus != null && Pin.IsValid (this.stringPin));
    this.stringBus.SetObject (this.stringPin, value);
    }

public override void Route (string net, Bus bus)
    {
    switch (net)
        {
        case "data": this.DataBus = bus; break;
        case "in": this.StringBus = bus; break;
        default:
            {
            this.DataBus = bus;
            this.StringBus = bus;
            break;
            }
        }
    }

private Wire stringInput = Wire.CELL_IN;
private Wire dataInput = Wire.CELL_DATA;

private string dataPin
    {
    get
        {
        var obj = this.stringBus == null || Pin.IsInvalid (this.stringPin) ? null : this.stringBus.GetObject (this.stringPin);
        var pin = obj == null ? null : obj.ToString ();
        return pin;
        }
    }

void OnEnable ()
    {
    this.dataInput.Attach (this, this.dataBus, this.dataPin);
    this.stringInput.Attach (this, this.stringBus, this.stringPin);
    }

void OnDisable ()
    {
    this.dataInput.Detach ();
	this.stringInput.Detach ();
    }

void OnValidate ()
    {
    this.refresh ();
    }

private void refresh ()
    {
	this.stringInput.Connect (this.stringBus, this.inputPin);
	this.dataInput.Connect (this.dataBus, this.dataPin);
    }

}



}
}
