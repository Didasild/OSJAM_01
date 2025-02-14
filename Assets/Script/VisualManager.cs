using System.Collections.Generic;
using Dida.Rendering;
using UnityEngine;
using Debug = UnityEngine.Debug;
using NaughtyAttributes;
using UnityEngine.U2D;

public class VisualManager : MonoBehaviour
{
    #region PARAMETERS
    public SpriteAtlas spriteAtlas;
    private readonly Dictionary<string, Sprite> spriteDictionary = new Dictionary<string, Sprite>();
    [ReadOnly] public Sprite[] sprites; 
    
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
    public bool inEditorScene;
    [Header("_______CELL EDITOR VISUAL")] 
    [ShowIf("inMainScene")] public Sprite coverSprite;
    [ShowIf("inMainScene")] public Sprite revealSprite;
    [ShowIf("inMainScene")] public Sprite mineIconSprite;
    
    [HideInInspector] public VisualSettings visualSettings;
    #endregion

    #region INIT

    public void Init()
    {
        if (GameManager.roomVisualManager.mainColorsVolume.profile.TryGet(out visualSettings)) { }
        LoadSprites();
    }
    private void LoadSprites()
    {
        sprites = new Sprite[spriteAtlas.spriteCount];
        spriteAtlas.GetSprites(sprites);
        foreach (Sprite sprite in sprites)
        {
            spriteDictionary[sprite.name.Replace("(Clone)", "")] = sprite;
        }
    }
    #endregion INIT
    
    public Sprite GetSprite(string spriteName)
    {
        
        if (spriteDictionary.TryGetValue(spriteName, out Sprite sprite))
        {
            return sprite;
        }
        else
        {
            return null;
        }
    }
    
    #region DEPRECATED GET CELLS VISUALS

    
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
