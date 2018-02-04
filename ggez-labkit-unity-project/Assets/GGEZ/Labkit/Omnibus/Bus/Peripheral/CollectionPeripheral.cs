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
using System.Collections;

namespace GGEZ.Omnibus
{

[
RequireComponent (typeof (Bus))
]
public sealed class CollectionPeripheral : MonoBehaviour, IPeripheral
{
    
#region Programming Interface

public void Add (object obj)
    {
    this.collection.Add (obj);
#if UNITY_EDITOR
    if (this.bus == null)
        {
        Debug.Log ("Peripheral has no bus. Open the asset in the editor to fix this.", this);
        }
#else
    if (this.bus == null)
        {
        this.bus = (Bus)this.GetComponent (typeof (Bus));
        }
#endif
    this.bus.SignalObject (this.addSignalPin, obj);
    }

public void Remove (object obj)
    {
    this.collection.Remove (obj);
#if UNITY_EDITOR
    if (this.bus == null)
        {
        Debug.Log ("Peripheral has no bus. Open the asset in the editor to fix this.", this);
        }
#else
    if (this.bus == null)
        {
        this.bus = (Bus)this.GetComponent (typeof (Bus));
        }
#endif
    this.bus.SignalObject (this.removeSignalPin, obj);
    }

#endregion

[Header ("Settings")]
[SerializeField] private string addSignalPin;
[SerializeField] private string removeSignalPin;

private Bus bus;
private ArrayList collection = new ArrayList ();


public void OnDidConnect (Wire wire)
    {
    if (wire == null)
        {
        throw new ArgumentNullException ("wire");
        }
    if (!wire.BusPin.Equals (this.addSignalPin))
        {
        return;
        }
    for (int i = this.collection.Count - 1; i >= 0; --i)
        {
        wire.Signal (this.collection[i]);
        }
    }

public void OnWillDisconnect (Wire wire)
    {
    if (wire == null)
        {
        throw new ArgumentNullException ("wire");
        }
    if (!wire.BusPin.Equals (this.removeSignalPin))
        {
        return;
        }
    for (int i = this.collection.Count - 1; i >= 0; --i)
        {
        wire.Signal (this.collection[i]);
        }
    }

#if UNITY_EDITOR

void Awake ()
    {
    
    // Do not put anything here. This is never called for asset busses.

    }
    
void Start ()
    {
    
    // Do not put anything here. This is never called for asset busses.

    }

void OnEnable ()
    {
    
    // Do not put anything here. This is never called for asset busses.

    }

void OnDisable ()
    {
    
    // Do not put anything here. This is never called for asset busses.

    }

void OnValidate ()
    {
    this.bus = (Bus)this.GetComponent (typeof (Bus));
    }

#endif

}

}