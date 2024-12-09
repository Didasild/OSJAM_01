using UnityEngine;
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
    #endregion


    public void Initialize(Vector2Int position)
    {
        roomPosition = position;
        // Debug : Afficher la position dans la console
        //Debug.Log($"Room initialized at grid position: {roomPosition}");
    }

    public void ChangeRoomSate(RoomState newRoomState)
    {
        currentRoomState = newRoomState;

        switch (currentRoomState)
        {
            case RoomState.FogOfWar:

                break;

            case RoomState.Started:

                break;

            case RoomState.Complete:

                break;
        }
    }
}
