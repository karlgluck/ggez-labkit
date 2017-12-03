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



public partial interface IBus
{

void Set (string key, bool value);
void Trigger (string key, bool value);
bool Get (string key, out bool value);
bool GetBool (string key, bool defaultValue);

void SetTrue (string key);
void SetFalse (string key);
void TriggerTrue (string key);
void TriggerFalse (string key);

}


public sealed partial class Bus
{

public void Set (string key, bool value) { this.set <bool> (key, value); }
public void Trigger (string key, bool value) { this.trigger<bool> (key, value); }
public bool Get (string key, out bool value) { return this.get<bool> (key, out value); }
public bool GetBool (string key, bool defaultValue) { return this.getT<bool> (key, defaultValue); }

public void SetTrue (string key) { this.set<bool> (key, true); }
public void SetFalse (string key) { this.set<bool> (key, false); }
public void TriggerTrue (string key) { this.trigger<bool> (key, true); }
public void TriggerFalse (string key) { this.trigger<bool> (key, false); }
}

public sealed partial class BusAsset
{

public void Set (string key, bool value) { this.set <bool> (key, value); }
public void Trigger (string key, bool value) { this.trigger<bool> (key, value); }
public bool Get (string key, out bool value) { return this.get<bool> (key, out value); }
public bool GetBool (string key, bool defaultValue) { return this.getT<bool> (key, defaultValue); }

public void SetTrue (string key) { this.set<bool> (key, true); }
public void SetFalse (string key) { this.set<bool> (key, false); }
public void TriggerTrue (string key) { this.trigger<bool> (key, true); }
public void TriggerFalse (string key) { this.trigger<bool> (key, false); }

}

public sealed partial class SerializedMemoryCell
{

public bool Value_Boolean;

}



}

}
