using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    private FloorManager _floorManager;
    private VisualManager _visualManager;
    private MinimapVisual _minimapVisual;
    public RectTransform minimapContentTransform;
    
    [Header("GENERAL SETTINGS")]
    public float roomSize;

    [Header("FOCUS BUTTON SETTINGS")] 
    public float lerpDuration = 0.3f;
    public Ease lerpEase = Ease.OutQuad;
    
    
    public void Init()
    {
        _floorManager = GameManager.Instance.FloorManager;
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
    
    public void FocusOnSelectedRoom(RoomData selectedRoomData)
    {
        Vector2Int selectedRoomPosition = selectedRoomData.roomPosition;
        Vector2 targetPosition = new Vector2(-selectedRoomPosition.x * roomSize, -selectedRoomPosition.y * roomSize);

        // Annule tout tween précédent sur ce RectTransform si nécessaire
        minimapContentTransform.DOKill();

        // Lance le tween
        minimapContentTransform.DOAnchorPos(targetPosition, lerpDuration)
            .SetEase(lerpEase);
    }
    
    public void OnFocusButtonClicked()
    {
        FocusOnSelectedRoom(_floorManager.currentRoom);
    }
}
