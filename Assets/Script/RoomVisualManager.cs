using System;
using DG.Tweening;
using Dida.Rendering;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.VFX;

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
    private VisualSettings _roomVisualSettings;
    private VisualSettings _roomTransitionVisualSettings;
    
    #endregion PARAMETERS

    #region INITIALIZATION
    public void Init()
    {
        _roomMainProfile = mainColorsVolume.profile;
        if (_roomMainProfile.TryGet(out _roomVisualSettings)){ }

    }
    #endregion INITIALIZATION

    #region SET FUNCTIONS
    public void UpdateRoomVisual(RoomData roomData)
    {
        //UNE CONDITION QUI CHECK SI C'EST LE MEME PROFILE ET RETURN SI C'EST LE CAS
        if (roomData.roomSettings.roomColorsVolumeProfile != null)
        {
            if (roomData.roomSettings.roomColorsVolumeProfile.TryGet(out _roomTransitionVisualSettings)){ }
        }
        else
        {
            if (GameManager.Instance.currentChapterSettings.chapterDefaultColorsVolume.TryGet(out _roomTransitionVisualSettings)){ }
        }
        TransitionVolume();
    }

    private void TransitionVolume()
    {
        //ICI
        //Transition avec le poids puis applique le profile de transition au main et clean le transition
    }
    
    //A DELETE
    // private void ApplyPaletteToVolume(ColorPaletteScriptable colorPaletteToApply)
    // {
    //     if (!_roomMainProfile.TryGet(out _roomVisualSettings)) return;
    //
    //     // Lancer une transition pour chaque couleur
    //     TransitionColor(() => _roomVisualSettings.Color1.value, x => _roomVisualSettings.Color1.value = x, colorPaletteToApply.colors[0], visualTransitionDuration);
    //     TransitionColor(() => _roomVisualSettings.Color2.value, x => _roomVisualSettings.Color2.value = x, colorPaletteToApply.colors[1], visualTransitionDuration);
    //     TransitionColor(() => _roomVisualSettings.Color3.value, x => _roomVisualSettings.Color3.value = x, colorPaletteToApply.colors[2], visualTransitionDuration);
    //     TransitionColor(() => _roomVisualSettings.Color4.value, x => _roomVisualSettings.Color4.value = x, colorPaletteToApply.colors[3], visualTransitionDuration);
    //     TransitionColor(() => _roomVisualSettings.Color5.value, x => _roomVisualSettings.Color5.value = x, colorPaletteToApply.colors[4], visualTransitionDuration);
    //     TransitionColor(() => _roomVisualSettings.Color6.value, x => _roomVisualSettings.Color6.value = x, colorPaletteToApply.colors[5], visualTransitionDuration);
    // }
    // private void TransitionColor(Func<Color> getter, Action<Color> setter, Color newColor, float transitionDuration)
    // {
    //     Color startColor = getter(); // Récupère la couleur actuelle
    //     DOTween.To(() => startColor, x => { startColor = x; setter(x); }, newColor, transitionDuration);
    // }
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
