using System;
using UnityEngine;

public static partial class MonoBehaviourExt
{
public static Component GetOrAddComponent (this MonoBehaviour self, Type type)
    {
    var component = self.GetComponent (type);
    if (component == null)
        {
        component = self.gameObject.AddComponent (type);
        }
    return component;
    }
}
