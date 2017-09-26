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
public struct Recti
{
public static Recti invalid = new Recti () { xMin = 1, xMax = -1, yMin = 1, yMax = -1 };
public static Recti zero = new Recti () { xMin = 0, xMax = 0, yMin = 0, yMax = 0 };

public int xMin, xMax, yMin, yMax;

public Vector2i CornerBottomLeft ()
    {
    return new Vector2i (this.xMin, this.yMin);
    }

public Vector2i CornerTopRight ()
    {
    return new Vector2i (this.xMin, this.yMin);
    }

public Vector2i CornerTopLeft ()
    {
    return new Vector2i (this.xMin, this.yMax);
    }

public Vector2i CornerBottomRight ()
    {
    return new Vector2i (this.xMax, this.yMin);
    }

public Recti (Vector2i bottomLeft, Vector2i size)
    {
    this.xMin = bottomLeft.x;
    this.yMin = bottomLeft.y;
    this.xMax = bottomLeft.x + size.x;
    this.yMax = bottomLeft.y + size.y;
    }

public int Height
    {
    get
        {
        return yMax - yMin;
        }
    }

public int Width
    {
    get
        {
        return xMax - xMin;
        }
    }

public bool IsValid
    {
    get
        {
        return this.xMin <= this.xMax && this.yMin <= this.yMax;
        }
    }

public void Invalidate ()
    {
    this.xMin = 1;
    this.xMax = -1;
    this.yMin = 1;
    this.yMax = -1;
    }

public static Recti MinMaxRect (int xMin, int yMin, int xMax, int yMax)
    {
    return new Recti ()
        {
        xMin = xMin,
        yMin = yMin,
        xMax = xMax,
        yMax = yMax,
        };
    }

public bool IsSameSizeAs (Recti other)
    {
    return this.IsValid && other.Width == this.Width && other.Height == this.Height;
    }

public override string ToString ()
    {
        return "{" + this.xMin + ", " + this.yMin + "," + this.xMax + "," + this.yMax + "}";
    }

// Calculates the limits for the set of tiles that are contained in
// the new region but are not present in this one.  The caller can
// iterate through the new tiles by iterating through all the
// locations within the returned rectangles.  Note that the
// rectangle coordinates are *inclusive*, so the right/bottom members
// are actually part of the set.
// This method requires that the size of the two rectangles is exactly
// the same, otherwise it will miss tiles.
public void GetRectRegionsAdded (Recti other, out Recti d1, out Recti d2)
    {
    bool thisIsValid = this.IsValid;
    bool otherIsValid = other.IsValid;

    if (thisIsValid && !otherIsValid)
        {
        d1 = Recti.invalid;
        d2 = Recti.invalid;
        return;
        }

    if (!thisIsValid && otherIsValid)
        {
        d1 = other;
        d2 = Recti.invalid;
        return;
        }

    if (!this.IsSameSizeAs(other))
        {
        throw new System.NotSupportedException("Don't use Recti.GetRectRegionsAdded with rects of different sizes");
        }

    bool rectanglesDontOverlap =
        other.xMin > this.xMax ||
        other.xMax < this.xMin ||
        other.yMin > this.yMax ||
        other.yMax < this.yMin;

    if (rectanglesDontOverlap)
        {
        d1 = other;
        d2 = Recti.invalid;
        return;
        }

    d1.xMin = other.xMin;
    d1.xMax = other.xMax;

    if (other.yMin < this.yMin)
        {
        d1.yMin = other.yMin;
        d1.yMax = this.yMin - 1;
        d2.yMin = this.yMin;
        d2.yMax = other.yMax;
        }
    else if (other.yMin > this.yMin)
        {
        d1.yMin = this.yMax + 1;
        d1.yMax = other.yMax;
        d2.yMin = other.yMin;
        d2.yMax = this.yMax;
        }
    else
        {
        d1 = Recti.invalid;
        d2.yMin = this.yMin;
        d2.yMax = this.yMax;
        }

    if (other.xMin < this.xMin)
        {
        d2.xMin = other.xMin;
        d2.xMax = this.xMin - 1;
        }
    else if (other.xMin > this.xMin)
        {
        d2.xMin = this.xMax + 1;
        d2.xMax = other.xMax;
        }
    else
        {
        d2 = Recti.invalid;
        }
    }

public void GetRectRegionsChanged (
        Recti other,
        out Recti thisOnly1,
        out Recti thisOnly2,
        out Recti otherOnly1,
        out Recti otherOnly2)
    {
    this.GetRectRegionsAdded (other, out otherOnly1, out otherOnly2);
    other.GetRectRegionsAdded (this, out thisOnly1, out thisOnly2);
    }

}
}