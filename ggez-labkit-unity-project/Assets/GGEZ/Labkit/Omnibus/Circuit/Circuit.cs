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
using UnityEngine;
using GGEZ.FullSerializer;

namespace GGEZ.Omnibus
{

    //-------------------------------------------------------------------------
    // Attributes used when writing cells
    //-------------------------------------------------------------------------

    [AttributeUsage(AttributeTargets.Field)]
    public class InAttribute : Attribute {}

    [AttributeUsage(AttributeTargets.Field)]
    public class OutAttribute : Attribute {}

    [AttributeUsage(AttributeTargets.Class)]
    public class RequireVariablesAttribute : Attribute {}

    [AttributeUsage(AttributeTargets.Class)]
    public class RequireSettingsAttribute : Attribute {}

    [AttributeUsage(AttributeTargets.Class)]
    public class RequireAspectAttribute : Attribute
    {
        public Type AspectType;
        public RequireAspectAttribute(Type aspectType)
        {
            AspectType = aspectType;
        }
    }

    [AttributeUsage(AttributeTargets.Enum)]
    public class PointerTypeAttribute : Attribute
    {
        public Type Type;
        public PointerTypeAttribute(Type type)
        {
            Type = type;
        }
    }

    //-------------------------------------------------------------------------
    // Cell
    //-------------------------------------------------------------------------
    public class Cell
    {
        public Cell Clone()
        {
            return MemberwiseClone() as Cell;
        }

        public virtual void Acquire(EntityContainer entity, ref bool running) { }
        public virtual void Update(EntityContainer entity, bool dirty, ref bool running) { }
    }

    //-------------------------------------------------------------------------
    // Register pointers for inputs and outputs
    //-------------------------------------------------------------------------
    [PointerType(typeof(object)), fsSerializeEnumAsInteger]   public enum ObjectPtr : int { Invalid = int.MaxValue }
    [PointerType(typeof(bool)), fsSerializeEnumAsInteger]     public enum BoolPtr : int { Invalid = int.MaxValue }
    [PointerType(typeof(float)), fsSerializeEnumAsInteger]    public enum FloatPtr : int { Invalid = int.MaxValue }

    //-------------------------------------------------------------------------
    // References an entity variable
    //-------------------------------------------------------------------------
    public enum EntityRelationship
    {
        Self,
        Owner,
        Subject,
        Target,
    }

    public struct VariableRef
    {
        public EntityRelationship Relationship;
        public string Name;
    }

    //-------------------------------------------------------------------------
    // References an entity setting
    //-------------------------------------------------------------------------
    public struct SettingRef
    {
        public string Name;
    }

}
