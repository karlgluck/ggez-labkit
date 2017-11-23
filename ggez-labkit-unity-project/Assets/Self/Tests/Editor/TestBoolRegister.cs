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
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using GGEZ;
using System.Linq;
using System.Reflection;

public class TestBoolRegister
{

public T CreateWithMonoBehaviour<T> () where T : Component
    {
    var go = new GameObject (typeof(T).FullName);
    var retval = go.AddComponent <T> ();
    return retval;
    }

public void CallOnEnable (MonoBehaviour mb)
    {
    var method = mb.GetType().GetMethod ("OnEnable", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
    method.Invoke (mb, null);
    }

public void CallOnDisable (MonoBehaviour mb)
    {
    var method = mb.GetType().GetMethod ("OnDisable", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
    method.Invoke (mb, null);
    }

[Test]
public void ListenerRegisterAndUnregister ()
    {
    var register = ScriptableObject.CreateInstance <BoolRegister> ();
    var firstListener = CreateWithMonoBehaviour <BoolRegisterListener> ();
    this.CallOnEnable (firstListener);

    Assert.AreEqual (register.Listeners.Count, 0, "no listeners at start");
    firstListener.Register = register;
    Assert.AreEqual (register.Listeners.Count, 1, "added 1 listener");

    var secondListener = CreateWithMonoBehaviour <BoolRegisterListener> ();
    secondListener.Register = register;
    Assert.AreEqual (register.Listeners.Count, 1, "added second listener but not enabled");
    this.CallOnEnable (secondListener);
    Assert.AreEqual (register.Listeners.Count, 2, "enabled second listener");

    this.CallOnDisable (secondListener); // must be explicit; no coroutines or frames in test mode
    GameObject.DestroyImmediate (secondListener.gameObject);
    Assert.AreEqual (register.Listeners.Count, 1, "destroyed second listener");

    this.CallOnDisable (firstListener); // must be explicit; no coroutines or frames in test mode
    GameObject.DestroyImmediate (firstListener.gameObject);
    Assert.AreEqual (register.Listeners.Count, 0, "destroyed first listener");

    ScriptableObject.DestroyImmediate (register, false);
    }


[Test]
public void ListenerGetsCalledWhenEnabledThenRegisterSetAndNotBefore ()
    {
    var register = ScriptableObject.CreateInstance <BoolRegister> ();
    var listener = CreateWithMonoBehaviour <BoolRegisterListener> ();
    int incrementWhenCalled = 0;
    listener.AddDidChangeCallback ( (bool value) => { ++incrementWhenCalled; } );
    this.CallOnEnable (listener);

    Assert.AreEqual (0, incrementWhenCalled, "no callbacks until register is set");
    listener.Register = register;
    Assert.AreEqual (1, incrementWhenCalled, "listener gets callback when register is set");

    GameObject.DestroyImmediate (listener.gameObject);
    ScriptableObject.DestroyImmediate (register, false);
    }


[Test]
public void ListenerGetsCalledWhenRegisterSetThenEnabledAndNotBefore ()
    {
    var register = ScriptableObject.CreateInstance <BoolRegister> ();
    var listener = CreateWithMonoBehaviour <BoolRegisterListener> ();
    int incrementWhenCalled = 0;
    listener.AddDidChangeCallback ( (bool value) => { ++incrementWhenCalled; } );
    listener.Register = register;

    Assert.AreEqual (0, incrementWhenCalled, "no callbacks until key is set");
    this.CallOnEnable (listener);
    Assert.AreEqual (1, incrementWhenCalled, "listener gets callback when enabled");

    GameObject.DestroyImmediate (listener.gameObject);
    ScriptableObject.DestroyImmediate (register, false);
    }


[Test]
public void ListenerGetsCalledWithInitialValueWhenRegistered ()
    {
    var register = ScriptableObject.CreateInstance <BoolRegister> ();
    register.InitialValue = register.Value = true;

    var listener = CreateWithMonoBehaviour <BoolRegisterListener> ();
    this.CallOnEnable (listener);

    bool valueFromCallback = false;
    listener.AddDidChangeCallback ( (bool value) => { valueFromCallback = value; } );
    listener.Register = register;

    Assert.IsTrue (valueFromCallback, "listener gets callback when registered");

    GameObject.DestroyImmediate (listener.gameObject);
    ScriptableObject.DestroyImmediate (register, false);
    }


[Test]
public void ListenerGetsCalledWhenValueIsChanged ()
    {
    var register = ScriptableObject.CreateInstance <BoolRegister> ();
    register.InitialValue = register.Value = false;
    var listener = CreateWithMonoBehaviour <BoolRegisterListener> ();
    listener.Register = register;
    this.CallOnEnable (listener);

    bool setWhenCalled = false;
    bool valueFromCallback = false;
    listener.AddDidChangeCallback (
            (bool value) => { setWhenCalled = true; valueFromCallback = value; }
            );
    register.Value = true;

    Assert.IsTrue (setWhenCalled, "listener gets callback when value is changed");
    Assert.IsTrue (valueFromCallback, "listener gets called with new value when changed");

    GameObject.DestroyImmediate (listener.gameObject);
    ScriptableObject.DestroyImmediate (register, false);
    }


[Test]
public void ListenerGetsCalledWhenRegisterIsChanged ()
    {
    var firstRegister = ScriptableObject.CreateInstance <BoolRegister> ();
    var secondRegister = ScriptableObject.CreateInstance <BoolRegister> ();
    var listener = CreateWithMonoBehaviour <BoolRegisterListener> ();
    listener.Register = firstRegister;
    this.CallOnEnable (listener);

    int incrementWhenCalled = 0;
    listener.AddDidChangeCallback ( (bool value) => { ++incrementWhenCalled; } );
    listener.Register = secondRegister;

    Assert.AreEqual (1, incrementWhenCalled, "listener gets callback when register is changed");
    Assert.AreEqual (0, firstRegister.Listeners.Count, "first register lost its listener");
    Assert.AreEqual (1, secondRegister.Listeners.Count, "second register gained a listener");

    GameObject.DestroyImmediate (listener.gameObject);
    ScriptableObject.DestroyImmediate (firstRegister, false);
    ScriptableObject.DestroyImmediate (secondRegister, false);
    }


[Test]
public void ManyListenersGetCalledWhenValueIsChanged ()
    {
    var register = ScriptableObject.CreateInstance <BoolRegister> ();
    register.InitialValue = register.Value = false;

    var firstListener = CreateWithMonoBehaviour <BoolRegisterListener> ();
    var secondListener = CreateWithMonoBehaviour <BoolRegisterListener> ();

    firstListener.Register = register;
    secondListener.Register = register;

    this.CallOnEnable (firstListener);
    this.CallOnEnable (secondListener);

    int callbacksReceived = 0;
    int truesFromCallback = 0;
    firstListener.AddDidChangeCallback (
            (bool value) => { ++callbacksReceived; truesFromCallback += value ? 1 : 0; }
            );
    secondListener.AddDidChangeCallback (
            (bool value) => { ++callbacksReceived; truesFromCallback += value ? 1 : 0; }
            );
    Assert.AreEqual (0, callbacksReceived, "no calls yet");
    register.Value = true;

    Assert.AreEqual (2, callbacksReceived, "both listeners for the key get callbacks when changed");
    Assert.AreEqual (2, truesFromCallback, "both listeners for the key get the new value when changed");

    GameObject.DestroyImmediate (firstListener.gameObject);
    GameObject.DestroyImmediate (secondListener.gameObject);
    ScriptableObject.DestroyImmediate (register, false);
    }



}
