using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class TilesManager : MonoBehaviour
{
    private List <GameObject> _tilesList;
    private List <(GameObject tile, int xPos, int yPos)> _tilesAndPositionsList;
    private GameObject _tilePrefab;
    private Tile _tile;

    private void Awake()
    {
        _tilesList = new List<GameObject>(); 
        _tilesAndPositionsList = new List<(GameObject tile, int xPos, int yPos)>();
        _tilePrefab = GameObject.Find("TilePrefab");   
    }
    public void GenerateGrid(int _width, int _height)
    { // This method creates tiles from the point 0, 0 until reaching the
    // Given width and height arguements when the method is called
        
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                GameObject spawnedTile = Instantiate(_tilePrefab, new Vector3(x + 1, y + 1, 10),Quaternion.identity);
                _tilesList.Add(spawnedTile); // Add tiles to list for eventual removal
                _tilesAndPositionsList.Add((spawnedTile, x + 1, y + 1));
                spawnedTile.name = $"Tile ({x + 1}, {y + 1})";
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
    public void SetTileToAttacked(int x, int y)
    {
        foreach (var tileFromList in _tilesAndPositionsList)
        {
            if (tileFromList.xPos == x && tileFromList.yPos == y)
            {
                Tile _tile = tileFromList.tile.GetComponent<Tile>();
                _tile._attackedOn = true;
            }
        }
    }

    public void SetTileToEliminated(int x, int y)
    {
        foreach (var tileFromList in _tilesAndPositionsList)
        {
            if (tileFromList.xPos == x && tileFromList.yPos == y)
            {
                Tile _tile = tileFromList.tile.GetComponent<Tile>();
                _tile._eliminatedOn = true;
            }
        }
    }
    public void ResetTiles()
    {
        foreach (GameObject tile in _tilesList)
        {
            Tile tileReference = tile.GetComponent<Tile>();
            tileReference.ResetTile();
        }
    }
}
