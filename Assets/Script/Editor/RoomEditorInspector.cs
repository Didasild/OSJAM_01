using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

[CustomEditor(typeof(RoomEditor))]
public class RoomEditorInspector : Editor
{
    #region VARIABLES
    // Serialized Properties
    SerializedProperty cellSelectionConditionsProperty;
    SerializedProperty cellsTypeChangeProperty;
    SerializedProperty cellsStateChangeProperty;
    SerializedProperty itemRangeProperty;
    
    //Bool Sections
    private bool _debugFoldout;
    private bool _generationSection = true;
    private bool _saveSection;
    private bool _saveCurrentSection;
    private bool _proceduralSection;
    private bool _debugSection;
    private bool _randomCellFoldout;
    private bool _loadCellFoldout;
    
    //GUI variables
    private int _smallSpacing = 5;
    #endregion
    void OnEnable()
    {
        cellSelectionConditionsProperty = serializedObject.FindProperty("cellSelectionConditions");
        cellsTypeChangeProperty = serializedObject.FindProperty("cellsTypeChange");
        cellsStateChangeProperty = serializedObject.FindProperty("cellsStateChange");
        itemRangeProperty = serializedObject.FindProperty("itemRanges");
    }
    public override void OnInspectorGUI()
    {
        #region SETUP
        // Synchronise les modifications dans l'inspecteur
        serializedObject.Update();
        RoomEditor roomEditor = (RoomEditor)target;
        Undo.RegisterCompleteObjectUndo(roomEditor, "Room Editor");
        
        GUIStyle centeredStyle = new GUIStyle(GUI.skin.textField)
        {
            alignment = TextAnchor.MiddleCenter
        };
        #endregion SETUP

        #region TITLE
        //_______TITLE
        EditorGUILayout.Space(_smallSpacing);
        EditorGUILayout.LabelField("ROOM EDITOR", EditorStyles.toolbarButton);
        EditorGUILayout.Space(_smallSpacing*2);
        #endregion TITLE

        #region GENERATION
        //________SECTION - GENERATION
        //Header Foldout - GENERATION
        CoreEditorUtils.DrawSplitter(); CoreEditorUtils.DrawSplitter();
        _generationSection = CoreEditorUtils.DrawHeaderFoldout("GENERATION", _generationSection, false, null);
        if (_generationSection)
        {
            EditorGUILayout.Space(_smallSpacing);
            //Room Size
            roomEditor.roomSize = EditorGUILayout.Vector2IntField("Room Size", roomEditor.roomSize);
            
            //Room Generation Functions
            if (GUILayout.Button("Generate New Room"))
            {
                roomEditor.GenerateEditorRoom();
            }
            EditorGUILayout.Space(_smallSpacing);
            CoreEditorUtils.DrawFoldoutEndSplitter();
            EditorGUILayout.Space(_smallSpacing);
            
            EditorGUILayout.BeginHorizontal();
            roomEditor.roomSettingsToLoad = (RoomSettings)EditorGUILayout.ObjectField(roomEditor.roomSettingsToLoad, typeof(RoomSettings), true);
            if (GUILayout.Button("Load From Room Settings"))
            {
                roomEditor.LoadEditorRoom();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(_smallSpacing);
            CoreEditorUtils.DrawFoldoutEndSplitter();
            EditorGUILayout.Space(_smallSpacing);
            
            if (GUILayout.Button("Clear Room"))
            {
                roomEditor.ClearEditorRoom();
            }
            EditorGUILayout.Space(_smallSpacing);
            CoreEditorUtils.DrawFoldoutEndSplitter();
            EditorGUILayout.Space(_smallSpacing);
            
            if (roomEditor.cells.Count > 0)
            {
                GUI.enabled = false;
                EditorGUILayout.TextField("ADD CELLS", centeredStyle);
                GUI.enabled = true;
                GUI.enabled = false;
                if (GUILayout.Button("Add Top Raw"))
                {
                    roomEditor.AddCells("Top");
                }
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Add Left Col"))
                {
                    roomEditor.AddCells("Left");
                }
                GUI.enabled = true;
                if (GUILayout.Button("Add Right Col"))
                {
                    roomEditor.AddCells("Right");
                }
                EditorGUILayout.EndHorizontal();
                if (GUILayout.Button("Add Bot Raw"))
                {
                    roomEditor.AddCells("Bot");
                }
                EditorGUILayout.Space(_smallSpacing);
                CoreEditorUtils.DrawFoldoutEndSplitter();
                EditorGUILayout.Space(_smallSpacing);
            }
        }
        #endregion GENERATION
        
        #region PROCEDURAL FUNCTION
        //________SECTION - PROCEDURAL FUNCTIONS
        //Header Foldout - PROCEDURAL FUNCTIONS
        CoreEditorUtils.DrawSplitter(); CoreEditorUtils.DrawSplitter();
        _proceduralSection = CoreEditorUtils.DrawHeaderFoldout("PROCEDURAL FUNCTION", _proceduralSection, false, null);
        if (_proceduralSection)
        {
            EditorGUILayout.Space(_smallSpacing);
            CoreEditorUtils.DrawFoldoutEndSplitter();
            EditorGUILayout.Space(_smallSpacing);
            
            EditorGUILayout.PropertyField(cellSelectionConditionsProperty);
            EditorGUILayout.Space(_smallSpacing);
            
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
            
            EditorGUILayout.Space(_smallSpacing);
            
            CoreEditorUtils.DrawSplitter();
            EditorGUILayout.Space(_smallSpacing);
            _loadCellFoldout = CoreEditorUtils.DrawSubHeaderFoldout("SET LOADED CELLS", _loadCellFoldout, false, null);
            if (_loadCellFoldout)
            {
                EditorGUILayout.Space(_smallSpacing);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(cellsTypeChangeProperty);
                EditorGUILayout.PropertyField(cellsStateChangeProperty);
                EditorGUILayout.Space(_smallSpacing);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(_smallSpacing);
            
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Change Type"))
                {
                    roomEditor.ChangeCellType(roomEditor.cellsTypeChange);
                }

                if (GUILayout.Button("Change State"))
                {
                    roomEditor.ChangeCellState(roomEditor.cellsStateChange);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.Space(_smallSpacing);
            
            CoreEditorUtils.DrawSplitter();
            EditorGUILayout.Space(_smallSpacing);
            _randomCellFoldout = CoreEditorUtils.DrawSubHeaderFoldout("SET PROCEDURAL CELLS", _randomCellFoldout, false, null);
            if (_randomCellFoldout)
            {
                EditorGUILayout.Space(_smallSpacing);
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Set as Random Cell"))
                {
                    roomEditor.SetRandomCell(true);
                }
                if (GUILayout.Button("Unset as Random Cell"))
                {
                    roomEditor.SetRandomCell(false);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                roomEditor.haveStair = EditorGUILayout.Toggle("Have Stair", roomEditor.haveStair);
                roomEditor.pourcentageOfRandomMine = EditorGUILayout.IntField("Pourcentage of Mine", roomEditor.pourcentageOfRandomMine);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.PropertyField(itemRangeProperty);
            }
            
            EditorGUILayout.Space(_smallSpacing);
            CoreEditorUtils.DrawFoldoutEndSplitter();
            EditorGUILayout.Space(_smallSpacing);
        }
        #endregion PROCEDURAL FUNCTION
        
        #region SAVE AS NEW
        //________SECTION - SAVE AS NEW
        //Header Foldout - SAVE AS NEW
        CoreEditorUtils.DrawSplitter(); CoreEditorUtils.DrawSplitter();
        _saveSection = CoreEditorUtils.DrawHeaderFoldout("SAVE AS NEW ROOM", _saveSection, false, null);
        if (_saveSection)
        {
            EditorGUILayout.Space(_smallSpacing);
            EditorGUILayout.BeginHorizontal();
            roomEditor.isMandatory = EditorGUILayout.Toggle("Is Mandatory", roomEditor.isMandatory);
            roomEditor.isInFoW = EditorGUILayout.Toggle("Is In FoW", roomEditor.isInFoW);
            EditorGUILayout.EndHorizontal();
            roomEditor.roomVolumeProfile = (VolumeProfile)EditorGUILayout.ObjectField(roomEditor.roomVolumeProfile, typeof(VolumeProfile), false);
            EditorGUILayout.Space(_smallSpacing);
            CoreEditorUtils.DrawSplitter();
            EditorGUILayout.Space(_smallSpacing);
 
            GUI.enabled = false;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField("Chapter", centeredStyle);
            EditorGUILayout.TextField("Room Type", centeredStyle);
            EditorGUILayout.EndHorizontal();
            GUI.enabled = true;
            EditorGUILayout.BeginHorizontal();
            roomEditor.chapter = (Chapters)EditorGUILayout.EnumPopup(roomEditor.chapter);
            roomEditor.roomType = (RoomType)EditorGUILayout.EnumPopup(roomEditor.roomType);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            roomEditor.floorID = EditorGUILayout.IntField("Floor ID", roomEditor.floorID);
            roomEditor.roomID = EditorGUILayout.IntField("Room ID", roomEditor.roomID);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(_smallSpacing);
            CoreEditorUtils.DrawFoldoutEndSplitter();
            EditorGUILayout.Space(_smallSpacing);
            
            GUI.enabled = false;
            EditorGUILayout.TextField(roomEditor.chapter + "_F" +
                                      roomEditor.floorID.ToString("D2") + "_" + 
                                      roomEditor.generationType + "_" + 
                                      roomEditor.roomType + "_" + 
                                      roomEditor.roomID.ToString("D2"), centeredStyle);
            GUI.enabled = true;
            if (GUILayout.Button("Create New Room Scriptable"))
            {
                roomEditor.CreateRoomScriptable();
                _saveCurrentSection = true;
                _saveSection = false;
            }
            EditorGUILayout.Space(_smallSpacing);
            CoreEditorUtils.DrawSplitter();
            EditorGUILayout.Space(_smallSpacing);
        }
        #endregion SAVE

        #region SAVE AS CURRENT
        CoreEditorUtils.DrawSplitter(); CoreEditorUtils.DrawSplitter();
        _saveCurrentSection = CoreEditorUtils.DrawHeaderFoldout("SAVE ON CURRENT ROOM", _saveCurrentSection, false, null);
        if (_saveCurrentSection)
        {
            EditorGUILayout.Space(_smallSpacing);
                   
            GUI.enabled = false;
            EditorGUILayout.TextField("ROOM CLEANING", centeredStyle);
            GUI.enabled = true;
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate Hint Cells"))
            {
                roomEditor.GenerateHintCells();
            }
            if (GUILayout.Button("Clear Cells Data"))
            {
                roomEditor.ClearCellsData();
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(_smallSpacing);
            CoreEditorUtils.DrawSplitter();
            EditorGUILayout.Space(_smallSpacing);
        
            EditorGUILayout.BeginHorizontal();
            roomEditor.roomSettingsToSave = (RoomSettings)EditorGUILayout.ObjectField(roomEditor.roomSettingsToSave, typeof(RoomSettings), true);
            if (GUILayout.Button("Update Existing Room Scriptable"))
            {
                roomEditor.UpdateExistingRoomScriptable();
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(_smallSpacing);
            CoreEditorUtils.DrawSplitter();
            EditorGUILayout.Space(_smallSpacing);
        }

        #endregion SAVE AS CURRENT
        
        // Applique les changements à l'objet
        serializedObject.ApplyModifiedProperties();
        // Applique les changements
        if (GUI.changed)
        {
            EditorUtility.SetDirty(roomEditor);
        }
        
        #region DEBUG
        //________SECTION - PROCEDURAL FUNCTIONS
        //Header Foldout - PROCEDURAL FUNCTIONS
        CoreEditorUtils.DrawSplitter(); CoreEditorUtils.DrawSplitter();
        _debugSection = CoreEditorUtils.DrawHeaderFoldout("DEBUG", _debugSection, false, null);
        if (_debugSection)
        {
            EditorGUILayout.Space(_smallSpacing);
            EditorGUILayout.HelpBox("Debug Section, don't changer value here", MessageType.Warning);
            DrawDefaultInspector();
        }
        EditorGUILayout.Space(-_smallSpacing - 1f);
        #endregion DEBUG
    }
}
