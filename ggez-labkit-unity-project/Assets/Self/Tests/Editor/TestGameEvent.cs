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

public class TestGameEvent
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
    var gameEvent = ScriptableObject.CreateInstance <GameEvent> ();
    var firstListener = CreateWithMonoBehaviour <GameEventListener> ();
    this.CallOnEnable (firstListener);

    Assert.AreEqual (gameEvent.Listeners.Count, 0, "no listeners at start");
    firstListener.GameEvent = gameEvent;
    Assert.AreEqual (gameEvent.Listeners.Count, 1, "added 1 listener");

    var secondListener = CreateWithMonoBehaviour <GameEventListener> ();
    secondListener.GameEvent = gameEvent;
    Assert.AreEqual (gameEvent.Listeners.Count, 1, "added second listener but not enabled");
    this.CallOnEnable (secondListener);
    Assert.AreEqual (gameEvent.Listeners.Count, 2, "enabled second listener");

    this.CallOnDisable (secondListener); // must be explicit; no coroutines or frames in test mode
    GameObject.DestroyImmediate (secondListener.gameObject);
    Assert.AreEqual (gameEvent.Listeners.Count, 1, "destroyed second listener");

    this.CallOnDisable (firstListener); // must be explicit; no coroutines or frames in test mode
    GameObject.DestroyImmediate (firstListener.gameObject);
    Assert.AreEqual (gameEvent.Listeners.Count, 0, "destroyed first listener");

    ScriptableObject.DestroyImmediate (gameEvent, false);
    }



[Test]
public void ListenerGetsCalledWhenGameEventIsTriggered ()
    {
    var gameEvent = ScriptableObject.CreateInstance <GameEvent> ();
    var listener = CreateWithMonoBehaviour <GameEventListener> ();
    listener.GameEvent = gameEvent;
    this.CallOnEnable (listener);

    bool setWhenCalled = false;
    listener.AddDidTriggerCallback (
            () => { setWhenCalled = true; }
            );
    gameEvent.Trigger ();

    Assert.IsTrue (setWhenCalled, "listener gets callback when value is changed");

    GameObject.DestroyImmediate (listener.gameObject);
    ScriptableObject.DestroyImmediate (gameEvent, false);
    }


[Test]
public void ManyListenersGetCalledWhenGameEventIsTriggered ()
    {
    var gameEvent = ScriptableObject.CreateInstance <GameEvent> ();

    var firstListener = CreateWithMonoBehaviour <GameEventListener> ();
    var secondListener = CreateWithMonoBehaviour <GameEventListener> ();

    firstListener.GameEvent = gameEvent;
    secondListener.GameEvent = gameEvent;

    this.CallOnEnable (firstListener);
    this.CallOnEnable (secondListener);

    int callbacksReceived = 0;
    firstListener.AddDidTriggerCallback (
            () => { ++callbacksReceived; }
            );
    secondListener.AddDidTriggerCallback (
            () => { ++callbacksReceived; }
            );
    Assert.AreEqual (0, callbacksReceived, "no calls yet");
    gameEvent.Trigger ();

    Assert.AreEqual (2, callbacksReceived, "both listeners for the key get callbacks when changed");

    GameObject.DestroyImmediate (firstListener.gameObject);
    GameObject.DestroyImmediate (secondListener.gameObject);
    ScriptableObject.DestroyImmediate (gameEvent, false);
    }



}
