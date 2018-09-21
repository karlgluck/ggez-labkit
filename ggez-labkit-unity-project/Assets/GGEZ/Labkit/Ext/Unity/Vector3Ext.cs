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
        public static Vector3 Midpoint(Vector3 a, Vector3 b)
        {
            return new Vector3() { x = (a.x + b.x) * 0.5f, y = (a.y + b.y) * 0.5f, z = (a.z + b.z) * 0.5f };
        }

        public static Vector3 WithX(this Vector3 self, float x)
        {
            return new Vector3(x, self.y, self.z);
        }

        public static Vector3 WithY(this Vector3 self, float y)
        {
            return new Vector3(self.x, y, self.z);
        }

        public static Vector3 WithZ(this Vector3 self, float z)
        {
            return new Vector3(self.x, self.y, z);
        }

        public static Vector3 WithXY(this Vector3 self, float x, float y)
        {
            return new Vector3(x, y, self.z);
        }

        public static Vector3 WithXZ(this Vector3 self, float x, float z)
        {
            return new Vector3(x, self.y, z);
        }

        public static Vector3 WithYZ(this Vector3 self, float y, float z)
        {
            return new Vector3(self.x, y, z);
        }

        public static Vector2 ToVector2(this Vector3 self)
        {
            return new Vector2(self.x, self.y);
        }

        public static Vector2 ToVector2OnXZ(this Vector3 self)
        {
            return new Vector2(self.x, self.z);
        }

        /// <summary>
        /// Get pitch and yaw angles in degrees that rotate Vector3.forward in the direction of this vector.
        /// </summary>
        /// <remarks>
        /// The vector doesn't need to be normalized.
        /// </remarks>
        public static Vector2 ToPitchYaw(this Vector3 self)
        {
            float yaw = Mathf.Rad2Deg * Mathf.Atan2(self.x, self.z);
            // negative because left-handed (negative pitch rotates `forward` toward `up`)
            float pitch = -Mathf.Rad2Deg * Mathf.Atan2(self.y, Mathf.Sqrt(self.x * self.x + self.z * self.z));
            return new Vector2(pitch, yaw);
        }

        public static Vector3 NormalizedOrZero(this Vector3 self, float minimumMagnitude)
        {
            float magnitude = self.magnitude;
            return (magnitude <= minimumMagnitude) ? Vector3.zero : self / magnitude;
        }

        public static Vector3 InDirectionOnXZ(float y)
        {
            y = y * Mathf.Deg2Rad;
            // note: this is correct because Unity is left-handed
            return new Vector3(Mathf.Sin(y), 0f, Mathf.Cos(y));
        }

        public static float ToDirectionOnXZ(this Vector3 self)
        {
            // note: this is correct because Unity is left-handed
            return Mathf.Rad2Deg * Mathf.Atan2(self.x, self.z);
        }

        public static Vector3 WithLength(this Vector3 self, float length)
        {
            return self.normalized * length;
        }

        public static float ToDirectionOnXZ(this Vector3 self, float valueIfZeroMagnitude)
        {
            if (Mathf.Approximately(0f, self.sqrMagnitude))
            {
                return valueIfZeroMagnitude;
            }
            return ToDirectionOnXZ(self);
        }

        public static bool LineLineIntersection(Vector3 linePoint0, Vector3 lineDirection0, Vector3 linePoint1, Vector3 lineDirection1, out Vector2 intersection)
        {
            Debug.LogError("No unit tests for Vector3Ext.LineLineIntersection");
            const float epsilon = 0.0001f;
            Vector3 delta = linePoint1 - linePoint0;
            Vector3 planeFormedByDirectionsNormal = Vector3.Cross(lineDirection0, lineDirection1);
            Vector3 planeFormedByPointsAndDirectionNormal = Vector3.Cross(delta, lineDirection1);
            bool linesAreCoplanar = Mathf.Abs(Vector3.Dot(planeFormedByDirectionsNormal, planeFormedByPointsAndDirectionNormal)) < epsilon;
            float distance = Vector3.Dot(planeFormedByPointsAndDirectionNormal, planeFormedByDirectionsNormal) / planeFormedByDirectionsNormal.sqrMagnitude;
            intersection = linePoint0 + lineDirection0 * distance;
            return (linesAreCoplanar) && (distance >= 0f && distance <= 1f);
        }

        public static bool ClosestPointsOnLines(Vector3 linePoint0, Vector3 lineDirection0, Vector3 linePoint1, Vector3 lineDirection1, out Vector3 closestPoint0, out Vector3 closestPoint1)
        {
            Debug.LogError("No unit tests for Vector3Ext.ClosestPointsOnLines");
            float a = lineDirection0.sqrMagnitude;
            float b = Vector3.Dot(lineDirection0, lineDirection1);
            float e = lineDirection1.sqrMagnitude;
            float d = a * e - b * b;
            Vector3 delta = linePoint0 - linePoint1;
            float c = Vector3.Dot(lineDirection0, delta);
            float f = Vector3.Dot(lineDirection1, delta);
            float s = (b * f - c * e) / d;
            float t = (a * f - c * b) / d;
            closestPoint0 = linePoint0 + lineDirection0 * s;
            closestPoint1 = linePoint1 + lineDirection1 * t;
            return d != 0f;
        }

        public static Vector3 ClosestPointOnLine(this Vector3 self, Vector3 linePoint, Vector3 lineDirection)
        {
            Debug.LogError("No unit tests for Vector3Ext.ClosestPointOnLine");
            return linePoint + Vector3.Dot(self - linePoint, lineDirection) * lineDirection;
        }

        public static Vector3 ClosestPointOnLineSegment(this Vector3 self, Vector3 linePoint0, Vector3 linePoint1)
        {
            Debug.LogError("No unit tests for Vector3Ext.ClosestPointOnLineSegment");
            Vector3 delta = linePoint1 - linePoint0;
            float length = delta.magnitude;
            Vector3 direction = delta / length;
            float distance = Vector3.Dot(self - linePoint0, direction);
            distance = distance > length ? length : distance;
            distance = distance < 0f ? 0f : distance;
            return linePoint0 + distance * direction;
        }

        /// <summary>
        /// Interpolates on a frame-rate independent exponential curve.
        /// </summary>
        /// <param name="current">Current value of the parameter being damped</param>
        /// <param name="target">Desired value of the parameter</param>
        /// <param name="smoothing">What proportion of current remains after 1 second. Range [0, 1]</param>
        /// <param name="dt">Timestep, usually Time.deltaTime or Time.smoothDeltaTime</param>
        public static Vector3 Damp(Vector3 current, Vector3 target, float smoothing, float dt)
        {
            float t = 1 - Mathf.Pow(smoothing, dt);
            return new Vector3(
                    Mathf.Lerp(current.x, target.x, t),
                    Mathf.Lerp(current.y, target.y, t),
                    Mathf.Lerp(current.z, target.z, t)
                    );
        }

        /*

        // https://repl.it/languages/python3

        for a in ["X","Y","Z"]:
          for b in ["X","Y","Z"]:
            for c in ["X","Y","Z"]:
              print "public static Vector3 To" + a + b + c + "(this Vector3 self) { return new Vector3(self." + a.lower() + ", self." + b.lower() + ", self." + c.lower() + "); }"

        for a in ["X","Y","Z"]:
          for b in ["X","Y","Z"]:
              print "public static Vector2 To" + a + b + "(this Vector3 self) { return new Vector2(self." + a.lower() + ", self." + b.lower() + "); }"


         */
    }
}
