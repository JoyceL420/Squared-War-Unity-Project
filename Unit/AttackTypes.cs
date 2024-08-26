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
    { 
        // Sample comment
        _teamController.affectedSquares.Add((coordinate, _unit.GetId(), _unit.GetTeam()));
    }
    public void ArcherAttack(Vector2Int coordinate, int direction)
    { 
        // Attack a space ahead dependent on move direction
        _teamController.affectedSquares.Add((coordinate, _unit.GetId(), _unit.GetTeam()));
        _teamController.affectedSquares.Add((coordinate + GetVectorFromDirectionArcher(direction), _unit.GetId(), _unit.GetTeam()));
    }
    public void MageAttack(Vector2Int coordinate, int direction)
    { 
        // Attack sides dependent on move direction
        _teamController.affectedSquares.Add((coordinate, _unit.GetId(), _unit.GetTeam()));
        _teamController.affectedSquares.Add((coordinate + GetVectorFromDirectionMage(direction), _unit.GetId(), _unit.GetTeam()));
        _teamController.affectedSquares.Add((coordinate - GetVectorFromDirectionMage(direction), _unit.GetId(), _unit.GetTeam()));
    }

    private Vector2Int GetVectorFromDirectionArcher(int direction)
    {
        if (direction == 1) return new Vector2Int(0, 1); // Moved Up
        if (direction == 3) return new Vector2Int(1, 0); // Moved Right
        if (direction == 5) return new Vector2Int(0, -1); // Moved Down
        if (direction == 7) return new Vector2Int(-1, 0); // Moved Left
        return new Vector2Int(0, 0); // Null
    }

    private Vector2Int GetVectorFromDirectionMage(int direction)
    {
        if (direction == 1) return new Vector2Int(1, 0); // Moved Up
        if (direction == 3) return new Vector2Int(0, 1); // Moved Right
        if (direction == 5) return new Vector2Int(1, 0); // Moved Down
        if (direction == 7) return new Vector2Int(0, 1); // Moved Left
        return new Vector2Int(0, 0); // Null
    }
}
