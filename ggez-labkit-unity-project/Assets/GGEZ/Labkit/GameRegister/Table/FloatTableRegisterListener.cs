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
using System.Collections.Generic;
using UnityEngine.Events;

namespace GGEZ
{




//----------------------------------------------------------------------
//----------------------------------------------------------------------
[Serializable]
public class UnityEventForFloatTableRegisterListener : UnityEvent<float>
{
}



//----------------------------------------------------------------------
// Listens for changes to TableRegister[key]. When this component is
// enabled or the entry changes, all callbacks registered to the Unity
// Event didChange will be invoked with the new value.
//----------------------------------------------------------------------
[
AddComponentMenu ("GGEZ/Game Register/Table/float Table Register Listener")
]
public class FloatTableRegisterListener : MonoBehaviour
{

[SerializeField, Delayed] private string key;
[SerializeField] private FloatTableRegister floatTableRegister;
[SerializeField] private UnityEventForFloatTableRegisterListener didChange;



// Value & Table are for convenience. If you only need to access
// the register and don't need change notifications, use
// FloatTableRegister as a serialized member field in your class.

public float Value
    {
    get
        {
        return this.floatTableRegister[this.key];
        }
    set
        {
        this.floatTableRegister[this.key] = value;
        }
    }

public FloatTableRegister Table
    {
    get
        {
        return this.floatTableRegister;
        }
    }



void OnEnable ()
    {
    if (this.floatTableRegister != null && this.key != null)
        {
        this.floatTableRegister.RegisterListener (this.key, this);
        }
#if UNITY_EDITOR
    this.previousKey = this.key;
    this.previousTableRegister = this.floatTableRegister;
    this.hasBeenEnabled = true;
#endif
    }




void OnDisable ()
    {
    if (this.floatTableRegister != null && this.key != null)
        {
        this.floatTableRegister.UnregisterListener (this.key, this);
        }
#if UNITY_EDITOR
    this.hasBeenEnabled = false;
    this.previousKey = null;
    this.previousTableRegister = null;
#endif
    }




public void OnDidChange (float newValue)
    {
    this.didChange.Invoke (newValue);
    }




//----------------------------------------------------------------------
// Handle the Editor changing values in the inspector
//----------------------------------------------------------------------
#if UNITY_EDITOR
#region Editor Runtime
private bool hasBeenEnabled;
private string previousKey;
private FloatTableRegister previousTableRegister;


void OnValidate ()
    {
    if (!this.hasBeenEnabled
            || (object.Equals (this.key, this.previousKey)
                    && object.ReferenceEquals (this.previousTableRegister, this.floatTableRegister)))
        {
        return;
        }
    if (this.previousTableRegister != null && this.previousKey != null)
        {
        this.previousTableRegister.UnregisterListener (this.previousKey, this);
        }
    this.previousKey = this.key;
    this.previousTableRegister = this.floatTableRegister;
    if (this.floatTableRegister != null && this.key != null)
        {
        this.floatTableRegister.RegisterListener (this.key, this);
        }
    }

#endregion
#endif






}


}
