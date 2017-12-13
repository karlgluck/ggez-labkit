
# Omnibus: Connect all the things!

Omnibus is a tool for establishing connections.

Omnibus **should be used to**:

 * Easily create data-driven behavior in prefabs
 * Hook application-level systems together without creating dependencies
 * Let you manually inspect and poke values at runtime to see what happens

Omnibus **should not be used**:

 * As a programming replacement (don't write code using Omnibus)
 * As a designer or artist tool

It is intentional that it isn't straightforward to hook outputs of one cell to the inputs of another. Don't build your game's logical systems in the GUI with Omnibus. Use Omnibus to easily represent the game state your code's functional systems create. If you find yourself needing more complex functionality, write a new cell rather than trying to combine existing cells in clever ways.

Omnibus is still a work in progress :)


## Terminology

Heavily repurposed from integrated circuit design and computer engineering.

-------------------------------------------------------------------------------
| Term            | Omnibus Meaning                                           |
-------------------------------------------------------------------------------
| Bus             | Data communication system with memory and events          |
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
| Connector       | Muxes, Gates and Filters. Link with a Terminal.           |
| Terminal (Cell) | Receives a signal from a Connector and does something     |
| Buffer          | No-op Connector                                           |
| Mux (Cell)      | Read bus using string from another bus. (Multiplexer)     |
| Latch (Cell)    | Connects signal to bus                                    |
| Gate (Cell)     | Logic-function Connector                                  |
| Filter (Cell)   | Parametric Connector                                      |
-------------------------------------------------------------------------------

Cells generally have:
 * Self-contained input and output (from bus to Unity), and are called modules
 * Separate input (connector) and output (terminal) units that hook together and are called compound cells

Other kinds of cells exist:
 * LatchWritesBusMemoryCell

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

# Other Stuff

A system created from Omnibus is theoretically synthesizeable: a purpose-built program should be able to take the codebase for a project and compile it into a version that resolves all types statically. Omnibus isn't made for production but this could possibly provide a route in that way if it matters.
