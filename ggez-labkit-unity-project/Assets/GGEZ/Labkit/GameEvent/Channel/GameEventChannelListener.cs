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



[
AddComponentMenu ("GGEZ/Game Event Channel Listener")
]
public class GameEventChannelListener : MonoBehaviour
{



#region Events
[Header ("Events")]
[SerializeField] private GameEventChannel gameEventChannel;
[SerializeField] private UnityEvent didTrigger;
#endregion

#region Key
[Header ("Key")]
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
        if (this.hasBeenEnabled && !string.IsNullOrEmpty (this.key) && this.gameEventChannel != null)
            {
            this.gameEventChannel.UnregisterListener (this.key, this);
            }
        this.key = value;
#if UNITY_EDITOR
        if (this.hasBeenEnabled)
            {
            this.previousKey = value;
            }
#endif
        if (this.hasBeenEnabled && !string.IsNullOrEmpty (this.key) && this.gameEventChannel != null)
            {
            this.gameEventChannel.RegisterListener (this.key, this);
            }
        }
    }
#endregion


public GameEventChannel GameEventChannel
    {
    get
        {
        return this.gameEventChannel;
        }
    set
        {
        if (object.ReferenceEquals (this.gameEventChannel, value))
            {
            return;
            }
        if (this.hasBeenEnabled && !string.IsNullOrEmpty (this.key) && this.gameEventChannel != null)
            {
            this.gameEventChannel.UnregisterListener (this.key, this);
            }
        this.gameEventChannel = value;
#if UNITY_EDITOR
        if (this.hasBeenEnabled)
            {
            this.previousGameEventChannel = value;
            }
#endif
        if (this.hasBeenEnabled && !string.IsNullOrEmpty (this.key) && this.gameEventChannel != null)
            {
            this.gameEventChannel.RegisterListener (this.key, this);
            }
        }
    }

public void AddDidTriggerCallback (UnityAction action)
    {
    this.didTrigger.AddListener (action);
    }

public void RemoveDidTriggerCallback (UnityAction action)
    {
    this.didTrigger.RemoveListener (action);
    }

public void RemoveAllDidTriggerCallbacks (UnityAction action)
    {
    this.didTrigger.RemoveListener (action);
    }





private bool hasBeenEnabled;

void OnEnable ()
    {
    if (this.gameEventChannel != null && this.key != null)
        {
        this.gameEventChannel.RegisterListener (this.Key, this);
        }
    this.hasBeenEnabled = true;
#if UNITY_EDITOR
    this.previousKey = this.key;
    this.previousGameEventChannel = this.gameEventChannel;
#endif
    }



void OnDisable ()
    {
    if (this.previousGameEventChannel != null && this.key != null)
        {
        this.previousGameEventChannel.UnregisterListener (this.key, this);
        }
    this.hasBeenEnabled = false;
#if UNITY_EDITOR
    this.previousKey = null;
    this.previousGameEventChannel = null;
#endif
    }



public void OnDidTrigger ()
    {
    this.didTrigger.Invoke ();
    }




//----------------------------------------------------------------------
// Handle the Unity Editor changing values in the inspector
//----------------------------------------------------------------------
#if UNITY_EDITOR
private string previousKey;
private GameEventChannel previousGameEventChannel;


void OnValidate ()
    {
    if (!this.hasBeenEnabled
            || (object.Equals (this.key, this.previousKey)
                    && object.ReferenceEquals (this.previousGameEventChannel, this.gameEventChannel)))
        {
        return;
        }
    if (this.previousGameEventChannel != null && this.previousKey != null)
        {
        this.previousGameEventChannel.UnregisterListener (this.previousKey, this);
        }
    this.previousKey = this.key;
    this.previousGameEventChannel = this.gameEventChannel;
    if (this.gameEventChannel != null && this.key != null)
        {
        this.gameEventChannel.RegisterListener (this.key, this);
        }
    }


#endif




}


}