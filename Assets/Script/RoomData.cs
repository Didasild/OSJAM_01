using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using NaughtyAttributes;

#region ENUMS
public enum RoomState
{
    FogOfWar,
    Started,
    Complete
}
public enum RoomCompletionCondition
{
    None,
    FlaggedAllMine,
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
    [ReadOnly] public RoomCompletionCondition roomCondition;

    [Header("NEIGHBORS")]
    [ReadOnly] public RoomData roomUp;
    [ReadOnly] public RoomData roomDown;
    [ReadOnly] public RoomData roomLeft;
    [ReadOnly] public RoomData roomRight;

    [Header("ROOM MINIMAP VISUAL")]
    public Image roomTypeVisual;
    public Image roomStateVisual;
    public Image roomSelectedVisual;
    
    private DungeonManager _floorManager;
    private Minimap _minimap;
    //private RoomVisualManager _roomVisualManager;
    private VisualManager _visualManager;
    #endregion

    #region INIT

    public void Initialize(Vector2Int position, RoomCompletionCondition newRoomCondition, Minimap minimap, float roomSize = 1.0f,  Vector3 offset = default)
    {
        _minimap = minimap;
        roomPosition = position;
        roomCondition = newRoomCondition;
        
        //Setup le visuel
        SetRoomPosition(position, roomSize, offset);

        //Setup le visuel
        _visualManager = GameManager.visualManager;
        roomStateVisual.sprite = _visualManager.GetRoomStateVisual(RoomState.FogOfWar);
        
        //Assigne le dungeon manager
        _floorManager = GameManager.Instance.floorManager;
        
    }
    public void InitializeRoomType()
    {
        currentRoomType = initRoomSettings.roomType;
        roomTypeVisual.sprite = _visualManager.GetRoomTypeVisual(RoomType.Base);
    }

    //APPELER DIRECTEMENT DEPUIS LE DUNGEON MANAGER? Demander a Guillaume 
    private void SetRoomPosition(Vector2Int position, float roomSize, Vector3 offset = default)
    {
        // Calculez la position dans le monde
        Vector3 worldPosition = new Vector3(
            position.x * roomSize,
            position.y * roomSize,
            0
        ) + offset; // Ajoutez le décalage global pour centrer la grille

        // Placez le GameObject à cette position
        transform.localPosition = worldPosition;
    }
    #endregion INIT

    #region ROOM STATE
    public void ChangeRoomSate(RoomState newRoomState)
    {
        currentRoomState = newRoomState;
        //Update le visuel de la room
        roomStateVisual.sprite = GameManager.visualManager.GetRoomStateVisual(currentRoomState);

        switch (currentRoomState)
        {
            case RoomState.FogOfWar:
                FogOfWarRoomState();
                break;
            case RoomState.Started:
                StartedRoomState();
                break;
            case RoomState.Complete:
                CompleteRoomState();
                break;
        }
        _floorManager.UpdateButtonStates();
    }

    private void FogOfWarRoomState()
    {

    }

    private void StartedRoomState()
    {
        
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
        roomTypeVisual.sprite = _visualManager.GetRoomTypeVisual(currentRoomType);
    }
    #endregion ROOM STATE

    #region POINTER
    //Click sur la minimap
    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentRoomState == RoomState.FogOfWar || _floorManager.currentRoom == this || _floorManager.currentRoom.currentRoomState == RoomState.Started)
        {
            return;
        }
        else
        {
            _floorManager.ChangeRoomMinimapIn(this);
        }

    }
    #endregion POINTER
}
