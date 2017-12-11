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
	typeof(ImplementTerminalForType<bool, UnityEventForBooleanTerminal>).InvokeMember (
		"OnEnable",
		BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy,
		null,
		mb,
		null
		);
    }

public void CallOnDisable (MonoBehaviour mb)
    {
	typeof(ImplementTerminalForType<bool, UnityEventForBooleanTerminal>).InvokeMember (
		"OnDisable",
		BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy,
		null,
		mb,
		null
		);
    }

[Test]
public void CellBusAndUnregister ()
    {
    var gameObject = new GameObject ();
    var bus = gameObject.AddComponent <Bus> ();
    var firstCell = CreateWithMonoBehaviour <BooleanTerminal> ();
    this.CallOnEnable (firstCell);
    firstCell.Pin = "TestKey";

    Assert.AreEqual (bus.GetConnectedKeys ().Count, 0, "no fubs at start");
    firstCell.Bus = bus;
    Assert.AreEqual (bus.GetConnectedKeys ().Count, 1, "added 1 fub");

    var secondCell = CreateWithMonoBehaviour <BooleanTerminal> ();
    this.CallOnEnable (secondCell);
    secondCell.Bus = bus;
    Assert.AreEqual (bus.GetConnectedKeys ().Count, 1, "set bus for fub but no key, so it shouldn't be registered");

    secondCell.Pin = "SecondTestKey";
    Assert.AreEqual (bus.GetConnectedKeys ().Count, 2, "set key for second fub");

    secondCell.Pin = "TestKey";
    Assert.AreEqual (bus.GetConnectedKeys ().Count, 1, "changed second fub to have same key as the first");
    secondCell.Pin = "SecondTestKey";
    Assert.AreEqual (bus.GetConnectedKeys ().Count, 2, "changed second fub back to a different key");

    this.CallOnDisable (secondCell); // must be explicit; no coroutines or frames in test mode
    GameObject.DestroyImmediate (secondCell.gameObject);
    Assert.AreEqual (bus.GetConnectedKeys ().Count, 1, "destroyed second fub");

    this.CallOnDisable (firstCell); // must be explicit; no coroutines or frames in test mode
    GameObject.DestroyImmediate (firstCell.gameObject);
    Assert.AreEqual (bus.GetConnectedKeys ().Count, 0, "destroyed first fub");

    ScriptableObject.DestroyImmediate (bus, false);
    }


[Test]
public void CellGetsCalledWhenEnabledThenBusSetAndNotBeforeForMemory ()
    {
    var gameObject = new GameObject ();
    var bus = gameObject.AddComponent <Bus> ();
	bus.Set ("TestKey", true);
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
public void CellGetsCalledWhenEnabledThenBusSetAndNotBeforeForEvent ()
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
	bus.Signal ("TestKey", true);
    Assert.AreEqual (1, incrementWhenCalled, "fub gets trigger callback when bus is set");

    GameObject.DestroyImmediate (fub.gameObject);
    ScriptableObject.DestroyImmediate (bus, false);
    }


[Test]
public void CellGetsCalledWhenEnabledThenKeySetAndNotBeforeForMemory ()
    {
    var gameObject = new GameObject ();
    var bus = gameObject.AddComponent <Bus> ();
	bus.Set ("TestKey", true);

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
public void CellGetsCalledWhenEnabledThenKeySetAndNotBeforeForEvent ()
    {
    var gameObject = new GameObject ();
    var bus = gameObject.AddComponent <Bus> ();
    var fub = CreateWithMonoBehaviour <BooleanTerminal> ();
    int incrementWhenCalled = 0;
    fub.AddCallback ( (bool value) => { ++incrementWhenCalled; } );
    fub.Bus = bus;
	bus.Signal ("TestKey", true);
    this.CallOnEnable (fub);

    Assert.AreEqual (0, incrementWhenCalled, "no callbacks until key is set");

    fub.Pin = "TestKey";
	bus.Signal ("TestKey", true);
    Assert.AreEqual (1, incrementWhenCalled, "fub gets trigger callback when key is set");

    GameObject.DestroyImmediate (fub.gameObject);
    ScriptableObject.DestroyImmediate (bus, false);
    }


[Test]
public void CellGetsCalledWhenBusAndKeySetThenEnabledAndNotBefore ()
    {
    var gameObject = new GameObject ();
    var bus = gameObject.AddComponent <Bus> ();
	bus.Set ("TestKey", true);

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
public void CellGetsCalledWithMemoryValueWhenConnected ()
    {
    var gameObject = new GameObject ();
    var bus = gameObject.AddComponent <Bus> ();
	bus.Set ("TestKey", true);

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
public void CellGetsCalledWhenValueIsChanged ()
    {
    var gameObject = new GameObject ();
    var bus = gameObject.AddComponent <Bus> ();
	bus.Set ("TestKey", false);
    var fub = CreateWithMonoBehaviour <BooleanTerminal> ();

    fub.Pin = "TestKey";
    fub.Bus = bus;
    this.CallOnEnable (fub);

    bool setWhenCalled = false;
    bool valueFromCallback = false;
    fub.AddCallback (
            (bool value) => { setWhenCalled = true; valueFromCallback = value; }
            );
	bus.Set ("TestKey", true);

    Assert.IsTrue (setWhenCalled, "fub gets callback when value is changed");
    Assert.IsTrue (valueFromCallback, "fub gets called with new value when changed");

    GameObject.DestroyImmediate (fub.gameObject);
    ScriptableObject.DestroyImmediate (bus, false);
    }


[Test]
public void CellGetsCalledWhenKeyIsChanged ()
    {
    var gameObject = new GameObject ();
    var bus = gameObject.AddComponent <Bus> ();
	bus.Set ("TestKey", true);
	bus.Set ("AnotherTestKey", true);

    var fub = CreateWithMonoBehaviour <BooleanTerminal> ();
    fub.Pin = "TestKey";
    fub.Bus = bus;
    this.CallOnEnable (fub);

    int incrementWhenCalled = 0;
    fub.AddCallback ( (bool value) => { ++incrementWhenCalled; } );
    fub.Pin = "AnotherTestKey";

    Assert.AreEqual (1, incrementWhenCalled, "fub gets callback when key is changed");
    Assert.AreEqual (1, bus.GetConnectedKeys ().Count, "bus has 1 connected key");

    GameObject.DestroyImmediate (fub.gameObject);
    ScriptableObject.DestroyImmediate (bus, false);
    }


[Test]
public void CellNotCalledWhenOtherKeyIsChanged ()
    {
    var gameObject = new GameObject ();
    var bus = gameObject.AddComponent <Bus> ();
	bus.Set ("TestKey", true);
	bus.Set ("AnotherTestKey", true);

    var fub = CreateWithMonoBehaviour <BooleanTerminal> ();
    fub.Pin = "TestKey";
    fub.Bus = bus;
    this.CallOnEnable (fub);

    int incrementWhenCalled = 0;
    fub.AddCallback ( (bool value) => { ++incrementWhenCalled; } );
    bus.Set ("AnotherTestKey", false);
    bus.Set ("ThirdTestKey", true);

    Assert.AreEqual (0, incrementWhenCalled, "fub gets callback when key is changed");
    Assert.AreEqual (1, bus.GetConnectedKeys ().Count, "bus has 1 connected key");
    Assert.AreEqual (3, bus.GetMemoryKeys ().Count, "bus has 3 memory values");

    GameObject.DestroyImmediate (fub.gameObject);
    ScriptableObject.DestroyImmediate (bus, false);
    }


[Test]
public void CellNotCalledWhenOtherKeyIsTriggered ()
    {
    var gameObject = new GameObject ();
    var bus = gameObject.AddComponent <Bus> ();
    var fub = CreateWithMonoBehaviour <BooleanTerminal> ();
    fub.Pin = "TestKey";
    fub.Bus = bus;
    this.CallOnEnable (fub);

    int incrementWhenCalled = 0;
    fub.AddCallback ( (bool value) => { ++incrementWhenCalled; } );
	bus.Signal ("AnotherTestKey", true);

    Assert.AreEqual (0, incrementWhenCalled, "fub gets callback when key is changed");
    Assert.AreEqual (1, bus.GetConnectedKeys ().Count, "bus has 1 connected key");

    GameObject.DestroyImmediate (fub.gameObject);
    ScriptableObject.DestroyImmediate (bus, false);
    }


[Test]
public void CellGetsCalledWhenTriggered ()
    {
    var gameObject = new GameObject ();
    var bus = gameObject.AddComponent <Bus> ();
    var fub = CreateWithMonoBehaviour <BooleanTerminal> ();
    fub.Pin = "TestKey";
    fub.Bus = bus;
    this.CallOnEnable (fub);

    int incrementWhenCalled = 0;
    fub.AddCallback ( (bool value) => { ++incrementWhenCalled; } );
	bus.Signal ("TestKey", true);

    Assert.AreEqual (1, incrementWhenCalled, "fub gets callback when key is changed");
    Assert.AreEqual (1, bus.GetConnectedKeys ().Count, "bus has 1 connected key");

    GameObject.DestroyImmediate (fub.gameObject);
    ScriptableObject.DestroyImmediate (bus, false);
    }


[Test]
public void CellGetsCalledWhenBusIsChanged ()
    {
    var gameObject = new GameObject ();
    var firstBus = gameObject.AddComponent <Bus> ();
	firstBus.Set ("TestKey", true);

    var secondGameObject = new GameObject ();
    var secondBus = secondGameObject.AddComponent <Bus> ();
	secondBus.Set ("TestKey", true);

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
public void ManyCellsGetCalledWhenValueIsChanged ()
    {
    var gameObject = new GameObject ();
    var bus = gameObject.AddComponent <Bus> ();
    bus.Set ("TestKey", false);
    bus.Set ("SomeDifferentTestKey", false);

    var firstCell = CreateWithMonoBehaviour <BooleanTerminal> ();
    firstCell.Pin = "TestKey";
    var secondCell = CreateWithMonoBehaviour <BooleanTerminal> ();
    secondCell.Pin = "TestKey";
    var thirdCell = CreateWithMonoBehaviour <BooleanTerminal> ();
    thirdCell.Pin = "SomeDifferentTestKey";

    firstCell.Bus = bus;
    secondCell.Bus = bus;
    thirdCell.Bus = bus;

    this.CallOnEnable (firstCell);
    this.CallOnEnable (secondCell);
    this.CallOnEnable (thirdCell);

    int callbacksReceived = 0;
    int truesFromCallback = 0;
    firstCell.AddCallback (
            (bool value) => { ++callbacksReceived; truesFromCallback += value ? 1 : 0; }
            );
    secondCell.AddCallback (
            (bool value) => { ++callbacksReceived; truesFromCallback += value ? 1 : 0; }
            );
    bool didThirdCellGetCalled = false;
    thirdCell.AddCallback (
            (bool value) => { didThirdCellGetCalled = true; }
            );
    Assert.AreEqual (0, callbacksReceived, "no calls yet");
    bus.Set ("TestKey", true);

    Assert.AreEqual (2, callbacksReceived, "both fubs for the key get callbacks when changed");
    Assert.AreEqual (2, truesFromCallback, "both fubs for the key get the new value when changed");
    Assert.IsFalse (didThirdCellGetCalled, "fub on a different key should not get a callback");

    GameObject.DestroyImmediate (firstCell.gameObject);
    GameObject.DestroyImmediate (secondCell.gameObject);
    GameObject.DestroyImmediate (thirdCell.gameObject);
    ScriptableObject.DestroyImmediate (bus, false);
    }




[Test]
public void ManyCellsGetCalledWhenEventTriggered ()
    {
    var gameObject = new GameObject ();
    var bus = gameObject.AddComponent <Bus> ();

    var firstCell = CreateWithMonoBehaviour <BooleanTerminal> ();
    firstCell.Pin = "TestKey";
    var secondCell = CreateWithMonoBehaviour <BooleanTerminal> ();
    secondCell.Pin = "TestKey";
    var thirdCell = CreateWithMonoBehaviour <BooleanTerminal> ();
    thirdCell.Pin = "SomeDifferentTestKey";

    firstCell.Bus = bus;
    secondCell.Bus = bus;
    thirdCell.Bus = bus;

    this.CallOnEnable (firstCell);
    this.CallOnEnable (secondCell);
    this.CallOnEnable (thirdCell);

    int callbacksReceived = 0;
    int truesFromCallback = 0;
    firstCell.AddCallback (
            (bool value) => { ++callbacksReceived; truesFromCallback += value ? 1 : 0; }
            );
    secondCell.AddCallback (
            (bool value) => { ++callbacksReceived; truesFromCallback += value ? 1 : 0; }
            );
    bool didThirdCellGetCalled = false;
    thirdCell.AddCallback (
            (bool value) => { didThirdCellGetCalled = true; }
            );
    Assert.AreEqual (0, callbacksReceived, "no calls yet");
    bus.Signal ("TestKey", true);

    Assert.AreEqual (2, callbacksReceived, "both fubs for the key get callbacks when triggered");
    Assert.AreEqual (2, truesFromCallback, "both fubs for the key get the new value when triggered");
    Assert.IsFalse (didThirdCellGetCalled, "fub on a different key should not get a callback");

    GameObject.DestroyImmediate (firstCell.gameObject);
    GameObject.DestroyImmediate (secondCell.gameObject);
    GameObject.DestroyImmediate (thirdCell.gameObject);
    ScriptableObject.DestroyImmediate (bus, false);
    }


[Test]
public void CellDoesNotCalledWhenValueIsRemoved ()
    {
    var gameObject = new GameObject ();
    var bus = gameObject.AddComponent <Bus> ();
    bus.Set ("TestKey", false);

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
public void CellGetsCalledWithValueSetOnBusWithoutCellsWhenBused ()
    {
var gameObject = new GameObject ();
    var bus = gameObject.AddComponent <Bus> ();
    bus.Set ("TestKey", true);

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
