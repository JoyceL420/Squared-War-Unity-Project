using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMovementTest : MonoBehaviour
{
    // This script is to give me (the developer) a "unit" to move and do test cases with
    // E.g. blocking movement with battle elements or eliminating units.
    // This script may not be used in the final version/sent version of this game but
    // May be repurposed as a controllable unit.


    [SerializeField] private float xPos;
    private string unitType;
    [SerializeField] private float yPos;
    [SerializeField] private int speed;
    [SerializeField] private int direction;
    // private string debugPrint; <I may use this variable to convert the list of tuples into a singular string for debug printing
    private List<(float x, float y)> passedSquares;
    // Start is called before the first frame update
    void Start()
    {
        // Initialisation of variables
        unitType = "player";
        xPos = transform.position.x;
        yPos = transform.position.y;
        speed = 1;
        direction = 0;
        passedSquares = new List<(float, float)>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
    }

    // Every frame this method is checking whether a key was pressed down in the same frame
    void CheckInput() {
        if (Input.GetKeyDown(KeyCode.W) ||Input.GetKeyDown(KeyCode.UpArrow))
        { 
            direction = 1;
            MovementPulse(speed);
            PrintAffectedTiles();
            passedSquares.Clear();
        }
        if (Input.GetKeyDown(KeyCode.D) ||Input.GetKeyDown(KeyCode.RightArrow))
        { 
            direction = 3;
            MovementPulse(speed);
            PrintAffectedTiles();
            passedSquares.Clear();
        }
        if (Input.GetKeyDown(KeyCode.S) ||Input.GetKeyDown(KeyCode.DownArrow))
        { 
            direction = 5;
            MovementPulse(speed);
            PrintAffectedTiles();
            passedSquares.Clear();
        }
        if (Input.GetKeyDown(KeyCode.A) ||Input.GetKeyDown(KeyCode.LeftArrow))
        { 
            direction = 7;
            MovementPulse(speed);
            PrintAffectedTiles();
            passedSquares.Clear();
        }
        
        // if (Input.GetKeyDown(KeyCode.Space))
        // { 
            // direction += 1;
            // MovementPulse(speed);
            // PrintAffectedTiles();
            // passedSquares.Clear();
        // }
    }

    void MovementPulse(int timesToMove) {
        while (timesToMove >= 1)
        { // Moves as many times as specified by the speed variable when this method is called
            if (direction == 1) 
            {
                // Up
                transform.position = new Vector2(transform.position.x, transform.position.y + 1);
            }
            if (direction == 2) 
            {
                // Up right
                transform.position = new Vector2(transform.position.x + 1, transform.position.y + 1);
            }
            if (direction == 3) 
            {
                // Right
                transform.position = new Vector2(transform.position.x + 1, transform.position.y);
            }
            if (direction == 4) 
            {
                // Down right
                transform.position = new Vector2(transform.position.x + 1, transform.position.y - 1);
            }
            if (direction == 5) 
            {
                // Down
                transform.position = new Vector2(transform.position.x , transform.position.y - 1);
            }
            if (direction == 6) 
            {
                // Down left
                transform.position = new Vector2(transform.position.x - 1, transform.position.y - 1);
            }
            if (direction == 7) 
            {
                // Left
                transform.position = new Vector2(transform.position.x - 1, transform.position.y);
            }
            if (direction == 8) 
            {
                // Up left
                transform.position = new Vector2(transform.position.x - 1, transform.position.y + 1);
            }
            if (direction > 8)
            {
                Debug.Log("MOVEMENT FAILURE (DIRECTION VARIABLE OVERLOAD) UNIT " + unitType);            
            }
            UpdatePosition();
            // Adds the tiles moved into to a list  
            passedSquares.Add((xPos, yPos));
            // Change the times to move variable
            timesToMove -= 1;
        }
    }

    void PrintAffectedTiles()
    { // Debug method for checking the current tiles moved into/across
        foreach (var coordinate in passedSquares)
        {
            Debug.Log("X: " + coordinate.x + "Y: " + coordinate.y);
        }
    }
    void AddAffectedTile()
    {
        // This is a method that may be unique for different units e.g. a wizard attacking in L shapes in front of him will also have his "affected" tiles recorded here.
        passedSquares.Add((xPos, yPos));
    }
    void UpdatePosition() 
    { // Just updates the xPos and yPos variables for ease of use
        xPos = transform.position.x;
        yPos = transform.position.y;
    }
}
