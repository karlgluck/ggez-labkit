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
//----------------------------------------------------------------------
[Serializable, CreateAssetMenu (fileName = "New Bool Register Table.asset", menuName="GGEZ/Game Register/[string] -> bool")]
public class BoolRegisterTable : GameRegisterTable
{

[SerializeField] private bool initialValue;

private Dictionary<string, bool> table = new Dictionary<string, bool>();

void Awake ()
    {
    }

#if UNITY_EDITOR
void Reset ()
    {
    this.table = new Dictionary<string, bool>();
    }

public void SetDirty ()
    {
    this.dirty = true;
    }

private bool dirty = false;

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
[SerializeField] private bool runtimeValue;
public bool Value
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




private List<GameRegisterBoolListener> listeners = new List<GameRegisterBoolListener>();





public void RegisterListener (GameRegisterBoolListener listener)
    {
    if (listener == null)
        {
        throw new ArgumentNullException ("listener");
        }
    listener.OnDidChange (this.runtimeValue);
    this.listeners.Add (listener);
    }




public void UnregisterListener (GameRegisterBoolListener listener)
    {
    if (listener == null)
        {
        throw new ArgumentNullException ("listener");
        }
    this.listeners.Remove (listener);
    }



}




}
