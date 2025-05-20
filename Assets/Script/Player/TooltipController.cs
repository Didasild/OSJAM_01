using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TooltipController : MonoBehaviour
{
    public static TooltipController Instance;

    public GameObject tooltipPanel;
    public TextMeshProUGUI tooltipText;
    
    [Serializable]
    public struct CellToolTip
    {
        public CellType cellType;
        public CellState cellState;
        public string tooltipText;
    }
    public List<CellToolTip> CellTooltips;

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

    public void CheckCellTooltip(Cell cellOver)
    {
        foreach (CellToolTip tooltip in CellTooltips)
        {
            if (cellOver.currentType == tooltip.cellType && cellOver.currentState == tooltip.cellState)
            {
                ShowTooltip(tooltip.tooltipText);
                return;
            }
        }

        if (cellOver.currentState == CellState.PlantedSword)
        {
            ShowTooltip("DESTROY.");
            return;
        }
        
        HideTooltip(); //Si aucun condition ne correspond
    }
}