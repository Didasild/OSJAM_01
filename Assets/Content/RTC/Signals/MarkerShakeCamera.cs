using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class MarkerShakeCamera : Marker, INotification
{
    public PropertyName id => new PropertyName();
    public float shakeDuration = 0.5f;
    public ShakeType shakeType;
}