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
using Keys = System.Collections.Generic.List<string>;
using System.Collections.Generic;

namespace GGEZ
{
namespace Omnibus
{


//----------------------------------------------------------------------
// Fub: Functional Unit Block
//----------------------------------------------------------------------
public class Fub : MonoBehaviour, IFub
{


//----------------------------------------------------------------------
// Implement the fub by overriding these methods in a derived class
//----------------------------------------------------------------------
public virtual void OnDidTrigger (string key, object value) {}
public virtual void OnDidChange (string key, object value) {}
public virtual IEnumerable<string> GetKeys ()
    {
    throw new System.NotImplementedException ();
    }

// //----------------------------------------------------------------------
// // Call these methods from the derived class to keep the fub in sync
// //----------------------------------------------------------------------
// protected void onDidAddKey (string key)
//     {
//     if (!this.hasBeenEnabled)
//         {
//         return;
//         }
//     var bus = this.bus as IBus;
//     if (bus != null)
//         {
//         bus.Connect (key, this);
//         }
// #if UNITY_EDITOR
//     this.previousKeys.Add (key);
// #endif
//     }

// protected void onDidRemoveKey (string key)
//     {
//     if (!this.hasBeenEnabled)
//         {
//         return;
//         }
//     var bus = this.bus as IBus;
//     if (bus != null)
//         {
//         bus.Disconnect (key, this);
//         }
// #if UNITY_EDITOR
//     this.previousKeys.Remove (key);
// #endif
//     }


// [SerializeField] private UnityEngine.Object bus; // MonoBehaviour or ScriptableObject
// public IBus Bus
//     {
//     get
//         {
//         return this.bus as IBus;
//         }
//     set
//         {
//         if (object.ReferenceEquals (this.bus, value))
//             {
//             return;
//             }
//         var keys = this.GetKeys ();
//         var bus = this.bus as IBus;
//         if (this.hasBeenEnabled && bus != null)
//             {
//             foreach (string key in keys)
//                 {
//                 if (!string.IsNullOrEmpty (key))
//                     {
//                     bus.Disconnect (key, this);
//                     }
//                 }
//             }
//         this.bus = (UnityEngine.Object)value;
//         bus = value;
// #if UNITY_EDITOR
//         if (this.hasBeenEnabled)
//             {
//             this.previousBus = value;
//             }
// #endif
//         if (this.hasBeenEnabled && bus != null)
//             {
//             foreach (string key in keys)
//                 {
//                 if (!string.IsNullOrEmpty (key))
//                     {
//                     bus.Connect (key, this);
//                     }
//                 }
//             }
//         }
//     }

// protected bool hasBeenEnabled { get; private set; }

// protected void OnEnable ()
//     {
//     var keys = this.GetKeys ();
//     var bus = this.bus as IBus;
//     if (bus != null)
//         {
//         foreach (string key in keys)
//             {
//             if (!string.IsNullOrEmpty (key))
//                 {
//                 bus.Connect (key, this);
//                 }
//             }
//         }
//     this.hasBeenEnabled = true;
// #if UNITY_EDITOR
//     this.previousKeys.Clear ();
//     this.previousKeys.AddRange (keys);
//     this.previousBus = bus;
// #endif
//     }


// protected void OnDisable ()
//     {
//     var bus = this.bus as IBus;
//     if (bus != null)
//         {
//         foreach (string key in this.GetKeys ())
//             {
//             if (!string.IsNullOrEmpty (key))
//                 {
//                 bus.Disconnect (key, this);
//                 }
//             }
//         }
//     this.hasBeenEnabled = false;
// #if UNITY_EDITOR
//     this.previousKeys.Clear ();
//     this.previousBus = null;
// #endif
//     }


// //----------------------------------------------------------------------
// // Handle the Editor changing values in the inspector
// //----------------------------------------------------------------------
// #if UNITY_EDITOR
// #region Editor Runtime
// private Keys previousKeys = new Keys ();
// private IBus previousBus;


// protected void OnValidate ()
//     {
//     var bus = this.bus as IBus;
//     if (bus == null)
//         {
//         var gameObject = this.bus as GameObject;
//         if (gameObject != null)
//             {
//             this.bus = gameObject.GetComponent <Bus> ();
//             bus = this.bus as IBus;
//             }
//         else
//             {
//             this.bus = null;
//             }
//         }

//     if (!this.hasBeenEnabled)
//         {
//         return;
//         }

//     var keys = this.GetKeys ();
//     if (object.ReferenceEquals (this.previousBus, bus))
//         {
//         var removed = new HashSet<string> (this.previousKeys);
//         var added = new HashSet<string> (keys);

//         removed.ExceptWith (keys);
//         added.ExceptWith (this.previousKeys);

//         if (bus != null)
//             {
//             foreach (var key in removed)
//                 {
//                 if (!string.IsNullOrEmpty (key))
//                     {
//                     this.previousBus.Disconnect (key, this);
//                     }
//                 }
//             }
//         this.previousKeys.Clear ();
//         this.previousKeys.AddRange (keys);
//         if (bus != null)
//             {
//             foreach (var key in added)
//                 {
//                 if (!string.IsNullOrEmpty (key))
//                     {
//                     bus.Connect (key, this);
//                     }
//                 }
//             }
//         }
//     else
//         {
//         if (this.previousBus != null)
//             {
//             foreach (var key in this.previousKeys)
//                 {
//                 if (!string.IsNullOrEmpty (key))
//                     {
//                     this.previousBus.Disconnect (key, this);
//                     }
//                 }
//             }
//         this.previousBus = bus;
//         this.previousKeys.Clear ();
//         this.previousKeys.AddRange (keys);
//         if (bus != null)
//             {
//             foreach (var key in keys)
//                 {
//                 if (!string.IsNullOrEmpty (key))
//                     {
//                     bus.Connect (key, this);
//                     }
//                 }
//             }
//         }


//     }

// #endregion
// #endif




}



}

}
