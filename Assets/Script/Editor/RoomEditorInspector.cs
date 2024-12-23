using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

[CustomEditor(typeof(RoomEditor))]
public class RoomEditorInspector : Editor
{
    #region VARIABLES
    // Serialized Properties
    SerializedProperty cellSelectionConditionsProperty;
    SerializedProperty cellsTypeChangeProperty;
    SerializedProperty cellsStateChangeProperty;
    
    //Bool Sections
    private bool debugFoldout;
    private bool generationSection;
    private bool saveSection;
    private bool proceduralSection;
    private bool debugSection;
    
    //GUI variables
    private int smallSpacing = 5;
    #endregion

    void OnEnable()
    {
        cellSelectionConditionsProperty = serializedObject.FindProperty("cellSelectionConditions");
        cellsTypeChangeProperty = serializedObject.FindProperty("cellsTypeChange");
        cellsStateChangeProperty = serializedObject.FindProperty("cellsStateChange");
        
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
            EditorGUILayout.Space(smallSpacing);
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
            EditorGUILayout.Space(smallSpacing);
        }
        EditorGUILayout.Space(smallSpacing);
        #endregion GENERATION
        
        #region PROCEDURAL FUNCTION
        //________SECTION - PROCEDURAL FUNCTIONS
        //Header Foldout - PROCEDURAL FUNCTIONS
        CoreEditorUtils.DrawSplitter();
        proceduralSection = CoreEditorUtils.DrawHeaderFoldout("PROCEDURAL FUNCTION", proceduralSection, false, null);
        if (proceduralSection)
        {
            EditorGUILayout.Space(smallSpacing);
            EditorGUILayout.HelpBox("This section allow to procedurally change some cells. First define the range of cells that have to be change and apply the modification you want", MessageType.Info);
            EditorGUILayout.Space(smallSpacing);
            
            CoreEditorUtils.DrawFoldoutEndSplitter();
            // Afficher uniquement la propriété 'cellSelectionConditions' par défaut
            EditorGUILayout.PropertyField(cellSelectionConditionsProperty);
            EditorGUILayout.Space(smallSpacing);
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Show Cells"))
            {
                roomEditor.SelectCells(roomEditor.cellSelectionConditions);
                foreach (CellEditor cell in roomEditor.selectedCells)
                {
                    cell.HighlightCell();
                }
            }
            if (GUILayout.Button("Clear Conditions"))
            {
                roomEditor.ClearConditions();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(smallSpacing);
            
            CoreEditorUtils.DrawFoldoutEndSplitter();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(cellsTypeChangeProperty);
            EditorGUILayout.PropertyField(cellsStateChangeProperty);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(smallSpacing);
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Random Change Type"))
            {
                roomEditor.ChangeCellType(roomEditor.cellsTypeChange);
            }

            if (GUILayout.Button("Random Change State"))
            {
                roomEditor.ChangeCellState(roomEditor.cellsStateChange);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(smallSpacing);
            
            CoreEditorUtils.DrawFoldoutEndSplitter();
            EditorGUILayout.Space(smallSpacing);
        }
        EditorGUILayout.Space(smallSpacing);
        #endregion PROCEDURAL FUNCTION
        
        #region SAVE
        //________SECTION - SAVE
        //Header Foldout - SAVE
        CoreEditorUtils.DrawSplitter();
        saveSection = CoreEditorUtils.DrawHeaderFoldout("SAVE", saveSection, false, null);
        if (saveSection)
        {
            EditorGUILayout.Space(smallSpacing);
            if (GUILayout.Button("Generate Hint Cells"))
            {
                roomEditor.GenerateHintCells();
            }
            roomEditor.scriptableName = EditorGUILayout.TextField("Scriptable Name", roomEditor.scriptableName);
            roomEditor.isMandatory = EditorGUILayout.Toggle("Is Mandatory", roomEditor.isMandatory);
            if (GUILayout.Button("Create Room Scriptable"))
            {
                roomEditor.CreateRoomScriptable();
            }
            EditorGUILayout.Space(smallSpacing);
        }
        EditorGUILayout.Space(smallSpacing);
        #endregion SAVE
        
        // Applique les changements à l'objet
        serializedObject.ApplyModifiedProperties();
        
        #region DEBUG
        //________SECTION - PROCEDURAL FUNCTIONS
        //Header Foldout - PROCEDURAL FUNCTIONS
        CoreEditorUtils.DrawSplitter();
        debugSection = CoreEditorUtils.DrawHeaderFoldout("DEBUG", debugSection, false, null);
        if (debugSection)
        {
            EditorGUILayout.Space(smallSpacing);
            EditorGUILayout.HelpBox("Debug Section, don't changer value here", MessageType.Warning);
            DrawDefaultInspector();
        }
        #endregion DEBUG
    }
}
