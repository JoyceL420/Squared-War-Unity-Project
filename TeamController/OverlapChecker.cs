using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlapChecker : MonoBehaviour
{
    // Script responsible for the checking of overlap between friendly and enemy units
    // It checks units in sequence based on ID, the first unit to move gets priority
    public void CheckAffectedSquares(unit unit, List<(Vector2Int coordinate, int id, int team)> affectedSquares)
    {
        foreach (var attackedTile in affectedSquares)
        { // If the unit isnt dead, and wasn't the one that attacked the affected tile it shares a position with
            if ((unit.isDead is false && attackedTile.coordinate.x == unit.UnitPosition.x && attackedTile.coordinate.y == unit.UnitPosition.y && attackedTile.id > unit.GetId()) || 
                (attackedTile.team != unit.GetTeam() && attackedTile.id == unit.GetId() && attackedTile.coordinate.y == unit.UnitPosition.y && attackedTile.coordinate.x == unit.UnitPosition.x))
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
