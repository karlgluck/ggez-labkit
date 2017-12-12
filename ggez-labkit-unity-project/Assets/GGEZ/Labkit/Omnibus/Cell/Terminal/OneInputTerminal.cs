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

namespace GGEZ.Omnibus
{


public abstract class OneInputTerminal<T> : MonoBehaviour, IOneInputTerminal
{

public void Signal (object value)
    {
    this.Signal (value);
    }

public abstract void Signal (T value);

public static void FindTerminal<D> (MonoBehaviour component, ref D terminal) where D : OneInputTerminal<T>
    {
    if (terminal != null)
        {
        if (terminal.gameObject == null || terminal.gameObject != component.gameObject)
            {
            terminal = null;
            }
        else
            {
            return;
            }
        }
    var discoveredTerminal = (D)component.gameObject.GetComponent (typeof(D));
    if (discoveredTerminal != null)
        {
        terminal = discoveredTerminal;
        }
    }

}


}
