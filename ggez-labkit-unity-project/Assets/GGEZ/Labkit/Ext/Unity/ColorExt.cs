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
public static partial class ColorblindSafeColors
{

public static readonly Color Orange = new Color(230/255f, 159/255f, 0f/255f);
public static readonly Color Black = new Color(0f/255f, 0f/255f, 0f/255f);
public static readonly Color SkyBlue = new Color(86f/255f, 180f/255f, 233f/255f);
public static readonly Color BluishGreen = new Color(0f/255f, 158f/255f, 115/255f);
public static readonly Color Yellow = new Color(240f/255f, 228f/255f, 66f/255f);
public static readonly Color Blue = new Color(0/255f, 114/255f, 178/255f);
public static readonly Color Vermillion = new Color(213/255f, 94/255f, 0f/255f);
public static readonly Color ReddishPurple = new Color(204/255f, 121/255f, 167/255f);
public static readonly Color White = new Color(255f/255f, 255f/255f, 255f/255f);
public static readonly Color[] WithPigment = new Color[] { Vermillion, Blue, Orange, ReddishPurple, SkyBlue, Yellow, BluishGreen, };
}

public static partial class ColorExt
{

public static Color WithR (this Color self, float r)
    {
    return new Color (r, self.g, self.b, self.a);
    }

public static Color WithG (this Color self, float g)
    {
    return new Color (self.r, g, self.b, self.a);
    }

public static Color WithB (this Color self, float b)
    {
    return new Color (self.r, self.g, b, self.a);
    }

public static Color WithA (this Color self, float a)
    {
    return new Color (self.r, self.g, self.b, a);
    }

public static Color WithRGB (this Color self, Color rgb)
    {
    return new Color (rgb.r, rgb.b, rgb.a, self.a);
    }

}

}
