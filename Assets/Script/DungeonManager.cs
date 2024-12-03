using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    public RoomData roomPrefab;
    public Vector2 floorSize = new Vector2 (3, 4);

    [SerializeField] private List<RoomData> roomDataList = new List<RoomData> ();

    public void Start()
    {
        GenerateFloor();
    }

    public void GenerateFloor()
    {
        for (int y = 0; y < floorSize.y; y++)
        {
            for (int x = 0; x < floorSize.x; x++)
            {
                // Calculer la position sur la grille
                Vector2Int gridPosition = new Vector2Int(x, y);

                // Instancier le GameObject room (les data uniquement)
                RoomData roomData = Instantiate(roomPrefab, transform);
                if (roomData != null)
                {
                    roomData.Initialize(gridPosition); // Initialisation avec la position
                    roomDataList.Add(roomData); // Ajouter à la liste
                    roomData.transform.SetParent(transform);
                }

                // Nommer la room pour faciliter le debug
                roomData.name = $"Room_{x}_{y}";
            }
        }
        GiveNeighbors();
    }

    public void GiveNeighbors()
    {
        foreach (RoomData room in roomDataList)
        {
            Vector2Int position = room.roomPosition;

            // Cherche les voisins
            room.roomUp = FindRoomAtPosition(position + Vector2Int.up);
            room.roomDown = FindRoomAtPosition(position + Vector2Int.down);
            room.roomLeft = FindRoomAtPosition(position + Vector2Int.left);
            room.roomRight = FindRoomAtPosition(position + Vector2Int.right);
        }
    }
    private GameObject FindRoomAtPosition(Vector2Int position)
    {
        RoomData room = roomDataList.Find(r => r.roomPosition == position);
        return room != null ? room.gameObject : null;
    }
}
