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
using StringCollection = System.Collections.Generic.List<string>;

namespace GGEZ
{
namespace Omnibus
{


[
Serializable,
AddComponentMenu ("GGEZ/Omnibus/Modules/Animator/Animator Wrapper (Module)"),
RequireComponent (typeof (RectTransform))
]
public sealed class AnimatorModule : Cell
{

#region Programming Interface
public Bus Bus
    {
    get { return this.bus; }
    set
        {
        this.bus = value;
        this.refresh ();
        }
    }


#endregion


[Header ("*:" + Omnibus.Pin.DATA + " (string)")]
[SerializeField] private Bus bus;

[Space]
[SerializeField] private StringPairs pinToFloatParameterPairs = new StringPairs ();

public override void OnDidSignal (string pin, object value)
    {
	Debug.Assert (pin == Omnibus.Pin.DATA);
	int layer = value == null ? 1 : LayerMask.NameToLayer (value.ToString ());
#if UNITY_EDITOR
	if (layer == ~0)
		{
		Debug.LogWarning ("Layer name " +  value.ToString () + " is invalid");
		}
#endif
	this.gameObject.layer = layer;
    }

public override void Route (string port, Bus bus)
    {
	this.Bus = bus;
    }


void OnEnable ()
    {
    // this.input.Attach (this, this.bus, this.pin);
    }

void OnDisable ()
    {
    // this.input.Detach ();
    }

void OnValidate ()
    {
	this.refresh ();
    }

void refresh ()
	{
	}

}


}
}

