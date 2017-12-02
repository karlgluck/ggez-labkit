using UnityEngine;
using UnityEngine.Events;

namespace GGEZ
{



public class Forwarder<T, D> : Listener where D : UnityEvent<T>
{
[SerializeField] private D didTriggerOrChange;

public override void OnDidTrigger (string key, object value)
    {
    this.didTriggerOrChange.Invoke ((T)value);
    }

public override void OnDidChange (string key, object value)
    {
    this.didTriggerOrChange.Invoke ((T)value);
    }
}



public class EventForwarder<T> : Listener where T : UnityEvent<T>
{
[SerializeField] private T didTrigger;
public override void OnDidTrigger (string key, object value)
    {
    this.didTrigger.Invoke ((T)value);
    }
}




public class ChangeForwarder<T> : Listener where T : UnityEvent<T>
{
[SerializeField] private T didChange;
public override void OnDidChange (string key, object value)
    {
    this.didChange.Invoke ((T)value);
    }
}



}
