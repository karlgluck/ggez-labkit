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

public Bus SelectBus
    {
    get { return this.selectBus; }
    set
        {
        this.selectBus = value;
        this.refresh ();
        }
    }

public string SelectPin
    {
    get { return this.selectPin; }
    set
        {
        this.selectPin = value;
        this.refresh ();
        }
    }

#endregion



[Header ("data:" + Pin.DATA + " (type)")]
[SerializeField] private Bus dataBus;

[Header ("select:" + Pin.SELECT + " (string)")]
[SerializeField] private Bus selectBus;
[SerializeField] private string selectPin;

[Space]
[SerializeField] private D didSignal = new D ();

public override void OnDidSignal (string pin, object value)
    {
    switch (pin)
        {

        case Pin.SELECT:
            this.dataWire.Connect (value == null ? null : value.ToString ());
            break;

        case Pin.DATA:
#if UNITY_EDITOR
            if (value == null ? typeof(T).IsValueType : !typeof(T).IsAssignableFrom (value.GetType ()))
                {
                throw new System.InvalidCastException ("`value` should be " + typeof(T).Name);
                }
#endif
            this.didSignal.Invoke ((T)value);
            break;

#if UNITY_EDITOR
        default:
            Debug.LogError ("signal on invalid pin: " + pin);
            break;
#endif

        }
    }

public override void Route (string port, Bus bus)
    {
    switch (port)
        {
        case "data":   this.DataBus = bus; break;
        case "select": this.SelectBus = bus; break;
        default:
            {
            this.DataBus = bus;
            this.SelectBus = bus;
            break;
            }
        }
    }

private Wire dataWire = Wire.CELL_DATA;
private Wire selectWire = Wire.CELL_SELECT;

private string dataPin
    {
    get
        {
        var obj = this.selectBus == null || Pin.IsInvalid (this.selectPin) ? null : this.selectBus.GetObject (this.selectPin);
        var pin = obj == null ? null : obj.ToString ();
        return pin;
        }
    }

void OnEnable ()
    {
    this.dataWire.Attach (this, this.dataBus, this.dataPin);
    this.selectWire.Attach (this, this.selectBus, this.selectPin);
    }

void OnDisable ()
    {
    this.dataWire.Detach ();
	this.selectWire.Detach ();
    }

void OnValidate ()
    {
    this.refresh ();
    }

private void refresh ()
    {
	this.selectWire.Connect (this.selectBus, this.selectPin);
	this.dataWire.Connect (this.dataBus, this.dataPin);
    }

}



}
}
