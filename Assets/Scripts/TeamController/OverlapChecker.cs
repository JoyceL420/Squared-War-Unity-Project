using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlapChecker : MonoBehaviour
{
    // Script responsible for the checking of overlap between friendly and enemy units
    // It checks units in sequence based on ID, the first unit to move gets priority
    public bool CheckAffectedSquares(unit unit, List<(float x, float y, int id)> affectedSquares)
    {
        foreach (var coordinate in affectedSquares)
        { // If the unit isnt dead, and wasn't the one that attacked the affected tile it shares a position with
            if (unit.isDead is false && coordinate.x == unit.xPos && coordinate.y == unit.yPos && coordinate.id != unit.GetId())
            { 
                unit.Kill(); // Kill unit
                return true;
            }
        }
        return false;
    }
}
