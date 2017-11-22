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
public class UnityEventForGameRegisterIntListener : UnityEvent<int>
{
}



//----------------------------------------------------------------------
//----------------------------------------------------------------------
[
AddComponentMenu ("GGEZ/Game Register Listener/int")
]
public class GameRegisterIntListener : MonoBehaviour
{



[SerializeField] private GameRegisterInt intRegister;
[SerializeField] private UnityEventForGameRegisterIntListener didChange;



// Provided for convenience. If you only need to access the value in
// the register and don't need change notifications, just create the
// reference directly.
public int Value
    {
    get
        {
        return this.intRegister.Value;
        }
    set
        {
        this.intRegister.Value = value;
        }
    }




void OnEnable ()
    {
    Debug.Log ("listener.OnEnable");
    if (this.intRegister != null)
        {
        this.intRegister.RegisterListener (this);
        }
#if UNITY_EDITOR
    this.registeredGameRegisterIn = this.intRegister;
#endif
    }




void OnDisable ()
    {
    if (this.intRegister != null)
        {
        this.intRegister.UnregisterListener (this);
        }
#if UNITY_EDITOR
    this.registeredGameRegisterIn = null;
#endif
    }




public void OnDidChange (int newValue)
    {
    this.didChange.Invoke (newValue);
    }




//----------------------------------------------------------------------
// Handle the Unity Editor changing gameEintRegister in the inspector
//----------------------------------------------------------------------
#if UNITY_EDITOR
#region Editor Runtime
[Header ("Editor Runtime")]
private GameRegisterInt registeredGameRegisterIn;




void OnValidate ()
    {
    if (this.registeredGameRegisterIn != null
            && !object.ReferenceEquals (this.registeredGameRegisterIn, this.intRegister))
        {
        this.registeredGameRegisterIn.UnregisterListener (this);
        this.registeredGameRegisterIn = this.intRegister;
        this.intRegister.RegisterListener (this);
        }
    }



#endregion
#endif






}


}