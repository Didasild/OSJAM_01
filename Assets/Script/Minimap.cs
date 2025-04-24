using System;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    private FloorManager _floorManager;
    private VisualManager _visualManager;
    
    [Header("GENERAL SETTINGS")]
    public Transform roomContainer;
    public float roomSize;
    
    public void Init()
    {
        _floorManager = GameManager.Instance.floorManager;
        _visualManager = GameManager.visualManager;
    }

    public void SetRoomPosition(RoomData roomData, Vector2Int position)
    {
        roomData.transform.SetParent(roomContainer);
        roomData.roomPosition = position;
        
        // Calculez la position dans le monde
        Vector3 worldPosition = new Vector3(position.x * roomSize, position.y * roomSize, 0);

        // Placez le GameObject à cette position
        roomData.transform.localPosition = worldPosition;
    }
    
    // A METTRE DANS UNE SCRIPT MINIMAP VISUAL
    public Sprite GetRoomStateVisual(RoomState roomState)
    {
        Sprite roomStateVisual = null;
        switch (roomState)
        {
            case RoomState.FogOfWar:
                roomStateVisual = _visualManager.GetSprite("TBD");
                break;
            case RoomState.Started:
                roomStateVisual = _visualManager.GetSprite("TBD");
                break;
            case RoomState.Complete:
                roomStateVisual = _visualManager.GetSprite("TBD");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(roomState), roomState, null);
        }
        return roomStateVisual;
    }

    public Sprite GetRoomTypeVisual(RoomType roomType)
    {
        Sprite roomTypeVisual = null;
        switch (roomType)
        {
            case RoomType.Base:
                return null;
            case RoomType.Stair:
                roomTypeVisual = _visualManager.GetSprite("TBD");
                break;
            case RoomType.Shop:
                break;
            case RoomType.Sword:
                break;
            case RoomType.Potion:
                break;
            case RoomType.Boss:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(roomType), roomType, null);
        }

        return roomTypeVisual;
    }
    
    public Sprite GetSelectedVisual(bool isSelected)
    {
        Sprite roomSelectedVisual = null;
        if (isSelected)
        {
            roomSelectedVisual = _visualManager.GetSprite("TBD");
        }
        return roomSelectedVisual;
    }
}
