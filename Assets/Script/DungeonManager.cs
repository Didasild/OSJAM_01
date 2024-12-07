using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

#region ENUMS
public enum RoomDirection
{
    Right = 0,
    Left = 1,
    Up = 2,
    Down = 3
}
#endregion

public class DungeonManager : MonoBehaviour
{
    [Header("GENERAL SETTINGS")]
    public RoomData roomPrefab;

    [Header("FLOOR SETTINGS")]
    public FloorSettings currentFloorSetting;
    public FloorSettings[] floorSettingsList;
    //public Vector2 floorSize = new Vector2 (3, 4);

    [Header("ROOM SETTINGS")]
    [NaughtyAttributes.ReadOnly]
    public RoomData currentRoom;
    [NaughtyAttributes.ReadOnly]
    [SerializeField] private List<RoomData> roomList = new List<RoomData> ();
    //Private Room Settings
    public RoomSettings[] roomSettingsList;

    [Header("BUTTONS")]
    public GameObject buttonRight;
    public GameObject buttonLeft;
    public GameObject buttonUp;
    public GameObject buttonDown;

    #region FLOOR GENERATION
    public void GenerateFloor(Vector2Int floorSize)
    {
        roomSettingsList = currentFloorSetting.roomSettingsList;

        ClearFloor();

        List<RoomSettings> availableRoomList = new List<RoomSettings>(roomSettingsList);
        for (int y = 0; y < floorSize.y; y++)
        {
            for (int x = 0; x < floorSize.x; x++)
            {
                // Calculer la position sur la grille
                Vector2Int gridPosition = new Vector2Int(x, y);

                // Instancier le GameObject room
                RoomData roomData = Instantiate(roomPrefab, transform);
                if (roomData != null)
                {
                    roomData.Initialize(gridPosition); // Initialisation avec la position
                    roomList.Add(roomData); // Ajouter à la liste
                    roomData.transform.SetParent(transform);
                }
                // Nommer la room pour faciliter le debug
                roomData.name = $"Room_{x}_{y}";
            }
        }
        GiveNeighbors();
        AssignRoomSettings(roomList, availableRoomList);
    }

    public void AssignRoomSettings(List<RoomData> roomList, List<RoomSettings> availableRoomList)
    {
        if (roomList.Count > availableRoomList.Count)
        {
            Debug.LogWarning("Pas assez de RoomSettings disponibles pour assigner toutes les RoomData.");
            return;
        }

        // Créer une liste temporaire de RoomSettings à assigner
        List<RoomSettings> tempRoomSettings = new List<RoomSettings>();

        // Ajouter tous les éléments `isMandatory`
        List<RoomSettings> mandatorySettings = availableRoomList.Where(rs => rs.isMandatory).ToList();
        tempRoomSettings.AddRange(mandatorySettings);

        // Compléter la liste avec des éléments non obligatoires
        List<RoomSettings> nonMandatorySettings = availableRoomList.Where(rs => !rs.isMandatory).ToList();
        int remainingSlots = roomList.Count - tempRoomSettings.Count;

        if (remainingSlots > 0)
        {
            // Ajouter aléatoirement des RoomSettings non obligatoires jusqu'à atteindre la taille de roomList
            System.Random rng = new System.Random();
            nonMandatorySettings = nonMandatorySettings.OrderBy(x => rng.Next()).Take(remainingSlots).ToList();
            tempRoomSettings.AddRange(nonMandatorySettings);
        }

        // Mélanger la liste finale de RoomSettings
        tempRoomSettings = tempRoomSettings.OrderBy(x => UnityEngine.Random.value).ToList();

        // Assigner les RoomSettings mélangés aux RoomData
        for (int i = 0; i < roomList.Count; i++)
        {
            roomList[i].roomSettings = tempRoomSettings[i];
        }
        AssignRandomFirstRoom();
    }

    public void AssignRandomFirstRoom()
    {
        RoomData selectedRoomData = null;
        int randomIndex = Random.Range(0, roomList.Count);
        selectedRoomData = roomList[randomIndex];
        currentRoom = selectedRoomData;
        GameManager.Instance.GenerateRoom(currentRoom);
        UpdateButtonStates();
    }

    public void GiveNeighbors()
    {
        foreach (RoomData room in roomList)
        {
            Vector2Int position = room.roomPosition;

            // Cherche les voisins
            room.roomUp = FindRoomAtPosition(position + Vector2Int.up);
            room.roomDown = FindRoomAtPosition(position + Vector2Int.down);
            room.roomLeft = FindRoomAtPosition(position + Vector2Int.left);
            room.roomRight = FindRoomAtPosition(position + Vector2Int.right);
        }
    }
    private RoomData FindRoomAtPosition(Vector2Int position)
    {
        // Cherche la room correspondant à la position donnée
        return roomList.Find(r => r.roomPosition == position);
    }

    public void ClearFloor()
    {
        foreach (RoomData room in roomList)
        {
            if (room != null)
            {
                Destroy(room.gameObject);
            }
        }
        roomList = new List<RoomData>();
    }

    #endregion

    #region ON CLICK BUTTON FONCTIONS
    public void ChangeRoomDirection(int directionValue)
    {
        SaveRoomData();
        RoomDirection direction = (RoomDirection)directionValue;
        switch (direction)
        {
            case RoomDirection.Right:
                if (currentRoom.roomRight != null)
                {
                    currentRoom = currentRoom.roomRight;
                    GameManager.Instance.GenerateRoom(currentRoom);
                }
                break;
            case RoomDirection.Left:
                if (currentRoom.roomLeft != null)
                {
                    currentRoom = currentRoom.roomLeft;
                    GameManager.Instance.GenerateRoom(currentRoom);
                }
                break;
            case RoomDirection.Up:
                if (currentRoom.roomUp != null)
                {
                    currentRoom = currentRoom.roomUp;
                    GameManager.Instance.GenerateRoom(currentRoom);
                }
                break;
            case RoomDirection.Down:
                if (currentRoom.roomDown != null)
                {
                    currentRoom = currentRoom.roomDown;
                    GameManager.Instance.GenerateRoom(currentRoom);
                }
                break;
        }
        UpdateButtonStates();
    }

    public void SaveRoomData()
    {
        currentRoom.roomSavedString = GameManager.Instance.gridManager.SaveGridString();
    }

    public void UpdateButtonStates()
    {
        StopAllCoroutines();
        buttonRight.SetActive(currentRoom.roomRight != null);
        buttonLeft.SetActive(currentRoom.roomLeft != null);
        buttonUp.SetActive(currentRoom.roomUp != null);
        buttonDown.SetActive(currentRoom.roomDown != null);
    }
    #endregion


}
