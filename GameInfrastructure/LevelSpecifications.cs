using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSpecifications : MonoBehaviour
{
    private List<(float x, float y)> obstructedSquares;

    // Start is called before the first frame update
    void Start()
    {
        // This script saves the specifications of each level with the different units and
        // Battle elements and what kinds of elements/units there are
        // This will be interpreted by different scripts
        SampleLevel();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void SampleLevel()
    { // Sample level for testing
        // This is the list of positions that are "obstructed" meaning units can't pass through it

        obstructedSquares.Add((4, 1));
    }
    void ClearLists()
    {
        obstructedSquares.Clear();
    }
}
