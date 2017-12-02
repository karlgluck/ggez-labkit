using System;

namespace GGEZ
{

[Serializable] public sealed class UnityEvent_String : UnityEngine.Events.UnityEvent<string> { }
[Serializable] public sealed class StringForwarder : Forwarder<string, UnityEvent_String> { }

}
