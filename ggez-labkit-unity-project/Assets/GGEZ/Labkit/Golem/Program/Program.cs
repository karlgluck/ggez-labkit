// This is free and unencumbered software released into the public domain.
//
// Anyone is free to copy, modify, publish, use, compile, sell, or
// distribute this software, either in source code form or as a compiled
// binary, for any purpose, commercial or non-commercial, and by any
// means.
//
// In jurisdictions that recognize copyright laws, the author or authors
// of this software dedicate any and all copyright interest in the
// software to the public domain. We make this dedication for the benefit
// of the public at large and to the detriment of our heirs and
// successors. We intend this dedication to be an overt act of
// relinquishment in perpetuity of all present and future rights to this
// software under copyright law.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// For more information, please refer to <http://unlicense.org/>

using System;
using UnityEngine;
using GGEZ.FullSerializer;

namespace GGEZ.Labkit
{

    [fsSerializeEnumAsInteger]
    public enum StateIndex : int
    {
        Any = -2,
        Idle = -1,
        Invalid = int.MaxValue,
    }

    //-------------------------------------------------------------------------
    // Layer
    //-------------------------------------------------------------------------
    public sealed class Layer
    {
        public StateIndex[] States;
        // TODO: separate entry and any-state transitions
        // then put the state-to-state transitions into the
        // states themselves so we don't have to scan the whole
        // list every single time
        public Transition[] Transitions;
    }

    //-------------------------------------------------------------------------
    // State
    //-------------------------------------------------------------------------
    public sealed class State
    {
        public int[] Scripts;
    }

    //-------------------------------------------------------------------------
    // Transition
    //-------------------------------------------------------------------------
    public sealed class Transition
    {
        public enum Operator
        {
            Push,
            And,
            Or,
            True,
            False,
        }

        /// If this is < 0, the transition can start in any state
        public StateIndex FromState;

        /// If this is < 0, taking the transition will disable the layer
        public StateIndex ToState;

        /// Whether or not this transition is active is determined by evaluating
        /// Expression. `Push` will evaluate the next trigger and add its value
        /// to a stack. `And` and `Or` take the top 2 values of the stack, pop
        /// them, then push the result of applying the boolean operator to those
        /// values. Once all operators have been executed, the value remaining
        /// on the stack defines whether the transition is active.
        public Operator[] Expression;
        public Trigger[] Triggers;
    }
}
