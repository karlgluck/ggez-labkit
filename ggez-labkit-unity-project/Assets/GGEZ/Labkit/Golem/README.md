# What is Golem?

Golem accelerates game prototyping by erasing **glue code** from your game.

**Glue code** is code that doesn't do much other than connect two (or more) systems to each other. An easy example is tying the `Hitpoints` and `MaxHitpoints` fields on a character to the UI bar floating above their head in-game. It's not hard code. We've all written it... but it's everywhere, it can quickly turn into spaghetti, and it's annoying to write over and over again. 

Golem fixes that! It gives you:

 - [x] **Circuits** - Visual expression of data-flow logic (declarative programming)
 - [x] **Programs** - Visual expression of state-machine logic (synchronous parallelized imperative programming)
 - [x] **Aspects** - Aribtrary classes for your own code to use that benefit from Golem's magic assignments
 - [x] **Settings** - To put values in fields of many aspects/circuits/programs from one single place. Inheritable!
 - [ ] **Mirrors** - To solve "for each member of list Y, give me an X, even if Y changes or I swap Y for Z". Great for UI.
 - [ ] **Channels** - Publisher/subscriber event notifications to help golems talk to each other

It gives you all this while also:

 - Minimizing overhead
 - Working on all Unity platforms
 - Respecting performance

# How does it work?

First things first: **using Golem requires you to write C# code**. The editor resembles visual scripting in a lot of ways, but that's really not what it's good at. If you try to use it that way it will kinda work but you'll mostly be sad.

Golem is a tool for programmers. It helps you focus on writing the part of your scripts you actually care about in prototyping. With Golem, you get to just sit down and write gameplay. The golem editor provides an interface for two kinds of behavior that are much more easily expressed in graphs than in text:

 1. Data logic that describes how inputs become outputs (e.g. "when my HP goes down, update my hp bar and for 250 milliseconds leave a white section showing how much I just lost")
 2. State machine logic that describes how parallel behaviors interact

## How do I use it?

Attach the Golem component to something. Add a Golem Component. Edit it to set up a **program** and/or a **circuit**. Hit play and away you go! ... more detail to come here ...

# Architectural Decisions

In this section, I describe why Golem is implemented the way it is. I've tried to write down the stuff that's not obvious from the code because it's either high level structural stuff or important nitty-gritty choices that could seem arbitrary at first glance.

## Phase Order

The phase order is:

 1. **Circuit** - Changed cells run update
 2. **Program** - Scripts run update
 3. **Collection Register** - Registers deriving from `ICollectionRegister` clear their change-tracking members
 4. **Variable Rollover** - Variables assign their new values to registers and mark changed cells for update

Because we need to maintain this phase order across all golems, the central `GolemManager` class manages all golem updates. A Golem itself has no `Update` function.

## Variables only change their value on a frame-boundary

So that each golem's update can be mutually order-independent. It also tees 

 * Golems running without modification quickly require no allocation and no reflection to execute. In particular, this means no boxing.

## Cells can be updated even if their inputs don't change

It could be that an input was changed and then changed back

## Cell register inputs can read named variables but register outputs cannot write them

Register outputs are written immediately. If a variable's register were to be written mid-frame, the value that a golem reads would depend on when it executed. This violates the principle that variables only change their value between frames.

To write a named variable, a cell must explicitly use a variable reference.

There is a way around this to allow scripts to read values from the circuit on the same frame: a scripts's variable can be a variable that is created for a register. Since registers are internal, the update order is fixed so there is no sequencing problem. Furthermore, because of the phase order, the circuit will always see the latest variable value written by a script and vice-versa.

## Cells update at most once per frame

Because:

 * Cells can be circularly linked to each other across golems. This causes an infinite loop in either case, but if we only allow one update per frame, the loop is non-blocking.
 * A cell should be able to rely on Time.deltaTime
 * If a cell can be updated multiple times in a single frame, the number of updates it gets depends on its own input dependencies and the overall circuit's dependencies on other golems. In particular, this could cause an issue if you rely on `Added` or `Removed` in a collection register. It'd be possible to see the same value multiple times, to see supersets of previous values, and to see a value `Removed` that was never seen added or vice-versa.

## Scripts can only write variables

If scripts only write variables, changes leapfrog the Collection Register phase so that they'll be seen by cells (and scripts) on the next frame. Otherwise, circuits would be able to propagate changes to scripts correctly but the reverse would not be true. Doing this also removes the ambiguity of whether scripts can expect to share data with one another through registers by answering "no".

## Cells and scripts that care about collection registers / variables must implement change tracking themselves

A cell can use an input collection register's `Added` / `Removed` sets to track changes in the input. However, the cell also needs to track changes in the entire collection itself since those changes are reflected in neither the old nor the new collection register's `Added` / `Removed` sets.

This could have been implemented by asking variables to be properties instead of fields so that a setter could be written to handle this case. At first glance, this seems like a clean solution since the setter would have access to both the old and new collections simultaneously. However, in practice, this still asks a cell to queue any changes it might make for later evaluation during `Update` in order to prevent out-of-order operations. I expect most users of collection registers will be more heavy-weight and need to manage complex state anyway, so adding the requirement that they also check for collection changes seems reasonable.

## Port direction is identified by name

 * Input registers are either called `Input` or they end in `In`.
 * Output registers are either called `Output` or they end in `Out`.

There are several ways this could be done and the choice is largely a matter of style. This was chosen because it reinforces the nature of the register each time it is used, which should help avoid bugs and it minimizes extra typing in an editor with autocomplete.

## Circuits can write variables without violating order-independence

The fact that this is possible has tripped me up a few times and I keep thinking I have to do something special to fix it, so I thought I'd just write down the conclusion. Circuits writing variables is fine because of the variable phase. Even though circuits queue cells for processing, writing a variable does not immediately cause the cells that read the variable to be processed; hence, a cell is able to write a variable during iteration of the dirty-cell priority queue without causing issues like having a cell be evaluated multiple times in one frame.


# Future Plans

## Undo / Redo

Involves making classes like `EditorCell`, `EditorState` and `EditorScript` into `ScriptableObject`s. Nice side-benefit is that Unity will then take care of serializing these things' references to each other, so we can back out the JSON serialization of the classes into just the user's runtime classes themselves.

## Golem pooling

To save memory and make creating/destroying golems cheap, they can be easily pooled. Much of the code is already there! Just need to test it.

## ECS

The structure of a Golem's cells and scripts shouldn't be too hard to modify to take advantage of Unity's new entity-component system. Cell and script instances already pretty much act like components with their behavior being defined by code in the archetype/component.

## Upgrade to the Unity 2018 Elements UI

It's got a built-in node editor. That's why I didn't spend too long getting all the node editing right in this version... just 40 hours or so. RIP.

## Network Variable Replication

One of the cool things about update order independence, variables only changing on frame boundaries and knowing all external variables from within Golem itself is that it is a natural fit for automatic replication. Mix that with messages basically being RPCs and away we go!

# Ideas

 - Make sure that Any State states are checked for validity: have no inputs, don't output to multiple layers, don't have scripts, etc...

Buncha crazy stuff down here... all WIP notes to myself




 - as simple to use and extend as possible
 - limit-zero per-frame allocations (no boxing/unboxing)
 - limit-zero per-frame reflection

cells only get updated when an input was changed
don't want to make different versions of every cell where
some read from variables in different combinations and
some read from registers

so for external references, maybe polling is fine?
    we want things like "owner" though which doesn't change

maybe external references have that second level of indirection? A special cell type just converts external variable into internal register. It can check the global change list to see if a variable has changed, then propagate the change internally.

Now we have to know how to split the variable lists:
- Single, global list (all instances of all types share)
    advantage: simplicity, reduced overhead
    disadvantage: basically have to do my own memory management on the buffers as instances are created/destroyed
- By Golem class (all instances share)
    advantage: pooling
    disadvantage: everything that interacts with a variable needs a reference to the class's variables array
- By Golem instance (nothing is shared)
    advantage: simplicity
    disadvantage: lots of overhead; everything that interacts with a variable needs a reference to the instance's variables array

If we assume golems are pooled and never destroyed, the single global list is pretty attractive.

Cell data storage for the class includes:
    - A dictionary of {type} -> {values count} so space can be reserved in the global data arrays
    - the initial cell data (to deserialize the cell)
    - A dictionary of {variable name} -> {register offset}
    - An array of pairs mapping cell field names to register offsets
        These get incremented by the golem's own offset into the global array on load (uses reflection)
    - A dictionary of {reference} -> {variable name, register}
        This dictionary is shared among all instances after being deserialized once

But the register needs access to the list of cells to dirty
This list is the same for all golems of a given type and is readonly
Which means we should probably have golems of a type share an array
And each register then needs its golem to get its data
Which means then that cells need golem references

Since a writing register needs:
    T[] Values; <-- the values array
    int golemBase; <-- where this golem starts
    int registerOffset; <-- this register's offset
    int[] cellsToDirty; <-- which cells are dirtied by writing this register

But at this point we are gaining what, 12 bytes per type per golem by pooling? Might as well just allocate their own registers and be done with it.

    T[] Values; <-- the values array, for this golem instance
    int registerOffset; <-- which register to write
    int[] cellsToDirty; <-- which cells are to be dirtied by writing this register
    bool[] dirtyArray; <-- golem instance's dirty cells array

can save on the dirtyArray reference by passing a golem pointer around

for variables, they have exactly the same thing only they are updated at end of frame
    T[] NextValues;     // internal to owner, shared with registers
    int registerOffset; // internal to owner
    int[] cellsToDirty; // internal to owner
    bool[] dirtyArray;  // internal to owner
    T PreviousValue;    // for reading values
    List<VariableListener<T>> OnChanged;
    public void DetachAll() { ... }

// a VariableListener is created for every register that listens to remote variables
// when a variable reference is hooked up, the appropriate IVariableListener (of
// true type VariableListener<T>) is given an IVariable (of actual type Variable<T>)
// to attach tiself to.
VariableListener<T> : IVariableListener
    T[] values;
    int registerOffset;
    int[] cellsToDirty;
    bool[] dirtyArray;
    public void OnDidChange(T newValue)
    {
        dirtyArray
    }
    public void Attach(IVariable variable)
    {
        var target = variable as Variable<T>;
        Debug.Assert(target != null);
    }
    public void Detach(IVariable variable)
    { ... }

// The golem has a dictionary of its variables by name -> IVariable (Variable<T>)


    when a variable changes, it adds itself to a global dirty list to be updated at end-of-frame.
    all references to a variable are the same object. Reading a value reads the PreviousValue.
    At end of frame, if the variable is in the dirty list, it gets PreviousValue updated
    and iterates the OnChanged list to propagate changes

    they set cell dirty flags (which is fine, we don't have to wait to do this
    since the transition occurs between program and circuit phases) then the
    program does a copy through of all the value types.

    but what about processing order? script insensitive relative processing order
    might be important but that doesnt hold for basic properties of aspects, inter-golem
    processing order is basically handled by ... well you can read remote references
    at any time so I don't know. Maybe Variable<T> is a class instead of a struct and
    it gets added to an update list on the target? Or all variable references are shared
    and it holds its previous value internally?

but variables can also have an external set of listeners

In the class...
    - A dictionary of {variableId} -> register
        Whenever a variable changes, it checks a global changelist for {variableId} and writes listening registers as well

There is a cell type that reads a remote reference into a register
    When a reference gets hooked up, the cell is given the index of
    the variable that changed and the cell starts updating

RegisterIn<T>
    T[] Values;

Golem
    int[] variableIndex;

registers are in the global list but not shared with variables so we don't have to use reflection to update them when the pointed-at object changes

instead, the

if each variable was a [type, changed] pair
every variable reference could just point to that memory location and do a direct check

get/set then get a backing field that is the reference

first N variablerefs are registers
next K variablerefs are instantiated for objects

on load:
    - keep dictionary of <<relationship,name>, IVariableRef>
    - scan all fields for anything that is an IVariableRef
    - add the field's value to the master list of variablerefs

- variablerefs array for registers, sorted by relationship
- variablerefs array for scripts & cells, sorted by relationship
- function to get bounds of relationship in variablerefs array


on change relationship:
- get bounds of relationship in variablerefs array
- foreach (int i in bounds) { variableref.UpdateTarget(Variables); copy value; SetRegisterDirty(i); }
- do the same thing for scripts and cells...and aspects?

each frame:
    foreach i in [variablerefs array for registers]:
        did variable i change?
            copy value; SetRegisterDirty(i);


RegisterPtr<bool> _register;


ELIMINATE THE COST OF BOXING AND UNBOXING
STRONGLY TYPE EVERYTHING AND TIE REFERENCES CLOSE TO USE POINTS

public interface IRegisterOut
{
    void RuntimeSetup(Golem golem);
}

public parital class Golem
{
    Dictionary<Type, object> _registerBanks;
    public RuntimeSetup<T>(out T[] registerBank, out bool[] dirty)
    {
        registerBank = _registerBanks[typeof(T)] as T[];
        dirty = _dirty;
    }
}

public class RegisterOut<T>
{
    public int Index;
    public int[] CellsToDirty;

    private T[] Values;
    private bool[] Dirty;

    public void RuntimeSetup(Golem golem)
    {
        golem.RuntimeSetup(out Values, out Dirty);
    }

    public void Set(T value)
    {
        if (Values[Index] != value)
        {
            Values[Index] = value;
            for (int i = 0; i < CellsToDirty.Length; ++i)
            {
                Dirty[CellsToDirty[i]] = true;
            }
        }
    }
}

public class HashSetRegisterOut<T>
{
    public int Index;
    public int[] CellsToDirty;

    private HashSet<T> Register;
    private bool[] Dirty;

    public void RuntimeSetup(Golem golem)
    {
        golem.RuntimeSetupHashSetRegister(Index, out Register, out Dirty);
    }

    public void Add(T value)
    { }

    public void Remove(T value)
    { }

    public void Set(IEnumerable<T> values)
    { }
}

public class RegisterIn<T>
{
    public T[] Values;
    public int Index;

    public T Get()
    {
        return Values[Index];
    }
}

private Variable<float> _foo; // aspect has a variable named _foo created in the golem that owns this variable
public float Foo { set { _foo.Value = value; } get { return _foo.Value; } }

private HashSetVariable<int> _bar; // this aspect has a variable named _bar
public void BarAdd(int i) { _bar.Add(i); }
public void BarRemove(int i) { _bar.Remove(i); }
public HashSet<int> BarValues(int i) { return _bar.Values; }
public HashSet<int> BarAdded(int i) { return _bar.Added; }
public HashSet<int> BarRemoved(int i) { return _bar.Removed; }

public interface IVariable
{
//    void Set(object value);
//    object Get();
    void NextFrame();
}

IVariable variable;
string name = variableField.Name;
if (!variables.TryGetValue(name, out variable))
{
    // This works on AOT because the field is explicitly declared
    variable = Activator.CreateInstance(variableField.FieldType);
    variables.Add(name, variable);
}
variableField.Set(aspect, variable);


Variables
{
    Dictionary<string, IVariable> Variables;

    public Variable<T> Get<T>(name)
    {
        Variables.TryGetValue(name out variable);
        var v = variable as Variable<T>;
        return v.Value;
    }
}

does dirty only matter for registers?
- if script scare about frame separation then no, because
  if golem passively separates frames then new values should not update until end of frame

public static class VariableUtility
{
    public static List<IVariable> ChangedNextFrame = new List<IVariable>();
}

public class Variable<T> : IVariable
{
    public T Value
    {
        get
        {
        }
        set
        {
            _nextValue = value;
            Dirty
        }
    }

    private T _value, _nextValue;

    // A variable might be dirty if it hasn't changed, but it WILL be dirty if it has changed
    public bool Dirty;

    private bool _nextDirty;

    public void NextFrame()
    {
        _value = _nextValue;
        Dirty = _nextDirty;
        _nextDirty = false;
    }
}

but VariableRef<T> would contain a Variable reference...

public interface IVariableRef
{
    bool IsLocal();
    IVariable Find(Dictionary<string, IVariable> variables);
}

public class GenericVariableRef : IVariableRef
{

}

public class VariableRef<T> : IVariableRef
{
    public string Relationship;
    public string Name;
    public Variable<T> Variable;

    public void Find(Dictionary<string, IVariable> variables)
    {
        IVariable value;
        variables.TryGetValue(Name, out value);
        Variable = value as Variable<T>;
        if (Variable)
        {
            Variable.Changed = true;
        }
    }
}


public class VariableRegisterRef<T> : IVariableRef
{
    public string Relationship;
    public string Name;
    public Variable<T> Variable;

    // When the value changes or the variable is dirty, write the given register
    public T[] Registers;
    public int Register;
    public bool[] Dirty;
    public int[] CellsToDirty;

    public void Find(Dictionary<string, IVariable> variables)
    {
        IVariable value;
        variables.TryGetValue(Name, out value);
        Variable = value as Variable<T>;
        if (Variable)
        {
            Registers[Register] = Variable.Value;
            Dirty[CellsToDirty] = true;
        }
    }
}

public class HashSetVariableRef<T> : IVariableRef
{
    public string Relationship;
    public string Name;
    public HashSetVariable<T> Variable;

    public void Find(Dictionary<string, IVariable> variables)
    {
        IVariable value;
        variables.TryGetValue(Name, out value);
        Variable = value as HashSetVariable<T>;
        if (Variable)
        {
            Variable.Changed = true;
        }
    }
}

after load, from fields of all scripts, get all IVariableRef instances
for each instance.IsLocal(): instance.Find(this.Variables)
for each reg in _variablesThatWriteRegister: where reg.IsLocal(): reg.Find(this.Variables) and set registers[reg.Register] dirty/value


the golem would have a list of references...

    List<IVariableRef> refs;

whenever a relationship changes, it looks through the list of references to update them

Dictionary<string, Relationship>

class Relationship
{
    public string Name;
    public Variables Target;
    public List<IVariableRef> References;
}

Dictionary<string, IVariable> Variables;


foreach (RegisterReferencingVariable regRef in _variablesThatWriteRegister)
{
    if (regRef.Variable && regRef.Variable.Dirty)
    {
        changeRegister(regRef.Register, regRef.Variable.GetValue());
    }
}

SetRelationship(string key, Variables target)
{
    // get Value ...
    Relationship r;
    foreach (var ref in r.References)
    {
        IVariable variable = ref.Find(target);

    }
}





































ulong[] _dirty = new ulong[MAX_REGISTERS/64];

for (int i = 0; i < _dirty.Length; ++i)
{
    _dirty[i] = 0;
}

for (int i = 0; i < _activeRegisterVariables.Count; ++i)
{
    var registerVariable = _activeRegisterVariables[i];
    int register = registerVariable.Register;
    _dirty[register / 64] = registerVariable.Variable.Dirty << (register & 64);
    _registers[register] = registerVariable.Variable.GetValue();
}

// Reverse the direction of cell reads to a pull rather than a push

int maskSize;
uint* mask = &_registersThatCellReadsMask[maskSize * i];
uint dirtyMask = 0;
for (int i = 0; i < maskSize; ++i)
{
    dirtyMask |= _dirty[i] & mask[i];
}

for (int i = 0; i < Cells.Length; ++i)
{
    RegisterPtr[] registers = _registersThatCellReads[cellIndex];
    bool dirty = false;
    for (int i = 0; !dirty && i < registers.Length; ++i)
    {
        dirty = _dirty[registers[i]]
    }
    bool running = _running[cellIndex];
    if (dirty || running)
    {
        Cells[cellIndex].Update(this, dirty, ref running);
        _running[cellIndex] = running;
    }
}
