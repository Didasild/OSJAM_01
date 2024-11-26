using NaughtyAttributes;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [Header("GRID GENERAL SETTINGS")]
    public Cell cellPrefab; // Le prefab de la cellule
    public float cellSize = 16f;   // Taille des cellules (espacement)


    [Header("GRID INFORMATIONS")]
    [NaughtyAttributes.ReadOnly]
    public List<Cell> cellList = new List<Cell>(); //Liste des cellules de la grid

    [Header("MINE LEFT")]
    [NaughtyAttributes.ReadOnly]
    public int numberOfMineLeft;
    [NaughtyAttributes.ReadOnly]
    public int theoricalMineLeft;
    [NaughtyAttributes.ReadOnly]
    //public int realMineLeft;
    public TMP_Text theoricalMineLeftText;

    [Header("GRID PROCEDURAL SETTINGS")]
    //public int numberOfMine;
    public List<Cell> cellMineList = new List<Cell>(); //Liste de mines de la grid
    [Button(enabledMode: EButtonEnableMode.Playmode)]

    #region GRID GENERATION
    public void GenerateGrid(Vector2Int gridSize, int pourcentageOfMine)
    {
        if (cellPrefab == null)
        {
            Debug.LogError("Prefab de cellule non assigné !");
            return;
        }

        // Efface les anciennes cellules si la grille est regénérée
        ClearGrid();

        // Calcul de l'offset pour centrer la grille
        float gridWidth = gridSize.x * cellSize; // Largeur totale de la grille
        float gridHeight = gridSize.y * cellSize;   // Hauteur totale de la grille

        // Ajustement pour la parité des dimensions
        float xAdjustment = (gridSize.x % 2 == 0) ? 0 : cellSize / 2; // Décalage si impair
        float yAdjustment = (gridSize.y % 2 == 0) ? 0 : -cellSize / 2; // Décalage si impair

        Vector2 gridOffset = new Vector2(
            -gridWidth / 2 + cellSize / 2 + xAdjustment, // Ajustement horizontal
            gridHeight / 2 - cellSize / 2 + yAdjustment  // Ajustement vertical
        );
        // Parcourir les lignes et colonnes pour générer la grille
        for (int row = 0; row < gridSize.y; row++)
        {
            for (int col = 0; col < gridSize.x; col++)
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
            cell.GenerateNeighborsList(this);
        }
        //Transforme tout les enfants en Empty
        foreach (Cell cell in cellList)
        {
            Cell cellToDefine = cell.GetComponent<Cell>();
            cellToDefine.ChangeType(CellType.Empty);
        }

        SetMineType(pourcentageOfMine);
    }
    // Méthode pour effacer l'ancienne grille
    public void SetMineType(int pourcentageOfMine)
    {
        if (cellList.Count == 0)
        {
            Debug.LogWarning("La liste des enfants est vide !");
            return;
        }

        // S'assurer que le nombre d'objets à changer ne dépasse pas la taille de la liste
        int countToChange = Mathf.RoundToInt(cellList.Count * (pourcentageOfMine / 100f));

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

    public void SetItemsType(CellType cellType, int numberOfItem)
    {
        List<Cell> emptyCellsList = GetCoverCellsByType(CellType.Empty);
        if (cellType != CellType.Gate)
        {
            emptyCellsList.AddRange(GetCoverCellsByType(CellType.Hint));
        }

        // Si aucune cellule dans la liste des "cover cells", utiliser la liste générale
        if (emptyCellsList.Count == 0)
        {
            emptyCellsList = GetCellsByType(CellType.Empty);
            if (cellType != CellType.Gate)
            {
                emptyCellsList.AddRange(GetCellsByType(CellType.Hint));
            }
        }

        // S'assurer de ne pas essayer de sélectionner plus de cellules que disponibles
        numberOfItem = Mathf.Min(numberOfItem, emptyCellsList.Count);

        // Liste temporaire pour éviter les doublons
        List<Cell> selectedCells = new List<Cell>();

        for (int i = 0; i < numberOfItem; i++)
        {
            // Génère un index aléatoire parmi les cellules restantes
            int randomIndex = Random.Range(0, emptyCellsList.Count);

            // Sélectionne une cellule et la retire de la liste temporaire
            Cell selectedCell = emptyCellsList[randomIndex];
            emptyCellsList.RemoveAt(randomIndex);

            // Ajoute la cellule à la liste des cellules sélectionnées
            selectedCells.Add(selectedCell);
        }

        // Change le type de chaque cellule sélectionnée
        foreach (Cell cell in selectedCells)
        {
            cell.ChangeType(cellType);
            cell.UpdateRegardingNeighbors();
        }
    }

    public void SetCellsVisuals(Cell cellIgnore = null)
    {
        List<Cell> cellsWithExcluded = new List<Cell>(cellList);
        cellsWithExcluded.Remove(cellIgnore);
        foreach (Cell cell in cellsWithExcluded)
        {
            cell.UpdateRegardingNeighbors();
        }
    }

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
        cellMineList = new List<Cell>();
    }
    #endregion

    #region GET GRID INFORMATIONS
    public List<Cell> GetCellsByType(CellType typeOfCellWanted)
    {
        List<Cell> emptyCells = new List<Cell>();
        foreach (Cell cell in cellList)
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
        foreach (Cell cell in cellList)
        {
            if (cell.currentState == stateOfCellWanted)
            {
                emptyCells.Add(cell);
            }
        }
        return emptyCells;
    }
    public List<Cell> GetCoverCellsByType(CellType typeOfCellWanted)
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
    #endregion

    #region MINE COUNTER
    public void UpdateMineCounter()
    {
        theoricalMineLeft = GetTheoricalMineLeft();
        theoricalMineLeftText.text = theoricalMineLeft.ToString();
    }
    #endregion
}
