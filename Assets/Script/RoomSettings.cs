using UnityEngine;

[CreateAssetMenu(fileName = "RoomSettings", menuName = "DungeonGeneration/RoomSettings")]
public class RoomSettings : ScriptableObject
{
    [Header("GENERAL")]
    public bool proceduralGrid = true;
    public bool isMandatory = false;

    [Header("________LOADED GRID SETTINGS")]
    public string gridSavedString;

    [Header("________PROCEDURAL GRID SETTINGS")]
    [Header("ROOM GRID")]
    [SerializeField] private Vector2Int minGridSize = new Vector2Int(5,5);
    [SerializeField] private Vector2Int maxGridSize = new Vector2Int(10, 10);
    public int roomPourcentageOfMine = 5;
    [Header("STAIR")]
    [SerializeField] private bool haveStair;
    [Header("HEALTH")]
    [SerializeField] private int minPotion;
    [SerializeField] private int maxPotion;
    [Header("SWORD")]
    [SerializeField] private int minSword;
    [SerializeField] private int maxSword;

    public Vector2Int GetGridSize()
    {
        int randomRow = Random.Range(minGridSize.x, maxGridSize.x+1);
        int randomCol = Random.Range(minGridSize.y, maxGridSize.y+1);
        Vector2Int gridSize = new Vector2Int(randomRow, randomCol);
        return gridSize;
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

    public Vector2Int GetGridSizeFromString()
    {
        int maxRow = 0;
        int maxCol = 0;

        // Diviser la chaîne en segments individuels
        string[] cellDataArray = gridSavedString.Split('|');

        foreach (string cellData in cellDataArray)
        {
            // Extraire les coordonnées de chaque cellule
            string[] parts = cellData.Split('_');
            if (parts.Length >= 2)
            {
                int row = int.Parse(parts[0]);
                int col = int.Parse(parts[1]);

                // Trouver les valeurs maximales pour les coordonnées
                maxRow = Mathf.Max(maxRow, row);
                maxCol = Mathf.Max(maxCol, col);
            }
        }

        // Ajouter 1 pour transformer les index en tailles (par exemple, index 0-4 = taille 5)
        return new Vector2Int(maxCol + 1, maxRow + 1);
    }
}
