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

namespace GGEZ
{
namespace Omnibus
{



[
Serializable,
AddComponentMenu ("GGEZ/Omnibus/Modules/RectTransform/Rect Sets Anchors (Module)"),
RequireComponent (typeof (RectTransform))
]
public class RectSetsAnchorsModule : Cell
{

#region Programming Interface
public Bus Bus
    {
    get { return this.bus; }
    set
        {
        this.bus = value;
        this.input.Connect (this.bus, this.pin);
        }
    }

public string Pin
    {
    get { return this.pin; }
    set
        {
        this.pin = value;
        this.input.Connect (this.bus, this.pin);
        }
    }

#endregion


[Header ("*:" + Omnibus.Pin.INPUT + " (Rect)")]
[SerializeField] private Bus bus;
[SerializeField] private string pin;

private Wire input = Wire.CELL_INPUT;
private RectTransform rectTransform;

public override void OnDidSignal (string pin, object value)
    {
	Debug.Assert (pin == Omnibus.Pin.INPUT);
#if UNITY_EDITOR
    if (value == null || !typeof(Rect).IsAssignableFrom (value.GetType ()))
        {
        throw new System.InvalidCastException ("`value` should be " + typeof(Rect).Name);
        }
#endif
    var rectValue = (Rect)value;
    this.rectTransform.anchorMin = rectValue.min;
    this.rectTransform.anchorMax = rectValue.max;
    }

public override void Route (string port, Bus bus)
    {
	this.Bus = bus;
    }


void Awake ()
    {
    this.rectTransform = (RectTransform)this.GetComponent (typeof (RectTransform));
    }

void OnEnable ()
    {
    this.input.Attach (this, this.bus, this.pin);
    }

void OnDisable ()
    {
    this.input.Detach ();
    }

void OnValidate ()
    {
	this.input.Connect (this.bus, this.pin);
    }


}

}

}





