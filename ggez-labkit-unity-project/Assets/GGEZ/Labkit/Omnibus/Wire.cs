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

namespace GGEZ.Omnibus
{

public class Wire
{

// Generators for wires that connect to common pin names
public static Wire CELL_INPUT { get { return new Wire (Pin.INPUT); } }
public static Wire CELL_DATA { get { return new Wire (Pin.DATA); } }
public static Wire CELL_SELECT { get { return new Wire (Pin.SELECT); } }
public static Wire CELL_ENABLE { get { return new Wire (Pin.ENABLE); } }

public Wire (string cellPin)
    {
    this.CellPin = cellPin;
    }

public Wire (string cellPin, string busPin)
    {
    this.CellPin = cellPin;
    this.BusPin = busPin;
    }

public Wire (string cellPin, Bus bus, string busPin)
    {
    this.CellPin = cellPin;
    this.Bus = bus;
    this.BusPin = busPin;
    }

public bool IsConnected { get { return this.IsAttached && this.Bus != null && Pin.IsValid (this.BusPin); } }
public bool IsAttached { get { return this.Cell != null; } }
public ICell Cell { get; private set; }
public string CellPin { get; private set; }
public Bus Bus { get; private set; }
public string BusPin { get; private set; }


// Usually called in OnEnable of a Cell. It is okay to attach to a
// null bus or bus-pin. The result will be that Connect will
// connect the cell automatically should these values become valid.
public void Attach (ICell cell, Bus bus, string busPin)
    {
    if (this.IsAttached)
        {
        throw new InvalidOperationException ("already attached");
        }
    if (cell == null)
        {
        throw new ArgumentNullException ("cell");
        }
    this.Cell = cell;
    this.Bus = bus;
    this.BusPin = busPin;
    if (bus != null && Pin.IsValid (busPin))
        {
        bus.Connect (this);
        }
    }

// These variants will not change values that are not provided.
// This can be useful if you might have called Connect in the
// past, or initialized the Wire with a bus and/or bus pin.

public void Attach (ICell cell)
    {
    this.Attach (cell, this.Bus, this.BusPin);
    }

public void Attach (ICell cell, Bus bus)
    {
    this.Attach (cell, bus, this.BusPin);
    }

public void Attach (ICell cell, string busPin)
    {
    this.Attach (cell, this.Bus, busPin);
    }

// Usually called in OnDisable from a Cell. Once called, Connect
// has no effect on the IsConnected status.
public void Detach ()
    {
    if (this.IsAttached && this.Bus != null && Pin.IsValid (this.BusPin))
        {
        this.Bus.Disconnect (this);
        }
    this.Cell = null;
    }

// Connect can be called with a null bus or invalid pin. If calling
// Connect results in a connectable Wire, what happens is based on
// the value of IsAttached:
//   - If true, Connect will connect the cell to the bus
//   - If false, Connect will store the values internally
// In either case, any current connection will be removed.
//
// Calling versions of Connect without the bus and/or pin parameter(s)
// will maintain the current value of those parameter(s).

public void Connect (Bus bus, string busPin)
    {
    if (object.ReferenceEquals (this.Bus, bus) && this.BusPin == busPin)
        {
        return;
        }
    if (this.IsAttached && this.Bus != null && Pin.IsValid (this.BusPin))
        {
        this.Bus.Disconnect (this);
        }
    this.Bus = bus;
    this.BusPin = busPin;
    if (this.IsAttached && this.Bus != null && Pin.IsValid (this.BusPin))
        {
        this.Bus.Connect (this);
        }
    }

public void Connect (Bus bus)
    {
    this.Connect (bus, this.BusPin);
    }

public void Connect (string busPin)
    {
    this.Connect (this.Bus, busPin);
    }

public void Connect ()
    {
    this.Connect (this.Bus, this.BusPin);
    }

public void Disconnect ()
    {
    this.Connect (null, null);
    }


// The Bus sends values using the Signal method. The editor implementation
// tracks references and will stop signaling if we reach a depth that is
// likely to mean we have an infinte loop.

#if UNITY_EDITOR

private static int recursions = 0;
private const int MaxRecursionDepth = 2048;

public void Signal (object value)
    {
    if (++recursions > MaxRecursionDepth)
        {
        throw new System.InvalidProgramException ("infinite signal feedback loop");
        }
    this.Cell.OnDidSignal (this.CellPin, value);
    --recursions;
    }

#else

public void Signal (object value)
    {
    this.Cell.OnDidSignal (this.CellPin, value);
    }

#endif

    

}


}
