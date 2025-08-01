using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
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

public class FloorManager : MonoBehaviour
{
    #region PARAMETERS
    [Header("GENERAL SETTINGS")]
    public RoomData roomPrefab;
    public FloorObjectivesController floorObjectivesController;

    [Header("FLOOR SETTINGS")]
    public Minimap minimap;
    [ReadOnly] public FloorSettings currentFloorSetting;
    public FloorSettings[] floorSettingsList;

    [Header("ROOM SETTINGS")]
    [ReadOnly] public RoomData currentRoom;
    [ReadOnly] public RoomState currentRoomState;
    public List<RoomData> roomList = new List<RoomData> ();

    [Header("BUTTONS")]
    public GameObject buttonRight;
    public GameObject buttonLeft;
    public GameObject buttonUp;
    public GameObject buttonDown;

    [Header("DEBUG")] public TMP_Text roomPositionDebugText;
    public TMP_Text roomNameDebugText;
    
    public EventsController eventsController;
    private RoomSettings[] _roomSettingsList;
    private bool initialized = false;
    private VisualManager _visualManager;
    [HideInInspector] public bool introPlayed = false;
    #endregion

    public void Update()
    {
        if (!initialized)
        {
            return;
        }
        currentRoomState = currentRoom.currentRoomState;
    }

    #region INIT
    public void Init()
    {
        _visualManager = GameManager.VisualManager;
        minimap.Init();
        floorObjectivesController.Init(this);
        initialized = true;

        eventsController = GetComponent<EventsController>();
        eventsController.Init(this, GameManager.Instance.Player.Health);
    }
    #endregion

    #region PROCEDRUAL GENERATION
    public void GenerateProceduralFloor(FloorSettings floorSetting)
    {
        _roomSettingsList = currentFloorSetting.roomSettingsList;
        
        ClearFloor();
        
        //Generate Room in position
        Vector2Int floorSize = floorSetting.GetProceduralFloorSize();
        for (int y = 0; y < floorSize.y; y++)
        {
            for (int x = 0; x < floorSize.x; x++)
            {
                // Calculer la position sur la grille
                Vector2Int gridPosition = new Vector2Int(x, y);

                // Instancier le GameObject room
                RoomData roomData = Instantiate(roomPrefab);
                if (roomData != null)
                {
                    FloorSettings.LoadedRoomData newRomDataInfos = new FloorSettings.LoadedRoomData();
                    newRomDataInfos.roomPosition = gridPosition;
                    newRomDataInfos.roomCompletion = RoomCompletion.RoomCompletionConditions.Default;
                    newRomDataInfos.roomUnlock = RoomCompletion.RoomCompletionConditions.Default;
                    roomData.Initialize(newRomDataInfos, minimap);
                    roomList.Add(roomData);
                    minimap.SetRoomPosition(roomData, gridPosition);
                }
                
                // Nommer la room pour faciliter le debug
                roomData.name = $"Room_{x}_{y}";
            }
        }
        
        List<RoomSettings> availableRoomList = new List<RoomSettings>(_roomSettingsList);
        Debug.Log(availableRoomList.Count);

        GiveNeighbors();
        AssignRoomSettings(availableRoomList);
    }

    private void AssignRoomSettings(List<RoomSettings> availableRoomList)
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
        minimap.FocusOnSelectedRoom(currentRoom);
    }
    #endregion PROCEDRUAL GENERATION

    #region LOAD GENERATION
    public void LoadFloor(FloorSettings floorSetting)
    {
        ClearFloor();
        foreach (FloorSettings.LoadedRoomData loadedRoomData in floorSetting.loadedRoomDatas)
        {
            RoomData roomData = Instantiate(roomPrefab);
            roomList.Add(roomData);
            
            roomData.Initialize(loadedRoomData, minimap);
            minimap.SetRoomPosition(roomData, loadedRoomData.roomPosition);
            roomData.name = $"Room_"+ loadedRoomData.roomPosition;
            
            roomData.startRoom = loadedRoomData.startRoom;
            roomData.initRoomSettings = loadedRoomData.initRoomSettings;
            roomData.InitializeRoomType();
        }
        GiveNeighbors();
        currentRoom = FindStartRoom();
        InstanciateFirstRoom(currentRoom);
        
        minimap.FocusOnSelectedRoom(currentRoom);
        floorObjectivesController.SetFloorObjectives();
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

            // Search Neighbors
            room.roomUp = FindRoomAtPosition(position + Vector2Int.up);
            room.roomDown = FindRoomAtPosition(position + Vector2Int.down);
            room.roomLeft = FindRoomAtPosition(position + Vector2Int.left);
            room.roomRight = FindRoomAtPosition(position + Vector2Int.right);
        }
    }
    
    private RoomData FindRoomAtPosition(Vector2Int position)
    {
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
        GameManager.Instance.ChangeRoom(roomToInstanciate);

        //Update Ambiance
        _visualManager.UpdateRoomAmbiance(roomToInstanciate);
        _visualManager.UpdateRoomID(roomToInstanciate);
        
        //minimap to move
        _visualManager.minimapVisual.ActiveSelectedVisual(roomToInstanciate, true);
        
        //Update la room
        roomToInstanciate.ChangeRoomSate(roomToInstanciate.currentRoomState);
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
    
    public void InitRoomTransition(RoomData nextRoom)
    {
        _visualManager.UpdateRoomAmbiance(nextRoom);
        Vector2Int roomDirection = GetNextRoomDirection(nextRoom.roomPosition);
        _visualManager.roomTransitionController.RoomOffsetTransition(roomDirection, nextRoom);
        minimap.FocusOnSelectedRoom(nextRoom);
    }

    public void ChangeRoomIn()
    {
        SaveRoomData();
        
        GameManager.Instance.Dialog.DialogVisual.DialogDisparition();
        
        _visualManager.minimapVisual.ActiveSelectedVisual(currentRoom, false);
        DisableButtons();
    }

    public void ChangeRoomOut(RoomData nextRoom)
    {
        currentRoom = nextRoom;
        GameManager.Instance.ChangeRoom(nextRoom);
        
        _visualManager.minimapVisual.ActiveSelectedVisual(currentRoom, true);

        if (currentRoom.isLocked == false)
        {
            _visualManager.centralFeedbackController.RooUnlockFeedback(currentRoom);
            UpdateButtonStates();
        }
        
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
                buttonRight.SetActive(CheckNeighborState(currentRoom.roomRight));
                buttonLeft.SetActive(CheckNeighborState(currentRoom.roomLeft));
                buttonUp.SetActive(CheckNeighborState(currentRoom.roomUp));
                buttonDown.SetActive(CheckNeighborState(currentRoom.roomDown));
                break;
            case RoomState.StartedLock:
                buttonRight.SetActive(false);
                buttonLeft.SetActive(false);
                buttonUp.SetActive(false);
                buttonDown.SetActive(false);
                break;
            case RoomState.StartedUnlock:
                buttonRight.SetActive(CheckNeighborState(currentRoom.roomRight));
                buttonLeft.SetActive(CheckNeighborState(currentRoom.roomLeft));
                buttonUp.SetActive(CheckNeighborState(currentRoom.roomUp));
                buttonDown.SetActive(CheckNeighborState(currentRoom.roomDown));
                break;
            case RoomState.Hide:
                break;
            case RoomState.FogOfWar:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    #endregion CHANGE ROOM

    #region UTILS
    private void SaveRoomData()
    {
        currentRoom.roomSavedString = GameManager.Instance.GridManager.SaveGridString();
    }

    private Vector2Int GetNextRoomDirection(Vector2Int nextRoomPosition)
    {
        return new Vector2Int(nextRoomPosition.x - currentRoom.roomPosition.x, nextRoomPosition.y - currentRoom.roomPosition.y);
    }
    
    
    public Boolean CheckNeighborState(RoomData neighborRoom)
    {
        if (neighborRoom != null && neighborRoom.currentRoomState != RoomState.Hide)
        {
            return true;
        }
        return false;
    }
    
//A METTRE DANS VISUAL SCRIPTING UTILS?
    public RoomData GetRoomDataFromPosition(Vector2Int roomPosition)
    {
        foreach (RoomData roomData in roomList)
        {
            if (roomData.roomPosition == roomPosition)
            {
                return roomData;
            }
        }
        return null;
    }

    
    #endregion UTILS
    
    #region DEBUG
    private void UpdateRoomDebugName()
    {
        roomPositionDebugText.text = currentRoom.roomPosition.ToString();
        roomNameDebugText.text = currentRoom.initRoomSettings.name;
    }
    #endregion DEBUG
    
}
