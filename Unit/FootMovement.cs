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
        _unit = GetComponent<unit>();
    }
    public void Move(int direction)
    {
        _unit.MoveTile(direction);
    }
}
