using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FootSoldierDragPlacer : MonoBehaviour
{
    [SerializeField] private GameObject cameraPosition;
    private bool dragging;
    private Vector3 mousePos;
    private Vector2 mousePos2D;
    private float roundedXPos;
    private float roundedYPos;
    private TeamController _teamsController;
    private (int mapWidth, int mapHeight) _mapSize;
    public List<(float x, float y)> occupiedSquares;
    private int _team; // Testing variable remove when unneeded
    // Start is called before the first frame update
    void Start()
    {
        _team = 0;
        dragging = false;
        GameObject TeamsController = GameObject.Find("TeamsController");
        _teamsController = TeamsController.GetComponent<TeamController>();
        occupiedSquares = new List<(float x, float y)>();
    }
    public void LevelLoadInitialize ((int, int) MapSize)
    {
        _mapSize = MapSize;
    }
    void Update()
    {
        if (dragging == false)
        {
            transform.position = new Vector2 (cameraPosition.transform.position.x - 9, cameraPosition.transform.position.y - 4);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _team = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _team = 0;
        }
    }   
    // Utilising collider2D to detect mouse clicks
    void OnMouseDown()
    {
        dragging = true;
    }

    void OnMouseDrag()
    {
        if (dragging)
        {
            transform.position = (Vector3)GetMousePos();
        }
    }

    void OnMouseUp()
    {
        dragging = false;
        // Check position and place unit in accordance to whichever grid tile is highlighted
        roundedXPos = Mathf.Round(transform.position.x);
        roundedYPos = Mathf.Round(transform.position.y);
        // If there are no units that have been spawned yet (this system should probably be redone)
        if (_teamsController.occupiedSquares.Count == 0)
        {
            _teamsController.occupiedSquares.Add((roundedXPos, roundedYPos));
            SummonUnit(roundedXPos, roundedYPos);
        }
        // Check if that square has already been used to place a unit or the spawn point is outside the allowed boundary
        else if (CheckObstruction())
        {
            _teamsController.occupiedSquares.Add((roundedXPos, roundedYPos));
            SummonUnit(roundedXPos, roundedYPos);  
        }
    }
    private bool CheckObstruction()
    {
        foreach (var coordinate in _teamsController.occupiedSquares)
        { // If the location where a unit is trying to be spawned in has been occupied
            if (coordinate.x == roundedXPos && coordinate.y == roundedYPos)
            { // Prevent the spawn
                Debug.Log("Spawn has been prevented");
                return false;
            }
        } 
        // Check for whether the spawn is taking place outside the boundaries
        if (roundedXPos > _mapSize.mapWidth -1 || roundedXPos < 0 || roundedYPos > _mapSize.mapHeight - 1 || roundedYPos < 0)
        {
            Debug.Log("Spawn has been prevented");
            return false;
        }
        // Otherwise it will be allowed
        return true;
    }
    void SummonUnit(float xSpawnPoint, float ySpawnPoint)
    {
        _teamsController.CloneCavalier(xSpawnPoint, ySpawnPoint, _team);
    }

    Vector2 GetMousePos()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector2 mousePos2D = Camera.main.ScreenToWorldPoint(mousePos);
        return mousePos2D;
    }
}
