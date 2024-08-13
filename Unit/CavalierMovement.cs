using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CavalierMovement : MonoBehaviour
{
    private unit _unit;
    private void Start()
    {
        _unit = GetComponent<unit>();
    }
    public void Move(int direction)
    { // Allows movement for given direction
        if (_unit.GetDirectionPreviouslyMoved() == 0 || direction == _unit.GetDirectionPreviouslyMoved())
        {
            _unit.MoveTile(direction);
            _unit.SetDirectionPreviouslyMoved(direction);
        }
        else
        {
            Debug.Log("Movement blocked");
        }
    }
}
