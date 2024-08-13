using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FootMovement : MonoBehaviour
{
    private unit _unit;
    private void Start()
    {
        _unit = GetComponent<unit>();
    }
    public void Move(int direction)
    { // Allows movement for given direction
        _unit.MoveTile(direction);
    }
}
