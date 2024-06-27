using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    // TEMPORARILY ASSIGNED TO Main Camera GAMEOBJECT FOR TESTING
    // WILL BE ASSIGNED TO AN EMPTY OBJECT LATER
    private TeamController _teamsController; 
    public GameObject obstaclePrefab;
    private List <(float x, float y)> obstructedSquares;
    // Start is called before the first frame update
    void Start()
    {
        obstructedSquares = new List<(float x, float y)>();
        // This Script gets the data from LevelSpecifications based on what level was chosen
        // And places the units and battle elements and tells them their starting positions
        // 
        GameObject TeamsController = GameObject.Find("TeamsController");
        _teamsController = TeamsController.GetComponent<TeamController>();
        obstructedSquares.Add((4, 2));
        obstructedSquares.Add((5, 3));
        obstructedSquares.Add((4, 4));
        BuildLevel();
    }

    void BuildLevel()
    {
        // Obstacle placement
        foreach (var coordinate in obstructedSquares)
        {
            PlaceObstacle(coordinate.x, coordinate.y);
            _teamsController.obstructedSquares.Add((coordinate.x, coordinate.y));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void PlaceObstacle(float xSpawnPoint, float ySpawnPoint)
    {
        Instantiate(obstaclePrefab, new Vector2(xSpawnPoint, ySpawnPoint), Quaternion.identity);
    }
}
