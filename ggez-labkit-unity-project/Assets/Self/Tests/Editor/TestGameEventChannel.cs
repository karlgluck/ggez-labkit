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

public class TestGameEventChannel
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
    var gameEventTable = ScriptableObject.CreateInstance <GameEventTable> ();
    var firstListener = CreateWithMonoBehaviour <GameEventTableListener> ();
    this.CallOnEnable (firstListener);
    firstListener.Key = "TestKey";

    Assert.AreEqual (gameEventTable.KeysWithListeners.Count, 0, "no listeners at start");
    firstListener.GameEventTable = gameEventTable;
    Assert.AreEqual (gameEventTable.KeysWithListeners.Count, 1, "added 1 listener");

    var secondListener = CreateWithMonoBehaviour <GameEventTableListener> ();
    this.CallOnEnable (secondListener);
    secondListener.GameEventTable = gameEventTable;
    Assert.AreEqual (gameEventTable.KeysWithListeners.Count, 1, "set gameEventTable for listener but no key, so it shouldn't be registered");

    secondListener.Key = "SecondTestKey";
    Assert.AreEqual (gameEventTable.KeysWithListeners.Count, 2, "set key for second listener");

    secondListener.Key = "TestKey";
    Assert.AreEqual (gameEventTable.KeysWithListeners.Count, 1, "changed second listener to have same key as the first");
    secondListener.Key = "SecondTestKey";
    Assert.AreEqual (gameEventTable.KeysWithListeners.Count, 2, "changed second listener back to a different key");

    this.CallOnDisable (secondListener); // must be explicit; no coroutines or frames in test mode
    GameObject.DestroyImmediate (secondListener.gameObject);
    Assert.AreEqual (gameEventTable.KeysWithListeners.Count, 1, "destroyed second listener");

    this.CallOnDisable (firstListener); // must be explicit; no coroutines or frames in test mode
    GameObject.DestroyImmediate (firstListener.gameObject);
    Assert.AreEqual (gameEventTable.KeysWithListeners.Count, 0, "destroyed first listener");

    ScriptableObject.DestroyImmediate (gameEventTable, false);
    }


[Test]
public void ListenerGetsCalledWhenEnabledThenTableSetAndNotBefore ()
    {
    var gameEventTable = ScriptableObject.CreateInstance <GameEventTable> ();
    var listener = CreateWithMonoBehaviour <GameEventTableListener> ();
    int incrementWhenCalled = 0;
    listener.AddDidTriggerCallback ( () => { ++incrementWhenCalled; } );
    listener.Key = "TestKey";
    this.CallOnEnable (listener);

    gameEventTable.Trigger ("TestKey");
    Assert.AreEqual (0, incrementWhenCalled, "no callbacks until gameEventTable is set");
    
    listener.GameEventTable = gameEventTable;
    gameEventTable.Trigger ("TestKey");
    Assert.AreEqual (1, incrementWhenCalled, "listener gets callback when gameEventTable is set");

    GameObject.DestroyImmediate (listener.gameObject);
    ScriptableObject.DestroyImmediate (gameEventTable, false);
    }


[Test]
public void ListenerGetsCalledWhenEnabledThenKeySetAndNotBefore ()
    {
    var gameEventTable = ScriptableObject.CreateInstance <GameEventTable> ();
    var listener = CreateWithMonoBehaviour <GameEventTableListener> ();
    int incrementWhenCalled = 0;
    listener.AddDidTriggerCallback ( () => { ++incrementWhenCalled; } );
    listener.GameEventTable = gameEventTable;
    gameEventTable.Trigger ("TestKey");
    this.CallOnEnable (listener);

    Assert.AreEqual (0, incrementWhenCalled, "no callbacks until key is set");
    listener.Key = "TestKey";
    gameEventTable.Trigger ("TestKey");
    Assert.AreEqual (1, incrementWhenCalled, "listener gets callback when key is set");

    GameObject.DestroyImmediate (listener.gameObject);
    ScriptableObject.DestroyImmediate (gameEventTable, false);
    }


[Test]
public void ListenerGetsCalledWhenRegisterAndKeySetThenEnabledAndNotBefore ()
    {
    var gameEventTable = ScriptableObject.CreateInstance <GameEventTable> ();
    var listener = CreateWithMonoBehaviour <GameEventTableListener> ();
    int incrementWhenCalled = 0;
    listener.AddDidTriggerCallback ( () => { ++incrementWhenCalled; } );
    listener.Key = "TestKey";
    listener.GameEventTable = gameEventTable;

    gameEventTable.Trigger ("TestKey");
    Assert.AreEqual (0, incrementWhenCalled, "no callbacks until key is set");
    this.CallOnEnable (listener);

    gameEventTable.Trigger ("TestKey");
    Assert.AreEqual (1, incrementWhenCalled, "listener gets callback when enabled");

    GameObject.DestroyImmediate (listener.gameObject);
    ScriptableObject.DestroyImmediate (gameEventTable, false);
    }


[Test]
public void ListenerGetsCalledWhenTriggered ()
    {
    var gameEventTable = ScriptableObject.CreateInstance <GameEventTable> ();
    var listener = CreateWithMonoBehaviour <GameEventTableListener> ();

    listener.Key = "TestKey";
    listener.GameEventTable = gameEventTable;
    this.CallOnEnable (listener);

    bool setWhenCalled = false;
    listener.AddDidTriggerCallback (
            () => { setWhenCalled = true; }
            );
    gameEventTable.Trigger ("TestKey");

    Assert.IsTrue (setWhenCalled, "listener gets callback when value is changed");

    GameObject.DestroyImmediate (listener.gameObject);
    ScriptableObject.DestroyImmediate (gameEventTable, false);
    }


[Test]
public void ManyListenersGetCalledWhenTriggered ()
    {
    var gameEventTable = ScriptableObject.CreateInstance <GameEventTable> ();

    var firstListener = CreateWithMonoBehaviour <GameEventTableListener> ();
    firstListener.Key = "TestKey";
    var secondListener = CreateWithMonoBehaviour <GameEventTableListener> ();
    secondListener.Key = "TestKey";
    var thirdListener = CreateWithMonoBehaviour <GameEventTableListener> ();
    thirdListener.Key = "SomeDifferentTestKey";

    firstListener.GameEventTable = gameEventTable;
    secondListener.GameEventTable = gameEventTable;
    thirdListener.GameEventTable = gameEventTable;

    this.CallOnEnable (firstListener);
    this.CallOnEnable (secondListener);
    this.CallOnEnable (thirdListener);

    int callbacksReceived = 0;
    firstListener.AddDidTriggerCallback (
            () => { ++callbacksReceived; }
            );
    secondListener.AddDidTriggerCallback (
            () => { ++callbacksReceived; }
            );
    bool didThirdListenerGetCalled = false;
    thirdListener.AddDidTriggerCallback (
            () => { didThirdListenerGetCalled = true; }
            );    
    Assert.AreEqual (0, callbacksReceived, "no calls yet");
    gameEventTable.Trigger ("TestKey");

    Assert.AreEqual (2, callbacksReceived, "both listeners for the key get callbacks when changed");
    Assert.IsFalse (didThirdListenerGetCalled, "listener on a different key should not get a callback");

    GameObject.DestroyImmediate (firstListener.gameObject);
    GameObject.DestroyImmediate (secondListener.gameObject);
    GameObject.DestroyImmediate (thirdListener.gameObject);
    ScriptableObject.DestroyImmediate (gameEventTable, false);
    }




}
