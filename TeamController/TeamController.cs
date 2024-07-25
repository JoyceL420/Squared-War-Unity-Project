using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

// This script is going to need serious optimizations when I make it functional

public class TeamController : MonoBehaviour
{
    private GameObject _unitPrefab;
    private List<GameObject> _blueClones = new List<GameObject>();
    private List<GameObject> _blueFootSoldiers = new List<GameObject>();
    private List<GameObject> _blueCavaliers = new List<GameObject>();
    private List<GameObject> _redClones = new List<GameObject>();
    private List<GameObject> _redFootSoldiers = new List<GameObject>();
    private List<GameObject> _redCavaliers = new List<GameObject>();
    private List<GameObject> _clones = new List<GameObject>();
    private List<GameObject> _unitsToMove = new List<GameObject>();
    public List<(float x, float y, int id, int team)> affectedSquares;
    public List <(float x, float y)> obstructedSquares;
    public List <(float x, float y)> occupiedSquares;
    private int _unitId;
    private bool _blueUnitIsAlive;
    private bool _redUnitIsAlive;
    private List <int> _unitsToRemove;
    private GameObject TurnCaller;
    private GameObject OverlapChecker;
    private OverlapChecker _overlapChecker;
    private int _moveThreshold;
    private TurnCaller _turnCaller;   
    private (int xMapSize, int yMapSize) _mapSize;
    // Awake runs before Start
    void Awake()
    {
        obstructedSquares = new List<(float x, float y)>();
    }
    // Start is called before the first frame update
    void Start()
    {
        _unitId = 1;
        affectedSquares = new List<(float x, float y, int id, int team)>();
        occupiedSquares = new List<(float x, float y)>();
        _unitsToRemove = new List<int>();
        _unitPrefab = GameObject.Find("UnitPrefab");
        TurnCaller = GameObject.Find("Main Camera/Game Manager");
        _turnCaller = TurnCaller.GetComponent<TurnCaller>();
        OverlapChecker = GameObject.Find("OverlapAfterMoveChecker");
        _overlapChecker = OverlapChecker.GetComponent<OverlapChecker>();
        _moveThreshold = 0;
        _blueUnitIsAlive = true;
        _redUnitIsAlive = true;

    }

    public void LevelLoadInitialize((int, int) MapSize)
    {
        _mapSize = MapSize;
    }
    public void Turn(int team)
    {
        // Initiate a coroutine for a whole team's movements
        StartCoroutine(TurnCoroutine(team));
    }

    private IEnumerator TurnCoroutine(int team)
    { // Set necessary variables and lists
        TurnPrep(team);
        for (int _timesMoved = 0; _timesMoved < _moveThreshold; _timesMoved++)
        {
            MoveUnits(_unitsToMove); // Move the units that still have movement left (remove themselves from the list when have no movement)
            CheckAllOverlaps(); // Check overlaps 
            yield return new WaitForSeconds(0.5f); // Delay between movements
        } // Turn end
        foreach (GameObject clone in _clones)
        {
            unit unit = clone.GetComponent<unit>();
            unit.MovementVariablesReset(); // Reset direction variables
        }
        Debug.Log("Turn finished");
        _turnCaller.FinishTurn();
    }
    private void TurnPrep(int team)
    {
        _unitsToMove.Clear(); // Clear the list at the start of each turn
        _moveThreshold = 0; // Reset move threshold

        switch (team)
        { // Determine move threshold from unit with highest movement
            case 0: // Blue team
                foreach (GameObject clone in _blueClones)
                {
                    _unitsToMove.Add(clone);
                    unit unit = clone.GetComponent<unit>();
                    if (unit.GetSpeed() > _moveThreshold)
                    {
                        _moveThreshold = unit.GetSpeed();
                    }
                }
                break;
            case 1: // Red team
                foreach (GameObject clone in _redClones)
                {
                    _unitsToMove.Add(clone);
                    unit unit = clone.GetComponent<unit>();
                    if (unit.GetSpeed() > _moveThreshold)
                    {
                        _moveThreshold = unit.GetSpeed();
                    }
                }
                break;
            default: // Error catcher
                Debug.LogError("Something went wrong (ERROR TurnPrep method TEAM INT OUTSIDE ACCEPTED RANGE)");
                break;
        }
    }

    private void MoveUnits(List<GameObject> _unitsToMove)
    { // Iterate by units to move backwards to avoid iteration errors 
        for (int i = _unitsToMove.Count - 1; i >= 0; i--)
        { // Assign clone object and unit script reference
            GameObject clone = _unitsToMove[i];
            unit unit = clone.GetComponent<unit>();
            unit.AddTimesMoved();
            unit.TurnCall(); // TurnCall method in unit script
            if (unit._timesMoved >= _moveThreshold)
            {
                _unitsToMove.Remove(clone); // Remove unit from list if it has moved enough
            }
        }
    }

    private void CheckAllOverlaps()
    {
        foreach (GameObject clone in _clones)
        {
            unit unit = clone.GetComponent<unit>();
            _overlapChecker.CheckAffectedSquares(unit, affectedSquares);
        }
        affectedSquares.Clear();
    }

    public void ResetPostions()
    { // Resets positions for all units by running the Reset() method
        foreach (GameObject clone in _clones)
        {
            unit unit = clone.GetComponent<unit>();
            if (unit != null)
            {
                unit.Reset();
            }
            else
            {
                Debug.LogError("FootSoldier component not found on unit: " + unit.name);
            }
        }
    }
    public void CloneFootSoldier(float xSpawnPoint, float ySpawnPoint, int team) // Foot soldier clone
    { // Clone and initialization of foot soldier as called by the player or LevelBuilder
        GameObject clone = Instantiate(_unitPrefab, new Vector2(xSpawnPoint, ySpawnPoint), Quaternion.identity);
        unit unit = clone.GetComponent<unit>();
        if (unit != null && team == 0)
        { // Blue Team
            _blueClones.Add(clone);
            _blueFootSoldiers.Add(clone);
            _clones.Add(clone);
            _unitId = _blueFootSoldiers.Count + 100;
            unit.Initialize(1, _unitId, team, new List<int> {3, 1, 5, 0}, (xSpawnPoint, ySpawnPoint), obstructedSquares, "Foot", 0, _mapSize.xMapSize);
        }
        else if (unit != null && team == 1)
        { // Red Team
            _redClones.Add(clone);
            _redFootSoldiers.Add(clone);
            _clones.Add(clone);
            _unitId = _redFootSoldiers.Count + 100;
            unit.Initialize(1, _unitId, team, new List<int> {7, 1, 5, 0}, (xSpawnPoint, ySpawnPoint), obstructedSquares, "Foot", 0, _mapSize.xMapSize);
        }
    }
    public void CloneCavalier(float xSpawnPoint, float ySpawnPoint, int team) // Foot soldier clone
    { // Clone and initialization of the cavalier as called by the player or LevelBuilder
        GameObject clone = Instantiate(_unitPrefab, new Vector2(xSpawnPoint, ySpawnPoint), Quaternion.identity);
        unit unit = clone.GetComponent<unit>();
        if (unit != null && team == 0)
        { // Blue Team
            _blueClones.Add(clone);
            _blueCavaliers.Add(clone);
            _clones.Add(clone);
            _unitId = _blueCavaliers.Count + 200;
            unit.Initialize(2, _unitId, team, new List<int> {3, 1, 5, 0}, (xSpawnPoint, ySpawnPoint), obstructedSquares, "Cavalier", 1, _mapSize.xMapSize);
        }
        else if (unit != null && team == 1)
        { // Red Team
            _redClones.Add(clone);
            _redCavaliers.Add(clone);
            _clones.Add(clone);
            _unitId = _redCavaliers.Count + 200;
            unit.Initialize(2, _unitId, team, new List<int> {7, 1, 5, 0}, (xSpawnPoint, ySpawnPoint), obstructedSquares, "Cavalier", 1, _mapSize.xMapSize);
        }
    }
    public void RemoveUnit(int unitId)
    { // Loops backwards to prevent errors from removing item during iteration
        for (int i = _blueClones.Count - 1; i >= 0; i--)
        {
            GameObject clone = _blueClones[i];
            unit unit = clone.GetComponent<unit>();
            // If the unit selectedto remove has the same id as the specified id AND the battle isnt ongoing.
            if (unit.GetId() == unitId && _turnCaller.PreparationStatus())
            { // Remove that unit from the game
                occupiedSquares.Remove((unit.xPos, unit.yPos));
                _blueClones.Remove(clone);
                _clones.Remove(clone);
                ResetIds(unit.GetUnitType(), clone);
                Destroy(clone);
            }
        }
    }
    private void Nuke()
    {
        // Empties every single list and removes all units
        // (Will be used when changing level)
    }
    private void ResetIds(int _unitType, GameObject unitToRemove)
    { // Only needs to be used EVER for the blue team :P
        Debug.Log("Reset Ids called");
        switch (_unitType)
        {
            case 0:
                // Foot soldiers
                _blueFootSoldiers.Remove(unitToRemove);
                _unitId = 0;
                foreach (GameObject clone in _blueFootSoldiers)
                {
                    unit unit = clone.GetComponent<unit>();
                    _unitId += 1;
                    unit?.SetId(100+_unitId);
                }
                break;
            case 1:
                // Cavaliers
                _blueCavaliers.Remove(unitToRemove);
                _unitId = 0;
                foreach (GameObject clone in _blueCavaliers)
                {
                    unit unit = clone.GetComponent<unit>();
                    _unitId += 1;
                    unit?.SetId(200+_unitId);
                }
                break;
            default:
                break;
        }
    }
    public bool CheckLoopStatus()
    {
        if (_blueClones.Count == 0 || _redClones.Count == 0)
        { // If for some reason either team has no units
            return false; // End turn loop
        }
        // Checks whether a team is still capable of fighting or not
        // If so the turn loop is flagged to continue
        // Otherwise it breaks and a winner for the battle will be declared
        _blueUnitIsAlive = false;
        _redUnitIsAlive = false;
        foreach (GameObject clone in _blueClones)
        {
            unit unit = clone.GetComponent<unit>();
            if (unit.UnitIsDead() == false && unit.cantMove == false)
            { // IF a blue unit is alive, and capable of moving
                _blueUnitIsAlive = true; // Set flag to continue loop
            }
        }
        foreach (GameObject clone in _redClones)
        {
            unit unit = clone.GetComponent<unit>();
            if (unit.UnitIsDead() == false && unit.cantMove == false)
            { // IF a red unit is alive, and capable of moving
                _redUnitIsAlive = true; // Set flag to continue loop
            }
        }
        if (_blueUnitIsAlive == false || _redUnitIsAlive == false)
        { // If either team has no usable units
            return false; // End turn loop
        }
        // Otherwise continue turn loop
        return true; // Default case
    }
}
