using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite _inactive;
    [SerializeField] private Sprite _active;
    private Vector2 _mousePosition;
    private float _roundedMouseXPos;
    private float _roundedMouseYPos;

    private void Update()
    {
        _mousePosition = (Vector3)GetMousePos();
        _roundedMouseXPos = Mathf.Round(_mousePosition.x);
        _roundedMouseYPos = Mathf.Round(_mousePosition.y);
        if (_roundedMouseXPos == transform.position.x && _roundedMouseYPos == transform.position.y)
        {
            _spriteRenderer.sprite = _active;
        }
        else
        {
            _spriteRenderer.sprite = _inactive;
        }
    }
    
    Vector2 GetMousePos()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector2 mousePos2D = Camera.main.ScreenToWorldPoint(mousePos);
        return mousePos2D;
    }
}
