using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Timeline;

[System.Serializable]
public class StartDialogSignal : Marker, INotification
{
    public PropertyName id => new PropertyName();
    public NpcDialogsSettings dialogSettings;
}

[System.Serializable]
public class RoomAmbianceTransition : Marker, INotification
{
    public PropertyName id => new PropertyName();
    public VolumeProfile newVolumeProfile;
}

[System.Serializable]
public class ShakeCamera : Marker, INotification
{
    public PropertyName id => new PropertyName();
    public float shakeDuration = 0.5f;
    public ShakeType shakeType;
}

[System.Serializable]
public class DestroyCells : Marker, INotification
{
    public PropertyName id => new PropertyName();
    public int numberOfCellsToDelete = 5;
    public CellState cellState = CellState.Reveal;
    public CellType cellType = CellType.Empty;
}

[System.Serializable]
public class HitFeedback : Marker, INotification
{
    public PropertyName id => new PropertyName();
}

[System.Serializable]
public class LooseHp : Marker, INotification
{
    public PropertyName id => new PropertyName();
    public int hpLoose = 1;
}