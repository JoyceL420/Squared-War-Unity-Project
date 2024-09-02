using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private bool isDragging = false;
    private float dragSpeed = 3.65f;
    private LevelBuilder levelBuilder;
    private (int x, int y) _mapSize;
    private bool isActive;
    private Vector3 dragOrigin;
    private Vector3 dragDifference;

    public void LevelLoadInitialize((int, int) mapSize)
    {
        _mapSize = mapSize;
        isActive = true;
        transform.position = new Vector3 (_mapSize.x / 2, _mapSize.y / 2, -10);
    }

    public void LevelUnload()
    {
        isActive = false;
        transform.position = new Vector3 (0, 0, -10);
    }
    void Update()
    {
        // If mouse3 (middle mouse button) is being pressed
        if (Input.GetMouseButtonDown(2))
        {
            isDragging = true;
            dragOrigin = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(2))
        {
            isDragging = false;
        }
        if (isDragging && isActive)
        {
            // Determine the difference in position from the origin of the drag and the current mouse position
            dragDifference = Input.mousePosition - dragOrigin;
            // Update position of the camera
            transform.position -= dragDifference * Time.deltaTime * dragSpeed;
            // Update drag origin to prevent light-speed travel
            dragOrigin = Input.mousePosition;
        }
        if (transform.position.x < 1 && isActive)
        {
            transform.position = new Vector3(1, transform.position.y, -10);
        }
        if (transform.position.x > _mapSize.x && isActive)
        {
            transform.position = new Vector3(_mapSize.x, transform.position.y, -10);
        }
        if (transform.position.y < 1 && isActive)
        {
            transform.position = new Vector3(transform.position.x, 1, -10);
        }
        if (transform.position.y > _mapSize.y && isActive)
        {
            transform.position = new Vector3(transform.position.x, _mapSize.y, -10);
        }
    }
}
