using UnityEngine;

public class CellVisualManager : MonoBehaviour
{
    #region PARAMETERS
    [Header("_______CELL VISUAL")]
    [Header("ITEMS VISUAL")]
    public Sprite stairSprite;
    public Sprite potionSprite;
    public Sprite swordSprite;

    [Header("CELL STATE VISUAL")]
    public Sprite clickedSprite;
    public Sprite flagSprite;
    public Sprite plantedSwordSprite;

    [Header("CELL TYPE VISUAL")]
    public Sprite stairType;

    [Header("_______CELL ANIMATIONS")]
    public GameObject mineExplosionAnimation;
    public GameObject mineSwordedAnimation;
    public GameObject plantedSwordAnimation;
    #endregion


    #region GET CELLS VISUALS
    public Sprite GetTypeVisual(CellType cellType)
    {
        Sprite cellTypeVisual = null;
        if (cellType == CellType.Gate)
        {
            cellTypeVisual = stairType;
        }
        return cellTypeVisual;
    }

    public Sprite GetStateVisual(CellState cellState)
    {
        Sprite cellStateVisual = null;
        if (cellState == CellState.None || cellState == CellState.Reveal || cellState == CellState.Cover) 
        {
            return null;
        }
        else if (cellState == CellState.Clicked)
        {
            cellStateVisual = clickedSprite;
        }
        else if (cellState == CellState.Flag)
        {
            cellStateVisual = flagSprite;
        }
        else if (cellState == CellState.PlantedSword)
        {
            cellStateVisual = plantedSwordSprite;
        }
        return cellStateVisual;
    }

    public Sprite GetItemVisuel(ItemTypeEnum itemType)
    {
        Sprite spriteItemVisual = null;
        if (itemType == ItemTypeEnum.None)
        {
            return null;
        }
        else if (itemType == ItemTypeEnum.Potion)
        {
            spriteItemVisual = potionSprite;
        }
        else if (itemType == ItemTypeEnum.Sword)
        {
            spriteItemVisual = swordSprite;
        }
        return spriteItemVisual;
    }
    #endregion

}
