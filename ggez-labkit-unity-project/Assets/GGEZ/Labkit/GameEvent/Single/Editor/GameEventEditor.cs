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
public abstract class ScriptableObjectWithFieldBase : ScriptableObject
{
public abstract object GetValue ();
}

[Serializable]
public class ScriptableObjectWithField<T> : ScriptableObjectWithFieldBase
{
public T value;
public override object GetValue ()
    {
    return this.value;
    }
}


[CustomEditor(typeof (BaseGameEvent), true)]
public class GameEventEditor : Editor
{

private FieldInfo listenersField;
private bool listenersFoldout;

private static Dictionary <Type, Type> backingTypeForType = new Dictionary <Type, Type> ();
private static SerializedProperty createSerializedPropertyFor (Type type, out UnityEngine.Object fieldBackingObject)
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
        tb.SetParent (typeof(ScriptableObjectWithField<>).MakeGenericType(type));
        ConstructorBuilder constructor = tb.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
        createdType = tb.CreateType ();
        // Debug.LogFormat ("Type created is {0}", createdType.FullName);
        backingTypeForType.Add (type, createdType);
        }
    var obj = (UnityEngine.Object)ScriptableObject.CreateInstance(createdType);
    // Debug.LogFormat ("Instance = {0}", obj);
    fieldBackingObject = obj;
    return new SerializedObject (obj).FindProperty ("value");
    }

private MethodInfo triggerMethod;
private SerializedProperty triggerParameter;
private UnityEngine.Object fieldBackingObject;
void OnEnable ()
    {
    var targetType = this.target.GetType();
    this.triggerMethod = targetType.GetMethod ("Trigger");
    var parameters = this.triggerMethod == null ? null : this.triggerMethod.GetParameters ();
    if (parameters.Length > 0)
        {
        var eventTriggerType = this.triggerMethod.GetParameters ()[0].ParameterType;
        this.triggerParameter = createSerializedPropertyFor (eventTriggerType, out this.fieldBackingObject);
        }
    else
        {
        this.triggerParameter = null;
        this.fieldBackingObject = null;
        }

    this.listenersField =
            this.targets.Length == 1
            ? this.target.GetType ().GetField ("listeners", BindingFlags.NonPublic | BindingFlags.Instance)
            : null;
    }

void OnDisable ()
    {
    this.triggerParameter = null;
    ScriptableObject.DestroyImmediate (this.fieldBackingObject, false);
    this.fieldBackingObject = null;
    }


public override void OnInspectorGUI ()
    {
    this.serializedObject.UpdateIfRequiredOrScript ();

    EditorGUILayout.Space ();
    EditorGUI.BeginDisabledGroup (!EditorApplication.isPlaying);

    if (this.triggerMethod != null)
        {
        GUILayout.Label ("Manual Trigger", EditorStyles.boldLabel);
        if (this.triggerParameter != null)
            {
            EditorGUILayout.PropertyField (triggerParameter);
            }

        if (GUILayout.Button ("Trigger"))
            {
            if (this.triggerParameter != null)
                {
                this.triggerParameter.serializedObject.ApplyModifiedPropertiesWithoutUndo ();
                foreach (UnityEngine.Object targetObject in this.serializedObject.targetObjects)
                    {
                    this.triggerMethod.Invoke (targetObject, new object[] {((ScriptableObjectWithFieldBase)this.fieldBackingObject).GetValue ()});
                    }
                }
            else
                {
                foreach (UnityEngine.Object targetObject in this.serializedObject.targetObjects)
                    {
                    this.triggerMethod.Invoke (targetObject, null);
                    }
                }
            }
        }

    EditorGUILayout.Space ();
    if (this.listenersField != null)
        {
        var listeners = this.listenersField.GetValue (this.target) as ICollection;
        int numberOfListeners = listeners == null ? 0 : listeners.Count;
        var controlRect = EditorGUILayout.GetControlRect ();
        var numberOfListenersRect = new Rect (controlRect);
        numberOfListenersRect.xMin += EditorGUIUtility.labelWidth;
        this.listenersFoldout = EditorGUI.Foldout (controlRect, this.listenersFoldout, "Listeners", true);
        GUI.Label (numberOfListenersRect, numberOfListeners.ToString ());
        if (this.listenersFoldout && numberOfListeners > 0)
            {
            EditorGUI.BeginDisabledGroup (true);
            EditorGUI.indentLevel++;
            foreach (MonoBehaviour mb in listeners)
                {
                EditorGUI.ObjectField (EditorGUILayout.GetControlRect (), mb, typeof(MonoBehaviour), true);
                }
            EditorGUI.indentLevel--;
            EditorGUI.EndDisabledGroup ();
            }
        EditorGUI.EndDisabledGroup ();
        }

    EditorGUILayout.Space ();

    EditorGUI.EndDisabledGroup ();
    }
}
}
