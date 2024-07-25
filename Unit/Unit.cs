using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Numerics;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class unit : MonoBehaviour
{
    // Creation of variables
    public bool isDead;
    [SerializeField] private int _team; // 0 = Blue/you, 1 = Red
    [SerializeField] public float xPos;
    [SerializeField] public float yPos;
    //private string unitType;
    [SerializeField] private int _speed;
    public int _timesMoved;
    public List<(float x, float y)> passedSquares;
    private int _unitType;
    public (float x, float y) attackedTile;
    [SerializeField] private int _unitId;
    private List <int> _movementPriority;
    private List <(float x, float y)> _obstructedSquares;
    public (float x, float y) _spawnPoint;
    public bool cantMove;
    private Collider2D _collider;
    private GameObject TeamController;
    private TeamController _teamController; 
    private FootMovement _footMovement;
    private CavalierMovement _cavalierMovement;
    private AttackTypes _attack;
    // private FootMovement _freeMovement;
    private string _movementType;
    [SerializeField] private int _directionMoved;
    [SerializeField] private Color _blue; // Update to sprite when graphics implemented
    [SerializeField] private Color _red; // Update to sprite when graphics implemented
    [SerializeField] private SpriteRenderer _spriteRenderer;

    void Awake()
    { // Runs on creation
        
    }
    void Start()
    { // Runs before first frame
        isDead = false;
        cantMove = false;
        passedSquares = new List<(float x, float y)>();
        attackedTile = (0, 0);
        _collider = GetComponent<Collider2D>();
        _timesMoved = 0;
        _directionMoved = -1; // -1 is inactive (relevant for cavalier movement)
        UpdatePosition();
    }
    public int GetUnitType()
    {
        return _unitType;
    }
    public void SetDirectionMoved(int direction)
    {
        _directionMoved = direction;
    }
    public int GetTeam()
    {
        return _team;
    }
    public void Initialize(int speed, int id, int team, List<int> movementPriority, (float x, float y) spawnPoint, List<(float, float)> obstructedSquares, string movementType, int unitType)
    {
        _team = team;
        if (_team == 1)
        {
            _spriteRenderer.color = _red;
        }
        else
        { // (Is 0)
            _spriteRenderer.color = _blue;
        }
        _speed = speed;
        _unitId = id;
        _movementPriority = movementPriority;
        _spawnPoint = (spawnPoint.x, spawnPoint.y);
        _obstructedSquares = obstructedSquares;
        TeamController = GameObject.Find("TeamsController");
        _teamController = TeamController.GetComponent<TeamController>();
        _attack = GetComponent<AttackTypes>();
        _unitType = unitType;
        switch (movementType.FirstCharacterToUpper())
        {
            case "Cavalier":
                _cavalierMovement = GetComponent<CavalierMovement>();
                // Debug.Log(movementType);
                _movementType = movementType;
                break;
            case "Free":
                // _freeMovement = GetComponent<FootMovement>();
                // Debug.Log(movementType);
                _movementType = movementType;
                break;
            default:
                movementType = "Foot";
                _footMovement = GetComponent<FootMovement>();
                // Debug.Log(movementType);
                _movementType = movementType;
                break;
        }
    }

    public void MovementVariablesReset()
    {
        // Debug.Log("Reset");
        _directionMoved = -1; // Inactive/no direction
        _timesMoved = 0; // Hasn't moved in a turn
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
        Move();
    }

    void Move() 
    {
        // Debug.Log("Move was called");
        switch (_movementType)
        {
            case "Foot":
                _footMovement.Move(_movementPriority, _obstructedSquares, xPos, yPos);
                UpdatePosition();
                _attack.FootSoldierAttack((xPos, yPos));
                break;
            case "Cavalier":
                _cavalierMovement.Move(_movementPriority, _obstructedSquares, xPos, yPos, _directionMoved);
                UpdatePosition();
                _attack.FootSoldierAttack((xPos, yPos));
                break;
            case "Free":
                UpdatePosition();
                break; 
            default:
                Debug.LogError("ERROR: _movementType variable OUTSIDE ACCEPTED RANGE");
                break;  
        }
        UpdatePosition(); // Redundant
    }
    public void MoveTile(int direction)
    {
        // Debug.Log("Move tile called");
        _directionMoved = direction;
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
                Debug.Log("ERROR: ATTEMPTED MOVEMEMENT OUTSIDE OF ACCEPTED RANGE UNIT:" + _unitId);
                break;
        }
        
    }

    public void Kill() 
    {
        // This is the method that is called by TurnCaller when this unit is positioned on an "affected" tile
        // Whether by an enemy or ally.

        isDead = true;
        Debug.Log("UNIT:" + _unitId + " OF TEAM " + _team + " IS DED!");
        transform.position = new Vector2(100, 100);
    }
    public void Reset()
    {
        isDead = false;
        cantMove = false;
        _timesMoved = 0;
        transform.position = new Vector2(_spawnPoint.x, _spawnPoint.y);
        MovementVariablesReset();
        UpdatePosition();
    }

    private void UpdatePosition() 
    { // Just updates the xPos and yPos variables for ease of use
        xPos = transform.position.x;
        yPos = transform.position.y;
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
