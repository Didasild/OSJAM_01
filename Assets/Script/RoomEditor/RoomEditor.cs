using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RoomEditor : MonoBehaviour
{
    [Header("____ROOM SETTINGS")]
    public Vector2Int gridSize = new Vector2Int(5, 5); // Taille de la grille
    public float cellSpacing = 1.0f;                  // Espacement entre les cellules
    public GameObject cellPrefab;                   // Prefab de la cellule
    public CellVisualManager cellVisualManager;
    
    [Header("____ROOM INFOS")]
    public List<CellEditor> cells;

    public string roomSaveString;
    
    private void OnValidate()
    {
        
    }

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
        for (int row = 0; row < gridSize.y; row++)
        {
            for (int col = 0; col < gridSize.x; col++)
            {
                Vector2 gridOffset = GridManager.GetGridOffset(cellSpacing, gridSize);                // Calculer la position de chaque cellule (ajust�e par l'offset)
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

    public String SaveRoomString()
    {
        ClearSavedString();
        System.Text.StringBuilder gridStringBuilder = new System.Text.StringBuilder();
        foreach (CellEditor cell in cells)
        {
            // Coordonn�es de la cellule
            int x = cell._cellPosition.x;
            int y = cell._cellPosition.y;

            // �tat de la cellule (par exemple "em" pour Empty, "co" pour Cover)
            string state = cell.cellState.ToString().Substring(0, 2);
            string type = cell.cellType.ToString().Substring(0, 2);
            string itemType = cell.itemType.ToString().Substring(0, 2);

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

    public void CreateRoomScriptable()
    {
        
    }

    public void ClearSavedString()
    {
        roomSaveString = string.Empty;
    }
}
