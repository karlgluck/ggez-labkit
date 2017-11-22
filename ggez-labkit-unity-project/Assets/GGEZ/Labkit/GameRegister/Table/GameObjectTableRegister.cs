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
using Object = UnityEngine.Object;

namespace GGEZ
{



//----------------------------------------------------------------------
//----------------------------------------------------------------------
[
Serializable,
CreateAssetMenu (fileName = "New GameObject Table Register.asset", menuName="GGEZ/Game Register/Table/GameObject Table Register")
]
public class GameObjectTableRegister : GameTableRegister, ISerializationCallbackReceiver
{

// The value used when a listener registers for a nonexistent key
[SerializeField] private GameObject defaultValue;


private Dictionary<string, GameObject> table = new Dictionary<string, GameObject>();


#region Serialization

// All the values that the table should start with
[SerializeField] private List<GameObjectTableRegisterKeyValuePair> initialTable = new List<GameObjectTableRegisterKeyValuePair> ();

// Values that the table has at runtime only. Entries are cleared out when
// listeners unregister.
[SerializeField] private List<GameObjectTableRegisterKeyValuePair> runtimeTable = new List<GameObjectTableRegisterKeyValuePair> ();

void ISerializationCallbackReceiver.OnBeforeSerialize ()
    {
    var runtimeKeys = new HashSet<string> (this.listenersTable.Keys);
    runtimeKeys.IntersectWith (this.table.Keys);
    this.runtimeTable.Clear ();
    this.runtimeTable.Capacity = Mathf.Max (this.runtimeTable.Capacity, runtimeKeys.Count);
    foreach (var key in runtimeKeys)
        {
        this.runtimeTable.Add (new GameObjectTableRegisterKeyValuePair (key, this.table[key]));
        }
    }

void ISerializationCallbackReceiver.OnAfterDeserialize ()
    {
#if UNITY_EDITOR

    // Clean up the runtime keys so that the editor never creates
    // elements or tries to contain values for keys without listeners.
    HashSet<string> runtimeTableKeys = new HashSet<string> ();
    for (int i = this.runtimeTable.Count - 1; i >= 0; --i)
        {
        var kvp = this.runtimeTable[i];
        if (string.IsNullOrEmpty(kvp.Name)
                || runtimeTableKeys.Contains (kvp.Name)
                || !this.listenersTable.ContainsKey (kvp.Name))
            {
            this.runtimeTable.RemoveAt (i);
            continue;
            }
        runtimeTableKeys.Add (kvp.Name);
        }

    // Make sure keys with listeners aren't dropped from the runtime table
    var deletedKeys = new HashSet<string> (this.listenersTable.Keys);
    deletedKeys.ExceptWith (runtimeTableKeys);
    foreach (var key in deletedKeys)
        {
        this.runtimeTable.Add (new GameObjectTableRegisterKeyValuePair (key, this[key]));
        }

    // Clean up initial keys so that there are no duplicates
    // or null entries. These happen when adding elements.
    HashSet<string> initialTableKeys = new HashSet<string> ();
    foreach (var kvp in this.initialTable)
        {
        if (string.IsNullOrEmpty(kvp.Name) || initialTableKeys.Contains (kvp.Name))
            {
            kvp.Name = Guid.NewGuid ().ToString ();
            }
        initialTableKeys.Add (kvp.Name);
        }

    // Dirty keys are runtime keys with changed values. We only
    // need to do this in the Editor because the app will change
    // values through the overloaded accessor.
    this.dirtyKeys.Clear ();
    foreach (var kvp in this.runtimeTable)
        {
        GameObject value;
        if (this.table.TryGetValue (kvp.Name, out value) && !value.Equals (kvp.Value))
            {
            this.dirtyKeys.Add (kvp.Name);
            }
        }

#endif

    this.table.Clear ();
    foreach (var kvp in this.initialTable)
        {
        this.table.Add (kvp.Name, kvp.Value);
        }
    foreach (var kvp in this.runtimeTable)
        {
        this.table[kvp.Name] = kvp.Value;
        }
    }

#if UNITY_EDITOR
private List<string> dirtyKeys = new List<string> ();

void Reset ()
    {
    this.table = new Dictionary<string, GameObject> ();
    this.initialTable = new List<GameObjectTableRegisterKeyValuePair> ();
    this.runtimeTable = new List<GameObjectTableRegisterKeyValuePair> ();
    this.dirtyKeys.Clear ();
    this.listenersTable.Clear ();
    }

void OnValidate ()
    {
    if (Application.isPlaying)
        {
        foreach (var key in this.dirtyKeys)
            {
            List<GameObjectTableRegisterListener> listeners;
            if (this.listenersTable.TryGetValue (key, out listeners))
                {
                var value = this[key];
                for (int i = listeners.Count - 1; i >= 0; --i)
                    {
                    listeners[i].OnDidChange (value);
                    }
                }
            }
        this.dirtyKeys.Clear ();
        }
    else
        {
        this.runtimeTable.Clear ();
        this.table.Clear ();
        foreach (var kvp in this.initialTable)
            {
            this.table.Add (kvp.Name, kvp.Value);
            }
        }
    }
#endif


#endregion


public GameObject this[string key]
    {
    get
        {
        GameObject value;
        if (this.table.TryGetValue (key, out value))
            {
            return value;
            }
        return this.defaultValue;
        }
    set
        {
        GameObject currentValue;
        if (this.table.TryGetValue (key, out currentValue) && currentValue.Equals (value))
            {
            return;
            }
        this.table[key] = value;
        List<GameObjectTableRegisterListener> listeners;
        if (this.listenersTable.TryGetValue (key, out listeners))
            {
            for (int i = listeners.Count - 1; i >= 0; --i)
                {
                listeners[i].OnDidChange (value);
                }
            }
        }
    }


public bool Remove (string key)
    {
    List<GameObjectTableRegisterListener> listeners;
    if (!this.listenersTable.TryGetValue (key, out listeners))
        {
        return this.table.Remove (key);
        }
    this[key] = this.defaultValue;
    return true;
    }




public Dictionary<string, GameObject>.Enumerator GetEnumerator ()
    {
    return this.table.GetEnumerator ();
    }




public Dictionary<string, GameObject>.KeyCollection Keys
    {
    get
        {
        return this.table.Keys;
        }
    }




public ICollection<string> KeysWithListeners
    {
    get
        {
        return this.listenersTable.Keys;
        }
    }




public Dictionary<string, GameObject>.ValueCollection Values
    {
    get
        {
        return this.table.Values;
        }
    }




private Dictionary<string,List<GameObjectTableRegisterListener>> listenersTable = new Dictionary<string,List<GameObjectTableRegisterListener>>();





public void RegisterListener (string key, GameObjectTableRegisterListener listener)
    {
    if (key == null)
        {
        throw new ArgumentNullException ("key");
        }
    if (listener == null)
        {
        throw new ArgumentNullException ("listener");
        }
    List<GameObjectTableRegisterListener> listeners;
    if (!this.listenersTable.TryGetValue (key, out listeners))
        {
        listeners = new List<GameObjectTableRegisterListener> ();
        this.listenersTable.Add (key, listeners);
        }
    listeners.Add (listener);

    GameObject value;
    if (!this.table.TryGetValue (key, out value))
        {
        value = this.defaultValue;
        this.table.Add (key, value);
        }

    listener.OnDidChange (value);
    }




public void UnregisterListener (string key, GameObjectTableRegisterListener listener)
    {
    if (key == null)
        {
        throw new ArgumentNullException ("key");
        }
    if (listener == null)
        {
        throw new ArgumentNullException ("listener");
        }
    List<GameObjectTableRegisterListener> listeners;
    if (this.listenersTable.TryGetValue (key, out listeners))
        {
        listeners.Remove (listener);
        if (listeners.Count == 0)
            {
            this.listenersTable.Remove (key);
            }
        }
    }




}








//----------------------------------------------------------------------
// Holds entries saved into the serialized form of the table
//----------------------------------------------------------------------
[System.Serializable]
public class GameObjectTableRegisterKeyValuePair
{

// By using the value "Name" for the key, Unity's default inspector will
// render the text more nicely.
[Delayed] public string Name;
public GameObject Value;

public GameObjectTableRegisterKeyValuePair ()
    {
    }

public GameObjectTableRegisterKeyValuePair (string key, GameObject value)
    {
    this.Name = key;
    this.Value = value;
    }
}







}
