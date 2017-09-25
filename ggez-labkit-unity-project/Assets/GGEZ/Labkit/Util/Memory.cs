using System.Reflection;

public static partial class Util
{
public static void Zero (object target)
	{
    var type = target.GetType ();
    PropertyInfo[] properties = type.GetProperties();
    for (int i = 0; i < properties.Length; ++i)
		{
        properties[i].SetValue (target, null, null);
		}
    FieldInfo[] fields = type.GetFields();
    for (int i = 0; i < fields.Length; ++i)
		{
        fields[i].SetValue (target, null);
		}
	}

public static void ReflectionCopy (object target, object source)
	{
    if (!object.ReferenceEquals (source.GetType (), target.GetType ()))
		{
        throw new System.InvalidOperationException ("Requires source and target to be of the same type");
		}
    var type = target.GetType ();
    PropertyInfo[] properties = type.GetProperties (BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.Public);
    for (int i = 0; i < properties.Length; ++i)
		{
        properties[i].SetValue (target, properties[i].GetValue (source, null), null);
		}
    FieldInfo[] fields = type.GetFields (BindingFlags.Instance | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.Public);
    for (int i = 0; i < fields.Length; ++i)
		{
        fields[i].SetValue (target, fields[i].GetValue (source));
		}
	}
}