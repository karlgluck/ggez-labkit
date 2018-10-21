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
    // InspectableScriptType
    //-----------------------------------------------------------------------------
    public class InspectableScriptType
    {
        public readonly string Name;
        public readonly Member[] Members;

        public struct Member
        {
            public readonly InspectableType Type;
            public readonly Type SpecificType;
            public readonly FieldInfo FieldInfo;
            public readonly PropertyInfo PropertyInfo;
            public readonly bool WantSetting;
            public readonly bool CanBeNull;
            public readonly bool IsVariable;

            public Member(InspectableType type, Type targetType, FieldInfo fieldInfo, PropertyInfo propertyInfo, bool wantsSetting, bool canBeNull, bool isVariable)
            {
                Type = type;
                SpecificType = targetType;
                FieldInfo = fieldInfo;
                PropertyInfo = propertyInfo;
                WantSetting = wantsSetting;
                CanBeNull = canBeNull;
                IsVariable = isVariable;
            }
        }

        public InspectableScriptType(string name, Member[] members)
        {
            Name = name;
            Members = members;
        }

        private static Dictionary<Type, InspectableScriptType> s_typeToInspectableType = new Dictionary<Type, InspectableScriptType>();
        public static InspectableScriptType GetInspectableScriptType(Type scriptType)
        {
            Debug.Assert(typeof(Script).IsAssignableFrom(scriptType));

            InspectableScriptType retval;
            if (s_typeToInspectableType.TryGetValue(scriptType, out retval))
            {
                return retval;
            }

            MemberInfo[] members = scriptType.GetMembers(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
            var returnedMembers = new Member[members.Length];
            int j = 0;
            for (int i = 0; i < members.Length; ++i)
            {
                Type specificType;
                bool isVariable;
                PropertyInfo property;
                FieldInfo field;

                switch (members[i].MemberType)
                {
                    case MemberTypes.Property:

                        property = scriptType.GetProperty(members[i].Name);
                        if (!typeof(IVariable).IsAssignableFrom(property.PropertyType))
                            break;

                        field = null;
                        specificType = property.PropertyType;
                        isVariable = true;

                        goto AddMember;
                    
                    case MemberTypes.Field:

                        field = scriptType.GetField(members[i].Name);
                        if (typeof(IVariable).IsAssignableFrom(field.FieldType))
                        {
                            Debug.LogError("Script " + scriptType.Name + " has field " + field.Name + " that must be a property");
                            break;
                        }

                        property = null;
                        specificType = field.FieldType;
                        isVariable = false;

                        goto AddMember;
                }

                continue;
            AddMember:

                InspectableType inspectableType = InspectableTypeExt.GetInspectableTypeOf(specificType);
                if (inspectableType == InspectableType.Invalid)
                    continue;

                bool canBeNull = members[i].IsDefined(typeof(CanBeNullAttribute), true);
                bool wantsSetting = members[i].IsDefined(typeof(SettingAttribute), true);
                returnedMembers[j++] = new Member(inspectableType, specificType, field, property, wantsSetting, canBeNull, isVariable);
            }

            retval = new InspectableScriptType(scriptType.Name, returnedMembers);
            s_typeToInspectableType.Add(scriptType, retval);
            return retval;
        }
    }

}

#endif
