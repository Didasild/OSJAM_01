using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class MarkerCloseDialogBox : Marker, INotification
{
    public PropertyName id => new PropertyName();
}