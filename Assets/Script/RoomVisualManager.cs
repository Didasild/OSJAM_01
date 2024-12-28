using UnityEngine;
using UnityEngine.UI;

public class RoomVisualManager : MonoBehaviour
{
    [Header("_______MINIMAP ROOM STATE VISUAL")]
    public Sprite roomFoWSprite;
    public Sprite roomStartedSprite;
    public Sprite roomCompleteSprite;
    public Sprite roomSelectedSprite;
    
    [Header("_______MINIMAP ROOM TYPE VISUAL")]
    public Sprite roomTypeStairSprite;
    public Sprite roomTypeShopSprite;
    public Sprite roomTypeBossSprite;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Sprite GetRoomStateVisual(RoomState roomState)
    {
        Sprite roomStateVisual = null;
        if (roomState == RoomState.FogOfWar)
        {
            roomStateVisual = roomFoWSprite;
        }
        else if (roomState == RoomState.Started)
        {
            roomStateVisual = roomStartedSprite;
        }
        else if ( roomState == RoomState.Complete)
        {
            roomStateVisual = roomCompleteSprite;
        }
        return roomStateVisual;
    }

    public Sprite GetRoomTypeVisual(RoomType roomType)
    {
        Sprite roomTypeVisual = null;
        if (roomType == RoomType.Base)
        {
            return null;
        }
        else if (roomType == RoomType.Stair)
        {
            roomTypeVisual = roomTypeStairSprite;
        }

        return roomTypeVisual;
    }

    public Sprite GetSelectedVisual(bool isSelected)
    {
        Sprite roomSelectedVisual = null;
        if (isSelected)
        {
            roomSelectedVisual = roomSelectedSprite;
        }
        return roomSelectedVisual;
    }
}
