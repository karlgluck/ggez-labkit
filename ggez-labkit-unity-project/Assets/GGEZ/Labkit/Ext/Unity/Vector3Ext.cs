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
using System.Collections;
using System;

namespace GGEZ
{
public static partial class Vector3Ext
{

public static Vector3 Midpoint (Vector3 a, Vector3 b)
    {
    return new Vector3 () { x = (a.x + b.x) / 2, y = (a.y + b.y) / 2, z = (a.z + b.z) / 2 };
    }

public static Vector3 WithX (this Vector3 self, float x)
    {
    return new Vector3 (x, self.y, self.z);
    }

public static Vector3 WithY (this Vector3 self, float y)
    {
    return new Vector3 (self.x, y, self.z);
    }

public static Vector3 WithZ (this Vector3 self, float z)
    {
    return new Vector3 (self.x, self.y, z);
    }

public static Vector3 WithXY (this Vector3 self, float x, float y)
    {
    return new Vector3 (x, y, self.z);
    }

public static Vector3 WithXZ (this Vector3 self, float x, float z)
    {
    return new Vector3 (x, self.y, z);
    }

public static Vector3 WithYZ (this Vector3 self, float y, float z)
    {
    return new Vector3 (self.x, y, z);
    }

public static Vector2 ToVector2 (this Vector3 self)
    {
    return new Vector2 (self.x, self.y);
    }

public static Vector2 ToVector2OnXZ (this Vector3 self)
    {
    return new Vector2 (self.x, self.z);
    }

public static Vector3 NormalizedOrZero (this Vector3 self, float minimumMagnitude)
    {
    float magnitude = self.magnitude;
    return (magnitude <= minimumMagnitude) ? Vector3.zero : self / magnitude;
    }

public static Vector3 InDirectionOnXZ (float y)
    {
    y = y * Mathf.Deg2Rad;
    // note: this is correct because Unity is left-handed
    return new Vector3 (Mathf.Sin (y), 0f, Mathf.Cos (y));
    }

public static float ToDirectionOnXZ (this Vector3 self)
    {
    // note: this is correct because Unity is left-handed
    return Mathf.Rad2Deg * Mathf.Atan2 (self.x, self.z);
    }

public static float ToDirectionOnXZ (this Vector3 self, float valueIfZeroMagnitude)
    {
    if (Mathf.Approximately (0f, self.sqrMagnitude))
        {
        return valueIfZeroMagnitude;
        }
    return ToDirectionOnXZ (self);
    }

}
}
