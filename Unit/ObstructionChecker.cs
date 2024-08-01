using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstructionChecker : MonoBehaviour
{
    // Start is called before the first frame update
    public bool CheckObstruction(List<Vector2Int> _obstructedSquares, (float x, float y) _squareToMoveTo)
    {
        foreach (Vector2Int coordinate in _obstructedSquares)
        {
            // Debug.Log("Checking X: " + coordinate.x + "Y: " + coordinate.y);
            // Go through every single instance of an obstructed square
            // IF coordiate moving to is equal to that of any return true
            // Otherwise return false (allow movement)
            if (coordinate.x == _squareToMoveTo.x && coordinate.y == _squareToMoveTo.y)
            {
                // Debug.Log("is obstructed");
                return true;
            }
        }
        // Debug.Log("is not obstructed");
        return false;
    }
}
