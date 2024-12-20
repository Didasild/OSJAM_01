using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CellEditor))]
public class CellEditorInspector : Editor
{
    public override void OnInspectorGUI()
    {
        // Récupérer l'instance de la cellule
        CellEditor cellEditor = (CellEditor)target;

        // Dessiner les champs par défaut
        DrawDefaultInspector();

        // Vérifier que le SpriteRenderer est assigné
        if (cellEditor.cellStateVisual == null)
        {
            EditorGUILayout.HelpBox("SpriteRenderer non assigné.", MessageType.Warning);
            return;
        }

        // Mettre à jour le sprite en fonction de l'état
        if (GUILayout.Button("Update Sprite"))
        {
            UpdateCellVisual(cellEditor);
        }
    }

    private void UpdateCellVisual(CellEditor cellEditor)
    {
        switch (cellEditor.cellState)
        {
            case CellState.Cover:
                cellEditor.cellStateVisual.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/2D/Tile_Cover.png");
                break;
            case CellState.Reveal:
                cellEditor.cellStateVisual.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets//Resources/2D/Tile_Empty.png");
                break;
        }

        // Appliquer immédiatement les modifications
        EditorUtility.SetDirty(cellEditor.cellStateVisual);
        Debug.Log($"Sprite mis à jour pour l'état : {cellEditor.cellState}");
    }
}
