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
#endregion ENUMS

public class RoomData : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    #region PARAMETERS

    [Header("GENERAL DATA")]
    [ReadOnly] public RoomSettings roomSettings;
    [ReadOnly] public RoomState currentRoomState;
    [ReadOnly] public RoomType currentRoomType;
    [ReadOnly] public string roomSavedString;
    [ReadOnly] public Vector2Int roomPosition;

    [Header("NEIGHBORS")]
    [ReadOnly] public RoomData roomUp;
    [ReadOnly] public RoomData roomDown;
    [ReadOnly] public RoomData roomLeft;
    [ReadOnly] public RoomData roomRight;

    [Header("ROOM MINIMAP VISUAL")]
    public Image roomTypeVisual;
    public Image roomStateVisual;
    public Image roomSelectedVisual;
    
    private DungeonManager _dungeonManager;
    //private RoomVisualManager _roomVisualManager;
    private VisualManager _visualManager;
    #endregion

    #region INIT
    public void Initialize(Vector2Int position, float roomSize = 1.0f, Vector3 offset = default)
    {
        roomPosition = position;

        // Calculez la position dans le monde
        Vector3 worldPosition = new Vector3(
            position.x * roomSize,
            position.y * roomSize,
            0
        ) + offset; // Ajoutez le décalage global pour centrer la grille

        // Placez le GameObject à cette position
        transform.localPosition = worldPosition;

        //Setup le visuel
        _visualManager = GameManager.VisualManager;
        roomStateVisual.sprite = _visualManager.GetRoomStateVisual(RoomState.FogOfWar);
        
        //Assigne le dungeon manager
        _dungeonManager = GameManager.Instance.dungeonManager;
        
    }
    #endregion INIT

    public void InitializeRoomType()
    {
        currentRoomType = roomSettings.roomType;
        roomTypeVisual.sprite = _visualManager.GetRoomTypeVisual(RoomType.Base);
    }

    #region ROOM STATE
    public void ChangeRoomSate(RoomState newRoomState)
    {
        currentRoomState = newRoomState;
        //Update le visuel de la room
        roomStateVisual.sprite = GameManager.VisualManager.GetRoomStateVisual(currentRoomState);

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
