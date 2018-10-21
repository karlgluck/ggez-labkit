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
using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using UnityObject = UnityEngine.Object;

namespace GGEZ.Labkit
{
    
public class CellPriorityQueue
{
    private List<Cell> _cells = new List<Cell>();
    private List<Cell> _cellsNextFrame = new List<Cell>();

    private int _lastReturnedCellSequencer = int.MinValue;

    /// <summary>
    ///     Dequeues the next cell to process or returns false
    ///</summary>
    /// <remarks>
    ///     The same cell can be added multiple times. Because of this, the presence of
    ///     cells in the list is not enough to determine whether or not there are more cells to
    ///     process. This is why iteration can't (efficiently) use enumerator-style HasNext/Current.
    /// </remarks>
    public bool PopNext(out Cell next)
    {

        // Execution jumps back here if the cell about to be returned is the same that was already
        // returned on the last call to PopNext.
    FoundDuplicate:

        if (_cells.Count == 0)
        {
            // All cells have been processed, so reset the sequencer for next frame
            _lastReturnedCellSequencer = int.MinValue;

            // Swap in the list of cells that need to be updated next frame
            List<Cell> swap = _cells;
            _cells = _cellsNextFrame;
            _cellsNextFrame = swap;

            // Return nothing
            next = null;
            return false;
        }

        // Pull the cell out of the highest-priority slot in the binary heap
        next = _cells[0];

        // Swap the last cell in to fill the slot that was just vacated
        int last = _cells.Count - 1;
        _cells[0] = _cells[last];
        _cells.RemoveAt(last);
        --last;

        // Re-sort the heap
        int parent = 0;
        while (true)
        {

            int leftChild = parent * 2 + 1;
            if (leftChild > last)
                break;

            int rightChild = leftChild + 1;
            if (rightChild <= last && _cells[rightChild].Sequencer < _cells[leftChild].Sequencer)
                leftChild = rightChild;

            if (_cells[parent].Sequencer <= _cells[leftChild].Sequencer)
                break;

            Cell swap = _cells[parent];
            _cells[parent] = _cells[leftChild];
            _cells[leftChild] = swap;

            parent = leftChild;

        }

        // If we already returned this cell, try again
        if (_lastReturnedCellSequencer == next.Sequencer)
        {
            goto FoundDuplicate;
        }

        // Return a new cell for processing
        _lastReturnedCellSequencer = next.Sequencer;
        return true;
    }

    /// <summary>
    ///     Queues a cell with dirty input registers to be processed. It is safe to queue the same cell multiple times.
    /// </summary>
    public void Add(Cell cell)
    {
        Debug.Assert(cell != null);

        List<Cell> cells;
        
        // Check if a cell is trying to dirty itself so it gets processed next frame
        if (cell.Sequencer == _lastReturnedCellSequencer)
        {
            cells = _cellsNextFrame;
        }
        else
        {
            // Add to the current frame
            cells = _cells;

            // Due to the variable update phase and that every cell created has a higher
            // sequence value than all those that came before it, it is normally never
            // possible that a cell with a higher priority than the last returned value
            // is added. However, in debug mode, this is verified.
            Debug.Assert(cell.Sequencer > _lastReturnedCellSequencer);
        }

        // Add the new cell in the slot with lowest priority
        cells.Add(cell);

        // Bubble the child up the binary heap
        int childIndex = cells.Count - 1;
        while (childIndex > 0)
        {
            int parentIndex = (childIndex - 1) / 2;
            if (cells[childIndex].Sequencer >= cells[parentIndex].Sequencer)
                break;

            Cell swap = cells[childIndex];
            cells[childIndex] = cells[parentIndex];
            cells[parentIndex] = swap;

            childIndex = parentIndex;
        }
    }

}

}
