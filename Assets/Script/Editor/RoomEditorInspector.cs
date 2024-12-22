using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RoomEditor))]
public class RoomEditorInspector : Editor
{
    // Référence à l'objet sérialisé
    SerializedProperty cellSelectionConditionsProperty;
    private bool debugFoldout;

    void OnEnable()
    {
        cellSelectionConditionsProperty = serializedObject.FindProperty("cellSelectionConditions");
    }
    public override void OnInspectorGUI()
    {
        // Synchronise les modifications dans l'inspecteur
        serializedObject.Update();

        RoomEditor roomEditor = (RoomEditor)target;

        // Ajouter un en-tête pour la génération de la grille
        GUILayout.Space(10);
        EditorGUI.BeginDisabledGroup(true); 
        EditorGUILayout.TextField("GENERATION FUNCTIONS");
        EditorGUI.EndDisabledGroup();
        GUILayout.Space(5);
        roomEditor.roomSize = EditorGUILayout.Vector2IntField("Room Size", roomEditor.roomSize);
        if (GUILayout.Button("Generate Room"))
        {
            roomEditor.GenerateEditorRoom();
        }

        if (GUILayout.Button("Clear Room"))
        {
            roomEditor.ClearEditorRoom();
        }

        
        GUILayout.Space(10);
        EditorGUILayout.LabelField("_____SAVE FUNCTIONS", EditorStyles.boldLabel);
        GUILayout.Space(5);
        roomEditor.scriptableName = EditorGUILayout.TextField("Scriptable Name", roomEditor.scriptableName);
        if (GUILayout.Button("Create Room Scriptable"))
        {
            roomEditor.CreateRoomScriptable();
        }
        
        GUILayout.Space(10);
        EditorGUILayout.LabelField("_____PROCEDURAL FUNCTIONS", EditorStyles.boldLabel);
        GUILayout.Space(5);
        // Afficher uniquement la propriété 'cellSelectionConditions' par défaut
        EditorGUILayout.PropertyField(cellSelectionConditionsProperty);
        if (GUILayout.Button("Select Cells"))
        {
            roomEditor.SelectCells(roomEditor.cellSelectionConditions);
        }
        
        // Applique les changements à l'objet
        serializedObject.ApplyModifiedProperties();
        
        debugFoldout = EditorGUILayout.Foldout(debugFoldout, "Debug Options");
        if (debugFoldout)
        {
            DrawDefaultInspector();
        }
    }
}
