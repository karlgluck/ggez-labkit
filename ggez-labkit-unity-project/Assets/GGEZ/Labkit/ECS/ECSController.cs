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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

//---------------------------------------------------------------------------------------
// ECSController is the worker class for the Entity Component System. You need exactly
// one of these in your scene.
//
// The way I suggest to do this is have 1 GameObject with all ECSBaseSystem-derived
// components in your scene, and add a script to that GameObject that initializes them
// all with the references they need using a function like this:
//
/*
void Awake ()
    {
    var controller = (ECSController)this.gameObject.AddComponent (typeof(ECSController));
    foreach (ECSBaseSystem system in this.gameObject.GetComponents (typeof(ECSBaseSystem)))
        {
        system.Controller = controller;
        }
    }
*/
//
// It's good practice to make singletons into components as well. To do this, it's
// common that you'll add a ECSBaseComponent-derived singleton to an object on your
// scene and set its properties there. If you do this, you'll need to let the ECS know
// to hook those up with their own entities:
/*

    controller.CreateEntitiesForExistingComponents ();

*/
//
// To create your own entities, make a prefab with an ECSEntity and some ECSBaseComponent
// components. Get a reference to the prefab in your code. Then, instantiate the prefab:
/*

    ECSEntity entity = controller.Acquire (prefab);

*/
//
// Components can be added to or removed from an entity using AcquireComponent and
// ReleaseComponent. This can even be done while the component type being removed is
// being iterated.
//---------------------------------------------------------------------------------------

namespace GGEZ
{
public class ECSController : MonoBehaviour
{
private ArrayList entities = new ArrayList ();



public ECSEntity GetEntity (int id)
    {
    if (id < 0 || id >= this.entities.Count)
        {
        throw new ArgumentException ("Invalid Entity ID");
        }
    ECSEntity retval = (ECSEntity)this.entities[id];
    if (retval.Id != id)
        {
        throw new ArgumentException ("Entity is released");
        }
    return retval;
    }



public ECSEntity Acquire (GameObject prefab)
    {
    return Acquire (prefab, null);
    }



public ECSEntity Acquire (GameObject prefab, Transform parent)
    {
    if (prefab == null)
        {
        throw new ArgumentNullException ();
        }
    var prefabEntity = (ECSEntity)prefab.GetComponent (typeof(ECSEntity));
    if (prefabEntity == null)
        {
        prefabEntity = (ECSEntity)prefab.AddComponent (typeof(ECSEntity));
        prefabEntity.Id = int.MinValue;
        }
    if (prefabEntity.Pool == null)
        {
        prefabEntity.Pool = ECSEntityPool.Create (prefab);
        }
    bool isNew;
    GameObject prefabCopy = prefabEntity.Pool.Acquire (parent, out isNew);
    ECSEntity entity = (ECSEntity)prefabCopy.GetComponent (typeof(ECSEntity));
    if (entity == null)
        {
        throw new InvalidOperationException ("Internal Error: Clone of entity prefab doesn't have an ECSEntity component");
        }
    if (entity.IdIsValid)
        {
        throw new InvalidOperationException ("Internal Error: Entity acquired from pool with valid ID. Possibly acquired an existing object.");
        }
    this.configureEntity (entity, prefabEntity.Pool, isNew);
    return entity;
    }



public void CreateEntitiesForExistingComponents ()
    {
    Debug.LogWarning ("CreateEntitiesForExistingComponents() is slow; don't use it frequently");
    var allExistingComponents = (ECSBaseComponent[])GameObject.FindObjectsOfType (typeof(ECSBaseComponent));
    for (int i = 0; i < allExistingComponents.Length; ++i)
        {
        var entityGameObject = allExistingComponents[i].gameObject;
        var entity = (ECSEntity)entityGameObject.GetComponent (typeof(ECSEntity));
        if (entity != null)
            {
            continue;
            }
        entity = (ECSEntity)entityGameObject.AddComponent (typeof(ECSEntity));
        this.configureEntity (entity, null, true);
        }
    }



private void configureEntity (ECSEntity entity, ECSEntityPool pool, bool isNew)
    {
    if (entity == null)
        {
        throw new ArgumentNullException ("entity");
        }
    if (isNew)
        {
        var components = entity.GetComponents();
        for (int i = 0; i < components.Length; ++i)
            {
            components[i].Entity = entity;
            }
        entity.Id = this.entities.Count;
        entity.Pool = pool;
        this.entities.Add (entity);
        }
    else
        {
        entity.Id = reverseEntityId (entity.Id);
        }
    this.debugVerifyEntityId (entity);
    if (this.isIterating)
        {
        this.entitiesCreatedDuringIteration.Add (entity);
        }
    else
        {
        this.insertComponentsOfEntity (entity);
        }
    entity.gameObject.SendMessage ("Acquire", null, SendMessageOptions.DontRequireReceiver);
    }

public void Release (ECSEntity entity)
    {
    if (entity == null)
        {
        throw new ArgumentNullException ("entity");
        }
    this.debugVerifyEntityId (entity);
    entity.gameObject.SendMessage ("Release", null, SendMessageOptions.DontRequireReceiver);
    ECSBaseComponent[] components = entity.GetComponents();
    for (int i = 0; i < components.Length; ++i)
        {
        this.deleteComponent (components[i]);
        }
    if (entity.Pool == null)
        {
        GameObject.Destroy (entity.gameObject);
        this.entities[entity.Id] = null;
        }
    else
        {
        entity.Pool.Release (entity.gameObject);
        entity.Id = reverseEntityId (entity.Id);
        }
    }



internal static string debugComponentArrayToString (ArrayList componentsOfSameType)
    {
    if (componentsOfSameType == null)
        {
        return "null";
        }
    string retval = "";
    string comma = "";
    for (int j = 0; j < componentsOfSameType.Count; ++j)
        {
        var component = ((ECSBaseComponent)((ArrayList)componentsOfSameType)[j]);
        if (component == null)
            {
            retval += comma + "null";
            }
        else
            {
            retval += comma + component.Entity.Id;
            }
        comma = ", ";
        }
    return "[" + retval + "]";
    }



private void deleteComponent (ECSBaseComponent component)
    {
    if (component == null)
        {
        throw new ArgumentNullException ("component");
        }
    ECSEntity entity = component.Entity;
    int entityId = entity.Id;
    var componentsOfSameType = this.getComponentsOfType (component.GetType());
    int index = componentsOfSameType.LinearSearchNullSafe
        (
        delegate (object element)
            {
            return ((ECSBaseComponent)element).Entity.Id - entityId;
            }
        );

    // Debug.LogFormat ("DELETE {0}: {1}", component.GetType().Name, debugComponentArrayToString (componentsOfSameType));

    if (index < 0)
        {
        Debug.LogErrorFormat ("Entity component of type {0} wasn't found for entity {1}", component.GetType().Name, component.Entity.Id);
        return;
        }
    if (!object.ReferenceEquals (entity, ((ECSBaseComponent)componentsOfSameType[index]).Entity))
        {
        throw new InvalidOperationException ("Internal Error: component-entity pairing is messed up");
        }
    if (!object.ReferenceEquals (component, componentsOfSameType[index]))
        {
        throw new InvalidOperationException ("Component found was not the component in the record; are you adding dupliates?");
        }
    componentsOfSameType[index] = null;
    this.anyComponentsWereRemoved = true;
    }



public ECSBaseComponent AcquireComponent (ECSEntity entity, Type componentType)
    {
    if (entity == null || componentType == null)
        {
        throw new ArgumentNullException ();
        }
    if (!typeof(ECSBaseComponent).IsAssignableFrom (componentType))
        {
        throw new ArgumentException ("Component type " + componentType.Name + " must derive from ECSBaseComponent");
        }
    this.debugVerifyEntityId (entity);
    if (null != entity.GetComponent (componentType))
        {
        throw new ArgumentException ("Component type " + componentType.Name + " already exists on entity " + entity.Id);
        }
    var component = (ECSBaseComponent)entity.gameObject.AddComponent (componentType);
    component.Entity = entity;
    if (this.isIterating)
        {
        if (!this.entitiesCreatedDuringIteration.Contains (component.Entity))
            {
            this.componentsAcquiredDuringIteration.Add (component);
            }
        }
    else
        {
        this.insertComponent (component);
        }
    this.debugAssertInvariants ();
    var acquireMethod = componentType.GetMethod ("Acquire", Type.EmptyTypes);
    if (acquireMethod != null)
        {
        acquireMethod.Invoke (component, null);
        }
    return component;
    }



public void ReleaseComponent (ECSBaseComponent component)
    {
    if (component == null)
        {
        throw new ArgumentNullException ();
        }
    var releaseMethod = component.GetType().GetMethod ("Release", Type.EmptyTypes);
    if (releaseMethod != null)
        {
        releaseMethod.Invoke (component, null);
        }
    this.deleteComponent (component);
    GameObject.Destroy (component);
    this.debugAssertInvariants ();
    }



private static int reverseEntityId (int id)
    {
    return -1 - id;
    }



private void debugVerifyEntityId (ECSEntity entity)
    {
    int id = entity.Id;
    if (id < 0
             || id >= this.entities.Count
             || !object.ReferenceEquals (entity, this.entities[id]))
        {
        throw new InvalidOperationException ("Internal Error: Recycled entity doesn't have its old ID");
        }
    }



private Dictionary<Type, ArrayList> components = new Dictionary<Type, ArrayList>();
private ArrayList entitiesCreatedDuringIteration = new ArrayList ();
private ArrayList componentsAcquiredDuringIteration = new ArrayList ();
private bool anyComponentsWereRemoved;
private bool isIterating = false;



private void insertComponentsOfEntity (ECSEntity entity)
    {
    var components = entity.GetComponents();
    for (int i = 0; i < components.Length; ++i)
        {
        this.insertComponent (components[i]);
        }
#if UNITY_EDITOR
    this.debugAssertInvariants ();
#endif
    }



private void insertComponent (ECSBaseComponent component)
    {
    if (this.anyComponentsWereRemoved)
        {
        this.removeNullsFromComponentsLists ();
        }
    var componentsOfSameType = this.getComponentsOfType (component.GetType());
    componentsOfSameType.BinaryInsert (component);

    // Debug.LogFormat ("INSERT {0} #{2}: {1}", component.GetType().Name, debugComponentArrayToString (componentsOfSameType), component.Entity.Id);
    }



private ArrayList getComponentsOfType (Type type)
    {
    ArrayList retval;
    if (!this.components.TryGetValue (type, out retval))
        {
        retval = new ArrayList ();
        this.components[type] = retval;
        }
    return retval;
    }



private void removeNullsFromComponentsLists ()
    {
    foreach (var kv in this.components)
        {
        kv.Value.RemoveNullElementsStable ();
        }
    this.anyComponentsWereRemoved = false;
    }

private int[] _current = new int[16];
private ArrayList[] _allComponentsOfParameterType = new ArrayList[16];
private ECSBaseComponent[][] _components = new ECSBaseComponent[][] {
        new ECSBaseComponent[ 0], new ECSBaseComponent[ 1], new ECSBaseComponent[ 2], new ECSBaseComponent[ 3],
        new ECSBaseComponent[ 4], new ECSBaseComponent[ 5], new ECSBaseComponent[ 6], new ECSBaseComponent[ 7],
        new ECSBaseComponent[ 8], new ECSBaseComponent[ 9], new ECSBaseComponent[10], new ECSBaseComponent[11],
        new ECSBaseComponent[12], new ECSBaseComponent[13], new ECSBaseComponent[14], new ECSBaseComponent[15],
        };



public void ForEachEntity (Delegate callback)
    {
    if (this.anyComponentsWereRemoved)
        {
        this.removeNullsFromComponentsLists ();
        }
    var parameters = callback.Method.GetParameters ();
    if (parameters.Length > this._allComponentsOfParameterType.Length)
        {
        throw new InvalidOperationException ("Too many components used in callback");
        }
    // Debug.LogFormat ("-------------------------------------------------");
    var current                      = this._current;
    var components                   = this._components[parameters.Length];
    var allComponentsOfParameterType = this._allComponentsOfParameterType;
    int _sentinel = 0;
    // Debug.LogFormat ("---------------");
    for (int i = 0; i < parameters.Length; ++i)
        {
        current[i] = 0;
        var parameterType = parameters[i].ParameterType;
        if (!typeof(ECSBaseComponent).IsAssignableFrom(parameterType))
            {
            throw new InvalidOperationException ("Parameter "+i+" ("+parameterType.Name+") does not derive from ECSBaseComponent");
            }
        allComponentsOfParameterType[i] = this.getComponentsOfType (parameterType);
        // Debug.LogFormat ("{0}: {1} ({2} components)", i, parameterType.Name, allComponentsOfParameterType[i].Count);
        _sentinel += ((ArrayList)allComponentsOfParameterType[i]).Count + 1;
        // Debug.LogFormat ("{0}: {1}", parameterType.Name, debugComponentArrayToString (allComponentsOfParameterType));
        }
    _sentinel *= 2;
    this.isIterating = true;
    int maxEntityId = int.MinValue;
    bool anyEntitiesAreLeft = true;
    while (anyEntitiesAreLeft && _sentinel-- > 0)
        {
        // Debug.LogFormat ("{0} [max = {1}]: {2}", _sentinel, maxEntityId, Util.ObjectToJsonPretty (current));
        for (int i = 0; i < parameters.Length; ++i)
            {
            var allComponents = (ArrayList)allComponentsOfParameterType[i];
            while (current[i] < allComponents.Count)
                {
                var component = (ECSBaseComponent)allComponents[current[i]];
                if (component != null)
                    {
                    int id = component.Entity.Id;
                    if (id == maxEntityId)
                        {
                        components[i] = component;
                        break;
                        }
                    if (id > maxEntityId)
                        {
                        maxEntityId = id;
                        goto EndOfEntityIterationLoop;
                        }
                    }
                ++current[i];
                }
            if (current[i] >= allComponents.Count)
                {
                anyEntitiesAreLeft = false;
                goto EndOfEntityIterationLoop;
                }
            }
        bool allComponentsAreFromSameEntity = true;
        if (allComponentsAreFromSameEntity)
            {
            for (int i = 0; i < parameters.Length; ++i)
                {
                if (!object.ReferenceEquals (((ECSBaseComponent)components[0]).Entity, ((ECSBaseComponent)components[i]).Entity))
                    {
                    throw new InvalidOperationException ("Internal Error: Components not all from the same entity in callback");
                    }
                }
            callback.Method.Invoke (callback.Target, components);
            for (int i = 0; i < parameters.Length; ++i)
                {
                ++current[i];
                }
            maxEntityId = int.MinValue;
            }
        EndOfEntityIterationLoop:
        #pragma warning disable 0168
        int _nop;
        #pragma warning restore 0168
        }
    if (anyEntitiesAreLeft)
        {
        throw new InvalidOperationException ("Infinite loop: should never iterate more times than the entire list of components");
        }

    for (int i = 0; i < this.entitiesCreatedDuringIteration.Count; ++i)
        {
        var entity = (ECSEntity)this.entitiesCreatedDuringIteration[i];
        int j = 0;
        while (j < parameters.Length)
            {
            object component = entity.gameObject.GetComponent (parameters[j].ParameterType);
            if (component == null)
                {
                break;
                }
            components[j] = (ECSBaseComponent)component;
            ++j;
            }
        if (j == parameters.Length)
            {
            callback.Method.Invoke (callback.Target, components);
            }
        }

    for (int i = 0; i < this.entitiesCreatedDuringIteration.Count; ++i)
        {
        var entity = (ECSEntity)this.entitiesCreatedDuringIteration[i];
        this.insertComponentsOfEntity (entity);
        }
    this.entitiesCreatedDuringIteration.Clear ();

    for (int i = 0; i < this.componentsAcquiredDuringIteration.Count; ++i)
        {
        var componentAcquiredDuringIteration = (ECSBaseComponent)this.componentsAcquiredDuringIteration[i];
        var entity = componentAcquiredDuringIteration.Entity;
        var componentType = componentAcquiredDuringIteration.GetType();
        bool anyParameterTypeIsThisComponentsType = false;
        for (int pi = 0; pi < parameters.Length; ++pi)
            {
            if (parameters[pi].ParameterType.Equals (componentType))
                {
                anyParameterTypeIsThisComponentsType = true;
                break;
                }
            }
        if (!anyParameterTypeIsThisComponentsType)
            {
            continue;
            }
        int j = 0;
        while (j < parameters.Length)
            {
            object component = entity.gameObject.GetComponent (parameters[j].ParameterType);
            if (component == null)
                {
                break;
                }
            components[j] = (ECSBaseComponent)component;
            ++j;
            }
        if (j == parameters.Length)
            {
            callback.Method.Invoke (callback.Target, components);
            }
        }
    this.componentsAcquiredDuringIteration.Clear ();

    if (this.entitiesCreatedDuringIteration.Count > 0)
        {
        Debug.LogErrorFormat ("New entities created while iterating components acquired during iteration; possible feedback loop?");
        }

    this.isIterating = false;       
    }



#if UNITY_EDITOR
private void debugAssertInvariants ()
    {
    foreach (var kv in this.components)
        {
        var componentsOfSameType = kv.Value;
        for (int i = 0; i < componentsOfSameType.Count; ++i)
            {
            if (componentsOfSameType[i] == null)
                {
                continue;
                }
            if (!componentsOfSameType[i].GetType().Equals (kv.Key))
                {
                throw new InvalidOperationException ("Wrong type stored in component list");
                }
            if (i > 0)
                {
                var last = ((ECSBaseComponent)componentsOfSameType[i-1]);
                var current = ((ECSBaseComponent)componentsOfSameType[i]);

                if ((last != null) && (current != null) && (last.Entity.Id >= current.Entity.Id))
                    {
                    for (int j = 0; j < componentsOfSameType.Count; ++j)
                        {
                        Debug.LogErrorFormat ("[{0}] = {1}", j, ((ECSBaseComponent)componentsOfSameType[j]).Entity.Id);
                        }
                    throw new InvalidOperationException ("Entities are out of order");
                    }
                }
            }
        }
    }
#endif


}
}
