using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class RtcCustomSignalReceiver : MonoBehaviour, INotificationReceiver
{
    private EventsController _events;
    private PlayableDirector _director;
    public void Init(EventsController events)
    {
        _events = events;
        _director = _events.RtcManager.PlayableDirector;
    }
    
    public void OnNotify(Playable origin, INotification notification, object context)
    {
        switch (notification)
        {
            case MarkerStartDialogSignal dialogSignal:
                if (dialogSignal.dialogSettings != null)
                    _events.StartDialogSequenceEvent(dialogSignal.dialogSettings);
                break;
            
            case MarkerCloseDialogBox closeDialogBox:
                _events.CloseDialogBoxEvent();
                break;
            
            case MarkerRoomAmbianceTransition ambianceSignal:
                if (ambianceSignal.newVolumeProfile != null)
                    _events.TransitionVolumeEvent(ambianceSignal.newVolumeProfile);
                break;
            
            case MarkerShakeCamera shakeCamera:
                _events.ShakeCameraEvent(shakeCamera.shakeDuration, shakeCamera.shakeType );
                break;
            
            case MarkerDestroyCells deleteCells:
                _events.DestroyNbOfCellType(deleteCells.numberOfCellsToDelete, deleteCells.cellType, deleteCells.cellState);
                break;
            
            case MarkerHitFeedback hitFeedback:
                _events.HitFeedbackEvent();
                break;
            
            case MarkerLooseHp looseHp:
                _events.LooseHpEvent(looseHp.hpLoose);
                break;
            
            case MarkerSetCurrentHp setCurrentHp:
                _events.SetCurrentHpEvent(setCurrentHp.currentHp);
                break;
            
            case MarkerUpdateRtcVolume updateRtcVolume:
                _events.RtcManager.UpdateVolume(updateRtcVolume.volumeType, updateRtcVolume.newVolumeProfile);
                break;
            
            case SignalEmitter:
                break;
            
            default:
                Debug.Log($"Unhandled signal type: {notification.GetType().Name}");
                break;
        }
    }
}