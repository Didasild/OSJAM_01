using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [Header("Grid General Settings")]
    public Cell cellPrefab; // Le prefab de la cellule
    public Vector2Int gridSize;
    public float cellSize = 16f;   // Taille des cellules (espacement)
    public List<Cell> cellList = new List<Cell>(); //Liste des cellules de la grid
    public List<Cell> testList = new List<Cell>();

    [Header("Grid Procedural Settings")]
    //public int numberOfMine;
    public List<Cell> cellMineList = new List<Cell>(); //Liste de mines de la grid
    

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    public void GenerateGrid(Vector2Int gridSize, int pourcentageOfMine)
    {
        if (cellPrefab == null)
        {
            Debug.LogError("Prefab de cellule non assign� !");
            return;
        }

        // Efface les anciennes cellules si la grille est reg�n�r�e
        ClearGrid();

        // Calcul de l'offset pour centrer la grille
        float gridWidth = gridSize.x * cellSize; // Largeur totale de la grille
        float gridHeight = gridSize.y * cellSize;   // Hauteur totale de la grille

        Vector2 gridOffset = new Vector2(-gridWidth / 2 + cellSize / 2, gridHeight / 2 - cellSize / 2);

        // Parcourir les lignes et colonnes pour g�n�rer la grille
        for (int row = 0; row < gridSize.y; row++)
        {
            for (int col = 0; col < gridSize.x; col++)
            {
                // Calculer la position de chaque cellule (ajust�e par l'offset)
                Vector2 cellPosition = new Vector2(col * cellSize, -row * cellSize) + gridOffset;

                // Instancier la cellule
                Cell newCell = Instantiate(cellPrefab, cellPosition, Quaternion.identity);
                cellList.Add(newCell);

                // Optionnel : Attacher la cellule � un parent dans la hi�rarchie
                newCell.transform.SetParent(transform);

                // Renommer la cellule pour faciliter le d�bogage
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
    // M�thode pour effacer l'ancienne grille
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
    public void SetMineType(int pourcentageOfMine)
    {
        if (cellList.Count == 0)
        {
            Debug.LogWarning("La liste des enfants est vide !");
            return;
        }

        // S'assurer que le nombre d'objets � changer ne d�passe pas la taille de la liste
        int countToChange = Mathf.RoundToInt(cellList.Count * (pourcentageOfMine / 100f));

        // Liste temporaire pour suivre les objets d�j� modifi�s
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
        List<Cell> emptyCellsList = GetEmptyCoverCells();

        // Si aucune cellule dans la liste des "cover cells", utiliser la liste g�n�rale
        if (emptyCellsList.Count == 0)
        {
            emptyCellsList = GetEmptyCells();
        }

        // S'assurer de ne pas essayer de s�lectionner plus de cellules que disponibles
        numberOfItem = Mathf.Min(numberOfItem, emptyCellsList.Count);

        // Liste temporaire pour �viter les doublons
        List<Cell> selectedCells = new List<Cell>();

        for (int i = 0; i < numberOfItem; i++)
        {
            // G�n�re un index al�atoire parmi les cellules restantes
            int randomIndex = Random.Range(0, emptyCellsList.Count);

            // S�lectionne une cellule et la retire de la liste temporaire
            Cell selectedCell = emptyCellsList[randomIndex];
            emptyCellsList.RemoveAt(randomIndex);

            // Ajoute la cellule � la liste des cellules s�lectionn�es
            selectedCells.Add(selectedCell);
        }

        // Change le type de chaque cellule s�lectionn�e
        foreach (Cell cell in selectedCells)
        {
            cell.ChangeType(cellType);
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
            Cell neighbor = cellList.Find(cell => cell._cellPosition == neighborPosition);

            if (neighbor != null)
            {
                neighbors.Add(neighbor); // Ajoute le voisin � la liste
            }
        }

        return neighbors;
    }
}
