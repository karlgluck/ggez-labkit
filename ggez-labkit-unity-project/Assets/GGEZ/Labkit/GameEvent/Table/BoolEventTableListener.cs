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
class UnityEventForBoolEventTableListener : UnityEvent<bool>
{
}

[
AddComponentMenu ("GGEZ/Game Event/bool Event Table Listener")
]
public class BoolEventTableListener : MonoBehaviour
{

#region Key
[SerializeField, Delayed]
private string key;
public string Key
    {
    get
        {
        return this.key;
        }
    set
        {
        if (object.Equals (this.key, value))
            {
            return;
            }
        if (this.hasBeenEnabled && !string.IsNullOrEmpty (this.key) && this.boolEventTable != null)
            {
            this.boolEventTable.UnregisterListener (this.key, this);
            }
        this.key = value;
#if UNITY_EDITOR
        if (this.hasBeenEnabled)
            {
            this.previousKey = value;
            }
#endif
        if (this.hasBeenEnabled && !string.IsNullOrEmpty (this.key) && this.boolEventTable != null)
            {
            this.boolEventTable.RegisterListener (this.key, this);
            }
        }
    }
#endregion



#region Event Table
[SerializeField] private BoolEventTable boolEventTable;
[SerializeField] private UnityEventForBoolEventTableListener didTrigger = new UnityEventForBoolEventTableListener ();
#endregion


public BoolEventTable BoolEventTable
    {
    get
        {
        return this.boolEventTable;
        }
    set
        {
        if (object.ReferenceEquals (this.boolEventTable, value))
            {
            return;
            }
        if (this.hasBeenEnabled && !string.IsNullOrEmpty (this.key) && this.boolEventTable != null)
            {
            this.boolEventTable.UnregisterListener (this.key, this);
            }
        this.boolEventTable = value;
#if UNITY_EDITOR
        if (this.hasBeenEnabled)
            {
            this.previousBoolEventTable = value;
            }
#endif
        if (this.hasBeenEnabled && !string.IsNullOrEmpty (this.key) && this.boolEventTable != null)
            {
            this.boolEventTable.RegisterListener (this.key, this);
            }
        }
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





private bool hasBeenEnabled;

void OnEnable ()
    {
    if (this.boolEventTable != null && this.key != null)
        {
        this.boolEventTable.RegisterListener (this.Key, this);
        }
    this.hasBeenEnabled = true;
#if UNITY_EDITOR
    this.previousKey = this.key;
    this.previousBoolEventTable = this.boolEventTable;
#endif
    }



void OnDisable ()
    {
    if (this.previousBoolEventTable != null && this.key != null)
        {
        this.previousBoolEventTable.UnregisterListener (this.key, this);
        }
    this.hasBeenEnabled = false;
#if UNITY_EDITOR
    this.previousKey = null;
    this.previousBoolEventTable = null;
#endif
    }



public void OnDidTrigger (bool value)
    {
    this.didTrigger.Invoke (value);
    }




//----------------------------------------------------------------------
// Handle the Unity Editor changing values in the inspector
//----------------------------------------------------------------------
#if UNITY_EDITOR
private string previousKey;
private BoolEventTable previousBoolEventTable;


void OnValidate ()
    {
    if (!this.hasBeenEnabled
            || (object.Equals (this.key, this.previousKey)
                    && object.ReferenceEquals (this.previousBoolEventTable, this.boolEventTable)))
        {
        return;
        }
    if (this.previousBoolEventTable != null && this.previousKey != null)
        {
        this.previousBoolEventTable.UnregisterListener (this.previousKey, this);
        }
    this.previousKey = this.key;
    this.previousBoolEventTable = this.boolEventTable;
    if (this.boolEventTable != null && this.key != null)
        {
        this.boolEventTable.RegisterListener (this.key, this);
        }
    }


#endif




}


}
