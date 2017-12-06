
namespace GGEZ
{
namespace Omnibus
{


public interface ICell
{

void OnDidSignal (string pin, object value);

void Route (string net, Bus bus);

}


}
}
