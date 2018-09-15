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

public class JoystickAspect : Aspect
{
    public Transform joystickRoot;
    public Transform joystick;

    [Setting("Joystick.DeadZone", 5f, "Moving the mouse fewer than this many pixels from center does nothing")]
    public float DeadZone;

    [Setting("Joystick.Limit", 155f, "Description")]
    public float Limit;

    [Setting("Joystick.AngleThreshold", 55f, "Description")]
    public float AngleThreshold;

    public bool JoystickActive;
    public int JoystickURDL;
    public float JoystickDirection;
    public bool JoystickTouch;
    public bool RawActionButtonInput;

    [Variable("ShowJoystick", true, "Whether the joystick is displayed on screen")]
    public bool Show
    {
        get { return (bool)Variables.Get("ShowJoystick"); }
        set { Variables.Set("ShowJoystick", value); }
    }
}

namespace GGEZ.Labkit
{
public partial class Golem
{
    public JoystickAspect JoystickAspect;
}
}

public class FishingRodAspect : GGEZ.Labkit.Aspect
{
    public float fishingRodPosition;
}

namespace GGEZ.Labkit
{
public partial class Golem
{
    public FishingRodAspect FishingRodAspect;
}
}



public class ACellWithManyIOs : Cell
{
    [In] public BoolPtr Input;
    [In] public BoolPtr Input1;
    [In] public BoolPtr Input2;
    [In] public BoolPtr Input3;
    [In] public BoolPtr Input4;
    [In] public BoolPtr Input5;
    [Out] public BoolPtr Output;
    [Out] public BoolPtr Output1;
    [Out] public BoolPtr Output2;
    [Out] public BoolPtr Output3;
    [Out] public BoolPtr Output4;
    [Out] public BoolPtr Output5;

}


public class BoolInverterCell : Cell
{
    [In] public BoolPtr Input;
    [Out] public BoolPtr Output;

    public override void Update(Golem entity, bool dirty, ref bool running)
    {
        entity.Set(Output, !entity.Get(Input));
    }
}

public class RendererToggleCell : Cell
{
    [In] public BoolPtr Input;
    public Renderer Renderer;

    public override void Update(Golem entity, bool dirty, ref bool running)
    {
        Renderer.enabled = entity.Get(Input);
    }
}

[RequireAspect(typeof(FishingRodAspect))]
public class ReadFishingRodPosition : Cell
{
    [Out] public FloatPtr Output;

    public override void Acquire(Golem entity, ref bool running)
    {
        running = true;
    }

    public override void Update(Golem entity, bool dirty, ref bool running)
    {
        entity.Set(Output, entity.FishingRodAspect.fishingRodPosition);
    }
}


[RequireVariables]
public class ReadFloatVariable : Cell
{
    public VariableRef Variable;
    [Out] public FloatPtr Output;
    public override void Acquire(Golem entity, ref bool running)
    {
        running = true;
    }
    public override void Update(Golem entity, bool dirty, ref bool running)
    {
        float value = 0f;
        if (entity.Read(Variable, ref value))
        {
            entity.Set(Output, value);
        }
    }
}




public class FloatConstant : Cell
{
    public float Value;
    [Out] public FloatPtr Output;
    public override void Update(Golem entity, bool dirty, ref bool running)
    {
        entity.Set(Output, Value);
    }
}


public class LerpFloatToZero : Cell
{
    public float Duration;
    [In] public FloatPtr Input;
    [Out] public FloatPtr Output;
    private float current, deltaPerSecond;
    public override void Update(Golem entity, bool dirty, ref bool running)
    {
        if (dirty)
        {
            current = entity.Get(Input);
            deltaPerSecond = current / Duration;
        }
        else
        {
            current = Mathf.MoveTowards(current, 0f, deltaPerSecond * Time.smoothDeltaTime);
        }
        running = current != 0f;
    }
}



public class EmitFloatSequence : Cell
{
    public float Duration;
    [In] public ObjectPtr Trigger;
    [Out] public FloatPtr Output;
    private float _startTime, _endTime;
    public override void Update(Golem entity, bool dirty, ref bool running)
    {
        if (dirty)
        {
            _startTime = Time.time;
            _endTime = _startTime + Duration;
        }
        entity.Set(Output, Mathf.InverseLerp(_startTime, _endTime, Time.time));
        running = Time.time < _endTime;
    }
}



public class SineWave : Cell
{
    public float Amplitude = 1f;
    public float Frequency = 1f;
    [In] public BoolPtr Enabled;
    [Out] public FloatPtr Output;

    public override void Acquire(Golem entity, ref bool running)
    {
        running = Enabled == BoolPtr.Invalid;
    }

    public override void Update(Golem entity, bool dirty, ref bool running)
    {
        if (dirty)
        {
            running = entity.Get(Enabled);
        }
        entity.Set(Output, Mathf.Sin(Frequency * Time.time) * Amplitude);
    }
}


public class Position : Cell
{
    public Transform Transform;
    [In] public FloatPtr X;
    [In] public FloatPtr Y;
    [In] public FloatPtr Z;

    public override void Update(Golem golem, bool dirty, ref bool running)
    {
        var position = Transform.position;
        golem.Get(X, ref position.x);
        golem.Get(Y, ref position.y);
        golem.Get(Z, ref position.z);
        Transform.position = position;
    }
}

public class KeyPressed : Cell
{
    public KeyCode Key;
    [Out] public BoolPtr Output;

    public override void Acquire(Golem entity, ref bool running)
    {
        running = true;
    }

    public override void Update(Golem entity, bool dirty, ref bool running)
    {
        entity.Set(Output, Input.GetKey(Key));
    }
}



public class SetVariableToTime : Script
{
    public VariableRef Variable;

    public override void OnUpdate(Golem entity)
    {
        entity.Write(Variable, Time.time);
    }
}

public class SetVariableToTimeInState : Script
{
    public VariableRef Variable;

    private float _entryTime;

    public override void OnEnter(Golem entity)
    {
        _entryTime = Time.time;
    }

    public override void OnUpdate(Golem entity)
    {
        entity.Write(Variable, Time.time - _entryTime);
    }
}

public class AddDeltaTimeToVariable : Script
{
    public VariableRef Variable;

    public override void OnUpdate(Golem entity)
    {
        float value = 0f;
        entity.Read(Variable, ref value);
        entity.Write(Variable, value + Time.smoothDeltaTime);
    }
}

public class TransitionAfterDelay : Script
{
    public float Delay;
    public Trigger Trigger;

    private float _exitTime;

    public override void OnEnter(Golem entity)
    {
        _exitTime = Time.time + Delay;
    }

    public override void OnUpdate(Golem entity)
    {
        if (Time.time > _exitTime)
        {
            entity.SetTrigger(Trigger);
        }
    }
}
