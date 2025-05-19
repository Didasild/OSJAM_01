using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("TOOLTIP")]
    public string tooltipText;

    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipSystem.ShowTooltip(tooltipText);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipSystem.HideTooltip();
    }
}
