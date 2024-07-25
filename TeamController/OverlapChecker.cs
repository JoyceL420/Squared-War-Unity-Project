using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlapChecker : MonoBehaviour
{
    // Script responsible for the checking of overlap between friendly and enemy units
    // It checks units in sequence based on ID, the first unit to move gets priority
    public void CheckAffectedSquares(unit unit, List<(float x, float y, int id, int team)> affectedSquares)
    {
        foreach (var coordinate in affectedSquares)
        { // If the unit isnt dead, and wasn't the one that attacked the affected tile it shares a position with
            if ((unit.isDead is false && coordinate.x == unit.xPos && coordinate.y == unit.yPos && coordinate.id > unit.GetId()) || 
                (coordinate.team != unit.GetTeam() && coordinate.id == unit.GetId() && coordinate.y == unit.yPos && coordinate.x == unit.xPos))
            { 
                // IF the following conditions are met
                // X and Y of the unit and attacked tile are the same
                // The ID is not greater. 
                // Is not dead (probably redundant)
                // OR
                // The same conditions +
                // Unit being checked is in a different team to the team that attacked
                unit.Kill(); // Kill unit
            }
        }
    }
}
