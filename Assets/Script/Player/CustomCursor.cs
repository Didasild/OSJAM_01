using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CustomCursor : MonoBehaviour
{
    public Image cursorRenderer;
    private Material customCursorMaterial;

    public void Init()
    {
        Cursor.visible = false;
        customCursorMaterial = cursorRenderer.material;
    }

    private void Update()
    {
        Vector2 mousePosition = Input.mousePosition;
        cursorRenderer.transform.position = mousePosition;
    }
}
