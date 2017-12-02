using System;

namespace GGEZ
{

[Serializable] public sealed class UnityEvent_Boolean : UnityEngine.Events.UnityEvent<Boolean> { }
[Serializable] public sealed class BooleanForwarder : Forwarder<Boolean, UnityEvent_Boolean> { }

}
