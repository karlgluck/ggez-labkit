using System;

namespace GGEZ
{

[Serializable] public sealed class UnityEvent_Float : UnityEngine.Events.UnityEvent<float> { }
[Serializable] public sealed class FloatForwarder : Forwarder<float, UnityEvent_Float> { }

}
