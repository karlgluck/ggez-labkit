using UnityEngine;
using GGEZ.Labkit;

public enum Trigger
{
    Next,
    Last,

    // add more triggers here

    __COUNT__,
    Invalid = int.MaxValue,
}

namespace GGEZ.Labkit
{
    public class AddFloat : Cell
    {
        // [In] // Hooked up to read-only input if not attached
        // [In, CanBeNull]  // Not hooked up if not attached
        public StructRegister<float> Input;

        // [Out] // Hooked up to write-only output if not attached
        // [Out, CanBeNull] // Not hooked up if not attached
        public StructRegister<float> Output;

        public float Amount;

        public override void Update()
        {
            Output.Value = Input.Value + Amount;
        }
    }

}


public class JoystickAspect : Aspect
{
    public Transform joystickRoot;
    public Transform joystick;

    public float DeadZone = 5f;

    public float Limit = 155f;

    public float AngleThreshold = 55f;

    public bool JoystickActive;
    public int JoystickURDL;
    public float JoystickDirection;
    public bool JoystickTouch;
    public bool RawActionButtonInput;

    [Variable("ShowJoystick", "Whether the joystick is displayed on screen")]
    public StructVariable<bool> Show;

    // private StructVariable<bool> _show;
    // public bool Showx
    // {
    //     get { return _show.Value; }
    //     set { _show.Value = value; }
    // }
}

public class FishingRodAspect : GGEZ.Labkit.Aspect
{
    public float fishingRodPosition;
}

public class PositionAspect : GGEZ.Labkit.Aspect
{
    [Variable("X", "The location of this object in 3d space")]
    public StructVariable<float> X;

    [Variable("Y", "The location of this object in 3d space")]
    public StructVariable<float> Y;

    [Variable("Z", "The location of this object in 3d space")]
    public StructVariable<float> Z;
}

// public class Instantiator : Cell
// {
//     [In(typeof(HashSetVariable<int>))] public RegisterPtr Input;
//     public GameObject Prefab;
//     public Transform Parent;
// }


// public class ACellWithManyIOs : Cell
// {
//     [In(typeof(bool))] public RegisterPtr Input;
//     [In(typeof(bool))] public RegisterPtr Input1;
//     [In(typeof(bool))] public RegisterPtr Input2;
//     [In(typeof(bool))] public RegisterPtr Input3;
//     [In(typeof(bool))] public RegisterPtr Input4;
//     [In(typeof(bool))] public RegisterPtr Input5;
//     [Out(typeof(bool))] public RegisterPtr Output;
//     [Out(typeof(bool))] public RegisterPtr Output1;
//     [Out(typeof(bool))] public RegisterPtr Output2;
//     [Out(typeof(bool))] public RegisterPtr Output3;
//     [Out(typeof(bool))] public RegisterPtr Output4;
//     [Out(typeof(bool))] public RegisterPtr Output5;

// }


// public class BoolInverterCell : Cell
// {
//     [In(typeof(bool))] public RegisterPtr Input;
//     [Out(typeof(bool))] public RegisterPtr Output;

//     public override void Update(Golem entity, bool dirty, ref bool running)
//     {
//         entity.Set(Output, !entity.Get<bool>(Input));
//     }
// }

// public class RendererToggleCell : Cell
// {
//     [In(typeof(bool))] public RegisterPtr Input;
//     public Renderer Renderer;

//     public override void Update(Golem entity, bool dirty, ref bool running)
//     {
//         Renderer.enabled = entity.Get<bool>(Input);
//     }
// }

// public class ReadFishingRodPosition : Cell
// {
//     [Out(typeof(float))] public RegisterPtr Output;

//     public override void Acquire(Golem entity, ref bool running)
//     {
//         running = true;
//     }

//     public override void Update(Golem entity, bool dirty, ref bool running)
//     {
//         entity.Set(Output, entity.FishingRodAspect.fishingRodPosition);
//     }
// }


// public class ReadFloatVariable : Cell
// {
//     [VariableType(typeof(float))]
//     public VariableRef Variable;

//     [Out(typeof(float))] public RegisterPtr Output;

//     public override void Acquire(Golem entity, ref bool running)
//     {
//         running = true;
//     }

//     public override void Update(Golem entity, bool dirty, ref bool running)
//     {
//         float value = 0f;
//         if (entity.Read(Variable, ref value))
//         {
//             entity.Set(Output, value);
//         }
//     }
// }




// public class FloatConstant : Cell
// {
//     public float Value;
//     [Out(typeof(float))] public RegisterPtr Output;
//     public override void Update(Golem entity, bool dirty, ref bool running)
//     {
//         entity.Set(Output, Value);
//     }
// }


// public class LerpFloatToZero : Cell
// {
//     public float Duration;
//     [In(typeof(float))] public RegisterPtr Input;
//     [Out(typeof(float))] public RegisterPtr Output;
//     private float current, deltaPerSecond;
//     public override void Update(Golem entity, bool dirty, ref bool running)
//     {
//         if (dirty)
//         {
//             current = entity.Get<float>(Input);
//             deltaPerSecond = current / Duration;
//         }
//         else
//         {
//             current = Mathf.MoveTowards(current, 0f, deltaPerSecond * Time.smoothDeltaTime);
//         }
//         running = current != 0f;
//     }
// }



// public class EmitFloatSequence : Cell
// {
//     public float Duration;
//     [In(typeof(object))] public RegisterPtr Trigger;
//     [Out(typeof(float))] public RegisterPtr Output;
//     private float _startTime, _endTime;
//     public override void Update(Golem entity, bool dirty, ref bool running)
//     {
//         if (dirty)
//         {
//             _startTime = Time.time;
//             _endTime = _startTime + Duration;
//         }
//         entity.Set(Output, Mathf.InverseLerp(_startTime, _endTime, Time.time));
//         running = Time.time < _endTime;
//     }
// }



// public class SineWave : Cell
// {
//     public float Amplitude = 1f;
//     public float Frequency = 1f;
//     [In(typeof(bool))] public RegisterPtr Enabled;
//     [Out(typeof(float))] public RegisterPtr Output;

//     public override void Acquire(Golem entity, ref bool running)
//     {
//         running = Enabled == RegisterPtr.Invalid;
//     }

//     public override void Update(Golem entity, bool dirty, ref bool running)
//     {
//         if (dirty)
//         {
//             running = entity.Get<bool>(Enabled);
//         }
//         entity.Set(Output, Mathf.Sin(Frequency * Time.time) * Amplitude);
//     }
// }


// public class Position : Cell
// {
//     public Transform Transform;
//     [In(typeof(float))] public RegisterPtr X;
//     [In(typeof(float))] public RegisterPtr Y;
//     [In(typeof(float))] public RegisterPtr Z;

//     public override void Update(Golem golem, bool dirty, ref bool running)
//     {
//         var position = Transform.position;
//         golem.TryGet(X, ref position.x);
//         golem.TryGet(Y, ref position.y);
//         golem.TryGet(Z, ref position.z);
//         Transform.position = position;
//     }
// }

// public class KeyPressed : Cell
// {
//     public KeyCode Key;
//     [Out(typeof(bool))] public RegisterPtr Output;

//     public override void Acquire(Golem entity, ref bool running)
//     {
//         running = true;
//     }

//     public override void Update(Golem entity, bool dirty, ref bool running)
//     {
//         entity.Set(Output, Input.GetKey(Key));
//     }
// }


public class SetObjectNameToTime : Script
{
    [GGEZ.FullSerializer.fsIgnore]
    public GameObject GameObject;

    public override void OnUpdate()
    {
        GameObject.name = Time.time.ToString();
    }
}


public class SetVariableToTime : Script
{
    public StructVariable<float> Variable;

    public override void OnUpdate()
    {
        Debug.Log("OnUpdate("+Time.time+")");
        Variable.Value = Time.time;
    }
}

// public class SetVariableToTimeInState : Script
// {
//     public VariableRef Variable;

//     private float _entryTime;

//     public override void OnEnter(Golem entity)
//     {
//         _entryTime = Time.time;
//     }

//     public override void OnUpdate(Golem entity)
//     {
//         entity.Write(Variable, Time.time - _entryTime);
//     }
// }

// public class AddDeltaTimeToVariable : Script
// {
//     [VariableType(typeof(float))]
//     public VariableRef Variable;

//     public override void OnUpdate(Golem entity)
//     {
//         float value = 0f;
//         entity.Read(Variable, ref value);
//         entity.Write(Variable, value + Time.smoothDeltaTime);
//     }
// }

// public class TransitionAfterDelay : Script
// {
//     public float Delay;
//     public Trigger Trigger;

//     private float _exitTime;

//     public override void OnEnter(Golem entity)
//     {
//         _exitTime = Time.time + Delay;
//     }

//     public override void OnUpdate(Golem entity)
//     {
//         if (Time.time > _exitTime)
//         {
//             entity.SetTrigger(Trigger);
//         }
//     }
// }
