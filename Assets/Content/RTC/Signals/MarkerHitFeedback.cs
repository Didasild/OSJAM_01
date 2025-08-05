using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class MarkerHitFeedback : Marker, INotification
{
    public PropertyName id => new PropertyName();
}