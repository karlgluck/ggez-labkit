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

public class TestBoolTableRegister
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
    var register = ScriptableObject.CreateInstance <BoolTableRegister> ();
    var firstListener = CreateWithMonoBehaviour <BoolTableRegisterListener> ();
    this.CallOnEnable (firstListener);
    firstListener.Key = "TestKey";

    Assert.AreEqual (register.KeysWithListeners.Count, 0, "no listeners at start");
    firstListener.TableRegister = register;
    Assert.AreEqual (register.KeysWithListeners.Count, 1, "added 1 listener");

    var secondListener = CreateWithMonoBehaviour <BoolTableRegisterListener> ();
    this.CallOnEnable (secondListener);
    secondListener.TableRegister = register;
    Assert.AreEqual (register.KeysWithListeners.Count, 1, "set register for listener but no key, so it shouldn't be registered");

    secondListener.Key = "SecondTestKey";
    Assert.AreEqual (register.KeysWithListeners.Count, 2, "set key for second listener");

    secondListener.Key = "TestKey";
    Assert.AreEqual (register.KeysWithListeners.Count, 1, "changed second listener to have same key as the first");
    secondListener.Key = "SecondTestKey";
    Assert.AreEqual (register.KeysWithListeners.Count, 2, "changed second listener back to a different key");

    this.CallOnDisable (secondListener); // must be explicit; no coroutines or frames in test mode
    GameObject.DestroyImmediate (secondListener.gameObject);
    Assert.AreEqual (register.KeysWithListeners.Count, 1, "destroyed second listener");

    this.CallOnDisable (firstListener); // must be explicit; no coroutines or frames in test mode
    GameObject.DestroyImmediate (firstListener.gameObject);
    Assert.AreEqual (register.KeysWithListeners.Count, 0, "destroyed first listener");

    ScriptableObject.DestroyImmediate (register, false);
    }


[Test]
public void ListenerGetsCalledWhenEnabledThenRegisterSetAndNotBefore ()
    {
    var register = ScriptableObject.CreateInstance <BoolTableRegister> ();
    var listener = CreateWithMonoBehaviour <BoolTableRegisterListener> ();
    int incrementWhenCalled = 0;
    listener.AddDidChangeCallback ( (bool value) => { ++incrementWhenCalled; } );
    listener.Key = "TestKey";
    this.CallOnEnable (listener);

    Assert.AreEqual (0, incrementWhenCalled, "no callbacks until register is set");
    listener.TableRegister = register;
    Assert.AreEqual (1, incrementWhenCalled, "listener gets callback when register is set");

    GameObject.DestroyImmediate (listener.gameObject);
    ScriptableObject.DestroyImmediate (register, false);
    }


[Test]
public void ListenerGetsCalledWhenEnabledThenKeySetAndNotBefore ()
    {
    var register = ScriptableObject.CreateInstance <BoolTableRegister> ();
    var listener = CreateWithMonoBehaviour <BoolTableRegisterListener> ();
    int incrementWhenCalled = 0;
    listener.AddDidChangeCallback ( (bool value) => { ++incrementWhenCalled; } );
    listener.TableRegister = register;
    this.CallOnEnable (listener);

    Assert.AreEqual (0, incrementWhenCalled, "no callbacks until key is set");

    listener.Key = "TestKey";

    Assert.AreEqual (1, incrementWhenCalled, "listener gets callback when key is set");

    GameObject.DestroyImmediate (listener.gameObject);
    ScriptableObject.DestroyImmediate (register, false);
    }


[Test]
public void ListenerGetsCalledWhenRegisterAndKeySetThenEnabledAndNotBefore ()
    {
    var register = ScriptableObject.CreateInstance <BoolTableRegister> ();
    var listener = CreateWithMonoBehaviour <BoolTableRegisterListener> ();
    int incrementWhenCalled = 0;
    listener.AddDidChangeCallback ( (bool value) => { ++incrementWhenCalled; } );
    listener.Key = "TestKey";
    listener.TableRegister = register;

    Assert.AreEqual (0, incrementWhenCalled, "no callbacks until key is set");
    this.CallOnEnable (listener);

    Assert.AreEqual (1, incrementWhenCalled, "listener gets callback when enabled");

    GameObject.DestroyImmediate (listener.gameObject);
    ScriptableObject.DestroyImmediate (register, false);
    }


[Test]
public void ListenerGetsCalledWithDefaultValueWhenRegistered ()
    {
    var register = ScriptableObject.CreateInstance <BoolTableRegister> ();
    register.DefaultValue = true;

    var listener = CreateWithMonoBehaviour <BoolTableRegisterListener> ();
    listener.Key = "TestKey";
    this.CallOnEnable (listener);

    bool valueFromCallback = false;
    listener.AddDidChangeCallback ( (bool value) => { valueFromCallback = value; } );
    listener.TableRegister = register;

    Assert.IsTrue (valueFromCallback, "listener gets callback when registered");

    GameObject.DestroyImmediate (listener.gameObject);
    ScriptableObject.DestroyImmediate (register, false);
    }


[Test]
public void ListenerGetsCalledWhenValueIsChanged ()
    {
    var register = ScriptableObject.CreateInstance <BoolTableRegister> ();
    register.DefaultValue = false;
    var listener = CreateWithMonoBehaviour <BoolTableRegisterListener> ();

    listener.Key = "TestKey";
    listener.TableRegister = register;
    this.CallOnEnable (listener);

    bool setWhenCalled = false;
    bool valueFromCallback = false;
    listener.AddDidChangeCallback (
            (bool value) => { setWhenCalled = true; valueFromCallback = value; }
            );
    register["TestKey"] = true;

    Assert.IsTrue (setWhenCalled, "listener gets callback when value is changed");
    Assert.IsTrue (valueFromCallback, "listener gets called with new value when changed");

    GameObject.DestroyImmediate (listener.gameObject);
    ScriptableObject.DestroyImmediate (register, false);
    }


[Test]
public void ListenerGetsCalledWhenKeyIsChanged ()
    {
    var register = ScriptableObject.CreateInstance <BoolTableRegister> ();
    var listener = CreateWithMonoBehaviour <BoolTableRegisterListener> ();
    listener.Key = "TestKey";
    listener.TableRegister = register;
    this.CallOnEnable (listener);

    int incrementWhenCalled = 0;
    listener.AddDidChangeCallback ( (bool value) => { ++incrementWhenCalled; } );
    listener.Key = "AnotherTestKey";

    Assert.AreEqual (1, incrementWhenCalled, "listener gets callback when key is changed");
    Assert.AreEqual (1, register.KeysWithListeners.Count, "register has only 1 listener");

    GameObject.DestroyImmediate (listener.gameObject);
    ScriptableObject.DestroyImmediate (register, false);
    }


[Test]
public void ListenerGetsCalledWhenRegisterIsChanged ()
    {
    var firstRegister = ScriptableObject.CreateInstance <BoolTableRegister> ();
    var secondRegister = ScriptableObject.CreateInstance <BoolTableRegister> ();
    var listener = CreateWithMonoBehaviour <BoolTableRegisterListener> ();
    listener.Key = "TestKey";
    listener.TableRegister = firstRegister;
    this.CallOnEnable (listener);

    int incrementWhenCalled = 0;
    listener.AddDidChangeCallback ( (bool value) => { ++incrementWhenCalled; } );
    listener.TableRegister = secondRegister;

    Assert.AreEqual (1, incrementWhenCalled, "listener gets callback when register is changed");
    Assert.AreEqual (0, firstRegister.KeysWithListeners.Count, "first register lost its listener");
    Assert.AreEqual (1, secondRegister.KeysWithListeners.Count, "second register gained a listener");

    GameObject.DestroyImmediate (listener.gameObject);
    ScriptableObject.DestroyImmediate (firstRegister, false);
    ScriptableObject.DestroyImmediate (secondRegister, false);
    }


[Test]
public void ManyListenersGetCalledWhenValueIsChanged ()
    {
    var register = ScriptableObject.CreateInstance <BoolTableRegister> ();
    register.DefaultValue = false;

    var firstListener = CreateWithMonoBehaviour <BoolTableRegisterListener> ();
    firstListener.Key = "TestKey";
    var secondListener = CreateWithMonoBehaviour <BoolTableRegisterListener> ();
    secondListener.Key = "TestKey";
    var thirdListener = CreateWithMonoBehaviour <BoolTableRegisterListener> ();
    thirdListener.Key = "SomeDifferentTestKey";

    firstListener.TableRegister = register;
    secondListener.TableRegister = register;
    thirdListener.TableRegister = register;

    this.CallOnEnable (firstListener);
    this.CallOnEnable (secondListener);
    this.CallOnEnable (thirdListener);

    int callbacksReceived = 0;
    int truesFromCallback = 0;
    firstListener.AddDidChangeCallback (
            (bool value) => { ++callbacksReceived; truesFromCallback += value ? 1 : 0; }
            );
    secondListener.AddDidChangeCallback (
            (bool value) => { ++callbacksReceived; truesFromCallback += value ? 1 : 0; }
            );
    bool didThirdListenerGetCalled = false;
    thirdListener.AddDidChangeCallback (
            (bool value) => { didThirdListenerGetCalled = true; }
            );
    Assert.AreEqual (0, callbacksReceived, "no calls yet");
    register["TestKey"] = true;

    Assert.AreEqual (2, callbacksReceived, "both listeners for the key get callbacks when changed");
    Assert.AreEqual (2, truesFromCallback, "both listeners for the key get the new value when changed");
    Assert.IsFalse (didThirdListenerGetCalled, "listener on a different key should not get a callback");

    GameObject.DestroyImmediate (firstListener.gameObject);
    GameObject.DestroyImmediate (secondListener.gameObject);
    GameObject.DestroyImmediate (thirdListener.gameObject);
    ScriptableObject.DestroyImmediate (register, false);
    }


[Test]
public void ListenerGetsCalledWhenValueIsRemoved ()
    {
    var register = ScriptableObject.CreateInstance <BoolTableRegister> ();
    register.DefaultValue = true;
    register["TestKey"] = false;

    var listener = CreateWithMonoBehaviour <BoolTableRegisterListener> ();
    listener.Key = "TestKey";
    listener.TableRegister = register;
    this.CallOnEnable (listener);

    bool setWhenCalled = false;
    bool valueFromCallback = false;
    listener.AddDidChangeCallback (
            (bool value) => { setWhenCalled = true; valueFromCallback = value; }
            );
    register.Remove ("TestKey");

    Assert.IsTrue (setWhenCalled, "listener gets callback when value is removed");
    Assert.IsTrue (valueFromCallback, "listener gets called with default value when removed");

    GameObject.DestroyImmediate (listener.gameObject);
    ScriptableObject.DestroyImmediate (register, false);
    }


[Test]
public void ListenerGetsCalledWithValueSetOnRegisterWithoutListenersWhenRegistered ()
    {

    var register = ScriptableObject.CreateInstance <BoolTableRegister> ();
    register.DefaultValue = false;
    register["TestKey"] = true;

    var listener = CreateWithMonoBehaviour <BoolTableRegisterListener> ();
    listener.Key = "TestKey";
    listener.TableRegister = register;

    bool valueFromCallback = false;
    listener.AddDidChangeCallback ( (bool value) => { valueFromCallback = value; } );
    this.CallOnEnable (listener);

    Assert.IsTrue (valueFromCallback, "listener gets callback with default value when registered; the value is not the one that was set");

    GameObject.DestroyImmediate (listener.gameObject);
    ScriptableObject.DestroyImmediate (register, false);
    }


}
