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
using PinToIntDict = System.Collections.Generic.Dictionary<string, int>;

namespace GGEZ.Omnibus
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
        if (object.ReferenceEquals (this.bus, value))
            {
            return;
            }
        this.bus = value;
        for (int i = 0; i < this.wires.Count; ++i)
            {
            this.wires[i].Connect (this.bus);
            }
        }
    }


#endregion


[SerializeField] private Bus bus;

[Header ("*:FPx (float)")]
[SerializeField] private StringPairs pinToFloatParameters = new StringPairs ();

[Header ("*:LWx (float)")]
[SerializeField] private StringPairs pinToLayerWeights = new StringPairs ();

[Header ("*:IPx (int)")]
[SerializeField] private StringPairs pinToIntParameters = new StringPairs ();

[Header ("*:BPx (bool)")]
[SerializeField] private StringPairs pinToBoolParameters = new StringPairs ();

[Header ("*:TRx (void)")]
[SerializeField] private StringPairs pinToTriggers = new StringPairs ();

private Animator animator;

private PinToIntDict pinToFloatParameterId = new PinToIntDict ();
private PinToIntDict pinToLayerId = new PinToIntDict ();
private PinToIntDict pinToIntParameterId = new PinToIntDict ();
private PinToIntDict pinToBoolParameterId = new PinToIntDict ();
private PinToIntDict pinToTriggerId = new PinToIntDict ();

public override void OnDidSignal (string pin, object value)
    {
    Debug.Assert (pin.Length > 2);
    var cc = pin.Substring (0, 2);
    switch (cc)
        {
        case "FP":
            {

#if UNITY_EDITOR
            if (value == null || !typeof(float).IsAssignableFrom (value.GetType ()))
                {
                throw new System.InvalidCastException ("`value` should be " + typeof(float).Name);
                }
#endif

            this.animator.SetFloat (this.pinToFloatParameterId[pin], (float)value);

            }
            break;

        case "LW":
            {

#if UNITY_EDITOR
            if (value == null || !typeof(float).IsAssignableFrom (value.GetType ()))
                {
                throw new System.InvalidCastException ("`value` should be " + typeof(float).Name);
                }
#endif

            this.animator.SetLayerWeight (this.pinToLayerId[pin], (float)value);

            }
            break;

        case "IP":
            {

#if UNITY_EDITOR
            if (value == null || !typeof(int).IsAssignableFrom (value.GetType ()))
                {
                throw new System.InvalidCastException ("`value` should be " + typeof(int).Name);
                }
#endif

            this.animator.SetInteger (this.pinToIntParameterId[pin], (int)value);

            }
            break;

        case "BP":
            {

#if UNITY_EDITOR
            if (value == null || !typeof(bool).IsAssignableFrom (value.GetType ()))
                {
                throw new System.InvalidCastException ("`value` should be " + typeof(bool).Name);
                }
#endif

            this.animator.SetBool (this.pinToBoolParameterId[pin], (bool)value);

            }
            break;

        case "TR":
            {

            this.animator.SetTrigger (this.pinToTriggerId[pin]);

            }
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
	this.Bus = bus;
    }

void Awake ()
	{
	this.animator = (Animator)this.GetComponent (typeof (Animator));
	}

void OnEnable ()
    {
    this.updateWireCount ();
    for (int i = 0; i < this.wires.Count; ++i)
        {
        this.wires[i].Attach (this);
        }
    }

void OnDisable ()
    {
    for (int i = 0; i < this.wires.Count; ++i)
        {
        this.wires[i].Detach ();
        }
    }

void OnValidate ()
    {
    this.animator = (Animator)this.GetComponent (typeof (Animator));
    if (this.updateWireCount ())
        {
        return;
        }
    int wireIndex = 0;
    if (this.reconnectWiresForAnimatorParameterDictionary (ref wireIndex, this.pinToFloatParameters, "FP", this.pinToFloatParameterId)
            || this.reconnectWiresForLayerDictionary (ref wireIndex, this.pinToLayerWeights, "LW", this.pinToLayerId)
            || this.reconnectWiresForAnimatorParameterDictionary (ref wireIndex, this.pinToIntParameters, "IP", this.pinToIntParameterId)
            || this.reconnectWiresForAnimatorParameterDictionary (ref wireIndex, this.pinToBoolParameters, "BP", this.pinToBoolParameterId)
            || this.reconnectWiresForAnimatorParameterDictionary (ref wireIndex, this.pinToTriggers, "TR", this.pinToTriggerId))
        {
        this.forceFullRebuildOfAllWires ();
        }
    }

void forceFullRebuildOfAllWires ()
    {

    Debug.LogWarning ("Forcing full rebuild of AnimatorModule wires. This should not be necessary unless you add and remove values from different tables in the editor in one update. So... knock that off.");

    bool wasAttached = false;
    for (int i = 0; i < this.wires.Count; ++i)
        {
        wasAttached = this.wires[i].IsAttached;
        this.wires[i].Detach ();
        }
    this.wires.Clear ();
    this.updateWireCount ();
    if (wasAttached)
        {
        for (int i = 0; i < this.wires.Count; ++i)
            {
            this.wires[i].Attach (this);
            }
        }
    }

bool updateWireCount ()
    {
    int wiresNeeded = 0;
    wiresNeeded += this.pinToFloatParameters.Keys.Count;
    wiresNeeded += this.pinToLayerWeights.Keys.Count;
    wiresNeeded += this.pinToIntParameters.Keys.Count;
    wiresNeeded += this.pinToBoolParameters.Keys.Count;
    wiresNeeded += this.pinToTriggers.Keys.Count;
    if (this.wires.Count == wiresNeeded)
        {
        return false;
        }
    bool wasAttached = false;
    for (int i = 0; i < this.wires.Count; ++i)
        {
        wasAttached = this.wires[i].IsAttached;
        this.wires[i].Detach ();
        }
    this.wires.Clear ();
    this.addWiresForAnimatorParameterDictionary (
            this.pinToFloatParameters,
            "FP",
            this.pinToFloatParameterId
            );
    this.addWiresForLayerDictionary (
            this.pinToLayerWeights,
            "LW",
            this.pinToLayerId
            );
    this.addWiresForAnimatorParameterDictionary (
            this.pinToIntParameters,
            "IP",
            this.pinToIntParameterId
            );
    this.addWiresForAnimatorParameterDictionary (
            this.pinToBoolParameters,
            "BP",
            this.pinToBoolParameterId
            );
    this.addWiresForAnimatorParameterDictionary (
            this.pinToTriggers,
            "TR",
            this.pinToTriggerId
            );
    if (wasAttached)
        {
        for (int i = 0; i < this.wires.Count; ++i)
            {
            this.wires[i].Attach (this);
            }
        }
    return true;
    }

private List<Wire> wires = new List<Wire> ();

public void addWiresForAnimatorParameterDictionary (StringPairs stringPairs, string pinPrefix, Dictionary<string, int> dictionary)
    {
    dictionary.Clear ();
    Debug.Assert (stringPairs.Keys.Count == stringPairs.Values.Count);
    for (int i = 0; i < stringPairs.Keys.Count; ++i)
        {
        var pin = pinPrefix + i.ToString ();
        dictionary[pin] = Animator.StringToHash (stringPairs.Values[i]);
        var wire = new Wire (pin, this.bus, stringPairs.Keys[i]);
        wires.Add (wire);
        }
    }

public void addWiresForLayerDictionary (StringPairs stringPairs, string pinPrefix, Dictionary<string, int> dictionary)
    {
    dictionary.Clear ();
    Debug.Assert (stringPairs.Keys.Count == stringPairs.Values.Count);
    for (int i = 0; i < stringPairs.Keys.Count; ++i)
        {
        var pin = pinPrefix + i.ToString ();
        dictionary[pin] = this.animator.GetLayerIndex (stringPairs.Values[i]);
        var wire = new Wire (pin, this.bus, stringPairs.Keys[i]);
        wires.Add (wire);
        }
    }

public bool reconnectWiresForAnimatorParameterDictionary (ref int wireIndex, StringPairs stringPairs, string pinPrefix, Dictionary <string, int> dictionary)
    {
    for (int i = 0; i < stringPairs.Keys.Count; ++i)
        {
        var wire = this.wires[wireIndex];
        var pin = pinPrefix + i.ToString ();
        if (wire.CellPin != pin)
            {
            return true;
            }
        int id = Animator.StringToHash (stringPairs.Values [i]);
        if (dictionary[pin] != id)
            {
            wire.Disconnect ();
            }
        dictionary[pin] = id;
        wire.Connect (this.bus, stringPairs.Keys[i]);
        ++wireIndex;
        }
    return false;
    }

public bool reconnectWiresForLayerDictionary (ref int wireIndex, StringPairs stringPairs, string pinPrefix, Dictionary <string, int> dictionary)
    {
    for (int i = 0; i < stringPairs.Keys.Count; ++i)
        {
        var wire = this.wires[wireIndex];
        var pin = pinPrefix + i.ToString ();
        if (wire.CellPin != pin)
            {
            return true;
            }
        int id = this.animator.GetLayerIndex (stringPairs.Values [i]);
        if (dictionary[pin] != id)
            {
            wire.Disconnect ();
            }
        dictionary[pin] = id;
        wire.Connect (this.bus, stringPairs.Keys[i]);
        ++wireIndex;
        }
    return false;
    }

}


}
