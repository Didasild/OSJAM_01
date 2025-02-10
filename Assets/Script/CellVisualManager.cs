using Dida.Rendering;
using UnityEngine;
using Debug = UnityEngine.Debug;
using NaughtyAttributes;

public class CellVisualManager : MonoBehaviour
{
    #region PARAMETERS
    [Header("_______CELL VISUAL")]
    [Header("ITEMS VISUAL")]
    public Sprite potionSprite;
    public Sprite swordSprite;

    [Header("CELL STATE VISUAL")]
    public Sprite inactiveSprite;
    public Sprite clickedSprite;
    public Sprite flagSprite;
    public Sprite plantedSwordSprite;

    [Header("CELL TYPE VISUAL")]
    public Sprite stairType;

    [Header("_______CELL ANIMATIONS")]
    public GameObject mineExplosionAnimation;
    public GameObject mineSwordedAnimation;
    public GameObject plantedSwordAnimation;
    
    [Header("_______OTHER")] 
    public bool inMainScene;
    [Header("_______CELL EDITOR VISUAL")] 
    [HideIf("inMainScene")] public Sprite coverSprite;
    [HideIf("inMainScene")] public Sprite revealSprite;
    [HideIf("inMainScene")] public Sprite mineIconSprite;
    
    [HideInInspector] public VisualSettings visualSettings;
    #endregion

    #region INIT

    public void Init()
    {
        if (GameManager.roomVisualManager.mainColorsVolume.profile.TryGet(out visualSettings)) { }
    }
    

    #endregion INIT

    #region GET CELLS VISUALS
    public Sprite GetCellTypeVisual(CellType cellType)
    {
        Sprite cellTypeVisual = null;
        if (cellType == CellType.Gate)
        {
            cellTypeVisual = stairType;
        }
        return cellTypeVisual;
    }

    public Sprite GetCellStateVisual(CellState cellState)
    {
        Sprite cellStateVisual = null;
        if (cellState == CellState.Reveal || cellState == CellState.Cover) 
        {
            return null;
        }
        else if (cellState == CellState.Inactive)
        {
            cellStateVisual = inactiveSprite;
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

    public Sprite GetCellItemVisuel(ItemTypeEnum itemType)
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

    public Color GetElementColor(int colorIndex)
    {
        Color returnedColor = default;
        switch (colorIndex)
        {
            case 1:
                returnedColor = visualSettings.Color1.value;
                break;
            case 2:
                returnedColor = visualSettings.Color2.value;
                break;
            case 3:
                returnedColor = visualSettings.Color3.value;
                break;
            case 4:
                returnedColor = visualSettings.Color4.value;
                break;
            case 5:
                returnedColor = visualSettings.Color5.value;
                break;
            default:
                Debug.LogWarning("Index de couleur invalide : " + colorIndex);
                break;
        }
        return returnedColor;
    }
    #endregion

}
