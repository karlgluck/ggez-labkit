using System;
using UnityEngine;
using Keys = System.Collections.Generic.List<string>;
using ReadonlyKeys = System.Collections.ObjectModel.ReadOnlyCollection<string>;
using System.Collections.Generic;

namespace GGEZ
{


public class Listener : MonoBehaviour
{

// Either a RegistrarBehaviour or a RegistrarAsset
[SerializeField] private UnityEngine.Object registrar;

public Registrar Registrar
    {
    get
        {
        return (Registrar)this.registrar;
        }
    set
        {
        if (object.ReferenceEquals (this.registrar, value))
            {
            return;
            }
        var thisRegistrar = (Registrar)this.registrar;
        if (this.hasBeenEnabled && this.registrar != null)
            {
            foreach (string key in this.keys.Keys)
                {
                if (!string.IsNullOrEmpty (key))
                    {
                    thisRegistrar.UnregisterListener (key, this);
                    }
                }
            }
        this.registrar = (UnityEngine.Object)value;
#if UNITY_EDITOR
        if (this.hasBeenEnabled)
            {
            this.previousRegistrar = value;
            }
#endif
        if (this.hasBeenEnabled && thisRegistrar != null)
            {
            foreach (string key in this.keys.Keys)
                {
                if (!string.IsNullOrEmpty (key))
                    {
                    thisRegistrar.RegisterListener (key, this);
                    }
                }
            }
        }
    }

private bool hasBeenEnabled;

void OnEnable ()
    {
    var thisRegistrar = (Registrar)this.registrar;
    if (thisRegistrar != null)
        {
        foreach (string key in this.keys.Keys)
            {
            if (!string.IsNullOrEmpty (key))
                {
                thisRegistrar.RegisterListener (key, this);
                }
            }
        }
    this.hasBeenEnabled = true;
#if UNITY_EDITOR
    this.previousKeys.Clear ();
    this.previousKeys.AddRange (this.keys.Keys);
    this.previousRegistrar = thisRegistrar;
#endif
    }


void OnDisable ()
    {
    var thisRegistrar = (Registrar)this.registrar;
    if (thisRegistrar != null)
        {
        foreach (string key in this.keys.Keys)
            {
            if (!string.IsNullOrEmpty (key))
                {
                thisRegistrar.UnregisterListener (key, this);
                }
            }
        }
    this.hasBeenEnabled = false;
#if UNITY_EDITOR
    this.previousKeys.Clear ();
    this.previousRegistrar = null;
#endif
    }

// All the keys this listener cares about
[SerializeField] private ListenerKeys keys = new ListenerKeys ();
public ReadonlyKeys Keys { get { return this.keys.Keys.AsReadOnly (); } }

public void AddKey (string key)
    {
    throw new NotImplementedException ();
    }

public void RemoveKey (string key)
    {
    throw new NotImplementedException ();
    }

public void ClearKeys ()
    {
    throw new NotImplementedException ();
    }

public virtual void OnDidTrigger (string key, object value) {}
public virtual void OnDidChange (string key, object value) {}
public virtual IEnumerable<string> GetKeys () { yield break; }


//----------------------------------------------------------------------
// Handle the Editor changing values in the inspector
//----------------------------------------------------------------------
#if UNITY_EDITOR
#region Editor Runtime
private Keys previousKeys = new Keys ();
private Registrar previousRegistrar;


void OnValidate ()
    {
    if (!this.hasBeenEnabled)
        {
        return;
        }
        
    var thisRegistrar = this.registrar as Registrar;
    if (thisRegistrar == null)
        {
        this.registrar = null;
        }

    if (!object.ReferenceEquals (this.previousRegistrar, thisRegistrar))
        {
        foreach (var key in this.previousKeys)
            {
            if (!string.IsNullOrEmpty (key))
                {
                this.previousRegistrar.UnregisterListener (key, this);
                }
            }
        this.previousRegistrar = thisRegistrar;
        this.previousKeys.Clear ();
        this.previousKeys.AddRange (this.keys.Keys);
        foreach (var key in this.previousKeys)
            {
            if (!string.IsNullOrEmpty (key))
                {
                thisRegistrar.RegisterListener (key, this);
                }
            }
        }
    else
        {
        var removed = new HashSet<string> (this.previousKeys);
        var added = new HashSet<string> (this.keys.Keys);

        removed.ExceptWith (this.keys.Keys);
        added.ExceptWith (this.previousKeys);

        foreach (var key in removed)
            {
            if (!string.IsNullOrEmpty (key))
                {
                this.previousRegistrar.UnregisterListener (key, this);
                }
            }
        this.previousRegistrar = thisRegistrar;
        this.previousKeys.Clear ();
        this.previousKeys.AddRange (this.keys.Keys);
        foreach (var key in added)
            {
            if (!string.IsNullOrEmpty (key))
                {
                thisRegistrar.RegisterListener (key, this);
                }
            }
        }


    }

#endregion
#endif



}



}
