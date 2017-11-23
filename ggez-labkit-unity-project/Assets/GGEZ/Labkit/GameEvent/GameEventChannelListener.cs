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
[SerializeField] private GameEventChannel GameEventChannelIn;
[SerializeField] private UnityEvent UnityEventOut;
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
        if (this.key.Equals (value))
            {
            this.GameEventChannelIn.UnregisterListener (this);
            }
        else
            {
            this.GameEventChannelIn.UnregisterListener (this.key, this);
            }
        this.key = value;
        this.GameEventChannelIn.RegisterListener (this.key, this);
        }
    }
#endregion



void OnValidate ()
    {
    this.Key = this.key;
    }



void OnEnable ()
    {
    this.GameEventChannelIn.RegisterListener (this.Key, this);
    }



void OnDisable ()
    {
    this.GameEventChannelIn.UnregisterListener (this.Key, this);
    }



public void OnDidTrigger ()
    {
    this.UnityEventOut.Invoke ();
    }




}


}