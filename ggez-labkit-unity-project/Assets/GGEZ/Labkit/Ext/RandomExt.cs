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
using System.Collections.Generic;


namespace GGEZ
{
public static partial class RandomExt
{

public static T Pick<T> (this Random self, T[] array)
    {
    return array[self.Next (0, array.Length-1)];
    }
    
public static object Pick (this Random self, ArrayList arrayList)
    {
    return arrayList[self.Next (0, arrayList.Count-1)];
    }
    
public static T Pick<T> (this Random self, List<T> list)
    {
    return list[self.Next (0, list.Count-1)];
    }

public static void Shuffle (this Random self, Array array)
    {
    int n = array.Length;
    while (n > 1) 
        {
        int k = self.Next (0, n);
        --n;
        object temp = array.GetValue (n);
        array.SetValue (array.GetValue(k), n);
        array.SetValue (temp, k);
        }
    }

public static void Shuffle (this Random self, ArrayList arrayList)
    {
    int n = arrayList.Count;
    while (n > 1) 
        {
        int k = self.Next (0, n);
        --n;
        object temp = arrayList[n];
        arrayList[n] = arrayList[k];
        arrayList[k] = temp;
        }
    }

public static void Shuffle<T> (this Random self, List<T> list)
    {
    int n = list.Count;
    while (n > 1) 
        {
        int k = self.Next (0, n);
        --n;
        T temp = list[n];
        list[n] = list[k];
        list[k] = temp;
        }
    }
}
}
