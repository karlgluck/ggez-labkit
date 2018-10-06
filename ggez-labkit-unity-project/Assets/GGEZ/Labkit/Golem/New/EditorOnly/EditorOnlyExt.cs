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
using UnityEditor;
using UnityObject = UnityEngine.Object;
using UnityObjectList = System.Collections.Generic.List<UnityEngine.Object>;
using System.Collections.Generic;
using System.Reflection;

#if UNITY_EDITOR

namespace GGEZ.Labkit
{
    public static class GolemEditorOnlyExt
    {
        public static bool ContainsVariable(this GolemArchetype self, string name, Type type)
        {
            if (name == null || type == null)
            {
                return false;
            }
            for (int i = 0; i < self.EditorVariables.Count; ++i)
            {
                if (self.EditorVariables[i].Name == name)
                {
                    return type.IsAssignableFrom(self.EditorVariables[i].Type);
                }
            }
            return false;
        }
    
        public static void AddNewAspect(this GolemArchetype self, Aspect aspect)
        {
            if (aspect == null)
            {
                throw new ArgumentNullException("aspect");
            }
            var aspectType = aspect.GetType();
            
            // Make sure this aspect doesn't already exist
            for (int i = 0; i < self.Aspects.Length; ++i)
            {
                Debug.Assert(!self.Aspects[i].GetType().Equals(aspectType));
            }

            var editorAspect = new GolemAspectEditorData();
            editorAspect.Field = null;
            editorAspect.Aspect = aspect;
            editorAspect.AspectFields = InspectableFieldInfo.GetFields(aspect);
            editorAspect.AspectVariables = InspectableVariablePropertyInfo.GetVariableProperties(aspect);
            self.EditorAspects.Add(editorAspect);

            // Mark fields for settings that want them
            for (int i = 0; i < editorAspect.AspectFields.Length; ++i)
            {
                var fieldOfAspect = editorAspect.AspectFields[i];
                if (fieldOfAspect.WantsSetting)
                {
                    editorAspect.FieldsUsingSettings.Add(fieldOfAspect.FieldInfo.Name, null);
                }
            }

            // Clean up FieldsUsingSettings to only include existing fields
            {
                foreach (var key in editorAspect.FieldsUsingSettings.Keys.Where(
                    (fieldName) => !editorAspect.AspectFields.Any((e) => e.FieldInfo.Name == fieldName)
                    ).ToArray())
                {
                    editorAspect.FieldsUsingSettings.Remove(key);
                }
            }

            ApplyFieldsUsingSettings(aspect, editorAspect.FieldsUsingSettings, Settings);

            // Add all of the aspect's declared variables
            for (int i = 0; i < editorAspect.AspectVariables.Length; ++i)
            {
                InspectableVariablePropertyInfo aspectVariable = editorAspect.AspectVariables[i];
                string variableName = aspectVariable.VariableAttribute.Name;
                string variableTooltip = aspectVariable.VariableAttribute.Tooltip;
                bool notFound = true;
                for (int j = 0; notFound && j < self.EditorVariables.Count; ++j)
                {
                    if (self.EditorVariables[j].Name == variableName)
                    {
                        self.EditorVariables[j].SourceAspects.Add(aspectType);
                        if (!string.IsNullOrEmpty(variableTooltip))
                        {
                            self.EditorVariables[j].Tooltip = variableTooltip;
                        }
                        notFound = false;
                    }
                }
                if (notFound)
                {
                    Type type = aspectVariable.PropertyInfo.PropertyType;
                    var variableEditorData = new GolemVariableEditorData()
                    {
                        Name = variableName,
                        Tooltip = variableTooltip,
                        SourceAspects = new HashSet<Type>(){ aspectType },
                        InspectableType = aspectVariable.Type,
                        Type = aspectVariable.PropertyInfo.PropertyType,
                        InitialValue = type.IsValueType ? Activator.CreateInstance(type) : null,
                    };
                    self.EditorVariables.Add(variableEditorData);
                }
            }
        }

        public static void RemoveAspect(this GolemArchetype self, GolemAspectEditorData editableAspect)
        {
            if (editableAspect == null)
            {
                throw new ArgumentNullException("editableAspect");
            }
            var index = self.EditorAspects.IndexOf(editableAspect);
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("editableAspect");
            }
            self.EditorAspects.RemoveAt(index);
            var aspectType = editableAspect.Aspect.GetType();
            for (int i = 0; i < self.EditorVariables.Count; ++i)
            {
                self.EditorVariables[i].SourceAspects.Remove(aspectType);
            }
            RemoveEditorVariablesWithoutSourceAspects(self);
        }

        public static void RemoveEditorVariablesWithoutSourceAspects(this GolemArchetype self)
        {
            for (int i = self.EditorVariables.Count - 1; i >= 0; --i)
            {
                if (self.EditorVariables[i].SourceAspects.Count == 0)
                {
                    self.EditorVariables.RemoveAt(i);
                }
            }
        }
    }

}

#endif
