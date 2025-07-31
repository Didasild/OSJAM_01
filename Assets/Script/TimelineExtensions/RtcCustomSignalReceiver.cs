using UnityEngine;
using UnityEngine.Playables;

public class RtcCustomSignalReceiver : MonoBehaviour, INotificationReceiver
{
    private EventsController _events;
    public void Init(EventsController events)
    {
        _events = events;
    }
    
    public void OnNotify(Playable origin, INotification notification, object context)
    {
        switch (notification)
        {
            case StartDialogSignal dialogSignal:
                if (dialogSignal.dialogSettings != null)
                    _events.StartDialogSequence(dialogSignal.dialogSettings);
                break;

            case RoomAmbianceTransition ambianceSignal:
                if (ambianceSignal.newVolumeProfile != null)
                    _events.TransitionVolume(ambianceSignal.newVolumeProfile);
                break;
            
            case ShakeCamera shakeCamera:
                _events.ShakeCamera(shakeCamera.shakeDuration, shakeCamera.shakeType );
                break;
            
            default:
                Debug.Log($"Unhandled signal type: {notification.GetType().Name}");
                break;
        }
    }
}