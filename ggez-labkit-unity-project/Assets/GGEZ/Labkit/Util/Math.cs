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

namespace GGEZ
{
    public static partial class Util
    {
        private static readonly char[] s_base62Alphabet = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        public static string ToBase62String(ulong number)
        {
            var n = number;
            ulong basis = 62;
            var ret = "";
            while (n > 0)
            {
                var temp = n % basis;
                ret = s_base62Alphabet[(int)temp] + ret;
                n = (n / basis);
            }
            return ret;
        }


        //----------------------------------------------------------------------
        // RepeatUniform doesn't mirror when crossing from positive to negative.
        //
        // Normally:
        //       [4 .. -4] % 3 ==> [1, 0, 2, 1, 0, -1, -2, 0, -1]
        //
        // Instead:
        //       RepeatUniform ([4 .. -4], 3) ==> [1, 0, 2, 1, 0, 2, 1, 0, 2]
        //
        // Using RepeatUniform makes the zero-boundary disappear. This is
        // useful for things like camera controls.
        //----------------------------------------------------------------------
        public static int RepeatUniform(int number, int range)
        {
            var retval = number % range;
            if (retval < 0)
            {
                retval += range;
            }
            return retval;
        }

        public static float RepeatUniform(float number, float range)
        {
            var retval = number - ((int)(number / range)) * range;
            if (retval < 0)
            {
                retval += range;
            }
            return retval;
        }
    }
}
