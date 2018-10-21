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
using System.Collections.Generic;

namespace GGEZ.Labkit
{

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

        /// <summary>
        ///     Determine whether this transition should be taken.
        /// </summary>
        public bool Evaluate(bool[] triggers)
        {
            int triggerPtr = 0;
            Stack<bool> evaluation = s_stackForProcessingTransitions;
            evaluation.Clear();
            for (int k = 0; k < Expression.Length; ++k)
            {
                switch (Expression[k])
                {
                    case Transition.Operator.And: evaluation.Push(evaluation.Pop() & evaluation.Pop()); break;
                    case Transition.Operator.Or: evaluation.Push(evaluation.Pop() | evaluation.Pop()); break;
                    case Transition.Operator.Push: evaluation.Push(triggers[(int)Triggers[triggerPtr++]]); break;
                    case Transition.Operator.True: evaluation.Push(true); break;
                    case Transition.Operator.False: evaluation.Push(false); break;
                }
            }
            Debug.Assert(evaluation.Count == 1);
            bool shouldTransition = evaluation.Pop();
            return shouldTransition;
        }

        private static Stack<bool> s_stackForProcessingTransitions = new Stack<bool>();

    }
}
