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
    public static partial class RectExt
    {
        public static Vector2[] GetVertices(this Rect self)
        {
            return new Vector2[] {
                new Vector2 (self.xMin, self.yMin),
                new Vector2 (self.xMin, self.yMax),
                new Vector2 (self.xMax, self.yMax),
                new Vector2 (self.xMax, self.yMin),
            };
        }

        /// <summary>
        /// Return the rect decreased in size evenly around its border
        /// </summary>
        public static Rect ContractedBy(this Rect self, float margin)
        {
            float twiceMargin = margin * 2;
            return new Rect(
                    self.x + margin,
                    self.y + margin,
                    self.width - twiceMargin,
                    self.height - twiceMargin
                    );
        }

        /// <summary>
        /// Return the rect decreased in size by fixed amounts on each edge
        /// </summary>
        public static Rect ContractedBy(this Rect self, float xMin, float xMax, float yMin, float yMax)
        {
            return Rect.MinMaxRect(
                self.xMin + xMin,
                self.yMin + yMin,
                self.xMax - xMax,
                self.yMax - yMax
            );
        }

        /// <summary>
        /// Return the rect increased in size evenly around its border
        /// </summary>
        public static Rect ExpandedBy(this Rect self, float margin)
        {
            float twiceMargin = margin * 2;
            return new Rect(
                    self.x - margin,
                    self.y - margin,
                    self.width + twiceMargin,
                    self.height + twiceMargin
                    );
        }

        /// <summary>
        /// Return the rect increased in size by fixed amounts on each edge
        /// </summary>
        public static Rect ExpandedBy(this Rect self, float xMin, float xMax, float yMin, float yMax)
        {
            return Rect.MinMaxRect(
                self.xMin - xMin,
                self.yMin - yMin,
                self.xMax + xMax,
                self.yMax + yMax
            );
        }

        /// <summary>
        /// Returns the point on the boundary of this rectangle in the given direction.
        /// </summary>
        /// <param name="angle">Angle (z) defining the direction in which to find the point</param>
        public static Vector2 PointAtAngleFromCenter(this Rect self, float angle)
        {
            float angleRad = angle * Mathf.Deg2Rad;

            float a = self.width * 0.5f;
            float b = self.height * 0.5f;
            float radius = Mathf.Abs(Mathf.Tan(angleRad)) < (b / a) ? a / Mathf.Abs(Mathf.Cos(angleRad)) : b / Mathf.Abs(Mathf.Sin(angleRad));

            Vector2 center = self.center;
            return new Vector2(Mathf.Cos(angleRad) * radius + center.x, Mathf.Sin(angleRad) * radius + center.y);
        }

        /// <summary>
        /// Returns the point on the boundary of this rectangle in the given direction.
        /// </summary>
        /// <param name="direction">Normalized direction in which to find the point</param>
        public static Vector2 PointInDirectionFromCenter(this Rect self, Vector2 direction)
        {
            Debug.Assert(Mathf.Approximately(direction.magnitude, 1f));

            float angleRad = Mathf.Atan2(direction.y, direction.x);

            float a = self.width * 0.5f;
            float b = self.height * 0.5f;
            float radius = Mathf.Abs(Mathf.Tan(angleRad)) < (b / a) ? a / Mathf.Abs(Mathf.Cos(angleRad)) : b / Mathf.Abs(Mathf.Sin(angleRad));

            Vector2 center = self.center;
            return new Vector2(direction.x * radius + center.x, direction.y * radius + center.y);
        }

        public static Rect InParentCoordinates(this Rect self, Rect parent)
        {
            return new Rect(
                self.x + parent.x,
                self.y + parent.y,
                self.width,
                self.height
            );
        }

        public static Rect MovedBy(this Rect self, Vector2 deltaPosition)
        {
            return new Rect(
                self.x + deltaPosition.x,
                self.y + deltaPosition.y,
                self.width,
                self.height
            );
        }

        public static Rect WithWidth(this Rect self, float width)
        {
            return new Rect(
                self.x,
                self.y,
                width,
                self.height
            );
        }

        public static Rect WithHeight(this Rect self, float height)
        {
            return new Rect(
                self.x,
                self.y,
                self.width,
                height
            );
        }

        public static Rect GetRow(this Rect self, int row, float rowHeight)
        {
            return new Rect(self.x, self.y + row * rowHeight, self.width, rowHeight);
        }

        public static Rect GetCenteredRight(this Rect self, float xOffset, float width, float height)
        {
            return new Rect(
                self.xMax + xOffset - width * 0.5f,
                (self.yMin + self.yMax) * 0.5f - height * 0.5f,
                width,
                height
            );
        }

        public static Rect GetCenteredLeft(this Rect self, float xOffset, float width, float height)
        {
            return new Rect(
                self.xMin + xOffset - width * 0.5f,
                (self.yMin + self.yMax) * 0.5f - height * 0.5f,
                width,
                height
            );
        }
    }
}
