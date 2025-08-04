using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class LooseHp : Marker, INotification
{
    public PropertyName id => new PropertyName();
    public int hpLoose = 1;
}
