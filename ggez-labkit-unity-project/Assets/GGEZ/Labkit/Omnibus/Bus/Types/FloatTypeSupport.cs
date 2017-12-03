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

void Set (string key, float value);
void Trigger (string key, float value);
bool Get (string key, out float value);
float GetFloat (string key, float defaultValue);

void Set0f (string key);
void Set1f (string key);
void Trigger0f (string key);
void Trigger1f (string key);

}


public sealed partial class Bus
{

public void Set (string key, float value) { this.set<float> (key, value); }
public void Trigger (string key, float value) { this.trigger<float> (key, value); }
public bool Get (string key, out float value) { return this.get<float> (key, out value); }
public float GetFloat (string key, float defaultValue) { return this.getT<float> (key, defaultValue); }


public void Set0f (string key) { this.set<float> (key, 0f); }
public void Set1f (string key) { this.set<float> (key, 1f); }
public void Trigger0f (string key) { this.trigger<float> (key, 0f); }
public void Trigger1f (string key) { this.trigger<float> (key, 1f); }

}

public sealed partial class BusAsset
{

public void Set (string key, float value) { this.set <float> (key, value); }
public void Trigger (string key, float value) { this.trigger<float> (key, value); }
public bool Get (string key, out float value) { return this.get<float> (key, out value); }
public float GetFloat (string key, float defaultValue) { return this.getT<float> (key, defaultValue); }

public void Set0f (string key) { this.set<float> (key, 0f); }
public void Set1f (string key) { this.set<float> (key, 1f); }
public void Trigger0f (string key) { this.trigger<float> (key, 0f); }
public void Trigger1f (string key) { this.trigger<float> (key, 1f); }

}

public sealed partial class SerializedMemoryCell
{

public float Value_Single;

}



}

}
