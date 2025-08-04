using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class DestroyCells : Marker, INotification
{
    public PropertyName id => new PropertyName();
    public int numberOfCellsToDelete = 5;
    public CellState cellState = CellState.Reveal;
    public CellType cellType = CellType.Empty;
}