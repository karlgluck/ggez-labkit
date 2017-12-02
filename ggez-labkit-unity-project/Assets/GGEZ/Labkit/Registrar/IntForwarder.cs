using System;
using UnityEngine;
using UnityEngine.Events;

namespace GGEZ
{

[Serializable] public sealed class UnityEvent_int : UnityEvent<int> { }
[Serializable] public sealed class IntForwarder : Forwarder<int, UnityEvent_int> { }

}
