using System.Collections.Generic;

public static partial class ListExt
{
    
public static T Last<T> (this List<T> self)
    {
    return self.Count > 0 ? self[self.Count - 1] : default(T);
    }

public static T SafeIndex<T> (this List<T> self, int i)
    {
    if (self != null)
        {
        int max = self.Count - 1;
        if (max >= 0)
            {
            return self[i < 0 ? 0 : (i > max ? max : i)];
            }
        }
    return default(T);
    }

public static void Swap<T> (this List<T> self, int i, int j)
    {
    T temp = self[i];
    self[i] = self[j];
    self[j] = temp;
    }
    
public static bool HasNullGap<T> (this List<T> self) where T : class
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
}