using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

#region ENUMS
public enum RoomState
{
    FogOfWar,
    Started,
    Complete
}
#endregion

public class RoomData : MonoBehaviour
{
    #region PARAMETERS
    [Header("GENERAL DATA")]
    [Header("ROOM MINIMAP VISUAL")]
    public Image roomStateVisual;

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
    #endregion


    public void Initialize(Vector2Int position, float roomSize = 1.0f)
    {
        this.roomPosition = position;

        // Calculez la position dans le monde
        Vector3 worldPosition = new Vector3(
            position.x * roomSize,
            -position.y * roomSize, // Utilisez `-gridPosition.y` pour descendre sur l'axe Y
            0 // Z reste constant (2D)
        );

        // Placez le GameObject � cette position
        transform.localPosition = worldPosition;
    }

    public void ChangeRoomSate(RoomState newRoomState)
    {
        currentRoomState = newRoomState;
        //Update le visuel de la room
        roomStateVisual.sprite = GameManager.RoomVisualManager.GetRoomStateVisual(currentRoomState);

        switch (currentRoomState)
        {
            case RoomState.FogOfWar:
                FogOfWarRoomState();
            case RoomState.FogOfWar:

                break;

            case RoomState.Started:
                StartedRoomState();
                break;

            case RoomState.Complete:
                CompleteRoomState();
                break;
        }
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
}
