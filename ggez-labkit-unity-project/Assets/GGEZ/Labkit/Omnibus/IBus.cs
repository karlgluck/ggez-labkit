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

public interface IBus
{
bool HasListeners (string key);
bool HasValue (string key);
StringCollection GetEventKeys ();
StringCollection GetRegisterKeys ();

void RegisterListener (string key, Fub listener);
void UnregisterListener (string key, Fub listener);


// ----[ void ]----------------------------------------------------------------
void Trigger (string key);

// ----[ object ]--------------------------------------------------------------
object Get (string key);


// ----[ int ]-----------------------------------------------------------------
void Set (string key, int value);
void Trigger (string key, int value);
bool Get (string key, out int value);
int GetInt (string key, int defaultValue);


// ----[ bool ]----------------------------------------------------------------
void Set (string key, bool value);
void Trigger (string key, bool value);
bool Get (string key, out bool value);
bool GetBool (string key, bool defaultValue);


// ----[ string ]--------------------------------------------------------------
void Set (string key, string value);
void Trigger (string key, string value);
bool Get (string key, out string value);
string GetString (string key, string defaultValue);


// ----[ float ]---------------------------------------------------------------
void Set (string key, float value);
void Trigger (string key, float value);
bool Get (string key, out float value);
float GetFloat (string key, float defaultValue);

}






}
}
