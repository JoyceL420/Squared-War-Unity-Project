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
        // This script is for as the name implies, types of attacks
        // E.g. archer attack could be setting Affected Tiles two tiles 
        // in front of the moved direction of the unit.
        // (Leaving these comments since I forgot what this script did and got spooked)
        _unit = GetComponent<unit>();
        TeamController = GameObject.Find("TeamsController");
        _teamController = TeamController.GetComponent<TeamController>();
    }
    public void FootSoldierAttack(Vector2Int coordinate)
    { // Foot soldier attacks means that the unit attacks the tiles
        // that it stands on.
        // This may apply to units that isn't a foot soldier e.g. cavalier
        _teamController.affectedSquares.Add((coordinate, _unit.GetId(), _unit.GetTeam()));
    }
    public void ArcherAttack(Vector2Int coordinate, int direction)
    { 
        // Attack a space ahead dependent on move direction
        _teamController.affectedSquares.Add((coordinate, _unit.GetId(), _unit.GetTeam()));
    }
    public void MageAttack(Vector2Int coordinate, int direction)
    { 
        // Attack sides dependent on move direction

        //TODO
        _teamController.affectedSquares.Add((coordinate, _unit.GetId(), _unit.GetTeam()));
    }
}
