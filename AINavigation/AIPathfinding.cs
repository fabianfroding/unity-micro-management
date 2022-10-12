using System.Collections.Generic;
using UnityEngine;

public class AIPathfinding
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    private AIGrid<AIPathNode> grid;
    private List<AIPathNode> openList;
    private List<AIPathNode> closedList;

    public static float CellSize { get; private set; }

    public AIPathfinding(int width, int height, float cellSize)
    {
        CellSize = cellSize;
        grid = new AIGrid<AIPathNode>(width, height, CellSize, Vector3.zero, 
            (AIGrid<AIPathNode> g, int x, int z, Vector3 loc) => new AIPathNode(x, z, loc));
    }

    public AIGrid<AIPathNode> GetAIGrid() => grid;

    public List<AIPathNode> FindPath(int startX, int startZ, int endX, int endZ)
    {
        AIPathNode startNode = grid.GetAIGridObject(startX, startZ);
        AIPathNode goalNode = grid.GetAIGridObject(endX, endZ);

        openList = new List<AIPathNode>() { startNode };
        closedList = new List<AIPathNode>();

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int z = 0; z < grid.GetHeight(); z++)
            {
                AIPathNode pathNode = grid.GetAIGridObject(x, z);
                pathNode.gCost = int.MaxValue;
                pathNode.CalculateFCost();
                pathNode.previousNode = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, goalNode);
        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            AIPathNode currentNode = GetLowestFCostNode(openList);
            if (currentNode == goalNode)
            {
                return CalculatePath(goalNode);
            }
            openList.Remove(currentNode); // Current node has already been searched.
            closedList.Add(currentNode);

            // Iterate through the neightbours of the current node.
            foreach (AIPathNode neighbourNode in GetNeighbourList(currentNode))
            {
                // Check if neighbour node is already in the closed list. (If so, we have already searched it, then continue).
                if (closedList.Contains(neighbourNode)) { continue; }
                if (!neighbourNode.walkable)
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                // If neighbour node is not in closed list, we need to search it.
                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);

                // Check if tentativeGCOst is lower than the gCost stored in the neighbour node.
                // We want to see if we have a faster path from the current node to the neighbour node than we had previously.
                if (tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.previousNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, goalNode);
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode)) { openList.Add(neighbourNode); }
                }
            }
        }

        // Out of nodes on the open list. (We searched through the whole map and we could not find a path).
        return null;

    }

    private List<AIPathNode> GetNeighbourList(AIPathNode node)
    {
        List<AIPathNode> neighbours = new();

        // Check left column
        if (node.x - 1 >= 0)
        {
            neighbours.Add(GetNode(node.x - 1, node.z));
            if (node.z - 1 >= 0)                { neighbours.Add(GetNode(node.x - 1, node.z - 1)); }
            if (node.z + 1 < grid.GetHeight())  { neighbours.Add(GetNode(node.x - 1, node.z + 1)); }
        }
        // Check right column
        if (node.x + 1 < grid.GetWidth())
        {
            neighbours.Add(GetNode(node.x + 1, node.z));
            if (node.z - 1 >= 0)                { neighbours.Add(GetNode(node.x + 1, node.z - 1)); }
            if (node.z + 1 < grid.GetHeight())  { neighbours.Add(GetNode(node.x + 1, node.z + 1)); }
        }
        // Up
        if (node.z - 1 >= 0)                    { neighbours.Add(GetNode(node.x, node.z - 1)); }
        // Down
        if (node.z + 1 < grid.GetHeight())      { neighbours.Add(GetNode(node.x, node.z + 1)); }

        return neighbours;
    }

    public AIPathNode GetNode(int x, int z) => grid.GetAIGridObject(x, z);

    private List<AIPathNode> CalculatePath(AIPathNode goalNode)
    {
        List<AIPathNode> path = new() { goalNode }; // Same as: new List<AIPathNode(); path.Add(goalNode);
        AIPathNode currentNode = goalNode;

        // Iterate through parent until we reach one that does not have a parent.
        while (currentNode.previousNode != null)
        {
            path.Add(currentNode.previousNode);
            currentNode = currentNode.previousNode;
        }
        path.Reverse();
        return path;
    }

    private int CalculateDistanceCost(AIPathNode a, AIPathNode b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int zDistance = Mathf.Abs(a.z - b.z);
        int remaining = Mathf.Abs(xDistance - zDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, zDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private AIPathNode GetLowestFCostNode(List<AIPathNode> pathNodeList)
    {
        AIPathNode lowestFCostNode = pathNodeList[0];
        for (int i = 0; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = pathNodeList[i];
            }
        }
        return lowestFCostNode;
    }
}
