
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


[Serializable]
public class RegistrarKeyPair
{
[Delayed] public string Name = "";

public bool CanEditType = true;
public string Type = null;
public string Value_String;
public int Value_Int32;
public Vector3 Value_Vector3;


public object GetValue ()
    {
    switch (this.Type)
        {
        case "String": return this.Value_String;
        case "Int32": return this.Value_Int32;
        case "Vector3": return this.Value_Vector3;
        }
    return null;
    }

public void SetValue (object value)
    {
    if (value == null)
        {
        this.Type = "";
        return;
        }
    this.Type = value.GetType ().Name;
    switch (this.Type)
        {
        case "String": this.Value_String = (string)value; break;
        case "Int32":
            this.Value_Int32 = (int)value;
            break;
        case "Vector3":
            this.Value_Vector3 = (Vector3)value;
            break;
        }
    }

public static RegistrarKeyPair Create (string key, object value, bool canEditType)
    {
    if (key == null)
        {
        throw new ArgumentNullException ("key");
        }
    var keyPair = new RegistrarKeyPair ();
    keyPair.Name = key;
    keyPair.SetValue (value);
    keyPair.CanEditType = canEditType;
    return keyPair;
    }
}

}
