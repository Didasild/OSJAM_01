using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    private DungeonManager _floorManager;
    
    [Header("GENERAL SETTINGS")]
    public RoomData roomPrefab;
    public Transform roomContainer;
    public float roomSize;
    
    
    public void Init()
    {
        _floorManager = GameManager.Instance.floorManager;
    }

    public void GenerateMinimap(RoomSettings[] _roomSettingsList, FloorSettings floorSetting)
    {
        Vector2Int floorSize = floorSetting.GetProceduralFloorSize();
        // Calcul du décalage pour centrer la grille
        Vector3 offset = new Vector3(
            -(floorSize.x * roomSize) / 2.0f + (roomSize / 2.0f), // Décalage X
            -(floorSize.y * roomSize) / 2.0f + (roomSize / 2.0f), // Décalage Y
            0 // Z reste constant
        );
        
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
                    roomData.Initialize(gridPosition, RoomCompletionCondition.None, roomSize, offset); // Initialisation avec la position
                    _floorManager.roomList.Add(roomData);
                    roomData.transform.SetParent(roomContainer);
                }
                
                // Nommer la room pour faciliter le debug
                roomData.name = $"Room_{x}_{y}";
            }
        }
    }
    
    
}
