using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class MarkerStartDialogSignal : Marker, INotification
{
    public PropertyName id => new PropertyName();
    public NpcDialogsSettings dialogSettings;
}