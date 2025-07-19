using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

[CustomEditor(typeof(FloorEditor))]
public class FloorEditorInspector : Editor
{
    
    //Bool Sections
    private bool _loadFloorSection = true;
    private bool _generationSection;
    private bool _modificationSection;
    
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
            CoreEditorUtils.DrawFoldoutEndSplitter();
            EditorGUILayout.Space(_smallSpacing);
            
            GUI.enabled = false;
            EditorGUILayout.TextField("EDITOR SWITCH", centeredStyle);
            if (floorEditor.selectedRoomEditorObjects.Count > 0 && floorEditor.selectedRoomEditorObjects[0].roomSettings != null)
            {
                EditorGUILayout.TextField(floorEditor.selectedRoomEditorObjects[0].roomSettings.ToString(), centeredStyle);
            }
            GUI.enabled = true;
            if (floorEditor.gameObject.activeInHierarchy && !floorEditor.roomEditor.gameObject.activeInHierarchy)
            {
                if (GUILayout.Button("Open in Room Editor"))
                {
                    floorEditor.SwitchToRoomEditor(floorEditor.selectedRoomEditorObjects[0].roomSettings);
                }
            }
            else
            {
                if (GUILayout.Button("Open Floor Editor"))
                {
                    floorEditor.SwitchToFloorEditor();
                }
            }
            
            EditorGUILayout.Space(_smallSpacing);
            CoreEditorUtils.DrawSplitter();
            EditorGUILayout.Space(_smallSpacing);
        }
        #endregion LOAD FLOOR

        #region GENERATION
        CoreEditorUtils.DrawSplitter(); CoreEditorUtils.DrawSplitter();
        _generationSection = CoreEditorUtils.DrawHeaderFoldout("GENERATION", _generationSection, false, null);
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
            CoreEditorUtils.DrawFoldoutEndSplitter();
            EditorGUILayout.Space(_smallSpacing);
            
            GUI.enabled = false;
            EditorGUILayout.TextField("REMOVE", centeredStyle);
            GUI.enabled = true;
            if (GUILayout.Button("Remove Selected Rooms"))
            {
                floorEditor.RemoveSelectedRooms();
            }

            EditorGUILayout.Space(_smallSpacing);
            CoreEditorUtils.DrawSplitter();
            EditorGUILayout.Space(_smallSpacing);
        }
        #endregion GENERATION

        #region MODIFICATION
        CoreEditorUtils.DrawSplitter(); CoreEditorUtils.DrawSplitter();
        _modificationSection = CoreEditorUtils.DrawHeaderFoldout("MODIFICATIONS", _modificationSection, false, null);
        if (_modificationSection)
        {
            EditorGUILayout.Space(_smallSpacing);
            CoreEditorUtils.DrawFoldoutEndSplitter();
            EditorGUILayout.Space(_smallSpacing);
            
            GUI.enabled = false;
            EditorGUILayout.TextField("MOVE SELECTED ROOM", centeredStyle);
            GUI.enabled = true;
            EditorGUILayout.HelpBox("You can use arrow key while having the inspector selected to move rooms", MessageType.Info);
            if (GUILayout.Button("MOVE UP"))
            {
                floorEditor.MoveSelectedRooms(Vector2Int.up);
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("MOVE LEFT"))
            {
                floorEditor.MoveSelectedRooms(Vector2Int.left);   
            }

            if (GUILayout.Button("MOVE RIGHT"))
            {
                floorEditor.MoveSelectedRooms(Vector2Int.right);
            }

            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("MOVE DOWN"))
            {
                floorEditor.MoveSelectedRooms(Vector2Int.down);
            }
            
            EditorGUILayout.Space(_smallSpacing);
            CoreEditorUtils.DrawFoldoutEndSplitter();
            EditorGUILayout.Space(_smallSpacing);
            
            GUI.enabled = false;
            EditorGUILayout.TextField("ASSIGN NEW ROOM SETTINGS", centeredStyle);
            GUI.enabled = true;
            floorEditor.roomSettingsToAssign = (RoomSettings)EditorGUILayout.ObjectField(floorEditor.roomSettingsToAssign, typeof(RoomSettings), true);
            if (GUILayout.Button("ASSIGN NEW ROOM SETTINGS"))
            {
                floorEditor.AssignNewRoomSettings(floorEditor.roomSettingsToAssign);
            }
            
            EditorGUILayout.Space(_smallSpacing);
            CoreEditorUtils.DrawSplitter();
            EditorGUILayout.Space(_smallSpacing);
        }

        #endregion MODIFICATION
        
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
            EditorGUILayout.TextField("CHAPTER", centeredStyle);
            EditorGUILayout.TextField("FLOOR TYPE", centeredStyle);
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            floorEditor.chapter = (Chapters)EditorGUILayout.EnumPopup(floorEditor.chapter);
            floorEditor.floorType = (FloorType)EditorGUILayout.EnumPopup(floorEditor.floorType);
            EditorGUILayout.EndHorizontal();
            GUI.enabled = false;
            EditorGUILayout.TextField("VOLUME PROFILE", centeredStyle);
            GUI.enabled = true;
            floorEditor.floorVolumeProfile = (VolumeProfile)EditorGUILayout.ObjectField(floorEditor.floorVolumeProfile, typeof(VolumeProfile), false);
            floorEditor.floorID = EditorGUILayout.IntField("Floor ID", floorEditor.floorID);
            
            EditorGUILayout.Space(_smallSpacing);
            CoreEditorUtils.DrawFoldoutEndSplitter();
            EditorGUILayout.Space(_smallSpacing);
            
            GUI.enabled = false;
            EditorGUILayout.TextField(floorEditor.chapter + "_Floor" + "_L_" + floorEditor.floorType + "_" + floorEditor.floorID.ToString("D2"), centeredStyle);
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

        #region METHODS
        //MOVE WITH KEYBOARD
        Event e = Event.current;

        if (e.type == EventType.KeyDown)
        {
            switch (e.keyCode)
            {
                case KeyCode.UpArrow:
                    floorEditor.MoveSelectedRooms(Vector2Int.up);
                    e.Use();
                    break;

                case KeyCode.DownArrow:
                    floorEditor.MoveSelectedRooms(Vector2Int.down);
                    e.Use();
                    break;

                case KeyCode.LeftArrow:
                    floorEditor.MoveSelectedRooms(Vector2Int.left);
                    e.Use();
                    break;

                case KeyCode.RightArrow:
                    floorEditor.MoveSelectedRooms(Vector2Int.right);
                    e.Use();
                    break;
            }
        }
        #endregion METHODS
        
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
