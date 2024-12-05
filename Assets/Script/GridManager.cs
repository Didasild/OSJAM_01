using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class GridManager : MonoBehaviour
{
    [Header("GRID GENERAL SETTINGS")]
    public Cell cellPrefab; // Le prefab de la cellule
    public float cellSize = 0.16f;   // Taille des cellules (espacement)
    public float timeBetweenApparition = 0.1f;


    [Header("GRID INFORMATIONS")]
    [NaughtyAttributes.ReadOnly]
    public List<Cell> cellList = new List<Cell>(); //Liste des cellules de la grid

    public string savedGridState;

    [Header("MINE LEFT")]
    [NaughtyAttributes.ReadOnly]
    public int numberOfMineLeft;
    [NaughtyAttributes.ReadOnly]
    public int theoricalMineLeft;
    //public int realMineLeft;
    public TMP_Text theoricalMineLeftText;

    [Header("GRID PROCEDURAL SETTINGS")]
    //public int numberOfMine;
    public List<Cell> cellMineList = new List<Cell>(); //Liste de mines de la grid
    [Button(enabledMode: EButtonEnableMode.Playmode)]


    #region PROCEDURAL GRID GENERATION
    public void GenerateGrid(Vector2Int gridSize, int pourcentageOfMine)
    {
        if (cellPrefab == null)
        {
            Debug.LogError("Prefab de cellule non assigné !");
            return;
        }

        //Sauvegarde l'état de la grille
        savedGridState = SaveGridString();

        // Efface les anciennes cellules si la grille est regénérée
        ClearGrid();

        // Parcourir les lignes et colonnes pour générer la grille
        for (int row = 0; row < gridSize.y; row++)
        {
            for (int col = 0; col < gridSize.x; col++)
            {
                // Calculer la position de chaque cellule (ajustée par l'offset)
                Vector2 cellPosition = new Vector2(col * cellSize, -row * cellSize) + GetGridOffset(cellSize, gridSize);

                Cell newCell = CellInstanciation(cellPosition, row, col);
                newCell.Initialize(new Vector2Int(row, col));
            }
        }
        //Génère la liste des voisins
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

        //Setup l'animation d'apparition
        ActiveListOfCells(timeBetweenApparition);
    }
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
                int randomIndex = UnityEngine.Random.Range(0, cellList.Count);
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

    public void SetItemsType(CellType cellType, int numberOfItem, ItemTypeEnum itemType = ItemTypeEnum.None)
    {
        // Crée la liste des cellule vide + hint
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
            int randomIndex = UnityEngine.Random.Range(0, emptyCellsList.Count);

            // Sélectionne une cellule et la retire de la liste temporaire
            Cell selectedCell = emptyCellsList[randomIndex];
            emptyCellsList.RemoveAt(randomIndex);

            // Ajoute la cellule à la liste des cellules sélectionnées
            selectedCells.Add(selectedCell);
        }

        // Change le type de chaque cellule sélectionnée
        foreach (Cell cell in selectedCells)
        {
            cell.ChangeItemType(itemType);
            cell.ChangeType(cellType, itemType);
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
    #endregion

    #region LOADED GRID GENERATION
    public void LoadGridFromString(string gridString, Vector2Int gridSize)
    {
        //Retourne une erreur si il n'y a pas de string
        if (string.IsNullOrEmpty(gridString))
        {
            Debug.LogError("Grid state est vide !");
            return;
        }
        // Efface l'ancienne grille
        ClearGrid();

        // Divise le string en segments pour chaque cellule
        string[] cellDataArray = gridString.Split('|');

        foreach (string cellData in cellDataArray)
        {
            // Découper chaque cellule par "_"
            string[] cellInfo = cellData.Split('_');
            if (cellInfo.Length != 5) continue; // Si les données ne sont pas complètes, ignorer

            // Extraire les coordonnées et les autres informations
            int y = int.Parse(cellInfo[0]);
            int x = int.Parse(cellInfo[1]);
            string stateAbbreviation = cellInfo[2];
            string typeAbbreviation = cellInfo[3];
            string itemTypeAbbreviation = cellInfo[4];

            // Créer une nouvelle cellule à ces coordonnées
            // Calculer la position de chaque cellule (ajustée par l'offset)
            Vector2 cellPosition = new Vector2(x * cellSize, -y * cellSize) + GetGridOffset(cellSize, gridSize);

            // Instancier une nouvelle cellule
            Cell newCell = CellInstanciation(cellPosition, y, x);

            // Convertir les abréviations en valeurs d'enum
            CellState state = GetStateFromAbbreviation(stateAbbreviation);
            CellType type = GetTypeFromAbbreviation(typeAbbreviation);
            ItemTypeEnum itemType = GetItemTypeFromAbbreviation(itemTypeAbbreviation);

            // Initialiser la cellule avec ses nouveaux états
            newCell.currentState = state;
            newCell.currentType = type;
            newCell.currentItemType = itemType;

            newCell.Initialize(new Vector2Int(x, y)); // Initialisation avec les bonnes coordonnées et le bon état
        }
        foreach (Cell cell in cellList)
        {
            cell.GenerateNeighborsList(this);
        }
        SetCellsVisuals();
        ActiveListOfCells(timeBetweenApparition);
    }

    public CellState GetStateFromAbbreviation(string abbreviation)
    {
        return abbreviation switch
        {
            "Co" => CellState.Cover,
            "Cl" => CellState.Clicked,
            "Fl" => CellState.Flag,
            "Pl" => CellState.PlantedSword,
            "Re" => CellState.Reveal,
            "No" => CellState.None,
            _ => throw new ArgumentException($"Abréviation inconnue : {abbreviation}")
        };
    }

    public CellType GetTypeFromAbbreviation(string abbreviation)
    {
        return abbreviation switch
        {
            "Em" => CellType.Empty,
            "Mi" => CellType.Mine,
            "Hi" => CellType.Hint,
            "Ga" => CellType.Gate,
            "It" => CellType.Item,
            _ => throw new ArgumentException($"Abréviation inconnue : {abbreviation}")
        };
    }

    public ItemTypeEnum GetItemTypeFromAbbreviation(string abbreviation)
    {
        return abbreviation switch
        {
            "Po" => ItemTypeEnum.Potion,
            "Sw" => ItemTypeEnum.Sword,
            "Co" => ItemTypeEnum.Coin,
            "No" => ItemTypeEnum.None,
            _ => throw new ArgumentException($"Abréviation inconnue : {abbreviation}")
        };
    }

    #endregion

    #region GRID GENERATION FONCTIONS
    public string SaveGridString()
    {
        System.Text.StringBuilder gridStringBuilder = new System.Text.StringBuilder();

        foreach (Cell cell in cellList)
        {
            // Coordonnées de la cellule
            int x = cell._cellPosition.x;
            int y = cell._cellPosition.y;

            // État de la cellule (par exemple "em" pour Empty, "co" pour Cover)
            string state = cell.currentState.ToString().Substring(0, 2);
            string type = cell.currentType.ToString().Substring(0, 2);
            string itemType = cell.currentItemType.ToString().Substring(0, 2);

            // Ajouter à la chaîne sous forme : x_y_state|
            gridStringBuilder.Append($"{x}_{y}_{state}_{type}_{itemType}|");
        }
        // Retirer le dernier caractère "|" pour une chaîne propre
        if (gridStringBuilder.Length > 0)
        {
            gridStringBuilder.Length--;
        }
        return gridStringBuilder.ToString();
    }

    public Vector2 GetGridOffset(float cellSize, Vector2Int gridSize)
    {
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
        return gridOffset;
    }

    public Cell CellInstanciation(Vector2 cellPosition, int row, int col)
    {
        Cell newCell = Instantiate(cellPrefab, cellPosition, Quaternion.identity);
        newCell.transform.SetParent(transform);
        cellList.Add(newCell);
        newCell.name = $"Cell_{row}_{col}"; // Renommer la cellule pour faciliter le débogage
        return newCell;
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

    #region GRID VISUAL FONCTIONS
    public void ActiveListOfCells(float timeBetweenApparition)
    {
        StartCoroutine(CO_ActiveWithDelay(timeBetweenApparition));
    }

    private IEnumerator CO_ActiveWithDelay(float timeBetweenApparition)
    {
        foreach (Cell cell in cellList)
        {
            cell.gameObject.SetActive(true);
            yield return new WaitForSeconds(timeBetweenApparition); // Attends le délai avant de continuer
        }
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
