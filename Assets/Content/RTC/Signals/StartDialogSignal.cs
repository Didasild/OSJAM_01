using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class StartDialogSignal : Marker, INotification
{
    public PropertyName id => new PropertyName();
    public NpcDialogsSettings dialogSettings;
}