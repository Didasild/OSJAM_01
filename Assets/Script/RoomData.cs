using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#region ENUMS
public enum RoomState
{
    FogOfWar,
    Started,
    Complete
}
#endregion

public class RoomData : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    #region PARAMETERS

    [Header("GENERAL DATA")]
    [NaughtyAttributes.ReadOnly] public RoomSettings roomSettings;
    [NaughtyAttributes.ReadOnly] public RoomState currentRoomState;
    [NaughtyAttributes.ReadOnly] public string roomSavedString;
    [NaughtyAttributes.ReadOnly] public Vector2Int roomPosition;

    [Header("NEIGHBORS")]
    [NaughtyAttributes.ReadOnly] public RoomData roomUp;
    [NaughtyAttributes.ReadOnly] public RoomData roomDown;
    [NaughtyAttributes.ReadOnly] public RoomData roomLeft;
    [NaughtyAttributes.ReadOnly] public RoomData roomRight;

    [Header("ROOM MINIMAP VISUAL")]
    public Image roomStateVisual;
    public Image roomSelectedVisual;
    
    private DungeonManager _dungeonManager;
    #endregion


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
        
        _dungeonManager = GameManager.Instance.dungeonManager;
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

    }
    #endregion ROOM STATE

    #region POINTER
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
