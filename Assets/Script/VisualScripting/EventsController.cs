using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.Timeline;

public class EventsController : MonoBehaviour
{
    private GameManager _gameManager;
    private FloorManager _floorManager;
    private Health _health;
    private Dialog _dialog;
    
    [Header("RTC COMPONENTS")]
    [SerializeField] private RtcCustomSignalReceiver _rtcReceiver;
    [SerializeField] private PlayableDirector _playableDirector;

    public void Init(FloorManager floorManager, Health health)
    {
        _gameManager = GameManager.Instance;
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

    public void PlayRTC(TimelineAsset timelineAsset)
    {
        _playableDirector.playableAsset = timelineAsset;
        _playableDirector.Play();
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

    public void TransitionVolume(VolumeProfile newVolumeProfile)
    {
        GameManager.VisualManager.roomAmbianceController.TransitionVolume(newVolumeProfile);
    }

    public void ShakeCamera(float duration, ShakeType shakeType = ShakeType.little)
    {
        switch (shakeType)
        {
            case ShakeType.little:
                GameManager.VisualManager.shakeCamController.LittleShakeCamera(duration);
                break;
            case ShakeType.medium:
                GameManager.VisualManager.shakeCamController.MidShakeCamera(duration);
                break;
            case ShakeType.big:
                GameManager.VisualManager.shakeCamController.BigShakeCamera(duration);
                break;
            default:
                Debug.LogWarning($"Unhandled shake type: {shakeType}");
                break;
        }
    }
    
    public void ChangeListOfCellType(int cellNumber, CellType cellType)
    {
        
    }
    #endregion RTC SIGNALS FUNCTIONS
    
}
