using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using NaughtyAttributes;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

#region ENUMS
public enum RoomState
{
    FogOfWar,
    Hide,
    StartedLock,
    StartedUnlock,
    Complete
}
#endregion ENUMS

public class RoomData : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    #region PARAMETERS
    [Header("GENERAL DATA")]
    [ReadOnly] public RoomSettings initRoomSettings;
    [ReadOnly] public RoomState currentRoomState;
    [ReadOnly] public RoomType currentRoomType;
    [ReadOnly] public string roomSavedString;
    [ReadOnly] public Vector2Int roomPosition;
    [ReadOnly] public bool startRoom;
    [ReadOnly] public RoomCompletion.RoomCompletionConditions roomConditions;
    [ReadOnly] public RoomCompletion.RoomCompletionConditions roomUnlockConditions;
    [ReadOnly] public bool isObjective = false;
    [ReadOnly] public List<NPC> roomNPCs = new List<NPC>();
    [ReadOnly] public Boolean isLocked = false;
    [ReadOnly] public VolumeProfile currentVolumeProfile;
     

    [Header("NEIGHBORS")]
    [ReadOnly] public RoomData roomUp;
    [ReadOnly] public RoomData roomDown;
    [ReadOnly] public RoomData roomLeft;
    [ReadOnly] public RoomData roomRight;

    [Header("ROOM MINIMAP VISUAL")]
    public Image roomTypeVisual;
    public Image roomStateVisual;
    public Image roomSelectedVisual;
    public Image roomOverVisual;
    public Image roomObjectiveVisual;
    
    [Header("TOOLTIP")]
    [SerializeField] private string tooltipGoToText;

    private FloorManager _floorManager;
    private Minimap _minimap;
    private VisualManager _visualManager;
    #endregion

    #region INIT

    public void Initialize(FloorSettings.LoadedRoomData roomData, Minimap minimap)
    {
        _floorManager = GameManager.Instance.FloorManager;
        _minimap = minimap;
        roomConditions = roomData.roomCompletion;
        roomUnlockConditions = roomData.roomUnlock;

        //Setup le visuel
        _visualManager = GameManager.VisualManager;
        currentRoomState = roomData.initRoomState;
        roomStateVisual.sprite = GameManager.VisualManager.minimapVisual.GetRoomStateVisual(currentRoomState);
        SetColor(currentRoomState);
    }
    public void InitializeRoomType()
    {
        if (initRoomSettings.roomVolumeProfile != null)
        {
            currentVolumeProfile = initRoomSettings.roomVolumeProfile;
        }
        currentRoomType = initRoomSettings.roomType;
        roomTypeVisual.sprite = _visualManager.minimapVisual.GetRoomTypeVisual(RoomType.Base);
        InitNpcs();
    }
    
    private void InitNpcs()
    {
        if (initRoomSettings.npcDatas == null || initRoomSettings.npcDatas.Count == 0)
        {
            return;
        }
        
        foreach (DialogUtils.NpcData npcData in initRoomSettings.npcDatas)
        {
            NPC npc = new NPC();
            npc.Init(npcData.npcDialogsSettings, npcData.npcPosition);
            roomNPCs.Add(npc);
            npc.ChangeNpcState(npcData.npcDialogsSettings.baseNPCState);
        }
    }
    #endregion INIT

    #region ROOM STATE
    public void ChangeRoomSate(RoomState newRoomState)
    {
        currentRoomState = newRoomState;
        //Update le visuel de la room
        roomStateVisual.sprite = GameManager.VisualManager.minimapVisual.GetRoomStateVisual(currentRoomState);
        SetColor(currentRoomState);
        gameObject.transform.SetParent(GameManager.VisualManager.minimapVisual.GetRoomNewParent(currentRoomState));

        switch (currentRoomState)
        {
            case RoomState.FogOfWar:
                FogOfWarRoomState();
                break;
            case RoomState.Hide:
                HideRoomState();
                break;
            case RoomState.StartedLock:
                StartedLockRoomState();
                break;
            case RoomState.StartedUnlock:
                StartedUnLockRoomState();
                _visualManager.minimapVisual.OpenMinimap();
                break;
            case RoomState.Complete:
                CompleteRoomState();
                _visualManager.minimapVisual.OpenMinimap();
                break;
        }
        _floorManager.UpdateButtonStates();
    }

    private void FogOfWarRoomState()
    {
        isLocked = true;
    }

    private void HideRoomState()
    {
        isLocked = true;
    }

    private void StartedLockRoomState()
    {
        _visualManager.minimapVisual.CloseMinimap();
        isLocked = true;
    }
    
    private void StartedUnLockRoomState()
    {
        isLocked = false;
    }

    private void CompleteRoomState()
    {
        if (currentRoomType != RoomType.Base)
        {
            //Fait apparaitre le type de la room
            Color roomTypeVisualColor = roomTypeVisual.color;
            roomTypeVisualColor.a = 1;
            roomTypeVisual.color = roomTypeVisualColor;
        }
        roomTypeVisual.sprite = _visualManager.minimapVisual.GetRoomTypeVisual(currentRoomType);
        UpdateObjective();
        _floorManager.floorObjectivesController.CheckObjectiveCompletion();
        isLocked = false;
    }

    public void UpdateObjective()
    {
        if (currentRoomState == RoomState.Complete)
        {
            isObjective = false;
            roomObjectiveVisual.gameObject.SetActive(false);
            _floorManager.floorObjectivesController.CheckObjectiveCompletion();
            return;
        }
        
        isObjective = true;
        roomObjectiveVisual.gameObject.SetActive(isObjective);
    }
    #endregion ROOM STATE

    #region GETTER
    //A DEPLACER DANS UN UTILS
    public NPC GetNpcFromCellPosition(Vector2Int cellPosition)
    {
        if (roomNPCs == null)
        {
            Debug.LogWarning("No room npc found");
            return null;
        }

        foreach (NPC roomNpc in roomNPCs)
        {
            if (roomNpc.currentPosition == cellPosition)
            {
                return roomNpc;
            }
        }
        return null;
    }

    public NPC GetNpcFromDialogSettings(NpcDialogsSettings npcDialogsSettings)
    {
        if (roomNPCs == null)
        {
            Debug.LogWarning("No room npc found");
            return null;
        }
        foreach (NPC roomNpc in roomNPCs)
        {
            if (roomNpc.NpcDialogsSettings == npcDialogsSettings)
            {
                return roomNpc;
            }
        }
        return null;
    }

    public void SetColor(RoomState currentRoomState)
    {
        switch (currentRoomState)
        {
            case RoomState.Hide:
                roomStateVisual.color = Color.clear;
                break;
            case RoomState.StartedLock:
                roomStateVisual.color = Color.white;
                break;
            case RoomState.StartedUnlock:
                roomStateVisual.color = Color.white;
                break;
            case RoomState.Complete:
                roomStateVisual.color = Color.white;
                break;
            case RoomState.FogOfWar:
                roomStateVisual.color = Color.white;
                break;
        }
    }
    #endregion GETTER

    #region POINTER
    //Click sur la minimap
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentRoomState == RoomState.FogOfWar || _floorManager.currentRoom == this || _floorManager.currentRoom.currentRoomState == RoomState.StartedLock)
        {
            return;
        }
        
        roomOverVisual.gameObject.SetActive(true);
        TooltipController.ShowTooltip(tooltipGoToText + roomPosition);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        roomOverVisual.gameObject.SetActive(false);
        TooltipController.HideTooltip();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentRoomState == RoomState.FogOfWar || _floorManager.currentRoom == this || _floorManager.currentRoom.currentRoomState == RoomState.StartedLock)
        {
            return;
        }

        _floorManager.minimap.ChangeOnClickIn(this);
    }
    #endregion POINTER
}
