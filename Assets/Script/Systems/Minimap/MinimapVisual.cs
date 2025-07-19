using System;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class MinimapVisual : MonoBehaviour
{
    [Header("ROOM TRANSFORM")]
    public Transform fowTransform;
    public Transform hideTransform;
    public Transform startedTransform;
    public Transform completedTransform;
    public Transform selectedRoom;
    
    [Header("CONTAINER")]
    public GameObject minimapContainer;
    public GameObject minimapContent;
    
    private Minimap _minimap;
    private VisualManager _visualManager;
    private UiTransition _uiTransition;
    
    
    public void Init()
    {
        _visualManager = GameManager.visualManager;
        _uiTransition = GetComponent<UiTransition>();
    }

    public Sprite GetRoomStateVisual(RoomState roomState)
    {
        Sprite roomStateVisual = null;
        switch (roomState)
        {
            case RoomState.FogOfWar:
                roomStateVisual = _visualManager.GetSprite("MM_RoomFoW");
                break;
            case RoomState.Hide:
                roomStateVisual = _visualManager.GetSprite("MM_RoomFoW");
                break;
            case RoomState.StartedLock:
                roomStateVisual = _visualManager.GetSprite("MM_RoomStarted");
                break;
            case RoomState.StartedUnlock:
                roomStateVisual = _visualManager.GetSprite("MM_RoomStarted");
                break;
            case RoomState.Complete:
                roomStateVisual = _visualManager.GetSprite("MM_RoomComplete");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(roomState), roomState, $"Unhandled RoomState in GetRoomStateVisual: {roomState}");
        }
        return roomStateVisual;
    }

    public Transform GetRoomNewParent(RoomState roomState)
    {
        Transform roomNewParent = null;
        switch (roomState)
        {
            case RoomState.FogOfWar:
                roomNewParent = fowTransform;
                break;
            case RoomState.Hide:
                roomNewParent = hideTransform;
                break;
            case RoomState.StartedLock:
                roomNewParent = startedTransform;
                break;
            case RoomState.StartedUnlock:
                roomNewParent = startedTransform;
                break;
            case RoomState.Complete:
                roomNewParent = completedTransform;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(roomState), roomState, null);
        }
        return roomNewParent;
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
    public void ActiveSelectedVisual(RoomData roomData,bool isSelected)
    {
        roomData.roomSelectedVisual.gameObject.SetActive(isSelected);
        if (isSelected)
        {
            roomData.transform.SetParent(selectedRoom);
        }
        else
        {
            roomData.transform.SetParent(GetRoomNewParent(roomData.currentRoomState));
        }
    }
    
    public void MinimapAppear()
    {
        minimapContainer.SetActive(true);
        _uiTransition.StartTransition();
        DOVirtual.DelayedCall(_uiTransition.transitionDuration / 1.5f, () =>
        {
            minimapContent.SetActive(true);
        });
    }

    public void MinimapDisappear()
    {
        minimapContent.SetActive(false);
        _uiTransition.StartTransition(false);
        DOVirtual.DelayedCall(_uiTransition.transitionDuration, () =>
        {
            minimapContainer.SetActive(false);
        });
    }
}
