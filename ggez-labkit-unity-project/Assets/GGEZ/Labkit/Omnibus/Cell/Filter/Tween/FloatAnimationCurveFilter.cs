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
using Image = UnityEngine.UI.Image;


namespace GGEZ.Omnibus
{


[Serializable] public sealed class UnityEventForFloatAnimationCurveFilter : UnityEngine.Events.UnityEvent<float> { }


[
Serializable,
AddComponentMenu ("GGEZ/Omnibus/Filters/Float Animation Curve (Filter)")
]
public abstract class FloatAnimationCurveFilter : Cell
{

#region Programming Interface

public Bus Bus
    {
    get { return this.bus; }
    set
        {
        this.bus = value;
        this.inputWire.Connect (this.bus, this.pin);
        }
    }

public string Pin
    {
    get { return this.pin; }
    set
        {
        this.pin = value;
        this.inputWire.Connect (this.bus, this.pin);
        }
    }

#endregion



[Header ("*:" + Omnibus.Pin.INPUT + " (float)")]
[SerializeField] private Bus bus;
[SerializeField] private string pin;

[Space, SerializeField] private UnityEventForFloatAnimationCurveFilter didSignal = new UnityEventForFloatAnimationCurveFilter ();

[Header ("Settings")]
[SerializeField] private AnimationCurve curve;

private Wire inputWire = Wire.CELL_INPUT;

private bool hasEmittedOutput;
private float input = 0f;
private float _output = 0f;
private void updateOutput ()
    {
    var value = this.curve.Evaluate (this.input);
    if (this._output != value || !hasEmittedOutput)
        {
		this.hasEmittedOutput = true;
		this._output = value;
        this.didSignal.Invoke (value);
        }
    }

public override void OnDidSignal (string pin, object value)
    {
	Debug.Assert (pin == Omnibus.Pin.INPUT);
#if UNITY_EDITOR
    if (value == null || !typeof(float).IsAssignableFrom (value.GetType ()))
        {
        throw new System.InvalidCastException ("`value` should be " + typeof(float).Name);
        }
#endif
	this.input = (float)value;
    this.updateOutput ();
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

