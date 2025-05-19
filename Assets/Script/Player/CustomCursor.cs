using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CustomCursor : MonoBehaviour
{
    public Image cursorRenderer;
    public TooltipSystem tooltipSystem;

    public void Init()
    {
        Cursor.visible = false;
        
        tooltipSystem = gameObject.GetComponent<TooltipSystem>();
        if (tooltipSystem != null)
        {
            tooltipSystem.Init();
        }
        else
        {
            Debug.LogWarning("CustomCursor could not find TooltipSystem");
        }

    }

    private void LateUpdate()
    {
        Vector2 mousePosition = Input.mousePosition;
        cursorRenderer.transform.position = mousePosition;
    }
}
