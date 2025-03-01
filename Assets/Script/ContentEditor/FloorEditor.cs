using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteInEditMode]
public class FloorEditor : MonoBehaviour
{
    [Header("SETUP")]
    public VisualManager visualManager;
    public RoomEditorObject roomEditorObjectPrefab;
    public RoomEditorObject selectedRoomEditorObject;
    public float roomOffset;
    
    [Header("LOAD DATA")]
    public FloorSettings floorToLoad;
    public List<FloorSettings.LoadedRoomData> loadedRoomDatas;
    
    public List<RoomEditorObject> roomEditorObjects = new List<RoomEditorObject>();

    [Button]
    private void DebugLoadFloor()
    {
        LoadFloor(floorToLoad);
    }
    private void LoadFloor(FloorSettings floorReference)
    {
        ClearFloor();
        loadedRoomDatas = floorReference.loadedRoomDatas;

        for (int i = 0; i < loadedRoomDatas.Count; i++)
        {
            InstantiateRoomFromSettings(loadedRoomDatas, i);
        }
    }
    private void InstantiateRoomFromSettings(List<FloorSettings.LoadedRoomData> roomDatas, int i)
    {
        RoomEditorObject roomObject = Instantiate(roomEditorObjectPrefab, InitRoomPosition(roomDatas[i]), Quaternion.identity, transform);
        roomEditorObjects.Add(roomObject);
        roomObject.Init(this, roomDatas[i].initRoomSettings, roomDatas[i].roomPosition, roomDatas[i].currentRoomState);
    }

    private Vector3 InitRoomPosition(FloorSettings.LoadedRoomData roomData)
    {
        return new Vector3(roomData.roomPosition.x * roomOffset, roomData.roomPosition.y * roomOffset, 0);
    }

    public void GenerateNewRoom()
    {
        
    }

    private void ClearFloor()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
        roomEditorObjects = new List<RoomEditorObject>();
        loadedRoomDatas = new List<FloorSettings.LoadedRoomData>();
    }
    
    [Button]
    private void addRoomTop()
    {
        
    }
    [Button]
    private void addRoomLeft()
    {
        
    }
    [Button]
    private void addRoomRight()
    {
        
    }
    [Button]
    private void addRoomBot()
    {
        
    }
        
    [Button]
    private void RemoveRoom()
    {
        
    }
}
