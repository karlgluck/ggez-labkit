using System;
using UnityEngine;
using System.Collections.Generic;
using ListenerList = System.Collections.Generic.List<GGEZ.Listener>;
using ListenersTable = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<GGEZ.Listener>>;
using RegistersTable = System.Collections.Generic.Dictionary<string, object>;
using SerializableKeyValuePairList = System.Collections.Generic.List<GGEZ.RegistrarKeyPair>;
using StringCollection = System.Collections.Generic.ICollection<string>;

namespace GGEZ
{




[
Serializable,
CreateAssetMenu (fileName = "New Registrar.asset", menuName="GGEZ/Game Registrar")
]
public sealed class RegistrarAsset : ScriptableObject, Registrar, ISerializationCallbackReceiver
{

#region Serialization
// All the values that the table should start with
[SerializeField] private SerializableKeyValuePairList initialTable = new SerializableKeyValuePairList ();

// Values that the table has at runtime only. Entries are cleared out when
// listeners unregister.
[SerializeField] private SerializableKeyValuePairList runtimeTable = new SerializableKeyValuePairList ();

void ISerializationCallbackReceiver.OnBeforeSerialize ()
    {
    //var runtimeKeys = new HashSet<string> (this.listenersTable.Keys);
    //runtimeKeys.IntersectWith (this.registersTable.Keys);
    var runtimeKeys = new HashSet<string> (this.registersTable.Keys);
    this.runtimeTable.Clear ();
    this.runtimeTable.Capacity = Mathf.Max (this.runtimeTable.Capacity, runtimeKeys.Count);
    foreach (var key in runtimeKeys)
        {
        this.runtimeTable.Add (RegistrarKeyPair.Create (key, this.Get (key), false));
        }
    }

void ISerializationCallbackReceiver.OnAfterDeserialize ()
    {
#if UNITY_EDITOR

    // Clean up the runtime keys
    HashSet<string> runtimeTableKeys = new HashSet<string> ();
    for (int i = this.runtimeTable.Count - 1; i >= 0; --i)
        {
        var kvp = this.runtimeTable[i];
        if (kvp == null
                || string.IsNullOrEmpty(kvp.Name)
                || runtimeTableKeys.Contains (kvp.Name))
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
        this.runtimeTable.Add (RegistrarKeyPair.Create (key, this.Get (key), false));
        }

    // Clean up initial keys so that there are no duplicates
    // or null entries. These can happen when adding elements.
    HashSet<string> initialTableKeys = new HashSet<string> ();
    for (int i = this.initialTable.Count - 1; i >= 0; --i)
        {
        var kvp = this.initialTable[i];
        if (kvp == null)
            {
            this.initialTable.RemoveAt (i);
            continue;
            }
        if (string.IsNullOrEmpty(kvp.Name) || initialTableKeys.Contains (kvp.Name))
            {
            kvp.Name = Guid.NewGuid ().ToString ();
            }
        initialTableKeys.Add (kvp.Name);
        }

    // Dirty keys are runtime keys with changed values. We only
    // need to do this in the Editor because the app will change
    // values through the class interface only.
    this.dirtyKeys.Clear ();
    foreach (var kvp in this.runtimeTable)
        {
        object oldValue;
        if (this.registersTable.TryGetValue (kvp.Name, out oldValue) && !object.Equals (oldValue, kvp.GetValue ()))
            {
            this.dirtyKeys.Add (kvp.Name);
            }
        }

#endif

    this.registersTable.Clear ();
    foreach (var kvp in this.initialTable)
        {
        this.registersTable.Add (kvp.Name, kvp.GetValue ());
        }
    foreach (var kvp in this.runtimeTable)
        {
        this.registersTable[kvp.Name] = kvp.GetValue ();
        }
    }

#if UNITY_EDITOR
private List<string> dirtyKeys = new List<string> ();

void Reset ()
    {
    this.registersTable = new RegistersTable ();
    this.initialTable = new SerializableKeyValuePairList ();
    this.runtimeTable = new SerializableKeyValuePairList ();
    this.dirtyKeys.Clear ();
    this.listenersTable.Clear ();
    }

void OnValidate ()
    {
    if (Application.isPlaying)
        {
        foreach (var key in this.dirtyKeys)
            {
            ListenerList listeners;
            if (this.listenersTable.TryGetValue (key, out listeners))
                {
                var value = this.Get (key);
                for (int i = listeners.Count - 1; i >= 0; --i)
                    {
                    listeners[i].OnDidChange (key, value);
                    }
                }
            }
        this.dirtyKeys.Clear ();
        }
    else
        {
        this.runtimeTable.Clear ();
        this.registersTable.Clear ();
        for (int i = 0; i < this.initialTable.Count; ++i)
            {
            var kvp = this.initialTable[i];
            this.registersTable.Add (kvp.Name, kvp.GetValue ());
            }
        }
    }
#endif
#endregion

private ListenersTable listenersTable = new ListenersTable ();
private RegistersTable registersTable = new RegistersTable ();

public bool HasListeners (string key)
    {
    return this.listenersTable.ContainsKey (key);
    }

public bool HasValue (string key)
    {
    return this.registersTable.ContainsKey (key);
    }

public StringCollection GetEventKeys ()
    {
    var retval = new HashSet<string> (this.listenersTable.Keys);
    retval.ExceptWith (this.registersTable.Keys);
    return retval;
    }

public StringCollection GetRegisterKeys ()
    {
    return this.registersTable.Keys;
    }

public void RegisterListener (string key, Listener listener)
    {
    if (key == null)
        {
        throw new ArgumentNullException ("key");
        }
    if (listener == null)
        {
        throw new ArgumentNullException ("listener");
        }
    ListenerList listenersForKey;
    if (!this.listenersTable.TryGetValue (key, out listenersForKey))
        {
        listenersForKey = new ListenerList ();
        this.listenersTable.Add (key, listenersForKey);
        }
    listenersForKey.Add (listener);

    object value;
    if (this.registersTable.TryGetValue (key, out value))
        {
        listener.OnDidChange (key, value);
        }

    }

public void UnregisterListener (string key, Listener listener)
    {
    if (key == null)
        {
        throw new ArgumentNullException ("key");
        }
    if (listener == null)
        {
        throw new ArgumentNullException ("listener");
        }
    ListenerList listenersForKey;
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


#region void
public void Trigger (string key)
    {
    if (key == null)
        {
        throw new ArgumentNullException ("key");
        }
    ListenerList listeners;
    if (!this.listenersTable.TryGetValue (key, out listeners))
        {
        return;
        }
    for (int i = listeners.Count - 1; i >= 0; --i)
        {
        listeners[i].OnDidTrigger (key, null);
        }
    }
#endregion




#region object
public object Get (string key)
    {
    if (key == null)
        {
        throw new ArgumentNullException ("key");
        }
    object value;
    if (!this.registersTable.TryGetValue (key, out value))
        {
        return null;
        }
    return value;
    }
#endregion




#region int
public void Set (string key, int value)
    {
    if (key == null)
        {
        throw new ArgumentNullException ("key");
        }
    object oldValue;
    if (this.registersTable.TryGetValue (key, out oldValue) && object.Equals (oldValue, value))
        {
        return;
        }
    this.registersTable[key] = value;
    ListenerList listeners;
    if (!this.listenersTable.TryGetValue (key, out listeners))
        {
        return;
        }
    for (int i = listeners.Count - 1; i >= 0; --i)
        {
        listeners[i].OnDidChange (key, value);
        }
    }


public void Trigger (string key, int value)
    {
    if (key == null)
        {
        throw new ArgumentNullException ("key");
        }
    ListenerList listeners;
    if (!this.listenersTable.TryGetValue (key, out listeners))
        {
        return;
        }
    for (int i = listeners.Count - 1; i >= 0; --i)
        {
        listeners[i].OnDidTrigger (key, value);
        }
    }

public bool Get (string key, out int value)
    {
    if (key == null)
        {
        throw new ArgumentNullException ("key");
        }
    object objectValue;
    if (!this.registersTable.TryGetValue (key, out objectValue))
        {
        value = default(int);
        return false;
        }
    value = (int)objectValue;
    return true;
    }

public int GetInt (string key, int defaultValue)
    {
    if (key == null)
        {
        throw new ArgumentNullException ("key");
        }
    object value;
    if (!this.registersTable.TryGetValue (key, out value))
        {
        return defaultValue;
        }
    return (int)value;
    }
    
#endregion





#region int
public void Set (string key, bool value)
    {
    if (key == null)
        {
        throw new ArgumentNullException ("key");
        }
    object oldValue;
    if (this.registersTable.TryGetValue (key, out oldValue) && object.Equals (oldValue, value))
        {
        return;
        }
    this.registersTable[key] = value;
    ListenerList listeners;
    if (!this.listenersTable.TryGetValue (key, out listeners))
        {
        return;
        }
    for (int i = listeners.Count - 1; i >= 0; --i)
        {
        listeners[i].OnDidChange (key, value);
        }
    }


public void Trigger (string key, bool value)
    {
    if (key == null)
        {
        throw new ArgumentNullException ("key");
        }
    ListenerList listeners;
    if (!this.listenersTable.TryGetValue (key, out listeners))
        {
        return;
        }
    for (int i = listeners.Count - 1; i >= 0; --i)
        {
        listeners[i].OnDidTrigger (key, value);
        }
    }

public bool Get (string key, out bool value)
    {
    if (key == null)
        {
        throw new ArgumentNullException ("key");
        }
    object objectValue;
    if (!this.registersTable.TryGetValue (key, out objectValue))
        {
        value = default(bool);
        return false;
        }
    value = (bool)objectValue;
    return true;
    }

public bool GetBool (string key, bool defaultValue)
    {
    if (key == null)
        {
        throw new ArgumentNullException ("key");
        }
    object value;
    if (!this.registersTable.TryGetValue (key, out value))
        {
        return defaultValue;
        }
    return (bool)value;
    }
    
#endregion



}







}


