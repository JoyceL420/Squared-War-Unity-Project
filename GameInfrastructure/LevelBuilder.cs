using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    // TEMPORARILY ASSIGNED TO Main Camera GAMEOBJECT FOR TESTING
    // WILL BE ASSIGNED TO AN EMPTY OBJECT LATER
    private TeamController _teamsController; 
    private TilesManager _tilesManager;
    private FootSoldierDragPlacer _unitPlacer;
    private int _mapWidth;
    private int _mapHeight;
    private GameObject _obstaclePrefab;
    private List <Vector2Int> obstructedSquares;
    // Start is called before the first frame update
    void Start()
    {
        obstructedSquares = new List<Vector2Int>();
        // This Script gets the data from LevelSpecifications based on what level was chosen
        // And places the units and battle elements and tells them their starting positions
        
        // Assigning references
        GameObject TeamsController = GameObject.Find("TeamsController");
        _teamsController = TeamsController.GetComponent<TeamController>();
        GameObject TilesManager = GameObject.Find("Main Camera/Game Manager");
        _tilesManager = TilesManager.GetComponent<TilesManager>();
        GameObject UnitPlacer = GameObject.Find("FootSoldierPlacer");
        _unitPlacer = UnitPlacer.GetComponent<FootSoldierDragPlacer>();

        _obstaclePrefab = GameObject.Find("ObstaclePrefab");

        // DEBUG STUFF
        _mapWidth = 16;
        _mapHeight = 9;
        _teamsController.LevelLoadInitialize((_mapWidth, _mapHeight));
        _unitPlacer.LevelLoadInitialize((_mapWidth, _mapHeight));
        obstructedSquares.Add(new Vector2Int(4, 2));
        obstructedSquares.Add(new Vector2Int(5, 3));
        obstructedSquares.Add(new Vector2Int(4, 4));
        EstablishBoundary();
    }

    void PlaceObstructions()
    {
        // Obstacle placement
        foreach (Vector2Int coordinate in obstructedSquares)
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
        for (int x = -1; x <= _mapWidth; x++)
        {
            obstructedSquares.Add(new Vector2Int (x, -1)); // bottom
            obstructedSquares.Add(new Vector2Int (x, _mapHeight)); // top
        }
        for (int y = 0; y < _mapHeight; y++)
        {
            obstructedSquares.Add(new Vector2Int (-1, y)); // left
            obstructedSquares.Add(new Vector2Int (_mapWidth, y)); // right
        }
        // Now spawn obstructions
        PlaceObstructions();
    }
    void PlaceObstacle(float xSpawnPoint, float ySpawnPoint)
    {
        Instantiate(_obstaclePrefab, new Vector2(xSpawnPoint, ySpawnPoint), Quaternion.identity);
    }
}
