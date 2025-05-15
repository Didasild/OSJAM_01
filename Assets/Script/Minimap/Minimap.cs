using System;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    private FloorManager _floorManager;
    private VisualManager _visualManager;
    private MinimapVisual _minimapVisual;
    public Transform minimapContent;
    
    [Header("GENERAL SETTINGS")]
    public float roomSize;
    
    public void Init()
    {
        _floorManager = GameManager.Instance.floorManager;
        _visualManager = GameManager.visualManager;
        _minimapVisual = gameObject.GetComponent<MinimapVisual>();
        
        _minimapVisual.Init();
    }

    public void SetRoomPosition(RoomData roomData, Vector2Int position)
    {
        roomData.transform.SetParent(_minimapVisual.GetRoomNewParent(RoomState.FogOfWar));
        roomData.roomPosition = position;
        
        // Calculez la position dans le monde
        Vector3 worldPosition = new Vector3(position.x * roomSize, position.y * roomSize, 0);

        // Placez le GameObject à cette position
        roomData.transform.localPosition = worldPosition;
    }
    
    public void ChangeOnClickIn(RoomData nextRoom)
    {
        _floorManager.ChangeRoomIn();
        _floorManager.InitRoomTransition(nextRoom);
    }
}
