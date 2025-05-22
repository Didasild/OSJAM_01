using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public enum RoomType
{
    Base,
    Stair,
    Shop,
    Sword,
    Potion,
    Boss,
    NPC
}

public enum GenerationType
{
    RP,
    RSP,
    RL,
}

[CreateAssetMenu(fileName = "RoomSettings", menuName = "MineCrawler/RoomSettings")]

public class RoomSettings : ScriptableObject
{
    #region PARAMETERS
    [Header("GENERAL")]
    public bool proceduralRoom = true;
    public bool mandatory;
    public bool isFoW;
    
    [Header("TAG")]
    public RoomType roomType;
    
    [Header("VISUAL")]
    public VolumeProfile roomVolumeProfile;

    [Header("________LOADED GRID SETTINGS")]
    [FormerlySerializedAs("roomLoadString")] public string roomIDString; // USE ONLY FOR FIRST GENERATION NEVER CHANGE
    public bool haveProceduralCells;

    [Header("________PROCEDURAL GRID SETTINGS")]
    [Header("ROOM GRID")]
    [SerializeField] public Vector2Int minRoomSize = new Vector2Int(5,5);
    [SerializeField] public Vector2Int maxRoomSize = new Vector2Int(10, 10);
    
    [Header("SPECIFIC CELLS")]
    public int roomPourcentageOfMine = 5;
    public int roomPourcentageOfNone = 5;
    public bool haveStair;
    
    [System.Serializable]
    public struct ItemRange
    {
        public ItemTypeEnum itemType;
        public int min;
        public int max;
    }
    public List<ItemRange> itemRanges;
    #endregion

    #region PROCEDURAL GET INFOS
    public Vector2Int GetRoomSize()
    {
        int randomRow = Random.Range(minRoomSize.x, maxRoomSize.x+1);
        int randomCol = Random.Range(minRoomSize.y, maxRoomSize.y+1);
        Vector2Int roomSize = new Vector2Int(randomRow, randomCol);
        return roomSize;
    }
    public int GetNumberOfItem(ItemTypeEnum itemType)
    {
        var range = GetRange(itemType);
        if (range.HasValue)
        {
            return Random.Range(range.Value.min, range.Value.max + 1);
        }
        else
        {
            return 0; // Si le type d'item n'est pas configuré
        }
    }
    private ItemRange? GetRange(ItemTypeEnum itemType)
    {
        foreach (var range in itemRanges)
        {
            if (range.itemType == itemType)
            {
                return range;
            }
        }
        Debug.Log($"Item type {itemType} non configuré dans {name} !");
        return null;
    }
    #endregion
    
    #region LOADED GET INFOS
    public Vector2Int GetRoomSizeFromString(string roomSavedString)
    {
        int maxRow = 0;
        int maxCol = 0;
        // Diviser la chaîne en segments individuels
        string[] cellDataArray = roomSavedString.Split('|');

        foreach (string cellData in cellDataArray)
        {
            // Extraire les coordonnées de chaque cellule
            string[] parts = cellData.Split('_');
            if (parts.Length >= 3) // Vérifier qu'on a bien les coordonnées et les états
            {
                if (int.TryParse(parts[0], out int row) && int.TryParse(parts[1], out int col))
                {
                    // Trouver les valeurs maximales pour les coordonnées
                    maxRow = Mathf.Max(maxRow, row);
                    maxCol = Mathf.Max(maxCol, col);
                }
            }
        }
        // Retourner les dimensions (colonnes = maxCol + 1, lignes = maxRow + 1)
        return new Vector2Int(maxCol + 1, maxRow + 1);
    }
    #endregion
}
