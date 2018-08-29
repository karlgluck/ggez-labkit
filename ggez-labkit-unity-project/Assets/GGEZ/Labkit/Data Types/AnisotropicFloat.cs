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


namespace GGEZ
{
    [Serializable]
    public class AnisotropicFloat
    {
        [Tooltip("Value to return if the input vector is too small. Not scaled.")]
        public float zeroVectorValue = 0f;

        [Tooltip("Magnitude of input vector under which to return zeroVectorValue"), Range(0f, 0.1f)]
        public float zeroThreshold = 0.001f;

        [Tooltip("Values are all multiplied by the scale factor")]
        public float scale = 1f;

        [Tooltip("Value to return at 0 degrees (straight ahead)"), Range(0f, 1f)]
        public float v000 = 1f;
        [Tooltip("Value to return at 45 degrees"), Range(0f, 1f)]
        public float v045 = 1f;

        [Tooltip("Value to return at 90 degrees (left)"), Range(0f, 1f)]
        public float v090 = 1f;
        [Tooltip("Value to return at 135 degrees"), Range(0f, 1f)]
        public float v135 = 1f;

        [Tooltip("Value to return at 180 degrees (back)"), Range(0f, 1f)]
        public float v180 = 1f;
        [Tooltip("Value to return at 225 (-135) degrees"), Range(0f, 1f)]
        public float v225 = 1f;

        [Tooltip("Value to return at 270 (-90) degrees (right)"), Range(0f, 1f)]
        public float v270 = 1f;
        [Tooltip("Value to return at 315 (-45) degrees"), Range(0f, 1f)]
        public float v315 = 1f;

        public int MirrorKey(int key)
        {
            switch (key)
            {
                case 0: return 4;
                case 1: return 5;
                case 2: return 6;
                case 3: return 7;
                case 4: return 0;
                case 5: return 1;
                case 6: return 2;
                case 7: return 3;
            }
            return key;
        }

        public int MirrorKeyLeftRight(int key)
        {
            switch (key)
            {
                case 0: return 0;
                case 1: return 7;
                case 2: return 6;
                case 3: return 5;
                case 4: return 4;
                case 5: return 3;
                case 6: return 2;
                case 7: return 1;
            }
            return key;
        }

        public int MirrorKeyForwardBackward(int key)
        {
            switch (key)
            {
                case 0: return 4;
                case 1: return 3;
                case 2: return 2;
                case 3: return 1;
                case 4: return 0;
                case 5: return 7;
                case 6: return 6;
                case 7: return 5;
            }
            return key;
        }

        public float GetKeyValue(int key)
        {
            switch (key)
            {
                case 0: return this.v000;
                case 1: return this.v045;
                case 2: return this.v090;
                case 3: return this.v135;
                case 4: return this.v180;
                case 5: return this.v225;
                case 6: return this.v270;
                case 7: return this.v315;
            }
            return this.zeroVectorValue;
        }

        public void SetKeyValue(int key, float value)
        {
            switch (key)
            {
                case 0: this.v000 = value; break;
                case 1: this.v045 = value; break;
                case 2: this.v090 = value; break;
                case 3: this.v135 = value; break;
                case 4: this.v180 = value; break;
                case 5: this.v225 = value; break;
                case 6: this.v270 = value; break;
                case 7: this.v315 = value; break;
            }
            this.zeroVectorValue = value;
        }

        public float GetKeyAngle(int key)
        {
            switch (key)
            {
                case 0: return 0f;
                case 1: return 45f;
                case 2: return 90f;
                case 3: return 135f;
                case 4: return 180f;
                case 5: return 225f;
                case 6: return 270f;
                case 7: return 315f;
            }
            throw new System.ArgumentOutOfRangeException("key");
        }

        public Vector3[] GetHandlePoints(float scaleOverride)
        {
            return new Vector3[] {
            scaleOverride * this.v000 * new Vector3 (0f, 0f, 1f),
            scaleOverride * this.v045 * new Vector3 (0.7071067811865475f, 0f, 0.7071067811865476f),
            scaleOverride * this.v090 * new Vector3 (1f, 0f, 0f),
            scaleOverride * this.v135 * new Vector3 (0.7071067811865476f, 0f, -0.7071067811865475f),
            scaleOverride * this.v180 * new Vector3 (0f, 0f, -1f),
            scaleOverride * this.v225 * new Vector3 (-0.7071067811865475f, 0f, -0.7071067811865477f),
            scaleOverride * this.v270 * new Vector3 (-1f, 0f, 0f),
            scaleOverride * this.v315 * new Vector3 (-0.7071067811865477f, 0f, 0.7071067811865474f),
            scaleOverride * this.v000 * new Vector3 (0f, 0f, 1f),
            };
        }

        public float GetValue(float angle)
        {
            angle = Util.RepeatUniform(angle, 360f);
            if (0f <= angle && angle < 45f) { return this.scale * Mathf.Lerp(this.v000, this.v045, Mathf.InverseLerp(0f, 45f, angle)); }
            else if (45f <= angle && angle < 90f) { return this.scale * Mathf.Lerp(this.v045, this.v090, Mathf.InverseLerp(45f, 90f, angle)); }
            else if (90f <= angle && angle < 135f) { return this.scale * Mathf.Lerp(this.v090, this.v135, Mathf.InverseLerp(90f, 135f, angle)); }
            else if (135f <= angle && angle < 180f) { return this.scale * Mathf.Lerp(this.v135, this.v180, Mathf.InverseLerp(135f, 180f, angle)); }
            else if (180f <= angle && angle < 225f) { return this.scale * Mathf.Lerp(this.v180, this.v225, Mathf.InverseLerp(180f, 225f, angle)); }
            else if (225f <= angle && angle < 270f) { return this.scale * Mathf.Lerp(this.v225, this.v270, Mathf.InverseLerp(225f, 270f, angle)); }
            else if (270f <= angle && angle < 315f) { return this.scale * Mathf.Lerp(this.v270, this.v315, Mathf.InverseLerp(270f, 315f, angle)); }
            else { return this.scale * Mathf.Lerp(this.v315, this.v000, Mathf.InverseLerp(315f, 360f, angle)); }
        }

        public float GetValue(float angle, float forwardAngle)
        {
            return GetValue(angle - forwardAngle);
        }

        public float GetValue(float angle, Vector2 forward)
        {
            return GetValue(angle - forward.ToDirection(angle));
        }

        public float GetValueOnXZ(float angle, Vector3 forward)
        {
            return GetValue(angle - forward.ToDirectionOnXZ(angle));
        }

        public float GetValue(Vector2 vector)
        {
            if (vector.sqrMagnitude < this.zeroThreshold * this.zeroThreshold)
            {
                return this.zeroVectorValue;
            }
            return this.GetValue(vector.ToDirection() * Mathf.Rad2Deg);
        }

        public float GetValue(Vector3 vector, Vector3 forward, Vector3 up)
        {
            if (vector.sqrMagnitude < this.zeroThreshold * this.zeroThreshold)
            {
                return this.zeroVectorValue;
            }
            Vector3 direction = vector.normalized;
            var rotation = Quaternion.FromToRotation(forward, direction);
            Vector3 axis;
            float angle;
            rotation.ToAngleAxis(out angle, out axis);
            return this.GetValue(angle * Mathf.Sign(Vector3.Dot(axis, up)));
        }
    }
}
