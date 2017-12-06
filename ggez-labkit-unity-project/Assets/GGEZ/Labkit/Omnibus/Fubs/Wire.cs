
namespace GGEZ
{
namespace Omnibus
{

public class Wire
{
public bool IsConnected { get { return this.cell != null; } }
private ICell cell;
public string CellPin;
public string BusPin;
public Bus Bus;

public static Wire CELL_IN { get { return new Wire (Pin.IN); } }
public static Wire CELL_DATA { get { return new Wire (Pin.DATA); } }

public Wire (string cellPin)
    {
    this.CellPin = cellPin;
    }

public void Connect (ICell cell)
    {
    this.Bus.Connect (this.BusPin, cell);
    this.cell = cell;
    }

public void Disconnect ()
    {
    this.Bus.Disconnect (this.BusPin, this.cell);
    }

public void ReconnectBus (Bus bus)
    {
    this.Reconnect (bus, this.BusPin);
    }

public void ReconnectPin (string pin)
    {
    this.Reconnect (this.Bus, pin);
    }

public void Reconnect (Bus bus, string pin)
    {
    if (object.ReferenceEquals (this.Bus, bus) && this.BusPin == pin)
        {
        return;
        }
    bool connected = this.IsConnected;
    if (connected && this.Bus != null && !Pin.IsValid (this.BusPin))
        {
        this.Bus.Disconnect (this.BusPin, this.cell);
        }
    this.Bus = bus;
    this.BusPin = pin;
    if (connected && this.Bus != null && !Pin.IsValid (this.BusPin))
        {
        this.Bus.Connect (this.BusPin, this.cell);
        }
    }

}


}
}
