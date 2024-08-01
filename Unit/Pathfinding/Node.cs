using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Vector2Int Position;
    public bool IsWalkable;
    public Node parent;
    public int gCost;
    public int hCost;
    public int fCost { get { return gCost + hCost; } }

    public Node(Vector2Int position)
    {
        this.Position = position;
        IsWalkable = true;
    }
}
