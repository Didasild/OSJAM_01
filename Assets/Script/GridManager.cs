using System;
using System.Collections.Generic;
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
    [NaughtyAttributes.ReadOnly]
    public List<Cell> cellProceduralList = new List<Cell>();

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
        ClearRoom();

        // Parcourir les lignes et colonnes pour g�n�rer la grille
        for (int row = 0; row < gridSize.y; row++)
        {
            for (int col = 0; col < gridSize.x; col++)
            {
                Vector2 gridOffset = GetRoomOffset(cellSize, gridSize);
                // Calculer la position de chaque cellule (ajust�e par l'offset)
                Vector2 cellPosition = new Vector2(col * cellSize, -row * cellSize) + gridOffset ;

                Cell newCell = CellInstanciation(cellPosition, row, col);
                newCell.Initialize(new Vector2Int(row, col));
            }
        }
        //Génère la liste des voisins
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
        //Set mines et les none
        SetCellType(GameManager.Instance.currentRoomSettings.roomPourcentageOfMine, CellType.Mine, cellList);
        SetCellType(GameManager.Instance.currentRoomSettings.roomPourcentageOfNone, CellType.None, cellList);

        //Setup l'animation d'apparition
        GameManager.visualManager.ActiveListOfCells(timeBetweenApparition, RoomState.FogOfWar);
    }

    public void FirstClickGeneration(Cell cellClicked)
    {
        cellClicked.ChangeType(CellType.Empty);
        cellClicked.RemoveNeighborsMine();
        if (GameManager.Instance.currentRoomSettings.haveStair)
        {
            SetItemsType(CellType.Gate, 1, cellList);
        }
        SetItemsType(CellType.Item, GameManager.Instance.currentRoomSettings.GetNumberOfItem(ItemTypeEnum.Potion), cellList, ItemTypeEnum.Potion);
        SetItemsType(CellType.Item, GameManager.Instance.currentRoomSettings.GetNumberOfItem(ItemTypeEnum.Sword), cellList, ItemTypeEnum.Sword);
        SetNoneState();
    }
    private void SetCellType(int pourcentageOfType, CellType cellType, List<Cell> usedCellList)
    {
        if (usedCellList.Count == 0)
        {
            Debug.LogWarning("La liste des enfants est vide !");
            return;
        }

        // S'assurer que le nombre d'objets à changer ne dépasse pas la taille de la liste
        int countToChange = Mathf.RoundToInt(usedCellList.Count * (pourcentageOfType / 100f));

        // Liste temporaire pour suivre les objets déjà modifiés
        List<Cell> alreadyChanged = new List<Cell>();

        for (int i = 0;i < countToChange;i++)
        {
            Cell randomCell = usedCellList[i];
            do
            {
                int randomIndex = UnityEngine.Random.Range(0, usedCellList.Count);
                randomCell = usedCellList[randomIndex];
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

    public void SetItemsType(CellType cellType, int numberOfItem, List<Cell> cellList, ItemTypeEnum itemType = ItemTypeEnum.None)
    {
        // Crée la liste des cellules vides + hint
        List<Cell> emptyCellsList = GetCoverCellsByType(CellType.Empty, cellList);
        if (cellType != CellType.Gate)
        {
            emptyCellsList.AddRange(GetCoverCellsByType(CellType.Hint, cellList));
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

        // Change le type de chaque cellule sélectionnée
        foreach (Cell cell in selectedCells)
        {
            cell.ChangeItemType(itemType);
            cell.ChangeType(cellType, itemType);
            cell.UpdateRegardingNeighbors(GameManager.Instance.currentRoomSettings.isFoW);
        }
    }

    public void SetCellsVisuals()
    {
        List<Cell> cellsWithExcluded = new List<Cell>(cellList);
        foreach (Cell cell in cellsWithExcluded)
        {
            if (cell.currentType != CellType.None)
            {
                cell.UpdateRegardingNeighbors(GameManager.Instance.currentRoomSettings.isFoW);
            }
            else
            {
                cell.ChangeType(CellType.None);
            }
        }
    }

    public void SetNoneState()
    {
        List<Cell> cellsNone = new List<Cell>(cellList);
        cellsNone = GetCellsByType(CellType.None);
        foreach (Cell cell in cellsNone)
        {
            cell.currentState = CellState.Reveal;
        }
    }
    
    #endregion PROCEDURAL GRID GENERATION

    #region LOADED ROOM GENERATION
    public void LoadRoomFromString(string roomString, Vector2Int roomSize, bool isRsp)
    {
        //Retourne une erreur s'il n'y a pas de string
        if (string.IsNullOrEmpty(roomString))
        {
            Debug.LogError("Room String est vide !");
            return;
        }
        // Efface l'ancienne grille
        ClearRoom();

        // Divise le string en segments pour chaque cellule
        string[] cellDataArray = roomString.Split('|');

        foreach (string cellData in cellDataArray)
        {
            // Découper chaque cellule par "_"
            string[] cellInfo = cellData.Split('_');
            if (cellInfo.Length < 5) return; // Si les données ne sont pas complètes, ignorer

            // Extraire les coordonnées et les autres informations
            int row = int.Parse(cellInfo[0]);
            int col = int.Parse(cellInfo[1]);
            string stateAbbreviation = cellInfo[2];
            string typeAbbreviation = cellInfo[3];
            string itemTypeAbbreviation = cellInfo[4];
            
            // Vérifie s'il y a un flag procédural (6e élément dans le tableau)
            bool isProcedural = cellInfo.Length > 5 && cellInfo[5] == "Pr";

            // Créer une nouvelle cellule à ces coordonnées
            // Calculer la position de chaque cellule (ajustée par l'offset)
            Vector2 roomOffset = GetRoomOffset(cellSize, roomSize);
            Vector2 cellPosition = new Vector2(col * cellSize, -row * cellSize) + roomOffset;
            
            // Convertir les abréviations en valeurs d'enum
            CellState state = GetStateFromAbbreviation(stateAbbreviation);
            CellType type = GetTypeFromAbbreviation(typeAbbreviation);
            ItemTypeEnum itemType = GetItemTypeFromAbbreviation(itemTypeAbbreviation);

            if (type != CellType.None)
            {
                // Instancier une nouvelle cellule
                Cell newCell = CellInstanciation(cellPosition, row, col);
            
                //Ajoute à la liste des cellules procédurales
                if (isProcedural)
                {
                    cellProceduralList.Add(newCell);
                }

                // Initialiser la cellule avec ses nouveaux états
                newCell.currentState = state;
                newCell.currentType = type;
                newCell.currentItemType = itemType;

                newCell.Initialize(new Vector2Int(row, col)); // Initialisation avec les bonnes coordonnées et le bon état
            }
        }

        if (isRsp && cellProceduralList.Count > 0)
        {
            if (GameManager.Instance.currentRoomSettings.haveStair)
            {
                SetItemsType(CellType.Gate, 1, cellProceduralList);
            }
            SetCellType(GameManager.Instance.currentRoomSettings.roomPourcentageOfMine, CellType.Mine, cellProceduralList);
            SetItemsType(CellType.Item, GameManager.Instance.currentRoomSettings.GetNumberOfItem(ItemTypeEnum.Potion), cellProceduralList, ItemTypeEnum.Potion);
            SetItemsType(CellType.Item, GameManager.Instance.currentRoomSettings.GetNumberOfItem(ItemTypeEnum.Sword), cellProceduralList, ItemTypeEnum.Sword);
        }
        
        foreach (Cell cell in cellList)
        {
            cell.GenerateNeighborsList(this);
        }
        cellMineList = GetCellsByType(CellType.Mine);
        SetCellsVisuals();
        GameManager.visualManager.ActiveListOfCells(timeBetweenApparition, GameManager.Instance.floorManager.currentRoom.currentRoomState);
    }

    public static CellState GetStateFromAbbreviation(string abbreviation)
    {
        return abbreviation switch
        {
            "Co" => CellState.Cover,
            "Cl" => CellState.Clicked,
            "Fl" => CellState.Flag,
            "Pl" => CellState.PlantedSword,
            "Re" => CellState.Reveal,
            "In" => CellState.Inactive,
            _ => throw new ArgumentException($"Abréviation inconnue : {abbreviation}")
        };
    }

    public static CellType GetTypeFromAbbreviation(string abbreviation)
    {
        return abbreviation switch
        {
            "Em" => CellType.Empty,
            "Mi" => CellType.Mine,
            "Hi" => CellType.Hint,
            "Ga" => CellType.Gate,
            "It" => CellType.Item,
            "No" => CellType.None,
            "Np" => CellType.Npc,
            _ => throw new ArgumentException($"Abréviation inconnue : {abbreviation}")
        };
    }

    public static ItemTypeEnum GetItemTypeFromAbbreviation(string abbreviation)
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

    #endregion LOADED ROOM GENERATION

    #region COMMON GENERATION FONCTIONS

    public static Vector2 GetRoomOffset(float cellSize, Vector2Int gridSize)
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
        // Retirer le dernier caractère "|" pour une chaine propre
        if (gridStringBuilder.Length > 0)
        {
            gridStringBuilder.Length--;
        }
        return gridStringBuilder.ToString();
    }

    private void ClearRoom()
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
        cellProceduralList = new List<Cell>();
    }
    #endregion COMMON GENERATION FONCTIONS

    #region ROOM COMPLETION CHECK
    public void CheckRoomCompletion(RoomCompletionCondition roomCondition)
    {
        if (GameManager.Instance.floorManager.currentRoom.currentRoomState == RoomState.Complete)
        {
            return;
        }
        bool switchToComplete = false;
        switch (roomCondition)
        {
            case RoomCompletionCondition.FlaggedAllMine:
                switchToComplete = FlaggedAllMineCondition();
                break;
            
            default:
                switchToComplete = FlaggedAllMineCondition();
                break;
        }

        if (switchToComplete)
        {
            GameManager.Instance.floorManager.currentRoom.ChangeRoomSate(RoomState.Complete);
            GameManager.visualManager.PlayRoomCompletionFeedbacks();
        }
    }

    private bool FlaggedAllMineCondition()
    {
        if (GetCellsByState(CellState.Flag).Count != GetCellsByType(CellType.Mine).Count || GetCellsByState(CellState.Cover).Count != 0)
        {
            return false;
        }
        foreach (Cell mineCell in GetCellsByType(CellType.Mine))
        {
            if (mineCell.currentState != CellState.Flag)
            {
                return false;
            }
        }
        return true;
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
    private List<Cell> GetCoverCellsByType(CellType typeOfCellWanted, List<Cell> cellList)
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

    public bool CheckFirstClickOnProcedural()
    {
        bool firstClickProcedural = false;
        int nbOfCellsCover = cellList.Count - GetCellsByState(CellState.Reveal).Count;
        int nbOfCells = cellList.Count;
        if (nbOfCells == nbOfCellsCover && GameManager.Instance.currentRoomSettings.proceduralRoom)
        {
            firstClickProcedural = true;
        }
        return firstClickProcedural;
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
