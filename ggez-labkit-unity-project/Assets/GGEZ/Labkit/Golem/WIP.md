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

    public T[] Values;
    public bool[] Dirty;

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
    void Set(object value);
    object Get();
    bool IsDirty();
    void SetDirty();
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




public class Variable<T> : IVariable
{
    public T Value { get; set; }

    // A variable might be dirty if it hasn't changed, but it WILL be dirty if it has changed
    public bool Dirty;
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
