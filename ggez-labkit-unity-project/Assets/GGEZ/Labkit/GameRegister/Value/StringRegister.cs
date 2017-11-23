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

namespace GGEZ
{



//----------------------------------------------------------------------
//----------------------------------------------------------------------
[Serializable, CreateAssetMenu (fileName = "New String Register.asset", menuName="GGEZ/Game Register/string Register")]
public class StringRegister : GameRegister
{

[SerializeField] private string initialValue;

void Awake ()
    {
    this.runtimeValue = this.initialValue;
    }

#if UNITY_EDITOR
void Reset ()
    {
    this.listeners = new List<StringRegisterListener>();
    this.runtimeValue = this.initialValue;
    }

void OnValidate ()
    {
    if (Application.isPlaying)
        {
        var value = this.runtimeValue;
        for (int i = this.listeners.Count - 1; i >= 0; --i)
            {
            this.listeners[i].OnDidChange (value);
            }
        }
    else
        {
        this.runtimeValue = this.initialValue;
        }
    }
#endif

#region Runtime
[SerializeField] private string runtimeValue;
public string Value
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




private List<StringRegisterListener> listeners = new List<StringRegisterListener>();





public void RegisterListener (StringRegisterListener listener)
    {
    if (listener == null)
        {
        throw new ArgumentNullException ("listener");
        }
    this.listeners.Add (listener);
    listener.OnDidChange (this.runtimeValue);
    }




public void UnregisterListener (StringRegisterListener listener)
    {
    if (listener == null)
        {
        throw new ArgumentNullException ("listener");
        }
    this.listeners.Remove (listener);
    }



}




}
