using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    private FloorManager _floorManager;
    
    [Header("GENERAL SETTINGS")]
    public RoomData roomPrefab;
    public Transform roomContainer;
    public float roomSize;
    
    public void Init()
    {
        _floorManager = GameManager.Instance.floorManager;
    }

    public void GenerateProceduralMinimap(RoomSettings[] _roomSettingsList, FloorSettings floorSetting)
    {
        Vector2Int floorSize = floorSetting.GetProceduralFloorSize();
        
        for (int y = 0; y < floorSize.y; y++)
        {
            for (int x = 0; x < floorSize.x; x++)
            {
                // Calculer la position sur la grille
                Vector2Int gridPosition = new Vector2Int(x, y);

                // Instancier le GameObject room
                RoomData roomData = Instantiate(roomPrefab, roomContainer);
                if (roomData != null)
                {
                    roomData.Initialize(gridPosition, RoomCompletionCondition.None, this);
                    SetRoomPosition(roomData, gridPosition);
                    _floorManager.roomList.Add(roomData);
                    roomData.transform.SetParent(roomContainer);
                }
                
                // Nommer la room pour faciliter le debug
                roomData.name = $"Room_{x}_{y}";
            }
        }
    }

    public void SetRoomPosition(RoomData roomData, Vector2Int position)
    {
        roomData.transform.SetParent(roomContainer);
        roomData.roomPosition = position;
        
        // Calculez la position dans le monde
        Vector3 worldPosition = new Vector3(position.x * roomSize, position.y * roomSize, 0);

        // Placez le GameObject à cette position
        roomData.transform.localPosition = worldPosition;
    }
}
