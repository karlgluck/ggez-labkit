using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace GGEZ
{



public class Forwarder<T, D> : Listener where D : UnityEvent<T>
{
[SerializeField] private ListenerKeys keys = new ListenerKeys ();
[SerializeField] private D didTriggerOrChange;

public override void OnDidTrigger (string key, object value)
    {
    this.didTriggerOrChange.Invoke ((T)value);
    }

public override void OnDidChange (string key, object value)
    {
    this.didTriggerOrChange.Invoke ((T)value);
    }

public override IEnumerable<string> GetKeys ()
    {
    return this.keys.Keys;
    }
}



public class EventForwarder<T> : Listener where T : UnityEvent<T>
{
[SerializeField] private ListenerKeys keys = new ListenerKeys ();
[SerializeField] private T didTrigger;
public override void OnDidTrigger (string key, object value)
    {
    this.didTrigger.Invoke ((T)value);
    }

public override IEnumerable<string> GetKeys ()
    {
    return this.keys.Keys;
    }
}




public class ChangeForwarder<T> : Listener where T : UnityEvent<T>
{
[SerializeField] private ListenerKeys keys = new ListenerKeys ();
[SerializeField] private T didChange;
public override void OnDidChange (string key, object value)
    {
    this.didChange.Invoke ((T)value);
    }

public override IEnumerable<string> GetKeys ()
    {
    return this.keys.Keys;
    }
}



}
