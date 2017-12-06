
namespace GGEZ
{
namespace Omnibus
{

public static partial class Pin
{
public const string IN = "in";
public const string OUT = "out";
public const string DATA = "out";
public static bool IsValid (string pin)
    {
    return !string.IsNullOrEmpty (pin);
    }
}

}
}
