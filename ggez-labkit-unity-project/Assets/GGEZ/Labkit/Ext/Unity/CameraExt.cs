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

namespace GGEZ
{
public static partial class CameraExt
{
public static Rect ScreenRect (this Camera self)
    {
    var screenSize = self.ViewportToScreenPoint (Vector3.one);
    return new Rect (0f, 0f, screenSize.x, screenSize.y);
    }

public static void GizmosDrawFrustum (Camera self)
    {
    GizmosDrawFrustum (self, self.farClipPlane);
    }

public static void GizmosDrawFrustum (this Camera self, float farPlane)
    {
    Matrix4x4 previous = Gizmos.matrix;
    Gizmos.matrix = Matrix4x4.TRS (self.transform.position, self.transform.rotation, Vector3.one);
    if (self.orthographic)
        {
        float spread = farPlane - self.nearClipPlane;
        float center = (farPlane + self.nearClipPlane) * 0.5f;
        Gizmos.DrawWireCube (new Vector3 (0,0,center), new Vector3 (self.orthographicSize * 2 * self.aspect, self.orthographicSize * 2, spread));
        }
    else
        {
        Gizmos.DrawFrustum (Vector3.zero, self.fieldOfView, farPlane, self.nearClipPlane, self.aspect);
        }
    Gizmos.matrix = previous;
    }
}
}
