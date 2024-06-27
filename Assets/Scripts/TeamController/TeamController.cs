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
    private List<GameObject> _redClones = new List<GameObject>();
    private List<GameObject> _clones = new List<GameObject>();
    private List<GameObject> _unitsToMove = new List<GameObject>();
    public List<(float x, float y, int id)> affectedSquares;
    public List <(float x, float y)> obstructedSquares;
    public List <(float x, float y)> occupiedSquares;
    private int unitId;
    private List <int> _unitsToRemove;
    private GameObject TurnCaller;
    private GameObject OverlapChecker;
    private OverlapChecker _overlapChecker;
    private int _moveThreshold;
    private TurnCaller _turnCaller;    
    // Awake runs before Start
    void Awake()
    {
        obstructedSquares = new List<(float x, float y)>();
    }
    // Start is called before the first frame update
    void Start()
    {
        unitId = 1;
        affectedSquares = new List<(float x, float y, int id)>();
        occupiedSquares = new List<(float x, float y)>();
        _unitsToRemove = new List<int>();
        _unitPrefab = GameObject.Find("UnitPrefab");
        TurnCaller = GameObject.Find("Main Camera/Game Manager");
        _turnCaller = TurnCaller.GetComponent<TurnCaller>();
        OverlapChecker = GameObject.Find("OverlapAfterMoveChecker");
        _overlapChecker = OverlapChecker.GetComponent<OverlapChecker>();
        _moveThreshold = 0;
    }
    public void Turn(int team)
    {
        StartCoroutine(TurnCoroutine(team));
    }

    private IEnumerator TurnCoroutine(int team)
    {
        TurnPrep(team);
        for (int _timesMoved = 0; _timesMoved < _moveThreshold; _timesMoved++)
        {
            MoveUnits(_unitsToMove);
            CheckAllOverlaps();
            yield return new WaitForSeconds(0.5f); // Delay between movements
        }
        Debug.Log("Turn finished");
        _turnCaller.FinishTurn();
    }
    private void TurnPrep(int team)
    {
        _unitsToMove.Clear(); // Clear the list at the start of each turn
        _moveThreshold = 0; // Reset move threshold

        switch (team)
        {
            case 0:
                foreach (GameObject clone in _blueClones)
                {
                    _unitsToMove.Add(clone);
                    unit unit = clone.GetComponent<unit>();
                    unit.MovementVariablesReset();
                    if (unit.GetSpeed() > _moveThreshold)
                    {
                        _moveThreshold = unit.GetSpeed();
                    }
                }
                break;
            case 1:
                foreach (GameObject clone in _redClones)
                {
                    _unitsToMove.Add(clone);
                    unit unit = clone.GetComponent<unit>();
                    unit.MovementVariablesReset();
                    if (unit.GetSpeed() > _moveThreshold)
                    {
                        _moveThreshold = unit.GetSpeed();
                    }
                }
                break;
            default:
                Debug.LogError("Something went wrong (ERROR TurnPrep method TEAM INT OUTSIDE ACCEPTED RANGE)");
                break;
        }
    }

    private void MoveUnits(List<GameObject> _unitsToMove)
    {
        for (int i = _unitsToMove.Count - 1; i >= 0; i--)
        {
            GameObject clone = _unitsToMove[i];
            unit unit = clone.GetComponent<unit>();
            unit.AddTimesMoved();
            unit.TurnCall();
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
    {
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
    {
        GameObject clone = Instantiate(_unitPrefab, new Vector2(xSpawnPoint, ySpawnPoint), Quaternion.identity);
        unit unit = clone.GetComponent<unit>();
        if (unit != null && team == 0)
        { // Blue Team
            _blueClones.Add(clone);
            _clones.Add(clone);
            unit.Initialize(2, unitId, team, new List<int> {3, 1, 5, 0}, (xSpawnPoint, ySpawnPoint), obstructedSquares, "Foot");
        }
        else if (unit != null && team == 1)
        { // Red Team
            _redClones.Add(clone);
            _clones.Add(clone);
            unit.Initialize(2, unitId, team, new List<int> {7, 1, 5, 0}, (xSpawnPoint, ySpawnPoint), obstructedSquares, "Foot");
        }
        unitId += 1;
    }
    public void CloneCavalier(float xSpawnPoint, float ySpawnPoint, int team) // Foot soldier clone
    {
        GameObject clone = Instantiate(_unitPrefab, new Vector2(xSpawnPoint, ySpawnPoint), Quaternion.identity);
        unit unit = clone.GetComponent<unit>();
        if (unit != null && team == 0)
        { // Blue Team
            _blueClones.Add(clone);
            _clones.Add(clone);
            unit.Initialize(2, unitId, team, new List<int> {3, 1, 5, 0}, (xSpawnPoint, ySpawnPoint), obstructedSquares, "Cavalier");
        }
        else if (unit != null && team == 1)
        { // Red Team
            _redClones.Add(clone);
            _clones.Add(clone);
            unit.Initialize(2, unitId, team, new List<int> {7, 1, 5, 0}, (xSpawnPoint, ySpawnPoint), obstructedSquares, "Cavalier");
        }
        unitId += 1;
    }
    public void RemoveUnit(int unitId)
    { // Loops backwards to prevent errors from removing item during iteration
        for (int i = _blueClones.Count - 1; i >= 0; i--)
        {
            GameObject clone = _blueClones[i];
            unit unit = clone.GetComponent<unit>();

            if (unit.GetId() == unitId && _turnCaller.PreparationStatus())
            {
                occupiedSquares.Remove((unit.xPos, unit.yPos));
                _blueClones.Remove(clone);
                _clones.Remove(clone);
                Destroy(clone);
            }
        }
    }
    private void Nuke()
    {
        // Empties every single list and removes all units
        // (Will be used when changing level)
    }
    IEnumerator WaitForSeconds(float seconds){
        yield return new WaitForSecondsRealtime(seconds);
    }
}
