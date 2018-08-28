using System;

namespace GGEZ.FullSerializer
{
    /// <summary>
    /// This attribute forces a given enum type to serialize as an integer
    /// rather than as a string when fsConfig.SerializeEnumsAsInteger is false.
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum)]
    public sealed class fsSerializeEnumAsIntegerAttribute : Attribute
    {
    }
}