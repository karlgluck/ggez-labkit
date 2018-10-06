

# Design Principles

## Phase Order

The phase order is:

 1. **Circuit** - Changed cells run update
 2. **Program** - Scripts run update
 3. **Collection Register** - Registers deriving from `ICollectionRegister` clear their change-tracking members
 4. **Variable Rollover** - Variables assign their new values to registers and mark changed cells for update

## Principles

 * Variables allow golems to be update-order independent since variables only change their value on a frame-boundary.
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

As a consequence of the phase order:

 * Circuits that write collection registers propagate changes correctly to both cells and scripts
 * Scripts that write collection registers would have their changes erased before they are seen by a cell

 The solution is for scripts to only write variables. This makes changes leapfrog the Collection Register phase so that they'll be seen by cells (and scripts) on the next frame.
 
 As a bonus, it removes the ambiguity of whether scripts can expect to share data with one another through registers.



















    OKAY AND in this verion SPECIAL BONUS

    - GolemClass becomes GolemComponent
    - You can add multiple components to a single Golem
        - When it's in a prefab, that's an Archetype!
    - This means that references need to be separable from the class
        - Which means that a Golem holds a list of references keyed by name
        - When a GolemComponent instantiates, it reads Unity Object references by name from the Golem and writes them into fields of aspects/cells/scripts

    WAAAAAIt what if the references were just in the Aspects for the golem? Just like variables!!!
        Inside of a GolemComponent, you get a dropdown for picking a reference to a name based on the field in the aspect
    An aspect declares a field that is of type UnityObject, and all fields with the same name get the same value
        When the golem instantiates, the list of references gets cloned
        the Golem itself maps which fields of which aspects get each UnityObject index

    For debugging purposes, each GolemClass can have a list of the Aspects it depends on
    And each external reference can have a list of the Aspects they require of the target
        These can both be checked at runtime

    the Golem says "I have these settings, these Aspects and these Components"
        - And, transparently, "these object references" as contained in Aspects.

    the Golem contains:
        - A list of GolemComponents it uses
        - A list of settings, a superset of settings used in the GolemComponents
        - A list of aspect types that Golem uses
        - A map of name -> Unity Object references
            - And what aspects to write the references into
        - A list of {variable name / register index / aspect index / field name} for variables

    the GolemComponent:
        - Contains a list of {variable name / [cell/script index] / field name / reftype} for local variables
        - Contains {setting name, [cell/script index], field name } to write values on load
        (contains variables to write to field values on load)
        (contains unity object references to write to field values on load)



                             frame start
----------------------------------------------------------------------

                         CIRCUIT UPDATE PHASE

----------------------------------------------------------------------

                         PROGRAM UPDATE PHASE

----------------------------------------------------------------------

                      COLLECTION REGISTER UPDATE

----------------------------------------------------------------------

                           VARIABLE ROLLOVER

----------------------------------------------------------------------
                               frame end

collection registers are tricky...

within a circuit, we want a collection register to immediately track adds/removes
    lets say we have a cell that adds numbers in sequence to a hashset
    and another cell that has that hashset as an input
        the input should see "Added" contain each number in sequence and "Values" contain all the numbers

    if a script does the same thing, we expect that the cell gets
        to see "Added" the exact same thing

    if a script writes to a variable, we again expect that a dependent
        cell sees the same thing (so added has to be updated DURING circuit)
        and that a variable reading that register sees the same thing (so added can't be cleared between circuit & program)
        and that a cell reading a variable sees added (so added can't be cleared between variable update and circuit)
    
So where is the boundary where "Added" gets cleared?
    in the circuit case, it's just before circuits are updated; in the script case, it's jsut before scripts are updated

    but actually it's even more subtle than that:
        if a circuit is evaluated multiple times because it is in a cell that gets its inputs re-dirtied
        by an external dependency, the cell COULD be evaluated multiple times in a frame

    SO multiple-evaluation is actually a problem...

        "added" really wants to mean "added since you last saw this"
        and the problem is multiple readers:
            - if a cell has 2 inputs, one of which is a collection register and the other of which
              reads a remote cell's value, which changes after that cell is dirtied, then the
              cell will get used twice and the reader will see "added" twice.
        
        basically, this construction would require that cells be IDEMPOTENT on their inputs
            scripts can be non-idempotent since they are only evaluated once per frame at most
        
            - if a cell has a register input and a script has a register variable input for that
              register, and a script has a register output
            
        if we say that scripts can't write registers--they can only write variables--then
        we know that all writes will survive a clear after the program phase. this would mean that
        collections written during the circuit phase propagate immediatly between cells and to scripts,
        and collections written during the program phase would propagate to cells and to scripts on
        the next frame. BAM!


problem registers on different golems that listen to each other
possibly creates an infinite loop if you aren't careful...
 - when iterating, dirty instructions for the current cell should
   be executed immediately, but dirty instructions for later
   cells should be batched, sorted and done later

 - if sequence > current:
    add to priority queue
 - else:
    add to new queue














 - as simple to use and extend as possible
 - limit-zero per-frame allocations (no boxing/unboxing)
 - limit-zero per-frame reflection

cells only get updated when an input was changed
don't want to make different versions of every cell where
some read from variables in different combinations and
some read from registers

variables are just named registers, possibly in other golems

each variable in a golem is stored in that golem's registers
    so internal variables are easily mapped
    and the "self" reference never changes so we don't have to worry about how to update it

external variables that read from other golems are different
    #1 we don't know if they exist
    #2 references can change, which changes the variable

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
