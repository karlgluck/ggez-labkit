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
        public static Rect ScreenRect(this Camera self)
        {
            var screenSize = self.ViewportToScreenPoint(Vector3.one);
            return new Rect(0f, 0f, screenSize.x, screenSize.y);
        }

        public static void GizmosDrawFrustum(Camera self)
        {
            GizmosDrawFrustum(self, self.farClipPlane);
        }

        public static void GizmosDrawFrustum(this Camera self, float farPlane)
        {
            Matrix4x4 previous = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(self.transform.position, self.transform.rotation, Vector3.one);
            if (self.orthographic)
            {
                float spread = farPlane - self.nearClipPlane;
                float center = (farPlane + self.nearClipPlane) * 0.5f;
                Gizmos.DrawWireCube(new Vector3(0, 0, center), new Vector3(self.orthographicSize * 2 * self.aspect, self.orthographicSize * 2, spread));
            }
            else
            {
                Gizmos.DrawFrustum(Vector3.zero, self.fieldOfView, farPlane, self.nearClipPlane, self.aspect);
            }
            Gizmos.matrix = previous;
        }

        /// <summary>
        /// Used with CameralessViewportToWorldPoint to turn a viewport point into a world point without using a Unity camera.
        /// Unity does a lot of weird stuff with its camera coordinates that make it really hard to reproduce this behavior.
        /// </summary>
        private static Matrix4x4 CameralessViewportToWorldPointMatrix(float fieldOfView, float aspect, Vector3 from, Vector3 to, Vector3 up)
        {
            // The zNear / zFar magic numbers are what Unity's viewport always uses regardless of camera settings
            var matP = Matrix4x4.Perspective(fieldOfView, aspect, /* zNear: */ 0.6f, /* zFar: */ 1000f);

            // For some reason we have to reverse these coordinates
            from = -from;
            to = -to;

            var matT = Matrix4x4.Translate(from);
            var matR = Matrix4x4.Rotate(Quaternion.LookRotation(to - from, up));

            var invMatVP = (s_unityViewportMatrix * matP * matR.transpose * matT).inverse;
            return invMatVP;
        }

        /// <summary>
        /// Turns a viewport point into a world point without using a Unity camera. The cameralessMatrix parameter is
        /// obtained by calling CameralessViewportToWorldPointMatrix using the camera's parameters.
        /// </summary>
        private static Vector3 CameralessViewportToWorldPoint(Vector3 point, Matrix4x4 cameralessMatrix)
        {
            // Another set of magic numbers!
            const float zNear = 1.2f;
            const float zFar = 1000f;

            // Convert the point's Z coordinate into depth-buffer space. Why? Who knows.
            float x = point.x;
            float y = point.y;
            float z = 0.5f * ((zFar + zNear - 2 * zNear * zFar / point.z) / (zFar - zNear) + 1f);
            return cameralessMatrix.MultiplyPoint(new Vector3(x, y, z));
        }

        // Unity camera uses a weird viewport in the range [0f, 1f] and flipped on X
        private static readonly Matrix4x4 s_unityViewportMatrix = Matrix4x4.TRS(new Vector3(0.5f, 0.5f, 0f), Quaternion.identity, new Vector3(-0.5f, 0.5f, 1f));
    }
}
