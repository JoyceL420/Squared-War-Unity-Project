using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelBuilder : MonoBehaviour
{
    // TEMPORARILY ASSIGNED TO Main Camera GAMEOBJECT FOR TESTING
    // WILL BE ASSIGNED TO AN EMPTY OBJECT LATER
    private TeamController _teamsController; 
    private TilesManager _tilesManager;
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
    // Start is called before the first frame update
    void Start()
    {
        _obstaclesList = new List<GameObject>(); 
        _obstructedSquares = new List<Vector2Int>();
        // This Script gets the data from LevelSpecifications based on what level was chosen
        // And places the units and battle elements and tells them their starting positions
        
        // Assigning references
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

        // DEBUG LEVEL
        LoadLevel(0);
    }

    private void Update()   
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // DEBUG
        {
            DeactivateLevel();
        }
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
        _obstructedSquares.Clear();
        _tilesManager.RemoveTiles();
        // This method simply removes all tiles that have been created
        for (int i = _obstaclesList.Count - 1; i >= 0; i--)
        { // Iterate backwards to prevent errors from removal of items mid-iteration
            GameObject obstacle = _obstaclesList[i];
            _obstaclesList.Remove(obstacle);
            Destroy(obstacle);
        }
        
    }
    private void LoadLevel(int level)
    {
        switch(level)
        {
            case 0:
            _mapWidth = 16;
            _mapHeight = 9;
            InitializeControllers();
            foreach (Vector2Int obstacle in _levelData.GetObstaclesForLevel(level))
            {
                _obstructedSquares.Add(obstacle);
            }
            PlaceObstructions();
            EstablishBoundary();
                break;
            default:
                break;
        }
    }

    private void InitializeControllers()
    {
        _teamsController.LevelLoadInitialize((_mapWidth, _mapHeight));
        _footsoldierPlacer.LevelLoadInitialize((_mapWidth, _mapHeight));
        _cavalierPlacer.LevelLoadInitialize((_mapWidth, _mapHeight));
        _roguePlacer.LevelLoadInitialize((_mapWidth, _mapHeight));
        _archerPlacer.LevelLoadInitialize((_mapWidth, _mapHeight));
        _magePlacer.LevelLoadInitialize((_mapWidth, _mapHeight));
    }
}
