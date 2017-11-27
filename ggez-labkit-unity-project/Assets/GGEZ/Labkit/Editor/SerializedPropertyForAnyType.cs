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

using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection.Emit;


namespace GGEZ
{
namespace Labkit
{

[Serializable]
public abstract class ScriptablePropertyBackingObject : ScriptableObject
{
public abstract object GetValue ();
}

[Serializable]
public class ScriptablePropertyForType<T> : ScriptablePropertyBackingObject
{
public T value;
public override object GetValue ()
    {
    return this.value;
    }
}

//----------------------------------------------------------------------
// Generates a field named "value" of the provided type on an object
// so that Unity's SerializedProperty can be used to access that value.
// This makes it so that we can take advantage of custom property
// drawers.
//
// It is the caller's responsibility to dispose of the SerializedObject
// when it is done using the property. Also, remember that the object's
// value (from ScriptableObjectWithFieldBase.GetValue) will not update
// until the property's serializedObject's ApplyModifiedPropertiesWithoutUndo
// method is called.
//
// This is only for use in editor mode.
//----------------------------------------------------------------------
public static class SerializedPropertyExt
{

public static SerializedProperty GetSerializedPropertyFor (Type type, out ScriptablePropertyBackingObject fieldBackingObject)
    {
    string typeName = "SerializedPropertyFor" + type.Name;
    Type createdType = null;
    if (backingTypeForType.TryGetValue (type, out createdType))
        {
        // Debug.LogFormat ("Type looked up is {0}", createdType.FullName);
        }
    else
        {
        var an = new AssemblyName (typeName);
        AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly (an, AssemblyBuilderAccess.Run);
        ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
        TypeBuilder tb = moduleBuilder.DefineType (typeName,
                TypeAttributes.Public |
                TypeAttributes.Class |
                TypeAttributes.AutoClass |
                TypeAttributes.AnsiClass |
                TypeAttributes.BeforeFieldInit |
                TypeAttributes.AutoLayout,
                null);

        // Derive from a generic class instead of doing a dynamic implementation
        // because the type used might need boxing and it's easier to have the
        // compiler figure out how to handle that.
        tb.SetParent (typeof(ScriptablePropertyForType<>).MakeGenericType(type));

        ConstructorBuilder constructor = tb.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);

        createdType = tb.CreateType ();
        // Debug.LogFormat ("Type created is {0}", createdType.FullName);

        // var bb = (ScriptableObjectWithFieldBase)Activator.CreateInstance (createdType);
        // Debug.Log (bb.GetValue ().GetType ());

        backingTypeForType.Add (type, createdType);
        }
    var obj = (ScriptablePropertyBackingObject)ScriptableObject.CreateInstance(createdType);
    // Debug.LogFormat ("Instance = {0}", obj != null ? "not null" : "NULL");
    fieldBackingObject = obj;
    return new SerializedObject (obj).FindProperty ("value");
    }

private static Dictionary <Type, Type> backingTypeForType = new Dictionary <Type, Type> ();




}
}
}
