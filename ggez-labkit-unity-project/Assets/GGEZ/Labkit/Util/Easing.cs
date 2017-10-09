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
public static partial class Util
{

public static float EaseElasticOut (float time, float start, float end, float duration)
    {
    if ((time /= duration) == 1)
        {
        return start + end;
        }

    float p = duration * .3f;
    float s = p / 4;
    return (end * Mathf.Pow (2, -10 * time) * Mathf.Sin((time * duration - s) * (2 * Mathf.PI) / p) + end + start);
    }

public static float EaseElasticOut2 (float t, float amplitude, float period)
    {
    var s = Mathf.Asin (1 / (amplitude = Mathf.Max (1, amplitude))) * (period /= (2 * Mathf.PI));
    return 1 - amplitude * Mathf.Pow (2, -10 * t) * Mathf.Sin ((t + s) / period);
    }

public static float EaseJump (float t)
    {
    return 4 * t * (1 - t);
    }

public static float EaseJumpBounce (float t)
    {
    float s = 1f;
    if (0 <= t && t < 4 / 8f)
        {
        t = Mathf.InverseLerp (0, 4 / 8f, t);
        }
    else if (t <= 6 / 8f)
        {
        s = 0.4f;
        t = Mathf.InverseLerp (4 / 8f, 6 / 8f, t);
        }
    else
        {
        s = 0.4f * 0.4f;
        t = Mathf.InverseLerp (6 / 8f, 1f, t);
        }
    return s * 4 * t * (1 - t);
    }
}
}
