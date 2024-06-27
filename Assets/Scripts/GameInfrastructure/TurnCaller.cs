using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnCaller : MonoBehaviour
{
    [SerializeField] private int _currentTurn; // 0 = (Blue team/default), 1 = (Red team)
    private GameObject TeamController;
    TeamController _teamController;
    private bool _preparationStatus;
    private bool _turnOngoing;
    public void FinishTurn()
    {
        _turnOngoing = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        _currentTurn = 0;
        _turnOngoing = false;
        _preparationStatus = true;
        TeamController = GameObject.Find("TeamsController");
        _teamController = TeamController.GetComponent<TeamController>();
        // This is the script that will call the passing of a turn to tell all units on the
        // Blue/red side that they will need to move

        // There will be a set delay 
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _turnOngoing is false)
        {   
            _preparationStatus  = false;
            switch (_currentTurn)
            {
                case 0:
                    _turnOngoing = true;
                    _teamController.Turn(_currentTurn);
                    _currentTurn = 1;
                    //Debug.Log("Turn=2");
                    break;
                default:
                    _turnOngoing = true;
                    _teamController.Turn(_currentTurn);
                    _currentTurn = 0;
                    //Debug.Log("Turn=1");
                    break;

            }
        }
        if (Input.GetKeyDown(KeyCode.R) && _turnOngoing is false)
        {
            _preparationStatus  = true;
            _currentTurn = 0;
            _teamController.ResetPostions();
        }
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
}
