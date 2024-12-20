using UnityEngine;

[ExecuteInEditMode]
public class CellEditor : MonoBehaviour
{
    public CellState cellState;
    public CellType cellType;
    public ItemTypeEnum itemType;

    public SpriteRenderer cellStateVisual;
    
    [HideInInspector]
    public Vector2Int gridPosition; // La position dans la grille

    private void OnValidate()
    {
        
    }

    public void ResetName()
    {
        gameObject.name = $"Cell ({gridPosition.x}, {gridPosition.y})";
    }
}
