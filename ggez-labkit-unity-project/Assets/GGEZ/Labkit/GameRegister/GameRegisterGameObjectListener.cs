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
public class UnityEventForGameRegisterGameObjectListener : UnityEvent<GameObject>
{
}



//----------------------------------------------------------------------
//----------------------------------------------------------------------
[
AddComponentMenu ("GGEZ/Game Register Listener/GameObject")
]
public class GameRegisterGameObjectListener : MonoBehaviour
{


[SerializeField] private GameRegisterGameObject gameObjectRegister;
[SerializeField] private UnityEventForGameRegisterGameObjectListener didChange;



// Provided for convenience. If you only need to access the value in
// the register and don't need change notifications, just create the
// reference directly.
public GameObject Value
    {
    get
        {
        return this.gameObjectRegister.Value;
        }
    set
        {
        this.gameObjectRegister.Value = value;
        }
    }




void OnEnable ()
    {
    Debug.Log ("listener.OnEnable");
    if (this.gameObjectRegister != null)
        {
        this.gameObjectRegister.RegisterListener (this);
        }
#if UNITY_EDITOR
    this.registeredGameRegisterIn = this.gameObjectRegister;
#endif
    }




void OnDisable ()
    {
    if (this.gameObjectRegister != null)
        {
        this.gameObjectRegister.UnregisterListener (this);
        }
#if UNITY_EDITOR
    this.registeredGameRegisterIn = null;
#endif
    }




public void OnDidChange (GameObject newValue)
    {
    this.didChange.Invoke (newValue);
    }




//----------------------------------------------------------------------
// Handle the Unity Editor changing gameEgameObjectRegister in the inspector
//----------------------------------------------------------------------
#if UNITY_EDITOR
#region Editor Runtime
[Header ("Editor Runtime")]
private GameRegisterGameObject registeredGameRegisterIn;




void OnValidate ()
    {
    if (this.registeredGameRegisterIn != null
            && !object.ReferenceEquals (this.registeredGameRegisterIn, this.gameObjectRegister))
        {
        this.registeredGameRegisterIn.UnregisterListener (this);
        this.registeredGameRegisterIn = this.gameObjectRegister;
        this.gameObjectRegister.RegisterListener (this);
        }
    }



#endregion
#endif






}


}
