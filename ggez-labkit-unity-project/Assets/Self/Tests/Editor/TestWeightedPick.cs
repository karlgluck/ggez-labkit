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
using System.Linq;

public class TestWeightedPick
{


[Test]
public void DoesntPickZeroWeight ()
	{
    var picked = new int[] { 0, 1, 0 }.PickWeighted (RandomExt.UnityRandom, 1, e => e).First ();
    Assert.AreEqual (picked, 1);
	}



[Test]
public void PicksByWeightTwoElements ()
	{
    var timesPicked = new int[2] { 0, 0 };
    var arrayToPickFrom = new int[] { 1, 2 };
    const int kIterations = 10000;
    for (int i = 0; i < kIterations; ++i)
        {
        var picked = arrayToPickFrom.PickWeighted (RandomExt.UnityRandom, 1, e => e).First ();
        timesPicked[picked - 1]++;
        }
    Assert.Greater (timesPicked[1], timesPicked[0], "should pick 2 weight more than 1 weight");
    Assert.AreEqual (
            (double)timesPicked[0],
            (double)timesPicked[1]/2.0,
            kIterations * 2 / 100.0,
            "should pick 2 weight about twice as much as 1 weight"
            );
	}



[Test]
public void PicksByWeightManyElements ()
	{
    const int kLength = 100;
    const int kIterations = 10000;
    var timesPicked = new int[kLength];
    var arrayToPickFrom = new int[kLength];
    int totalWeight = 0;
    for (int i = 0; i < kLength; ++i)
        {
        arrayToPickFrom[i] = i + 1;
        totalWeight += arrayToPickFrom[i];
        }
    for (int i = 0; i < kIterations; ++i)
        {
        var picked = arrayToPickFrom.PickWeighted (RandomExt.UnityRandom, 1, e => e).First ();
        timesPicked[picked - 1]++;
        }
    for (int i = 0; i < kLength; ++i)
        {
        Assert.AreEqual (
                timesPicked[i] / ((double)kIterations),
                (i + 1) / ((double)totalWeight),
                kIterations * 1 / 100.0,
                "weight of " + (i+1)
                );
        }
	}


}
