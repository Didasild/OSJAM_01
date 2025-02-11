using System;
using DG.Tweening;
using Dida.Rendering;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;


public class RoomVisualManager : MonoBehaviour
{
    #region PARAMETERS
    [Header("AMBIANCE / POST PROCESS")]
    public Volume mainColorsVolume;
    public Volume transitionColorsVolume;
    public float visualTransitionDuration;
    
    [Header("_______MINIMAP ROOM STATE VISUAL")]
    public Sprite roomFoWSprite;
    public Sprite roomStartedSprite;
    public Sprite roomCompleteSprite;
    public Sprite roomSelectedSprite;
    
    [Header("_______MINIMAP ROOM TYPE VISUAL")]
    public Sprite roomTypeStairSprite;
    public Sprite roomTypeShopSprite;
    public Sprite roomTypeBossSprite;
    
    private VolumeProfile _roomMainProfile;
    private VisualSettings _roomTransitionVisualSettings;

    private Tweener _currentWeightTween;
    
    #endregion PARAMETERS

    #region INITIALIZATION
    public void Init()
    {
        _roomMainProfile = mainColorsVolume.profile;

    }
    #endregion INITIALIZATION

    #region SET FUNCTIONS
    public void UpdateRoomVisual(RoomData roomData)
    {
        TransitionVolume(roomData.roomSettings.roomColorsVolumeProfile);
    }

    private void TransitionVolume(VolumeProfile roomProfile = null)
    {
        //Check le volume a récup
        if (roomProfile != null)
        {
            if (roomProfile == _roomMainProfile)
            {
                return;
            }
            transitionColorsVolume.profile = roomProfile;
        }
        else
        {
            if (GameManager.Instance.currentChapterSettings.chapterDefaultColorsVolume == transitionColorsVolume.profile)
            {
                return;
            }
            transitionColorsVolume.profile = GameManager.Instance.currentChapterSettings.chapterDefaultColorsVolume;
        }
        
        // Si un tween est déjà en cours, on l'annule
        _currentWeightTween?.Kill();
        //Fait la transition si le volume est bon
        _currentWeightTween = DOWeight(transitionColorsVolume, 1f, visualTransitionDuration)
            .SetEase(Ease.Linear)
            .OnComplete(UpdateVolumeProfile);
    }
    
    private static Tweener DOWeight(Volume volume, float endValue, float duration)
    {
        return DOTween.To(() => volume.weight, x => volume.weight = x, endValue, duration);
    }

    private void UpdateVolumeProfile()
    {
        mainColorsVolume.profile = null;
        mainColorsVolume.profile = transitionColorsVolume.profile;
        transitionColorsVolume.weight = 0;
    }
    #endregion SET FUNCTIONS
    
    #region GET FUNCTIONS
    public Sprite GetRoomStateVisual(RoomState roomState)
    {
        Sprite roomStateVisual = null;
        switch (roomState)
        {
            case RoomState.FogOfWar:
                roomStateVisual = roomFoWSprite;
                break;
            case RoomState.Started:
                roomStateVisual = roomStartedSprite;
                break;
            case RoomState.Complete:
                roomStateVisual = roomCompleteSprite;
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
                roomTypeVisual = roomTypeStairSprite;
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
            roomSelectedVisual = roomSelectedSprite;
        }
        return roomSelectedVisual;
    }
    

    #endregion
    

    

    

}
