using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

[CustomEditor(typeof(RoomSettings))]
public class RoomSettingsInspector : Editor
{
    #region VARIABLES
    // Serialized Properties
    SerializedProperty itemGenerationProperty;
    SerializedProperty npcDataProperty;
    
    //Bool Sections
    private bool _debugSection;
    private bool _generalSection = true;
    private bool _proceduralSection = true;
    private bool _proceduralCellsSection = true;
    
    //GUI variables
    private int _smallSpacing = 5;
    private RoomSettings _roomSettings;
    private Texture2D _customIcon;
    #endregion
    
    // permet d'exécuter du code à la selection de l'objet
    void OnEnable()
    {
        itemGenerationProperty = serializedObject.FindProperty("itemRanges");
        npcDataProperty = serializedObject.FindProperty("npcDatas");
    }
    public override void OnInspectorGUI()
    {
        #region SETUP
        // Synchronise les modifications dans l'inspecteur
        serializedObject.Update();
        _roomSettings = (RoomSettings)target; //Style centered
        
        GUIStyle centeredStyle = new GUIStyle(GUI.skin.textField)
        {
            alignment = TextAnchor.MiddleCenter
        };
        #endregion SETUP

        #region GENERAL
        //________SECTION - GENERAL
        //Header Foldout - GENERAL
        CoreEditorUtils.DrawSplitter();
        _generalSection = CoreEditorUtils.DrawHeaderFoldout("GENERAL", _generalSection);
        if (_generalSection)
        {
            EditorGUILayout.Space(_smallSpacing);
            _roomSettings.proceduralRoom = EditorGUILayout.Toggle("Fully Procedural Room", _roomSettings.proceduralRoom);
            EditorGUILayout.Space(_smallSpacing);
            
            GUI.enabled = false;
            EditorGUILayout.TextField("MAIN SETTINGS", centeredStyle);
            GUI.enabled = true;

            EditorGUILayout.BeginHorizontal();
            _roomSettings.mandatory = EditorGUILayout.Toggle("Is Mandatory", _roomSettings.mandatory);
            _roomSettings.roomType = (RoomType)EditorGUILayout.EnumPopup("Room Type", _roomSettings.roomType);
            EditorGUILayout.EndHorizontal();
            _roomSettings.isFoW = EditorGUILayout.Toggle("Is FoW", _roomSettings.isFoW);
            EditorGUILayout.Space(_smallSpacing);
            if (!_roomSettings.proceduralRoom)
            {
                GUI.enabled = false;
                EditorGUILayout.TextField("ROOM ID INFO", centeredStyle);
                GUI.enabled = true;
                _roomSettings.roomIDString = EditorGUILayout.TextField("Room ID String", _roomSettings.roomIDString);
                _roomSettings.haveProceduralCells = EditorGUILayout.Toggle("Have Procedural Cells", _roomSettings.haveProceduralCells);
            }
            
            GUI.enabled = false;
            EditorGUILayout.TextField("VISUAL SETTINGS", centeredStyle);
            GUI.enabled = true;
            _roomSettings.roomVolumeProfile = (VolumeProfile)EditorGUILayout.ObjectField("Room Color Palette", _roomSettings.roomVolumeProfile, typeof(VolumeProfile), false);
            EditorGUILayout.Space(_smallSpacing);
            
            GUI.enabled = false;
            EditorGUILayout.TextField("NPC SETTINGS", centeredStyle);
            GUI.enabled = true;
            EditorGUILayout.LabelField("NPC SETTINGS", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(npcDataProperty);
            EditorGUILayout.Space(_smallSpacing);
        }
        #endregion GENERAL

        #region PROCEDURAL
        if (_roomSettings.proceduralRoom)
        {
            //________SECTION - PROCEDURAL PARAMETERS
            //Header Foldout - PROCEDURAL PARAMETERS
            CoreEditorUtils.DrawSplitter();
            _proceduralSection = CoreEditorUtils.DrawHeaderFoldout("PROCEDURAL ROOM PARAMETERS", _proceduralSection);
            if (_proceduralSection)
            {
                EditorGUILayout.Space(_smallSpacing);
                
                GUI.enabled = false;
                EditorGUILayout.TextField("ROOM SETTINGS", centeredStyle);
                GUI.enabled = true;
                
                EditorGUILayout.LabelField("ROOM SIZE", EditorStyles.boldLabel);
                _roomSettings.minRoomSize = EditorGUILayout.Vector2IntField("Min Room Size", _roomSettings.minRoomSize);
                _roomSettings.maxRoomSize = EditorGUILayout.Vector2IntField("Max Room Size", _roomSettings.maxRoomSize);
                
                EditorGUILayout.Space(_smallSpacing);
                CoreEditorUtils.DrawFoldoutEndSplitter();
                EditorGUILayout.Space(_smallSpacing);
                
                EditorGUILayout.LabelField("SPECIFIC CELLS SETTINGS", EditorStyles.boldLabel);
                EditorGUILayout.BeginHorizontal();
                _roomSettings.roomPourcentageOfMine = EditorGUILayout.IntField("Pourcentage of Mine", _roomSettings.roomPourcentageOfMine);
                _roomSettings.roomPourcentageOfNone = EditorGUILayout.IntField("Pourcentage of None", _roomSettings.roomPourcentageOfNone);
                EditorGUILayout.EndHorizontal();
                _roomSettings.haveStair = EditorGUILayout.Toggle("Have Stair", _roomSettings.haveStair);
                
                EditorGUILayout.Space(_smallSpacing);
                CoreEditorUtils.DrawFoldoutEndSplitter();
                EditorGUILayout.Space(_smallSpacing);
                
                EditorGUILayout.LabelField("ITEMS GENERATION", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(itemGenerationProperty);
                EditorGUILayout.Space(_smallSpacing);
            }
            
            GUI.enabled = false;
            CoreEditorUtils.DrawSplitter();
            _proceduralCellsSection = CoreEditorUtils.DrawHeaderFoldout("PROCEDURAL CELLS PARAMETERS", _proceduralCellsSection);
            GUI.enabled = true;
        }
        #endregion PROCEDURAL

        #region PROCEDURAL CELLS
        if (_roomSettings.haveProceduralCells && !_roomSettings.proceduralRoom)
        {
            GUI.enabled = false;
            CoreEditorUtils.DrawSplitter();
            _proceduralSection = CoreEditorUtils.DrawHeaderFoldout("PROCEDURAL ROOM PARAMETERS", _proceduralSection);
            GUI.enabled = true;
            //________SECTION - PROCEDURAL CELLS PARAMETERS
            //Header Foldout - PROCEDURAL CELLS PARAMETERS
            CoreEditorUtils.DrawSplitter();
            _proceduralCellsSection = CoreEditorUtils.DrawHeaderFoldout("PROCEDURAL CELLS PARAMETERS", _proceduralCellsSection);
            if (_proceduralCellsSection)
            {
                EditorGUILayout.Space(_smallSpacing);
                GUI.enabled = false;
                EditorGUILayout.TextField("CELLS SETTINGS", centeredStyle);
                GUI.enabled = true;
                EditorGUILayout.Space(_smallSpacing);
                
                EditorGUILayout.BeginHorizontal();
                _roomSettings.haveStair = EditorGUILayout.Toggle("Have Stair", _roomSettings.haveStair);
                _roomSettings.roomPourcentageOfMine = EditorGUILayout.IntField("Pourcentage of Mine", _roomSettings.roomPourcentageOfMine);
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.Space(_smallSpacing);
                CoreEditorUtils.DrawFoldoutEndSplitter();
                EditorGUILayout.Space(_smallSpacing);
                
                EditorGUILayout.LabelField("ITEMS GENERATION", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(itemGenerationProperty);
                EditorGUILayout.Space(_smallSpacing);
            }
        }
        #endregion PROCEDURAL CELLS
        
        #region DEBUG
        //________SECTION - PROCEDURAL FUNCTIONS
        //Header Foldout - PROCEDURAL FUNCTIONS
        CoreEditorUtils.DrawSplitter();
        _debugSection = CoreEditorUtils.DrawHeaderFoldout("DEBUG", _debugSection);
        EditorGUILayout.Space(_smallSpacing);
        if (_debugSection)
        {
            EditorGUILayout.Space(_smallSpacing);
            EditorGUILayout.HelpBox("Debug Section, don't changer value here", MessageType.Warning);
            EditorGUILayout.Space(_smallSpacing);
            DrawDefaultInspector();
        }
        #endregion DEBUG
        
        // Applique les changements à l'objet
        serializedObject.ApplyModifiedProperties();
        // Applique les changements
        if (GUI.changed)
        {
            EditorUtility.SetDirty(_roomSettings);
        }
    }
}
