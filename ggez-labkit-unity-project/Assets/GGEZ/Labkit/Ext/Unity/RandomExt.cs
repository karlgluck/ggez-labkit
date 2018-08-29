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

namespace GGEZ
{
    public static partial class RandomExt
    {
        public static double Range(double min, double max)
        {
            return UnityEngine.Random.value * (max - min) + min;
        }

        private class WrapUnityRandomAsSystemRandom : System.Random
        {
            public override int Next(int minValue, int maxValue)
            {
                return UnityEngine.Random.Range(minValue, maxValue);
            }

            public override int Next(int maxValue)
            {
                return UnityEngine.Random.Range(0, maxValue);
            }

            public override int Next()
            {
                return UnityEngine.Random.Range(0, int.MaxValue);
            }

            public override void NextBytes(byte[] buffer)
            {
                for (int i = 0; i < buffer.Length; ++i)
                {
                    buffer[i] = (byte)UnityEngine.Random.Range(0, byte.MaxValue);
                }
            }

            public override double NextDouble()
            {
                return UnityEngine.Random.value;
            }

            protected override double Sample()
            {
                return UnityEngine.Random.value;
            }
        }

        public static readonly System.Random UnityRandom = new WrapUnityRandomAsSystemRandom();
    }
}
