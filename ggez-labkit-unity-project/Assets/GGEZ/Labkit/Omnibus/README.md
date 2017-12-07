
# Omnibus: Connect all the things!

 * Easily create data-driven UI behavior in prefabs
 * Hook application-level systems together without creating dependencies
 * Let me manually poke things at runtime


## Terminology

Heavily repurposed from integrated circuit design and computer engineering.

-------------------------------------------------------------------------------
| Term            | Omnibus Meaning                                           |
-------------------------------------------------------------------------------
| Bus             | Data communication system that ties together cells        |
| Signal          | Events and memory cell changes                            |
| Cell            | Uses a bus for I/O. Has ports and pins. Uses wires.       |
| Router          | Connects ports (on cells) to busses                       |
| Port            | Pins on a cell with related functionality                 |
| Wire            | Connects a bus's output pin to a cell's input pin         |
| Event           | A one-time signal with an optional payload                |
| Memory Cell     | A single System.Object held at an address                 |
| ROM             | Read-Only Memory to initialize the Bus's memory cells     |
| Module          | A complex cell                                            |
| Fub             | Functional Unit Block. Collections of other parts.        |
| Mux (Cell)      | Read bus using string from another bus. (Multiplexer)     |
| Terminal (Cell) | Connects signal to Unity Event                            |
| Gate (Cell)     | A Terminal with functionality                             |
| Latch (Cell)    | Connects signal to bus                                    |
-------------------------------------------------------------------------------

## Quick Demo

 0. Create a new scene
 1. Create a `Bus` as an asset (right-click > Create > GGEZ > Omnibus > Bus)
 2. Add a `Boolean` value to its ROM using the Unity inspector
 3. Name it "enabled" and check to box to set its value to `true`
 4. Add a `Boolean Via` to the `Directional Light` game object in the scene
 5. Add an event handler to the `Boolean Via` that dynamically sets the light component's `enabled` property
 6. Hook up the Bus asset you created to the `Bus` property of the `Boolean Via`
 7. Run the scene. Open your Bus asset and toggle the checkbox. Your light will toggle on and off.

## Concepts

A Fub registers itself to a Bus for one or more keys in OnEnable. If a key is in memory, it immediately receives an OnDidChange callback with the key's value. After that:
 * The fub will receive OnDidTrigger if Bus.Trigger is called for that key
 * The fub will receive OnDidChange if Bus.Set is called and with a non-equal value