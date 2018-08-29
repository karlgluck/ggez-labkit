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

using System.Reflection;

namespace GGEZ
{
    public static partial class Util
    {
        public static void Zero(object target)
        {
            var type = target.GetType();
            PropertyInfo[] properties = type.GetProperties();
            for (int i = 0; i < properties.Length; ++i)
            {
                properties[i].SetValue(target, null, null);
            }
            FieldInfo[] fields = type.GetFields();
            for (int i = 0; i < fields.Length; ++i)
            {
                fields[i].SetValue(target, null);
            }
        }

        public static void ReflectionCopy(object source, object target)
        {
            if (!object.ReferenceEquals(source.GetType(), target.GetType()))
            {
                throw new System.InvalidOperationException("Requires source and target to be of the same type");
            }
            var type = target.GetType();
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.Public);
            for (int i = 0; i < properties.Length; ++i)
            {
                properties[i].SetValue(target, properties[i].GetValue(source, null), null);
            }
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.Public);
            for (int i = 0; i < fields.Length; ++i)
            {
                fields[i].SetValue(target, fields[i].GetValue(source));
            }
        }
    }
}
