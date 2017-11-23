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
[Serializable]
public class UnityEventForBoolRegisterListener : UnityEvent<bool>
{
}



//----------------------------------------------------------------------
//----------------------------------------------------------------------
[
AddComponentMenu ("GGEZ/Game Register Listener/bool Register Listener")
]
public class BoolRegisterListener : MonoBehaviour
{



[SerializeField] private BoolRegister boolRegister;
[SerializeField] private UnityEventForBoolRegisterListener didChange;



// Provided for convenience. If you only need to access the value in
// the register and don't need change notifications, use
// BoolRegister as a serialized member field in your class.
public bool Value
    {
    get
        {
        return this.boolRegister.Value;
        }
    set
        {
        this.boolRegister.Value = value;
        }
    }




void OnEnable ()
    {
    if (this.boolRegister != null)
        {
        this.boolRegister.RegisterListener (this);
        }
#if UNITY_EDITOR
    this.hasBeenEnabled = true;
    this.previousRegister = this.boolRegister;
#endif
    }




void OnDisable ()
    {
    if (this.boolRegister != null)
        {
        this.boolRegister.UnregisterListener (this);
        }
#if UNITY_EDITOR
    this.hasBeenEnabled = false;
    this.previousRegister = null;
#endif
    }




public void OnDidChange (bool newValue)
    {
    this.didChange.Invoke (newValue);
    }




//----------------------------------------------------------------------
// Handle the Unity Editor changing boolRegister in the inspector
//----------------------------------------------------------------------
#if UNITY_EDITOR
#region Editor Runtime
[Header ("Editor Runtime")]
private bool hasBeenEnabled;
private BoolRegister previousRegister;




void OnValidate ()
    {
    if (this.hasBeenEnabled
            && !object.ReferenceEquals (this.previousRegister, this.boolRegister))
        {
        if (this.previousRegister != null)
            {
            this.previousRegister.UnregisterListener (this);
            }
        this.previousRegister = this.boolRegister;
        if (this.boolRegister != null)
            {
            this.boolRegister.RegisterListener (this);
            }
        }
    }



#endregion
#endif






}


}
