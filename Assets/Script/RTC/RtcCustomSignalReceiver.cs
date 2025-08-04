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

        if (_director != null)
        {
            _director.RebuildGraph();
            int outputCount = _director.playableGraph.GetOutputCount();

            for (int i = 0; i < outputCount; i++)
            {
                PlayableOutput output = _director.playableGraph.GetOutput(i);
                output.AddNotificationReceiver(this);
            }
        }
    }
    
    public void OnNotify(Playable origin, INotification notification, object context)
    {
        switch (notification)
        {
            case StartDialogSignal dialogSignal:
                if (dialogSignal.dialogSettings != null)
                    _events.StartDialogSequence(dialogSignal.dialogSettings);
                break;
            
            case CloseDialogBox closeDialogBox:
                _events.CloseDialogBox();
                break;

            case RoomAmbianceTransition ambianceSignal:
                if (ambianceSignal.newVolumeProfile != null)
                    _events.TransitionVolume(ambianceSignal.newVolumeProfile);
                break;
            
            case ShakeCamera shakeCamera:
                _events.ShakeCamera(shakeCamera.shakeDuration, shakeCamera.shakeType );
                break;
            
            case DestroyCells deleteCells:
                _events.DestroyNbOfCellType(deleteCells.numberOfCellsToDelete, deleteCells.cellType, deleteCells.cellState);
                break;
            
            case HitFeedback hitFeedback:
                _events.HitFeedback();
                break;
            
            case LooseHp looseHp:
                _events.LooseHp(looseHp.hpLoose);
                break;
            
            case SetCurrentHp setCurrentHp:
                _events.SetCurrentHp(setCurrentHp.currentHp);
                break;
            
            case UpdateRtcVolume updateRtcVolume:
                _events.RtcManager.UpdateVolume(updateRtcVolume.volumeType, updateRtcVolume.newVolumeProfile);
                break;
            
            case SignalEmitter:
                break;
            
            default:
                Debug.Log($"Unhandled signal type: {notification.GetType().Name}");
                break;
        }
    }
    
    public void RegisterReceiver()
    {
        if (_director != null)
        {
            PlayableGraph graph = _director.playableGraph;
            if (graph.IsValid())
            {
                for (int i = 0; i < graph.GetOutputCount(); i++)
                {
                    PlayableOutput output = graph.GetOutput(i);
                    output.AddNotificationReceiver(this);
                }
            }
        }
    }
}