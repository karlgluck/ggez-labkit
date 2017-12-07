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

using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace GGEZ
{
namespace Omnibus
{



public class ImplementMuxForType<T, D> : Fub where D : UnityEvent<T>, new ()
{

protected class DataBusConnector : IFub
	{
	public ImplementMuxForType<T, D> Owner;
	public void OnDidTrigger (string key, object value) {}
	public void OnDidChange (string key, object value)
		{
		this.Owner.onDataBusValueDidChange (value);
		}
	}

[SerializeField] private string key;
[SerializeField] private UnityEngine.Object dataBus; // MonoBehaviour or ScriptableObject
private object currentValue = null;
[SerializeField] private D didChange = new D ();
private DataBusConnector dataBusConnector;


public ImplementMuxForType ()
	{
	this.dataBusConnector = new DataBusConnector ()
			{
			Owner = this,
			};
	}

private string dataKey = null;

public override void OnDidChange (string key, object value)
    {
#if UNITY_EDITOR
	if (value != null && !typeof(string).IsAssignableFrom (value.GetType ()))
		{
		throw new System.InvalidCastException ("`value` should be a string");
		}
#endif
	var dataBus = this.dataBus as Bus;
	if (dataBus != null && !string.IsNullOrEmpty (this.dataKey))
		{
		// dataBus.Disconnect (this.dataKey, this.dataBusConnector);
		}
	this.dataKey = (string)value;
	if (dataBus != null && !string.IsNullOrEmpty (this.dataKey))
		{
		// dataBus.Connect (this.dataKey, this.dataBusConnector);
		}
    }

private void onDataBusValueDidChange (object value)
	{
#if UNITY_EDITOR
	if (value != null && !typeof(T).IsAssignableFrom (value.GetType ()))
		{
		throw new System.InvalidCastException ("`dataBus[" + dataKey + "]` should be " + typeof(T).Name);
		}
#endif
	if (!object.Equals (this.currentValue, value))
		{
		T typedValue = (T)value;
		this.currentValue = value;
    	this.didChange.Invoke (typedValue);
		}
	}

public override IEnumerable<string> GetKeys ()
    {
    yield return this.key;
    }

public void AddCallback (UnityAction<T> action)
    {
    this.didChange.AddListener (action);
    }

public void RemoveCallback (UnityAction<T> action)
    {
    this.didChange.RemoveListener (action);
    }

public void RemoveAllCallbacks ()
    {
    this.didChange.RemoveAllListeners ();
    }

new protected void OnEnable ()
	{
	// base.OnEnable ();
	var dataBus = this.dataBus as Bus;
	if (dataBus != null && !string.IsNullOrEmpty (this.dataKey))
		{
		// dataBus.Connect (this.dataKey, this.dataBusConnector);
		}
#if UNITY_EDITOR
	this.previousDataBus = dataBus;
#endif
	}

new protected void OnDisable ()
	{
	var dataBus = this.dataBus as Bus;
	if (dataBus != null && !string.IsNullOrEmpty (this.dataKey))
		{
		// dataBus.Disconnect (this.dataKey, this.dataBusConnector);
		}
	this.dataKey = null;
	// base.OnDisable ();
#if UNITY_EDITOR
	this.previousDataBus = null;
#endif
	}


//----------------------------------------------------------------------
// Handle the Editor changing values in the inspector
//----------------------------------------------------------------------
#if UNITY_EDITOR
#region Editor Runtime
private Bus previousDataBus;


new protected void OnValidate ()
    {
	var dataBus = this.dataBus as Bus;
	if (dataBus == null)
        {
        var gameObject = this.dataBus as GameObject;
        if (gameObject != null)
            {
            this.dataBus = gameObject.GetComponent <Bus> ();
            dataBus = this.dataBus as Bus;
            }
        else
            {
            this.dataBus = null;
            }
        }

    // if (!this.hasBeenEnabled)
        {
		// base.OnValidate ();
        return;
        }

    if (!object.ReferenceEquals (this.previousDataBus, dataBus) && !string.IsNullOrEmpty (this.dataKey))
        {
		if (this.previousDataBus != null)
			{
			// this.previousDataBus.Disconnect (this.dataKey, this.dataBusConnector);
			}
		this.previousDataBus = dataBus;
		if (dataBus != null)
			{
			// dataBus.Connect (this.dataKey, this.dataBusConnector);
			}
        }

	// base.OnValidate ();
    }

#endregion
#endif
}





}

}
