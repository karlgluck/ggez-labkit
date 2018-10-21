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

namespace GGEZ.Tests.Golem
{

    public class CellPriorityQueueTests
    {
        private class TestCell : Cell { }

        [Test]
        public void ReturnsCorrectOrder()
        {
            TestCell sourceCell = new TestCell();
            TestCell[] testCells = new TestCell[999];

            for (int i = 0; i < testCells.Length; ++i)
                testCells[i] = sourceCell.Clone(i) as TestCell;

            for (int frame = 0; frame < 3; ++frame)
            {

                testCells.Shuffle();
                CellPriorityQueue queue = new CellPriorityQueue();
                for (int i = 0; i < testCells.Length; ++i)
                    queue.Add(testCells[i]);

                Cell poppedCell = null;
                int expectedIndex = 0;

                while (queue.PopNext(out poppedCell))
                {
                    Assert.AreEqual(poppedCell.Sequencer, expectedIndex, "sequencer, frame " + frame);
                    ++expectedIndex;
                }

                Assert.AreEqual(expectedIndex, testCells.Length, "count, frame " + frame);
            }
        }

        [Test]
        public void HandlesInsertionDuringIteration()
        {
            TestCell sourceCell = new TestCell();
            TestCell[] testCells = new TestCell[999];
            for (int i = 0; i < testCells.Length; ++i)
            {
                testCells[i] = sourceCell.Clone(i) as TestCell;
            }

            for (int frame = 0; frame < 3; ++frame)
            {

                CellPriorityQueue queue = new CellPriorityQueue();
                queue.Add(testCells[0]);
                int nextCell = 1;

                Cell poppedCell = null;
                while (queue.PopNext(out poppedCell) && nextCell < testCells.Length)
                {
                    Assert.AreEqual(poppedCell.Sequencer + 1, nextCell);
                    queue.Add(testCells[nextCell]);
                    ++nextCell;
                }

            }
        }

        [Test]
        public void ReturnsEachCellOnlyOnce()
        {
            Cell poppedCell = null;

            TestCell sourceCell = new TestCell();
            TestCell[] testCells = new TestCell[999];
            for (int i = 0; i < testCells.Length; ++i)
            {
                testCells[i] = sourceCell.Clone(i) as TestCell;
            }
            testCells.Shuffle();

            CellPriorityQueue queue = new CellPriorityQueue();
            int[] times = new int[testCells.Length];
            for (int i = 0; i < testCells.Length; ++i)
            for (int j = 0; j < 10; ++j)
                queue.Add(testCells[i]);

            while (queue.PopNext(out poppedCell))
                ++times[poppedCell.Sequencer];
            
            for (int i = 0; i < testCells.Length; ++i)
                Assert.AreEqual(times[i], 1);

        }
    }
}
