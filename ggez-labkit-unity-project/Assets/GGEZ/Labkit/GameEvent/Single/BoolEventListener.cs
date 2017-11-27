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

[Serializable]
public class UnityEventForBoolEvent : UnityEvent<bool>
{
}


//----------------------------------------------------------------------
//----------------------------------------------------------------------
[
AddComponentMenu ("GGEZ/Game Event/bool Event Listener")
]
public class BoolEventListener : MonoBehaviour
{



#region Serialized
[Header ("Serialized")]
[SerializeField] private BoolEvent boolEvent;
[SerializeField] private UnityEventForBoolEvent didTrigger = new UnityEventForBoolEvent ();
#endregion


public BoolEvent BoolEvent
    {
    get
        {
        return this.boolEvent;
        }
    set
        {
        if (object.ReferenceEquals (this.boolEvent, value))
            {
            return;
            }
        if (this.hasBeenEnabled && this.boolEvent != null)
            {
            this.boolEvent.UnregisterListener (this);
            }
        this.boolEvent = value;
#if UNITY_EDITOR
        if (this.hasBeenEnabled)
            {
            this.previousBoolEvent = value;
            }
#endif
        if (this.hasBeenEnabled && this.boolEvent != null)
            {
            this.boolEvent.RegisterListener (this);
            }
        }
    }




private bool hasBeenEnabled;

void OnEnable ()
    {
    if (this.boolEvent != null)
        {
        this.boolEvent.RegisterListener (this);
        }
#if UNITY_EDITOR
    this.hasBeenEnabled = true;
    this.previousBoolEvent = this.boolEvent;
#endif
    }




void OnDisable ()
    {
    if (this.boolEvent != null)
        {
        this.boolEvent.UnregisterListener (this);
        }
#if UNITY_EDITOR
    this.hasBeenEnabled = false;
    this.previousBoolEvent = null;
#endif
    }

public void AddDidTriggerCallback (UnityAction<bool> action)
    {
    this.didTrigger.AddListener (action);
    }

public void RemoveDidTriggerCallback (UnityAction<bool> action)
    {
    this.didTrigger.RemoveListener (action);
    }

public void RemoveAllDidTriggerCallbacks (UnityAction<bool> action)
    {
    this.didTrigger.RemoveListener (action);
    }




public void OnDidTrigger (bool value)
    {
    this.didTrigger.Invoke (value);
    }




//----------------------------------------------------------------------
// Handle the Unity Editor changing boolEvent from the inspector
//----------------------------------------------------------------------
#if UNITY_EDITOR
private BoolEvent previousBoolEvent;


void OnValidate ()
    {
    if (this.hasBeenEnabled
            && !object.ReferenceEquals (this.boolEvent, this.previousBoolEvent))
        {
        if (this.previousBoolEvent != null)
            {
            this.previousBoolEvent.UnregisterListener (this);
            }
        this.previousBoolEvent = this.boolEvent;
        if (this.boolEvent != null)
            {
            this.boolEvent.RegisterListener (this);
            }
        }
    }


#endif






}


}
