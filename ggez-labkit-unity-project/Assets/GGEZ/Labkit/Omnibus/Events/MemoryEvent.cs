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

namespace GGEZ
{
namespace Omnibus
{





[
Serializable,
AddComponentMenu ("GGEZ/Omnibus/Cells/Latch to Bus Memory Cell")
]
public sealed class LatchToBusSignalCell : Cell
{

public Bus InputBus
    {
    get { return this.inputBus; }
    set
        {
        this.inputBus = value;
        this.refresh ();
        }
    }

public string InputPin
    {
    get { return this.inputPin; }
    set
        {
        this.inputPin = value;
        this.refresh ();
        }
    }

public Bus OutputBus
    {
    get { return this.outputBus; }
    set
        {
        this.outputBus = value;
        this.refresh ();
        }
    }

public string OutputPin
    {
    get { return this.outputPin; }
    set
        {
        this.outputPin = value;
        this.refresh ();
        }
    }


[SerializeField] private Bus inputBus;
[SerializeField] private string inputPin;
[SerializeField] private Bus outputBus;
[SerializeField] private string outputPin;

public override void OnDidSignal (string pin, object value)
    {
    Debug.Assert (this.outputBus != null && Pin.IsValid (this.outputPin));
    this.outputBus.SetObject (this.outputPin, value);
    }

public override void Route (string net, Bus bus)
    {
    switch (net)
        {
        case "in":  this.InputBus = bus; break;
        case "out": this.OutputBus = bus; break;
        default:
            {
            this.InputBus = bus;
            this.OutputBus = bus;
            break;
            }
        }
    }

private Wire input = Wire.CELL_IN;

void OnEnable ()
    {
    this.input.Attach (this, this.inputBus, this.inputPin);
    }

void OnDisable ()
    {
    this.input.Detach ();
    }

void OnValidate ()
    {
    this.refresh ();
    }

private void refresh ()
    {
    if (this.outputBus == null || Pin.IsInvalid (this.outputPin))
        {
        this.input.Disconnect ();
        }
    else
        {
        this.input.Connect (this.inputBus, this.inputPin);
        }
    }

}

}

}
