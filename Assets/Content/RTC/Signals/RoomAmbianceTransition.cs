using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Timeline;

[System.Serializable]
public class RoomAmbianceTransition : Marker, INotification
{
    public PropertyName id => new PropertyName();
    public VolumeProfile newVolumeProfile;
}