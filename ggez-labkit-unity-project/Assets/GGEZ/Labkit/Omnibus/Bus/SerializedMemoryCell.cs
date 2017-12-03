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
using System.Reflection;

namespace GGEZ
{
namespace Omnibus
{


[
Serializable
]
public sealed partial class SerializedMemoryCell
{
public string Key = "";

public string Type = null;


public object GetValue ()
    {
    var field = this.GetType ().GetField (nameof_Value_ + this.Type);
    if (field != null)
        {
        return field.GetValue (this);
        }
    return null;
    }

public void SetValue (object value)
    {
    if (value == null)
        {
        this.Type = "";
        return;
        }
    this.Type = value.GetType ().Name;
    var field = this.GetType ().GetField (nameof_Value_ + this.Type);
    if (field == null)
        {
        throw new System.NotImplementedException ("SerializedMemorySell needs Omnibus support added for " + this.Type);
        }
    field.SetValue (this, value);
    }

public static SerializedMemoryCell Create (string key, object value)
    {
    if (key == null)
        {
        throw new ArgumentNullException ("key");
        }
    var retval = new SerializedMemoryCell ();
    retval.Key = key;
    retval.SetValue (value);
    return retval;
    }

private static List<Type> fieldTypes = null;
public static IEnumerable<Type> GetFieldTypes ()
    {
    if (fieldTypes == null)
        {
        var valueFields = typeof(SerializedMemoryCell).FindMembers (
                MemberTypes.Field,
                BindingFlags.Public | BindingFlags.Instance,
                delegate (MemberInfo m, object lastArgument)
                    {
                    return m.Name.StartsWith (SerializedMemoryCell.nameof_Value_);
                    },
                null
                );
        fieldTypes = new List<Type> ();
        foreach (var memberInfo in valueFields)
            {
            FieldInfo fieldInfo = (FieldInfo)memberInfo;
            fieldTypes.Add (fieldInfo.FieldType);
            }
        }
    return fieldTypes;
    }


#region nameof
public const string nameof_Key = "Key";
public const string nameof_Type = "Type";
public const string nameof_Value_ = "Value_";
#endregion
}

}

}
