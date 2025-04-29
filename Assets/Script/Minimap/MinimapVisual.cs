using System;
using UnityEngine;

public class MinimapVisual : MonoBehaviour
{
    Minimap _minimap;
    VisualManager _visualManager;
    
    
    public void Init()
    {
        _visualManager = GameManager.visualManager;
    }
    
    // A METTRE DANS UNE SCRIPT MINIMAP VISUAL
    public Sprite GetRoomStateVisual(RoomState roomState)
    {
        Sprite roomStateVisual = null;
        switch (roomState)
        {
            case RoomState.FogOfWar:
                roomStateVisual = _visualManager.GetSprite("Cell_Cover");
                break;
            case RoomState.Started:
                roomStateVisual = _visualManager.GetSprite("Cell_State_Clicked");
                break;
            case RoomState.Complete:
                roomStateVisual = _visualManager.GetSprite("Cell_Empty");
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
                roomTypeVisual = _visualManager.GetSprite("Cell_Item_Sword");
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
    
    //DESACTIVER LE GAMEOBJECT A LA PLACE
    public Sprite GetSelectedVisual(bool isSelected)
    {
        Sprite roomSelectedVisual = null;
        if (isSelected)
        {
            roomSelectedVisual = _visualManager.GetSprite("Cell_State_Flag");
        }
        return roomSelectedVisual;
    }
}
