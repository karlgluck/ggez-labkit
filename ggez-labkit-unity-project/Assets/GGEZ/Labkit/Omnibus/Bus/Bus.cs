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
using WireList = System.Collections.Generic.List<GGEZ.Omnibus.Wire>;
using Connections = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<GGEZ.Omnibus.Wire>>;
using Memory = System.Collections.Generic.Dictionary<string, object>;
using SerializedMemory = System.Collections.Generic.List<GGEZ.Omnibus.SerializedMemoryCell>;
using StringCollection = System.Collections.Generic.ICollection<string>;

namespace GGEZ.Omnibus
{

// TODO: add "channels" or maybe "passthru pins" or something like that which do
// not have values held in this bus but are used exclusively with SignalObject

[
DisallowMultipleComponent,
AddComponentMenu ("GGEZ/Omnibus/Bus")
]
public sealed partial class Bus : MonoBehaviour, ISerializationCallbackReceiver
{

private Connections connections = new Connections ();
private Memory memory = new Memory ();

public void Connect (Wire wire)
    {
    if (wire == null)
        {
        throw new ArgumentNullException ("wire");
        }
    Debug.Assert (object.ReferenceEquals (wire.Bus, this));
    string pin = wire.BusPin;
    if (Pin.IsInvalid (pin))
        {
        throw new InvalidOperationException ("wire.pin is invalid");
        }
    WireList wires;
    if (!this.connections.TryGetValue (pin, out wires))
        {
        wires = new WireList ();
        this.connections.Add (pin, wires);
        }
    wires.Add (wire);

    object value;
    if (this.memory.TryGetValue (pin, out value))
        {
        wire.Signal (value);
        }

    }

public void Disconnect (Wire wire)
    {
    if (wire == null)
        {
        throw new ArgumentNullException ("cell");
        }
    Debug.Assert (object.ReferenceEquals (wire.Bus, this));
    string pin = wire.BusPin;
    if (Pin.IsInvalid (pin))
        {
        throw new InvalidOperationException ("wire.pin is invalid");
        }
    WireList wires;
    if (!this.connections.TryGetValue (pin, out wires))
        {
        throw new InvalidOperationException ("pin does not exist");
        }
    wires.Remove (wire);
    if (wires.Count == 0)
        {
        this.connections.Remove (pin);
        }
    }

public void Signal (string pin)
    {
    if (Pin.IsInvalid (pin))
        {
        throw new ArgumentException ("pin");
        }
    object value;
    this.memory.TryGetValue (pin, out value);
    WireList wires;
    if (!this.connections.TryGetValue (pin, out wires))
        {
        return;
        }
    for (int i = wires.Count - 1; i >= 0; --i)
        {
        wires[i].Signal (value);
        }
    }

public void SignalObject (string pin, object value)
    {
    if (Pin.IsInvalid (pin))
        {
        throw new ArgumentException ("pin");
        }
    WireList wires;
    if (!this.connections.TryGetValue (pin, out wires))
        {
        return;
        }
    for (int i = wires.Count - 1; i >= 0; --i)
        {
        wires[i].Signal (value);
        }
    }

public void SetObject (string pin, object value)
    {
    if (Pin.IsInvalid (pin))
        {
        throw new ArgumentException ("pin");
        }

    object oldValue;
    if (this.memory.TryGetValue (pin, out oldValue))
        {
        if (object.Equals (oldValue, value))
            {
            return;
            }
#if UNITY_EDITOR
        if (value != null && oldValue != null && value.GetType () != oldValue.GetType ())
            {
            throw new InvalidOperationException ("memory being replaced with object of a different type");
            }
#endif
        }

    this.memory[pin] = value;

    WireList wires;
    if (!this.connections.TryGetValue (pin, out wires))
        {
        return;
        }
    for (int i = wires.Count - 1; i >= 0; --i)
        {
        wires[i].Signal (value);
        }
    }

public object GetObject (string pin)
    {
    if (Pin.IsInvalid (pin))
        {
        throw new ArgumentException ("pin");
        }
    object value;
    this.memory.TryGetValue (pin, out value);
    return value;
    }

public object GetObject (string pin, object defaultValue)
    {
    if (Pin.IsInvalid (pin))
        {
        throw new ArgumentException ("pin");
        }
    object value;
    if (!this.memory.TryGetValue (pin, out value))
        {
        return defaultValue;
        }
    return value;
    }

public bool GetObject (string pin, out object value)
    {
    if (Pin.IsInvalid (pin))
        {
        throw new ArgumentException ("pin");
        }
    return this.memory.TryGetValue (pin, out value);
    }

public void Unset (string pin)
    {
    if (Pin.IsInvalid (pin))
        {
        throw new ArgumentException ("pin");
        }
    this.memory.Remove (pin);
    }

public void SetNull (string pin)
    {
    if (Pin.IsInvalid (pin))
        {
        throw new ArgumentException ("pin");
        }

    object oldValue;
    if (this.memory.TryGetValue (pin, out oldValue) && object.ReferenceEquals (oldValue, null))
        {
        return;
        }

    this.memory[pin] = null;

    WireList wires;
    if (!this.connections.TryGetValue (pin, out wires))
        {
        return;
        }
    for (int i = wires.Count - 1; i >= 0; --i)
        {
        wires[i].Signal (null);
        }
    }


public void SignalNull (string pin)
    {
    if (Pin.IsInvalid (pin))
        {
        throw new ArgumentException ("pin");
        }
    WireList wires;
    if (!this.connections.TryGetValue (pin, out wires))
        {
        return;
        }
    for (int i = wires.Count - 1; i >= 0; --i)
        {
        wires[i].Signal (null);
        }
    }



#region Templated versions of get/set

bool getT <T> (string pin, out T value)
    {
    if (pin == null)
        {
        throw new ArgumentNullException ("pin");
        }
    object objectValue;
    if (!this.memory.TryGetValue (pin, out objectValue))
        {
        value = default(T);
        return false;
        }
    value = (T)objectValue;
    return true;
    }

public T getT<T> (string pin, T defaultValue)
    {
    if (pin == null)
        {
        throw new ArgumentNullException ("pin");
        }
    object value;
    if (!this.memory.TryGetValue (pin, out value))
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
        this.serializedMemory.Add (SerializedMemoryCell.Create (key, this.GetObject (key)));
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
            WireList wires;
            if (this.connections.TryGetValue (key, out wires))
                {
                var value = this.GetObject (key);
                for (int i = wires.Count - 1; i >= 0; --i)
                    {
                    wires[i].Signal (value);
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

#if UNITY_EDITOR
#region workaround for lack of nameof() in Unity
public const string nameof_connections = "connections";
public const string nameof_serializedRom = "serializedRom";
public const string nameof_serializedMemory = "serializedMemory";
public const string nameof_Signal = "Signal";
public const string nameof_SignalObject = "SignalObject";
#endregion
#endif

}







}
