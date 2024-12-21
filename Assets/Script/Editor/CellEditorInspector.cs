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
    }
}
