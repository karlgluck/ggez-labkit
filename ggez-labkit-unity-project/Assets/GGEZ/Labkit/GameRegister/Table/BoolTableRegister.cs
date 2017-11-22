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
CreateAssetMenu (fileName = "New Bool Register Table.asset", menuName="GGEZ/Game Register/[string] -> bool")
]
public class BoolTableRegister : GameRegisterTable, ISerializationCallbackReceiver
{

// The value returned when a listener registers for a nonexistent key
[SerializeField] private bool initialValue;


private Dictionary<string, bool> table = new Dictionary<string, bool>();


#region Serialization

[SerializeField] private List<BoolRegisterTableKeyValuePair> initialTable = new List<BoolRegisterTableKeyValuePair> ();
[SerializeField] private List<BoolRegisterTableKeyValuePair> runtimeTable;



void ISerializationCallbackReceiver.OnBeforeSerialize ()
    {
    if (this.runtimeTable == null)
        {
        this.runtimeTable = new List<BoolRegisterTableKeyValuePair> (this.table.Count);
        }
    else
        {
        this.runtimeTable.Clear ();
        this.runtimeTable.Capacity = this.table.Count;
        }
    foreach (var kvp in this.table)
        {
        this.runtimeTable.Add (new BoolRegisterTableKeyValuePair (kvp.Key, kvp.Value));
        }
    }

void ISerializationCallbackReceiver.OnAfterDeserialize ()
    {
#if UNITY_EDITOR

    this.dirtyKeys.Clear ();

    // Dirty if in runtime set and the value changed
    HashSet<string> runtimeTableKeys = new HashSet<string> ();
    foreach (var kvp in this.runtimeTable)
        {
        if (string.IsNullOrEmpty(kvp.Name) || runtimeTableKeys.Contains (kvp.Name))
            {
            kvp.Name = Guid.NewGuid ().ToString ();
            }
        runtimeTableKeys.Add (kvp.Name);
        }
    var runtimeKeys = new HashSet<string> (runtimeTableKeys);
    runtimeKeys.IntersectWith (this.listenersTable.Keys);
    foreach (var kvp in this.runtimeTable)
        {
        if (!runtimeKeys.Contains (kvp.Name))
            {
            continue;
            }
        bool value;
        if (!this.table.TryGetValue (kvp.Name, out value) || !value.Equals (kvp.Value))
            {
            this.dirtyKeys.Add (kvp.Name);
            }
        }

    // Dirty if not in initial but not runtime and the value changed
    HashSet<string> initialTableKeys = new HashSet<string> ();
    foreach (var kvp in this.initialTable)
        {
        if (string.IsNullOrEmpty(kvp.Name) || initialTableKeys.Contains (kvp.Name))
            {
            kvp.Name = Guid.NewGuid ().ToString ();
            }
        initialTableKeys.Add (kvp.Name);
        }
    var initialKeys = new HashSet<string> (initialTableKeys);
    initialKeys.IntersectWith (this.listenersTable.Keys);
    initialKeys.ExceptWith (runtimeKeys);
    foreach (var kvp in this.initialTable)
        {
        if (!initialKeys.Contains (kvp.Name))
            {
            continue;
            }
        bool value;
        if (!this.table.TryGetValue (kvp.Name, out value) || !value.Equals (kvp.Value))
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
    this.table = new Dictionary<string, bool> ();
    this.initialTable = new List<BoolRegisterTableKeyValuePair> ();
    this.runtimeTable = new List<BoolRegisterTableKeyValuePair> ();
    this.dirtyKeys.Clear ();
    }

void OnValidate ()
    {
    if (Application.isPlaying)
        {
        Debug.LogFormat ("{0} dirty keys", this.dirtyKeys.Count);
        foreach (var key in this.dirtyKeys)
            {
            List<BoolTableRegisterListener> listeners;
            Debug.LogFormat ("Listeners for {0}...", key);
            if (this.listenersTable.TryGetValue (key, out listeners))
                {
                Debug.LogFormat ("Listeners for {0} = {1}", key, listeners.Count);
                var value = this[key];
                for (int i = listeners.Count - 1; i >= 0; --i)
                    {
                    Debug.LogFormat ("Changed {0}", key);
                    listeners[i].OnDidChange (value);
                    }
                }
            }
        this.dirtyKeys.Clear ();
        }
    else
        {
        this.runtimeTable.Clear ();
        foreach (var kvp in this.initialTable)
            {
            this.runtimeTable.Add (new BoolRegisterTableKeyValuePair (kvp.Name, kvp.Value));
            }
        this.table.Clear ();
        foreach (var kvp in this.initialTable)
            {
            this.table.Add (kvp.Name, kvp.Value);
            }
        }
    }
#endif


#endregion


void Awake ()
    {
    Debug.Log ("AWAKE");
    }

void OnEnabled ()
    {
    Debug.Log ("ENABLED");
    }

public bool this[string key]
    {
    get
        {
        bool value;
        if (this.table.TryGetValue (key, out value))
            {
            return value;
            }
        return this.initialValue;
        }
    set
        {
        bool currentValue;
        if (this.table.TryGetValue (key, out currentValue) && currentValue.Equals (value))
            {
            return;
            }
        this.table[key] = value;
        List<BoolTableRegisterListener> listeners;
        if (this.listenersTable.TryGetValue (key, out listeners))
            {
            for (int i = listeners.Count - 1; i >= 0; --i)
                {
                listeners[i].OnDidChange (value);
                }
            }
        }
    }




private Dictionary<string,List<BoolTableRegisterListener>> listenersTable = new Dictionary<string,List<BoolTableRegisterListener>>();





public void RegisterListener (string key, BoolTableRegisterListener listener)
    {
    if (listener == null)
        {
        throw new ArgumentNullException ("listener");
        }
    List<BoolTableRegisterListener> listeners;
    if (!this.listenersTable.TryGetValue (key, out listeners))
        {
        listeners = new List<BoolTableRegisterListener> ();
        this.listenersTable.Add (key, listeners);
        }
    listeners.Add (listener);

    bool value;
    if (!this.table.TryGetValue (key, out value))
        {
        this.table.Add (key, this.initialValue);
        }

    listener.OnDidChange (value);
    }




public void UnregisterListener (string key, BoolTableRegisterListener listener)
    {
    if (listener == null)
        {
        throw new ArgumentNullException ("listener");
        }
    List<BoolTableRegisterListener> listeners;
    if (this.listenersTable.TryGetValue (key, out listeners))
        {
        listeners.Remove (listener);
        }
    }




}








//----------------------------------------------------------------------
// Holds entries saved into the serialized form of the table
//----------------------------------------------------------------------
[System.Serializable]
public class BoolRegisterTableKeyValuePair
{

// By using the value "Name" for the key, Unity's default inspector will
// render the text more nicely.
[Delayed] public string Name;
public bool Value;

public BoolRegisterTableKeyValuePair ()
    {
    }

public BoolRegisterTableKeyValuePair (string key, bool value)
    {
    this.Name = key;
    this.Value = value;
    }
}







}
