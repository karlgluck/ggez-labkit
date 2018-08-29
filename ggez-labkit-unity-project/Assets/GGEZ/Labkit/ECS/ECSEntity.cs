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
using UnityEngine;

namespace GGEZ
{
    //---------------------------------------------------------------------------------------
    // Component attached to all game objects used to represent ECS entities.
    //
    // Every ECSBaseComponent holds a reference to this object. You probably don't need to
    // use this class directly. However, if you want to use the Id field, it is guaranteed to
    // be unique among active objects in the current execution. However, it can be recycled
    // when objects are released and re-acquired.
    //---------------------------------------------------------------------------------------
    public class ECSEntity : MonoBehaviour
    {
        public int Id;

        public bool IdIsValid
        {
            get
            {
                return this.Id >= 0;
            }
        }

        [System.NonSerialized]
        internal ECSEntityPool Pool;



        public ECSBaseComponent[] GetComponents()
        {
            Debug.LogWarningFormat("ECSEntity.GetComponents() is slow and should not be used frequently");
            var components = this.gameObject.GetComponents(typeof(ECSBaseComponent));
            return (ECSBaseComponent[])Array.ConvertAll(components, (e) => (ECSBaseComponent)e);
        }



        public static int GetId(GameObject gameObject)
        {
            var entity = (ECSEntity)gameObject.GetComponent(typeof(ECSEntity));
            if (entity == null)
            {
                throw new System.InvalidOperationException("GameObject is not an entity");
            }
            if (!entity.IdIsValid)
            {
                if (!gameObject.activeSelf)
                {
                    throw new System.InvalidOperationException("Entity is in the 'Available' pool; are you holding an old reference?");
                }
                else
                {
                    throw new System.InvalidOperationException("Entity hasn't been assigned an ID; is it a prefab?");
                }
            }
            return entity.Id;
        }
    }
}
