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

namespace GGEZ
{
namespace Omnibus
{


[
Serializable,
AddComponentMenu ("GGEZ/Omnibus/Router/Bus in Parent Router")
]
public class BusInParentRouter : MonoBehaviour
{

#region Programming Interface

public void Route (Bus bus)
    {
    var list = this.cells.Cells;
    for (int i = 0; i < list.Count; ++i)
        {
        var gameObject = list[i];
        var cells = gameObject.GetComponents (typeof (ICell));
#if UNITY_EDITOR
        if (cells.Length == 0)
            {
            Debug.LogWarning ("GameObject `" + gameObject.name + "` has no cells for fub `" + this.name + "` to assign");
            }
#endif
        for (int j = 0; j < cells.Length; ++j)
            {
            var cell = (ICell)cells[j];
            cell.Route (this.port, bus);
            }
        }
    }

#endregion

[SerializeField] private string port;
[SerializeField] private CellsList cells = new CellsList ();

void Awake ()
    {
    var bus = this.gameObject.GetComponentInParent <Bus> ();
    if (bus != null)
        {
        this.Route (bus);
        }
    }

}



}

}
