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
        for (int y = 0; y < gridSize.y; y++)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                Vector3 position = new Vector3(x * cellSpacing, y * cellSpacing, 0);
                GameObject cell = Instantiate(cellPrefab, position, Quaternion.identity, transform);
                cells.Add(cell.GetComponent<CellEditor>());
                
                // Configure la cellule
                CellEditor cellEditor = cell.GetComponent<CellEditor>();
                if (cellEditor != null)
                {
                    cellEditor.gridPosition = new Vector2Int(x, y);
                    cellEditor.Initialisation(cellVisualManager);
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

    public void CreateRoomScriptable()
    {
        
    }
}
