using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class TilesManager : MonoBehaviour
{
    private List <GameObject> _tilesList;
    private GameObject _tilePrefab;
    private Tile _tile;

    private void Start()
    {
        _tilesList = new List<GameObject>();
        _tilePrefab = GameObject.Find("TilePrefab");    }
    public void GenerateGrid(int _width, int _height)
    { // This method creates tiles from the point 0, 0 until reaching the
    // Given width and height arguements when the method is called
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                GameObject spawnedTile = Instantiate(_tilePrefab, new Vector2(x, y),Quaternion.identity);
                _tilesList.Add(spawnedTile); // Add tiles to list for eventual removal
                spawnedTile.name = $"Tile ({x}, {y})";
            }
        }
    }

    public void RemoveTiles()
    { // This method simply removes all tiles that have been created
        for (int i = _tilesList.Count - 1; i >= 0; i--)
        { // Iterate backwards to prevent errors from removal of items mid-iteration
            GameObject tile = _tilesList[i];
            _tilesList.Remove(tile);
            Destroy(tile);
        }
    }
}
