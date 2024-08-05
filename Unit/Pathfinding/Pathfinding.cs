using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public int GridHeight; // Limit for width
    public int GridWidth; // Limit for height
    public Vector2Int StartPoint; // (Unit's spawn point)
    public int EndPointX; // End goal (X axis as )
    public bool DiagonalIsAllowed; // Allow diagonal movement flag

    private List<Node> openList; // List of tiles that have yet to be evaulated
    private HashSet<Node> closedList; // List of evaulated tiles

    private int targetX;
    private int minY;
    private int maxY;
    private Node[,] nodes;

    public List<int> FindPath(Vector2Int start, Vector2Int mapSize, List<Vector2Int> obstacles)
    {
        // Sets limit for grid + non-walkable spaces
        InitializeGridAndVariables(obstacles, mapSize);

        // Setting of start and goal/end nodes
        Node startNode = nodes[start.x, start.y];
        List<Node> goalNodes = GetGoalNodes(targetX, minY, maxY);

        openList = new List<Node> { startNode };
        closedList = new HashSet<Node>();

        while (openList.Count > 0)
        {
            // Select the node with the current lowest cost
            Node currentNode = GetLowestFCostNode(openList);
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            // If the node selected is in goal nodes
            if (goalNodes.Contains(currentNode))
            {
                // Trace the path and end loop (returns to whatever movement script called this bungaloo)
                return RetracePath(startNode, currentNode);
            }

            // Otherwise get the neighbors of the current node
            foreach (Node neighbor in GetNeighbors(currentNode))
            {
                // If the neighbor has already been evaluated or is an obstacle
                if (!neighbor.IsWalkable || closedList.Contains(neighbor))
                {
                    // Reiterate
                    continue;
                }
                int newGCost = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (newGCost < neighbor.gCost || !openList.Contains(neighbor))
                {
                    neighbor.gCost = newGCost;
                    neighbor.hCost = GetDistanceFromTarget(neighbor, targetX, minY, maxY);
                    neighbor.parent = currentNode;

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }
        Debug.LogError("Pathfinding error: No path found");
        return null;
    }

    private void InitializeGridAndVariables(List<Vector2Int> obstacles, Vector2Int mapSize)
    {
        // Initialize variables
        targetX = mapSize.x;
        minY = 0;
        maxY = mapSize.y;
        GridWidth = mapSize.x + 1;
        GridHeight = mapSize.y + 1;

        // Initialize nodes (size of map)

        nodes = new Node[GridWidth, GridHeight];
        for (int x = -1; x < GridWidth; x++)
        {
            for (int y = -1; y < GridHeight; y++)
            { // Set node positions
                nodes[x, y] = new Node(new Vector2Int(x, y));
            }
        }
        // Set obstructed tiles with the not walkable flag
        foreach (var obstacle in obstacles)
        { // Node that shares the position of an obstructed square
            // Set IsWalkable to false
            // Debug.Log($"Processing obstacle at ({obstacle.x}, {obstacle.y})");
            if (obstacle.x >= 0 && obstacle.x < GridWidth && obstacle.y >= 0 && obstacle.y < GridHeight)
            {
                nodes[obstacle.x, obstacle.y].IsWalkable = false;
            }
            else
            {
                Debug.LogError($"Obstacle position ({obstacle.x}, {obstacle.y}) is out of bounds. GridWidth: {GridWidth}, GridHeight: {GridHeight}");
            }
        }
    }

    private List<Node> GetGoalNodes(int targetX, int minY, int maxY)
    { // Set nodes that are used as goals for the pathfinder to pathfind to
        List<Node> goalNodes = new List<Node>();
        for (int y = minY; y <= maxY; y++)
        {
            goalNodes.Add(nodes[targetX, y]);
        }
        return goalNodes;
    }
    
    private Node GetLowestFCostNode(List<Node> nodeList)
    { // Find the closest node with the lowest cost (fastest route)
        Node lowestFCostNode = nodeList[0];
        foreach (var node in nodeList)
        {
            if (node.fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = node;
            }
        }
        return lowestFCostNode;
    }

    private List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        // Horizontal/vertical directions (default)
        Vector2Int[] directions = 
        {
                new Vector2Int(1, 0), new Vector2Int(-1, 0),
                new Vector2Int(0, 1), new Vector2Int(0, -1),
        };

        // If the unit is able to move diagonally
        if (DiagonalIsAllowed)
        { // Take into account diagonal neighbors as well
            directions = new Vector2Int[] 
            {
                new Vector2Int(1, 0), new Vector2Int(-1, 0),
                new Vector2Int(0, 1), new Vector2Int(0, -1),
                new Vector2Int(1, 1), new Vector2Int(-1, -1),
                new Vector2Int(1, -1), new Vector2Int(-1, 1)
            };
        }

        foreach (var direction in directions)
        {
            Vector2Int neighborPos = node.Position + direction;
            if (IsWithinGrid(neighborPos))
            {
                neighbors.Add(nodes[neighborPos.x, neighborPos.y]);
            }
        }
        return neighbors;
    }

    private bool IsWithinGrid(Vector2Int position)
    { // Make sure that the space referenced is actually in the grid in the first place
        return position.x >= 0 && position.x < GridWidth && position.y >= 0 && position.y < GridHeight;
    }

    private int GetDistance(Node a, Node b)
    { // Get the distance to a selected node (Manhattan distance)
        int distanceX = Mathf.Abs(a.Position.x - b.Position.x);
        int distanceY = Mathf.Abs(a.Position.y - b.Position.y);
        return distanceX + distanceY;
    }

    private int GetDistanceFromTarget(Node node, int targetX, int minY, int maxY)
    { // Get the distance to target node(s)
        int distanceX = Mathf.Abs(node.Position.x - targetX);
        int distanceY = Mathf.Min(Mathf.Abs(node.Position.y - minY), Mathf.Abs(node.Position.y - maxY));
        return distanceX + distanceY;
    }

    private List<int> RetracePath(Node startNode, Node endNode)
    { // Make the final path
        List<int> path = new List <int>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            Node parentNode = currentNode.parent; // Parent node of current node
            if (parentNode == null) // If there is none
            {
                break; // That's literally the end of the path no more work needed :P
            }
            // Otherwise go backwards and get the direction from parentNode to the child node until there is no more ndoes left
            Vector2Int direction = currentNode.Position - parentNode.Position;
            path.Add(GetDirectionFromVector(direction));
        }
        path.Reverse();
        return path;
    }

    private int GetDirectionFromVector(Vector2Int direction)
    {
        if (direction == new Vector2Int(0, 1)) return 1; // Up
        if (direction == new Vector2Int(1, 1)) return 2; // Up-right
        if (direction == new Vector2Int(1, 0)) return 3; // Right
        if (direction == new Vector2Int(1, -1)) return 4; // Down-right
        if (direction == new Vector2Int(0, 1)) return 5; // Down
        if (direction == new Vector2Int(-1, -1)) return 6; // Down-left
        if (direction == new Vector2Int(-1, 0)) return 7; // Left
        if (direction == new Vector2Int(-1, 1)) return 8; // Up-left
        return 0; // Null
    }

}
