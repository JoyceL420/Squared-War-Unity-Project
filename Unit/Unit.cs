using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class unit : MonoBehaviour
{
    // Creation of variables
    public bool isDead;
    [SerializeField] private int _team; // 0 = Blue/you, 1 = Red
    public Vector2Int UnitPosition;
    private int _lastMovedDirection;
    //private string unitType;
    [SerializeField] private int _speed;
    public int _timesMoved;
    private int _timesMovedInTurn;
    public List<(float x, float y)> passedSquares;
    private string _unitType;
    public (float x, float y) attackedTile;
    [SerializeField] private int _unitId;
    private List <int> _path;
    private List <Vector2Int> _obstacles;
    public (float x, float y) _spawnPoint;
    public bool _cantMove;
    private Collider2D _collider;
    private GameObject TeamController;
    private TeamController _teamController; 
    private SpriteController _spriteController; 
    private Pathfinding _pathfinder;
    private FootMovement _footMovement;
    private int _mapWidth;
    private Vector2Int _mapSize;
    private AttackTypes _attack;
    // private FootMovement _freeMovement;
    private int _directionPreviouslyMoved;
    private bool _diagonalMovementAllowed; // Not referenced in this script
    [SerializeField] private SpriteRenderer _spriteRenderer;
    void Start()
    { // Runs before first frame
        isDead = false;
        _cantMove = false;
        passedSquares = new List<(float x, float y)>();
        attackedTile = (0, 0);
        _collider = GetComponent<Collider2D>();
        _timesMoved = 0;
        _directionPreviouslyMoved = 0;
        UpdatePosition();
        _footMovement = GetComponent<FootMovement>();
    }
    public string GetUnitType()
    {
        return _unitType;
    }
    public void SetDirectionPreviouslyMoved(int direction)
    {
        _directionPreviouslyMoved = direction;
    }
    public int GetDirectionPreviouslyMoved()
    {
        return _directionPreviouslyMoved;
    }
    public int GetTeam()
    {
        return _team;
    }
    public void Initialize(int speed, int id, int team, (float x, float y) spawnPoint, List<Vector2Int> obstructedSquares, string unitType, Vector2Int mapSize, int sprite)
    {
        _team = team;
        _speed = speed;
        _unitId = id;
        _spawnPoint = (spawnPoint.x, spawnPoint.y);
        _obstacles = obstructedSquares;
        TeamController = GameObject.Find("TeamsController");
        _teamController = TeamController.GetComponent<TeamController>();
        _spriteController = GetComponent<SpriteController>();
        _spriteController.InitializeSprite(sprite);
        _pathfinder = GetComponent<Pathfinding>();
        _attack = GetComponent<AttackTypes>();
        _mapSize = mapSize;
        _unitType = unitType;
        switch (unitType.FirstCharacterToUpper())
        { // Validates/sets movement settings
            case "Cavalier":
                _diagonalMovementAllowed = false;
                break;
            case "Rogue":
                _diagonalMovementAllowed = true;
                Debug.Log("Diagonal set to true");
                break;
            case "Archer":
                _diagonalMovementAllowed = false;
                break;
            case "Mage":
                _diagonalMovementAllowed = false;
                break;
            default:
                _unitType = "Foot";
                _unitType = unitType;
                _diagonalMovementAllowed = false;
                break;
        }
        // Debug.Log($"Unit of type {_unitType} Initialized with id {_unitId}");
        _timesMoved = 0;
        // Finds the path
        _path = _pathfinder.FindPath(CurrentPosition(), _mapSize, _obstacles, team, _diagonalMovementAllowed);
    }

    public void MovementVariablesReset()
    {
        // Debug.Log("Reset");
        _directionPreviouslyMoved = 0; // Inactive/no direction
        _timesMovedInTurn = 0; // Hasn't moved in a turn
    }
    public bool UnitIsDead()
    {
        if (isDead)
        {
            return true;
        }
        return false;
    }
    public int GetId()
    {
        return _unitId;
    }
    public int GetTimesMovedInTurn()
    {
        return _timesMovedInTurn;
    }
    public void SetId(int id)
    {
        _unitId = id;
    }
    public void AddTimesMoved()
    {
        _timesMoved += 1;
    }
    public int GetSpeed()
    {
        return _speed;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            CheckMouse2Overlap();
        }
    }

    public void TurnCall()
    {
        if (!_cantMove)
        {
            Move();
        }
    }

    private Vector2Int CurrentPosition()
    {
        return new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
    }
    private int GetDirection()
    {
        // Debug.Log($"Index: {_timesMoved -1}");
        if (_path.Count == _timesMoved)
        {
            _cantMove = true;
            return _path[_timesMoved - 1];
        }
        return _path[_timesMoved - 1];
    }
    void Move() 
    {
        if (_timesMovedInTurn < _speed)
        {
            _timesMovedInTurn += 1; // Attempt to move being called
            switch (_unitType)
            { // Determines how a unit will move and attack
                case "Foot": // Complete
                    _footMovement.Move(GetDirection());
                    UpdatePosition();
                    _attack.FootSoldierAttack(UnitPosition);
                    break;
                case "Cavalier": // Complete
                    _footMovement.Move(GetDirection());
                    UpdatePosition();
                    _attack.FootSoldierAttack(UnitPosition);
                    break;
                case "Rogue": // Needs testing
                    _footMovement.Move(GetDirection());
                    UpdatePosition();
                    _attack.FootSoldierAttack(UnitPosition);
                    break; 
                case "Archer": // Needs testing
                    _footMovement.Move(GetDirection());
                    UpdatePosition();
                    _attack.ArcherAttack(UnitPosition, _lastMovedDirection);
                    break;
                case "Mage": // Needs testing
                    _footMovement.Move(GetDirection());
                    UpdatePosition();
                    _attack.MageAttack(UnitPosition, _lastMovedDirection);
                    break;
                default:
                    Debug.LogError("ERROR: _movementType variable OUTSIDE ACCEPTED RANGE");
                    break;  
            }
            UpdatePosition(); // Redundant
        }
    }
    public void MoveTile(int direction)
    {
        // Debug.Log("Move tile called");
        _lastMovedDirection = direction;
        switch (direction)
        {
            case 1:
                // Up
                transform.position = new Vector2(transform.position.x, transform.position.y + 1);
                break;
            case 2:
                // Up right
                transform.position = new Vector2(transform.position.x + 1, transform.position.y + 1);
                break;
            case 3:
                // Right
                transform.position = new Vector2(transform.position.x + 1, transform.position.y);
                break;
            case 4:
                // Down right
                transform.position = new Vector2(transform.position.x + 1, transform.position.y - 1);
                break;
            case 5:
                // Down
                transform.position = new Vector2(transform.position.x , transform.position.y - 1);
                break;
            case 6:
                // Down left
                transform.position = new Vector2(transform.position.x - 1, transform.position.y - 1);
                break;
            case 7:
                // Left
                transform.position = new Vector2(transform.position.x - 1, transform.position.y);
                break;
            case 8:
                transform.position = new Vector2(transform.position.x - 1, transform.position.y + 1);
                break;
            default:
                // Error case for an attempted movement outside of allowed range
                Debug.LogError($"Attempted movement outside range {direction} for: {_unitId}");
                break;
        }
        UpdatePosition(); // Redundant
        if ((_team == 0 && UnitPosition.x == _mapWidth - 1) || (_team == 1 && UnitPosition.x == 0))
        { // If on blue team and on the right end of the map or on red team and on the left side of the map
            _cantMove = true; // Set cant move flag
        }
    }

    public void Kill() 
    {
        // This is the method that is called by TurnCaller when this unit is positioned on an "affected" tile
        // Whether by an enemy or ally.

        isDead = true;
        // Debug.Log("UNIT:" + _unitId + " OF TEAM " + _team + " IS DED!");
        transform.position = new Vector2(100, 100);
    }
    public void Reset()
    {
        isDead = false;
        _cantMove = false;
        _timesMoved = 0;
        transform.position = new Vector2(_spawnPoint.x, _spawnPoint.y);
        MovementVariablesReset();
        UpdatePosition();
    }

    private void UpdatePosition() 
    { // Just updates the xPos and yPos variables for ease of use
        UnitPosition = new Vector2Int (Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
    }
    private void CheckMouse2Overlap()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        if (_collider.OverlapPoint(mouseWorldPosition))
        {
            _teamController.RemoveUnit(_unitId);
        }
    }
}