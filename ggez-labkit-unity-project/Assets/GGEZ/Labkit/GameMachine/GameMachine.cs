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

using System.Collections;
using System;
using System.Reflection;

namespace GGEZ
{
    /*

    you can "call up" by dynamic dispatch
    - can replace your own state machine with another
    - can invoke a sub-state machine
    - interrupts can pull control out of a sub-state machine, but run in their parents' context
    - state machine has an "exit" queue that can have functions added to it dynamically. All will be
        called, in sequence, if the state machine is being shut down (e.g. because of superstate transition)

        */
    public class NewGameMachine
    {
        // public void Main ()
        //     {
        //     // Delegate d = null;
        //     // d.Method.Invoke (object, parameters)
        //     this.gameMachine.PushInterrupt ("LostNetworkConnection");
        //     this.gameMachine.PopInterrupt ("LostNetworkConection");
        //     this.gameMachine.PushInterrupt ("OnExit");
        //     }

        // public IEnumerator LostNetworkConnection ()
        //     {
        //     this.gameMachine.PopAll ();
        //     this.gameMachine.Push ("ReconnectToGame");
        //     }

        // public IEnumerator OnExit ()
        //     {

        //     }
    }

    //---------------------------------------------------------------------------------------
    // The GameMachine is an implementation of my Inverted Coroutines concept. It is designed
    // to be used with an EventPipe, but the two classes are not dependent on each other.
    //
    // The goal is to let coroutines govern high-level state logic, and yield when they need
    // to wait for some other process to do something.
    //
    // If you have dynamic dispatch methods named Handle and a main Run coroutine,
    // use this snippet in a MonoBehaviour to start the GameMachine:
    /*

    void Start ()
        {
        this.StartCoroutine (GameMachine.Start (this.Run(), GameMachine.CreateDynamicDispatchDelegate (this, "Handle")));
        }

    */
    //---------------------------------------------------------------------------------------

    public sealed class GameMachine
    {
        private IEnumerator _codePointer;
        private Func<object, IEnumerator> _dynamicDispatch;

        private GameMachine()
        {
        }

        public static IEnumerator Start(IEnumerator codePointer, Func<object, IEnumerator> dynamicDispatch)
        {
            return new GameMachine()
            {
                _codePointer = codePointer,
                _dynamicDispatch = dynamicDispatch,
            }.run();
        }

        public class GameMachineGoSub
        {
            internal IEnumerator CodePointer;
        }

        public static GameMachineGoSub GoSub(IEnumerator codePointer)
        {
            return new GameMachineGoSub
            {
                CodePointer = codePointer,
            };
        }

        public class GameMachineGoTo
        {
            public Func<IEnumerator> Code;
        }

        public static GameMachineGoTo GoTo(Func<IEnumerator> code)
        {
            return new GameMachineGoTo
            {
                Code = code
            };
        }

        private IEnumerator run()
        {
            while (_codePointer.MoveNext())
            {
                object retval = _codePointer.Current;
                var requestGoSub = retval as GameMachineGoSub;
                if (requestGoSub != null)
                {
                    var sub = new GameMachine()
                    {
                        _codePointer = requestGoSub.CodePointer,
                        _dynamicDispatch = _dynamicDispatch,
                    };
                    var subCodePointer = sub.run();
                    while (subCodePointer.MoveNext())
                    {
                        yield return subCodePointer.Current;
                    }
                    continue;
                }
                var requestGoTo = retval as GameMachineGoTo;
                if (requestGoTo != null)
                {
                    _codePointer = requestGoTo.Code.Invoke();
                    continue;
                }
                if (retval != null)
                {
                    var queryCodePointer = (IEnumerator)_dynamicDispatch(retval);
                    if (queryCodePointer == null)
                    {
                        continue;
                    }
                    while (queryCodePointer.MoveNext())
                    {
                        yield return queryCodePointer.Current;
                    }
                    continue;
                }
                else
                {
                    yield return null;
                }
            }
        }

        public static Func<object, IEnumerator> CreateDynamicDispatchDelegate(object target, string methodName)
        {
            return delegate (object parameter)
                {
                    var method = target.GetType().GetMethod(
                    methodName,
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                    null,
                    new Type[] { parameter.GetType() },
                    null
                    );
                    return (IEnumerator)method.Invoke(target, new object[] { parameter });
                };
        }
    }
}
