using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

[CustomEditor(typeof(RoomEditor))]
public class RoomEditorInspector : Editor
{
    #region VARIABLES
    // Référence à l'objet sérialisé
    SerializedProperty cellSelectionConditionsProperty;
    private bool debugFoldout;
    private bool generationSection;
    private bool saveSection;
    
    private int smallSpacing = 5;
    #endregion


    void OnEnable()
    {
        cellSelectionConditionsProperty = serializedObject.FindProperty("cellSelectionConditions");
    }
    public override void OnInspectorGUI()
    {
        #region SETUP
        // Synchronise les modifications dans l'inspecteur
        serializedObject.Update();
        RoomEditor roomEditor = (RoomEditor)target;
        #endregion SETUP

        #region TITLE
        //_______TITLE
        EditorGUILayout.LabelField("ROOM EDITOR", EditorStyles.toolbarButton);
        //Spacing
        EditorGUILayout.Space(smallSpacing*2);
        #endregion TITLE

        #region GENERATION
        //________SECTION - GENERATION
        //Header Foldout - GENERATION
        CoreEditorUtils.DrawSplitter();
        generationSection = CoreEditorUtils.DrawHeaderFoldout("GENERATION", generationSection, false, null);
        if (generationSection)
        {
            //Room Size
            roomEditor.roomSize = EditorGUILayout.Vector2IntField("Room Size", roomEditor.roomSize);
            
            //Room Generation Functions
            if (GUILayout.Button("Generate Room"))
            {
                roomEditor.GenerateEditorRoom();
            }
            if (GUILayout.Button("Clear Room"))
            {
                roomEditor.ClearEditorRoom();
            }
        }
        #endregion GENERATION

        //________SECTION - SAVE
        //Header Foldout - SAVE
        CoreEditorUtils.DrawSplitter();
        saveSection = CoreEditorUtils.DrawHeaderFoldout("SAVE", saveSection, false, null);
        if (saveSection)
        {
            roomEditor.scriptableName = EditorGUILayout.TextField("Scriptable Name", roomEditor.scriptableName);
            if (GUILayout.Button("Create Room Scriptable"))
            {
                roomEditor.CreateRoomScriptable();
            }
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
