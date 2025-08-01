using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class GridInfos
{
    private GridManager _gridManager;
    [ReadOnly] public int numberOfMineLeft;
    [ReadOnly] public int theoricalMineLeft;
    
    public void Init(GridManager gridManager)
    {
        _gridManager = gridManager;
    }
    public List<Cell> GetCellsByType(CellType typeOfCellWanted)
    {
        List<Cell> emptyCells = new List<Cell>();
        foreach (Cell cell in _gridManager.cellList)
        {
            if (cell.currentType == typeOfCellWanted)
            {
                emptyCells.Add(cell);
            }
        }
        return emptyCells;
    }
    public List<Cell> GetCellsByState(CellState stateOfCellWanted)
    {
        List<Cell> emptyCells = new List<Cell>();
        foreach (Cell cell in _gridManager.cellList)
        {
            if (cell.currentState == stateOfCellWanted)
            {
                emptyCells.Add(cell);
            }
        }
        return emptyCells;
    }
    public List<Cell> GetCoverCellsByType(CellType typeOfCellWanted, List<Cell> cellList)
    {
        List<Cell> emptyCoverCells = new List<Cell>();
        foreach (Cell cell in cellList)
        {
            if (cell.currentType == typeOfCellWanted && cell.currentState == CellState.Cover)
            {
                emptyCoverCells.Add(cell);
            }
        }
        return emptyCoverCells;
    }

    public int GetTheoricalMineLeft()
    {
        int nbRealOfMine = GetCellsByType(CellType.Mine).Count;
        int nbOfFlagged = GetCellsByState(CellState.Flag).Count;
        numberOfMineLeft = nbRealOfMine - nbOfFlagged;
        return numberOfMineLeft;
    }
    public List<Cell> GetNeighbors(Vector2Int cellPosition)
    {
        List<Cell> neighbors = new List<Cell>();

        // D�finir les offsets pour les 8 directions autour d'une cellule
        int[,] directions = new int[,]
        {
            { -1, -1 }, { -1, 0 }, { -1, 1 }, // Haut-gauche, Haut, Haut-droite
            {  0, -1 },            {  0, 1 }, // Gauche, Droite
            {  1, -1 }, {  1, 0 }, {  1, 1 }  // Bas-gauche, Bas, Bas-droite
        };

        for (int i = 0; i < directions.GetLength(0); i++)
        {
            int newRow = cellPosition.x + directions[i, 0];
            int newCol = cellPosition.y + directions[i, 1];
            Vector2Int neighborPosition = new Vector2Int(newRow, newCol);

            //Recherche dans la liste
            Cell neighbor = _gridManager.cellList.Find(cell => cell._cellPosition == neighborPosition);

            if (neighbor != null)
            {
                neighbors.Add(neighbor); // Ajoute le voisin � la liste
            }
        }
        return neighbors;
    }
}
