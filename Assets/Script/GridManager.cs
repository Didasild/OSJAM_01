using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    #region PARAMETERS
    [Header("GRID GENERAL SETTINGS")]
    public Cell cellPrefab; // Le prefab de la cellule
    public float cellSize = 0.16f;   // Taille des cellules (espacement)
    public float timeBetweenApparition = 0.1f;

    [Header("GRID INFORMATIONS")]
    [NaughtyAttributes.ReadOnly]
    public List<Cell> cellList = new List<Cell>(); //Liste des cellules de la grid
    [NaughtyAttributes.ReadOnly]
    public List<Cell> cellMineList = new List<Cell>(); //Liste de mines de la grid

    [Header("MINE LEFT")]
    public TMP_Text theoricalMineLeftText;
    [NaughtyAttributes.ReadOnly]
    public int numberOfMineLeft;
    [NaughtyAttributes.ReadOnly]
    public int theoricalMineLeft;
    #endregion
    
    #region PROCEDURAL GRID GENERATION
    public void GenerateGrid(Vector2Int gridSize)
    {
        if (cellPrefab == null)
        {
            Debug.LogError("Prefab de cellule non assign� !");
            return;
        }

        // Efface les anciennes cellules si la grille est reg�n�r�e
        ClearGrid();

        // Parcourir les lignes et colonnes pour g�n�rer la grille
        for (int row = 0; row < gridSize.y; row++)
        {
            for (int col = 0; col < gridSize.x; col++)
            {
                Vector2 gridOffset = GetGridOffset(cellSize, gridSize);
                // Calculer la position de chaque cellule (ajust�e par l'offset)
                Vector2 cellPosition = new Vector2(col * cellSize, -row * cellSize) + gridOffset ;

                Cell newCell = CellInstanciation(cellPosition, row, col);
                newCell.Initialize(new Vector2Int(row, col));
            }
        }
        //G�n�re la liste des voisins
        foreach (Cell cell in cellList)
        {
            cell.GenerateNeighborsList(this);
        }

        //Transforme tous les enfants en Empty
        foreach (Cell cell in cellList)
        {
            Cell cellToDefine = cell.GetComponent<Cell>();
            cellToDefine.ChangeType(CellType.Empty);
        }
        //Set mines
        SetCellType(GameManager.Instance.currentRoomSettings.roomPourcentageOfMine, CellType.Mine);
        SetCellType(GameManager.Instance.currentRoomSettings.roomPourcentageOfNone, CellType.None);

        //Setup l'animation d'apparition
        ActiveListOfCells(timeBetweenApparition, RoomState.FogOfWar);
    }

    private void SetCellType(int pourcentageOfType, CellType cellType)
    {
        if (cellList.Count == 0)
        {
            Debug.LogWarning("La liste des enfants est vide !");
            return;
        }

        // S'assurer que le nombre d'objets à changer ne dépasse pas la taille de la liste
        int countToChange = Mathf.RoundToInt(cellList.Count * (pourcentageOfType / 100f));

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
            if (cell != null && cellType == CellType.Mine)
            {
                cell.ChangeType(CellType.Mine);
                cellMineList.Add(randomCell);
            }
            else if (cell != null && cellType == CellType.None)
            {
                cell.ChangeType(CellType.None);
            }
        }
    }

    public void SetItemsType(CellType cellType, int numberOfItem, ItemTypeEnum itemType = ItemTypeEnum.None)
    {
        // Crée la liste des cellules vides + hint
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

        // Liste temporaire pour �viter les doublons
        List<Cell> selectedCells = new List<Cell>();

        for (int i = 0; i < numberOfItem; i++)
        {
            // Génère un index al�atoire parmi les cellules restantes
            int randomIndex = UnityEngine.Random.Range(0, emptyCellsList.Count);

            // Sélectionne une cellule et la retire de la liste temporaire
            Cell selectedCell = emptyCellsList[randomIndex];
            emptyCellsList.RemoveAt(randomIndex);

            // Ajoute la cellule à la liste des cellules sélectionnées
            selectedCells.Add(selectedCell);
        }

        // Change le type de chaque cellule s�lectionn�e
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
            if (cell.currentType != CellType.None)
            {
                cell.UpdateRegardingNeighbors();
            }
            else
            {
                cell.ChangeType(CellType.None);
            }
        }
    }
    #endregion PROCEDURAL GRID GENERATION

    #region LOADED GRID GENERATION
    public void LoadGridFromString(string gridString, Vector2Int gridSize)
    {
        //Retourne une erreur s'il n'y a pas de string
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
            // D�couper chaque cellule par "_"
            string[] cellInfo = cellData.Split('_');
            if (cellInfo.Length != 5) continue; // Si les donn�es ne sont pas compl�tes, ignorer

            // Extraire les coordonn�es et les autres informations
            int x = int.Parse(cellInfo[0]);
            int y = int.Parse(cellInfo[1]);
            string stateAbbreviation = cellInfo[2];
            string typeAbbreviation = cellInfo[3];
            string itemTypeAbbreviation = cellInfo[4];

            // Cr�er une nouvelle cellule � ces coordonn�es
            // Calculer la position de chaque cellule (ajust�e par l'offset)
            Vector2 gridOffset = GetGridOffset(cellSize, gridSize);
            Vector2 cellPosition = new Vector2(y * cellSize, -x * cellSize) + gridOffset;

            // Instancier une nouvelle cellule
            Cell newCell = CellInstanciation(cellPosition, x, y);

            // Convertir les abr�viations en valeurs d'enum
            CellState state = GetStateFromAbbreviation(stateAbbreviation);
            CellType type = GetTypeFromAbbreviation(typeAbbreviation);
            ItemTypeEnum itemType = GetItemTypeFromAbbreviation(itemTypeAbbreviation);

            // Initialiser la cellule avec ses nouveaux �tats
            newCell.currentState = state;
            newCell.currentType = type;
            newCell.currentItemType = itemType;

            newCell.Initialize(new Vector2Int(x, y)); // Initialisation avec les bonnes coordonn�es et le bon �tat
        }
        foreach (Cell cell in cellList)
        {
            cell.GenerateNeighborsList(this);
        }
        SetCellsVisuals();
        ActiveListOfCells(timeBetweenApparition, GameManager.Instance.dungeonManager.currentRoom.currentRoomState);
    }

    private CellState GetStateFromAbbreviation(string abbreviation)
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

    private CellType GetTypeFromAbbreviation(string abbreviation)
    {
        return abbreviation switch
        {
            "Em" => CellType.Empty,
            "Mi" => CellType.Mine,
            "Hi" => CellType.Hint,
            "Ga" => CellType.Gate,
            "It" => CellType.Item,
            "No" => CellType.None,
            _ => throw new ArgumentException($"Abréviation inconnue : {abbreviation}")
        };
    }

    private ItemTypeEnum GetItemTypeFromAbbreviation(string abbreviation)
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

    #endregion LOADED GRID GENERATION

    #region COMMON GENERATION FONCTIONS

    private Vector2 GetGridOffset(float cellSize, Vector2Int gridSize)
    {
        // Calcul de l'offset pour centrer la grille
        float gridWidth = gridSize.x * cellSize; // Largeur totale de la grille
        float gridHeight = gridSize.y * cellSize;   // Hauteur totale de la grille

        // Ajustement pour la parit� des dimensions
        float xAdjustment = (gridSize.x % 2f == 0f) ? 0f : cellSize / 2f; // D�calage si impair
        float yAdjustment = (gridSize.y % 2f == 0f) ? 0f : -cellSize / 2f; // D�calage si impair

        Vector2 gridOffset = new Vector2(
            -gridWidth / 2f + cellSize / 2f + xAdjustment, // Ajustement horizontal
            gridHeight / 2f - cellSize / 2f + yAdjustment  // Ajustement vertical
        );
        return gridOffset;
    }

    private Cell CellInstanciation(Vector2 cellPosition, int row, int col)
    {
        Cell newCell = Instantiate(cellPrefab, cellPosition, Quaternion.identity);
        newCell.transform.SetParent(transform);
        cellList.Add(newCell);
        newCell.name = $"Cell_{row}_{col}"; // Renommer la cellule pour faciliter le d�bogage
        return newCell;
    }

    public string SaveGridString()
    {
        System.Text.StringBuilder gridStringBuilder = new System.Text.StringBuilder();

        foreach (Cell cell in cellList)
        {
            // Coordonn�es de la cellule
            int x = cell._cellPosition.x;
            int y = cell._cellPosition.y;

            // �tat de la cellule (par exemple "em" pour Empty, "co" pour Cover)
            string state = cell.currentState.ToString().Substring(0, 2);
            string type = cell.currentType.ToString().Substring(0, 2);
            string itemType = cell.currentItemType.ToString().Substring(0, 2);

            // Ajouter � la cha�ne sous forme : x_y_state|
            gridStringBuilder.Append($"{x}_{y}_{state}_{type}_{itemType}|");
        }
        // Retirer le dernier caract�re "|" pour une cha�ne propre
        if (gridStringBuilder.Length > 0)
        {
            gridStringBuilder.Length--;
        }
        return gridStringBuilder.ToString();
    }

    private void ClearGrid()
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
    #endregion COMMON GENERATION FONCTIONS

    #region GRID VISUAL FONCTIONS
    public void ActiveListOfCells(float timeBetweenApparition, RoomState roomState)
    {
        if (roomState != RoomState.FogOfWar)
        {
            foreach (Cell cell in cellList)
            {
                cell.gameObject.SetActive(true);
            }
        }
        else
        {
            StartCoroutine(CO_ActiveCellsWithDelay(timeBetweenApparition));
        }
    }

    private IEnumerator CO_ActiveCellsWithDelay(float timeBetweenApparition)
    {
        // Grouper les cellules par distance diagonale
        Dictionary<int, List<Cell>> diagonalGroups = new Dictionary<int, List<Cell>>();

        foreach (Cell cell in cellList)
        {
            // Calculer la distance diagonale
            int diagonalIndex = cell._cellPosition.x + cell._cellPosition.y;

            // Ajouter la cellule dans le groupe correspondant
            if (!diagonalGroups.ContainsKey(diagonalIndex))
            {
                diagonalGroups[diagonalIndex] = new List<Cell>();
            }
            diagonalGroups[diagonalIndex].Add(cell);
        }

        // Tri des groupes par distance diagonale (clé du dictionnaire)
        var sortedKeys = diagonalGroups.Keys.OrderBy(key => key).ToList();

        // Faire apparaître chaque groupe avec un délai
        foreach (int key in sortedKeys)
        {
            foreach (Cell cell in diagonalGroups[key])
            {
                cell.gameObject.SetActive(true); // Activer la cellule
            }
            yield return new WaitForSecondsRealtime(timeBetweenApparition); // Délai entre les groupes
        }
    }
    
    
    #endregion GRID VISUAL FONCTIONS

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
    private List<Cell> GetCoverCellsByType(CellType typeOfCellWanted)
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

    private int GetTheoricalMineLeft()
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
            Cell neighbor = cellList.Find(cell => cell._cellPosition == neighborPosition);

            if (neighbor != null)
            {
                neighbors.Add(neighbor); // Ajoute le voisin � la liste
            }
        }
        return neighbors;
    }
    
    public void CheckRoomCompletion()
    {
        int nbOfCoverCells = cellList.Count - GetCellsByState(CellState.Reveal).Count;
        int nbOfMine = GetCellsByType(CellType.Mine).Count;
        if (nbOfMine == nbOfCoverCells && nbOfMine == GetCellsByState(CellState.Flag).Count)
        {
            GameManager.Instance.dungeonManager.currentRoom.ChangeRoomSate(RoomState.Complete);
        }
    }
    #endregion GET GRID INFORMATIONS

    #region MINE COUNTER // A DÉPLACER DANS PLAYER OU AUTRE PLUS PERTINENT
    public void UpdateMineCounter()
    {
        theoricalMineLeft = GetTheoricalMineLeft();
        theoricalMineLeftText.text = theoricalMineLeft.ToString();
    }
    #endregion
}
