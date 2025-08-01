using DG.Tweening;
using Dida.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

public class RoomAmbianceController : MonoBehaviour
{
    [Header("AMBIANCE / POST PROCESS")]
    public Volume mainColorsVolume;
    public Volume transitionColorsVolume;
    public float visualTransitionDuration;
    
    private GameManager _gameManager;
    private VisualManager _visualManager;
    private VolumeProfile _roomMainProfile;
    private GlobalColorSettings _roomTransitionGlobalColorSettings;
    private TextController _textController;
    
    private Tweener _currentWeightTween;

    public void Init(VisualManager manager)
    {
        _visualManager = manager;
        _gameManager = GameManager.Instance;
        _textController = manager.textController;
    }
    
    public void TransitionVolume(VolumeProfile roomProfile)
    {
        if (roomProfile != null)
        {
            if (roomProfile == _roomMainProfile)
            {
                return;
            }
            transitionColorsVolume.profile = roomProfile;
        }
        else if (_gameManager.FloorManager.currentFloorSetting.floorBaseVolumeProfile != null)
        {
            transitionColorsVolume.profile = _gameManager.FloorManager.currentFloorSetting.floorBaseVolumeProfile;
        }
        else
        {
            if (_gameManager.currentChapterSettings.chapterDefaultColorsVolume == transitionColorsVolume.profile)
            {
                return;
            }
            transitionColorsVolume.profile = _gameManager.currentChapterSettings.chapterDefaultColorsVolume;
        }
        
        //Texts Color Transition
        _textController.UpdateTextColors(visualTransitionDuration);
        
        _currentWeightTween?.Kill();
        //Texts Volume Transition
        _currentWeightTween = VisualUtils.DOWeight(transitionColorsVolume, 1f, visualTransitionDuration)
            .SetEase(Ease.Linear)
            .OnComplete(UpdateVolumeProfile);
    }

    private void UpdateVolumeProfile()
    {
        mainColorsVolume.profile = null;
        mainColorsVolume.profile = transitionColorsVolume.profile;
        transitionColorsVolume.weight = 0;
    }
}
