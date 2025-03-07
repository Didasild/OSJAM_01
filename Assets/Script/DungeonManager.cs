using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

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
    
    //private RoomVisualManager _roomVisualManager;
    private VisualManager _visualManager;
    #endregion

    #region INIT

    public void Init()
    {
        _visualManager = GameManager.VisualManager;
    }
    #endregion

    #region FLOOR GENERATION
    public void GenerateProceduralFloor(FloorSettings floorSetting)
    {
        Vector2Int floorSize = floorSetting.GetProceduralFloorSize();
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
                    roomData.Initialize(gridPosition, RoomCompletionCondition.None, roomSize, offset); // Initialisation avec la position
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
            roomList[i].initRoomSettings = tempRoomSettings[i];
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

        InstanciateFirstRoom(currentRoom);
    }
    #endregion PROCEDRUAL GENERATION

    #region LOAD GENERATION
    public void LoadFloor(FloorSettings floorSetting)
    {
        ClearFloor();
        foreach (FloorSettings.LoadedRoomData loadedRoomData in floorSetting.loadedRoomDatas)
        {
            RoomData roomData = Instantiate(roomPrefab, roomContainer);
            roomList.Add(roomData);
            roomData.transform.SetParent(roomContainer);
            roomData.name = $"Room_"+ loadedRoomData.roomPosition;
            
            roomData.Initialize(loadedRoomData.roomPosition, loadedRoomData.roomCondition, roomSize);
            roomData.startRoom = loadedRoomData.startRoom;
            roomData.initRoomSettings = loadedRoomData.initRoomSettings;
            roomData.InitializeRoomType();
        }
        GiveNeighbors();
        currentRoom = FindStartRoom();
        InstanciateFirstRoom(currentRoom);
    }

    private RoomData FindStartRoom()
    {
        List<RoomData> startRooms = new List<RoomData>();
        
        foreach (RoomData room in roomList)
        {
            if (room.startRoom)
            {
                startRooms.Add(room);
            }
        }
        if (startRooms.Count == 0)
        {
            Debug.LogWarning("Aucune start room trouvée !");
            return null;
        }
        else if (startRooms.Count > 1)
        {
            Debug.LogWarning("Plusieurs Start Rooms!");
        }

        // Sélectionner une room au hasard parmi celles trouvées
        return startRooms[Random.Range(0, startRooms.Count)];
    }
    
    #endregion
    
    #region FLOOR GENERATION METHODS
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

    private void InstanciateFirstRoom(RoomData roomToInstanciate)
    {
        //La génère
        GameManager.Instance.ChangeRoom(roomToInstanciate);

        //Update le visuel de la minimap
        _visualManager.UpdateRoomAmbiance(roomToInstanciate);
        _visualManager.UpdateRoomID(roomToInstanciate);
        roomToInstanciate.roomSelectedVisual.sprite = _visualManager.GetSelectedVisual(true);

        //Update les boutons
        UpdateButtonStates();
        UpdateRoomDebugName();
    }

    #endregion FLOOR GENERATION METHODS

    #region CHANGE ROOM
    public void ChangeRoomDirection(RoomDirection direction)
    {
        ChangeRoomIn();
        switch (direction)
        {
            case RoomDirection.Right:
                if (currentRoom.roomRight != null)
                {
                    InitRoomTransition(currentRoom.roomRight);
                }
                break;
            case RoomDirection.Left:
                if (currentRoom.roomLeft != null)
                {
                    InitRoomTransition(currentRoom.roomLeft);
                }
                break;
            case RoomDirection.Up:
                if (currentRoom.roomUp != null)
                {
                    InitRoomTransition(currentRoom.roomUp);
                }
                break;
            case RoomDirection.Down:
                if (currentRoom.roomDown != null)
                {
                    InitRoomTransition(currentRoom.roomDown);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }

    public void ChangeRoomMinimapIn(RoomData nextRoom)
    {
        ChangeRoomIn();
        InitRoomTransition(nextRoom);
    }
    
    private void InitRoomTransition(RoomData nextRoom)
    {
        _visualManager.UpdateRoomAmbiance(nextRoom);
        Vector2Int roomDirection = GetNextRoomDirection(nextRoom.roomPosition);
        _visualManager.RoomOffsetTransition(roomDirection, nextRoom);
    }

    private void ChangeRoomIn()
    {
        SaveRoomData();
        currentRoom.roomSelectedVisual.sprite = _visualManager.GetSelectedVisual(false);
        DisableButtons();
    }

    public void ChangeRoomOut(RoomData nextRoom)
    {
        currentRoom = nextRoom;
        GameManager.Instance.ChangeRoom(nextRoom);
        
        currentRoom.roomSelectedVisual.sprite = _visualManager.GetSelectedVisual(true);
        
        UpdateButtonStates();
        UpdateRoomDebugName();
    }

    private void DisableButtons()
    {
        buttonRight.SetActive(false);
        buttonLeft.SetActive(false);
        buttonUp.SetActive(false);
        buttonDown.SetActive(false);
    }

    public void UpdateButtonStates()
    {
        switch (currentRoom.currentRoomState)
        {
            case RoomState.Complete:
                buttonRight.SetActive(currentRoom.roomRight != null);
                buttonLeft.SetActive(currentRoom.roomLeft != null);
                buttonUp.SetActive(currentRoom.roomUp != null);
                buttonDown.SetActive(currentRoom.roomDown != null);
                break;
            case RoomState.Started:
                buttonRight.SetActive(false);
                buttonLeft.SetActive(false);
                buttonUp.SetActive(false);
                buttonDown.SetActive(false);
                break;
            case RoomState.FogOfWar:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    #endregion CHANGE ROOM

    #region UTILITARY FUNCTIONS
    private void SaveRoomData()
    {
        currentRoom.roomSavedString = GameManager.Instance.gridManager.SaveGridString();
    }

    private Vector2Int GetNextRoomDirection(Vector2Int nextRoomPosition)
    {
        return new Vector2Int(nextRoomPosition.x - currentRoom.roomPosition.x, nextRoomPosition.y - currentRoom.roomPosition.y);
    }

    private float GetNextRoomDistance(Vector2Int nextRoomPosition)
    {
        return Vector2Int.Distance(currentRoom.roomPosition, nextRoomPosition);
    }
    #endregion UTILITARY FUNCTIONS
    
    #region DEBUG
    private void UpdateRoomDebugName()
    {
        roomNameDebugText.text = currentRoom.initRoomSettings.name;
    }
    #endregion DEBUG


}
