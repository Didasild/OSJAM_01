using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class CloseDialogBox : Marker, INotification
{
    public PropertyName id => new PropertyName();
}