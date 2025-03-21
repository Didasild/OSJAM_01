using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CustomCursor : MonoBehaviour
{
    public Image cursorRenderer;

    public void Init()
    {
        Cursor.visible = false;
    }

    private void Update()
    {
        Vector2 mousePosition = Input.mousePosition;
        cursorRenderer.transform.position = mousePosition;
    }
}
