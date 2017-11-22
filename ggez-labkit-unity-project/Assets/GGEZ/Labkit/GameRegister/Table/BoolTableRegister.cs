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
public class BoolTableRegister : GameTableRegister, ISerializationCallbackReceiver
{

// The value returned when a listener registers for a nonexistent key
[SerializeField] private bool defaultValue;


private Dictionary<string, bool> table = new Dictionary<string, bool>();


#region Serialization

[SerializeField] private List<BoolRegisterTableKeyValuePair> initialTable = new List<BoolRegisterTableKeyValuePair> ();
[SerializeField] private List<BoolRegisterTableKeyValuePair> runtimeTable = new List<BoolRegisterTableKeyValuePair> ();



void ISerializationCallbackReceiver.OnBeforeSerialize ()
    {
    var runtimeKeys = new HashSet<string> (this.listenersTable.Keys);
    runtimeKeys.IntersectWith (this.table.Keys);
    this.runtimeTable.Clear ();
    this.runtimeTable.Capacity = Mathf.Max (this.runtimeTable.Capacity, runtimeKeys.Count);
    foreach (var key in runtimeKeys)
        {
        this.runtimeTable.Add (new BoolRegisterTableKeyValuePair (key, this.table[key]));
        }
    }

void ISerializationCallbackReceiver.OnAfterDeserialize ()
    {
#if UNITY_EDITOR

    // Clean up the initial table and runtime table keys so that
    // there are no duplicates or blank/null entries.
    HashSet<string> runtimeTableKeys = new HashSet<string> ();
    foreach (var kvp in this.runtimeTable)
        {
        if (string.IsNullOrEmpty(kvp.Name) || runtimeTableKeys.Contains (kvp.Name))
            {
            kvp.Name = Guid.NewGuid ().ToString ();
            }
        runtimeTableKeys.Add (kvp.Name);
        }
    HashSet<string> initialTableKeys = new HashSet<string> ();
    foreach (var kvp in this.initialTable)
        {
        if (string.IsNullOrEmpty(kvp.Name) || initialTableKeys.Contains (kvp.Name))
            {
            kvp.Name = Guid.NewGuid ().ToString ();
            }
        initialTableKeys.Add (kvp.Name);
        }

    // Dirty keys are runtime keys with changed values. Newly added
    // keys don't matter because there are no listeners for them.
    this.dirtyKeys.Clear ();
    foreach (var kvp in this.runtimeTable)
        {
        bool value;
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
    this.table = new Dictionary<string, bool> ();
    this.initialTable = new List<BoolRegisterTableKeyValuePair> ();
    this.runtimeTable = new List<BoolRegisterTableKeyValuePair> ();
    this.dirtyKeys.Clear ();
    }

void OnValidate ()
    {
    if (Application.isPlaying)
        {
        foreach (var key in this.dirtyKeys)
            {
            List<BoolTableRegisterListener> listeners;
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


public bool this[string key]
    {
    get
        {
        bool value;
        if (this.table.TryGetValue (key, out value))
            {
            return value;
            }
        return this.defaultValue;
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
        value = this.defaultValue;
        this.table.Add (key, value);
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
