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

using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using GGEZ;
using GGEZ.Labkit;
using System.Linq;
using System.Diagnostics;

namespace GGEZ.Tests
{
    public class VariablesTests
    {
        public const string Key0 = "Key 0";
        public const string Key1 = "Key 1";

        public static readonly object Value0 = "value0";
        public static readonly object Value1 = 9999;
        
        struct ABC
        {
            public int foo;

            public void SetMyValue(int v)
            {
                foo = v;
            }
        }

[StructLayout(LayoutKind.Sequential)]
        class ABCtainer
        {
            public ABC abc = new ABC();
        }

        [Test]
        public void WackyStuff()
        {
            ABC aBC = new ABC();
            System.Action<int> i = aBC.SetMyValue;
            UnityEngine.Debug.Log("foo = " + System.Runtime.InteropServices.Marshal.SizeOf(typeof(ABC)));
            i.Invoke(123);
            UnityEngine.Debug.Log("foo = " + System.Runtime.InteropServices.Marshal.SizeOf(typeof(ABCtainer)));

            

            // double sum = 0.0;
            // int iterations = 99;
            // for (int j = 0; j < iterations; ++j)
            // {
            //     Stopwatch watch = Stopwatch.StartNew();
            //     for (int i = 0; i < 99999; ++i)
            //     {
            //         RuntimeVariables<float>.Values.Add(i / 128f);
            //     }
            //     watch.Stop();
            //     RuntimeVariables<float>.Values.Clear();
            //     RuntimeVariables<float>.Values.Add(0f);
            //     double microSeconds = (watch.ElapsedTicks * 1.0e6 / Stopwatch.Frequency + 0.4999);
            //     sum += microSeconds;
            // }
            // sum /= (double)iterations;
            // UnityEngine.Debug.Log("microSeconds = " + sum);
        }

        [Test]
        public void MultiAddSingle()
        {
            HashSetVariable<object> variables = new HashSetVariable<object>();
            variables.Add(Value0);

            Assert.False(variables.Values.Contains(Value0));
            Assert.False(variables.Added.Contains(Value0));
            Assert.False(variables.Removed.Contains(Value0));

            variables.EndFrame();

            Assert.True(variables.Values.Contains(Value0));
            Assert.True(variables.Added.Contains(Value0));
            Assert.False(variables.Removed.Contains(Value0));

            variables.EndFrame();

            Assert.True(variables.Values.Contains(Value0));
            Assert.False(variables.Added.Contains(Value0));
            Assert.False(variables.Removed.Contains(Value0));
        }

        [Test]
        public void MultiRemoveSingle()
        {
            HashSetVariable<object> variables = new HashSetVariable<object>();
            variables.Add(Value0);

            variables.EndFrame();

            variables.Remove(Value0);

            Assert.True(variables.Values.Contains(Value0));
            Assert.True(variables.Added.Contains(Value0));
            Assert.False(variables.Removed.Contains(Value0));

            variables.EndFrame();

            Assert.False(variables.Values.Contains(Value0));
            Assert.False(variables.Added.Contains(Value0));
            Assert.True(variables.Removed.Contains(Value0));
        }

        [Test]
        public void MultiRemoveAdded()
        {
            HashSetVariable<object> variables = new HashSetVariable<object>();
            variables.Add(Value0);
            variables.Remove(Value0);

            variables.EndFrame();

            Assert.False(variables.Values.Contains(Value0));
            Assert.False(variables.Added.Contains(Value0));
            Assert.False(variables.Removed.Contains(Value0));
        }

        [Test]
        public void MultiAddRemoved()
        {
            HashSetVariable<object> variables = new HashSetVariable<object>();
            variables.Add(Value0);

            variables.EndFrame();

            variables.Remove(Value0);
            variables.Add(Value0);

            variables.EndFrame();

            Assert.True(variables.Values.Contains(Value0));
            Assert.False(variables.Added.Contains(Value0));
            Assert.False(variables.Removed.Contains(Value0));
        }
    }
}
