using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnCaller : MonoBehaviour
{
    [SerializeField] private int _currentTurn; // 0 = (Blue team/default), 1 = (Red team)
    private GameObject TeamController;
    TeamController _teamController;
    private bool _preparationStatus;
    private bool _cachedReset;
    private bool _turnOngoing;
    private bool _turnLoopReiterate;
    private bool _allowReload;
    private TilesManager _tilesManager;
    // Going to redo the systems so that checks can be done to determine IF no more moves can be made 
    // OR all the units on one team are gone.
    public void FinishTurn()
    {
        _turnOngoing = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        _currentTurn = 0;
        _turnOngoing = false;
        _turnLoopReiterate = true;
        _preparationStatus = true;
        _allowReload = true;
        TeamController = GameObject.Find("TeamsController");
        _teamController = TeamController.GetComponent<TeamController>();
        GameObject tilesManager = GameObject.Find("Main Camera/Game Manager");
        _tilesManager = tilesManager.GetComponent<TilesManager>();
        // This is the script that will call the passing of a turn to tell all units on the
        // Blue/red side that they will need to move

        // There will be a set delay 
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _turnOngoing is false)
        { 
            
            StartCoroutine(TurnLoop());
        }
        if (Input.GetKeyDown(KeyCode.R))
        { 
            _cachedReset = true;
        }
    }

    public void Reset()
    {
        _cachedReset = false;
        _preparationStatus  = true;
        _currentTurn = 0;
        _teamController.ResetPostions();
    }
    public bool PreparationStatus()
    {
            if (_preparationStatus)
            {
                return true;
            }
            else
            {
                return false;
            }
    }

    IEnumerator TurnLoop()
    { // THIS WILL RESULT IN AN INFINITE LOOP IF LEFT UNBROKEN
        _preparationStatus = false;
        _turnLoopReiterate = true;
        while (_turnLoopReiterate)
        {
            // Reset flag
            _turnLoopReiterate = false;
            _tilesManager.ResetTiles();
            // Check turn and run methods for that turn
            switch (_currentTurn)
            {
                case 0:
                    _turnOngoing = true;
                    _teamController.Turn(_currentTurn);
                    _currentTurn = 1;
                    break;

                default:
                    _turnOngoing = true;
                    _teamController.Turn(_currentTurn);
                    _currentTurn = 0;
                    break;
            }
            // Delay before reiteration
            yield return new WaitForSeconds(1f);
            
            // Determine if reiteration should happen
            _turnLoopReiterate = _teamController.CheckLoopStatus(_cachedReset);
            if (_teamController._nukeCalled)
            {
                _turnLoopReiterate = false;
            }
        }
        Debug.Log("Loop ended");
        _tilesManager.ResetTiles();
        Reset();
    }
}
