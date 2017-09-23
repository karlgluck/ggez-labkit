using UnityEngine;
using System.Collections;


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

public static void Shuffle<T> (this T[] self)
    {
        for (int i = self.Length - 1; i > 0; --i)
        {
            int j = Random.Range (0, i);
            var temp = self[i];
            self[i] = self[j];
            self[j] = temp;
        }
    }
    
public static void Shuffle<T> (this T[] self, System.Random rng)
    {
        for (int i = self.Length - 1; i > 0; --i)
        {
            int j = rng.Next (0, i);
            var temp = self[i];
            self[i] = self[j];
            self[j] = temp;
        }
    }

public static void Swap<T> (this T[] self, int i, int j)
    {
    T temp = self[i];
    self[i] = self[j];
    self[j] = temp;
    }
}