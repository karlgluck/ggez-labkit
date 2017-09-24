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

public static partial class Vector2Ext
{
public static Vector3 ToVector3 (this Vector2 self)
    {
    return new Vector3 (self.x, self.y, 0f);
    }

public static Vector3 ToVector3 (this Vector2 self, float z)
    {
    return new Vector3 (self.x, self.y, z);
    }

public static Vector3 ToVector3 (this Vector2 self, Vector3 sourceOfZ)
    {
    return new Vector3 (self.x, self.y, sourceOfZ.z);
    }

public static Vector3 ToVector3OnXZ (this Vector2 self)
    {
    return new Vector3 (self.x, 0f, self.y);
    }

public static Vector3 ToVector3OnXZ (this Vector2 self, float y)
    {
    return new Vector3 (self.x, y, self.y);
    }

public static Vector3 ToVector3OnXZ (this Vector2 self, Vector3 sourceOfY)
    {
    return new Vector3 (self.x, sourceOfY.y, self.y);
    }

public static Vector2 Rotate (this Vector2 self, float z)
    {
    z = z * Mathf.Deg2Rad;
    float cosz = Mathf.Cos(z);
    float sinz = Mathf.Sin(z);
    return new Vector2 (self.x * cosz - self.y * sinz, self.y * cosz + self.x * sinz);
    }

public static Vector2 InDirection (float z)
    {
    z = z * Mathf.Deg2Rad;
    return new Vector2 (Mathf.Cos(z), Mathf.Sin(z));
    }

public static Vector2 NormalizedOrZero (this Vector2 self, float minimumMagnitude)
    {
    float sqrMagnitude = self.sqrMagnitude;
    return (sqrMagnitude <= minimumMagnitude * minimumMagnitude)
            ? Vector2.zero
            : self / Mathf.Sqrt (sqrMagnitude);
    }

public static Vector2 Clamp (this Vector2 self, Vector2 min, Vector2 max)
    {
    return new Vector2 (
            Mathf.Min (max.x, Mathf.Max (min.x, self.x)),
            Mathf.Min (max.y, Mathf.Max (min.y, self.y))
            );
    }

public static float MagnitudeFast (this Vector2 self)
    {
    float dx = Mathf.Abs (self.x);
    float dy = Mathf.Abs (self.y);
    float max = Mathf.Max (dx, dy);
    float min = Mathf.Min (dx, dy);
    const float alpha = 0.96043387010f;
    const float beta = 0.39782473476f;
    return alpha * max + beta * min;
    }

public static float DistanceSquared (this Vector2 self, Vector2 v)
    {
    return (v - self).sqrMagnitude;
    }

public static float DistanceExact (this Vector2 self, Vector2 v)
    {
    return (v - self).magnitude;
    }

public static float DistanceFast (this Vector2 self, Vector2 v)
    {
    return (v - self).MagnitudeFast ();
    }

public static Vector2 DeltaTo (this Vector2 self, Vector2 v)
    {
    return v - self;
    }

public static bool DeltaToLineSegmentPerpendicular (this Vector2 self, Vector2 a, Vector2 b, ref Vector2 delta)
    {
    var lengthSquared = a.DistanceSquared (b);
    if (0 == lengthSquared)
        {
        return false;
        }
    float t = Vector2.Dot (a.DeltaTo (self), a.DeltaTo (b)) / (float)lengthSquared;
    if (t < 0.0f || t > 1.0f)
        {
        return false;
        }
    else
        {
        var projection = Vector2.Lerp (a, b, t);
        delta = self.DeltaTo (projection);
        return true;
        }
    }

public static float DistanceToLineSegmentFast (this Vector2 self, Vector2 a, Vector2 b)
    {
    var lengthSquared = a.DistanceSquared(b);
    if (Mathf.Approximately (0f, lengthSquared))
        {
        return self.DistanceFast (a);
        }
    float t = Vector2.Dot(a.DeltaTo (self), a.DeltaTo (b)) / (float)lengthSquared;
    if (t < 0.0f)
        {
        return self.DistanceFast (a);
        }
    else if (t > 1.0f)
        {
        return self.DistanceFast (b);
        }
    else
        {
        var projection = Vector2.Lerp (a, b, t);
        return self.DistanceFast (projection);
        }
    }

public static float DistanceToLineSegmentSquared (this Vector2 self, Vector2 a, Vector2 b)
    {
    var lengthSquared = a.DistanceSquared(b);
    if (0 == lengthSquared)
        {
        return self.DistanceSquared (a);
        }
    float t = Vector2.Dot(a.DeltaTo(self), a.DeltaTo(b)) / (float)lengthSquared;
    if (t < 0.0f)
        {
        return self.DistanceSquared (a);
        }
    else if (t > 1.0f)
        {
        return self.DistanceSquared (b);
        }
    else
        {
        var projection = Vector2.Lerp(a, b, t);
        return self.DistanceSquared(projection);
        }
    }

public static float DistanceToLineSegmentExact (this Vector2 self, Vector2 a, Vector2 b)
    {
    return (float)Mathf.Sqrt (self.DistanceToLineSegmentSquared (a, b));
    }
}