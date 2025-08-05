using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Timeline;

[System.Serializable]
public class MarkerUpdateRtcVolume : Marker, INotification
{
    public PropertyName id => new PropertyName();
    public VolumeType volumeType;
    public VolumeProfile newVolumeProfile;
}