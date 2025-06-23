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

    
    [Header("VISUAL CONTROLLERS")]
    public RoomAmbianceController roomAmbianceController;
    public TextController textController;
    
    [Header("FEEDBACKS CONTROLLERS")]
    public ShakeCamController shakeCamController;
    public CentralFeedbackController centralFeedbackController;
    public FullScreenFeedbackController fullScreenFeedbackController;
    [FormerlySerializedAs("roomMovementController")] public RoomTransitionController roomTransitionController;

    [Header("AMBIANCE / POST PROCESS")]
    public Volume mainColorsVolume;
    
    [Header("GRID / ROOM TRANSITION")]
    public float roomTransitionDuration;
    public GameObject roomID_Raw;
    public GameObject roomID_Col;

    [Header("_______CELL ANIMATIONS")] 
    public List<GameObject> animationPrefabs;
    public GameObject mineExplosionAnimation;
    public GameObject mineSwordedAnimation;
    public GameObject plantedSwordAnimation;
    public GameObject flagInAnimation;
    
    [Header("_______MINIMAP")]
    public MinimapVisual minimapVisual;
    
    private VolumeProfile _roomMainProfile;
    private GlobalColorSettings _roomTransitionGlobalColorSettings;
    [FormerlySerializedAs("visualSettings")] [HideInInspector] public GlobalColorSettings globalColorSettings;
    private GridManager _gridManager;
    private GameManager _gameManager;
    
    private SpriteRenderer _roomIDRawRenderer;
    private SpriteRenderer _roomIDColRenderer;
    
    private Tweener _currentWeightTween;
    
    #endregion

    #region INIT
    public void Init(GameManager manager)
    {
        if (mainColorsVolume.profile.TryGet(out globalColorSettings)) { }
        LoadSprites();
        
        _roomMainProfile = mainColorsVolume.profile;
        
        _roomIDRawRenderer = roomID_Raw.GetComponent<SpriteRenderer>();
        _roomIDColRenderer = roomID_Col.GetComponent<SpriteRenderer>();
        
        _gameManager = manager;
        _gridManager = manager.GridManager;
        DOTween.SetTweensCapacity(1000, 500);
        
        centralFeedbackController.Init(this);
        fullScreenFeedbackController.Init(this);
        textController.Init(this);
        shakeCamController.Init();
        roomAmbianceController.Init(this);
        roomTransitionController.Init(this);
    }
    
    [Button]
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
   
    public Sprite GetCellTypeVisual(Cell cell)
    {
        CellType cellType = cell.currentType;
        Sprite cellTypeVisual = null;
        if (cellType == CellType.Gate)
        {
            cellTypeVisual = GetSprite("Cell_Type_Stair");
        }
        else if (cellType == CellType.Npc)
        {
            cellTypeVisual = GetNpcStateVisual(cell.npc._currentNpcState);
        }
        return cellTypeVisual;
    }

    public Sprite GetNpcStateVisual(DialogUtils.NPCState npcState)
    {
        Sprite cellTypeVisual = null;
        if (npcState == DialogUtils.NPCState.Active)
        {
            cellTypeVisual = GetSprite("Cell_Type_Npc_Active");
        }
        else if (npcState == DialogUtils.NPCState.Inactive)
        {
            cellTypeVisual = GetSprite("Cell_Type_Npc_Inactive");
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
                returnedColor = globalColorSettings.Color1.value;
                break;
            case 2:
                returnedColor = globalColorSettings.Color2.value;
                break;
            case 3:
                returnedColor = globalColorSettings.Color3.value;
                break;
            case 4:
                returnedColor = globalColorSettings.Color4.value;
                break;
            case 5:
                returnedColor = globalColorSettings.Color5.value;
                break;
            default:
                Debug.LogWarning("Index de couleur invalide : " + colorIndex);
                break;
        }
        return returnedColor;
    }
    #endregion

    #region FEEDBACKS
    //Central feedbacks
    public void PlayCellRevealFeedbacks()
    {
        centralFeedbackController.CellRevealFeedbackIn();
    }
    public void PlayRoomUnlockFeedbacks()
    {
        centralFeedbackController.RoomCompletionFeedback();
    }
    
    //FullScreenFeedbacks
    public void PlayHitFeedbacks()
    {
        fullScreenFeedbackController.HitFeedback();
        shakeCamController.littleShakeCamera();
    }
    
    //UTILS
    public void FadeProperty(Material targetMaterial, string propertyID, float targetValue, float duration,  float delay = 0, Ease ease = Ease.Linear, bool resetProperty = false)
    {
        Tween tween = targetMaterial.DOFloat(targetValue, propertyID, duration)
            .SetDelay(delay)
            .SetEase(ease);
        if (resetProperty)
        {
            tween.OnComplete(() => ResetProperty(targetMaterial, 0, propertyID));
        }
    }
    private void ResetProperty(Material targetMaterial, float targetValue, string propertyID)
    {
        targetMaterial.SetFloat(propertyID, targetValue);
    }
    #endregion FEEDBACKS
    
    #region ROOM TRANSITION
    public void UpdateRoomAmbiance(RoomData roomData)
    {
        roomAmbianceController.TransitionVolume(roomData.initRoomSettings.roomVolumeProfile);
    }
    
    public void ActiveListOfCells(float timeBetweenApparition, RoomState roomState)
    {
        if (roomState != RoomState.FogOfWar)
        {
            //Debug.Log("ActiveList Of Cells is not FogOfWar");
            foreach (Cell cell in _gridManager.cellList)
            {
                cell.gameObject.SetActive(true);
                if (cell.currentState != CellState.Inactive)
                {
                    cell.SpawnAnimation();
                }
                else
                {
                    cell.ActiveCollider();
                }
            }
        }
        else
        {
            StartCoroutine(CO_ActiveCellsWithDelay(timeBetweenApparition));
        }
        _gridManager.isGeneratingRoom = false;
    }

    private IEnumerator CO_ActiveCellsWithDelay(float timeBetweenApparition)
    {
        // Grouper les cellules par distance diagonale
        Dictionary<int, List<Cell>> diagonalGroups = new Dictionary<int, List<Cell>>();

        List<Cell> inactiveCells = new List<Cell>(_gridManager.GetCellsByState(CellState.Inactive));
        List<Cell> activeCells = new List<Cell>(_gridManager.cellList);
        activeCells.RemoveAll(cell => inactiveCells.Contains(cell));
        
        foreach (Cell cell in activeCells)
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
        List<int> sortedKeys = diagonalGroups.Keys.OrderBy(key => key).ToList();

        foreach (Cell cell in inactiveCells)
        {
            cell.gameObject.SetActive(true);
            cell.ActiveCollider();
        }
        
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

    public void UpdateRoomID(RoomData roomData)
    {
        _roomIDRawRenderer.sprite = GetSprite(roomData.roomPosition.x.ToString() + "b");
        _roomIDColRenderer.sprite = GetSprite(roomData.roomPosition.y.ToString() + "b");
    }

    #endregion
}
