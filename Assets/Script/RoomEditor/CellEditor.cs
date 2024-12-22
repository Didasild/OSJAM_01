using UnityEngine;
using UnityEngine.Serialization;

[ExecuteInEditMode]
public class CellEditor : MonoBehaviour
{
    [Header("CELL PARAMETERS")]
    public CellState cellState;
    public CellType cellType;
    public ItemTypeEnum itemType;
    public bool isSelected;

    [Header("CELL VISUAL")]
    public SpriteRenderer cellStateVisual;
    public SpriteRenderer cellTypeVisual;
    
    public Vector2Int _cellPosition; // La position dans la grille
    
    //PRIVATE
    private CellVisualManager _cellVisualManager;
    
    private void OnValidate()
    {
        gameObject.name = $"Cell ({cellState}, {cellType})";
        UpdateCellVisual();
    }

    public void Initialize(CellVisualManager cellVisualManager)
    {
        gameObject.name = $"Cell ({cellState}, {cellType})";
        _cellVisualManager = cellVisualManager;
        UpdateCellVisual();
        foreach (Transform child in transform)
        {
            child.gameObject.hideFlags = HideFlags.HideInHierarchy; // Rend l'objet non sélectionnable
        }
    }
    
    public void UpdateCellVisual()
    {
        switch (cellState)
        {
            case CellState.Cover:
                cellStateVisual.sprite = _cellVisualManager.coverSprite;
                break;
            case CellState.Reveal:
                cellStateVisual.sprite = _cellVisualManager.revealSprite;
                break;
        }

        switch (cellType)
        {
            case CellType.None:
                cellStateVisual.sprite = null;
                cellTypeVisual.sprite = null;
                break;
            case CellType.Empty:
                cellTypeVisual.sprite = null;
                break;
            case CellType.Mine:
                cellTypeVisual.sprite = _cellVisualManager.mineIconSprite;
                break;
            case CellType.Gate:
                cellTypeVisual.sprite = _cellVisualManager.stairType;
                break;
            case CellType.Item:
                UpdateItemVisual(itemType);
                break;
        }
        Debug.Log($"Sprite mis à jour pour l'état : {cellState}");
    }

    public void UpdateItemVisual(ItemTypeEnum itemType)
    {
        switch (itemType)
        {
            case ItemTypeEnum.None:
                cellTypeVisual.sprite = null;
                break;
            case ItemTypeEnum.Potion:
                cellTypeVisual.sprite = _cellVisualManager.potionSprite;
                break;
            case ItemTypeEnum.Sword:
                cellTypeVisual.sprite = _cellVisualManager.swordSprite;
                break;
            case ItemTypeEnum.Coin:
                cellTypeVisual.sprite = null;
                Debug.LogWarning("Pas de visuel pour le coin");
                break;
        }
    }

}
