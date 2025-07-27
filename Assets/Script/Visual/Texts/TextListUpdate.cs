using System;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class TextListUpdate : MonoBehaviour
{
    [SerializeField] private List<String> lines;
    public TextMeshProUGUI textToUpdate;
    private int currentLine = 0;

    public void Start()
    {
        textToUpdate.text = string.Empty;
    }

    public void UpdateToNextLine()
    {
        if (lines == null || lines.Count == 0 || textToUpdate == null)
            return;
        
        textToUpdate.text = lines[currentLine];
        currentLine = (currentLine + 1) % lines.Count;
    }
}
