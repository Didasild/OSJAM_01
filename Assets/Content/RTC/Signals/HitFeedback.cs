using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class HitFeedback : Marker, INotification
{
    public PropertyName id => new PropertyName();
}