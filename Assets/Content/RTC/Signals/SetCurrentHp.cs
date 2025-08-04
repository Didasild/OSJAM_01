using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class SetCurrentHp : Marker, INotification
{
    public PropertyName id => new PropertyName();
    public int currentHp = 1;
}