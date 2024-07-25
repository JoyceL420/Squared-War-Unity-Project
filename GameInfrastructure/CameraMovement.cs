using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private bool isDragging = false;
    private float dragSpeed = 3.65f;
    private Vector3 dragOrigin;
    private Vector3 dragDifference;
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
        if (isDragging)
        {
            // Determine the difference in position from the origin of the drag and the current mouse position
            dragDifference = Input.mousePosition - dragOrigin;
            // Update position of the camera
            transform.position -= dragDifference * Time.deltaTime * dragSpeed;
            // Update drag origin to prevent light-speed travel
            dragOrigin = Input.mousePosition;
        }
    }
}
