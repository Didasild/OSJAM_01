using UnityEngine;

public class RoomVisualManager : MonoBehaviour
{
    [Header("_______MINIMAP ROOM VISUAL")]
    public Sprite roomFoWSprite;
    public Sprite roomStartedSprite;
    public Sprite roomCompleteSprite;
    public Sprite roomSelectedSprite;
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
