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
using System.Collections.Generic;

namespace GGEZ
{
    [System.Serializable]
    public struct Vector2i
    {
        public static readonly Vector2i zero = new Vector2i(0, 0);
        public static readonly Vector2i one = new Vector2i(1, 1);
        public static readonly Vector2i right = new Vector2i(1, 0);
        public static readonly Vector2i up = new Vector2i(0, 1);
        public static readonly Vector2i left = new Vector2i(-1, 0);
        public static readonly Vector2i down = new Vector2i(0, -1);

        public int x, y;

        public Vector2i(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override int GetHashCode()
        {
            var X = (ulong)(x >= 0 ? 2 * (long)x : -2 * (long)x - 1);
            var Y = (ulong)(y >= 0 ? 2 * (long)y : -2 * (long)y - 1);
            var K = (long)((X >= Y ? X * X + X + Y : X + Y * Y) / 2);
            var perfectHash = x < 0 && y < 0 || x >= 0 && y >= 0 ? K : -K - 1;
            return (int)perfectHash;
        }

        public override bool Equals(object other)
        {
            var otherActual = (Vector2i)other;
            if (other is Vector2i)
            {
                return otherActual.x == this.x && otherActual.y == this.y;
            }
            return false;
        }

        public bool Equals(Vector2i other)
        {
            return other.x == this.x && other.y == this.y;
        }

        public override string ToString()
        {
            return "{" + this.x + ", " + this.y + "}";
        }

        public Vector2i DeltaTo(Vector2i v)
        {
            return new Vector2i() { x = v.x - this.x, y = v.y - this.y };
        }

        public bool IsOutsideBounds(int xMax, int yMax)
        {
            return this.x < 0 || this.y < 0 || this.x >= xMax || this.y >= yMax;
        }

        public static Vector2i operator +(Vector2i lhs, Vector2i rhs)
        {
            return new Vector2i(lhs.x + rhs.x, lhs.y + rhs.y);
        }

        public static Vector2i operator -(Vector2i lhs, Vector2i rhs)
        {
            return new Vector2i(lhs.x - rhs.x, lhs.y - rhs.y);
        }

        public int DistanceManhattan(Vector2i v)
        {
            int dx = Math.Abs(v.x - this.x);
            int dy = Math.Abs(v.y - this.y);
            return dx + dy;
        }

        public float Distance(Vector2i v)
        {
            return (v - this).magnitude;
        }

        public static IEnumerator<Vector2i> FindPointsBetween(Vector2i start, Vector2i end, int stepSize)
        {
            int dx = Math.Abs(end.x - start.x);
            int dy = Math.Abs(end.y - start.y);
            int n = 1 + dx + dy;
            int x_inc = (end.x > start.x) ? 1 : -1;
            int y_inc = (end.y > start.y) ? 1 : -1;
            int error = dx - dy;
            dx *= 2;
            dy *= 2;

            var current = start;
            int stepsUntilYield = (n - 1) % stepSize;

            for (; n > 0; --n, --stepsUntilYield)
            {
                if (0 >= stepsUntilYield)
                {
                    yield return current;
                    stepsUntilYield = stepSize;
                }

                if (error > 0)
                {
                    current.x += x_inc;
                    error -= dy;
                }
                else
                {
                    current.y += y_inc;
                    error += dx;
                }
            }
        }

        public static Vector2i Midpoint(Vector2i a, Vector2i b)
        {
            return new Vector2i() { x = (a.x + b.x) / 2, y = (a.y + b.y) / 2 };
        }

        public static Vector2i Lerp(Vector2i a, Vector2i b, float t)
        {
            return new Vector2i()
            {
                x = (int)((b.x - a.x) * t + a.x),
                y = (int)((b.y - a.y) * t + a.y),
            };
        }


        public static int Dot(Vector2i a, Vector2i b)
        {
            return a.x * b.x + a.y * b.y;
        }

        public int DistanceSquared(Vector2i other)
        {
            var dx = other.x - this.x;
            var dy = other.y - this.y;
            return dx * dx + dy * dy;
        }


        public float magnitude
        {
            get
            {
                double xAbsAsDouble = Math.Abs((double)this.x);
                double yOverX = this.y / xAbsAsDouble;
                return (float)(xAbsAsDouble * Math.Sqrt(1.0 + yOverX * yOverX));
            }
        }


        public int sqrMagnitude
        {
            get
            {
                return this.x * this.x + this.y * this.y;
            }
        }


        public bool GetPerpendicularDeltaFromLineSegment(Vector2i a, Vector2i b, ref Vector2i delta)
        {
            var lengthSquared = a.DistanceSquared(b);
            if (0 == lengthSquared)
            {
                return false;
            }
            float t = Dot(a.DeltaTo(this), a.DeltaTo(b)) / (float)lengthSquared;
            if (t < 0.0f || t > 1.0f)
            {
                return false;
            }
            else
            {
                var projection = Vector2i.Lerp(a, b, t);
                delta = projection.DeltaTo(this);
                return true;
            }
        }


        public int GetDistanceToLineSegmentSquared(Vector2i a, Vector2i b)
        {
            var lengthSquared = a.DistanceSquared(b);
            if (0 == lengthSquared)
            {
                return this.DistanceSquared(a);
            }
            float t = Vector2i.Dot(a.DeltaTo(this), a.DeltaTo(b)) / (float)lengthSquared;
            if (t < 0.0f)
            {
                return this.DistanceSquared(a);
            }
            else if (t > 1.0f)
            {
                return this.DistanceSquared(b);
            }
            else
            {
                var projection = Vector2i.Lerp(a, b, t);
                return this.DistanceSquared(projection);
            }
        }


        public float GetDistanceToLineSegment(Vector2i a, Vector2i b)
        {
            var lengthSquared = a.Distance(b);
            if (0 == lengthSquared)
            {
                return this.Distance(a);
            }
            float t = Vector2i.Dot(a.DeltaTo(this), a.DeltaTo(b)) / (float)lengthSquared;
            if (t < 0.0f)
            {
                return this.Distance(a);
            }
            else if (t > 1.0f)
            {
                return this.Distance(b);
            }
            else
            {
                var projection = Vector2i.Lerp(a, b, t);
                return this.Distance(projection);
            }
        }
    }
}
