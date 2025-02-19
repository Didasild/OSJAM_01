using System;
using System.Collections.Generic;
using DG.Tweening;
using Dida.Rendering;
using UnityEngine;
using Debug = UnityEngine.Debug;
using NaughtyAttributes;
using UnityEngine.Rendering;
using UnityEngine.U2D;

public class VisualManager : MonoBehaviour
{
    #region PARAMETERS
    public SpriteAtlas spriteAtlas;
    private readonly Dictionary<string, Sprite> spriteDictionary = new Dictionary<string, Sprite>();
    [ReadOnly] public Sprite[] sprites; 

    [Header("AMBIANCE / POST PROCESS")]
    public Volume mainColorsVolume;
    public Volume transitionColorsVolume;
    public float visualTransitionDuration;

    [Header("_______CELL ANIMATIONS")] 
    public List<GameObject> animationPrefabs;
    public GameObject mineExplosionAnimation;
    public GameObject mineSwordedAnimation;
    public GameObject plantedSwordAnimation;
    public GameObject flagInAnimation;
    
    [Header("_______MINIMAP ROOM STATE VISUAL")]
    public Sprite roomFoWSprite;
    public Sprite roomStartedSprite;
    public Sprite roomCompleteSprite;
    public Sprite roomSelectedSprite;
    
    [Header("_______MINIMAP ROOM TYPE VISUAL")]
    public Sprite roomTypeStairSprite;
    public Sprite roomTypeShopSprite;
    public Sprite roomTypeBossSprite;
    
    private VolumeProfile _roomMainProfile;
    private VisualSettings _roomTransitionVisualSettings;

    private Tweener _currentWeightTween;
    
    [Header("_______EDITOR")] 
    public bool inEditorScene;
    [Header("_______CELL VISUAL")]
    [Header("ITEMS VISUAL")]
    [ShowIf("inEditorScene")] public Sprite potionSprite;
    [ShowIf("inEditorScene")] public Sprite swordSprite;

    [Header("CELL STATE VISUAL")]
    [ShowIf("inEditorScene")]public Sprite flagSprite;
    [ShowIf("inEditorScene")]public Sprite plantedSwordSprite;

    [Header("CELL TYPE VISUAL")]
    [ShowIf("inEditorScene")]public Sprite stairType;
    
    [Header("_______CELL EDITOR VISUAL")] 
    [ShowIf("inEditorScene")] public Sprite coverSprite;
    [ShowIf("inEditorScene")] public Sprite revealSprite;
    [ShowIf("inEditorScene")] public Sprite mineIconSprite;
    
    [HideInInspector] public VisualSettings visualSettings;
    #endregion

    #region INIT

    public void Init()
    {
        if (mainColorsVolume.profile.TryGet(out visualSettings)) { }
        LoadSprites();
        _roomMainProfile = mainColorsVolume.profile;
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
            Debug.LogError($"Sprite {spriteName} not found");
            return null;
        }
    }
    
    #region GET CELLS VISUALS
   
    public Sprite GetCellTypeVisual(CellType cellType)
    {
        Sprite cellTypeVisual = null;
        if (cellType == CellType.Gate)
        {
            cellTypeVisual = GetSprite("Cell_Type_Stair");
        }
        return cellTypeVisual;
    }

    public Sprite GetCellStateVisual(CellState cellState)
    {
        Sprite cellStateVisual = null;
        switch (cellState)
        {
            case CellState.Reveal:
            case CellState.Cover:
                return null;
            case CellState.Inactive:
                cellStateVisual = GetSprite("Cell_None");
                break;
            case CellState.Clicked:
                cellStateVisual = GetSprite("Cell_State_Clicked");
                break;
            case CellState.Flag:
                cellStateVisual = GetSprite("Cell_State_Flag");
                break;
            case CellState.PlantedSword:
                cellStateVisual = GetSprite("Cell_State_Sword");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(cellState), cellState, null);
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
            spriteItemVisual = GetSprite("Cell_Item_Potion");
        }
        else if (itemType == ItemTypeEnum.Sword)
        {
            spriteItemVisual = GetSprite("Cell_Item_Sword");
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
    
    #region SET ROOM VISUAL
    public void UpdateRoomVisual(RoomData roomData)
    {
        TransitionVolume(roomData.roomSettings.roomColorsVolumeProfile);
    }

    private void TransitionVolume(VolumeProfile roomProfile = null)
    {
        //Check le volume a récup
        if (roomProfile != null)
        {
            if (roomProfile == _roomMainProfile)
            {
                return;
            }
            transitionColorsVolume.profile = roomProfile;
        }
        else
        {
            if (GameManager.Instance.currentChapterSettings.chapterDefaultColorsVolume == transitionColorsVolume.profile)
            {
                return;
            }
            transitionColorsVolume.profile = GameManager.Instance.currentChapterSettings.chapterDefaultColorsVolume;
        }
        
        // Si un tween est déjà en cours, on l'annule
        _currentWeightTween?.Kill();
        //Fait la transition si le volume est bon
        _currentWeightTween = DOWeight(transitionColorsVolume, 1f, visualTransitionDuration)
            .SetEase(Ease.Linear)
            .OnComplete(UpdateVolumeProfile);
    }
    
    private static Tweener DOWeight(Volume volume, float endValue, float duration)
    {
        return DOTween.To(() => volume.weight, x => volume.weight = x, endValue, duration);
    }

    private void UpdateVolumeProfile()
    {
        mainColorsVolume.profile = null;
        mainColorsVolume.profile = transitionColorsVolume.profile;
        transitionColorsVolume.weight = 0;
    }
    #endregion SET ROOM FUNCTIONS
    
    #region GET ROOM FUNCTIONS
    public Sprite GetRoomStateVisual(RoomState roomState)
    {
        Sprite roomStateVisual = null;
        switch (roomState)
        {
            case RoomState.FogOfWar:
                roomStateVisual = roomFoWSprite;
                break;
            case RoomState.Started:
                roomStateVisual = roomStartedSprite;
                break;
            case RoomState.Complete:
                roomStateVisual = roomCompleteSprite;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(roomState), roomState, null);
        }
        return roomStateVisual;
    }

    public Sprite GetRoomTypeVisual(RoomType roomType)
    {
        Sprite roomTypeVisual = null;
        switch (roomType)
        {
            case RoomType.Base:
                return null;
            case RoomType.Stair:
                roomTypeVisual = roomTypeStairSprite;
                break;
            case RoomType.Shop:
                break;
            case RoomType.Sword:
                break;
            case RoomType.Potion:
                break;
            case RoomType.Boss:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(roomType), roomType, null);
        }

        return roomTypeVisual;
    }

    public Sprite GetSelectedVisual(bool isSelected)
    {
        Sprite roomSelectedVisual = null;
        if (isSelected)
        {
            roomSelectedVisual = roomSelectedSprite;
        }
        return roomSelectedVisual;
    }
    

    #endregion GET ROOM FUNCTIONS

}
