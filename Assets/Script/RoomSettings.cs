using UnityEngine;

[CreateAssetMenu(fileName = "RoomSettings", menuName = "DungeonGeneration/RoomSettings")]
public class RoomSettings : ScriptableObject
{
    #region PARAMETERS
    [Header("GENERAL")]
    public bool proceduralRoom = true;
    public bool isMandatory;

    [Header("________LOADED GRID SETTINGS")]
    public string roomLoadString; // USE ONLY FOR FIRST GENERATION NEVER CHANGE

    [Header("________PROCEDURAL GRID SETTINGS")]
    [Header("ROOM GRID")]
    [SerializeField] private Vector2Int minRoomSize = new Vector2Int(5,5);
    [SerializeField] private Vector2Int maxRoomSize = new Vector2Int(10, 10);
    public int roomPourcentageOfMine = 5;
    [Header("STAIR")]
    public bool haveStair;
    [Header("HEALTH")]
    [SerializeField] private int minPotion;
    [SerializeField] private int maxPotion;
    [Header("SWORD")]
    [SerializeField] private int minSword;
    [SerializeField] private int maxSword;
    #endregion

    #region PROCEDURAL GET INFOS
    public Vector2Int GetRoomSize()
    {
        int randomRow = Random.Range(minRoomSize.x, maxRoomSize.x+1);
        int randomCol = Random.Range(minRoomSize.y, maxRoomSize.y+1);
        Vector2Int roomSize = new Vector2Int(randomRow, randomCol);
        return roomSize;
    }

    public int GetNumberOfPotion()
    {
        int numberOfPotion = Random.Range(minPotion, maxPotion+1);
        return numberOfPotion;
    }

    public int GetNumberOfSword()
    {
        int numberOfSword = Random.Range(minSword, maxSword + 1);
        return numberOfSword;
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
        Debug.Log($"Grid Size Save:{maxRow + 1} rows {maxCol + 1} columns");

        // Retourner les dimensions (colonnes = maxCol + 1, lignes = maxRow + 1)
        return new Vector2Int(maxCol + 1, maxRow + 1);
    }
    #endregion
}
