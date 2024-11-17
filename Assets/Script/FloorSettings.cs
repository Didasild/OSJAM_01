using UnityEngine;

[CreateAssetMenu(fileName = "FloorSettings", menuName = "Balancing /FloorSettings")]
public class FloorSettings : ScriptableObject
{
    [SerializeField] private Vector2Int minGridSize = new Vector2Int(5,5);
    [SerializeField] private Vector2Int maxGridSize = new Vector2Int(10, 10);
    public int floorPourcentageOfMine = 5;
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
}