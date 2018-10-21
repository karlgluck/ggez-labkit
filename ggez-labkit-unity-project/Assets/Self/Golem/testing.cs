using UnityEngine;
using GGEZ.Labkit;
using System.Linq;

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
        public StructRegister<float> Input;

        public StructRegister<float> Output;

        public float Amount;

        public override void Update()
        {
            Output.Value = Input.Value + Amount;
        }
    }

    public class Damp : Cell
    {
        public StructRegister<float> Input;

        public StructRegister<float> Output;

        public float Smoothing;

        public override void Update()
        {
            Output.Value = Util.Damp(Output.Value, Input.Value, Smoothing, Time.smoothDeltaTime);
            if (!Mathf.Approximately(Output.Value, Input.Value))
            {
                GolemManager.UpdateCellNextFrame(this);
            }
        }
    }

    public class DebugLogFloat : Cell
    {
        public StructRegister<float> Input;

        public override void Update()
        {
            Debug.Log("@ " + Time.time + " Value = " + Input.Value);
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

    [Tooltip("Whether the joystick is displayed on screen")]
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
    [Tooltip("The location of this object in 3d space")]
    public StructVariable<float> X;

    [Tooltip("The location of this object in 3d space")]
    public StructVariable<float> Y;

    [Tooltip("The location of this object in 3d space")]
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
//     [In] public StructRegister<float> Input;
//     [In] public StructRegister<float> Input1;
//     [In] public StructRegister<float> Input2;
//     [In] public StructRegister<float> Input3;
//     [In] public StructRegister<float> Input4;
//     [In] public StructRegister<float> Input5;
//     [Out] public StructRegister<float> Output;
//     [Out] public StructRegister<float> Output1;
//     [Out] public StructRegister<float> Output2;
//     [Out] public StructRegister<float> Output3;

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
//     [Out]
//     public StructRegister<float> Output;
//     [Out]
//     public StructRegister<float> Output2x;
//     [Out]
//     public StructRegister<float> Output4x;
//     [Out]
//     public StructRegister<float> xOutput;
//     [Out]
//     public StructRegister<float> xOutput2x;
//     [Out]
//     public StructRegister<float> xOutput4x;
//     [Out]
//     public StructRegister<float> yOutput;
//     [Out]
//     public StructRegister<float> yOutput2x;
//     [Out]
//     public StructRegister<float> yOutput4x;
//     [Out]
//     public StructRegister<float> zOutput;
//     [Out]
//     public StructRegister<float> zOutput2x;
//     [Out]
//     public StructRegister<float> zOutput4x;
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

    public float Multiplier;

    public override void OnUpdate()
    {
        Variable.Value = Time.time;
    }
}


public class TransitionAfterDelay : Script
{
    public float Delay;
    public Trigger Trigger;

    private float _exitTime;

    public Golem Golem;

    public override void OnEnter()
    {
        _exitTime = Time.time + Delay;
    }

    public override void OnUpdate()
    {
        if (Time.time > _exitTime)
        {
            Golem.SetTrigger(Trigger);
        }
    }
}

public class OnKeySetTrigger : Script
{
    public Golem Golem;

    public KeyCode Key;

    public Trigger Trigger;

    public override void OnUpdate()
    {
        if (Input.GetKeyDown(Key))
        {
            Golem.SetTrigger(Trigger);
        }
    }
}

public class CollectionTester : Script
{
    public HashSetVariable<float> Times;

    public KeyCode AddKey;
    public KeyCode RemoveKey;

    public override void OnUpdate()
    {
        if (Input.GetKeyDown(AddKey) || (Input.GetKey(AddKey) && Input.GetKeyDown(RemoveKey)))
        {
            for (int i = Random.Range(1, 5); i >= 0; --i)
            {
                Times.Add(Random.value);
            }
        }
        if (Input.GetKeyDown(RemoveKey) || (Input.GetKey(RemoveKey) && Input.GetKeyDown(AddKey)))
        {
            var array = Times.Values.ToArray();
            GGEZ.RandomExt.Shuffle(GGEZ.RandomExt.UnityRandom, array);
            for (int i = Mathf.Min(array.Length-1, Random.Range(1, 5)); i >= 0 && Times.Values.Count > 0; --i)
            {
                Times.Remove(array[i]);
            }
        }
    }
}

public class DebugMirror : Cell
{
    public HashSetRegister<float> Input;

    public override void Update()
    {
        Debug.Log(Input);
    }

}