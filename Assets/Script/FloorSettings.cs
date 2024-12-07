using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(fileName = "FloorSettings", menuName = "DungeonGeneration/FloorSettings")]
public class FloorSettings : ScriptableObject
{
    #region PARAMETERS
    [Header("GENERAL")]
    public bool proceduralFloor = true;

    [Header("________PROCEDURAL FLOOR SETTINGS")]
    [Header("FLOOR GRID")]
    [SerializeField] private Vector2Int minGridSize = new Vector2Int(2, 2);
    [SerializeField] private Vector2Int maxGridSize = new Vector2Int(4, 4);

    [Header("ROOM SETTINGS")]
    [SerializeField] public RoomSettings[] roomSettingsList;
    #endregion

    public Vector2Int GetFloorSize()
    {
        int randomRow = Random.Range(minGridSize.x, maxGridSize.x + 1);
        int randomCol = Random.Range(minGridSize.y, maxGridSize.y + 1);
        Vector2Int gridSize = new Vector2Int(randomRow, randomCol);
        return gridSize;
    }
}
