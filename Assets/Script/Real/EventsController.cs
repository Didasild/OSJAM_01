using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.Timeline;

public class EventsController : MonoBehaviour
{
    private GameManager _gameManager;
    private VisualManager _visualManager;
    private FloorManager _floorManager;
    private Health _health;
    private Dialog _dialog;
    
    [Header("RTC COMPONENTS")]
    [SerializeField] private RtcCustomSignalReceiver _rtcReceiver;
    [SerializeField] private PlayableDirector _playableDirector;

    public void Init(FloorManager floorManager, Health health)
    {
        _gameManager = GameManager.Instance;
        _visualManager = GameManager.VisualManager;
        _floorManager = floorManager;
        _dialog = GameManager.Instance.Dialog;
        _health = health;
        _rtcReceiver.Init(this);
    }
    
    public void VSIncreaseMaxHealth(int increment)
    {
        _health.IncreaseMaxHealth(increment);
    }
    
    public RoomData GetRoomDataFromPosition(Vector2Int roomPosition)
    {
        return _floorManager.GetRoomDataFromPosition(roomPosition);
    }

    public NPC GetNpcFromDialogSettings(RoomData roomData, NpcDialogsSettings npcDialogsSettings)
    {
        return roomData.GetNpcFromDialogSettings(npcDialogsSettings);
    }

    public void PlayRtc(TimelineAsset timelineAsset)
    {
        _playableDirector.playableAsset = timelineAsset;
        _playableDirector.Play();
    }

    public void StopRtc()
    {
        _playableDirector.Stop();
    }

    public void SkipRtc()
    {
        _playableDirector.time = _playableDirector.duration;
        _playableDirector.Evaluate();
    }

    public void GoToNextFloor()
    {
        _gameManager.GoToNextFloor();
    }

    #region RTC SIGNALS FUNCTIONS
    public void StartDialogSequence(NpcDialogsSettings npcDialogsSettings)
    {
        _dialog.StartEventDialogSequence(npcDialogsSettings);
    }

    public void LooseHp(int hpLoose)
    {
        _health.DecreaseHealth(hpLoose);
    }

    public void TransitionVolume(VolumeProfile newVolumeProfile)
    {
        _visualManager.roomAmbianceController.TransitionVolume(newVolumeProfile);
    }

    public void ShakeCamera(float duration, ShakeType shakeType = ShakeType.little)
    {
        switch (shakeType)
        {
            case ShakeType.little:
                _visualManager.shakeCamController.LittleShakeCamera(duration);
                break;
            case ShakeType.medium:
                _visualManager.shakeCamController.MidShakeCamera(duration);
                break;
            case ShakeType.big:
                _visualManager.shakeCamController.BigShakeCamera(duration);
                break;
            default:
                Debug.LogWarning($"Unhandled shake type: {shakeType}");
                break;
        }
    }

    public void HitFeedback()
    {
        _visualManager.fullScreenFeedbackController.HitFeedback();
    }
    
    public void DestroyNbOfCellType(int cellNumber, CellType cellType, CellState cellState)
    {
        _gameManager.GridManager.DestroyNbOfCellType(cellNumber, cellType, cellState);
    }
    #endregion RTC SIGNALS FUNCTIONS
    
}
