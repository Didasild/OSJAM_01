using TMPro;
using UnityEngine;

public class TooltipSystem : MonoBehaviour
{
    public static TooltipSystem Instance;

    public GameObject tooltipPanel;
    public TextMeshProUGUI tooltipText;

    public void Init()
    {
        Instance = this;
        HideTooltip();
    }

    public static void ShowTooltip(string content)
    {
        Instance.tooltipText.text = content;
        Instance.tooltipPanel.SetActive(true);
    }

    public static void HideTooltip()
    {
        Instance.tooltipPanel.SetActive(false);
    }
}