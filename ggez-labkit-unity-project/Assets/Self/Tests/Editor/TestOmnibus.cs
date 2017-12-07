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
using GGEZ.Omnibus;

public class TestOmnibus
{

public T CreateWithMonoBehaviour<T> () where T : Component
    {
    var go = new GameObject (typeof(T).FullName);
    var retval = go.AddComponent <T> ();
	Debug.Assert (retval != null);
    return retval;
    }

public void CallOnEnable (MonoBehaviour mb)
    {
	typeof(Fub).InvokeMember (
		"OnEnable",
		BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy,
		null,
		mb,
		null
		);
    }

public void CallOnDisable (MonoBehaviour mb)
    {
	typeof(Fub).InvokeMember (
		"OnDisable",
		BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy,
		null,
		mb,
		null
		);
    }

[Test]
public void FubBusAndUnregister ()
    {
    var gameObject = new GameObject ();
    var bus = gameObject.AddComponent <Bus> ();
    var firstFub = CreateWithMonoBehaviour <BooleanTerminal> ();
    this.CallOnEnable (firstFub);
    firstFub.Pin = "TestKey";

    Assert.AreEqual (bus.GetConnectedKeys ().Count, 0, "no fubs at start");
    firstFub.Bus = bus;
    Assert.AreEqual (bus.GetConnectedKeys ().Count, 1, "added 1 fub");

    var secondFub = CreateWithMonoBehaviour <BooleanTerminal> ();
    this.CallOnEnable (secondFub);
    secondFub.Bus = bus;
    Assert.AreEqual (bus.GetConnectedKeys ().Count, 1, "set bus for fub but no key, so it shouldn't be registered");

    secondFub.Pin = "SecondTestKey";
    Assert.AreEqual (bus.GetConnectedKeys ().Count, 2, "set key for second fub");

    secondFub.Pin = "TestKey";
    Assert.AreEqual (bus.GetConnectedKeys ().Count, 1, "changed second fub to have same key as the first");
    secondFub.Pin = "SecondTestKey";
    Assert.AreEqual (bus.GetConnectedKeys ().Count, 2, "changed second fub back to a different key");

    this.CallOnDisable (secondFub); // must be explicit; no coroutines or frames in test mode
    GameObject.DestroyImmediate (secondFub.gameObject);
    Assert.AreEqual (bus.GetConnectedKeys ().Count, 1, "destroyed second fub");

    this.CallOnDisable (firstFub); // must be explicit; no coroutines or frames in test mode
    GameObject.DestroyImmediate (firstFub.gameObject);
    Assert.AreEqual (bus.GetConnectedKeys ().Count, 0, "destroyed first fub");

    ScriptableObject.DestroyImmediate (bus, false);
    }


[Test]
public void FubGetsCalledWhenEnabledThenBusSetAndNotBeforeForMemory ()
    {
    var gameObject = new GameObject ();
    var bus = gameObject.AddComponent <Bus> ();
	bus.SetBoolean ("TestKey", true);
    var fub = CreateWithMonoBehaviour <BooleanTerminal> ();
    int incrementWhenCalled = 0;
    fub.AddCallback ( (bool value) => { ++incrementWhenCalled; } );
    fub.Pin = "TestKey";
    this.CallOnEnable (fub);

    Assert.AreEqual (0, incrementWhenCalled, "no callbacks until bus is set");
    fub.Bus = bus;
    Assert.AreEqual (1, incrementWhenCalled, "fub gets change callback when bus is set");

    GameObject.DestroyImmediate (fub.gameObject);
    ScriptableObject.DestroyImmediate (bus, false);
    }


[Test]
public void FubGetsCalledWhenEnabledThenBusSetAndNotBeforeForEvent ()
    {
    var gameObject = new GameObject ();
    var bus = gameObject.AddComponent <Bus> ();

    var fub = CreateWithMonoBehaviour <BooleanTerminal> ();
    int incrementWhenCalled = 0;
    fub.AddCallback ( (bool value) => { ++incrementWhenCalled; } );
    fub.Pin = "TestKey";
    this.CallOnEnable (fub);

    Assert.AreEqual (0, incrementWhenCalled, "no callbacks until bus is set");
    fub.Bus = bus;
	bus.SignalBoolean ("TestKey", true);
    Assert.AreEqual (1, incrementWhenCalled, "fub gets trigger callback when bus is set");

    GameObject.DestroyImmediate (fub.gameObject);
    ScriptableObject.DestroyImmediate (bus, false);
    }


[Test]
public void FubGetsCalledWhenEnabledThenKeySetAndNotBeforeForMemory ()
    {
    var gameObject = new GameObject ();
    var bus = gameObject.AddComponent <Bus> ();
	bus.SetBoolean ("TestKey", true);

    var fub = CreateWithMonoBehaviour <BooleanTerminal> ();
    int incrementWhenCalled = 0;
    fub.AddCallback ( (bool value) => { ++incrementWhenCalled; } );
    fub.Bus = bus;
    this.CallOnEnable (fub);

    Assert.AreEqual (0, incrementWhenCalled, "no callbacks until key is set");

    fub.Pin = "TestKey";

    Assert.AreEqual (1, incrementWhenCalled, "fub gets change callback when key is set");

    GameObject.DestroyImmediate (fub.gameObject);
    ScriptableObject.DestroyImmediate (bus, false);
    }


[Test]
public void FubGetsCalledWhenEnabledThenKeySetAndNotBeforeForEvent ()
    {
    var gameObject = new GameObject ();
    var bus = gameObject.AddComponent <Bus> ();
    var fub = CreateWithMonoBehaviour <BooleanTerminal> ();
    int incrementWhenCalled = 0;
    fub.AddCallback ( (bool value) => { ++incrementWhenCalled; } );
    fub.Bus = bus;
	bus.SignalBoolean ("TestKey", true);
    this.CallOnEnable (fub);

    Assert.AreEqual (0, incrementWhenCalled, "no callbacks until key is set");

    fub.Pin = "TestKey";
	bus.SignalBoolean ("TestKey", true);
    Assert.AreEqual (1, incrementWhenCalled, "fub gets trigger callback when key is set");

    GameObject.DestroyImmediate (fub.gameObject);
    ScriptableObject.DestroyImmediate (bus, false);
    }


[Test]
public void FubGetsCalledWhenBusAndKeySetThenEnabledAndNotBefore ()
    {
    var gameObject = new GameObject ();
    var bus = gameObject.AddComponent <Bus> ();
	bus.SetBoolean ("TestKey", true);

    var fub = CreateWithMonoBehaviour <BooleanTerminal> ();
    int incrementWhenCalled = 0;
    fub.AddCallback ( (bool value) => { ++incrementWhenCalled; } );
    fub.Pin = "TestKey";
    fub.Bus = bus;

    Assert.AreEqual (0, incrementWhenCalled, "no callbacks until key is set");
    this.CallOnEnable (fub);

    Assert.AreEqual (1, incrementWhenCalled, "fub gets callback when enabled");

    GameObject.DestroyImmediate (fub.gameObject);
    ScriptableObject.DestroyImmediate (bus, false);
    }


[Test]
public void FubGetsCalledWithMemoryValueWhenConnected ()
    {
    var gameObject = new GameObject ();
    var bus = gameObject.AddComponent <Bus> ();
	bus.SetBoolean ("TestKey", true);

    var fub = CreateWithMonoBehaviour <BooleanTerminal> ();
    fub.Pin = "TestKey";
    this.CallOnEnable (fub);

    bool valueFromCallback = false;
    fub.AddCallback ( (bool value) => { valueFromCallback = value; } );
    fub.Bus = bus;

    Assert.IsTrue (valueFromCallback, "fub gets callback when registered");

    GameObject.DestroyImmediate (fub.gameObject);
    ScriptableObject.DestroyImmediate (bus, false);
    }


[Test]
public void FubGetsCalledWhenValueIsChanged ()
    {
    var gameObject = new GameObject ();
    var bus = gameObject.AddComponent <Bus> ();
	bus.SetBoolean ("TestKey", false);
    var fub = CreateWithMonoBehaviour <BooleanTerminal> ();

    fub.Pin = "TestKey";
    fub.Bus = bus;
    this.CallOnEnable (fub);

    bool setWhenCalled = false;
    bool valueFromCallback = false;
    fub.AddCallback (
            (bool value) => { setWhenCalled = true; valueFromCallback = value; }
            );
	bus.SetBoolean ("TestKey", true);

    Assert.IsTrue (setWhenCalled, "fub gets callback when value is changed");
    Assert.IsTrue (valueFromCallback, "fub gets called with new value when changed");

    GameObject.DestroyImmediate (fub.gameObject);
    ScriptableObject.DestroyImmediate (bus, false);
    }


[Test]
public void FubGetsCalledWhenKeyIsChanged ()
    {
    var gameObject = new GameObject ();
    var bus = gameObject.AddComponent <Bus> ();
	bus.SetBoolean ("TestKey", true);
	bus.SetBoolean ("AnotherTestKey", true);

    var fub = CreateWithMonoBehaviour <BooleanTerminal> ();
    fub.Pin = "TestKey";
    fub.Bus = bus;
    this.CallOnEnable (fub);

    int incrementWhenCalled = 0;
    fub.AddCallback ( (bool value) => { ++incrementWhenCalled; } );
    fub.Pin = "AnotherTestKey";

    Assert.AreEqual (1, incrementWhenCalled, "fub gets callback when key is changed");
    Assert.AreEqual (2, bus.GetConnectedKeys ().Count, "bus has 2 connected keys");

    GameObject.DestroyImmediate (fub.gameObject);
    ScriptableObject.DestroyImmediate (bus, false);
    }


[Test]
public void FubNotCalledWhenOtherKeyIsChanged ()
    {
    var gameObject = new GameObject ();
    var bus = gameObject.AddComponent <Bus> ();
	bus.SetBoolean ("TestKey", true);
	bus.SetBoolean ("AnotherTestKey", true);

    var fub = CreateWithMonoBehaviour <BooleanTerminal> ();
    fub.Pin = "TestKey";
    fub.Bus = bus;
    this.CallOnEnable (fub);

    int incrementWhenCalled = 0;
    fub.AddCallback ( (bool value) => { ++incrementWhenCalled; } );
    bus.SetBoolean ("AnotherTestKey", false);
    bus.SetBoolean ("ThirdTestKey", true);

    Assert.AreEqual (0, incrementWhenCalled, "fub gets callback when key is changed");
    Assert.AreEqual (1, bus.GetConnectedKeys ().Count, "bus has 1 connected key");
    Assert.AreEqual (3, bus.GetMemoryKeys ().Count, "bus has 3 memory values");

    GameObject.DestroyImmediate (fub.gameObject);
    ScriptableObject.DestroyImmediate (bus, false);
    }


[Test]
public void FubNotCalledWhenOtherKeyIsTriggered ()
    {
    var gameObject = new GameObject ();
    var bus = gameObject.AddComponent <Bus> ();
    var fub = CreateWithMonoBehaviour <BooleanTerminal> ();
    fub.Pin = "TestKey";
    fub.Bus = bus;
    this.CallOnEnable (fub);

    int incrementWhenCalled = 0;
    fub.AddCallback ( (bool value) => { ++incrementWhenCalled; } );
	bus.SignalBoolean ("AnotherTestKey", true);

    Assert.AreEqual (0, incrementWhenCalled, "fub gets callback when key is changed");
    Assert.AreEqual (1, bus.GetConnectedKeys ().Count, "bus has 1 connected key");

    GameObject.DestroyImmediate (fub.gameObject);
    ScriptableObject.DestroyImmediate (bus, false);
    }


[Test]
public void FubGetsCalledWhenTriggered ()
    {
    var gameObject = new GameObject ();
    var bus = gameObject.AddComponent <Bus> ();
    var fub = CreateWithMonoBehaviour <BooleanTerminal> ();
    fub.Pin = "TestKey";
    fub.Bus = bus;
    this.CallOnEnable (fub);

    int incrementWhenCalled = 0;
    fub.AddCallback ( (bool value) => { ++incrementWhenCalled; } );
	bus.SignalBoolean ("TestKey", true);

    Assert.AreEqual (1, incrementWhenCalled, "fub gets callback when key is changed");
    Assert.AreEqual (1, bus.GetConnectedKeys ().Count, "bus has 1 connected key");

    GameObject.DestroyImmediate (fub.gameObject);
    ScriptableObject.DestroyImmediate (bus, false);
    }


[Test]
public void FubGetsCalledWhenBusIsChanged ()
    {
    var gameObject = new GameObject ();
    var firstBus = gameObject.AddComponent <Bus> ();
	firstBus.SetBoolean ("TestKey", true);

    var secondGameObject = new GameObject ();
    var secondBus = secondGameObject.AddComponent <Bus> ();
	secondBus.SetBoolean ("TestKey", true);

    var fub = CreateWithMonoBehaviour <BooleanTerminal> ();
    fub.Pin = "TestKey";
    fub.Bus = firstBus;
    this.CallOnEnable (fub);

    int incrementWhenCalled = 0;
    fub.AddCallback ( (bool value) => { ++incrementWhenCalled; } );
    fub.Bus = secondBus;

    Assert.AreEqual (1, incrementWhenCalled, "fub gets callback when bus is changed");
    Assert.AreEqual (0, firstBus.GetConnectedKeys ().Count, "first bus lost its fub");
    Assert.AreEqual (1, secondBus.GetConnectedKeys ().Count, "second bus gained a fub");

    GameObject.DestroyImmediate (fub.gameObject);
    GameObject.DestroyImmediate (gameObject);
    GameObject.DestroyImmediate (secondGameObject);
    }




[Test]
public void ManyFubsGetCalledWhenValueIsChanged ()
    {
    var gameObject = new GameObject ();
    var bus = gameObject.AddComponent <Bus> ();
    bus.SetBoolean ("TestKey", false);
    bus.SetBoolean ("SomeDifferentTestKey", false);

    var firstFub = CreateWithMonoBehaviour <BooleanTerminal> ();
    firstFub.Pin = "TestKey";
    var secondFub = CreateWithMonoBehaviour <BooleanTerminal> ();
    secondFub.Pin = "TestKey";
    var thirdFub = CreateWithMonoBehaviour <BooleanTerminal> ();
    thirdFub.Pin = "SomeDifferentTestKey";

    firstFub.Bus = bus;
    secondFub.Bus = bus;
    thirdFub.Bus = bus;

    this.CallOnEnable (firstFub);
    this.CallOnEnable (secondFub);
    this.CallOnEnable (thirdFub);

    int callbacksReceived = 0;
    int truesFromCallback = 0;
    firstFub.AddCallback (
            (bool value) => { ++callbacksReceived; truesFromCallback += value ? 1 : 0; }
            );
    secondFub.AddCallback (
            (bool value) => { ++callbacksReceived; truesFromCallback += value ? 1 : 0; }
            );
    bool didThirdFubGetCalled = false;
    thirdFub.AddCallback (
            (bool value) => { didThirdFubGetCalled = true; }
            );
    Assert.AreEqual (0, callbacksReceived, "no calls yet");
    bus.SetBoolean ("TestKey", true);

    Assert.AreEqual (2, callbacksReceived, "both fubs for the key get callbacks when changed");
    Assert.AreEqual (2, truesFromCallback, "both fubs for the key get the new value when changed");
    Assert.IsFalse (didThirdFubGetCalled, "fub on a different key should not get a callback");

    GameObject.DestroyImmediate (firstFub.gameObject);
    GameObject.DestroyImmediate (secondFub.gameObject);
    GameObject.DestroyImmediate (thirdFub.gameObject);
    ScriptableObject.DestroyImmediate (bus, false);
    }




[Test]
public void ManyFubsGetCalledWhenEventTriggered ()
    {
    var gameObject = new GameObject ();
    var bus = gameObject.AddComponent <Bus> ();

    var firstFub = CreateWithMonoBehaviour <BooleanTerminal> ();
    firstFub.Pin = "TestKey";
    var secondFub = CreateWithMonoBehaviour <BooleanTerminal> ();
    secondFub.Pin = "TestKey";
    var thirdFub = CreateWithMonoBehaviour <BooleanTerminal> ();
    thirdFub.Pin = "SomeDifferentTestKey";

    firstFub.Bus = bus;
    secondFub.Bus = bus;
    thirdFub.Bus = bus;

    this.CallOnEnable (firstFub);
    this.CallOnEnable (secondFub);
    this.CallOnEnable (thirdFub);

    int callbacksReceived = 0;
    int truesFromCallback = 0;
    firstFub.AddCallback (
            (bool value) => { ++callbacksReceived; truesFromCallback += value ? 1 : 0; }
            );
    secondFub.AddCallback (
            (bool value) => { ++callbacksReceived; truesFromCallback += value ? 1 : 0; }
            );
    bool didThirdFubGetCalled = false;
    thirdFub.AddCallback (
            (bool value) => { didThirdFubGetCalled = true; }
            );
    Assert.AreEqual (0, callbacksReceived, "no calls yet");
    bus.SignalBoolean ("TestKey", true);

    Assert.AreEqual (2, callbacksReceived, "both fubs for the key get callbacks when triggered");
    Assert.AreEqual (2, truesFromCallback, "both fubs for the key get the new value when triggered");
    Assert.IsFalse (didThirdFubGetCalled, "fub on a different key should not get a callback");

    GameObject.DestroyImmediate (firstFub.gameObject);
    GameObject.DestroyImmediate (secondFub.gameObject);
    GameObject.DestroyImmediate (thirdFub.gameObject);
    ScriptableObject.DestroyImmediate (bus, false);
    }


[Test]
public void FubDoesNotCalledWhenValueIsRemoved ()
    {
    var gameObject = new GameObject ();
    var bus = gameObject.AddComponent <Bus> ();
    bus.SetBoolean ("TestKey", false);

    var fub = CreateWithMonoBehaviour <BooleanTerminal> ();
    fub.Pin = "TestKey";
    fub.Bus = bus;
    this.CallOnEnable (fub);

    bool setWhenCalled = false;
    fub.AddCallback (
            (bool value) => { setWhenCalled = true; }
            );
    bus.Unset ("TestKey");

    Assert.IsFalse (setWhenCalled, "fub does not get callback when value is removed");

    GameObject.DestroyImmediate (fub.gameObject);
    ScriptableObject.DestroyImmediate (bus, false);
    }


[Test]
public void FubGetsCalledWithValueSetOnBusWithoutFubsWhenBused ()
    {
var gameObject = new GameObject ();
    var bus = gameObject.AddComponent <Bus> ();
    bus.SetBoolean ("TestKey", true);

    var fub = CreateWithMonoBehaviour <BooleanTerminal> ();
    fub.Pin = "TestKey";
    fub.Bus = bus;

    bool valueFromCallback = false;
    fub.AddCallback ( (bool value) => { valueFromCallback = value; } );
    this.CallOnEnable (fub);

    Assert.IsTrue (valueFromCallback, "fub gets callback with default value when registered; the value is not the one that was set");

    GameObject.DestroyImmediate (fub.gameObject);
    ScriptableObject.DestroyImmediate (bus, false);
    }


}
