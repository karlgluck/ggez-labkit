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
using UnityEngine;

//---------------------------------------------------------------------------------------
// You don't need to explicitly use this class.
//
// ECSEntityPool manages pooling of entity game objects via the ECSController's Acquire
// and Release methods.
//---------------------------------------------------------------------------------------

namespace GGEZ
{
public class ECSEntityPool : MonoBehaviour
{
private GameObject prefab;
private ArrayList available = new ArrayList ();
private Transform availableContainerTransform;



public static ECSEntityPool Create (GameObject prefab)
    {
    if (prefab == null)
        {
        throw new ArgumentNullException ("prefab");
        }
    var gameObject = new GameObject ("Entity Pool: " + prefab.name);
    GameObject.DontDestroyOnLoad (gameObject);
    var pool = (ECSEntityPool)gameObject.AddComponent (typeof(ECSEntityPool));
    pool.prefab = prefab;
    const bool createAvailableContainer = true;
    if (createAvailableContainer)
        {
        var availableContainer = new GameObject ("Available");
        availableContainer.transform.SetParent (gameObject.transform, false);
        availableContainer.SetActive (false);
        pool.availableContainerTransform = availableContainer.transform;
        }
    return pool;
    }



public GameObject Acquire (Transform parent, out bool isNew)
    {
    GameObject instance = null;
    int availableIndex = this.available.Count - 1;
    isNew = availableIndex < 0;
    if (isNew)
        {
        instance = (GameObject)GameObject.Instantiate (this.prefab);
        }
    else
        {
        instance = (GameObject)this.available[availableIndex];
        this.available.RemoveAt (availableIndex);
        }
    if (parent != null)
        {
        instance.transform.SetParent (parent, false);
        }
    return instance;
    }



public void Release (GameObject instance)
    {
    if (instance == null)
        {
        throw new ArgumentNullException ("instance");
        }
    instance.transform.SetParent (this.availableContainerTransform, false);
    this.available.Add (instance);
    }
}
}
