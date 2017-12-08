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
AddComponentMenu ("GGEZ/Omnibus/Modules/RectTransform/Anchor Faces Anchor (Module)"),
RequireComponent (typeof (RectTransform))
]
public class AnchorFacesAnchorModule : Cell
{

#region Programming Interface

public Bus PositionBus
    {
    get { return this.positionBus; }
    set
        {
        this.positionBus = value;
        this.positionWire.Connect (this.positionBus, this.positionPin);
        }
    }

public string PositionPin
    {
    get { return this.positionPin; }
    set
        {
        this.positionPin = value;
        this.positionWire.Connect (this.positionBus, this.positionPin);
        }
    }
    
public Bus TargetBus
    {
    get { return this.targetBus; }
    set
        {
        this.targetBus = value;
        this.targetWire.Connect (this.targetBus, this.targetPin);
        }
    }

public string TargetPin
    {
    get { return this.targetPin; }
    set
        {
        this.targetPin = value;
        this.targetWire.Connect (this.targetBus, this.targetPin);
        }
    }

#endregion

const string POSITION = "POS";
const string TARGET = "TGT";


[Header ("position:" + POSITION + " (Vector2)")]
[SerializeField] private Bus positionBus;
[SerializeField] private string positionPin;

[Header ("target:" + TARGET + " (Vector2)")]
[SerializeField] private Bus targetBus;
[SerializeField] private string targetPin;

private RectTransform rectTransform;
private Vector2 position;
private Vector2 target;

public override void OnDidSignal (string pin, object value)
    {

#if UNITY_EDITOR
    if (value == null || !typeof(Vector2).IsAssignableFrom (value.GetType ()))
        {
        throw new System.InvalidCastException ("`value` should be " + typeof(Vector2).Name);
        }
#endif

    switch (pin)
        {

        case POSITION:
            this.position = (Vector2)value;
            break;

        case TARGET:
            this.target = (Vector2)value;
            break;

#if UNITY_EDITOR
        default:
            Debug.LogError ("signal on invalid pin: " + pin);
            break;
#endif

        }

    if (this.positionWire.IsConnected && this.targetWire.IsConnected)
        {
        this.rectTransform.anchorMax = this.position;
        this.rectTransform.anchorMin = this.position;
        this.rectTransform.rotation = Quaternion.Euler (0f, 0f, (this.target - this.position).ToDirection ());
        }

    }

public override void Route (string port, Bus bus)
    {
    switch (port)
        {
        case "position": this.PositionBus  = bus; break;
        case "target":   this.TargetBus = bus; break;
        default:
            {
            this.PositionBus  = bus;
            this.TargetBus = bus;
            break;
            }
        }
    }

private Wire positionWire = new Wire (POSITION);
private Wire targetWire = new Wire (TARGET);

void Awake ()
    {
    this.rectTransform = this.GetComponent <RectTransform> ();
    }

void OnEnable ()
    {
    this.positionWire.Attach (this, this.positionBus, this.positionPin);
    this.targetWire.Attach (this, this.targetBus, this.targetPin);
    }

void OnDisable ()
    {
    this.positionWire.Detach ();
    this.targetWire.Detach ();
    }

void OnValidate ()
    {
    this.positionWire.Connect (this.positionBus, this.positionPin);
    this.targetWire.Connect (this.targetBus, this.targetPin);
    }

}

}

}





