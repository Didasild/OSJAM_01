using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteInEditMode]
public class RoomEditor : MonoBehaviour
{
    #region VARIABLES
    [Header("____GENERATION")]
    public Vector2Int roomSize = new Vector2Int(5, 5); // Taille de la grille
    public RoomSettings roomSettings;
    
    [Header("____SAVE")]
    private string scriptableName;
    public bool isMandatory;
    public Chapters chapter;
    public int floorID;
    public int roomID;
    public RoomType roomType;
    public GenerationType generationType;
    
    //Procedural Functions
    public CellSelectionConditions cellSelectionConditions;
    public CellsTypeChange cellsTypeChange;
    public CellsStateChange cellsStateChange;
    
    [Header("____DEBUG")]
    public float cellSpacing = 1.0f;                  // Espacement entre les cellules
    public GameObject cellPrefab;                   // Prefab de la cellule
    public CellVisualManager cellVisualManager;
    [NaughtyAttributes.ReadOnly] public List<CellEditor> selectedCells;
    [NaughtyAttributes.ReadOnly] public List<CellEditor> cells;
    public string roomSaveString;
    private string defaultSaveFolder;
    
    [System.Serializable]
    public struct CellSelectionConditions
    {
        public bool onlySelected;
        public List<CellType> cellType;
        public List<CellState> cellState;
    }
    [System.Serializable]
    public struct CellsTypeChange
    {
        public CellType cellNewType;
        public ItemTypeEnum newItemType;
        public int numberTypeChange;
        public bool isAPourcentage;
    }
    [System.Serializable]
    public struct CellsStateChange
    {
        public CellState cellNewState;
        public int numberStateChange;
        public bool isAPourcentage;
    }
    #endregion
    
    #region GENERATION FUNCTIONS
    public void GenerateEditorRoom()
    {
        ClearEditorRoom();
        // Vérifiez que le prefab est assigné
        if (cellPrefab == null)
        {
            Debug.LogWarning("Cell prefab is not assigned.");
            return;
        }

        // Générer la grille
        for (int row = 0; row < roomSize.y; row++)
        {
            for (int col = 0; col < roomSize.x; col++)
            {
                Vector2 gridOffset = GridManager.GetGridOffset(cellSpacing, roomSize);                // Calculer la position de chaque cellule (ajust�e par l'offset)
                Vector2 cellPosition = new Vector2(col * cellSpacing, -row * cellSpacing) + gridOffset ;
                GameObject cell = Instantiate(cellPrefab, cellPosition, Quaternion.identity, transform);
                cells.Add(cell.GetComponent<CellEditor>());
                
                // Configure la cellule
                CellEditor cellEditor = cell.GetComponent<CellEditor>();
                if (cellEditor != null)
                {
                    cellEditor._cellPosition = new Vector2Int(row, col);
                    cellEditor.Initialize(cellVisualManager);
                }
            }
        }
        foreach (CellEditor cellEditor in cells)
        {
            cellEditor.neighborsCellList = GiveNeighbors(cellEditor._cellPosition);
        }
    }
    public List<CellEditor> GiveNeighbors(Vector2Int cellPosition)
    {
        List<CellEditor> neighbors = new List<CellEditor>();
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
            CellEditor neighbor = cells.Find(cell => cell._cellPosition == neighborPosition);

            if (neighbor != null)
            {
                neighbors.Add(neighbor); // Ajoute le voisin � la liste
            }
        }

        return neighbors;
    }
    public void ClearEditorRoom()
    {
        // Supprimez tous les enfants
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
        cells = new List<CellEditor>();
    }
    
    #endregion GENERATION FUNCTIONS

    #region SAVE FUNCTIONS
    public void GenerateHintCells()
    {
        List<CellEditor> emptyCells = new List<CellEditor>();
        foreach (CellEditor cell in cells)
        {
            if (cell.cellType == CellType.Empty)
            {
                emptyCells.Add(cell);
            }
        }

        foreach (CellEditor cell in emptyCells)
        {
            foreach (CellEditor neighbor in cell.neighborsCellList)
            {
                if (neighbor.cellType == CellType.Mine)
                {
                    // Change le type de la cellule si un voisin est de type "Mine"
                    cell.cellType = CellType.Hint;
                    cell.UpdateCellVisual();
                    cell.HighlightCell();
                    break;
                }
            }
        }
    }

    public void ClearCellsData()
    {
        List<CellEditor> hintCells = new List<CellEditor>();
        //Set les None en state reveal
        foreach (CellEditor cell in cells)
        {
            if (cell.cellType == CellType.None && cell.cellState != CellState.Reveal)
            {
                cell.cellState = CellState.Reveal;
                cell.HighlightCell();
            }

            if (cell.cellType == CellType.Hint)
            {
                hintCells.Add(cell);
            }
        }
        
        //Update les Hint

        foreach (CellEditor hintCell in hintCells)
        {
            bool isHint = false;
            foreach (CellEditor neighbor in hintCell.neighborsCellList)
            {
                if (neighbor.cellType == CellType.Mine)
                {
                    // Change le type de la cellule si un voisin est de type "Mine"
                    hintCell.cellType = CellType.Hint;
                    hintCell.UpdateCellVisual();
                    hintCell.HighlightCell();
                    isHint = true;
                    break;
                }
            }
            if (isHint == false)
            {
                hintCell.cellType = CellType.Empty;
                hintCell.UpdateCellVisual();
                hintCell.HighlightCell();
            }
        }
    }
    public void CreateRoomScriptable()
    {
        GenerateHintCells();
        ClearSavedString();
        roomSaveString = SaveRoomString();
        
        //A UPDATE lorsqu'il y a aura du semi procédural
        generationType = GenerationType.RL;
        
        //Set le nom
        defaultSaveFolder = "Assets/Resources/Chapters/" + chapter.ToString();
        scriptableName = chapter.ToString() + "_F" +  floorID.ToString("D2") + "_" + generationType.ToString() + "_" + roomType.ToString() + "_";
        
        //Crée l'instance
        string path = EditorUtility.SaveFilePanelInProject("Save Room", scriptableName, "asset", "Message test", defaultSaveFolder);
        if (string.IsNullOrEmpty(path))
            return;
        RoomSettings newRoomSettings = ScriptableObject.CreateInstance<RoomSettings>();
        
        //Set les parametres du room setting
        SetRoomSetting(newRoomSettings);
        
        // Sauvegarde l'instance dans le fichier spécifié
        AssetDatabase.CreateAsset(newRoomSettings, path);
        AssetDatabase.SaveAssets();
        
        // Met l'objet nouvellement créé en surbrillance dans le Project
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newRoomSettings;

        Debug.Log($"ScriptableObject created at {path}");
    }
    public String SaveRoomString()
    {
        ClearSavedString();
        System.Text.StringBuilder gridStringBuilder = new System.Text.StringBuilder();
        foreach (CellEditor cell in cells)
        {
            // Coordonnées de la cellule
            int x = cell._cellPosition.x;
            int y = cell._cellPosition.y;

            // �tat de la cellule (par exemple "em" pour Empty, "co" pour Cover)
            string state = cell.cellState.ToString().Substring(0, 2);
            string type = cell.cellType.ToString().Substring(0, 2);
            string itemType = cell.itemType.ToString().Substring(0, 2);

            // Ajouter à la chaine sous forme : x_y_state|
            gridStringBuilder.Append($"{x}_{y}_{state}_{type}_{itemType}|");
        }
        // Retirer le dernier caractère "|" pour une cha�ne propre
        if (gridStringBuilder.Length > 0)
        {
            gridStringBuilder.Length--;
        }
        return gridStringBuilder.ToString();
    }
    private void ClearSavedString()
    {
        roomSaveString = string.Empty;
    }

    private void SetRoomSetting(RoomSettings roomSettings)
    {
        roomSettings.roomLoadString = roomSaveString;
        roomSettings.proceduralRoom = false;
        roomSettings.Mandatory = isMandatory;
    }
    #endregion SAVE FUNCTIONS

    #region EDITOR FUNCTION
    public List<CellEditor> SelectCells(CellSelectionConditions cellSelectionConditions)
    {
        selectedCells = new List<CellEditor>();
        //Etablis la liste de cellule à traiter en fonction de la condition de selection
        List<CellEditor> matchingCells = new List<CellEditor>();
        if (cellSelectionConditions.onlySelected)
        {
            foreach (CellEditor cell in cells)
            {
                if (cell.isSelected)
                {
                    matchingCells.Add(cell);
                }
            }
        }
        else
        {
            matchingCells = cells;
        }
        // Vérifie les listes state et type 
        bool hasStateConditions = cellSelectionConditions.cellState != null && cellSelectionConditions.cellState.Count > 0;
        bool hasTypeConditions = cellSelectionConditions.cellType != null && cellSelectionConditions.cellType.Count > 0;

        // Si aucune condition n'est spécifiée, sélectionne toutes les cellules
        if (!hasStateConditions && !hasTypeConditions)
        {
            foreach (CellEditor cell in matchingCells)
            {
                selectedCells.Add(cell);
            }
            return matchingCells;
        }

        // Liste temporaire pour collecter les cellules correspondant aux conditions
        List<CellEditor> tempSelectedCells = new List<CellEditor>();

        foreach (CellEditor cell in matchingCells)
        {
            bool matchesState = true;
            bool matchesType = true;

            if (hasStateConditions)
            {
                matchesState = cellSelectionConditions.cellState.Contains(cell.cellState);
            }

            if (hasTypeConditions)
            {
                matchesType = cellSelectionConditions.cellType.Contains(cell.cellType);
            }

            if (matchesState && matchesType)
            {
                tempSelectedCells.Add(cell);
            }
        }

        // Ajoute les cellules correspondantes à la liste principale et applique le highlight
        foreach (CellEditor cell in tempSelectedCells)
        {
            selectedCells.Add(cell);
        }
        return selectedCells;
    }

    public void ChangeCellType(CellsTypeChange cellTypeChange)
    {
        int numberOfCells = cellsTypeChange.numberTypeChange;
        // Vérifie que le pourcentage est valide (entre 0 et 100)
        if (cellTypeChange.isAPourcentage)
        {
            numberOfCells = GetPourcentage(numberOfCells);
        }

        List<CellEditor> shuffledCellList = SelectCells(this.cellSelectionConditions);
        ShuffleList(shuffledCellList);
        for (int i = 0; i < numberOfCells; i++)
        {
            shuffledCellList[i].cellType = cellsTypeChange.cellNewType;
            if (cellTypeChange.cellNewType == CellType.Item)
            {
                shuffledCellList[i].itemType = cellsTypeChange.newItemType;
            }
            shuffledCellList[i].UpdateCellVisual();
            shuffledCellList[i].HighlightCell();
        }
    }

    public void ChangeCellState(CellsStateChange cellStateChange)
    {
        int numberOfCells = cellsStateChange.numberStateChange;

        if (cellStateChange.isAPourcentage)
        {
            numberOfCells = GetPourcentage(numberOfCells);
        }
        
        List<CellEditor> shuffledCellList = SelectCells(this.cellSelectionConditions);
        ShuffleList(shuffledCellList);
        for (int i = 0; i < numberOfCells; i++)
        {
            shuffledCellList[i].cellState = cellStateChange.cellNewState;
            shuffledCellList[i].UpdateCellVisual();
            shuffledCellList[i].HighlightCell();
        }
    }
    
    //Fonction pour retourner un pourcentage par rapport à une liste
    private int GetPourcentage(int numberToPourcent)
    {
        if (numberToPourcent < 0 || numberToPourcent > 100)
        {
            Debug.LogError("Le pourcentage doit être entre 0 et 100.");
            return numberToPourcent;
        }
        else
        {
            numberToPourcent = numberToPourcent * selectedCells.Count / 100;
        }

        return numberToPourcent;
    }
    
    // Fonction pour mélanger la liste
    private void ShuffleList(List<CellEditor> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            CellEditor temp = list[i];
            int randomIndex = UnityEngine.Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public void ClearConditions()
    {
        cellSelectionConditions.onlySelected = false;
        cellSelectionConditions.cellState = null;
        cellSelectionConditions.cellType = null;
    }
    
    #endregion EDITOR FUNCTIONS
}
