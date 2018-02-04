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
using Pool = System.Collections.Generic.List<UnityEngine.GameObject>;
using InstanceMap = System.Collections.Generic.Dictionary<GGEZ.Omnibus.ReferenceKey, UnityEngine.GameObject>;
using System.Collections.Generic;

namespace GGEZ.Omnibus
{


public interface IPrefabSpawnerPlugin
{
void OnWillCreateInstance (PrefabSpawnerModule self, Bus bus, out bool allowCreation);
void OnDidCreateInstance (PrefabSpawnerModule self, Bus bus, GameObject instance, IRouter router);
void OnWillDestroyInstance (PrefabSpawnerModule self, Bus bus, out bool allowDestruction);
void OnDidDestroyInstance (PrefabSpawnerModule self, Bus bus);
}


[
Serializable,
AddComponentMenu ("GGEZ/Omnibus/Module/Prefab/Prefab Spawner (Module)")
]
public class PrefabSpawnerModule : Cell
{

#region Programming Interface

public Bus Bus
    {
    get { return this.bus; }
    set
        {
        this.bus = value;
        this.createWire.Connect (this.bus, this.createPin);
        }
    }

public string CreatePin
    {
    get { return this.createPin; }
    set
        {
        this.createPin = value;
        this.createWire.Connect (this.bus, this.createPin);
        }
    }
   
public string DestroyPin
    {
    get { return this.destroyPin; }
    set
        {
        this.destroyPin = value;
        this.destroyWire.Connect (this.bus, this.destroyPin);
        }
    }

public GameObject Prefab
    {
    get { return this.prefab; }
    }

#endregion

const string CREATE_PIN = "CREATE";
const string DESTROY_PIN = "DESTROY";


[SerializeField] private Bus bus;

[Header ("*:" + CREATE_PIN + " (Bus)")]
[SerializeField] private string createPin;


[Header ("*:" + DESTROY_PIN + " (Bus)")]
[SerializeField] private string destroyPin;


[Header ("Settings")]
[SerializeField] private bool usePool;
[SerializeField] private Transform parent;
[SerializeField] private GameObject prefab;

[Header ("Runtime Data")]
private IPrefabSpawnerPlugin plugin;
private Pool pool = new Pool (); 
private InstanceMap instanceMap = new InstanceMap ();

public override void OnDidSignal (string pin, object value)
    {
    var valueType = value.GetType ();
    Bus bus;
    if (typeof(Bus).IsAssignableFrom (value.GetType ()))
        {
        bus = (Bus)value;
        }
    else if (typeof (GameObject).IsAssignableFrom (value.GetType ()))
        {
        bus = (Bus)((GameObject)value).GetComponent (typeof (Bus));
        }
    else if (typeof (KeyValuePair<string, object>).IsAssignableFrom (value.GetType ()))
        {
        var kvp = (KeyValuePair<string, object>)value;
        this.OnDidSignal (pin, kvp.Value);
        return;
        }
    else
        {
        throw new System.InvalidCastException ("`value` should be " + typeof(Bus).Name + " or " + typeof(GameObject).Name + ", not " + value.GetType().Name);
        }

    switch (pin)
        {

        case CREATE_PIN:
            this.createInstance (bus);
            break;

        case DESTROY_PIN:
            this.destroyInstance (bus);
            break;

#if UNITY_EDITOR
        default:
            Debug.LogError ("signal on invalid pin: " + pin);
            break;
#endif

        }

    }

private void createInstance (Bus bus)
    {
    // Debug.Log (this.GetHashCode () + " createInstance " + bus.GetHashCode (), bus);
    if (bus == null)
        {
        throw new ArgumentNullException ("bus");
        }
    var key = new ReferenceKey (bus);
    GameObject instance;
    if (this.instanceMap.TryGetValue (key, out instance) && instance != null)
        {
#if UNITY_EDITOR
        Debug.LogError ("PrefabSpawnerModule received duplicate on the " + CREATE_PIN + " pin. Ignoring.");
#endif
        return;
        }
    if (this.plugin != null)
        {
        bool allowCreation;
        this.plugin.OnWillCreateInstance (this, bus, out allowCreation);
        if (!allowCreation)
            {
            return;
            }
        }
    if (!this.usePool || null == (instance = this.acquireInstanceFromPool ()))
        {
        instance = GameObject.Instantiate (this.prefab);
        }
    var router = (IRouter)instance.GetComponent (typeof (IRouter));
    if (router != null)
        {
        router.Route (bus);
        }
    instance.transform.SetParent (this.parent, false);
    instance.SetActive (true);
    this.instanceMap[key] = instance;
    // Debug.Log ("instanceMap now contains " + this.instanceMap.Count);
    if (this.plugin != null)
        {
        this.plugin.OnDidCreateInstance (this, bus, instance, router);
        }
    }

private GameObject acquireInstanceFromPool ()
    {
    if (this.pool.Count == 0)
        {
        return null;
        }
    var retval = this.pool[0];
    this.pool.RemoveAt (0);
    return retval;
    }

private void destroyInstance (Bus bus)
    {
    // Debug.Log (this.GetHashCode () + " destroyInstance " + bus.GetHashCode (), bus);
    if (bus == null)
        {
        throw new ArgumentNullException ("bus");
        }
    var key = new ReferenceKey (bus);
    GameObject instance;
    if (!this.instanceMap.TryGetValue (key, out instance))
        {
        return; // this happens when a plugin doesn't allow creation
        }
    if (this.plugin != null)
        {
        bool allowDestruction;
        this.plugin.OnWillDestroyInstance (this, bus, out allowDestruction);
        if (!allowDestruction)
            {
            return;
            }
        }
    this.instanceMap.Remove (key);
    // Debug.Log ("instanceMap now contains " + this.instanceMap.Count, this);
    if (this.usePool)
        {
        this.releaseInstanceToPool (instance);
        }
    else
        {
        // Debug.Log ("Destroying " + instance.name);
        GameObject.Destroy (instance);
        }
    if (this.plugin != null)
        {
        this.plugin.OnDidDestroyInstance (this, bus);
        }
    }

private void releaseInstanceToPool (GameObject instance)
    {
    // Debug.Log ("releaseInstanceToPool " + instance.GetHashCode (), instance);
    if (instance == null)
        {
        throw new ArgumentNullException ("instance");
        }
    instance.SetActive (false);
    instance.transform.SetParent (this.transform, false);
    // instance.SendMessage ("OnDidRelease", null, SendMessageOptions.DontRequireReceiver);
    var router = (IRouter)instance.GetComponent (typeof (IRouter));
    if (router != null)
        {
        router.Route (null);
        }
    this.pool.Add (instance);
    }

public override void Route (string port, Bus bus)
    {
    this.Bus = bus;
    }

private Wire createWire = new Wire (CREATE_PIN);
private Wire destroyWire = new Wire (DESTROY_PIN);

void Awake ()
    {
    this.plugin = (IPrefabSpawnerPlugin)this.GetComponent (typeof (IPrefabSpawnerPlugin));
    }

void OnEnable ()
    {
    this.createWire.Attach (this, this.bus, this.createPin);
    this.destroyWire.Attach (this, this.bus, this.destroyPin);
    }

void OnDisable ()
    {
    this.createWire.Detach ();
    this.destroyWire.Detach ();
    }

void OnDestroy ()
    {
    foreach (var kv in this.instanceMap)
        {
        GameObject.Destroy (kv.Value);
        }
    this.instanceMap.Clear ();
    if (this.usePool)
        {
        for (int i = 0; i < this.pool.Count; ++i)
            {
            GameObject.Destroy (this.pool[i]);
            }
        this.pool.Clear ();
        }
    }

void OnValidate ()
    {
    this.createWire.Disconnect ();
    this.destroyWire.Disconnect ();
    this.createWire.Connect (this.bus, this.createPin);
    this.destroyWire.Connect (this.bus, this.destroyPin);
    }

}

internal class ReferenceKey
{
private object key;

public ReferenceKey (object obj)
    {
    this.key = obj;
    }

public override bool Equals (object other)
    {
    return object.ReferenceEquals (((ReferenceKey)other).key, this.key);
    }
    
public override int GetHashCode ()
    {
    return this.key.GetHashCode ();
    }
}

}
