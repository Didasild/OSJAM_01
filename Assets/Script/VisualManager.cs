using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using Dida.Rendering;
using UnityEngine;
using Debug = UnityEngine.Debug;
using NaughtyAttributes;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
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
    
    [Header("GRID / ROOM TRANSITION")]
    public GameObject grid;
    [FormerlySerializedAs("grindIndicatorParent")] public GameObject gridIndicatorParent;
    public GameObject roomParent;

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
    [HideInInspector] public VisualSettings visualSettings;
    private GridManager _gridManager;
    
    private Material _gridMaterial;
    private List<TransformOffset> _gridIndicatorOffsetScript;
    public TransformOffset _RoomParentOffsetScript;
    
    private Tweener _currentWeightTween;
    private bool _roomTransitionComplete;

    #region EDITOR ONLY PARAMETERS
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
    #endregion EDITOR ONLY PARAMETERS

    #endregion

    #region INIT
    public void Init()
    {
        if (mainColorsVolume.profile.TryGet(out visualSettings)) { }
        LoadSprites();
        
        _roomMainProfile = mainColorsVolume.profile;
        
        _gridMaterial = grid.GetComponent<Renderer>().material;
        _gridIndicatorOffsetScript = GetTransformOffsets(gridIndicatorParent);
        _RoomParentOffsetScript = roomParent.GetComponent<TransformOffset>();
        
        _gridManager = GameManager.Instance.gridManager;
        DOTween.SetTweensCapacity(1000, 500);
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
    private List<TransformOffset> GetTransformOffsets(GameObject gameObjectParent)
    {
        return new List<TransformOffset>(gameObjectParent.GetComponentsInChildren<TransformOffset>());
    }
    
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
    #endregion INIT
    
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

    #region ROOM TRANSITION

    #region SET ROOM MOVEMENT
    public void RoomOffsetTransition(Vector2Int roomDirection, RoomData nextRoom)
    {
        int roomXDirection = roomDirection.x * 3;
        int roomYDirection = roomDirection.y * 3;
        _roomTransitionComplete = false;
        
        //Animation de la room
        if (roomXDirection != 0)
        {
            DOFloat(() => _RoomParentOffsetScript.primaryOffSetValue, x => _RoomParentOffsetScript.primaryOffSetValue = x,
                    roomXDirection > 0 ? - 1f : 1f, visualTransitionDuration)
                .SetEase(Ease.Linear);
        }

        if (roomYDirection != 0)
        {
            DOFloat(() => _RoomParentOffsetScript.secondaryOffSetValue, x => _RoomParentOffsetScript.secondaryOffSetValue = x,
                    roomYDirection > 0 ? -1f : 1f, visualTransitionDuration)
                .SetEase(Ease.Linear);
        }
        
        //Animation des room indicator
        foreach (TransformOffset transformOffset in _gridIndicatorOffsetScript)
        {
            if (!transformOffset.verticalOffset)
            {
                AnimateRoomTransitionValue(roomXDirection, visualTransitionDuration / Mathf.Abs(roomXDirection),
                    value => transformOffset.primaryOffSetValue = value,
                    () => transformOffset.primaryOffSetValue = 0f);
            }
            else
            {
                AnimateRoomTransitionValue(roomYDirection, visualTransitionDuration / Mathf.Abs(roomYDirection),
                    value => transformOffset.primaryOffSetValue = value,
                    () => transformOffset.primaryOffSetValue = 0f);
            }
        }
        
        //Animation de la Grid
        AnimateRoomTransitionValue(-roomXDirection, visualTransitionDuration / Mathf.Abs(roomXDirection),
            value => _gridMaterial.SetFloat("_GridXOffset", value * 11f),
            () =>
            {
                _gridMaterial.SetFloat("_GridXOffset", 0);
                CompleteRoomTransition(nextRoom);
            });
        AnimateRoomTransitionValue(-roomYDirection, visualTransitionDuration / Mathf.Abs(roomYDirection),
            value => _gridMaterial.SetFloat("_GridYOffset", value * 11f),
            () =>
            {
                _gridMaterial.SetFloat("_GridYOffset", 0);
                CompleteRoomTransition(nextRoom);
            });
    }

    private Tween AnimateRoomTransitionValue(int targetValue, float duration, Action<float> onUpdate, Action onComplete = null)
    {
        if (targetValue == 0)
        {
            return null;
        }

        int absLoops = Mathf.Abs(targetValue);
        float finalTarget = targetValue > 0 ? 1f : -1f;
        Sequence seq = DOTween.Sequence();

        // Variable locale pour animer la valeur
        float animValue = 0f;

        // Tweens intermédiaires
        for (int i = 1; i < absLoops - 1; i++)
        {
            seq.Append(
                CreateTweenForValue(() => 0f, x => { animValue = x; onUpdate?.Invoke(x); }, finalTarget, duration)
                    .SetEase(Ease.Linear)
                    .OnComplete(() => { animValue = 0f; onUpdate?.Invoke(0f); })
            );
        }

        // Dernière tween avec un easing de sortie
        seq.Append(
            CreateTweenForValue(() => animValue, x => animValue = x, finalTarget, visualTransitionDuration * 2, onUpdate)
                .SetEase(Ease.OutBack)
        );

        seq.OnComplete(() =>
        {
            onUpdate?.Invoke(0f);
            onComplete?.Invoke();
        });

        return seq;
    }
    
    private static Tween CreateTweenForValue(DOGetter<float> getter, DOSetter<float> setter, float finalTarget, float duration, Action<float> onUpdate = null)
    {
        return DOTween.To(getter, x => { setter(x); onUpdate?.Invoke(x); }, finalTarget, duration);
    }
    
    private static Tweener DOFloat(DOGetter<float> getter, DOSetter<float> setter, float endValue, float duration)
    {
        return DOTween.To(getter, setter, endValue, duration);
    }

    private void CompleteRoomTransition(RoomData nextRoom)
    {
        if (_roomTransitionComplete)
        {
            return;
        }
        _roomTransitionComplete = true;
        _RoomParentOffsetScript.ResetOffset();
        GameManager.Instance.floorManager.ChangeRoomOut(nextRoom);
        Debug.Log("Transition Complete");
    }

    #endregion SET ROOM MOVEMENT
    
    
    #region SET ROOM AMBIANCE
    public void UpdateRoomAmbiance(RoomData roomData)
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
    #endregion SET ROOM MOVEMENT
    
    public void ActiveListOfCells(float timeBetweenApparition, RoomState roomState)
    {
        if (roomState != RoomState.FogOfWar)
        {
            //Debug.Log("ActiveList Of Cells is not FogOfWar");
            foreach (Cell cell in _gridManager.cellList)
            {
                cell.gameObject.SetActive(true);
                cell.SpawnAnimation();
            }
        }
        else
        {
            StartCoroutine(CO_ActiveCellsWithDelay(timeBetweenApparition));
        }
    }

    private IEnumerator CO_ActiveCellsWithDelay(float timeBetweenApparition)
    {
        // Grouper les cellules par distance diagonale
        Dictionary<int, List<Cell>> diagonalGroups = new Dictionary<int, List<Cell>>();

        foreach (Cell cell in _gridManager.cellList)
        {
            // Calculer la distance diagonale
            int diagonalIndex = cell._cellPosition.x + cell._cellPosition.y;

            // Ajouter la cellule dans le groupe correspondant
            if (!diagonalGroups.ContainsKey(diagonalIndex))
            {
                diagonalGroups[diagonalIndex] = new List<Cell>();
            }
            diagonalGroups[diagonalIndex].Add(cell);
        }

        // Tri des groupes par distance diagonale (clé du dictionnaire)
        var sortedKeys = diagonalGroups.Keys.OrderBy(key => key).ToList();

        // Faire apparaître chaque groupe avec un délai
        foreach (int key in sortedKeys)
        {
            foreach (Cell cell in diagonalGroups[key])
            {
                cell.gameObject.SetActive(true);
                cell.SpawnAnimation();
            }
            yield return new WaitForSecondsRealtime(timeBetweenApparition); // Délai entre les groupes
        }
    }
    #endregion

}
