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
//
// Similar to GameEvent. However, listeners register for events that
// match a certain key and the trigger must provide a key.
//----------------------------------------------------------------------
[CreateAssetMenu (fileName = "New Game Event Table.asset", menuName="GGEZ/Game Event/Event Table")]
public class GameEventTable : BaseGameEventTable
{


#region Runtime
private Dictionary<string, List<GameEventTableListener>> listenersTable = new Dictionary<string, List<GameEventTableListener>> ();
#endregion




public void RegisterListener (string key, GameEventTableListener listener)
    {
    if (key == null)
        {
        throw new ArgumentNullException ("key");
        }
    if (listener == null)
        {
        throw new ArgumentNullException ("listener");
        }
    List<GameEventTableListener> listenersForKey;
    if (!this.listenersTable.TryGetValue (key, out listenersForKey))
        {
        listenersForKey = new List<GameEventTableListener>();
        this.listenersTable.Add (key, listenersForKey);
        }
    listenersForKey.Add (listener);
    }




public void UnregisterListener (string key, GameEventTableListener listener)
    {
    if (key == null)
        {
        throw new ArgumentNullException ("key");
        }
    if (listener == null)
        {
        throw new ArgumentNullException ("listener");
        }
    List<GameEventTableListener> listenersForKey;
    if (!this.listenersTable.TryGetValue (key, out listenersForKey))
        {
        throw new InvalidOperationException ("key does not exist");
        }
    listenersForKey.Remove (listener);
    if (listenersForKey.Count == 0)
        {
        this.listenersTable.Remove (key);
        }
    }




public void Trigger (string key)
    {
    if (key == null)
        {
        throw new ArgumentNullException ("key");
        }
    List<GameEventTableListener> listenersForKey;
    if (!this.listenersTable.TryGetValue (key, out listenersForKey))
        {
        return;
        }
    for (int i = listenersForKey.Count - 1; i >= 0; --i)
        {
        listenersForKey[i].OnDidTrigger ();
        }
    }




public ICollection<string> KeysWithListeners
    {
    get
        {
        return this.listenersTable.Keys;
        }
    }




}

}