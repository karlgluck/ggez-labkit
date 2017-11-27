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
using System.Collections.Generic;
using UnityEngine.Events;

namespace GGEZ
{


//----------------------------------------------------------------------
// Helper class for binding events through prefabs, scenes and assets.
// UnityEvent fields are bound to GameEvent.Trigger on a named asset.
// GameEventListeners register themselves to a named asset in order
// to be notified when an event occurs. The GameEventListener then has
// its own UnityEvent field that dispatches the event.
//----------------------------------------------------------------------
[CreateAssetMenu (fileName = "New Game Event.asset", menuName="GGEZ/Game Event/Single Event")]
public class GameEvent : BaseGameEvent
{


#region Runtime
private List<GameEventListener> listeners = new List<GameEventListener>();
#endregion


public IList<GameEventListener> Listeners
    {
    get
        {
        return this.listeners.AsReadOnly ();
        }
    }



public void RegisterListener (GameEventListener listener)
    {
    if (listener == null)
        {
        throw new ArgumentNullException ("listener");
        }
    this.listeners.Add (listener);
    }




public void UnregisterListener (GameEventListener listener)
    {
    if (listener == null)
        {
        throw new ArgumentNullException ("listener");
        }
    this.listeners.Remove (listener);
    }




public void Trigger ()
    {
    for (int i = this.listeners.Count - 1; i >= 0; --i)
        {
        this.listeners[i].OnDidTrigger ();
        }
    }




}

}
