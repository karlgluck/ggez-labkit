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

namespace GGEZ.Omnibus
{



[
Serializable,
AddComponentMenu ("GGEZ/Omnibus/Modules/GameObject/Set gameObject.layer by Name with Enable (Module)")
]
public sealed class SetGameObjectLayerByNameWithEnableModule : Cell
{

#region Programming Interface

public Bus Bus
    {
    get { return this.bus; }
    set
        {
        this.bus = value;
        this.layerWire.Connect (this.bus, this.layerPin);
        }
    }

public string LayerPin
    {
    get { return this.layerPin; }
    set
        {
        this.layerPin = value;
        this.layerWire.Connect (this.bus, this.layerPin);
        }
    }


public string EnablePin
    {
    get { return this.enablePin; }
    set
        {
        this.enablePin = value;
        this.enableWire.Connect (this.bus, this.enablePin);
        }
    }

#endregion


[Header ("*:" + Omnibus.Pin.DATA + " (string)")]
[SerializeField] private Bus bus;
[SerializeField] private string layerPin;

[Header ("*:" + Omnibus.Pin.ENABLE + " (bool)")]
[SerializeField] private string enablePin;


[Header ("Settings")]
[SerializeField, Layer] private int defaultLayer;
[SerializeField] private bool invertEnable;


private bool enable;
private int layer;

public override void OnDidSignal (string pin, object value)
    {

    switch (pin)
        {

        case Pin.ENABLE:

#if UNITY_EDITOR
            if (value == null || !typeof(bool).IsAssignableFrom (value.GetType ()))
                {
                throw new System.InvalidCastException ("`value` should be " + typeof(bool).Name);
                }
#endif
            this.enable = this.invertEnable != (bool)value;
            break;

        case Pin.DATA:
            this.layer = value == null ? 1 : LayerMask.NameToLayer (value.ToString ());
#if UNITY_EDITOR
            if (this.layer == ~0)
                {
                Debug.LogWarning ("Layer name " +  value.ToString () + " is invalid");
                }
#endif
            break;

#if UNITY_EDITOR
        default:
            Debug.LogError ("signal on invalid pin: " + pin);
            break;
#endif

        }

    if (this.layerWire.IsConnected && this.enableWire.IsConnected)
        {
        this.gameObject.layer = this.enable ? this.layer : this.defaultLayer;
        }

    }

public override void Route (string port, Bus bus)
    {
    this.Bus = bus;
    }

private Wire layerWire = Wire.CELL_DATA;
private Wire enableWire = Wire.CELL_ENABLE;

void OnEnable ()
    {
    this.layerWire.Attach (this, this.bus, this.layerPin);
    this.enableWire.Attach (this, this.bus, this.enablePin);
    }

void OnDisable ()
    {
    this.layerWire.Detach ();
    this.enableWire.Detach ();
    }

void OnValidate ()
    {
    this.layerWire.Disconnect ();
    this.enableWire.Disconnect ();
    this.layerWire.Connect (this.bus, this.layerPin);
    this.enableWire.Connect (this.bus, this.enablePin);
    }

}

}
