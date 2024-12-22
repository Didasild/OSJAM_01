using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RoomEditor))]
public class RoomEditorInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RoomEditor roomEditor = (RoomEditor)target;

        // Ajouter un en-tête pour la génération de la grille
        GUILayout.Space(10);
        EditorGUILayout.LabelField("_____ROOM GENERATION FONCTIONS", EditorStyles.boldLabel);
        GUILayout.Space(5);
        
        if (GUILayout.Button("Generate Room"))
        {
            roomEditor.GenerateEditorRoom();
        }

        if (GUILayout.Button("Clear Room"))
        {
            roomEditor.ClearEditorRoom();
        }
        
        GUILayout.Space(10);
        EditorGUILayout.LabelField("_____ROOM SAVE FONCTION", EditorStyles.boldLabel);
        GUILayout.Space(5);
        if (GUILayout.Button("Create Room Scriptable"))
        {
            roomEditor.CreateRoomScriptable();
        }
        
        GUILayout.Space(10);
        EditorGUILayout.LabelField("_____ROOM FUNCTIONS", EditorStyles.boldLabel);
        GUILayout.Space(5);
        if (GUILayout.Button("Select Cells"))
        {
            roomEditor.SelectCells(roomEditor.cellSelectionConditions);
        }
    }
}
