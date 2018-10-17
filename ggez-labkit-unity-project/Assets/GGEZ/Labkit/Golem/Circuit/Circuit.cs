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
using System.Collections.Generic;
using GGEZ.FullSerializer;

namespace GGEZ.Labkit
{

    //-------------------------------------------------------------------------
    // Attributes used when writing cells
    //-------------------------------------------------------------------------

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class InAttribute : Attribute
    { }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class OutAttribute : Attribute
    { }

    //-------------------------------------------------------------------------
    // Register pointers for inputs and outputs
    //-------------------------------------------------------------------------
    [fsSerializeEnumAsInteger] public enum RegisterPtr : int { Invalid = int.MaxValue }

    /// <summary>
    /// Provides access to a variable in a Cell or Script.
    /// </summary>
    /// <remarks>
    /// There are at least 3 good options for knowing the target type:
    ///   1. e.g. "struct FloatVariableRef : IVariableRef { ... }"
    ///   2. Annotating each use of VariableRef with the intended type
    ///   3. Storing the type as a field inside the VariableRef struct
    /// (1) adds mandatory implementation work for every new variable type
    //  and adds additional runtime burden.
    /// (2) requires more information to be passed wherever the variable
    /// is edited, and target type is not strongly tied to the reference
    /// (3) Can create bugs involving serialization if the user changes
    /// the type of a field but not the name.
    ///
    /// I chose (2) even though it is a little ugly and requires more work
    /// because it is flexible to users needs and won't cause bugs.
    /// </remarks>
    public class VariableRef
    {
        /// <summary>Key to use to look up a variable</summary>
        public readonly string Name;

        /// <remarks>Null/empty for a local variable</remarks>
        public readonly string Relationship;

        public VariableRef(string relationship, string name)
        {
            Relationship = relationship;
            Name = name;
        }

        public bool IsExternal
        {
            get
            {
                return !string.IsNullOrEmpty(Relationship);
            }
        }

        public bool IsValid
        {
            get { return !string.IsNullOrEmpty(Name); }
        }

        public int RegisterIndex
        {
            get { throw new InvalidOperationException(); }
        }

        public static Dictionary<VariableRef, T> NewDictionary<T>()
        {
            return new Dictionary<VariableRef, T>(EqualityComparer);
        }

        // Use this equality comparer when using a VariableRef as a dictionary key
        public readonly static IEqualityComparer<VariableRef> EqualityComparer = new VariableRefEqualityComparer();
        private sealed class VariableRefEqualityComparer : IEqualityComparer<VariableRef>
        {
            public bool Equals(VariableRef a, VariableRef b)
            {
                return string.Equals(a.Name, b.Name) && string.Equals(a.Relationship, b.Relationship);
            }

            public int GetHashCode(VariableRef obj)
            {
                unchecked
                {
                    return ((obj.Relationship != null ? obj.Relationship.GetHashCode() * 397 : 0))
                        ^ (obj.Name != null ? obj.Name.GetHashCode() : 0);
                }
            }
        }
    }

}
