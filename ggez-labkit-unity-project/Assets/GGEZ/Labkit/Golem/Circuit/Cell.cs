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
using System.Collections.Generic;
using GGEZ.FullSerializer;

namespace GGEZ.Labkit
{

    //-------------------------------------------------------------------------
    // Cell
    //-------------------------------------------------------------------------
    public class Cell : IEquatable<Cell>
    {

        [System.Obsolete]
        public virtual void Acquire(Golem golem, ref bool running) { }

        [System.Obsolete]
        public virtual void Update(Golem golem, bool dirty, ref bool running) { }


        // This is a globally unique sequencing value used to organize cells into a priority queue
        // It is in the order of cells in a single golem, and unique between golems
        /// <remarks>Could use the high bits of this sequencer to distinguish
        /// cells of different golems so that they can be parallelized.</remarks>
        public int Sequencer { get; private set; }

        public virtual void Acquire()
        { }

        public virtual void Update()
        { }

        public Cell Clone(int sequencer)
        {
            Cell cell = MemberwiseClone() as Cell;
            cell.Sequencer = sequencer;
            return cell;
        }

        private static int nextSequencer = 1;
        public Cell Clone()
        {
            Cell cell = MemberwiseClone() as Cell;
            cell.Sequencer = nextSequencer++;
            return cell;
        }

        public bool Equals(Cell other)
        {
            return object.ReferenceEquals(this, other);
        }
    }

}
