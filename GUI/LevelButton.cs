using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class LevelButton : MonoBehaviour
{
    [SerializeField] private int _levelToLoad;
    private SpriteRenderer _spriteRenderer;
    private Vector2 _mousePosition;
    private float _roundedMouseXPos;
    private float _roundedMouseYPos;
    private LevelBuilder _levelBuilder;
    private Collider2D _collider;
    [SerializeField] private Color _hovering;
    [SerializeField] private Color _default;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        GameObject LevelBuilder = GameObject.Find("Main Camera/Game Manager");
        _levelBuilder = LevelBuilder.GetComponent<LevelBuilder>();
    }


    private void Update()
    {
        if (CheckMousePos())
        {
            _spriteRenderer.color = _hovering;
        }
        else
        {
            _spriteRenderer.color = _default;
        }
    }

    void OnMouseDown()
    {
        CheckMouseOverlap();
    }

    private void CheckMouseOverlap()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        if (_collider.OverlapPoint(mouseWorldPosition))
        {
            _levelBuilder.LoadLevel(_levelToLoad);
        }
    }

    private Vector2 GetMousePos()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector2 mousePos2D = Camera.main.ScreenToWorldPoint(mousePos);
        return mousePos2D;
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


}
