using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;


[ExecuteInEditMode]
public class RoomEditor : MonoBehaviour
{
    #region VARIABLES
    [Header("____GENERATION")]
    public Vector2Int roomSize = new Vector2Int(5, 5); // Taille de la grille
    public RoomSettings roomSettingsToLoad;
    public RoomSettings roomSettingsToSave;
    
    [Header("____SAVE")]
    private string _scriptableName;
    public bool isMandatory;
    public bool isInFoW;
    public Chapters chapter;
    public int floorID;
    public int roomID;
    public RoomType roomType;
    public GenerationType generationType;
    public VolumeProfile roomVolumeProfile;
    
    //Procedural Functions
    public CellSelectionConditions cellSelectionConditions;
    public CellsTypeChange cellsTypeChange;
    public CellsStateChange cellsStateChange;
    public List<RoomSettings.ItemRange> itemRanges;
    public bool haveStair;
    public int pourcentageOfRandomMine;
    
    [Header("____DEBUG")]
    public float cellSpacing = 1.0f;                  // Espacement entre les cellules
    public GameObject cellPrefab;                   // Prefab de la cellule
    public VisualEditorManager visualManager;
    [ReadOnly] public List<CellEditor> selectedCells;
    [ReadOnly] public List<CellEditor> cells;
    public string roomSaveString;
    private string _defaultSaveFolder;
    
    [Serializable]
    public struct CellSelectionConditions
    {
        public bool onlySelected;
        public List<CellType> cellType;
        public List<CellState> cellState;
    }
    [Serializable]
    public struct CellsTypeChange
    {
        public CellType cellNewType;
        public ItemTypeEnum newItemType;
        public int numberTypeChange;
        public bool isAPourcentage;
    }
    [Serializable]
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

        if (cellPrefab == null)
        {
            Debug.LogWarning("Cell prefab is not assigned.");
            return;
        }

        // Calculer le centre de la grille en fonction de sa taille
        Vector2 gridOffset = new Vector2((roomSize.x - 1) * cellSpacing * -0.5f, (roomSize.y - 1) * cellSpacing * 0.5f);

        for (int row = 0; row < roomSize.y; row++)
        {
            for (int col = 0; col < roomSize.x; col++)
            {
                // Calculer la position de la cellule en appliquant l'offset centré
                Vector2 cellPosition = new Vector2(col * cellSpacing, -row * cellSpacing) + gridOffset;
                GameObject cell = Instantiate(cellPrefab, cellPosition, Quaternion.identity, transform);
                cells.Add(cell.GetComponent<CellEditor>());
            
                // Configurer la cellule
                CellEditor cellEditor = cell.GetComponent<CellEditor>();
                if (cellEditor != null)
                {
                    cellEditor.cellPosition = new Vector2Int(row, col);
                    cellEditor.Initialize(visualManager);
                }
            }
        }

        foreach (CellEditor cellEditor in cells)
        {
            cellEditor.neighborsCellList = GiveNeighbors(cellEditor.cellPosition);
        }

        roomSettingsToLoad = null;
        roomSettingsToSave = null;
    }
    public void LoadEditorRoom()
    {
        roomSaveString = roomSettingsToLoad.roomIDString;
        roomSize = roomSettingsToLoad.GetRoomSizeFromString(roomSaveString);
        
        if (string.IsNullOrEmpty(roomSaveString))
        {
            Debug.LogError("Room String est vide !");
            return;
        }
        ClearEditorRoom();
        
        // Divise le string en segments pour chaque cellule
        string[] cellDataArray = roomSaveString.Split('|');

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
            Vector2 roomOffset = GridManager.GetRoomOffset(cellSpacing, roomSize);
            Vector2 cellPosition = new Vector2(col * cellSpacing, -row * cellSpacing) + roomOffset;
            
            // Instancier une nouvelle cellule
            GameObject cell = Instantiate(cellPrefab, cellPosition, Quaternion.identity, transform);
            cells.Add(cell.GetComponent<CellEditor>());
            CellEditor cellEditor = cell.GetComponent<CellEditor>();
            
            // Convertir les abréviations en valeurs d'enum
            CellState state = GridManager.GetStateFromAbbreviation(stateAbbreviation);
            CellType type = GridManager.GetTypeFromAbbreviation(typeAbbreviation);
            ItemTypeEnum itemType = GridManager.GetItemTypeFromAbbreviation(itemTypeAbbreviation);

            if (isProcedural)
            {
                cellEditor.proceduralCell = true;
                itemRanges  = new List<RoomSettings.ItemRange>();
                foreach (var itemRange in roomSettingsToLoad.itemRanges)
                {
                   itemRanges.Add(new RoomSettings.ItemRange {itemType = itemRange.itemType, min = itemRange.min, max = itemRange.max});
                }
                haveStair = roomSettingsToLoad.haveStair;
                pourcentageOfRandomMine = roomSettingsToLoad.roomPourcentageOfMine;
            }
            cellEditor.cellState = state;
            cellEditor.cellType = type;
            cellEditor.itemType = itemType;
            
            if (cellEditor != null)
            {
                cellEditor.cellPosition = new Vector2Int(row, col);
                cellEditor.Initialize(visualManager);
            }

            if (cellEditor.cellType == CellType.Npc)
            {
                foreach (var npcData in roomSettingsToLoad.npcDatas)
                {
                    if (npcData.npcPosition == cellEditor.cellPosition)
                    {
                        cellEditor.npcSettings = npcData.npcSettings;
                        cellEditor.dialogSequenceOverride = npcData.dialogSequenceOverride;
                        break;
                    }
                }
            }
            
            //Update les infos de save de l'inspector
            UpdateEditorInfos(roomSettingsToLoad);
        }

        foreach (CellEditor cellEditor in cells)
        {
            cellEditor.neighborsCellList = GiveNeighbors(cellEditor.cellPosition);
        }
    }
    private List<CellEditor> GiveNeighbors(Vector2Int cellPosition)
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
            CellEditor neighbor = cells.Find(cell => cell.cellPosition == neighborPosition);

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

    private void UpdateEditorInfos(RoomSettings roomSettings)
    {
        roomSettingsToSave = roomSettings;
        isMandatory = roomSettings.mandatory;
        roomType = roomSettings.roomType;
        roomVolumeProfile = roomSettings.roomVolumeProfile;
    }

    public void AddCells(string cellsToAdd)
    {
        Vector3 cellPosition = new Vector3();
        int maxX = GetGridSize().x;
        int maxY = GetGridSize().y;
        List<CellEditor> newCells = new List<CellEditor>();

        foreach (CellEditor cell in cells)
        {
            switch (cellsToAdd)
            {
                case "Top":
                    if (cell.cellPosition.x == 0)
                    {
                        cellPosition = new Vector3 (cell.transform.position.x, cell.transform.position.y + cellSpacing, 0);
                        CellEditor newCell = InstantiateCell(cellPosition);
                        newCell.cellPosition += new Vector2Int(cell.cellPosition.x + 1, cell.cellPosition.y);
                        newCells.Add(newCell);
                    }
                    break; 
                case "Right":
                    if (cell.cellPosition.y == maxY)
                    {
                        cellPosition = new Vector3 (cell.transform.position.x + cellSpacing, cell.transform.position.y, 0);
                        CellEditor newCell = InstantiateCell(cellPosition);
                        newCell.cellPosition += new Vector2Int(cell.cellPosition.x, cell.cellPosition.y + 1);
                        newCells.Add(newCell);
                    }
                    break;
                case "Bot":
                    if (cell.cellPosition.x == maxX)
                    {
                        cellPosition = new Vector3 (cell.transform.position.x, cell.transform.position.y - cellSpacing, 0);
                        CellEditor newCell = InstantiateCell(cellPosition);
                        newCell.cellPosition += new Vector2Int(cell.cellPosition.x + 1, cell.cellPosition.y);
                        newCells.Add(newCell);
                    }
                    break;
                case "Left":
                    
                    break;
            }
        }
        foreach (CellEditor newCell in newCells)
        {
            cells.Add(newCell);
            newCell.neighborsCellList = GiveNeighbors(newCell.cellPosition);
        }
    }
    private Vector2Int GetGridSize()
    {
        Vector2Int gridSize = new Vector2Int();
       
        // Trouver la taille actuelle de la grille (en supposant que ta grille est rectangulaire)
        foreach (CellEditor cell in cells)
        {
            if (cell.cellPosition.x > gridSize.x)
                gridSize.x = cell.cellPosition.x;

            if (cell.cellPosition.y > gridSize.y)
                gridSize.y = cell.cellPosition.y;
        }
        return gridSize;
    }
    
    

    private CellEditor InstantiateCell(Vector3 cellPosition)
    {
        // Instancier une nouvelle cellule
        GameObject cell = Instantiate(cellPrefab, cellPosition, Quaternion.identity, transform);
        CellEditor cellEditor = cell.GetComponent<CellEditor>();
        cellEditor.Initialize(visualManager);
        return cellEditor;
    }




    #endregion GENERATION FUNCTIONS

    #region SAVE FUNCTIONS
    public void GenerateHintCells()
    {
        List<CellEditor> emptyCells = new List<CellEditor>();
        foreach (CellEditor cell in cells)
        {
            if (cell.cellType == CellType.Empty && !cell.proceduralCell)
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
        GenerateHintCells();
        List<CellEditor> hintCells = new List<CellEditor>();
        List<CellEditor> mineCells = new List<CellEditor>();
        //Set les None en state reveal
        foreach (CellEditor cell in cells)
        {
            if (cell.cellType == CellType.None && cell.cellState != CellState.Inactive)
            {
                cell.cellState = CellState.Inactive;
                cell.HighlightCell();
            }
            
            if (cell.cellType == CellType.Hint)
            {
                hintCells.Add(cell);
            }

            if (cell.cellType == CellType.Mine)
            {
                mineCells.Add(cell);
            }
        }
        
        //Update les Hint
        foreach (CellEditor hintCell in hintCells)
        {
            int numberOfNeighborsMine = 0;
            foreach (CellEditor neighbor in hintCell.neighborsCellList)
            {
                if (neighbor.cellType == CellType.Mine)
                {
                    numberOfNeighborsMine++;
                }
            }
            if (numberOfNeighborsMine == 0)
            {
                hintCell.cellType = CellType.Empty;
                hintCell.UpdateCellVisual();
                hintCell.HighlightCell();
            }
            else if (numberOfNeighborsMine != hintCell.hintNumber)
            {
                hintCell.UpdateCellVisual();
                hintCell.HighlightCell();
            }
        }
        
        //Update les mines
        foreach (CellEditor mineCell in mineCells)
        {
            if (mineCell.cellState == CellState.Reveal)
            {
                mineCell.cellState = CellState.Cover;
                mineCell.UpdateCellVisual();
                mineCell.HighlightCell();
            }
        }
        
        //check si RSP
        if (isRoomSemiProcedural())
        {
            generationType = GenerationType.RSP;
        }
        else
        {
            generationType = GenerationType.RL;
        }
    }
    public void CreateRoomScriptable()
    {
        GenerateHintCells();
        ClearCellsData();
        ClearSavedString();
        roomSaveString = SaveRoomString();
        
        //A UPDATE lorsqu'il y a aura du semi procédural
        if (isRoomSemiProcedural())
        {
            generationType = GenerationType.RSP;
        }
        else
        {
            generationType = GenerationType.RL;
        }
        
        //Set le nom
        _defaultSaveFolder = "Assets/Resources/Chapters/" + chapter.ToString();
        _scriptableName = chapter.ToString() + "_F" +  floorID.ToString("D2") + "_" + generationType.ToString() + "_" + roomType.ToString() + "_" + roomID.ToString("D2");
        
        //Crée l'instance
        string path = EditorUtility.SaveFilePanelInProject("Save Room", _scriptableName, "asset", "Message test", _defaultSaveFolder);
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
        roomSettingsToSave = newRoomSettings;
        EditorUtility.SetDirty(newRoomSettings);

        Debug.Log($"ScriptableObject created at {path}");

    }

    public void UpdateExistingRoomScriptable()
    {
        GenerateHintCells();
        ClearCellsData();
        SetRoomSetting(roomSettingsToSave);

        EditorUtility.FocusProjectWindow();
        EditorUtility.SetDirty(roomSettingsToSave);
    }
    private String SaveRoomString()
    {
        ClearSavedString();
        System.Text.StringBuilder gridStringBuilder = new System.Text.StringBuilder();
        foreach (CellEditor cell in cells)
        {
            // Coordonnées de la cellule
            int x = cell.cellPosition.x;
            int y = cell.cellPosition.y;

            // �tat de la cellule (par exemple "em" pour Empty, "co" pour Cover)
            string state = cell.cellState.ToString().Substring(0, 2);
            string type = cell.cellType.ToString().Substring(0, 2);
            string itemType = cell.itemType.ToString().Substring(0, 2);
            if (cell.proceduralCell)
            {
                string proceduralString = "Pr";
                // Ajouter à la chaine sous forme : x_y_state|
                gridStringBuilder.Append($"{x}_{y}_{state}_{type}_{itemType}_{proceduralString}|");
            }
            else
            {
                // Ajouter à la chaine sous forme : x_y_state|
                gridStringBuilder.Append($"{x}_{y}_{state}_{type}_{itemType}|");
            }
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
        roomSettings.roomIDString = SaveRoomString();
        roomSettings.proceduralRoom = false;
        roomSettings.mandatory = isMandatory;
        roomSettings.isFoW = isInFoW;
        roomSettings.roomVolumeProfile = roomVolumeProfile;
        if (isRoomSemiProcedural())
        {
            roomSettings.haveProceduralCells = true;
            roomSettings.haveStair = haveStair;
            roomSettings.roomPourcentageOfMine = pourcentageOfRandomMine;
            // Copie profonde de la liste itemRanges
            roomSettings.itemRanges = new List<RoomSettings.ItemRange>();
            foreach (var itemRange in itemRanges)
            {
                roomSettings.itemRanges.Add(new RoomSettings.ItemRange { itemType = itemRange.itemType, min = itemRange.min, max = itemRange.max });
            }
        }
        else
        {
            roomSettings.haveProceduralCells = false;
        }

        if (!containNpc()) return;
        roomSettings.npcDatas = new List<RoomSettings.NpcData>();
        foreach (CellEditor cell in cells)
        {
            if (cell.cellType == CellType.Npc)
            {
                roomSettings.npcDatas.Add(new RoomSettings.NpcData {npcPosition = cell.cellPosition, npcSettings = cell.npcSettings, dialogSequenceOverride = cell.dialogSequenceOverride});
            }
        }
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

    public void SetRandomCell(bool setRandomCell)
    {
        List<CellEditor> selectedCellList = SelectCells(this.cellSelectionConditions);
        foreach (CellEditor selectedCells in selectedCellList)
        {
            selectedCells.proceduralCell = setRandomCell;
            selectedCells.UpdateCellVisual();
            selectedCells.HighlightCell();
        }
        if (isRoomSemiProcedural())
        {
            generationType = GenerationType.RSP;
        }
        else
        {
            generationType = GenerationType.RL;
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

    private bool isRoomSemiProcedural()
    {
        bool roomSemiProcedural = false;
        foreach (CellEditor cell in cells)
        {
            if (cell.proceduralCell)
            {
                roomSemiProcedural = true;
                break;
            }
        }
        return roomSemiProcedural;
    }

    private bool containNpc()
    {
        bool npcCell = false;
        foreach (CellEditor cell in cells)
        {
            if (cell.cellType == CellType.Npc)
            {
                npcCell = true;
                break;
            }
        }
        return npcCell;
    }
    #endregion EDITOR FUNCTIONS
}
