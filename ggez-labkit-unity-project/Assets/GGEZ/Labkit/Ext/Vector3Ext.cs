using UnityEngine;
using System.Collections;

public static partial class Vector3Ext
{
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

public static Vector2 NormalizedOrZero (this Vector3 self, float minimumMagnitude)
    {
    float sqrMagnitude = self.sqrMagnitude;
    return (sqrMagnitude <= minimumMagnitude * minimumMagnitude)
            ? Vector3.zero
            : self / Mathf.Sqrt (sqrMagnitude);
    }
}
