using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class LevelData : MonoBehaviour
{
    public List<Vector2Int> GetObstaclesForLevel(int level)
    {
        List<Vector2Int> _listToReturn = new List<Vector2Int>(); 
        switch (level)
        {
            case 0: // Debug level
                _listToReturn.Add(new Vector2Int(4, 2));
                _listToReturn.Add(new Vector2Int(3, 2));
                _listToReturn.Add(new Vector2Int(4, 3));
                _listToReturn.Add(new Vector2Int(4, 4));
                _listToReturn.Add(new Vector2Int(3, 4));
                _listToReturn.Add(new Vector2Int(4, 6));
                _listToReturn.Add(new Vector2Int(4, 7));
                _listToReturn.Add(new Vector2Int(4, 8));
                break;
            default:
                // Loads debug level in case of broken level
                level = 0;
                _listToReturn.Add(new Vector2Int(4, 2));
                _listToReturn.Add(new Vector2Int(3, 2));
                _listToReturn.Add(new Vector2Int(4, 3));
                _listToReturn.Add(new Vector2Int(4, 4));
                _listToReturn.Add(new Vector2Int(3, 4));
                _listToReturn.Add(new Vector2Int(4, 6));
                _listToReturn.Add(new Vector2Int(4, 7));
                _listToReturn.Add(new Vector2Int(4, 8));
                break;
        }
        return _listToReturn;
    }
    public List<(Vector2Int position, int type)> GetUnitsForLevel(int level)
    { // DO NOT USE IN CURRENT STATE
        return null;
    }
}
