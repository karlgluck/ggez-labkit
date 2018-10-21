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
using UnityEngine;
using System.Collections.Generic;

namespace GGEZ.Labkit
{

    public enum AssignmentType
    {

        // Used in GolemArchetype:

        AspectGolem,                    // The Golem instance
        AspectSetting,                  // The setting [Name] from the archetype
        AspectAspect,                   // Aspect of the target field's type or null

        AspectLocalVariable,            // The variable [Name]
        AspectVariableOrNull,           // The variable [Name] or null
        AspectVariableOrDummy,          // The variable [Name] or a newly allocated variable
        AspectDummyVariable,            // Newly allocated variable

        // Used in GolemComponent:

        CellGolem,                      // The Golem instance
        ScriptGolem,                    // The Golem instance

        CellSetting,                    // The setting [Name] from the archetype
        ScriptSetting,                  // The setting [Name] from the archetype

        CellAspect,                     // Aspect of the target field's type or null
        ScriptAspect,                   // Aspect of the target field's type or null

        CellLocalVariable,              // The variable [Name]
        CellVariableOrNull,             // The variable [Name] or null
        CellVariableOrDummy,            // The variable [Name] or a newly allocated variable
        CellInputVariableRegisterOrNull,  // The register for variable [Name] or null
        CellInputVariableRegisterOrDummy, // The register for variable [Name] or the global read-only register
        CellInputRegister,              // The register [RegisterIndex] from this Component
        CellOutputRegister,             // The register [RegisterIndex] from this Component
        CellDummyInputRegister,         // Global read-only register
        CellDummyOutputRegister,        // Global write-only register
        CellDummyVariable,              // Newly allocated variable

        ScriptLocalVariable,            // The variable [Name]
        ScriptVariableOrNull,           // The variable [Name] or null
        ScriptVariableOrDummy,          // The variable [Name] or a newly allocated variable
        ScriptRegisterVariable,         // The variable created for register [RegisterIndex]
        ScriptDummyVariable,            // Newly allocated variable
    }

    public class Assignment
    {
        public AssignmentType Type;
        public string Name;
        public int RegisterIndex;
        public int TargetIndex;
        public string TargetFieldName;

        private static readonly Dictionary<string, IVariable> s_emptyVariables = new Dictionary<string, IVariable>();

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

        public static void Assign(Golem golem, Assignment[] assignments, Dictionary<string, IVariable> variables, GolemComponentRuntimeData component, IRegister[] registers)
        {
            variables = variables ?? s_emptyVariables;

            IVariable[] registerVariables = null;
            for (int assignmentIndex = 0; assignmentIndex < assignments.Length; ++assignmentIndex)
                DoAssignment(golem, assignments[assignmentIndex], variables, component, registers, ref registerVariables);
        }

        private static void DoAssignment(
            Golem golem,
            Assignment assignment,
            Dictionary<string, IVariable> variables,
            GolemComponentRuntimeData component,
            IRegister[] registers,
            ref IVariable[] registerVariables
            )
        {
            object target;
            FieldInfo fieldInfo;

            switch (assignment.Type)
            {
                case AssignmentType.AspectGolem:  assignment.GetObjectFieldInfo(golem.Aspects,     out target, out fieldInfo).SetValue(target, golem); break;
                case AssignmentType.CellGolem:    assignment.GetObjectFieldInfo(component.Cells,   out target, out fieldInfo).SetValue(target, golem); break;
                case AssignmentType.ScriptGolem:  assignment.GetObjectFieldInfo(component.Scripts, out target, out fieldInfo).SetValue(target, golem); break;

                case AssignmentType.AspectSetting:  assignment.GetObjectFieldInfo(golem.Aspects,     out target, out fieldInfo).SetValue(target, golem.Settings.Get(assignment.Name, fieldInfo.FieldType)); break;
                case AssignmentType.CellSetting:    assignment.GetObjectFieldInfo(component.Cells,   out target, out fieldInfo).SetValue(target, golem.Settings.Get(assignment.Name, fieldInfo.FieldType)); break;
                case AssignmentType.ScriptSetting:  assignment.GetObjectFieldInfo(component.Scripts, out target, out fieldInfo).SetValue(target, golem.Settings.Get(assignment.Name, fieldInfo.FieldType)); break;

                case AssignmentType.AspectAspect:   assignment.GetObjectFieldInfo(golem.Aspects,     out target, out fieldInfo).SetValue(target, golem.GetAspect(fieldInfo.FieldType)); break;
                case AssignmentType.CellAspect:     assignment.GetObjectFieldInfo(component.Cells,   out target, out fieldInfo).SetValue(target, golem.GetAspect(fieldInfo.FieldType)); break;
                case AssignmentType.ScriptAspect:   assignment.GetObjectFieldInfo(component.Scripts, out target, out fieldInfo).SetValue(target, golem.GetAspect(fieldInfo.FieldType)); break;

                case AssignmentType.AspectLocalVariable: assignment.GetObjectFieldInfo(golem.Aspects,     out target, out fieldInfo).SetValue(target, GetLocalVariable(variables, assignment.Name, fieldInfo.FieldType)); break;
                case AssignmentType.CellLocalVariable:   assignment.GetObjectFieldInfo(component.Cells,   out target, out fieldInfo).SetValue(target, GetLocalVariable(variables, assignment.Name, fieldInfo.FieldType)); break;
                case AssignmentType.ScriptLocalVariable: assignment.GetObjectFieldInfo(component.Scripts, out target, out fieldInfo).SetValue(target, GetLocalVariable(variables, assignment.Name, fieldInfo.FieldType)); break;

                case AssignmentType.AspectVariableOrNull:   assignment.GetObjectFieldInfo(golem.Aspects,     out target, out fieldInfo).SetValue(target, GetVariableOrNull(variables, assignment.Name)); break;
                case AssignmentType.CellVariableOrNull:     assignment.GetObjectFieldInfo(component.Cells,   out target, out fieldInfo).SetValue(target, GetVariableOrNull(variables, assignment.Name)); break;
                case AssignmentType.ScriptVariableOrNull:   assignment.GetObjectFieldInfo(component.Scripts, out target, out fieldInfo).SetValue(target, GetVariableOrNull(variables, assignment.Name)); break;

                case AssignmentType.AspectVariableOrDummy:   assignment.GetObjectFieldInfo(golem.Aspects,     out target, out fieldInfo).SetValue(target, GetVariableOrDummy(variables, assignment.Name, fieldInfo.FieldType)); break;
                case AssignmentType.CellVariableOrDummy:     assignment.GetObjectFieldInfo(component.Cells,   out target, out fieldInfo).SetValue(target, GetVariableOrDummy(variables, assignment.Name, fieldInfo.FieldType)); break;
                case AssignmentType.ScriptVariableOrDummy:   assignment.GetObjectFieldInfo(component.Scripts, out target, out fieldInfo).SetValue(target, GetVariableOrDummy(variables, assignment.Name, fieldInfo.FieldType)); break;

                case AssignmentType.ScriptRegisterVariable:   assignment.GetObjectFieldInfo(component.Scripts, out target, out fieldInfo).SetValue(target, GetOrCreateRegisterVariable(assignment.RegisterIndex, registers, ref registerVariables)); break;

                case AssignmentType.CellInputVariableRegisterOrNull:
                    {
                        Cell targetCell = component.Cells[assignment.TargetIndex];
                        fieldInfo = assignment.GetFieldInfo(targetCell);
                        {
                            IRegister oldRegister = fieldInfo.GetValue(targetCell) as IRegister;
                            if (oldRegister != null)
                            {
                                oldRegister.RemoveListener(targetCell);
                            }
                        }
                        {
                            IRegister register = GetVariableRegisterOrNull(variables, assignment.Name);
                            if (register != null)
                            {
                                register.AddListener(targetCell);
                            }
                            fieldInfo.SetValue(targetCell, register);
                        }
                    }
                    break;

                case AssignmentType.CellInputVariableRegisterOrDummy:
                    {
                        Cell targetCell = component.Cells[assignment.TargetIndex];
                        fieldInfo = assignment.GetFieldInfo(targetCell);
                        {
                            IRegister oldRegister = fieldInfo.GetValue(targetCell) as IRegister;
                            if (oldRegister != null)
                            {
                                oldRegister.RemoveListener(targetCell);
                            }
                        }
                        {
                            IRegister register = GetVariableRegisterOrNull(variables, assignment.Name);
                            if (register == null)
                            {
                                register = GetReadonlyRegister(fieldInfo.FieldType);
                            }
                            else
                            {
                                register.AddListener(targetCell);
                            }
                            fieldInfo.SetValue(targetCell, register);
                        }
                    }
                    break;

                case AssignmentType.CellInputRegister:
                    {
                        #warning we only need to update cell input registers when the golem is brand new

                        Cell targetCell = component.Cells[assignment.TargetIndex];
                        fieldInfo = assignment.GetFieldInfo(targetCell);
                        {
                            IRegister oldRegister = fieldInfo.GetValue(targetCell) as IRegister;
                            if (oldRegister != null)
                            {
                                oldRegister.RemoveListener(targetCell);
                            }
                        }
                        {
                            IRegister register = registers[assignment.RegisterIndex];
                            register.AddListener(targetCell);
                            fieldInfo.SetValue(targetCell, register);
                        }
                    }
                    break;

                case AssignmentType.CellOutputRegister:         assignment.GetObjectFieldInfo(component.Cells,   out target, out fieldInfo).SetValue(target, registers[assignment.RegisterIndex]); break;
                case AssignmentType.CellDummyInputRegister:     assignment.GetObjectFieldInfo(component.Cells, out target, out fieldInfo).SetValue(target, GetReadonlyRegister(fieldInfo.FieldType)); break;
                case AssignmentType.CellDummyOutputRegister:    assignment.GetObjectFieldInfo(component.Cells, out target, out fieldInfo).SetValue(target, GetWriteonlyRegister(fieldInfo.FieldType)); break;
                case AssignmentType.CellDummyVariable:          assignment.GetObjectFieldInfo(component.Cells, out target, out fieldInfo).SetValue(target, GetWriteonlyRegister(fieldInfo.FieldType)); break;
                case AssignmentType.ScriptDummyVariable:        assignment.GetObjectFieldInfo(component.Scripts, out target, out fieldInfo).SetValue(target, GetDummyVariable(fieldInfo.FieldType)); break;
                case AssignmentType.AspectDummyVariable:        assignment.GetObjectFieldInfo(golem.Aspects, out target, out fieldInfo).SetValue(target, GetDummyVariable(fieldInfo.FieldType)); break;
            }
        }

        private static IVariable GetVariableOrNull(Dictionary<string, IVariable> variables, string name)
        {
            IVariable variable;
            variables.TryGetValue(name, out variable);
            return variable;
        }

        private static IVariable GetLocalVariable(Dictionary<string, IVariable> variables, string name, Type type)
        {
            Debug.Assert(typeof(IVariable).IsAssignableFrom(type));
            IVariable variable;
            if (!variables.TryGetValue(name, out variable))
            {
                Debug.LogError("Local variable '" + name + "' of type " + type.Name + " does not exist!");
                variable = Activator.CreateInstance(type) as IVariable;
                variables.Add(name, variable);
            }
            Debug.Assert(variable != null);
            return variable;
        }

        private static IVariable GetOrCreateRegisterVariable(int index, IRegister[] registers, ref IVariable[] registerVariables)
        {
            Debug.Assert(registers != null);
            if (registerVariables == null)
            {
                registerVariables = new IVariable[registers.Length];
            }
            IVariable variable = registerVariables[index];
            if (registerVariables[index] == null)
            {
                variable = registers[index].CreateVariable();
                registerVariables[index] = variable;
            }
            return variable;
        }

        private static IVariable GetVariableOrDummy(Dictionary<string, IVariable> variables, string name, Type type)
        {
            Debug.Assert(typeof(IVariable).IsAssignableFrom(type));
            IVariable variable;
            if (!variables.TryGetValue(name, out variable))
            {
                variable = Activator.CreateInstance(type) as IVariable;
            }
            Debug.Assert(variable != null);
            return variable;
        }

        private static IRegister GetVariableRegisterOrNull(Dictionary<string, IVariable> variables, string name)
        {
            IVariable variable;

            if (variables.TryGetValue(name, out variable))
            {
                return variable.GetRegister();
            }

            return null;
        }

        private class Defaults
        {
            public IRegister InputRegister;
            public IRegister OutputRegister;
            public IVariable InputVariable;
            public IVariable OutputVariable;

            private static Dictionary<Type, Defaults> s_cache = new Dictionary<Type, Defaults>();

            public static Defaults GetDefaultsForType(Type fieldType)
            {
                Defaults value;

                if (!s_cache.TryGetValue(fieldType, out value))
                {
                    value = new Defaults();

                    if (typeof(IVariable).IsAssignableFrom(fieldType))
                    {
                        value.InputVariable = Activator.CreateInstance(fieldType) as IVariable;
                        value.InputRegister = value.InputVariable.GetRegister();
                        value.OutputRegister = value.InputRegister.Clone();
                        value.OutputVariable = value.OutputRegister.CreateVariable();
                    }
                    else
                    {
                        value.InputRegister = Activator.CreateInstance(fieldType) as IRegister;
                        value.OutputRegister = value.InputRegister.Clone();
                        value.InputVariable = value.InputRegister.CreateVariable();
                        value.OutputVariable = value.OutputRegister.CreateVariable();
                    }

                    s_cache.Add(value.InputRegister.GetType(), value);
                    s_cache.Add(value.InputVariable.GetType(), value);
                }

                return value;
            }

        }

        /// <summary></summary>
        private static IRegister GetReadonlyRegister(Type fieldType)
        {
            Debug.Assert(typeof(IRegister).IsAssignableFrom(fieldType));
            return Defaults.GetDefaultsForType(fieldType).InputRegister;
        }

        /// <summary></summary>
        private static IRegister GetWriteonlyRegister(Type fieldType)
        {
            Debug.Assert(typeof(IRegister).IsAssignableFrom(fieldType));
            return Defaults.GetDefaultsForType(fieldType).OutputRegister;
        }

        /// <summary></summary>
        private static IVariable GetReadonlyVariable(Type fieldType)
        {
            Debug.Assert(typeof(IVariable).IsAssignableFrom(fieldType));
            return Defaults.GetDefaultsForType(fieldType).InputVariable;
        }

        /// <summary></summary>
        private static IVariable GetWriteonlyVariable(Type fieldType)
        {
            Debug.Assert(typeof(IVariable).IsAssignableFrom(fieldType));
            return Defaults.GetDefaultsForType(fieldType).OutputVariable;
        }

        private static IVariable GetDummyVariable(Type fieldType)
        {
            Debug.Assert(typeof(IVariable).IsAssignableFrom(fieldType));
            IVariable instance = Activator.CreateInstance(fieldType) as IVariable;
            Debug.Assert(instance != null);
            Debug.Assert(instance.GetRegister() != null);
            return instance;
        }



    }


}