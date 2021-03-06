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

namespace GGEZ
{
    public static partial class Vector2Ext
    {
        public static Vector3 ToVector3(this Vector2 self)
        {
            return new Vector3(self.x, self.y, 0f);
        }

        public static Vector3 ToVector3(this Vector2 self, float z)
        {
            return new Vector3(self.x, self.y, z);
        }

        public static Vector3 ToVector3(this Vector2 self, Vector3 sourceOfZ)
        {
            return new Vector3(self.x, self.y, sourceOfZ.z);
        }

        public static Vector3 ToVector3OnXZ(this Vector2 self)
        {
            return new Vector3(self.x, 0f, self.y);
        }

        public static Vector3 ToVector3OnXZ(this Vector2 self, float y)
        {
            return new Vector3(self.x, y, self.y);
        }

        public static Vector3 ToVector3OnXZ(this Vector2 self, Vector3 sourceOfY)
        {
            return new Vector3(self.x, sourceOfY.y, self.y);
        }

        public static Vector2 Rotate(this Vector2 self, float z)
        {
            z = z * Mathf.Deg2Rad;
            float cosz = Mathf.Cos(z);
            float sinz = Mathf.Sin(z);
            return new Vector2(self.x * cosz - self.y * sinz, self.y * cosz + self.x * sinz);
        }

        public static Vector2 InDirection(float z)
        {
            z = z * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(z), Mathf.Sin(z));
        }

        public static Vector2 NormalizedOrZero(this Vector2 self, float minimumMagnitude)
        {
            float magnitude = self.magnitude;
            return (magnitude <= minimumMagnitude) ? Vector2.zero : self / magnitude;
        }

        public static Vector2 Clamp(this Vector2 self, Vector2 min, Vector2 max)
        {
            return new Vector2(
                    Mathf.Min(max.x, Mathf.Max(min.x, self.x)),
                    Mathf.Min(max.y, Mathf.Max(min.y, self.y))
                    );
        }

        public static Vector2 Midpoint(Vector2 a, Vector2 b)
        {
            return new Vector2() { x = (a.x + b.x) / 2, y = (a.y + b.y) / 2 };
        }

        public static float Distance(this Vector2 self, Vector2 v)
        {
            return (v - self).magnitude;
        }

        public static float DistanceSquared(this Vector2 self, Vector2 v)
        {
            return (v - self).sqrMagnitude;
        }

        public static Vector2 DeltaTo(this Vector2 self, Vector2 v)
        {
            return v - self;
        }

        public static Vector2 DeltaFrom(this Vector2 self, Vector2 v)
        {
            return self - v;
        }

        public static bool DeltaToLineSegmentPerpendicular(this Vector2 self, Vector2 a, Vector2 b, ref Vector2 delta)
        {
            var lengthSquared = a.DistanceSquared(b);
            if (0 == lengthSquared)
            {
                return false;
            }
            float t = Vector2.Dot(a.DeltaTo(self), a.DeltaTo(b)) / (float)lengthSquared;
            if (t < 0.0f || t > 1.0f)
            {
                return false;
            }
            else
            {
                var projection = Vector2.Lerp(a, b, t);
                delta = self.DeltaTo(projection);
                return true;
            }
        }




        public static float DistanceToLineSegment(this Vector2 self, Vector2 a, Vector2 b)
        {
            var lengthSquared = a.Distance(b);
            if (0 == lengthSquared)
            {
                return self.Distance(a);
            }
            float t = Vector2.Dot(a.DeltaTo(self), a.DeltaTo(b)) / (float)lengthSquared;
            if (t < 0.0f)
            {
                return self.Distance(a);
            }
            else if (t > 1.0f)
            {
                return self.Distance(b);
            }
            else
            {
                var projection = Vector2.Lerp(a, b, t);
                return self.Distance(projection);
            }
        }



        public static float DistanceToLineSegmentSquared(this Vector2 self, Vector2 a, Vector2 b)
        {
            var lengthSquared = a.DistanceSquared(b);
            if (0 == lengthSquared)
            {
                return self.DistanceSquared(a);
            }
            float t = Vector2.Dot(a.DeltaTo(self), a.DeltaTo(b)) / (float)lengthSquared;
            if (t < 0.0f)
            {
                return self.DistanceSquared(a);
            }
            else if (t > 1.0f)
            {
                return self.DistanceSquared(b);
            }
            else
            {
                var projection = Vector2.Lerp(a, b, t);
                return self.DistanceSquared(projection);
            }
        }

        /// <summary>
        /// Returns whether or not this point is inside the given polygon
        /// </summary>
        /// <param name="polygonBoundary">Points of polygon to test</param>
        public static bool IsInside(this Vector2 self, Vector2[] polygonBoundary)
        {
            int i = 0, j = 1;
            int retval = 0;
            while (j < polygonBoundary.Length)
            {
                var pointI = polygonBoundary[i];
                var pointJ = polygonBoundary[j];
                float side = (pointJ.x - pointI.x) * (self.y - pointI.y) - (self.x - pointI.x) * (pointJ.y - pointI.y);
                int upward = ((side > 0) & (pointI.y <= self.y) & (pointJ.y > self.y)) ? +1 : 0;
                int downward = ((side < 0) & (pointI.y > self.y) & (pointJ.y <= self.y)) ? -1 : 0;
                ++i;
                ++j;
                retval += upward;
                retval += downward;
            }
            return retval != 0;
        }

        /// <summary>
        /// Turns a Vector2 representing a pair of arbitrary angles into a pair
        /// where each member is in the range [-180, 180]
        /// </summary>
        public static Vector2 ToUnwrappedAngles(this Vector2 self)
        {
            return new Vector2(Mathf.DeltaAngle(0, self.x), Mathf.DeltaAngle(0f, self.y));
        }

        /// <summary>
        /// Converts a displacement amount to a Z-axis rotation in degrees.
        /// </summary>
        public static float ToDirection(this Vector2 self)
        {
            return Mathf.Rad2Deg * Mathf.Atan2(self.y, self.x);
        }

        /// <summary>
        /// Converts a displacement amount to a Z-axis rotation in degrees.
        /// </summary>
        /// <param name="valueIfZeroMagnitude">Value to return if the vector has no length</param>
        public static float ToDirection(this Vector2 self, float valueIfZeroMagnitude)
        {
            if (Mathf.Approximately(0f, self.sqrMagnitude))
            {
                return valueIfZeroMagnitude;
            }
            return ToDirection(self);
        }

        /// <summary>
        /// Interpolates on a frame-rate independent exponential curve.
        /// </summary>
        /// <param name="current">Current value of the parameter being damped</param>
        /// <param name="target">Desired value of the parameter</param>
        /// <param name="smoothing">What proportion of current remains after 1 second. Range [0, 1]</param>
        /// <param name="dt">Timestep, usually Time.deltaTime or Time.smoothDeltaTime</param>
        public static Vector2 Damp(Vector2 current, Vector2 target, float smoothing, float dt)
        {
            float t = 1 - Mathf.Pow(smoothing, dt);
            return new Vector2(
                    Mathf.Lerp(current.x, target.x, t),
                    Mathf.Lerp(current.y, target.y, t)
                    );
        }
    }
}
