using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [Header("Grid General Settings")]
    public GameObject cellPrefab; // Le prefab de la cellule
    public int rows = 5;          // Nombre de lignes
    public int columns = 5;       // Nombre de colonnes
    public float cellSize = 16f;   // Taille des cellules (espacement)
    public List<GameObject> cellList = new List<GameObject>(); //Liste des cellules de la grid

    [Header("Grid Procedural Settings")]
    public int numberOfMine;
    public List<GameObject> cellMineList = new List<GameObject>(); //Liste de mines de la grid
    private Transform gridParent; // GameObjectParent de la grid
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gridParent = transform;
        GenerateGrid();
    }

    public void GenerateGrid()
    {
        if (cellPrefab == null)
        {
            Debug.LogError("Prefab de cellule non assigné !");
            return;
        }

        // Efface les anciennes cellules si la grille est regénérée
        ClearGrid();

        // Calcul de l'offset pour centrer la grille
        float gridWidth = columns * cellSize; // Largeur totale de la grille
        float gridHeight = rows * cellSize;   // Hauteur totale de la grille

        Vector3 gridOffset = new Vector3(-gridWidth / 2 + cellSize / 2, gridHeight / 2 - cellSize / 2, 0);

        // Parcourir les lignes et colonnes pour générer la grille
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                // Calculer la position de chaque cellule (ajustée par l'offset)
                Vector3 cellPosition = new Vector3(col * cellSize, -row * cellSize, 0) + gridOffset;

                // Instancier la cellule
                GameObject newCell = Instantiate(cellPrefab, cellPosition, Quaternion.identity);

                // Optionnel : Attacher la cellule à un parent dans la hiérarchie
                if (gridParent != null)
                {
                    newCell.transform.SetParent(gridParent);
                }

                // Renommer la cellule pour faciliter le débogage
                newCell.name = $"Cell_{row}_{col}";
            }
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform childTransform = transform.GetChild(i);
            cellList.Add(childTransform.gameObject);
        }

        SetCellsType(numberOfMine);
    }
    // Méthode pour effacer l'ancienne grille
    public void ClearGrid()
    {
        foreach (GameObject cell in cellList)
        {
            if (cell != null)
            {
                Destroy(cell);
            }
        }
        cellList.Clear();
    }

    public void SetCellsType(int numberOfMine)
    {
        if (cellList.Count == 0)
        {
            Debug.LogWarning("La liste des enfants est vide !");
            return;
        }

        //Transforme tout les enfants en Empty
        foreach (GameObject cell in cellList)
        {
            Cell cellToDefine = cell.GetComponent<Cell>();
            cellToDefine.ChangeType(Cell.Celltype.Empty);
        }

        // S'assurer que le nombre d'objets à changer ne dépasse pas la taille de la liste
        int countToChange = Mathf.Min(numberOfMine, cellList.Count);

        // Liste temporaire pour suivre les objets déjà modifiés
        List<GameObject> alreadyChanged = new List<GameObject>();

        for (int i = 0;i < countToChange;i++)
        {
            GameObject randomCell = cellList[i];
            do
            {
                int randomIndex = Random.Range(0, cellList.Count);
                randomCell = cellList[randomIndex];
            } while (alreadyChanged.Contains(randomCell));

            alreadyChanged.Add(randomCell);

            Cell cell = randomCell.GetComponent<Cell>();
            if (cell != null)
            {
                cell.ChangeType(Cell.Celltype.Mine);
                cellMineList.Add(randomCell.gameObject);
            }
        }
    }
}
