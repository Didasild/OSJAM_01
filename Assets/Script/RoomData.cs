using UnityEngine;

#region
public enum roomState
{
    Undiscover,
    Started,
    Complete
}
#endregion

public class RoomData : MonoBehaviour
{
    public RoomSettings roomSettings;
    public roomState currentRoomState;
    public string roomSavedString;
    public Vector2Int roomPosition;

    public RoomData roomUp;
    public RoomData roomDown;
    public RoomData roomLeft;
    public RoomData roomRight;

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
