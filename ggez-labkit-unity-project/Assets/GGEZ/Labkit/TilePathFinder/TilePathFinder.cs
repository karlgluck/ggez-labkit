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

// This code import is WIP
//   * Some parameters are constants that should be user-configurable
//   * It needs testing (now that I've changed a bunch of stuff)
//   * Some of the code can be inlined


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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;

namespace GGEZ
{
    //----------------------------------------------------------------------
    // This is a highly optimized pathfinder based on the Gamasutra
    // article "Toward More Realistic Pathfinding" by Marco Pinter.
    //
    // This pathfinder does not just compute paths between tiles. Each tile
    // is divided into some number of points (e.g. 8 or 16) and start/end
    // coordinates are specified in this coordinate system. Whether a tile
    // can be entered still depends on tile coordinates.
    //
    // The pathfinder assumes the object being pathed is 1 tile in size.
    // Paths are checked at each of the 4 corners + center to make sure
    // the object doesn't overlap blocking tiles as it moves along the path.
    //
    // The pathfinder will attempt to find the most natural path first, then
    // falls back to A*. After finding an A* path, it will smooth the result
    // to avoid zigzags.
    //
    // To use the TilePathFinder:
    /*

        int searchAreaInTiles = 32;
        var pathfinder = new TilePathFinder (searchAreaInTiles, searchAreaInTiles);
        pathfinder.canEnterTile = delegate (int tileX, int tileY) { return !((tileX % 3 == 0) && (tileY % 3 == 0)); }
        var path = pathfinder.FindSimplePath (new Vector2i (0, 1), new Vector2i (5, 5));
        foreach (Vector2i position in path)
            {
            Debug.LogFormat ("Position: {0}", position);
            }

    */
    //
    // Reference:
    // http://www.gamasutra.com/view/feature/131505/toward_more_realistic_pathfinding.php?page=2
    //----------------------------------------------------------------------
    public class TilePathFinder
    {
        private struct Node
        {
            // constant
            public int IndexOfNeighborN;
            public int IndexOfNeighborE;
            public int IndexOfNeighborS;
            public int IndexOfNeighborW;
            public Vector2i NodePosition;

            // variable
            public float costToArrive;
            public float costOfHeuristic;
            public int indexOfParentNode;
            public bool isOpen;
            public int indexOfPreviousOpen;
            public int indexOfNextOpen;
            public int depth;
        }

        private struct SmoothingNode
        {
            public Vector2i Position;
        }

        public int TileSize { get { return 16; } }
        private float StepCost { get { return 1; } }
        private float NoCornerWeight { get { return 0.77f; } }
        private float DistanceToLineToTargetWeight { get { return 0.392f; } }

        private Node[] _nodes;
        private Vector2i _positionOfNodesMatrixOrigin;
        private int _nodesWidth, _nodesHeight;

        private BitArray _nodeIsTouched;

        private const int kMaxPathLength = 64;

        private const int kSmoothingPathLength = kMaxPathLength * 2 + 3;

        // These are in reversed order since A* reports the final path by
        // navigating parent pointers from end back to the start.
        private SmoothingNode[] _smoothingNodes = new SmoothingNode[kSmoothingPathLength];
        private int _smoothingNodesLength;

        private Vector2i[] _finalPath = new Vector2i[kSmoothingPathLength];
        private int _finalPathLength;

        public delegate bool CanEnterTileCallback(int tileX, int tileY);
        public CanEnterTileCallback canEnterTile;


        public TilePathFinder(
                int searchAreaTileWidth,
                int searchAreaTileHeight
                )
        {
            var tileHalfWidth = searchAreaTileWidth / 2 + 1;
            var tileHalfHeight = searchAreaTileHeight / 2 + 1;
            _nodesWidth = tileHalfWidth * 2 + 1;
            _nodesHeight = tileHalfHeight * 2 + 1;
            int numberOfNodes = _nodesWidth * _nodesHeight;
            _nodes = new Node[numberOfNodes];
            for (int y = 0, index = 0; y < _nodesHeight; ++y)
            {
                for (int x = 0; x < _nodesWidth; ++x, ++index)
                {
                    _nodes[index].IndexOfNeighborN = (y <= 0) ? -1 : (index - _nodesWidth);
                    _nodes[index].IndexOfNeighborE = (x + 1 >= _nodesWidth) ? -1 : (index + 1);
                    _nodes[index].IndexOfNeighborS = (y + 1 >= _nodesHeight) ? -1 : (index + _nodesWidth);
                    _nodes[index].IndexOfNeighborW = (x <= 0) ? -1 : (index - 1);
                    _nodes[index].NodePosition.x = x;
                    _nodes[index].NodePosition.y = y;
                }
            }
            _nodeIsTouched = new BitArray(numberOfNodes);
        }


        private Vector2i GetTilePosition(Vector2i point)
        {
            return new Vector2i(point.x / this.TileSize, point.y / this.TileSize);
        }

        private Vector2i GetTileCenter(Vector2i tile)
        {
            return new Vector2i(tile.x * this.TileSize + this.TileSize / 2, tile.y * this.TileSize + this.TileSize / 2);
        }

        private Vector2i GetTilePositionAtOffset(Vector2i point, int dx, int dy)
        {
            return new Vector2i((point.x + dx) / this.TileSize, (point.y + dy) / this.TileSize);
        }



        private class FinalPathReversedEnumerator : IEnumerator<Vector2i>
        {
            internal Vector2i[] path;
            internal int pathLength;
            private int _iteratorPosition;

            public FinalPathReversedEnumerator(Vector2i[] finalPath, int finalPathLength)
            {
                this.path = finalPath;
                this.pathLength = finalPathLength;
                this.Reset();
            }

            public Vector2i Current
            {
                get
                {
                    return this.path[_iteratorPosition];
                }
            }

            object System.Collections.IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public bool MoveNext()
            {
                return --_iteratorPosition >= 0;
            }

            public void Reset()
            {
                _iteratorPosition = this.pathLength;
            }
        }


        public IEnumerator<Vector2i> FindPath(Vector2i start, Vector2i end)
        {
            this.planTilePath(start, end);

            bool couldNotFindPath = _smoothingNodesLength == 0;
            if (couldNotFindPath)
            {
                return null;
            }

            _smoothingNodes[_smoothingNodesLength++].Position = start;
            _smoothingNodes[0].Position = end;
            const bool doFixTerminalPositionApproaches = true;
            if (doFixTerminalPositionApproaches)
            {
                {
                    var secondApproach = _smoothingNodes[2].Position.DeltaTo(_smoothingNodes[1].Position);
                    if (secondApproach.x != 0)
                    {
                        _smoothingNodes[1].Position.x = _smoothingNodes[0].Position.x;
                        _smoothingNodes[1].Position.y = _smoothingNodes[2].Position.y;
                    }
                    else
                    {
                        _smoothingNodes[1].Position.y = _smoothingNodes[0].Position.y;
                        _smoothingNodes[1].Position.x = _smoothingNodes[2].Position.x;
                    }
                }

                {
                    int lastSmoothingNodeIndex = _smoothingNodesLength - 1;
                    var secondApproach = _smoothingNodes[lastSmoothingNodeIndex - 2].Position.DeltaTo(_smoothingNodes[lastSmoothingNodeIndex - 1].Position);
                    if (secondApproach.y != 0)
                    {
                        _smoothingNodes[lastSmoothingNodeIndex - 1].Position.y = _smoothingNodes[lastSmoothingNodeIndex - 0].Position.y;
                        _smoothingNodes[lastSmoothingNodeIndex - 1].Position.x = _smoothingNodes[lastSmoothingNodeIndex - 2].Position.x;
                    }
                    else
                    {
                        _smoothingNodes[lastSmoothingNodeIndex - 1].Position.x = _smoothingNodes[lastSmoothingNodeIndex - 0].Position.x;
                        _smoothingNodes[lastSmoothingNodeIndex - 1].Position.y = _smoothingNodes[lastSmoothingNodeIndex - 2].Position.y;
                    }
                }
            }


            for (int i = 0; i < _smoothingNodesLength; ++i)
            {
                _finalPath[i] = _smoothingNodes[i].Position;
            }
            _finalPathLength = _smoothingNodesLength;

            const bool doFinalForwardLookingClean = true;
            if (doFinalForwardLookingClean)
            {
                int indexOfWrite = 1;
                int i = 0;
                while (i + 1 < _finalPathLength)
                {
                    var anchor = _finalPath[i];
                    int furthestIndexOnCleanPath = i + 1;
                    Debug.Assert(indexOfWrite <= furthestIndexOnCleanPath);
                    Debug.Assert(this.isPathNavigable(anchor, _finalPath[i + 1]));

                    for (int j = i + 2; j < _finalPathLength; ++j)
                    {
                        var current = _finalPath[j];
                        if (!this.isPathNavigable(anchor, current))
                        {
                            continue;
                        }
                        furthestIndexOnCleanPath = j;
                    }
                    Debug.Assert(indexOfWrite <= furthestIndexOnCleanPath);
                    _finalPath[indexOfWrite++] = _finalPath[furthestIndexOnCleanPath];
                    i = furthestIndexOnCleanPath;
                }
                _finalPathLength = indexOfWrite;
            }

            return new FinalPathReversedEnumerator(_finalPath, _finalPathLength);
        }



        private class FinalPathForwardEnumerator : IEnumerator<Vector2i>
        {
            internal Vector2i[] path;
            internal int pathLength;
            private int _iteratorPosition;

            public FinalPathForwardEnumerator(Vector2i[] finalPath, int finalPathLength)
            {
                this.path = finalPath;
                this.pathLength = finalPathLength;
                this.Reset();
            }

            public Vector2i Current
            {
                get
                {
                    return this.path[_iteratorPosition];
                }
            }

            object System.Collections.IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public bool MoveNext()
            {
                return ++_iteratorPosition < this.pathLength;
            }

            public void Reset()
            {
                _iteratorPosition = -1;
            }
        }

        public IEnumerator<Vector2i> FindSimplePath(Vector2i start, Vector2i end)
        {
            const bool attemptLinearPath = true;
            if (attemptLinearPath)
            {
                if (this.isPathNavigable(start, end))
                {
                    return new List<Vector2i>() { start, end }.GetEnumerator();
                }
            }

            const bool attemptSimplePathStartingWithX = true;
            if (attemptSimplePathStartingWithX)
            {
                var retval = this.planSimplePathStartingWithX(start, end);
                if (retval != null)
                {
                    return retval;
                }
            }

            const bool attemptSimplePathStartingWithY = true;
            if (attemptSimplePathStartingWithY)
            {
                var retval = this.planSimplePathStartingWithY(start, end);
                if (retval != null)
                {
                    return retval;
                }
            }

            return this.FindPath(start, end);
        }


        private IEnumerator<Vector2i> planSimplePathStartingWithX(Vector2i start, Vector2i end)
        {
            int index = 0, steps;

            int x = start.x;
            int dx = (end.x < start.x) ? -this.TileSize : +this.TileSize;
            steps = (end.x - start.x) / dx;
            while (steps >= 0)
            {
                _finalPath[index].x = x;
                _finalPath[index].y = start.y;
                ++index;
                x += dx;
                --steps;
            }
            _finalPath[index].x = end.x;
            _finalPath[index].y = start.y;
            ++index;

            int y = start.y;
            int dy = (end.y < start.y) ? -this.TileSize : +this.TileSize;
            while (steps >= 0)
            {
                _finalPath[index].x = end.x;
                _finalPath[index].y = y;
                ++index;
                y += dy;
                --steps;
            }
            _finalPath[index].x = end.x;
            _finalPath[index].y = end.y;
            ++index;

            _finalPathLength = index;
            return this.planSimplePath_cleanAnglesAndValidate(end);
        }




        private IEnumerator<Vector2i> planSimplePathStartingWithY(Vector2i start, Vector2i end)
        {
            int index = 0, steps;

            int y = start.y;
            int dy = (end.y < start.y) ? -this.TileSize : +this.TileSize;
            steps = (end.y - start.y) / dy;
            while (steps >= 0)
            {
                _finalPath[index].x = start.x;
                _finalPath[index].y = y;
                ++index;
                y += dy;
                --steps;
            }
            _finalPath[index].x = start.x;
            _finalPath[index].y = end.y;
            ++index;

            int x = start.x;
            int dx = (end.x < start.x) ? -this.TileSize : +this.TileSize;
            steps = (end.x - start.x) / dx;
            while (steps >= 0)
            {
                _finalPath[index].x = x;
                _finalPath[index].y = end.y;
                ++index;
                x += dx;
                --steps;
            }
            _finalPath[index].x = end.x;
            _finalPath[index].y = end.y;
            ++index;

            _finalPathLength = index;
            return this.planSimplePath_cleanAnglesAndValidate(end);
        }




        private IEnumerator<Vector2i> planSimplePath_cleanAnglesAndValidate(Vector2i end)
        {
            int indexOfWrite = 1;
            int i = 0;
            while (i + 1 < _finalPathLength)
            {
                var segmentStart = _finalPath[i];
                int furthestIndexOnCleanPath = -1;

                for (int j = i + 1; j < _finalPathLength; ++j)
                {
                    var current = _finalPath[j];
                    if (!this.isPathNavigable(segmentStart, current))
                    {
                        continue;
                    }
                    furthestIndexOnCleanPath = j;
                }

                if (furthestIndexOnCleanPath < 0)
                {
                    break;
                }

                Debug.Assert(indexOfWrite <= furthestIndexOnCleanPath);
                _finalPath[indexOfWrite++] = _finalPath[furthestIndexOnCleanPath];
                i = furthestIndexOnCleanPath;
            }
            _finalPathLength = indexOfWrite;

            var pathEnd = _finalPath[_finalPathLength - 1];
            var distanceXFirst = pathEnd.DistanceManhattan(end);
            if (distanceXFirst == 0)
            {
                return new FinalPathForwardEnumerator(_finalPath, _finalPathLength);
            }
            else
            {
                return null;
            }
        }









        private bool isPathNavigable(Vector2i start, Vector2i end)
        {
            var step = Vector2i.FindPointsBetween(start, end, this.TileSize / 4);
            while (step.MoveNext())
            {
                if (!this.isPositionNavigable(step.Current))
                {
                    return false;
                }
            }
            return true;
        }






        private bool isPositionNavigable(Vector2i currentPoint)
        {
            var tile = this.GetTilePosition(currentPoint);
            if (!this.canEnterTile(tile.x, tile.y))
            {
                return false;
            }
            tile = this.GetTilePositionAtOffset(currentPoint, -this.TileSize / 2 + 1, 0);
            if (!this.canEnterTile(tile.x, tile.y))
            {
                return false;
            }
            tile = this.GetTilePositionAtOffset(currentPoint, this.TileSize / 2 - 1, 0);
            if (!this.canEnterTile(tile.x, tile.y))
            {
                return false;
            }
            tile = this.GetTilePositionAtOffset(currentPoint, 0, -this.TileSize / 2 + 1);
            if (!this.canEnterTile(tile.x, tile.y))
            {
                return false;
            }
            tile = this.GetTilePositionAtOffset(currentPoint, 0, this.TileSize / 2 - 1);
            if (!this.canEnterTile(tile.x, tile.y))
            {
                return false;
            }
            return true;
        }




        private void planTilePath(Vector2i start, Vector2i end)
        {
            var centerNode = this.GetTilePosition(Vector2i.Midpoint(start, end));
            _positionOfNodesMatrixOrigin = new Vector2i() { x = centerNode.x - (_nodesWidth / 2), y = centerNode.y - (_nodesHeight / 2) };

            var startNode = _positionOfNodesMatrixOrigin.DeltaTo(this.GetTilePosition(start));
            Debug.Assert(startNode.x >= 0 && startNode.x < _nodesWidth);
            Debug.Assert(startNode.y >= 0 && startNode.y < _nodesHeight);
            Debug.Assert(this.tilePositionFromNodePosition(startNode).Equals(this.GetTilePosition(start)));

            var endNode = _positionOfNodesMatrixOrigin.DeltaTo(this.GetTilePosition(end));
            Debug.Assert(endNode.x >= 0 && endNode.x < _nodesWidth);
            Debug.Assert(endNode.y >= 0 && endNode.y < _nodesHeight);

            Debug.Assert(this.tilePositionFromNodePosition(endNode).Equals(this.GetTilePosition(end)));

            if (!this.isNodeNavigable(endNode))
            {
                _smoothingNodesLength = 0;
                return;
            }

            _nodeIsTouched.SetAll(false);

            int indexOfOpenHead = this.getIndexOfNode(startNode);
            _nodes[indexOfOpenHead].indexOfNextOpen = -1;
            _nodes[indexOfOpenHead].indexOfPreviousOpen = -1;
            _nodes[indexOfOpenHead].isOpen = true;
            _nodes[indexOfOpenHead].costToArrive = 0;
            _nodes[indexOfOpenHead].depth = 0;
            _nodes[indexOfOpenHead].indexOfParentNode = -1;
            _nodeIsTouched[indexOfOpenHead] = true;

            int indexOfEnd = this.getIndexOfNode(endNode);
            _nodes[indexOfEnd].indexOfParentNode = -1;

            int maxDepth = 0;
            while (maxDepth < kMaxPathLength && indexOfOpenHead >= 0)
            {
                int indexOfCurrent = this.findIndexOfLowestCostOpenNode(indexOfOpenHead);
                if (indexOfCurrent == indexOfEnd)
                {
                    break;
                }

                this.removeFromOpenListAndAddToClosedList(indexOfCurrent, ref indexOfOpenHead);

                int indexOfParentOfCurrent = _nodes[indexOfCurrent].indexOfParentNode;
                bool currentHasParent = indexOfParentOfCurrent >= 0;
                var positionOfCurrent = _nodes[indexOfCurrent].NodePosition;
                Vector2i deltaToArriveAtCurrent = default(Vector2i);
                if (currentHasParent)
                {
                    deltaToArriveAtCurrent = _nodes[indexOfParentOfCurrent].NodePosition.DeltaTo(positionOfCurrent);
                }

                for (int neighbor = 0; neighbor < 4; ++neighbor)
                {
                    int indexOfNeighbor;
                    switch (neighbor)
                    {
                        default:
                        case 3: indexOfNeighbor = _nodes[indexOfCurrent].IndexOfNeighborN; break;
                        case 1: indexOfNeighbor = _nodes[indexOfCurrent].IndexOfNeighborE; break;
                        case 2: indexOfNeighbor = _nodes[indexOfCurrent].IndexOfNeighborS; break;
                        case 0: indexOfNeighbor = _nodes[indexOfCurrent].IndexOfNeighborW; break;
                    }


                    bool neighborIsOutsideBoundaries = indexOfNeighbor < 0;
                    if (neighborIsOutsideBoundaries || !this.isNodeNavigable(_nodes[indexOfNeighbor].NodePosition))
                    {
                        continue;
                    }

                    var costToArriveAtNeighbor = _nodes[indexOfCurrent].costToArrive + this.StepCost;

                    bool neighborWasTouched = _nodeIsTouched[indexOfNeighbor];
                    bool attemptPathThroughNeighbor = !neighborWasTouched;
                    bool neighborWasClosed = true;
                    if (neighborWasTouched && costToArriveAtNeighbor < _nodes[indexOfNeighbor].costToArrive)
                    {
                        attemptPathThroughNeighbor = true;
                        neighborWasClosed = !_nodes[indexOfNeighbor].isOpen;
                    }

                    if (attemptPathThroughNeighbor)
                    {
                        _nodes[indexOfNeighbor].costToArrive = costToArriveAtNeighbor;

                        {
                            var positionOfNeighbor = _nodes[indexOfNeighbor].NodePosition;
                            _nodeIsTouched[indexOfNeighbor] = true;

                            bool continuesStraightFromLastNode = false;
                            if (currentHasParent)
                            {
                                var deltaToArriveAtNeighbor = positionOfCurrent.DeltaTo(positionOfNeighbor);
                                if (deltaToArriveAtCurrent.Equals(deltaToArriveAtNeighbor))
                                {
                                    continuesStraightFromLastNode = true;
                                }
                            }

                            var distanceCost = positionOfNeighbor.Distance(endNode);
                            var distanceToLineToTargetCost = positionOfNeighbor.GetDistanceToLineSegment(startNode, endNode);
                            var heuristic = (continuesStraightFromLastNode ? NoCornerWeight : 1.0f) * distanceCost
                                    + DistanceToLineToTargetWeight * distanceToLineToTargetCost;
                            _nodes[indexOfNeighbor].costOfHeuristic = heuristic;
                        }

                        {
                            _nodes[indexOfNeighbor].indexOfParentNode = indexOfCurrent;
                            int depth = _nodes[indexOfCurrent].depth + 1;
                            _nodes[indexOfNeighbor].depth = depth;
                            maxDepth = Mathf.Max(maxDepth, depth);
                        }

                        if (neighborWasClosed)
                        {
                            _nodes[indexOfNeighbor].isOpen = true;
                            _nodes[indexOfNeighbor].indexOfNextOpen = indexOfOpenHead;
                            _nodes[indexOfNeighbor].indexOfPreviousOpen = -1;
                            if (indexOfOpenHead >= 0)
                            {
                                _nodes[indexOfOpenHead].indexOfPreviousOpen = indexOfNeighbor;
                            }
                            indexOfOpenHead = indexOfNeighbor;
                        }
                        else
                        {
                            Debug.Assert(neighborWasTouched);
                        }
                    }
                }
            }

            // Start reporting the path at 1 because smoothingNodes[0] will be set to the actual start
            // position vs. the starting tile.
            int pathIndex = 1;
            int indexOfPointer = indexOfEnd;
            while (indexOfPointer >= 0)
            {
                var segmentStart = this.GetTileCenter(
                        this.tilePositionFromNodePosition(
                                _nodes[indexOfPointer].NodePosition
                                )
                        );
                _smoothingNodes[pathIndex++].Position = segmentStart;
                indexOfPointer = _nodes[indexOfPointer].indexOfParentNode;
            }
            _smoothingNodesLength = pathIndex;
        }





        private void removeFromOpenListAndAddToClosedList(int index, ref int indexOfOpenHead)
        {
            Debug.Assert(_nodeIsTouched[index] && _nodes[index].isOpen);
            int indexOfPreviousOpen = _nodes[index].indexOfPreviousOpen;
            int indexOfNextOpen = _nodes[index].indexOfNextOpen;
            if (indexOfPreviousOpen >= 0)
            {
                _nodes[indexOfPreviousOpen].indexOfNextOpen = indexOfNextOpen;
            }
            if (indexOfNextOpen >= 0)
            {
                _nodes[indexOfNextOpen].indexOfPreviousOpen = indexOfPreviousOpen;
            }
            if (index == indexOfOpenHead)
            {
                indexOfOpenHead = indexOfNextOpen;
                Debug.Assert(indexOfPreviousOpen < 0);
            }
            _nodes[index].isOpen = false;
        }




        private int findIndexOfLowestCostOpenNode(int indexOfOpenListHead)
        {
            var retval = indexOfOpenListHead;
            float lowestCost = _nodes[indexOfOpenListHead].costOfHeuristic;
            int sentinel = _nodes.Length;
            var indexOfCandidate = retval;
            while (sentinel-- >= 0)
            {
                indexOfCandidate = _nodes[indexOfCandidate].indexOfNextOpen;
                if (indexOfCandidate < 0)
                {
                    break;
                }
                float cost = _nodes[indexOfCandidate].costOfHeuristic;
                if (cost < lowestCost)
                {
                    retval = indexOfCandidate;
                    lowestCost = cost;
                }
            }
            return retval;
        }




        private bool isNodeNavigable(Vector2i nodePosition)
        {
            var actualPosition = this.tilePositionFromNodePosition(nodePosition);
            bool navigable = this.canEnterTile(actualPosition.x, actualPosition.y);
            return navigable;
        }




        private Vector2i tilePositionFromNodePosition(Vector2i nodePosition)
        {
            return new Vector2i()
            {
                x = nodePosition.x + _positionOfNodesMatrixOrigin.x,
                y = nodePosition.y + _positionOfNodesMatrixOrigin.y
            };
        }




        private int getIndexOfNode(Vector2i nodePosition)
        {
            return nodePosition.y * _nodesWidth + nodePosition.x;
        }
    }
}
