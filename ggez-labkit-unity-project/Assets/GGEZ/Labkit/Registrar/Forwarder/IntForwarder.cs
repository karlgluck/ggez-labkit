using System;

namespace GGEZ
{

[Serializable] public sealed class UnityEvent_int : UnityEngine.Events.UnityEvent<int> { }
[Serializable] public sealed class IntForwarder : Forwarder<int, UnityEvent_int> { }

}
