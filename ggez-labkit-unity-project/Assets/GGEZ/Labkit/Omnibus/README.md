
# Omnibus: Connect all the things!

 * Easily create data-driven UI behavior in prefabs
 * Hook application-level systems together without creating dependencies
 * Let me manually poke things at runtime

Importantly, Omnibus is for establishing connections. It is *not* intended to replace programming or be an easy language for designers. It is intentional that there is no simple way to hook outputs of one cell to the inputs of another. Write code to do that or use a tool that's built for that purpose. Don't build your game's functional systems in the GUI with Omnibus. Use Omnibus to easily represent the game state your code's functional systems create.

Cells are either self-contained units called Modules or two-part compound units composed of a Gate or Filter and a Terminal.


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
| Filter (Cell)   | Converts a value and some parameters to another value     |
-------------------------------------------------------------------------------

Handy unused terms:

 * Buffer
 * Interconnect
 * Via
 * Register
 * Fabric
 * Junction
 * Macroblock
 * Bridge
 * Clock
 * Edge
 * Flip-Flop
 * Plug

## Quick Demo

 0. Create a new scene
 1. Create a `Bus` as an asset (right-click > Create > GGEZ > Omnibus > Bus)
 2. Add a `Boolean` value to its ROM using the Unity inspector
 3. Name it "enabled" and check to box to set its value to `true`
 4. Add a `Boolean Via` to the `Directional Light` game object in the scene
 5. Add an event handler to the `Boolean Via` that dynamically sets the light component's `enabled` property
 6. Hook up the Bus asset you created to the `Bus` property of the `Boolean Via`
 7. Run the scene. Open your Bus asset and toggle the checkbox. Your light will toggle on and off.

