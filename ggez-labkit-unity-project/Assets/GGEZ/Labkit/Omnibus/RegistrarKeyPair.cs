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
public string Name = "";

public string Type = null;
public string Value_String;
public int Value_Int32;
public Vector3 Value_Vector3;
public bool Value_Boolean;
public float Value_Single;


public object GetValue ()
    {
    switch (this.Type)
        {
        case "String": return this.Value_String;
        case "Int32": return this.Value_Int32;
        case "Vector3": return this.Value_Vector3;
        case "Boolean": return this.Value_Boolean;
        case "Single": return this.Value_Single;
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
        case "Int32": this.Value_Int32 = (int)value; break;
        case "Vector3": this.Value_Vector3 = (Vector3)value; break;
        case "Boolean": this.Value_Boolean = (bool)value; break;
        case "Single": this.Value_Single = (float)value; break;
        }
    }

public static RegistrarKeyPair Create (string key, object value)
    {
    if (key == null)
        {
        throw new ArgumentNullException ("key");
        }
    var keyPair = new RegistrarKeyPair ();
    keyPair.Name = key;
    keyPair.SetValue (value);
    return keyPair;
    }
}

}
