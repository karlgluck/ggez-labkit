using System;
using UnityEngine;

public static partial class GameObjectExt
{
public static Component GetOrAddComponent (this GameObject self, Type type)
    {
    var component = self.GetComponent (type);
    if (component == null)
        {
        component = self.AddComponent (type);
        }
    return component;
    }

// public static GameObject FindGameObjectNamed (this GameObject[] self, string name)
// 	{
//     for (int i = 0; i < self.Length; ++i)
//         {
//         if (self[i].name.Equals (name))
//             {
//             return self[i];
//             }
//         }
//     return null;
//     }
}
