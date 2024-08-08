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

    // Override equals check to ensure nodes are compared by position
    public override bool Equals(object obj)
    {
        if (obj == null || !(obj is Node))
        {
            return false;
        }

        Node other = (Node) obj;
        return this.Position.Equals(other.Position);
    }
    // Objects that are 'equal' have the same hash code
    public override int GetHashCode()
    {
        return Position.GetHashCode();
    }
}
