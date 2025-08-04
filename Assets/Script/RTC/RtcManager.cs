using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Timeline;

public enum VolumeType
{
    ambianceVolume,
    effectVolume01,
    effectVolume02,
}
public class RtcManager : MonoBehaviour
{
    private EventsController _eventsController;
    [SerializeField] private Volume _rtcAmbianceVolume;
    [SerializeField] private Volume _rtcEffectVolume1;
    [SerializeField] private Volume _rtcEffectVolume2;
    private PlayableDirector _playableDirector;
    private RtcCustomSignalReceiver _rtcReceiver;
    
    public PlayableDirector PlayableDirector => _playableDirector;
    
    public void Init(EventsController eventsController)
    {
        _eventsController = eventsController;
        _playableDirector = GetComponent<PlayableDirector>();
        _rtcReceiver = GetComponent<RtcCustomSignalReceiver>();
        _rtcReceiver.Init(_eventsController);
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

    public void UpdateVolume(VolumeType volumeType, VolumeProfile newProfile)
    {
        switch (volumeType)
        {
            case VolumeType.ambianceVolume:
                _rtcAmbianceVolume.profile = newProfile;
                break;
            case VolumeType.effectVolume01:
                _rtcEffectVolume1.profile = newProfile;
                break;
            case VolumeType.effectVolume02:
                _rtcEffectVolume2.profile = newProfile;
                break; 
            
            default:
                Debug.LogWarning($"Unhandled shake type: {volumeType}");
                break;
        }
    }
}
