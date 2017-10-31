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

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
namespace GGEZ
{

//---------------------------------------------------------------------------------------
// Object queue allowing coroutines to wait for object types, trigger delegates and time.
//
// Usage:
/*
private EventPipe mainPipe = new EventPipe ();

IEnumerator SomeCoroutine ()
    {
    var wait = this.mainPipe.WaitForEventType (typeof (MyEvent));
    yield return wait;
    var myEvent = (MyEvent)wait.ReturnedEvent;
    // do something with myEvent here
    }

*/
//
// Sample dynamic-dispatch code for these requests using the method "Handle". This is
// for use with the GameMachine.
//
/*
IEnumerator Handle (EventPipe.WaitForEventTypeEnumerator request)
    {
    while (request.MoveNext())
        {
        yield return request.Current;
        }
    }


IEnumerator Handle (EventPipe.WaitForEventInTypeSetEnumerator request)
    {
    while (request.MoveNext())
        {
        yield return request.Current;
        }
    }

IEnumerator Handle (EventPipe.WaitForTriggerOrEventTypeEnumerator request)
    {
    while (request.MoveNext())
        {
        yield return request.Current;
        }
    }
*/
//---------------------------------------------------------------------------------------

public class EventPipe
{
private ArrayList queue = new ArrayList();
private HashSet<Type> typesBeingBuffered = new HashSet<Type>();
private ArrayList buffer = new ArrayList();

public void Add (object obj)
    {
    if (this.typesBeingBuffered.Contains (obj.GetType()))
        {
        this.buffer.Add (obj);
        }
    else
        {
        this.queue.Add (obj);
        }
    }

public void Clear ()
    {
    this.queue.Clear ();
    this.buffer.Clear ();
    }

public void BeginBuffering (Type type)
    {
    this.typesBeingBuffered.Add (type);
    }

public void StopBufferingAll ()
    {
    this.queue.AddRange (this.buffer);
    this.buffer.Clear();
    this.typesBeingBuffered.Clear();
    }

public WaitForEventTypeEnumerator WaitForEventType (Type type)
    {
    return new WaitForEventTypeEnumerator (this, type);
    }

public class WaitForEventTypeEnumerator : IEnumerator
    {
    private EventPipe owner;

    public Type EventType { get; private set; }

    public object ReturnedEvent;

    internal WaitForEventTypeEnumerator (EventPipe owner, Type type)
        {
        this.EventType = type;
        this.owner = owner;
        }

    public bool MoveNext ()
        {
        for (int i = 0; i < this.owner.queue.Count; ++i)
            {
            var e = this.owner.queue[i];
            if (this.EventType.IsAssignableFrom (e.GetType()))
                {
                this.ReturnedEvent = e;
                this.owner.queue.RemoveAt (i);
                return false;
                }
            }
        this.owner.queue.Clear();
        return true;
        }

    public object Current
        {
        get
            {
            return null;
            }
        }

    public void Reset ()
        {
        throw new System.NotImplementedException();
        }
    }

public WaitForEventInTypeSetEnumerator WaitForEventInTypeSet (params Type[] types)
    {
    return new WaitForEventInTypeSetEnumerator (this, types);
    }

public class WaitForEventInTypeSetEnumerator : IEnumerator
    {
    private EventPipe owner;

    internal WaitForEventInTypeSetEnumerator (EventPipe owner, Type[] types)
        {
        this.owner = owner;
        this.EventTypes = types;
        }

    public Type[] EventTypes { get; private set; }
    public object ReturnedEvent;
    public int ReturnedIndex;


    public bool MoveNext ()
        {
        for (int i = 0; i < this.owner.queue.Count; ++i)
            {
            var e = this.owner.queue[i];
            for (int j = 0; j < this.EventTypes.Length; ++j)
                {
                var eventType = this.EventTypes[j];
                if (eventType.IsAssignableFrom (e.GetType ()))
                    {
                    this.ReturnedEvent = e;
                    this.ReturnedIndex = j;
                    this.owner.queue.RemoveAt (i);
                    return false;
                    }
                }
            }
        this.owner.queue.Clear();
        return true;
        }

    public object Current
        {
        get
            {
            return null;
            }
        }

    public void Reset ()
        {
        throw new System.NotImplementedException();
        }
    }

public WaitForTriggerOrEventTypeEnumerator WaitForTriggerOrEventType (Func<bool> trigger, Type type)
    {
    return new WaitForTriggerOrEventTypeEnumerator (this, trigger, type);
    }

#if UNITY
public WaitForTriggerOrEventTypeEnumerator WaitForSecondsOrEventType (float seconds, Type type)
    {
    float endTime = UnityEngine.Time.time + seconds;
    return new WaitForTriggerOrEventTypeEnumerator (this, () => return UnityEngine.Time.realtimeSinceStartup > endTime, type);
    }

public WaitForTriggerOrEventTypeEnumerator WaitForSecondsRealtimeOrEventType (float seconds, Type type)
    {
    float endTime = UnityEngine.Time.realtimeSinceStartup + seconds;
    return new WaitForTriggerOrEventTypeEnumerator (this, () => return UnityEngine.Time.realtimeSinceStartup > endTime, type);
    }
#endif

public class WaitForTriggerOrEventTypeEnumerator : IEnumerator
    {
    private EventPipe owner;
    internal WaitForTriggerOrEventTypeEnumerator (EventPipe owner, Func<bool> trigger, Type type)
        {
        this.owner = owner;
        this.Trigger = trigger;
        this.EventType = type;
        }

    public Func<bool> Trigger;
    public Type EventType;
    public object ReturnedEvent;


    public bool MoveNext ()
        {
        if (this.Trigger ())
            {
            this.ReturnedEvent = null;
            return false;
            }
        for (int i = 0; i < this.owner.queue.Count; ++i)
            {
            var e = this.owner.queue[i];
            if (this.EventType.IsAssignableFrom (e.GetType()))
                {
                this.ReturnedEvent = e;
                this.owner.queue.RemoveAt (i);
                return false;
                }
            }
        this.owner.queue.Clear();
        return true;
        }

    public object Current
        {
        get
            {
            return null;
            }
        }

    public void Reset ()
        {
        throw new System.NotImplementedException();
        }
    }

}



}
