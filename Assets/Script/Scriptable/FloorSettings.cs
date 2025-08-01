using System;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.Timeline;
using Random = UnityEngine.Random;

public enum FloorType
{
    FRT,
    SHI,
    HOU,
    CAV,
    WAT,
}

[CreateAssetMenu(fileName = "FloorSettings", menuName = "MineCrawler/FloorSettings")]
public class FloorSettings : ScriptableObject
{
    #region PARAMETERS

    [Header("GENERAL")] 
    public bool proceduralFloor;
    public VolumeProfile floorBaseVolumeProfile;
    public TimelineAsset introRTC;

    [Header("________PROCEDURAL FLOOR SETTINGS")]
    [Header("FLOOR GRID")]
    [SerializeField] private Vector2Int minGridSize = new Vector2Int(2, 2);
    [SerializeField] private Vector2Int maxGridSize = new Vector2Int(4, 4);

    [Header("ROOM SETTINGS")]
    [SerializeField] [Expandable] public RoomSettings[] roomSettingsList;
    
    [Header("________LOADED FLOOR SETTINGS")]
    public List<LoadedRoomData> loadedRoomDatas;

    [Header("________OBJECTIVES")] 
    public ScriptGraphAsset objectiveGraph;
    public List<FloorObjectives> floorObjectivesList;
    
    [Serializable]
    public struct LoadedRoomData
    {
        public bool startRoom;
        public RoomSettings initRoomSettings;
        public RoomState initRoomState;
        public Vector2Int roomPosition;
        public RoomCompletion.RoomCompletionConditions roomCompletion;
        public RoomCompletion.RoomCompletionConditions roomUnlock;
    }
    
    [Serializable]
    public struct FloorObjectives
    {
        public List<Vector2Int> objectivesPositions;
    }
    #endregion

    public Vector2Int GetProceduralFloorSize()
    {
        int randomRow = Random.Range(minGridSize.x, maxGridSize.x + 1);
        int randomCol = Random.Range(minGridSize.y, maxGridSize.y + 1);
        Vector2Int gridSize = new Vector2Int(randomRow, randomCol);
        return gridSize;
    }
}
