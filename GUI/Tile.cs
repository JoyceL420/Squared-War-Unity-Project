using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite _inactive;
    [SerializeField] private Sprite _active;
    [SerializeField] private Sprite _attacked;
    [SerializeField] private Sprite _eliminated;
    private Vector2 _mousePosition;
    public bool _highlight;
    public bool _attackedOn;
    public bool _eliminatedOn;
    private float _roundedMouseXPos;
    private float _roundedMouseYPos;
    
    private void Start()
    {
        _highlight = false;
        _attackedOn = false;
        _eliminatedOn = false;
    }
    private void Update()
    {
        if (_eliminatedOn)
        {
            _spriteRenderer.sprite = _eliminated;
            // Debug.Log("tile set to eliminated");
        }
        else if (_attackedOn)
        {
            _spriteRenderer.sprite = _attacked;
            // Debug.Log("tile set to attacked");
        }
        else if (CheckMousePos())
        {
            _spriteRenderer.sprite = _active;
        }   
        else if (_highlight)
        {
            _spriteRenderer.sprite = _active;
        }
        else
        {
            _spriteRenderer.sprite = _inactive;
        }
    }

    private bool CheckMousePos()
    {
        _mousePosition = (Vector3)GetMousePos();
        _roundedMouseXPos = Mathf.Round(_mousePosition.x);
        _roundedMouseYPos = Mathf.Round(_mousePosition.y);
        if (_roundedMouseXPos == transform.position.x && _roundedMouseYPos == transform.position.y)
        {
            return true;
        }
        return false;
    }
    
    private Vector2 GetMousePos()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector2 mousePos2D = Camera.main.ScreenToWorldPoint(mousePos);
        return mousePos2D;
    }

    public void ResetTile()
    {
        _highlight = false;
        _attackedOn = false;
        _eliminatedOn = false;
    }
}
