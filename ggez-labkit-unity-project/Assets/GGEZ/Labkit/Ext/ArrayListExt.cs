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
public static partial class ArrayListExt
{
public static void RemoveByReference (this ArrayList self, object toRemove)
    {
    for (int i = self.Count - 1; i >= 0; --i)
        {
        if (object.ReferenceEquals (self[i], toRemove))
            {
            self.RemoveAt (i);
            }
        }
    }

public static object Last (this ArrayList self)
    {
    return self.Count > 0 ? self[self.Count - 1] : null;
    }

public static T Last<T> (this ArrayList self)
    {
    return (T)self.Last ();
    }

public static T SafeIndex<T> (this ArrayList self, int i)
    {
    return (T)self.SafeIndex(i);
    }

public static object SafeIndex (this ArrayList self, int i)
    {
    if (self != null)
        {
        int max = self.Count - 1;
        if (max >= 0)
            {
            return self[i < 0 ? 0 : (i > max ? max : i)];
            }
        }
    return null;
    }

public static void Swap (this ArrayList self, int i, int j)
    {
    object temp = self[i];
    self[i] = self[j];
    self[j] = temp;
    }
    
public static bool HasNullGap (this ArrayList self)
    {
    if (self.Count <= 1)
        {
        return false;
        }
    object last = self[0];
    for (int i = 1; i < self.Count; ++i)
        {
        object current = self[i];
        if (current != null && last == null)
            {
            return true;
            }
        last = current;
        }
    return false;
    }
    
public static object GetNextNonNullElement (this ArrayList self, ref int index)
    {
    return self.GetNextNonNullElement (ref index, self.Count);
    }

public static object GetNextNonNullElement (this ArrayList self, ref int index, int end)
    {
    while (index < end)
		{
        object retval = self[index];
        if (retval != null)
			{
            return retval;
			}
        ++index;
		}
    return null;
    }

public static void RemoveNullElementsStable (this ArrayList self)
    {
    int offset = 0;
    for (int i = 0; i < self.Count; ++i)
        {
        object element = self[i];
        if (element == null)
            {
            ++offset;
            }
        else if (offset > 0)
            {
            self[i - offset] = element;
            }
        }
    self.RemoveRange (self.Count - offset, offset);
    }

public static int BinarySearch (this ArrayList self, System.Func<object, int> compare)
    {
    return self.BinarySearch (compare, 0, self.Count-1);
    }

public static int BinarySearch (this ArrayList self, System.Func<object, int> compare, int min, int max)
    {
    while (min <= max)
        {
        int mid = (min + max) / 2;
        int comparison = compare (self[mid]);
        if (comparison == 0)
            {
            return mid;
            }
        else if (comparison < 0)
            {
            max = mid - 1;
            }
        else
            {
            min = mid + 1;
            }
        }
    return -1;
    }
    
public static int LinearSearchNullSafe (this ArrayList self, System.Func<object, int> compare)
    {
    for (int i = 0; i < self.Count; ++i)
        {
        object element = self[i];
        if (element == null)
            {
            continue;
            }
        int comparison = compare (element);
        if (comparison == 0)
            {
            return i;
            }
        if (comparison > 0)
            {
            break;
            }
        }
    return -1;
    }

public static int BinaryInsert (this ArrayList self, System.Func<object, int> compare)
    {
    return self.BinaryInsert (compare, 0, self.Count-1);
    }

public static int BinaryInsert (this ArrayList self, System.Func<object, int> compare, int min, int max)
    {
    while (min <= max)
        {
        int mid = (min + max) / 2;
        int comparison = compare (self[mid]);
        if (comparison < 0)
            {
            max = mid - 1;
            }
        else
            {
            min = mid + 1;
            }
        }
    return min;
    }

public static void BinaryInsert (this ArrayList self, System.IComparable element)
    {
    int min = 0;
    int max = self.Count - 1;
    while (min <= max)
        {
        int mid = (min + max) / 2;
        int comparison = element.CompareTo ((System.IComparable)self[mid]);
        if (comparison < 0)
            {
            max = mid - 1;
            }
        else
            {
            min = mid + 1;
            }
        }
    self.Insert (min, element);
    }
}
}
