using System.Collections.Generic;
using System.Linq;
using TMPro;
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

    [Header("DEBUG")] 
    public TMP_Text roomNameDebugText;
    
    private RoomVisualManager _roomVisualManager;
    #endregion

    #region INIT

    public void Init()
    {
        _roomVisualManager = GameManager.RoomVisualManager;
    }
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

        // Créer une liste temporaire de RoomSettings à assigner
        List<RoomSettings> tempRoomSettings = new List<RoomSettings>();

        // Ajouter tous les éléments `isMandatory`
        List<RoomSettings> mandatorySettings = availableRoomList.Where(rs => rs.mandatory).ToList();
        tempRoomSettings.AddRange(mandatorySettings);

        // Compléter la liste avec des éléments non obligatoires
        List<RoomSettings> nonMandatorySettings = availableRoomList.Where(rs => !rs.mandatory).ToList();
        int remainingSlots = roomList.Count - tempRoomSettings.Count;

        if (remainingSlots > 0)
        {
            // Ajouter aléatoirement des RoomSettings non obligatoires jusqu'� atteindre la taille de roomList
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

        foreach (RoomData room in roomList)
        {
            room.InitializeRoomType();
        }
        AssignRandomFirstRoom();
    }

    private void AssignRandomFirstRoom()
    {
        RoomData selectedRoomData = null;
        
        //Sélectionne une room random
        int randomIndex = Random.Range(0, roomList.Count);
        selectedRoomData = roomList[randomIndex];
        currentRoom = selectedRoomData;

        //La génère
        GameManager.Instance.ChangeRoom(currentRoom);

        //Update le visuel de la minimap
        currentRoom.roomSelectedVisual.sprite = _roomVisualManager.GetSelectedVisual(true);

        //Update les boutons
        UpdateButtonStates();
        UpdateRoomDebugName();
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
    
    #endregion FLOOR GENERATION

    #region CHANGE ROOMS
    public void ChangeRoomDirection(RoomDirection direction)
    {
        SaveRoomData();
        currentRoom.roomSelectedVisual.sprite = _roomVisualManager.GetSelectedVisual(false);
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
        currentRoom.roomSelectedVisual.sprite = _roomVisualManager.GetSelectedVisual(true);
        UpdateButtonStates();
        UpdateRoomDebugName();
    }

    public void ChangeRoomMinimap(RoomData room)
    {
        SaveRoomData();
        currentRoom.roomSelectedVisual.sprite = _roomVisualManager.GetSelectedVisual(false);
        currentRoom = room;
        GameManager.Instance.ChangeRoom(room);
        currentRoom.roomSelectedVisual.sprite = _roomVisualManager.GetSelectedVisual(true);
        UpdateButtonStates();
    }

    private void SaveRoomData()
    {
        currentRoom.roomSavedString = GameManager.Instance.gridManager.SaveGridString();
    }

    public void UpdateButtonStates()
    {
        if (currentRoom.currentRoomState == RoomState.Complete)
        {
            buttonRight.SetActive(currentRoom.roomRight != null);
            buttonLeft.SetActive(currentRoom.roomLeft != null);
            buttonUp.SetActive(currentRoom.roomUp != null);
            buttonDown.SetActive(currentRoom.roomDown != null);
        }
        else if(currentRoom.currentRoomState == RoomState.Started)
        {
            buttonRight.SetActive(false);
            buttonLeft.SetActive(false);
            buttonUp.SetActive(false);
            buttonDown.SetActive(false);
        }
    }
    #endregion BUTTON FONCTIONS

    #region DEBUG
    private void UpdateRoomDebugName()
    {
        roomNameDebugText.text = currentRoom.roomSettings.name;
    }
    #endregion DEBUG


}
