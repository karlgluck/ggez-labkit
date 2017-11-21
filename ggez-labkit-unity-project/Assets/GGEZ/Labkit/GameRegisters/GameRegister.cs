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
// This class makes use of boxing value types for simplicity. There is a
// performance cost, but the whole concept of registers as assets isn't
// meant to be screaming fast anyway. It's about getting things done!
//----------------------------------------------------------------------
[Serializable]
public class GameRegister : ScriptableObject
{
public float floatValue
    {
    set { this.Value = value; }
    get { return (float)this.Value; }
    }
public int intValue
    {
    set { this.Value = value; }
    get { return (int)this.Value; }
    }
public string stringValue
    {
    set { this.Value = value; }
    get { return (string)this.Value; }
    }
public GameObject gameObjectValue
    {
    set { this.Value = value; }
    get { return (GameObject)this.Value; }
    }
public Object unityObjectValue
    {
    set { this.Value = value; }
    get { return (Object)this.Value; }
    }

// Overridden by derived classes that have an `initialValue` member
protected virtual object getInitialValue ()
    {
    return null;
    }

void Awake ()
    {
    this.runtimeValue = this.getInitialValue ();
    }

void Reset ()
    {
    this.runtimeValue  = this.getInitialValue ();
    }

void OnValidate ()
    {
    if (!Application.isPlaying)
        {
        this.runtimeValue = this.getInitialValue ();
        }
    }

#region Runtime
protected object runtimeValue;
public object Value
    {
    get
        {
        return this.runtimeValue;
        }
    set
        {
        if (this.runtimeValue.Equals (value))
            {
            return;
            }
        this.runtimeValue = value;
        for (int i = this.listeners.Count - 1; i >= 0; --i)
            {
            this.listeners[i].OnDidChange (value);
            }
        }
    }
#endregion

private List<GameRegisterListener> listeners = new List<GameRegisterListener>();





public void RegisterListener (GameRegisterListener listener)
    {
    if (listener == null)
        {
        throw new ArgumentNullException ("listener");
        }
    this.listeners.Add (listener);
    }




public void UnregisterListener (GameRegisterListener listener)
    {
    if (listener == null)
        {
        throw new ArgumentNullException ("listener");
        }
    this.listeners.Remove (listener);
    }



}




}
