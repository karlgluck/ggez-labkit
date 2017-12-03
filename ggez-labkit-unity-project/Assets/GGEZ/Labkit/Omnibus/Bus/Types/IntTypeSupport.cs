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

void Set (string key, int value);
void Trigger (string key, int value);
bool Get (string key, out int value);
int GetInt (string key, int defaultValue);

}


public sealed partial class Bus
{

public void Set (string key, int value) { this.set <int> (key, value); }
public void Trigger (string key, int value) { this.trigger<int> (key, value); }
public bool Get (string key, out int value) { return this.get<int> (key, out value); }
public int GetInt (string key, int defaultValue) { return this.getT<int> (key, defaultValue); }

}

public sealed partial class BusAsset
{

public void Set (string key, int value) { this.set <int> (key, value); }
public void Trigger (string key, int value) { this.trigger<int> (key, value); }
public bool Get (string key, out int value) { return this.get<int> (key, out value); }
public int GetInt (string key, int defaultValue) { return this.getT<int> (key, defaultValue); }

}

public sealed partial class SerializedMemoryCell
{

public int Value_Int32;

}



}

}
