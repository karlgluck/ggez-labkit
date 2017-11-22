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
public class UnityEventForBoolRegisterTableListener : UnityEvent<bool>
{
}



//----------------------------------------------------------------------
//----------------------------------------------------------------------
[
AddComponentMenu ("GGEZ/Registers/Table/bool Table Listener")
]
public class BoolTableRegisterListener : MonoBehaviour
{

[SerializeField] private string key;
[SerializeField] private BoolTableRegister boolRegisterTable;
[SerializeField] private UnityEventForBoolRegisterTableListener didChange;



// Provided for convenience. If you only need to access the value in
// the register and don't need change notifications, just create the
// reference directly.
public bool this[string key]
    {
    get
        {
        return this.boolRegisterTable[key];
        }
    set
        {
        this.boolRegisterTable[key] = value;
        }
    }




void OnEnable ()
    {
    if (this.boolRegisterTable != null)
        {
        this.boolRegisterTable.RegisterListener (this.key, this);
        }
#if UNITY_EDITOR
    this.registeredWith = this.boolRegisterTable;
#endif
    }




void OnDisable ()
    {
    if (this.boolRegisterTable != null)
        {
        this.boolRegisterTable.UnregisterListener (this.key, this);
        }
#if UNITY_EDITOR
    this.registeredWith = null;
#endif
    }




public void OnDidChange (bool newValue)
    {
    this.didChange.Invoke (newValue);
    }




//----------------------------------------------------------------------
// Handle the Unity Editor changing gameEboolRegister in the inspector
//----------------------------------------------------------------------
#if UNITY_EDITOR
#region Editor Runtime
[Header ("Editor Runtime")]
private string registeredKey;
private BoolTableRegister registeredWith;




void OnValidate ()
    {
    if (this.registeredWith != null
            && (this.registeredKey.Equals (this.key) ||
                    !object.ReferenceEquals (this.registeredWith, this.boolRegisterTable)))
        {
        this.registeredWith.UnregisterListener (this.registeredKey, this);
        this.registeredKey = this.key;
        this.registeredWith = this.boolRegisterTable;
        this.boolRegisterTable.RegisterListener (this.registeredKey, this);
        }
    }



#endregion
#endif






}


}
