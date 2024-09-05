using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelSelectionText : MonoBehaviour
{
    private TMP_Text textMeshPro;
    void Start()
    {
        textMeshPro = GetComponent<TMP_Text>();
    }
    public void UpdateText(string updatedText)
    {
        textMeshPro.text = updatedText;
    }

    public void ResetText()
    {
        textMeshPro.text = "";
    }
}
