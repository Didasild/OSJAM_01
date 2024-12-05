using UnityEngine;

public class RoomData : MonoBehaviour
{
    public RoomSettings roomSettings;
    public string roomSavedDate;
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

}
