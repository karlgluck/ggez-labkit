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
public static partial class CapsuleColliderExt
{


public static Collider[] Overlap (this CapsuleCollider self, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
    {
        
    // Get a unit vector in the target direction
    var axis = new Vector3 (self.direction == 0 ? 1f : 0f, self.direction == 1 ? 1f : 0f, self.direction == 2 ? 1f : 0f);
    Vector3 scale = self.transform.lossyScale;

    // Get components on the orthogonal axes
    var v = Vector3.Scale (Vector3.one - axis, scale);

    // Find the biggest one for the radius
    var radius = self.radius * Mathf.Max (v.x, v.y, v.z);

    // Scale the height by the axis
    var height = Vector3.Dot (axis, scale) * self.height;

    // Convert to world-space
    var center = self.transform.TransformPoint (self.center);
    var dir = height < radius * 2f ? Vector3.zero : self.transform.TransformDirection (axis);

    // Run OverlapCapsule
    Vector3 point0 = center + dir * (height * 0.5f - radius);
    Vector3 point1 = center - dir * (height * 0.5f - radius);
    return Physics.OverlapCapsule (point0, point1, self.radius, layerMask, queryTriggerInteraction);

    }


public static Collider[] Overlap (this CapsuleCollider self, int layerMask)
    {
        
    // Get a unit vector in the target direction
    var axis = new Vector3 (self.direction == 0 ? 1f : 0f, self.direction == 1 ? 1f : 0f, self.direction == 2 ? 1f : 0f);
    Vector3 scale = self.transform.lossyScale;

    // Get components on the orthogonal axes
    var v = Vector3.Scale (Vector3.one - axis, scale);

    // Find the biggest one for the radius
    var radius = self.radius * Mathf.Max (v.x, v.y, v.z);

    // Scale the height by the axis
    var height = Vector3.Dot (axis, scale) * self.height;

    // Convert to world-space
    var center = self.transform.TransformPoint (self.center);
    var dir = height < radius * 2f ? Vector3.zero : self.transform.TransformDirection (axis);

    // Run OverlapCapsule
    Vector3 point0 = center + dir * (height * 0.5f - radius);
    Vector3 point1 = center - dir * (height * 0.5f - radius);
    return Physics.OverlapCapsule (point0, point1, self.radius, layerMask);

    }


public static Collider[] Overlap (this CapsuleCollider self)
    {
        
    // Get a unit vector in the target direction
    var axis = new Vector3 (self.direction == 0 ? 1f : 0f, self.direction == 1 ? 1f : 0f, self.direction == 2 ? 1f : 0f);
    Vector3 scale = self.transform.lossyScale;

    // Get components on the orthogonal axes
    var v = Vector3.Scale (Vector3.one - axis, scale);

    // Find the biggest one for the radius
    var radius = self.radius * Mathf.Max (v.x, v.y, v.z);

    // Scale the height by the axis
    var height = Vector3.Dot (axis, scale) * self.height;

    // Convert to world-space
    var center = self.transform.TransformPoint (self.center);
    var dir = height < radius * 2f ? Vector3.zero : self.transform.TransformDirection (axis);

    // Run OverlapCapsule
    Vector3 point0 = center + dir * (height * 0.5f - radius);
    Vector3 point1 = center - dir * (height * 0.5f - radius);
    return Physics.OverlapCapsule (point0, point1, self.radius);

    }


}
}
