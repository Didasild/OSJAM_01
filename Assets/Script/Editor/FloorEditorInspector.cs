using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

[CustomEditor(typeof(FloorEditor))]
public class FloorEditorInspector : Editor
{
    
    //Bool Sections
    private bool _loadFloorSection = true;
    private bool _generationSection;
    
    private bool _debugFoldout;
    private bool _debugSection;
    private bool _saveSection;
    
    //GUI variables
    private int _smallSpacing = 5;
    
    public override void OnInspectorGUI()
    {
        #region SETUP
        // Synchronise les modifications dans l'inspecteur
        serializedObject.Update();
        FloorEditor floorEditor = (FloorEditor)target;
        Undo.RegisterCompleteObjectUndo(floorEditor, "Room Editor");
        
        GUIStyle centeredStyle = new GUIStyle(GUI.skin.textField)
        {
            alignment = TextAnchor.MiddleCenter
        };
        #endregion SETUP

        #region TITLE
        //_______TITLE
        EditorGUILayout.Space(_smallSpacing);
        EditorGUILayout.LabelField("FLOOR EDITOR", EditorStyles.toolbarButton);
        EditorGUILayout.Space(_smallSpacing*2);
        #endregion TITLE
        
        #region LOAD FLOOR
        //________SECTION - GENERATION
        //Header Foldout - GENERATION
        CoreEditorUtils.DrawSplitter(); CoreEditorUtils.DrawSplitter();
        _loadFloorSection = CoreEditorUtils.DrawHeaderFoldout("LOAD FLOOR", _loadFloorSection, false, null);
        if (_loadFloorSection)
        {
            EditorGUILayout.Space(_smallSpacing);
            
            EditorGUILayout.BeginHorizontal();
            floorEditor.floorToLoad = (FloorSettings)EditorGUILayout.ObjectField(floorEditor.floorToLoad, typeof(FloorSettings), true);
            if (GUILayout.Button("Load From Room Settings"))
            {
                floorEditor.LoadFloor();
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(_smallSpacing);
            CoreEditorUtils.DrawFoldoutEndSplitter();
            EditorGUILayout.Space(_smallSpacing);
            
            if (GUILayout.Button("Clear Floor"))
            {
                floorEditor.ClearFloor();
            }
            EditorGUILayout.Space(_smallSpacing);
            CoreEditorUtils.DrawSplitter();
            EditorGUILayout.Space(_smallSpacing);
        }
        #endregion LOAD FLOOR

        #region GENERATION
        CoreEditorUtils.DrawSplitter(); CoreEditorUtils.DrawSplitter();
        _generationSection = CoreEditorUtils.DrawHeaderFoldout("ROOM MANAGEMENT", _generationSection, false, null);
        if (_generationSection)
        {
            EditorGUILayout.Space(_smallSpacing);
            GUI.enabled = false;
            EditorGUILayout.TextField("ROOM TO GENERATE DATAS", centeredStyle);
            GUI.enabled = true;
            floorEditor.roomSettingsToLoad = (RoomSettings)EditorGUILayout.ObjectField(floorEditor.roomSettingsToLoad, typeof(RoomSettings), true);
            EditorGUILayout.BeginHorizontal();
            floorEditor.isStartRoom = EditorGUILayout.Toggle("Is Start Room", floorEditor.isStartRoom);
            floorEditor.roomStateToLoad = (RoomState)EditorGUILayout.EnumPopup(floorEditor.roomStateToLoad);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(_smallSpacing);
            CoreEditorUtils.DrawSplitter();
            EditorGUILayout.Space(_smallSpacing);

            GUI.enabled = false;
            EditorGUILayout.TextField("GENERATE NEW ROOM", centeredStyle);
            GUI.enabled = true;
            if (floorEditor.roomEditorObjects.Count == 0)
            {
                //Room Generation Functions
                if (GUILayout.Button("Generate First Room"))
                {
                    floorEditor.GenerateFirstRoom();
                    floorEditor.isStartRoom = false;
                }
            }
            else
            {
                if (GUILayout.Button("UP"))
                {
                    floorEditor.GenerateNeighborRoom("up");
                }
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("LEFT"))
                {
                    floorEditor.GenerateNeighborRoom("left");
                }
                if (GUILayout.Button("RIGHT"))
                {
                    floorEditor.GenerateNeighborRoom("right");
                }
                EditorGUILayout.EndHorizontal();
                if (GUILayout.Button("DOWN"))
                {
                    floorEditor.GenerateNeighborRoom("down");
                }
            }
            
            EditorGUILayout.Space(_smallSpacing);
            CoreEditorUtils.DrawSplitter();
            EditorGUILayout.Space(_smallSpacing);
            
            if (GUILayout.Button("Remove Selected Rooms"))
            {
                floorEditor.RemoveSelectedRooms();
            }
            
            EditorGUILayout.Space(_smallSpacing);
            CoreEditorUtils.DrawSplitter();
            EditorGUILayout.Space(_smallSpacing);
        }
        #endregion GENERATION

        #region SAVE
        //________SECTION - SAVE
        //Header Foldout - SAVE
        CoreEditorUtils.DrawSplitter(); CoreEditorUtils.DrawSplitter();
        _saveSection = CoreEditorUtils.DrawHeaderFoldout("SAVE", _saveSection, false, null);
        if (_saveSection)
        {
            EditorGUILayout.Space(_smallSpacing);
            EditorGUILayout.BeginHorizontal();
            GUI.enabled = false;
            EditorGUILayout.TextField("Chapter", centeredStyle);
            GUI.enabled = true;
            floorEditor.chapter = (Chapters)EditorGUILayout.EnumPopup(floorEditor.chapter);
            EditorGUILayout.EndHorizontal();
            floorEditor.floorID = EditorGUILayout.IntField("Floor ID", floorEditor.floorID);
            
            EditorGUILayout.Space(_smallSpacing);
            CoreEditorUtils.DrawFoldoutEndSplitter();
            EditorGUILayout.Space(_smallSpacing);
            
            GUI.enabled = false;
            EditorGUILayout.TextField(floorEditor.chapter + "_Floor" + "_L_" + floorEditor.floorID.ToString("D2"), centeredStyle);
            GUI.enabled = true;
            if (GUILayout.Button("Create Floor Scriptable"))
            {
                floorEditor.CreateFloorScriptable();
            }
            
            EditorGUILayout.Space(_smallSpacing);
            CoreEditorUtils.DrawSplitter();
            EditorGUILayout.Space(_smallSpacing);
            
            EditorGUILayout.BeginHorizontal();
            floorEditor.floorToSave = (FloorSettings)EditorGUILayout.ObjectField(floorEditor.floorToSave, typeof(FloorSettings), true);
            if (GUILayout.Button("Update Existing Room Scriptable"))
            {
                floorEditor.UpdateExistingFloorScriptable();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(_smallSpacing);
            CoreEditorUtils.DrawSplitter();
            EditorGUILayout.Space(_smallSpacing);
            
        }
        #endregion SAVE
        
        // Applique les changements à l'objet
        serializedObject.ApplyModifiedProperties();
        // Applique les changements
        if (GUI.changed)
        {
            EditorUtility.SetDirty(floorEditor);
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
