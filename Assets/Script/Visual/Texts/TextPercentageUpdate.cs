using System;
using TMPro;
using UnityEngine;

public class TextPercentageUpdate : MonoBehaviour
{
    [Header("UI Elements")]
    public RectTransform fillRect;               
    public float maxWidth = 811f;                
    public TextMeshProUGUI percentageText;

    private void OnEnable()
    {
        if (percentageText != null)
        {
            percentageText = GetComponent<TextMeshProUGUI>();
        }
    }

    private void Update()
    {
        float currentWidth = fillRect.rect.width;
        float progress = Mathf.Clamp01(currentWidth / maxWidth); // 0f à 1f
        int percent = Mathf.RoundToInt(progress * 100f);
        percentageText.text = percent + "%";
    }
}
