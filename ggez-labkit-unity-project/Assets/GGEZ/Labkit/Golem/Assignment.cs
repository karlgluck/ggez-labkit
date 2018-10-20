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
using System.Reflection;

namespace GGEZ.Labkit
{

    public class Assignment
    {
        public AssignmentType Type;
        public string Name;
        public int RegisterIndex;
        public int TargetIndex;
        public string TargetFieldName;

        public FieldInfo GetObjectFieldInfo(Array array, out object target, out FieldInfo fieldInfo)
        {
            target = array.GetValue(TargetIndex);
            fieldInfo = target.GetType().GetField(TargetFieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            return fieldInfo;
        }

        public FieldInfo GetFieldInfo(object target)
        {
            return target.GetType().GetField(TargetFieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        }
    }

    public enum AssignmentType
    {

        // Used in GolemArchetype:

        AspectGolem,                    // The Golem instance
        AspectSetting,                  // The setting [Name] from the archetype
        AspectAspect,                   // Aspect of the target field's type or null
        [Obsolete]
        AspectVariable,                 // Find [TargetFieldName] on aspect [TargetIndex] and assign it to variable [Name], creating that variable if necessary
        AspectVariableOrNull,           // The existing variable [Name] or null
        AspectVariableOrDummy,          // The existing variable [Name] or a newly allocated variable
        AspectDummyVariable,            // Newly allocated variable

        // Used in GolemComponent:

        CellGolem,                      // The Golem instance
        ScriptGolem,                    // The Golem instance

        CellSetting,                    // The setting [Name] from the archetype
        ScriptSetting,                  // The setting [Name] from the archetype

        CellAspect,                     // Aspect of the target field's type or null
        ScriptAspect,                   // Aspect of the target field's type or null
[Obsolete]
        CellVariable,                   // Find [TargetFieldName] on cell [TargetIndex] and assign it to variable [Name], creating that variable if necessary
        CellVariableOrNull,             // The variable [Name] or null
        CellVariableOrDummy,            // The variable [Name] or a newly allocated variable
        CellRegisterVariable,           // The variable created for register [RegisterIndex]
        CellInputVariableRegisterOrNull,  // The register for variable [Name] or null
        CellInputVariableRegisterOrDummy, // The register for variable [Name] or the global read-only register
        CellInputRegister,              // The register [RegisterIndex] from this Component
        CellOutputRegister,             // The register [RegisterIndex] from this Component
        CellDummyInputRegister,         // Global read-only register
        CellDummyOutputRegister,        // Global write-only register
        CellDummyVariable,              // Newly allocated variable
[Obsolete]
        ScriptVariable,                 // Find [TargetFieldName] on script [TargetIndex] and assign it to variable [Name], creating that variable if necessary
        ScriptVariableOrNull,           // The variable [Name] or null
        ScriptVariableOrDummy,          // The variable [Name] or a newly allocated variable
        ScriptRegisterVariable,         // The variable created for register [RegisterIndex]
        ScriptDummyVariable,            // Newly allocated variable
    }

}