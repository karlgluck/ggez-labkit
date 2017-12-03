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
using FubList = System.Collections.Generic.List<GGEZ.Omnibus.IFub>;
using Connections = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<GGEZ.Omnibus.IFub>>;
using Memory = System.Collections.Generic.Dictionary<string, object>;
using SerializedMemory = System.Collections.Generic.List<GGEZ.Omnibus.SerializedMemoryCell>;
using StringCollection = System.Collections.Generic.ICollection<string>;

namespace GGEZ
{
namespace Omnibus
{



[
DisallowMultipleComponent,
AddComponentMenu ("GGEZ/Omnibus/Bus")
]
public sealed partial class Bus : MonoBehaviour, IBus, ISerializationCallbackReceiver
{

private Connections connections = new Connections ();
private Memory memory = new Memory ();


public void Connect (string key, IFub fub)
    {
    if (key == null)
        {
        throw new ArgumentNullException ("key");
        }
    if (fub == null)
        {
        throw new ArgumentNullException ("fub");
        }
    FubList fubs;
    if (!this.connections.TryGetValue (key, out fubs))
        {
        fubs = new FubList ();
        this.connections.Add (key, fubs);
        }
    fubs.Add (fub);

    object value;
    if (this.memory.TryGetValue (key, out value))
        {
        fub.OnDidChange (key, value);
        }

    }

public void Disconnect (string key, IFub fub)
    {
    if (key == null)
        {
        throw new ArgumentNullException ("key");
        }
    if (fub == null)
        {
        throw new ArgumentNullException ("fub");
        }
    FubList fubs;
    if (!this.connections.TryGetValue (key, out fubs))
        {
        throw new InvalidOperationException ("key does not exist");
        }
    fubs.Remove (fub);
    if (fubs.Count == 0)
        {
        this.connections.Remove (key);
        }
    }

public void Trigger (string key)
    {
    if (key == null)
        {
        throw new ArgumentNullException ("key");
        }
    FubList fubs;
    if (!this.connections.TryGetValue (key, out fubs))
        {
        return;
        }
    for (int i = fubs.Count - 1; i >= 0; --i)
        {
        fubs[i].OnDidTrigger (key, null);
        }
    }

public void Set (string key, object value)
    {
    if (key == null)
        {
        throw new ArgumentNullException ("key");
        }
    object oldValue;
    if (this.memory.TryGetValue (key, out oldValue) && object.Equals (oldValue, value))
        {
        return;
        }
    this.memory[key] = value;
    FubList fubs;
    if (!this.connections.TryGetValue (key, out fubs))
        {
        return;
        }
    for (int i = fubs.Count - 1; i >= 0; --i)
        {
        fubs[i].OnDidChange (key, value);
        }
    }

public void Trigger (string key, object value)
    {
    if (key == null)
        {
        throw new ArgumentNullException ("key");
        }
    FubList fubs;
    if (!this.connections.TryGetValue (key, out fubs))
        {
        return;
        }
    for (int i = fubs.Count - 1; i >= 0; --i)
        {
        fubs[i].OnDidTrigger (key, value);
        }
    }

public object Get (string key)
    {
    if (key == null)
        {
        throw new ArgumentNullException ("key");
        }
    object value;
    if (!this.memory.TryGetValue (key, out value))
        {
        return null;
        }
    return value;
    }

public object Get (string key, object defaultValue)
    {
    if (key == null)
        {
        throw new ArgumentNullException ("key");
        }
    object value;
    if (!this.memory.TryGetValue (key, out value))
        {
        return defaultValue;
        }
    return value;
    }

public bool Get (string key, out object value)
    {
    if (key == null)
        {
        throw new ArgumentNullException ("key");
        }
    return this.memory.TryGetValue (key, out value);
    }

public void Unset (string key)
    {
    if (key == null)
        {
        throw new ArgumentNullException ("key");
        }
    this.memory.Remove (key);
    }

public void SetNull (string key)
    {
    if (key == null)
        {
        throw new ArgumentNullException ("key");
        }
    this.memory[key] = null;
    }

#region Templated versions of get/set/trigger
void set<T> (string key, T value)
    {
    if (key == null)
        {
        throw new ArgumentNullException ("key");
        }
    object oldValue;
    if (this.memory.TryGetValue (key, out oldValue) && object.Equals (oldValue, value))
        {
        return;
        }
    this.memory[key] = value;
    FubList fubs;
    if (!this.connections.TryGetValue (key, out fubs))
        {
        return;
        }
    for (int i = fubs.Count - 1; i >= 0; --i)
        {
        fubs[i].OnDidChange (key, value);
        }
    }

void trigger <T> (string key, T value)
    {
    if (key == null)
        {
        throw new ArgumentNullException ("key");
        }
    FubList fubs;
    if (!this.connections.TryGetValue (key, out fubs))
        {
        return;
        }
    for (int i = fubs.Count - 1; i >= 0; --i)
        {
        fubs[i].OnDidTrigger (key, value);
        }
    }

bool get <T> (string key, out T value)
    {
    if (key == null)
        {
        throw new ArgumentNullException ("key");
        }
    object objectValue;
    if (!this.memory.TryGetValue (key, out objectValue))
        {
        value = default(T);
        return false;
        }
    value = (T)objectValue;
    return true;
    }

public T getT<T> (string key, T defaultValue)
    {
    if (key == null)
        {
        throw new ArgumentNullException ("key");
        }
    object value;
    if (!this.memory.TryGetValue (key, out value))
        {
        return defaultValue;
        }
    return (T)value;
    }
#endregion


#region Serialization
[SerializeField] private SerializedMemory serializedRom = new SerializedMemory ();
[SerializeField] private SerializedMemory serializedMemory = new SerializedMemory ();

void ISerializationCallbackReceiver.OnBeforeSerialize ()
    {
    var runtimeKeys = new HashSet<string> (this.memory.Keys);
    this.serializedMemory.Clear ();
    this.serializedMemory.Capacity = Mathf.Max (this.serializedMemory.Capacity, runtimeKeys.Count);
    foreach (var key in runtimeKeys)
        {
        this.serializedMemory.Add (SerializedMemoryCell.Create (key, this.Get (key)));
        }
    }

void ISerializationCallbackReceiver.OnAfterDeserialize ()
    {
#if UNITY_EDITOR

    // Clean up deserialized values
    HashSet<string> runtimeTableKeys = new HashSet<string> ();
    for (int i = this.serializedMemory.Count - 1; i >= 0; --i)
        {
        var kvp = this.serializedMemory[i];
        if (kvp == null
                || string.IsNullOrEmpty(kvp.Key)
                || runtimeTableKeys.Contains (kvp.Key))
            {
            this.serializedMemory.RemoveAt (i);
            continue;
            }
        runtimeTableKeys.Add (kvp.Key);
        }

    // Clean up deserialized values
    HashSet<string> initialTableKeys = new HashSet<string> ();
    for (int i = this.serializedRom.Count - 1; i >= 0; --i)
        {
        var kvp = this.serializedRom[i];
        if (kvp == null)
            {
            this.serializedRom.RemoveAt (i);
            continue;
            }
        if (string.IsNullOrEmpty(kvp.Key) || initialTableKeys.Contains (kvp.Key))
            {
            kvp.Key = Guid.NewGuid ().ToString ();
            }
        initialTableKeys.Add (kvp.Key);
        }

    // Find dirty keys
    this.dirtyKeys.Clear ();
    foreach (var kvp in this.serializedMemory)
        {
        object oldValue;
        if (this.memory.TryGetValue (kvp.Key, out oldValue) && !object.Equals (oldValue, kvp.GetValue ()))
            {
            this.dirtyKeys.Add (kvp.Key);
            }
        }

#endif

    this.memory.Clear ();
    foreach (var kvp in this.serializedRom)
        {
        this.memory.Add (kvp.Key, kvp.GetValue ());
        }
    foreach (var kvp in this.serializedMemory)
        {
        this.memory[kvp.Key] = kvp.GetValue ();
        }
    }

#if UNITY_EDITOR
private List<string> dirtyKeys = new List<string> ();

void Reset ()
    {
    this.memory = new Memory ();
    this.serializedRom = new SerializedMemory ();
    this.serializedMemory = new SerializedMemory ();
    this.dirtyKeys.Clear ();
    this.connections.Clear ();
    }

void OnValidate ()
    {
    if (Application.isPlaying)
        {
        foreach (var key in this.dirtyKeys)
            {
            FubList fubs;
            if (this.connections.TryGetValue (key, out fubs))
                {
                var value = this.Get (key);
                for (int i = fubs.Count - 1; i >= 0; --i)
                    {
                    fubs[i].OnDidChange (key, value);
                    }
                }
            }
        this.dirtyKeys.Clear ();
        }
    else
        {
        this.serializedMemory.Clear ();
        this.memory.Clear ();
        for (int i = 0; i < this.serializedRom.Count; ++i)
            {
            var kvp = this.serializedRom[i];
            this.memory.Add (kvp.Key, kvp.GetValue ());
            }
        }
    }
#endif
#endregion


#region Accessors

public bool HasConnections (string key)
    {
    return this.connections.ContainsKey (key);
    }

public bool HasValue (string key)
    {
    return this.memory.ContainsKey (key);
    }

public StringCollection GetAllKeys ()
    {
    var retval = new HashSet<string> (this.connections.Keys);
    retval.UnionWith (this.memory.Keys);
    return retval;
    }

public StringCollection GetConnectedKeys ()
    {
    return this.connections.Keys;
    }

public StringCollection GetEventKeys ()
    {
    var retval = new HashSet<string> (this.connections.Keys);
    retval.ExceptWith (this.memory.Keys);
    return retval;
    }

public StringCollection GetMemoryKeys ()
    {
    return this.memory.Keys;
    }

#endregion

#region workaround for lack of nameof() in Unity
public const string nameof_connections = "connections";
public const string nameof_serializedRom = "serializedRom";
public const string nameof_serializedMemory = "serializedMemory";
#endregion
}







}



}


// Create BusAsset.cs by copying this file and replacing the class
// declaration with the following:
/*
[
Serializable,
CreateAssetMenu (fileName = "New Bus.asset", menuName="GGEZ/Omnibus/Bus")
]
public sealed partial class BusAsset : ScriptableObject, IBus, ISerializationCallbackReceiver
*/
