using System.Diagnostics;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

public class VisualEditorManager : VisualManager
{
    #region PARAMETERS
    [Header("_______CELL VISUAL")]
    [Header("ITEMS VISUAL")]
     public Sprite potionSprite;
     public Sprite swordSprite;

    [Header("CELL STATE VISUAL")]
    public Sprite flagSprite;
    public Sprite plantedSwordSprite;
    public Sprite stairSprite;

    [Header("FLOOR EDITOR VISUAL")] 
    public Sprite fowRoom;
    public Sprite startedRoom;
    public Sprite completedRoom;
    
    [Header("_______CELL EDITOR VISUAL")] 
    public Sprite coverSprite;
    public Sprite revealSprite;
    public Sprite mineIconSprite;
    #endregion PARAMETERS

    public Sprite GetRoomSprite(RoomState roomState)
    {
        switch (roomState)
        {
            case RoomState.Started:
                return startedRoom;
            case RoomState.Complete:
                return completedRoom;
            case RoomState.FogOfWar:
                return fowRoom;
        }
        Debug.LogError("Unknown Room State : " + roomState);
        return null;
    }
}
