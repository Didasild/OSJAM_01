using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteInEditMode]
public class RoomEditor : MonoBehaviour
{
    #region VARIABLES
    [FormerlySerializedAs("gridSize")] [Header("____GENERATION")]
    public Vector2Int roomSize = new Vector2Int(5, 5); // Taille de la grille
    
    [Header("____SAVE")]
    public string scriptableName;

    [Header("____PROCEDURAL FONCTIONS")]
    public CellSelectionConditions cellSelectionConditions;
    public CellsModifications cellsModifications;
    
    [Header("____DEBUG")]
    public float cellSpacing = 1.0f;                  // Espacement entre les cellules
    public GameObject cellPrefab;                   // Prefab de la cellule
    public CellVisualManager cellVisualManager;
    [NaughtyAttributes.ReadOnly] public List<CellEditor> selectedCells;
    [NaughtyAttributes.ReadOnly] public List<CellEditor> cells;
    public string roomSaveString;
    
    [System.Serializable]
    public struct CellSelectionConditions
    {
        public bool onlySelected;
        public List<CellState> cellState;
        public List<CellType> cellType;
    }
    [System.Serializable]
    public struct CellsModifications
    {
        public CellState cellNewState;
        public CellType cellNewType;
        public int numberOfModifications;
        //public bool isAPourcentage; A FAIRE SI NECESSAIRE
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
    public void CreateRoomScriptable()
    {
        ClearSavedString();
        roomSaveString = SaveRoomString();
        
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
    #endregion SAVE FUNCTIONS

    #region EDITOR FUNCTIONS
    
    
    
    public void SelectCells(CellSelectionConditions cellSelectionConditions)
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

        if (!hasStateConditions && !hasTypeConditions)
        {
            selectedCells = matchingCells;
        }

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
                selectedCells.Add(cell);
            }
        }
    }
    #endregion EDITOR FUNCTIONS
}
