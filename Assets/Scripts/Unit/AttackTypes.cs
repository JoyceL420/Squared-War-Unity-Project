using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTypes : MonoBehaviour
{
    private GameObject TeamController;
    private TeamController _teamController; 
    private unit _unit;
    void Start()
    {
        _unit = GetComponent<unit>();
        TeamController = GameObject.Find("TeamsController");
        _teamController = TeamController.GetComponent<TeamController>();
    }
    public void FootSoldierAttack((float x, float y) coordinate)
    { // Foot soldier attacks means that the unit attacks the tiles
        // that it stands on.
        // This may apply to units that isn't a foot soldier e.g. cavalier
        _teamController.affectedSquares.Add((coordinate.x, coordinate.y, _unit.GetId()));
    }
}
