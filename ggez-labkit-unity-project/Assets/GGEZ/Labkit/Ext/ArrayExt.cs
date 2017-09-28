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
using System.Collections;


namespace GGEZ
{
public static partial class ArrayExt
{
public static T Last<T> (this T[] self)
    {
    return self.Length > 0 ? self[self.Length - 1] : default(T);
    }

public static T SafeIndex<T>(this T[] self, int i)
    {
    if (self != null)
        {
        int max = self.Length - 1;
        if (max >= 0)
            {
            return self[i < 0 ? 0 : (i > max ? max : i)];
            }
        }
    return default(T);
    }

public static void Swap<T> (this T[] self, int i, int j)
    {
    T temp = self[i];
    self[i] = self[j];
    self[j] = temp;
    }
}
}
