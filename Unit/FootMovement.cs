using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FootMovement : MonoBehaviour
{
    private ObstructionChecker _obstructionChecker;
    private unit _unit;
    private (float x, float y) _squareToMoveTo;
    private bool _hasMoved;

    private void Start()
    {
        _obstructionChecker = GetComponent<ObstructionChecker>();
        _unit = GetComponent<unit>();
        _squareToMoveTo = (0, 0);
    }

    public void Move(List<int> movementPriority, List<Vector2Int> obstructedSquares, float xPos, float yPos)
    {
        // Resets variables necessary for checks
        _squareToMoveTo = (0, 0);
        Movement(movementPriority, obstructedSquares, xPos, yPos);
    }
    private void Movement(List<int> movementPriority, List<Vector2Int> obstructedSquares, float xPos, float yPos)
    {
        foreach (int direction in movementPriority)
        { // Iterate direction
            switch (direction)
            { // If direction == directon and that space is not obstructed run MoveTile()
                case 0:
                    // Cant move
                    _unit.cantMove = true;
                    break;
                case 1:
                    // Up
                    _squareToMoveTo = (xPos, yPos + 1);
                    if (_obstructionChecker.CheckObstruction(obstructedSquares, _squareToMoveTo) is false)
                    {
                        _unit.MoveTile(direction);
                        _hasMoved = true;
                    }
                    break;
                case 2:
                    // Up right
                    _squareToMoveTo = (xPos + 1, yPos + 1);
                    if (_obstructionChecker.CheckObstruction(obstructedSquares, _squareToMoveTo) is false)
                    {
                        _unit.MoveTile(direction);
                        _hasMoved = true;
                    }
                    break;
                case 3:
                    // Right
                    _squareToMoveTo = (xPos + 1, yPos);
                    if (_obstructionChecker.CheckObstruction(obstructedSquares, _squareToMoveTo) is false)
                    {
                        _unit.MoveTile(direction);
                        _hasMoved = true;
                    }
                    break;
                case 4:
                    // Down right
                    _squareToMoveTo = (xPos + 1, yPos - 1);
                    if (_obstructionChecker.CheckObstruction(obstructedSquares, _squareToMoveTo) is false)
                    {
                        _unit.MoveTile(direction);
                        _hasMoved = true;
                    }
                    break;
                case 5:
                    // Down
                    _squareToMoveTo = (xPos, yPos - 1);
                    if (_obstructionChecker.CheckObstruction(obstructedSquares, _squareToMoveTo) is false)
                    {
                        _unit.MoveTile(direction);
                        _hasMoved = true;
                    }
                    break;
                case 6:
                    // Down left
                    _squareToMoveTo = (xPos - 1, yPos - 1);
                    if (_obstructionChecker.CheckObstruction(obstructedSquares, _squareToMoveTo) is false)
                    {
                        _unit.MoveTile(direction);
                        _hasMoved = true;
                    }
                    break;
                case 7:
                    // Left
                    _squareToMoveTo = (xPos - 1, yPos);
                    if (_obstructionChecker.CheckObstruction(obstructedSquares, _squareToMoveTo) is false)
                    {
                        _unit.MoveTile(direction);
                        _hasMoved = true;
                    }
                    break;
                case 8:
                    // Up left
                    _squareToMoveTo = (xPos - 1, yPos + 1);
                    if (_obstructionChecker.CheckObstruction(obstructedSquares, _squareToMoveTo) is false)
                    {
                        _unit.MoveTile(direction);
                        _hasMoved = true;
                    }
                    break;
                default:
                    break;
            }
            if (_hasMoved)
            {
                _hasMoved = false;
                break;
            }
        }
    }
}
