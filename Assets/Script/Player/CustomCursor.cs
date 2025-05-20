using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CustomCursor : MonoBehaviour
{
    public Image cursorRenderer;
    [FormerlySerializedAs("tooltipSystem")] public TooltipController tooltipController;

    public void Init()
    {
        Cursor.visible = false;
        
        tooltipController = gameObject.GetComponent<TooltipController>();
        if (tooltipController != null)
        {
            tooltipController.Init();
        }
        else
        {
            Debug.LogWarning("CustomCursor could not find TooltipController");
        }

    }

    private void LateUpdate()
    {
        Vector2 mousePosition = Input.mousePosition;
        cursorRenderer.transform.position = mousePosition;
    }
}
