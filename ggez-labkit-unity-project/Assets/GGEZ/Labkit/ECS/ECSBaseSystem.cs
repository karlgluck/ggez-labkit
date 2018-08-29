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

namespace GGEZ
{
    //---------------------------------------------------------------------------------------
    // Derive your own base system from ECSBaseSystem. Set up by initializing the Controller
    // reference to your global ECSController. Each frame of simulation, use ForEachEntity
    // to receive a callback for every entity with the given set of components. The
    // components should be derived from ECSBaseComponent.
    //
    // If you need more than 4 component types, this.Controller.ForEachEntity can be called
    // manually with a delegate with any number of parameters.
    //
    // Acquire & Release are used to acquire & release entity prefabs
    // AcquireComponent & ReleaseComponent are used to add/remove components from an entity
    //---------------------------------------------------------------------------------------
    public class ECSBaseSystem : MonoBehaviour
    {
        [System.NonSerialized]
        public ECSController Controller;



        public ECSEntity Acquire(GameObject prefab, Transform parent)
        {
            return this.Controller.Acquire(prefab, parent);
        }




        public void Release(ECSEntity instance)
        {
            this.Controller.Release(instance);
        }




        public ECSBaseComponent AcquireComponent(ECSEntity entity, Type componentType)
        {
            return this.Controller.AcquireComponent(entity, componentType);
        }




        public void ReleaseComponent(ECSBaseComponent component)
        {
            this.Controller.ReleaseComponent(component);
        }




        public void ForEachEntity<T0>(Action<T0> callback)
            where T0 : ECSBaseComponent
        {
            this.Controller.ForEachEntity(callback);
        }




        public void ForEachEntity<T0, T1>(Action<T0, T1> callback)
            where T0 : ECSBaseComponent
            where T1 : ECSBaseComponent
        {
            this.Controller.ForEachEntity(callback);
        }




        public void ForEachEntity<T0, T1, T2>(Action<T0, T1, T2> callback)
            where T0 : ECSBaseComponent
            where T1 : ECSBaseComponent
            where T2 : ECSBaseComponent
        {
            this.Controller.ForEachEntity(callback);
        }




        public void ForEachEntity<T0, T1, T2, T3>(Action<T0, T1, T2, T3> callback)
            where T0 : ECSBaseComponent
            where T1 : ECSBaseComponent
            where T2 : ECSBaseComponent
            where T3 : ECSBaseComponent
        {
            this.Controller.ForEachEntity(callback);
        }
    }
}
