using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class SpriteController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite _blueFootSoldier;
    [SerializeField] private Sprite _redFootSoldier;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void InitializeSprite(int SpriteId)
    {
        Debug.Log(SpriteId);
        switch (SpriteId)
        {
            case 0: // Blue foot soldier
                _spriteRenderer.sprite = _blueFootSoldier;
                break;
            case 1: // Red foot soldier
                _spriteRenderer.sprite = _redFootSoldier;
                break; 
            default: // Invalid case
                Debug.LogError("Unit has no sprite for given id, defaulting to 0");
                _spriteRenderer.sprite = _blueFootSoldier;
                break;
        }
    }
}
