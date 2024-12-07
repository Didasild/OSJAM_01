using UnityEngine;

#region ENUMS
public enum roomState
{
    Undiscover,
    Started,
    Complete
}
#endregion

public class RoomData : MonoBehaviour
{
    #region PARAMETERS
    [Header("GENERAL SETTINGS")]
    [NaughtyAttributes.ReadOnly] public RoomSettings roomSettings;
    [NaughtyAttributes.ReadOnly] public roomState currentRoomState;
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

    public void ChangeRoomSate(roomState newRoomState)
    {
        currentRoomState = newRoomState;

        switch (currentRoomState)
        {
            case roomState.Undiscover:

                break;

            case roomState.Started:

                break;

            case roomState.Complete:

                break;
        }
    }
}
