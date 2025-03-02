using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteInEditMode]
public class FloorEditor : MonoBehaviour
{
    #region VARIABLES
    [Header("SETUP")]
    public VisualManager visualManager;
    public RoomEditorObject roomEditorObjectPrefab;
    public float roomOffset;
    
    [Header("ROOM GENERATION")]
    public RoomEditorObject selectedRoomEditorObject;
    public RoomSettings roomSettingsToLoad;
    public RoomState roomStateToLoad;
    public bool isStartRoom;
    
    [Header("LOAD DATA")]
    public FloorSettings floorToLoad;
    public List<FloorSettings.LoadedRoomData> roomDatasToSave;
    
    public List<RoomEditorObject> roomEditorObjects = new List<RoomEditorObject>();
    #endregion VARIABLES
    
    #region LOAD FLOOR
    private void LoadFloor(FloorSettings floorReference)
    {
        ClearFloor();
        roomDatasToSave = floorReference.loadedRoomDatas;

        for (int i = 0; i < roomDatasToSave.Count; i++)
        {
            InstantiateRoomFromSettings(roomDatasToSave, i);
        }
    }
    private void InstantiateRoomFromSettings(List<FloorSettings.LoadedRoomData> roomDatas, int i)
    {
        GenerateRoom(roomDatas[i].roomPosition, InitRoomPosition(roomDatas[i].roomPosition), roomDatas[i].initRoomSettings, roomDatas[i].currentRoomState);
    }

    private Vector3 InitRoomPosition(Vector2Int roomPosition)
    {
        return new Vector3(roomPosition.x * roomOffset, roomPosition.y * roomOffset, 0);
    }
    #endregion LOAD FLOOR

    #region GENERATION
    public void GenerateFirstRoom()
    {
        ClearFloor();
        GenerateRoom(Vector2Int.zero, Vector3.zero, roomSettingsToLoad, roomStateToLoad);
    }

    public void GenerateNeighborRoom(string direction)
    {
        Vector2Int roomNeighborOffset = Vector2Int.zero;
        switch (direction)
        {
            case "up":
                roomNeighborOffset = Vector2Int.up;
                break;
            case "down":
                roomNeighborOffset = Vector2Int.down;
                break;
            case "left":
                roomNeighborOffset = Vector2Int.left;
                break;
            case "right":
                roomNeighborOffset = Vector2Int.right;
                break;
        }
        GenerateRoom(selectedRoomEditorObject.roomPosition + roomNeighborOffset, InitRoomPosition(selectedRoomEditorObject.roomPosition + roomNeighborOffset), roomSettingsToLoad, roomStateToLoad);
    }
    #endregion

    #region METHODS
    private void GenerateRoom(Vector2Int roomGridPosition, Vector3 roomWorldPosition, RoomSettings roomSettings, RoomState roomState)
    {
        RoomEditorObject roomObject = Instantiate(roomEditorObjectPrefab, roomWorldPosition , Quaternion.identity, transform);
        roomObject.Init(this, roomSettingsToLoad, roomGridPosition, roomState);
        roomEditorObjects.Add(roomObject);
        selectedRoomEditorObject = roomObject;
        UpdateSelectedVisual();
    }
    public void ClearFloor()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
        roomEditorObjects = new List<RoomEditorObject>();
        roomDatasToSave = new List<FloorSettings.LoadedRoomData>();
        selectedRoomEditorObject = null;
    }
    
    private void RemoveRoom()
    {
        
    }

    public void UpdateSelectedVisual()
    {
        foreach (RoomEditorObject roomEditorObject in roomEditorObjects)
        {
            roomEditorObject.selectedVisual.SetActive(false);
        }
        selectedRoomEditorObject.selectedVisual.SetActive(true);
    }
    #endregion METHODS

    #region DEBUG
    public void LoadFloor()
    {
        LoadFloor(floorToLoad);
    }
    #endregion DEBUG
}
