using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class UnitsCounter : MonoBehaviour
{
    private TMP_Text textMeshPro;
    // Start is called before the first frame update
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
