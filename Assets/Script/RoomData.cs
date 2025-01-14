using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

#region ENUMS
public enum RoomState
{
    FogOfWar,
    Started,
    Complete
}
#endregion ENUMS

public class RoomData : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    #region PARAMETERS

    [Header("GENERAL DATA")]
    [NaughtyAttributes.ReadOnly] public RoomSettings roomSettings;
    [NaughtyAttributes.ReadOnly] public RoomState currentRoomState;
    [NaughtyAttributes.ReadOnly] public RoomType currentRoomType;
    [NaughtyAttributes.ReadOnly] public string roomSavedString;
    [NaughtyAttributes.ReadOnly] public Vector2Int roomPosition;

    [Header("NEIGHBORS")]
    [NaughtyAttributes.ReadOnly] public RoomData roomUp;
    [NaughtyAttributes.ReadOnly] public RoomData roomDown;
    [NaughtyAttributes.ReadOnly] public RoomData roomLeft;
    [NaughtyAttributes.ReadOnly] public RoomData roomRight;

    [Header("ROOM MINIMAP VISUAL")]
    public Image roomTypeVisual;
    public Image roomStateVisual;
    public Image roomSelectedVisual;
    
    private DungeonManager _dungeonManager;
    private RoomVisualManager _roomVisualManager;
    #endregion

    #region INIT
    public void Initialize(Vector2Int position, float roomSize = 1.0f, Vector3 offset = default)
    {
        this.roomPosition = position;

        // Calculez la position dans le monde
        Vector3 worldPosition = new Vector3(
            position.x * roomSize,
            position.y * roomSize,
            0
        ) + offset; // Ajoutez le décalage global pour centrer la grille

        // Placez le GameObject à cette position
        transform.localPosition = worldPosition;

        //Setup le visuel
        _roomVisualManager = GameManager.RoomVisualManager;
        roomStateVisual.sprite = _roomVisualManager.GetRoomStateVisual(RoomState.FogOfWar);
        
        //Assigne le dungeon manager
        _dungeonManager = GameManager.Instance.dungeonManager;
    }
    #endregion INIT

    public void InitializeRoomType()
    {
        currentRoomType = roomSettings.roomType;
        roomTypeVisual.sprite = _roomVisualManager.GetRoomTypeVisual(RoomType.Base);
    }

    #region ROOM STATE
    public void ChangeRoomSate(RoomState newRoomState)
    {
        currentRoomState = newRoomState;
        //Update le visuel de la room
        roomStateVisual.sprite = GameManager.RoomVisualManager.GetRoomStateVisual(currentRoomState);

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
        _dungeonManager.UpdateButtonStates();
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
        roomTypeVisual.sprite = _roomVisualManager.GetRoomTypeVisual(currentRoomType);
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
        if (currentRoomState == RoomState.FogOfWar || _dungeonManager.currentRoom == this || _dungeonManager.currentRoom.currentRoomState == RoomState.Started)
        {
            return;
        }
        else
        {
            _dungeonManager.ChangeRoomMinimap(this);
        }

    }
    #endregion POINTER
}
