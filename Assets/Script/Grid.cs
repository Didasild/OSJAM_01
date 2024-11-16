using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [Header("Grid General Settings")]
    public Cell cellPrefab; // Le prefab de la cellule
    public int rows = 5;          // Nombre de lignes
    public int columns = 5;       // Nombre de colonnes
    public float cellSize = 16f;   // Taille des cellules (espacement)
    public List<Cell> cellList = new List<Cell>(); //Liste des cellules de la grid
    public List<Cell> testList = new List<Cell>();

    [Header("Grid Procedural Settings")]
    public int numberOfMine;
    public List<Cell> cellMineList = new List<Cell>(); //Liste de mines de la grid
    

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    public void GenerateGrid()
    {
        if (cellPrefab == null)
        {
            Debug.LogError("Prefab de cellule non assigné !");
            return;
        }

        // Efface les anciennes cellules si la grille est regénérée
        ClearGrid();

        // Calcul de l'offset pour centrer la grille
        float gridWidth = columns * cellSize; // Largeur totale de la grille
        float gridHeight = rows * cellSize;   // Hauteur totale de la grille

        Vector2 gridOffset = new Vector2(-gridWidth / 2 + cellSize / 2, gridHeight / 2 - cellSize / 2);

        // Parcourir les lignes et colonnes pour générer la grille
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                // Calculer la position de chaque cellule (ajustée par l'offset)
                Vector2 cellPosition = new Vector2(col * cellSize, -row * cellSize) + gridOffset;

                // Instancier la cellule
                Cell newCell = Instantiate(cellPrefab, cellPosition, Quaternion.identity);
                cellList.Add(newCell);

                // Optionnel : Attacher la cellule à un parent dans la hiérarchie
                newCell.transform.SetParent(transform);

                // Renommer la cellule pour faciliter le débogage
                newCell.name = $"Cell_{row}_{col}";

                newCell.Initialize(this, new Vector2Int(row, col));
            }
        }
        foreach (Cell cell in cellList)
        {
            cell.InitializeType(this);
        }

        SetMineType(numberOfMine);
    }
    // Méthode pour effacer l'ancienne grille
    public void ClearGrid()
    {
        foreach (Cell cell in cellList)
        {
            if (cell != null)
            {
                Destroy(cell.gameObject);
            }
        }
        cellList = new List<Cell>();
        cellMineList = new List<Cell> ();
    }
    //
    public void SetMineType(int numberOfMine)
    {
        if (cellList.Count == 0)
        {
            Debug.LogWarning("La liste des enfants est vide !");
            return;
        }

        if (cellList.Count < numberOfMine)
        {
            Debug.LogWarning("Pas assez de cellule vide");
            return;
        }

        //Transforme tout les enfants en Empty
        foreach (Cell cell in cellList)
        {
            Cell cellToDefine = cell.GetComponent<Cell>();
            cellToDefine.ChangeType(CellType.Empty);
        }

        // S'assurer que le nombre d'objets à changer ne dépasse pas la taille de la liste
        int countToChange = Mathf.Min(numberOfMine, cellList.Count);

        // Liste temporaire pour suivre les objets déjà modifiés
        List<Cell> alreadyChanged = new List<Cell>();

        for (int i = 0;i < countToChange;i++)
        {
            Cell randomCell = cellList[i];
            do
            {
                int randomIndex = Random.Range(0, cellList.Count);
                randomCell = cellList[randomIndex];
            } while (alreadyChanged.Contains(randomCell));

                alreadyChanged.Add(randomCell);

            Cell cell = randomCell.GetComponent<Cell>();
            if (cell != null)
            {
                cell.ChangeType(CellType.Mine);
                cellMineList.Add(randomCell);
            }
        }
    }

    public void SetStairType(int clickCount = 0) 
    { 
        List<Cell> emptyCoverCellsList = GetEmptyCoverCells();
        if (emptyCoverCellsList.Count == 0)
        {
            List<Cell> emptyCellsList = GetEmptyCells();
            int randomIndex = Random.Range(0, emptyCellsList.Count);
            Cell selectedCell = emptyCellsList[randomIndex];
            selectedCell.ChangeType(CellType.Stair);
        }
        else 
        {
            int randomIndex = Random.Range(0, emptyCoverCellsList.Count);
            Cell selectedCell = emptyCoverCellsList[randomIndex];
            selectedCell.ChangeType(CellType.Stair);
        }
    }

    public List<Cell> GetEmptyCoverCells()
    {
        List<Cell> emptyCells = new List<Cell>();
        foreach (Cell cell in cellList)
        {
            if (cell.currentType == CellType.Empty && cell.currentState == CellState.Cover)
            {
                emptyCells.Add(cell);
            }
        }
        return emptyCells;
    }
    public List<Cell> GetEmptyCells()
    {
        List<Cell> emptyCells = new List<Cell>();
        foreach (Cell cell in cellList)
        {
            if (cell.currentType == CellType.Empty)
            {
                emptyCells.Add(cell);
            }
        }
        return emptyCells;
    }

    public void SetCellsVisuals(Cell cellIgnore = null)
    {
        List<Cell> cellsWithExcluded = new List<Cell>(cellList);
        cellsWithExcluded.Remove(cellIgnore);
        foreach (Cell cell in cellsWithExcluded)
        {
            cell.InitalizeVisual();
        }
    }

    public List<Cell> GetNeighbors(Vector2Int cellPosition)
    {
        List<Cell> neighbors = new List<Cell>();

        // Définir les offsets pour les 8 directions autour d'une cellule
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
            Cell neighbor = cellList.Find(cell => cell._cellPosition == neighborPosition);

            if (neighbor != null)
            {
                neighbors.Add(neighbor); // Ajoute le voisin à la liste
            }
        }

        return neighbors;
    }
}
