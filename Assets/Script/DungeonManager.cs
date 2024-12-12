using System.Collections.Generic;
using System.Linq;
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
    #region PARAMETERS
    [Header("GENERAL SETTINGS")]
    public RoomData roomPrefab;
    public Transform roomContainer;
    public float roomSize;

    [Header("FLOOR SETTINGS")]
    [NaughtyAttributes.ReadOnly]
    public FloorSettings currentFloorSetting;
    public FloorSettings[] floorSettingsList;
    //public Vector2 floorSize = new Vector2 (3, 4);

    [Header("ROOM SETTINGS")]
    [NaughtyAttributes.ReadOnly]
    public RoomData currentRoom;
    [NaughtyAttributes.ReadOnly]
    [SerializeField] private List<RoomData> roomList = new List<RoomData> ();
    //Private Room Settings
    private RoomSettings[] _roomSettingsList;

    [Header("BUTTONS")]
    public GameObject buttonRight;
    public GameObject buttonLeft;
    public GameObject buttonUp;
    public GameObject buttonDown;
    #endregion

    #region FLOOR GENERATION
    public void GenerateFloor(Vector2Int floorSize)
    {
        _roomSettingsList = currentFloorSetting.roomSettingsList;

        ClearFloor();

        // Calcul du décalage pour centrer la grille
        Vector3 offset = new Vector3(
            -(floorSize.x * roomSize) / 2.0f + (roomSize / 2.0f), // Décalage X
            -(floorSize.y * roomSize) / 2.0f + (roomSize / 2.0f), // Décalage Y
            0 // Z reste constant
        );

        List<RoomSettings> availableRoomList = new List<RoomSettings>(_roomSettingsList);
        for (int y = 0; y < floorSize.y; y++)
        {
            for (int x = 0; x < floorSize.x; x++)
            {
                // Calculer la position sur la grille
                Vector2Int gridPosition = new Vector2Int(x, y);

                // Instancier le GameObject room
                RoomData roomData = Instantiate(roomPrefab, roomContainer);
                if (roomData != null)
                {
                    roomData.Initialize(gridPosition, roomSize, offset); // Initialisation avec la position
                    roomList.Add(roomData); // Ajouter � la liste
                    roomData.transform.SetParent(roomContainer);
                    roomData.roomStateVisual.sprite = GameManager.RoomVisualManager.GetRoomStateVisual(RoomState.FogOfWar);
                }
                // Nommer la room pour faciliter le debug
                roomData.name = $"Room_{x}_{y}";
            }
        }
        GiveNeighbors();
        AssignRoomSettings(roomList, availableRoomList);
    }

    private void AssignRoomSettings(List<RoomData> roomList, List<RoomSettings> availableRoomList)
    {
        if (roomList.Count > availableRoomList.Count)
        {
            Debug.LogWarning("Pas assez de RoomSettings disponibles pour assigner toutes les RoomData.");
            return;
        }

        // Cr�er une liste temporaire de RoomSettings � assigner
        List<RoomSettings> tempRoomSettings = new List<RoomSettings>();

        // Ajouter tous les �l�ments `isMandatory`
        List<RoomSettings> mandatorySettings = availableRoomList.Where(rs => rs.isMandatory).ToList();
        tempRoomSettings.AddRange(mandatorySettings);

        // Compl�ter la liste avec des �l�ments non obligatoires
        List<RoomSettings> nonMandatorySettings = availableRoomList.Where(rs => !rs.isMandatory).ToList();
        int remainingSlots = roomList.Count - tempRoomSettings.Count;

        if (remainingSlots > 0)
        {
            // Ajouter al�atoirement des RoomSettings non obligatoires jusqu'� atteindre la taille de roomList
            System.Random rng = new System.Random();
            nonMandatorySettings = nonMandatorySettings.OrderBy(x => rng.Next()).Take(remainingSlots).ToList();
            tempRoomSettings.AddRange(nonMandatorySettings);
        }

        // M�langer la liste finale de RoomSettings
        tempRoomSettings = tempRoomSettings.OrderBy(x => UnityEngine.Random.value).ToList();

        // Assigner les RoomSettings m�lang�s aux RoomData
        for (int i = 0; i < roomList.Count; i++)
        {
            roomList[i].roomSettings = tempRoomSettings[i];
        }
        AssignRandomFirstRoom();
    }

    private void AssignRandomFirstRoom()
    {
        RoomData selectedRoomData = null;
        //Selectionne une room random
        int randomIndex = Random.Range(0, roomList.Count);
        selectedRoomData = roomList[randomIndex];
        currentRoom = selectedRoomData;

        //La génère
        GameManager.Instance.ChangeRoom(currentRoom);

        //Update le visuel de la minimap
        currentRoom.roomSelectedVisual.sprite = GameManager.RoomVisualManager.GetSelectedVisual(true);

        //Update les boutons
        UpdateButtonStates();
    }

    private void GiveNeighbors()
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
        // Cherche la room correspondant � la position donn�e
        return roomList.Find(r => r.roomPosition == position);
    }
    
    private void ClearFloor()
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
    

    #region BUTTON FONCTIONS
    public void ChangeRoomDirection(int directionValue)
    {
        SaveRoomData();
        currentRoom.roomSelectedVisual.sprite = GameManager.RoomVisualManager.GetSelectedVisual(false);
        RoomDirection direction = (RoomDirection)directionValue;
        switch (direction)
        {
            case RoomDirection.Right:
                if (currentRoom.roomRight != null)
                {
                    currentRoom = currentRoom.roomRight;
                    GameManager.Instance.ChangeRoom(currentRoom);
                }
                break;
            case RoomDirection.Left:
                if (currentRoom.roomLeft != null)
                {
                    currentRoom = currentRoom.roomLeft;
                    GameManager.Instance.ChangeRoom(currentRoom);
                }
                break;
            case RoomDirection.Up:
                if (currentRoom.roomUp != null)
                {
                    currentRoom = currentRoom.roomUp;
                    GameManager.Instance.ChangeRoom(currentRoom);
                }
                break;
            case RoomDirection.Down:
                if (currentRoom.roomDown != null)
                {
                    currentRoom = currentRoom.roomDown;
                    GameManager.Instance.ChangeRoom(currentRoom);
                }
                break;
        }
        currentRoom.roomSelectedVisual.sprite = GameManager.RoomVisualManager.GetSelectedVisual(true);
        UpdateButtonStates();
    }

    public void SaveRoomData()
    {
        currentRoom.roomSavedString = GameManager.Instance.gridManager.SaveGridString();
    }

    private void UpdateButtonStates()
    {
        StopAllCoroutines();
        buttonRight.SetActive(currentRoom.roomRight != null);
        buttonLeft.SetActive(currentRoom.roomLeft != null);
        buttonUp.SetActive(currentRoom.roomUp != null);
        buttonDown.SetActive(currentRoom.roomDown != null);
    }
    #endregion


}
