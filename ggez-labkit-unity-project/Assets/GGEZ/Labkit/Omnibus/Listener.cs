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
        var keys = this.GetKeys ();
        var thisRegistrar = (Registrar)this.registrar;
        if (this.hasBeenEnabled && this.registrar != null)
            {
            foreach (string key in keys)
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
            foreach (string key in keys)
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
    var keys = this.GetKeys ();
    var thisRegistrar = (Registrar)this.registrar;
    if (thisRegistrar != null)
        {
        foreach (string key in keys)
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
    this.previousKeys.AddRange (keys);
    this.previousRegistrar = thisRegistrar;
#endif
    }


void OnDisable ()
    {
    var thisRegistrar = (Registrar)this.registrar;
    if (thisRegistrar != null)
        {
        foreach (string key in this.GetKeys ())
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


public virtual void OnDidTrigger (string key, object value) {}
public virtual void OnDidChange (string key, object value) {}
public virtual IEnumerable<string> GetKeys ()
    {
    throw new System.NotImplementedException ();
    }


//----------------------------------------------------------------------
// Handle the Editor changing values in the inspector
//----------------------------------------------------------------------
#if UNITY_EDITOR
#region Editor Runtime
private Keys previousKeys = new Keys ();
private Registrar previousRegistrar;


void OnValidate ()
    {
    var thisRegistrar = this.registrar as Registrar;
    if (thisRegistrar == null)
        {
        var gameObject = this.registrar as GameObject;
        if (gameObject != null)
            {
            this.registrar = gameObject.GetComponent <RegistrarBehaviour> ();
            }
        else
            {
            this.registrar = null;
            }
        }

    if (!this.hasBeenEnabled)
        {
        return;
        }

    var keys = this.GetKeys ();
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
        this.previousKeys.AddRange (keys);
        foreach (var key in keys)
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
        var added = new HashSet<string> (keys);

        removed.ExceptWith (keys);
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
        this.previousKeys.AddRange (keys);
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
