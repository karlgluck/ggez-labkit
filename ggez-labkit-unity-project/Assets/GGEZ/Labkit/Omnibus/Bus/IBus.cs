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
using StringCollection = System.Collections.Generic.ICollection<string>;

namespace GGEZ
{
namespace Omnibus
{

public partial interface IBus
{
void Connect (string key, IFub fub);
void Disconnect (string key, IFub fub);

void Trigger (string key);
void Set (string key, object value);
void Trigger (string key, object value);
object Get (string key);
object Get (string key, object defaultValue);
bool Get (string key, out object value);
void Unset (string key);
void SetNull (string key);

bool HasConnections (string key);
bool HasValue (string key);

StringCollection GetAllKeys ();
StringCollection GetConnectedKeys ();
StringCollection GetEventKeys ();
StringCollection GetMemoryKeys ();


}






}
}
