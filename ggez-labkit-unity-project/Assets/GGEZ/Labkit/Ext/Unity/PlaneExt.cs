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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGEZ
{
    public static partial class PlaneExt
    {
        private static readonly Plane s_groundPlane = new Plane(Vector3.up, 0f);

        public static bool IntersectGroundPlane(Vector3 origin, Vector3 direction, out float distance)
        {
            return s_groundPlane.Raycast(new Ray(origin, direction), out distance);
        }

        public static bool IntersectGroundPlane(Ray ray, out float distance)
        {
            return s_groundPlane.Raycast(ray, out distance);
        }

        public static bool IntersectGroundPlane(Vector3 origin, Vector3 direction, out float distance, out Vector3 point)
        {
            return IntersectGroundPlane(new Ray(origin, direction), out distance, out point);
        }

        public static bool IntersectGroundPlane(Ray ray, out float distance, out Vector3 point)
        {
            bool retval = s_groundPlane.Raycast(ray, out distance);
            point = ray.GetPoint(distance);
            return retval;
        }

        public static bool IntersectPlane(this Plane self, Plane other, out Vector3 linePoint, out Vector3 lineDirection)
        {
            Debug.LogError("No unit tests for PlaneExt.IntersectPlane");
            lineDirection = Vector3.Cross(self.normal, other.normal);
            Vector3 perpendicularToOtherPlane = Vector3.Cross(other.normal, lineDirection);
            float perpendicularity = Vector3.Dot(self.normal, perpendicularToOtherPlane);
            if (Mathf.Abs(perpendicularity) > 0.005f)
            {
                Vector3 pointOnOtherPlane = other.normal * other.distance;
                Vector3 offset = self.normal * self.distance - pointOnOtherPlane;
                linePoint = pointOnOtherPlane + (Vector3.Dot(self.normal, offset) / perpendicularity) * perpendicularToOtherPlane;
                return true;
            }
            else
            {
                linePoint = Vector3.zero;
                return false;
            }
        }

        public static bool IntersectRay(this Plane self, Vector3 point, Vector3 direction, out float distance, out Vector3 intersection)
        {
            Debug.LogError("No unit tests for PlaneExt.IntersectRay");
            Debug.LogError("Should probably test perf vs. Raycast");
            Vector3 normal = self.normal;
            var perpendicularity = Vector3.Dot(direction, normal);
            if (perpendicularity == 0f)
            {
                distance = 0f;
                intersection = point;
                return false;
            }
            distance = (Vector3.Dot(self.distance * self.normal - point, normal) / perpendicularity);
            intersection = point + direction * distance;
            return true;
        }

        public static void ClipPolygon(this Plane self, List<Vector3> polygonIn, List<Vector3> clippedPolygonOut)
        {
            int count = polygonIn.Count;
            Vector3 v0, v1;
            v0 = polygonIn[count-1];
            for (int i = 0; i < count; ++i)
            {
                v1 = polygonIn[i];
                bool s0 = self.GetSide(v0);
                bool s1 = self.GetSide(v1);
                if (s0)
                {
                    clippedPolygonOut.Add(v0);
                }
                if (s0 != s1)
                {
                    var ray = new Ray(v0, v1 - v0);
                    float distance;
                    self.Raycast(ray, out distance);
                    clippedPolygonOut.Add(ray.GetPoint(distance));
                }
                v0 = v1;
            }
        }
    }
}