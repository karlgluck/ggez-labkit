using UnityEngine;

public static partial class ObjectExt
{
public static Object FindByName (this Object[] self, string name)
	{
    for (int i = 0; i < self.Length; ++i)
        {
        if (self[i].name.Equals (name))
            {
            return self[i];
            }
        }
    return null;
    }
}
