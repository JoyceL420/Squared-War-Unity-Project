using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelBuilder : MonoBehaviour
{
    private TeamController _teamsController; 
    private bool _levelActive;
    private TilesManager _tilesManager;
    private CameraMovement _cameraMover;
    private FootSoldierDragPlacer _footsoldierPlacer;
    private CavalierDragPlacer _cavalierPlacer;
    private RogueDragPlacer _roguePlacer;
    private ArcherDragPlacer _archerPlacer;
    private MageDragPlacer _magePlacer;
    private LevelData _levelData;
    private int _mapWidth;
    private int _mapHeight;
    private GameObject _obstaclePrefab;
    private List <Vector2Int> _obstructedSquares;
    private List <GameObject> _obstaclesList;
    private UnitsCounter _unitsCounter;
    private List<int> _unitLimits;
    void Start()
    {
        _obstaclesList = new List<GameObject>(); 
        _obstructedSquares = new List<Vector2Int>();
        _levelActive = false;
        // This Script gets the data from LevelSpecifications based on what level was chosen
        // And places the units and battle elements and tells them their starting positions
        
        // Assigning references
        _cameraMover = GetComponent<CameraMovement>();
        GameObject TeamsController = GameObject.Find("TeamsController");
        _teamsController = TeamsController.GetComponent<TeamController>();
        GameObject TilesManager = GameObject.Find("Main Camera/Game Manager");
        _tilesManager = TilesManager.GetComponent<TilesManager>();
        GameObject UnitPlacer = GameObject.Find("FootSoldierPlacer");
        _footsoldierPlacer = UnitPlacer.GetComponent<FootSoldierDragPlacer>();
        UnitPlacer = GameObject.Find("CavalierPlacer");
        _cavalierPlacer = UnitPlacer.GetComponent<CavalierDragPlacer>();
        UnitPlacer = GameObject.Find("RoguePlacer");
        _roguePlacer = UnitPlacer.GetComponent<RogueDragPlacer>();
        UnitPlacer = GameObject.Find("ArcherPlacer");
        _archerPlacer = UnitPlacer.GetComponent<ArcherDragPlacer>();
        UnitPlacer = GameObject.Find("MagePlacer");
        _magePlacer = UnitPlacer.GetComponent<MageDragPlacer>();
        _obstaclePrefab = GameObject.Find("ObstaclePrefab");
        _levelData = GetComponent<LevelData>();
        GameObject unitsCounter = GameObject.Find("UnitsCounter");
        _unitsCounter = unitsCounter.GetComponent<UnitsCounter>();

        // DEBUG LEVEL
        LoadLevel(0);
    }

    private void Update()   
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // DEBUG
        {
            _levelActive = false;
            DeactivateLevel();
        }
        if (_levelActive)
        {
            UpdateUnitsCounter();
        }
    }

    private void UpdateUnitsCounter()
    {
        _unitsCounter.UpdateText($"{_footsoldierPlacer._amountPlaced} of {_unitLimits[0]} allowed foot soldiers spawned\n{_cavalierPlacer._amountPlaced} of {_unitLimits[1]} allowed cavaliers spawned\n{_roguePlacer._amountPlaced} of {_unitLimits[2]} allowed rogues spawned\n{_archerPlacer._amountPlaced} of {_unitLimits[3]} allowed archers spawned\n{_magePlacer._amountPlaced} of {_unitLimits[4]} allowed mages spawned");
    }

    void PlaceObstructions()
    {
        // Obstacle placement
        foreach (Vector2Int coordinate in _obstructedSquares)
        {
            PlaceObstacle(coordinate.x, coordinate.y);
            _teamsController.obstructedSquares.Add(new Vector2Int (coordinate.x, coordinate.y));
        }
    }
    void EstablishBoundary()
    { // This method spawns the grids and sets an obstructedSquares boundary
        // Current width and height arguements are for debugging purposes
        // Make this system more flexible when working on level generation
        
        // Spawns grid
        _tilesManager.GenerateGrid(_mapWidth, _mapHeight);

        // Adds additional obstructed tiles surrounding the grid
        for (int x = 0; x <= _mapWidth + 1; x++)
        {
            _obstructedSquares.Add(new Vector2Int (x, 0)); // bottom
            _obstructedSquares.Add(new Vector2Int (x, _mapHeight + 1)); // top
        }
        for (int y = 1; y < _mapHeight + 1; y++)
        {
            _obstructedSquares.Add(new Vector2Int (0, y)); // left
            _obstructedSquares.Add(new Vector2Int (_mapWidth + 1, y)); // right
        }
        // Now spawn obstructions
        PlaceObstructions();
    }
    void PlaceObstacle(float xSpawnPoint, float ySpawnPoint)
    {
        GameObject obstacle = Instantiate(_obstaclePrefab, new Vector2(xSpawnPoint, ySpawnPoint), Quaternion.identity);
        _obstaclesList.Add(obstacle);
    }

    private void DeactivateLevel()
    {
        _teamsController.Nuke();
        _obstructedSquares.Clear();
        _tilesManager.RemoveTiles();
        // This method simply removes all tiles that have been created
        for (int i = _obstaclesList.Count - 1; i >= 0; i--)
        { // Iterate backwards to prevent errors from removal of items mid-iteration
            GameObject obstacle = _obstaclesList[i];
            _obstaclesList.Remove(obstacle);
            Destroy(obstacle);
        }
        _cameraMover.LevelUnload();
        _footsoldierPlacer.LevelUnload();
        _cavalierPlacer.LevelUnload();
        _roguePlacer.LevelUnload();
        _archerPlacer.LevelUnload();
        _magePlacer.LevelUnload();
        _unitsCounter.ResetText();
    }
    private void LoadLevel(int level)
    {
        switch(level)
        {
            case 0:
                _levelActive = true;
                _mapWidth = 16;
                _mapHeight = 9;
                foreach (Vector2Int obstacle in _levelData.GetObstaclesForLevel(level))
                {
                    _obstructedSquares.Add(obstacle);
                }
                InitializeControllers(level);
                foreach (var unit in _levelData.GetUnitsForLevel(level))
                {
                    SummonUnit(unit.position, unit.type);
                }
                break;
            default:
                break;
        }
        PlaceObstructions();
        EstablishBoundary();
        _cameraMover.LevelLoadInitialize((_mapWidth, _mapHeight));
    }

    private void InitializeControllers(int level)
    {
        _teamsController.LevelLoadInitialize((_mapWidth, _mapHeight), _levelData.GetObstaclesForLevel(level));
        _unitLimits = _levelData.GetUnitLimitsForLevel(level);
        _footsoldierPlacer.LevelLoadInitialize((_mapWidth, _mapHeight), _levelData.GetUnitLimitsForLevel(level)[0]);
        _cavalierPlacer.LevelLoadInitialize((_mapWidth, _mapHeight), _levelData.GetUnitLimitsForLevel(level)[1]);
        _roguePlacer.LevelLoadInitialize((_mapWidth, _mapHeight), _levelData.GetUnitLimitsForLevel(level)[2]);
        _archerPlacer.LevelLoadInitialize((_mapWidth, _mapHeight), _levelData.GetUnitLimitsForLevel(level)[3]);
        _magePlacer.LevelLoadInitialize((_mapWidth, _mapHeight), _levelData.GetUnitLimitsForLevel(level)[4]);
    }
    private void SummonUnit(Vector2Int position, int type)
    {
        switch (type)
        {
            case 0: // Foot soldier
                _teamsController.CloneFootSoldier(position.x, position.y, 1);
                break;
            case 1: // Cavalier
                _teamsController.CloneFootSoldier(position.x, position.y, 1);
                break;
            case 2: // Rogue
                _teamsController.CloneFootSoldier(position.x, position.y, 1);
                break;
            case 3: // Archer
                _teamsController.CloneFootSoldier(position.x, position.y, 1);
                break;
            case 4: // Mage
                _teamsController.CloneFootSoldier(position.x, position.y, 1);
                break;
            default:
                _teamsController.CloneFootSoldier(position.x, position.y, 1);
                break;
        }
    }
}
