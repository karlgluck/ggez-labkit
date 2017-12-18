
# Omnibus: Connect all the things!

Omnibus is a tool for establishing connections in Unity. It is intended for fast prototyping.

Omnibus **should be used** to:

 * Easily create data-driven behavior for prefabs and scenes
 * Hook systems together without creating dependencies
 * Let you manually inspect and poke values at runtime to see what happens

Use Omnibus to easily represent the game state your code's functional systems create. If you find yourself needing more complex functionality, write a new cell rather than trying to combine existing cells in clever ways.

Each piece of Omnibus is robust and simple. New, self-contained functionality with a narrow scope is preferred to adding flags to existing code. As a result, combining parts should not have surprising results. It should also not be capable of complex behaviour that requires debugging or analysis. Omnibus is meant to reduce boilerplate, not replace programming. On that note...

Omnibus **should not be used**:

 * As a programming replacement
 * As a designer or artist tool

Don't build your systems by stringing together Omnibus components. You'll be tempted, but again: *Omnibus is not a replacement for Playmaker or Blueprints*.

It will seem that if only some seemingly small changes are made to how cells hook together, you could easily express more complex behavior using Omnibus. This is a siren's song! If you really need those things, don't use Omnibus.



## Terminology

Heavily repurposed from integrated circuit design and computer engineering.

-------------------------------------------------------------------------------
| Term            | Omnibus Meaning                                           |
-------------------------------------------------------------------------------
| Bus             | Data communication object. Has memory cells and channels. |
| ROM             | Read-Only Memory to initialize the bus's memory cells     |
| Signal          | Events and memory cell changes                            |
| Cell            | Receives from a bus. Has ports and pins. Uses wires.      |
| Pin             | Named input or output                                     |
| Wire            | Connects a bus's output pin to a cell's input pin         |
| Memory Cell     | A single System.Object held at a named address            |
| Event           | A one-time signal with an optional payload                |
| Channel         | Named address for event                                   |
| Module          | A single-object Fub. A cell with no junction.             |
| Fub             | Functional Unit Block = input cell + output terminal      |
| Junction        | Interface between a fub's input cell and terminal         |
| Terminal        | Performs output action with a cell's signal               |
| Buffer          | No-op cell to pass signal to junction                     |
| Mux             | Read bus using string from another bus. (Multiplexer)     |
| Gate            | Logic function                                            |
| Filter          | Parametric function                                       |
| Port            | Named group of pins on a cell                             |
| Router          | Connects ports (on cells) to busses                       |
| Latch (Cell)    | This is being reworked.                                   |
-------------------------------------------------------------------------------

Cells are the only things that can receive signals from a bus. Cells form Fubs that are:
 * Self-contained input and output, and are called modules
 * An input cell and an output terminal that hook together


## Quick Demo

 0. Create a new scene
 1. Create a `Bus` as an asset (right-click > Create > GGEZ > Omnibus > Bus)
 2. Add a `Boolean` value to its ROM using the Unity inspector
 3. Name it "enabled" and check to box to set its value to `true`
 4. Add a `BooleanUnityEventModule` to the `Directional Light` game object in the scene
 5. Add an event handler to the `BooleanUnityEventModule` that dynamically sets the light component's `enabled` property
 6. Hook up the Bus asset you created to the `Bus` property of the `BooleanUnityEventModule`
 7. Run the scene. Open your Bus asset and toggle the checkbox. Your light will toggle on and off.

# Other Stuff

A system created from Omnibus is theoretically synthesizeable: a purpose-built program should be able to take the codebase for a project and compile it into a version that statically resolves all types and connections. The current version of Omnibus focuses on being good for prototyping. If having a way to production matters, this would be the way I'd attempt.




Handy unused terms:

 * Buffer
 * Interconnect
 * Via
 * Register
 * Fabric
 * Junction
 * Block, Macroblock
 * Bridge
 * Clock
 * Trace
 * Edge
 * Flip-Flop
 * Plug
 * Connector
 * Adapter
