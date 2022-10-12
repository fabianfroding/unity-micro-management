using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;

// NOTE: In the Unity project, we are dealing with a top-down 3D environment, therefore all y-values need to be translated as
// z-values. Hence, some variables have z-properties instead of y.

public class AIPathfindingDOTS : MonoBehaviour
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    private void Start()
    {
        FindPath(new(0, 0), new(3, 1));
    }

    private void FindPath(int2 startPos, int2 goalPos)
    {
        int2 gridSize = new(20, 20);

        NativeArray<PathNode> pathNodes = new(gridSize.x * gridSize.y, Allocator.Temp);

        // Initialize the nodes.
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int z = 0; z < gridSize.y; z++)
            {
                PathNode node = new()
                {
                    x = x,
                    z = z,
                    idx = CalculateIndex(x, z, gridSize.x),
                    gCost = int.MaxValue,
                    hCost = CalculateDistanceCost(new int2(x, z), goalPos),
                    walkable = true,
                    previousNodeIdx = -1 // Using -1 as invlid value.
                };
                node.CalculcateFCost();

                pathNodes[node.idx] = node;
            }
        }

        // Test walkability.
        {
            PathNode walkableNode = pathNodes[CalculateIndex(1, 0, gridSize.x)];
            walkableNode.SetWalkable(false);
            pathNodes[CalculateIndex(1, 0, gridSize.x)] = walkableNode;

            walkableNode = pathNodes[CalculateIndex(1, 1, gridSize.x)];
            walkableNode.SetWalkable(false);
            pathNodes[CalculateIndex(1, 1, gridSize.x)] = walkableNode;

            walkableNode = pathNodes[CalculateIndex(1, 2, gridSize.x)];
            walkableNode.SetWalkable(false);
            pathNodes[CalculateIndex(1, 2, gridSize.x)] = walkableNode;
        }
        

        PathNode startNode = pathNodes[CalculateIndex(startPos.x, startPos.y, gridSize.x)];
        startNode.gCost = 0;
        startNode.CalculcateFCost();
        pathNodes[startNode.idx] = startNode; // Insert the node into the array since value types generates copies.

        // Algorithm.
        NativeList<int> openList = new(Allocator.Temp);
        NativeList<int> closedList = new(Allocator.Temp);

        openList.Add(startNode.idx);
        int goalNodeIdx = CalculateIndex(goalPos.x, goalPos.y, gridSize.x);
        NativeArray<int2> neighbourOffsets = new(new int2[]
        {
            new int2(-1, 0),    // Left
            new int2(+1, 0),    // Right
            new int2(0, +1),    // Up
            new int2(0, -1),    // Down
            new int2(-1, -1),   // Left Down
            new int2(-1, +1),   // Left Up
            new int2(+1, -1),   // Right Down
            new int2(+1, +1)    // Right Up
        }, Allocator.Temp);

        while (openList.Length > 0)
        {
            int currentNodeIdx = GetIndexOfLowestFCostNode(openList, pathNodes);
            PathNode currentNode = pathNodes[currentNodeIdx];

            if (currentNodeIdx == goalNodeIdx) { break; } // Reached goal node.

            // Not yet at goal node.
            for (int i = 0; i < openList.Length; i++)
            {
                if (openList[i] == currentNodeIdx)
                {
                    openList.RemoveAtSwapBack(i);
                    break;
                }
            }
            closedList.Add(currentNodeIdx);

            // Iterative through neighbours of current node.
            for (int i = 0; i < neighbourOffsets.Length; i++)
            {
                int2 neighbourOffset = neighbourOffsets[i];
                int2 neighbourPos = new(currentNode.x + neighbourOffset.x, currentNode.z + neighbourOffset.y);

                // Check neighbour position.
                if (!IsPositionInsideGrid(neighbourPos, gridSize)) { continue; } // Invalid neighbour position.

                // Check if closedList contains this index.
                int neighbourIdx = CalculateIndex(neighbourPos.x, neighbourPos.y, gridSize.x);
                if (closedList.Contains(neighbourIdx)) { continue; } // Neighbour node already searched.

                // Check if walkable.
                PathNode neighbourNode = pathNodes[neighbourIdx];
                if (!neighbourNode.walkable) { continue; }

                // All above conditions met which means we found a valid neighbour node.
                int2 currentNodPos = new(currentNode.x, currentNode.z);
                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNodPos, neighbourPos);
                if (tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.previousNodeIdx = currentNodeIdx;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.CalculcateFCost();
                    pathNodes[neighbourIdx] = neighbourNode; // Place the updated node back in the array since we use structs.

                    if (!openList.Contains(neighbourNode.idx)) { openList.Add(neighbourNode.idx); }
                }
            }
        }

        PathNode goalNode = pathNodes[goalNodeIdx];
        if (goalNode.previousNodeIdx == -1) { Debug.Log("Didn't find a path."); }
        else
        {
            NativeList<int2> path = CalculatePath(pathNodes, goalNode);
            foreach (int2 pathPos in path)
            {
                Debug.Log(pathPos);
            }
            path.Dispose(); // TODO: Return.
        }

        // Cleanup.
        pathNodes.Dispose();
        openList.Dispose();
        closedList.Dispose();
        neighbourOffsets.Dispose();
    }

    private NativeList<int2> CalculatePath(NativeArray<PathNode> pathNodes, PathNode goalNode)
    {
        if (goalNode.previousNodeIdx == -1) { return new(Allocator.Temp); } // Return empty list of invalid goal node index.
        else
        {
            // Path found. Walk backwards to find the path.
            NativeList<int2> path = new(Allocator.Temp) { new(goalNode.x, goalNode.z) };
            PathNode currentNode = goalNode;

            while (currentNode.previousNodeIdx != -1)
            {
                PathNode prevNode = pathNodes[currentNode.previousNodeIdx];
                path.Add(new(prevNode.x, prevNode.z));
                currentNode = prevNode;
            }
            return path;
        }
    }

    private bool IsPositionInsideGrid(int2 gridPos, int2 gridSize) =>
        gridPos.x >= 0 && gridPos.y >= 0 && gridPos.x < gridSize.x && gridPos.y < gridSize.y;

    private int CalculateIndex(int x, int z, int gridWidth) => x + z * gridWidth;

    private int CalculateDistanceCost(int2 aPos, int2 bPos)
    {
        int xDistance = math.abs(aPos.x - bPos.x);
        int zDistance = math.abs(aPos.y - bPos.y);
        int remaining = math.abs(xDistance - zDistance);
        return MOVE_DIAGONAL_COST * math.min(xDistance, zDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private int GetIndexOfLowestFCostNode(NativeList<int> openList, NativeArray<PathNode> pathNodes)
    {
        PathNode lowestCostNode = pathNodes[openList[0]];
        for (int i = 0; i < openList.Length; i++)
        {
            PathNode testNode = pathNodes[openList[i]];
            if (testNode.fCost < lowestCostNode.fCost) { lowestCostNode = testNode; }
        }
        return lowestCostNode.idx;
    }

    private struct PathNode
    {
        public int x, z;
        public int idx;
        public int fCost, gCost, hCost; // f = g + h. g = Cost from start to this. h = Estimated cost from this to goal.
        public bool walkable;
        public int previousNodeIdx;

        public void CalculcateFCost() => fCost = gCost + hCost;

        public void SetWalkable(bool walkable) => this.walkable = walkable;
    }
}
