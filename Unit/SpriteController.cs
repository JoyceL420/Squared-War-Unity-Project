using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class SpriteController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite _blueFootSoldier;
    [SerializeField] private Sprite _redFootSoldier;
    [SerializeField] private Sprite _blueCavalier;
    [SerializeField] private Sprite _redCavalier;
    [SerializeField] private Sprite _blueRogue;
    [SerializeField] private Sprite _redRogue;
    [SerializeField] private Sprite _blueArcher;
    [SerializeField] private Sprite _redArcher;
    [SerializeField] private Sprite _blueMage;
    [SerializeField] private Sprite _redMage;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void InitializeSprite(int SpriteId)
    {
        // Debug.Log(SpriteId);
        switch (SpriteId)
        {
            case 0: // Blue foot soldier
                _spriteRenderer.sprite = _blueFootSoldier;
                break;
            case 1: // Red foot soldier
                _spriteRenderer.sprite = _redFootSoldier;
                break; 
            case 2: // Blue cavalier
                _spriteRenderer.sprite = _blueCavalier;
                break; 
            case 3: // Red cavalier
                _spriteRenderer.sprite = _redCavalier;
                break; 
            case 4: // Blue rogue
                _spriteRenderer.sprite = _blueRogue;
                break; 
            case 5: // Red rogue
                _spriteRenderer.sprite = _redRogue;
                break; 
            case 6: // Blue archer
                _spriteRenderer.sprite = _blueArcher;
                break; 
            case 7: // Red archer
                _spriteRenderer.sprite = _redArcher;
                break; 
            case 8: // Blue mage
                _spriteRenderer.sprite = _blueMage;
                break; 
            case 9: // Red mage
                _spriteRenderer.sprite = _redMage;
                break; 
            default: // Invalid case
                Debug.LogError("Unit has no sprite for given id, defaulting to 0");
                _spriteRenderer.sprite = _blueFootSoldier;
                break;
        }
    }
}
