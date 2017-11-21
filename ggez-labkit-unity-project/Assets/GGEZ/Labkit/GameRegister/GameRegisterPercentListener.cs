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
public class UnityEventForGameRegisterPercentListener : UnityEvent<float>
{
}



//----------------------------------------------------------------------
//----------------------------------------------------------------------
[
AddComponentMenu ("GGEZ/Game Register Listener/percent (float)")
]
public class GameRegisterPercentListener : MonoBehaviour
{




[SerializeField] private GameRegisterPercent percentRegister;
[SerializeField] private UnityEventForGameRegisterPercentListener didChange;



// Provided for convenience. If you only need to access the value in
// the register and don't need change notifications, just create the
// reference directly.
public float Value
    {
    get
        {
        return this.percentRegister.Value;
        }
    set
        {
        this.percentRegister.Value = value;
        }
    }




void OnEnable ()
    {
    Debug.Log ("listener.OnEnable");
    if (this.percentRegister != null)
        {
        this.percentRegister.RegisterListener (this);
        }
#if UNITY_EDITOR
    this.registeredGameRegisterIn = this.percentRegister;
#endif
    }




void OnDisable ()
    {
    if (this.percentRegister != null)
        {
        this.percentRegister.UnregisterListener (this);
        }
#if UNITY_EDITOR
    this.registeredGameRegisterIn = null;
#endif
    }




public void OnDidChange (float newValue)
    {
    this.didChange.Invoke (newValue);
    }




//----------------------------------------------------------------------
// Handle the Unity Editor changing gameEpercentRegister in the inspector
//----------------------------------------------------------------------
#if UNITY_EDITOR
#region Editor Runtime
[Header ("Editor Runtime")]
private GameRegisterPercent registeredGameRegisterIn;




void OnValidate ()
    {
    if (this.registeredGameRegisterIn != null
            && !object.ReferenceEquals (this.registeredGameRegisterIn, this.percentRegister))
        {
        this.registeredGameRegisterIn.UnregisterListener (this);
        this.registeredGameRegisterIn = this.percentRegister;
        this.percentRegister.RegisterListener (this);
        }
    }



#endregion
#endif






}


}
