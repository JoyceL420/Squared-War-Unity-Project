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
    private List<GameObject> _blueRogues = new List<GameObject>();
    private List<GameObject> _blueArchers = new List<GameObject>();
    private List<GameObject> _blueMages = new List<GameObject>();
    private List<GameObject> _redClones = new List<GameObject>();
    private List<GameObject> _redFootSoldiers = new List<GameObject>();
    private List<GameObject> _redCavaliers = new List<GameObject>();
    private List<GameObject> _redRogues = new List<GameObject>();
    private List<GameObject> _redArchers = new List<GameObject>();
    private List<GameObject> _redMages = new List<GameObject>();
    private List<GameObject> _clones = new List<GameObject>();
    private List<GameObject> _unitsToMove = new List<GameObject>();
    public List<(Vector2Int coordinate, int id, int team)> affectedSquares;
    public List <Vector2Int> obstructedSquares;
    public List <Vector2Int> occupiedSquares;
    private int _unitId;
    private bool _blueUnitIsAlive;
    private bool _redUnitIsAlive;
    private GameObject TurnCaller;
    private GameObject OverlapChecker;
    private OverlapChecker _overlapChecker;
    private FootSoldierDragPlacer _footSoldierPlacer;
    private CavalierDragPlacer _cavalierPlacer;
    private RogueDragPlacer _roguePlacer;
    private ArcherDragPlacer _archerPlacer;
    private MageDragPlacer _magePlacer;
    private TilesManager _tilesManager;
    private int _moveThreshold;
    public bool _nukeCalled;
    private TurnCaller _turnCaller;   
    private Vector2Int _mapSize;
    // Awake runs before Start
    void Awake()
    {
        _nukeCalled = false;
        obstructedSquares = new List <Vector2Int>();
        _unitPrefab = GameObject.Find("UnitPrefab");
        GameObject FootSoldierPlacer = GameObject.Find("FootSoldierPlacer");
        GameObject CavalierPlacer = GameObject.Find("CavalierPlacer");
        GameObject RoguePlacer = GameObject.Find("RoguePlacer");
        GameObject ArcherPlacer = GameObject.Find("ArcherPlacer");
        GameObject MagePlacer = GameObject.Find("MagePlacer");
        _footSoldierPlacer = FootSoldierPlacer.GetComponent<FootSoldierDragPlacer>();
        _cavalierPlacer = CavalierPlacer.GetComponent<CavalierDragPlacer>();
        _roguePlacer = RoguePlacer.GetComponent<RogueDragPlacer>();
        _archerPlacer = ArcherPlacer.GetComponent<ArcherDragPlacer>();
        _magePlacer = MagePlacer.GetComponent<MageDragPlacer>();
        GameObject tilesManager = GameObject.Find("Main Camera/Game Manager");
        _tilesManager = tilesManager.GetComponent<TilesManager>();
        _unitId = 1;
        affectedSquares = new List<(Vector2Int coordinate, int id, int team)>();
        occupiedSquares = new List<Vector2Int>();
        TurnCaller = GameObject.Find("Main Camera/Game Manager");
        _turnCaller = TurnCaller.GetComponent<TurnCaller>();
        OverlapChecker = GameObject.Find("OverlapAfterMoveChecker");
        _overlapChecker = OverlapChecker.GetComponent<OverlapChecker>();
        _moveThreshold = 0;
        _blueUnitIsAlive = true;
        _redUnitIsAlive = true;
    }

    public void LevelLoadInitialize((int x, int y) MapSize, List<Vector2Int> obstacles)
    {
        _mapSize = new Vector2Int(MapSize.x, MapSize.y);
        obstructedSquares = obstacles;
    }
    public void Turn(int team)
    {
        // Initiate a coroutine for a whole team's movements
        StartCoroutine(TurnCoroutine(team));
    }

    private IEnumerator TurnCoroutine(int team)
    { // Set necessary variables and lists
        _tilesManager.ResetTiles();
        _unitsToMove.Clear();
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
        // Debug.Log("Turn finished");
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
            if (unit.GetTimesMovedInTurn() >= _moveThreshold)
            {
                _unitsToMove.Remove(clone); // Remove unit from list if it has moved enough
            }
        }
    }

    private void CheckAllOverlaps()
    {
        foreach (var position in affectedSquares)
        {
            _tilesManager.SetTileToAttacked(position.coordinate.x, position.coordinate.y);
        }
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
            unit.Initialize(1, _unitId, team, (xSpawnPoint, ySpawnPoint), obstructedSquares, "Foot", _mapSize, 0);
        }
        else if (unit != null && team == 1)
        { // Red Team
            _redClones.Add(clone);
            _redFootSoldiers.Add(clone);
            _clones.Add(clone);
            _unitId = _redFootSoldiers.Count + 100;
            unit.Initialize(1, _unitId, team, (xSpawnPoint, ySpawnPoint), obstructedSquares, "Foot", _mapSize, 1);
        }
    }
    public void CloneCavalier(float xSpawnPoint, float ySpawnPoint, int team) // Cavalier clone
    { // Clone and initialization of the cavalier as called by the player or LevelBuilder
        GameObject clone = Instantiate(_unitPrefab, new Vector2(xSpawnPoint, ySpawnPoint), Quaternion.identity);
        unit unit = clone.GetComponent<unit>();
        if (unit != null && team == 0)
        { // Blue Team
            _blueClones.Add(clone);
            _blueCavaliers.Add(clone);
            _clones.Add(clone);
            _unitId = _blueCavaliers.Count + 200;
            unit.Initialize(2, _unitId, team, (xSpawnPoint, ySpawnPoint), obstructedSquares, "Cavalier", _mapSize, 2);
        }
        else if (unit != null && team == 1)
        { // Red Team
            _redClones.Add(clone);
            _redCavaliers.Add(clone);
            _clones.Add(clone);
            _unitId = _redCavaliers.Count + 200;
            unit.Initialize(2, _unitId, team, (xSpawnPoint, ySpawnPoint), obstructedSquares, "Cavalier", _mapSize, 3);
        }
    }
    public void CloneRogue(float xSpawnPoint, float ySpawnPoint, int team) // Rogue clone
    { // Clone and initialization of the rogue as called by the player or LevelBuilder
        GameObject clone = Instantiate(_unitPrefab, new Vector2(xSpawnPoint, ySpawnPoint), Quaternion.identity);
        unit unit = clone.GetComponent<unit>();
        if (unit != null && team == 0)
        { // Blue Team
            _blueClones.Add(clone);
            _blueRogues.Add(clone);
            _clones.Add(clone);
            _unitId = _blueRogues.Count + 300;
            unit.Initialize(2, _unitId, team, (xSpawnPoint, ySpawnPoint), obstructedSquares, "Rogue", _mapSize, 4);
        }
        else if (unit != null && team == 1)
        { // Red Team
            _redClones.Add(clone);
            _redRogues.Add(clone);
            _clones.Add(clone);
            _unitId = _redRogues.Count + 300;
            unit.Initialize(2, _unitId, team, (xSpawnPoint, ySpawnPoint), obstructedSquares, "Rogue", _mapSize, 5);
        }
    }
    public void CloneArcher(float xSpawnPoint, float ySpawnPoint, int team) // Archer clone
    { // Clone and initialization of the archer as called by the player or LevelBuilder
        GameObject clone = Instantiate(_unitPrefab, new Vector2(xSpawnPoint, ySpawnPoint), Quaternion.identity);
        unit unit = clone.GetComponent<unit>();
        if (unit != null && team == 0)
        { // Blue Team
            _blueClones.Add(clone);
            _blueArchers.Add(clone);
            _clones.Add(clone);
            _unitId = _blueArchers.Count + 400;
            unit.Initialize(1, _unitId, team, (xSpawnPoint, ySpawnPoint), obstructedSquares, "Archer", _mapSize, 6);
        }
        else if (unit != null && team == 1)
        { // Red Team
            _redClones.Add(clone);
            _redArchers.Add(clone);
            _clones.Add(clone);
            _unitId = _redArchers.Count + 400;
            unit.Initialize(1, _unitId, team, (xSpawnPoint, ySpawnPoint), obstructedSquares, "Archer", _mapSize, 7);
        }
    }
    public void CloneMage(float xSpawnPoint, float ySpawnPoint, int team) // Mage clone
    { // Clone and initialization of the mage as called by the player or LevelBuilder
        GameObject clone = Instantiate(_unitPrefab, new Vector2(xSpawnPoint, ySpawnPoint), Quaternion.identity);
        unit unit = clone.GetComponent<unit>();
        if (unit != null && team == 0)
        { // Blue Team
            _blueClones.Add(clone);
            _blueMages.Add(clone);
            _clones.Add(clone);
            _unitId = _blueMages.Count + 500;
            unit.Initialize(1, _unitId, team, (xSpawnPoint, ySpawnPoint), obstructedSquares, "Mage", _mapSize, 8);
        }
        else if (unit != null && team == 1)
        { // Red Team
            _redClones.Add(clone);
            _redMages.Add(clone);
            _clones.Add(clone);
            _unitId = _redMages.Count + 500;
            unit.Initialize(1, _unitId, team, (xSpawnPoint, ySpawnPoint), obstructedSquares, "Mage", _mapSize, 9);
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
                Debug.Log(unit.GetId());
                occupiedSquares.Remove(new Vector2Int (unit.UnitPosition.x, unit.UnitPosition.y));
                _blueClones.Remove(clone);
                _clones.Remove(clone);
                ResetIds(unit.GetUnitType(), clone);
                Destroy(clone);
            }
        }
    }
    public void Nuke()
    { // Clears all lists (used when level is unloaded)
        Debug.Log("Nuke called");
        foreach (GameObject unit in _clones)
        {
            Destroy(unit);
        }
        _clones.Clear();
        _redClones.Clear();
        _blueClones.Clear();
        _blueFootSoldiers.Clear();
        _blueCavaliers.Clear();
        _blueRogues.Clear();
        _blueArchers.Clear();
        _blueMages.Clear();
        _redFootSoldiers.Clear();
        _redCavaliers.Clear();
        _redRogues.Clear();
        _redArchers.Clear();
        _redMages.Clear();
        occupiedSquares.Clear();
        _nukeCalled = true;
    }
    private void ResetIds(string _unitType, GameObject unitToRemove)
    { // Only needs to be used EVER for the blue team :P
        Debug.Log("Reset Ids called");
        switch (_unitType)
        {
            case "Foot":
                // Foot soldiers
                _blueFootSoldiers.Remove(unitToRemove);
                _footSoldierPlacer._amountPlaced -= 1;
                _unitId = 0;
                foreach (GameObject clone in _blueFootSoldiers)
                {
                    unit unit = clone.GetComponent<unit>();
                    _unitId += 1;
                    unit?.SetId(100+_unitId);
                }
                break;
            case "Cavalier":
                // Cavaliers
                _blueCavaliers.Remove(unitToRemove);
                _cavalierPlacer._amountPlaced -= 1;
                _unitId = 0;
                foreach (GameObject clone in _blueCavaliers)
                {
                    unit unit = clone.GetComponent<unit>();
                    _unitId += 1;
                    unit?.SetId(200+_unitId);
                }
                break;
            case "Rogue":
                // Cavaliers
                _blueRogues.Remove(unitToRemove);
                _roguePlacer._amountPlaced -= 1;
                _unitId = 0;
                foreach (GameObject clone in _blueCavaliers)
                {
                    unit unit = clone.GetComponent<unit>();
                    _unitId += 1;
                    unit?.SetId(300+_unitId);
                }
                break;
            case "Archer":
                // Cavaliers
                _blueArchers.Remove(unitToRemove);
                _archerPlacer._amountPlaced -= 1;
                _unitId = 0;
                foreach (GameObject clone in _blueCavaliers)
                {
                    unit unit = clone.GetComponent<unit>();
                    _unitId += 1;
                    unit?.SetId(400+_unitId);
                }
                break;
            case "Mage":
                // Cavaliers
                _blueMages.Remove(unitToRemove);
                _magePlacer._amountPlaced -= 1;
                _unitId = 0;
                foreach (GameObject clone in _blueCavaliers)
                {
                    unit unit = clone.GetComponent<unit>();
                    _unitId += 1;
                    unit?.SetId(500+_unitId);
                }
                break;
            default:
                break;
        }
    }
    public bool CheckLoopStatus(bool _shouldReset)
    {
        _nukeCalled = false;
        if (_shouldReset)
        { // If a reset has been called/cached
            return false; // End turn loop
        }
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
            if (unit.UnitIsDead() == false && unit._cantMove == false)
            { // IF a blue unit is alive, and capable of moving
                _blueUnitIsAlive = true; // Set flag to continue loop
            }
        }
        foreach (GameObject clone in _redClones)
        {
            unit unit = clone.GetComponent<unit>();
            if (unit.UnitIsDead() == false && unit._cantMove == false)
            { // IF a red unit is alive, and capable of moving
                _redUnitIsAlive = true; // Set flag to continue loop
            }
        }
        if (_nukeCalled)
        { // If a level was unloaded mid-loop
            return false;
        }
        if (_blueUnitIsAlive == false || _redUnitIsAlive == false)
        { // If either team has no usable units
            return false; // End turn loop
        }
        // Otherwise continue turn loop
        return true; // Default case
    }
}
