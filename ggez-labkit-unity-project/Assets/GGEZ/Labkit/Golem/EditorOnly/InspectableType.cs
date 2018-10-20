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
using UnityObject = UnityEngine.Object;
using UnityObjectList = System.Collections.Generic.List<UnityEngine.Object>;
using System.Collections.Generic;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;

namespace GGEZ.Labkit
{

    //-----------------------------------------------------------------------------
    // InspectableType
    //-----------------------------------------------------------------------------
    public enum InspectableType
    {
        [Represents(typeof(float))] Float,
        [Represents(typeof(int))] Int,
        [Represents(typeof(bool))] Bool,
        [Represents(typeof(string))] String,
        [Represents(typeof(Rect))] Rect,
        [Represents(typeof(Color))] Color,
        [Represents(typeof(Vector2))] Vector2,
        [Represents(typeof(Vector3))] Vector3,
        // [Represents(typeof(MonoBehaviour))] MonoBehaviour,
        // [Represents(typeof(Component))] UnityComponent,
        // [Represents(typeof(GameObject))] GameObject,
        [Represents(typeof(UnityEngine.Object))] UnityObject,
        [Represents(typeof(VariableRef), false)] VariableRef,
        [Represents(typeof(Enum))] Enum,
        [Represents(typeof(KeyCode))] KeyCode,
        [Represents(typeof(Trigger), false)] TriggerRef,
        [Represents(typeof(Golem), false)] Golem,
        [Represents(typeof(Aspect), false)] Aspect,
        [Represents(typeof(IVariable), false)] Variable,

        #warning TODO: bounds, boundsint, AnimationCurve, double, Texture, Sprite, long, mask, object? RectInt, vector2int, vector4,

        Invalid = int.MaxValue,
    }

    //-----------------------------------------------------------------------------
    // RepresentsAttribute
    //-----------------------------------------------------------------------------
    public class RepresentsAttribute : Attribute
    {
        public readonly Type Type;
        public readonly bool CanUseSettings;
        public RepresentsAttribute(Type type, bool canUseSettings = true)
        {
            Type = type;
            CanUseSettings = canUseSettings;
        }
    }


    //-----------------------------------------------------------------------------
    // InspectableTypeExt
    //-----------------------------------------------------------------------------
    public static partial class InspectableTypeExt
    {
        private static Dictionary<Type, InspectableType> s_typeToInspectableType;
        private static HashSet<InspectableType> s_typesThatCanUseSettings = new HashSet<InspectableType>();
        private static Type[] s_representedType;
        public static InspectableType GetInspectableTypeOf(Type type)
        {
            ensureTypeMapExists();

            // Try a direct lookup
            InspectableType retval;
            if (s_typeToInspectableType.TryGetValue(type, out retval))
            {
                return retval;
            }

            // Try inheritance
            foreach (var kvp in s_typeToInspectableType)
            {
                if (kvp.Key.IsAssignableFrom(type))
                {
                    return kvp.Value;
                }
            }

            Debug.LogWarning(type.Name + " is not an InspectableType");
            return InspectableType.Invalid;
        }

        public static Type GetRepresentedType(InspectableType inspectableType)
        {
            ensureTypeMapExists();

            int index = (int)inspectableType;
            if (index < 0 || index >= s_representedType.Length)
            {
                return null;
            }
            return s_representedType[index];
        }

        public static Type GetSpecificType(InspectableType inspectableType, FieldInfo fieldInfo)
        {
            Debug.Assert(GetRepresentedType(inspectableType).IsAssignableFrom(fieldInfo.FieldType));
            return fieldInfo.FieldType;
        }

        public static bool CanUseSetting(InspectableType inspectableType)
        {
            return s_typesThatCanUseSettings.Contains(inspectableType);
        }

        private static void ensureTypeMapExists()
        {
            if (s_typeToInspectableType != null)
            {
                return;
            }

            s_typeToInspectableType = new Dictionary<Type, InspectableType>();
            var enumType = typeof(InspectableType);
            var enumValues = Enum.GetValues(enumType);
            var enumNames = Enum.GetNames(enumType);
            s_representedType = new Type[enumValues.Length];
            for (int i = 0; i < enumValues.Length; ++i)
            {
                var value = (InspectableType)enumValues.GetValue(i);
                var member = enumType.GetMember(enumNames[i]);
                var attributes = member[0].GetCustomAttributes(typeof(RepresentsAttribute), false);
                foreach (RepresentsAttribute attribute in attributes)
                {
                    s_representedType[(int)value] = attribute.Type;
                    s_typeToInspectableType.Add(attribute.Type, value);
                    if (attribute.CanUseSettings)
                    {
                        s_typesThatCanUseSettings.Add(value);
                    }
                }
            }
        }

    }
}

#endif
