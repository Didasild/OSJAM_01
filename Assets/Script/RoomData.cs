using UnityEngine;

public class RoomData : MonoBehaviour
{
    public RoomSettings floorSettings;
    public string roomSavedDate;
    public Vector2Int roomPosition;

    public GameObject roomUp;
    public GameObject roomDown;
    public GameObject roomLeft;
    public GameObject roomRight;

    public void Initialize(Vector2Int position)
    {
        roomPosition = position;
        // Debug : Afficher la position dans la console
        //Debug.Log($"Room initialized at grid position: {roomPosition}");
    }

}
