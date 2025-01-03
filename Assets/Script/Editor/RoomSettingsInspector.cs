using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

[CustomEditor(typeof(RoomSettings))]
public class RoomSettingsInspector : Editor
{
    #region VARIABLES
    // Serialized Properties
    SerializedProperty itemGenerationProperty;
    
    //Bool Sections
    private bool debugSection;
    private bool generalSection = true;
    private bool proceduralSection = true;
    private bool proceduralCellsSection = true;
    
    //GUI variables
    private int _smallSpacing = 5;
    #endregion
    
    void OnEnable()
    {
        itemGenerationProperty = serializedObject.FindProperty("itemRanges");
    }
    public override void OnInspectorGUI()
    {
        #region SETUP
        // Synchronise les modifications dans l'inspecteur
        serializedObject.Update();
        RoomSettings roomSettings = (RoomSettings)target;
        Undo.RegisterCompleteObjectUndo(roomSettings, "Room Settings");
        
        //Style centered
        GUIStyle centeredStyle = new GUIStyle(GUI.skin.textField)
        {
            alignment = TextAnchor.MiddleCenter
        };
        #endregion SETUP


        //________SECTION - GENERAL
        //Header Foldout - GENERAL
        CoreEditorUtils.DrawSplitter();
        generalSection = CoreEditorUtils.DrawHeaderFoldout("GENERAL", generalSection, false, null);
        if (generalSection)
        {
            EditorGUILayout.Space(_smallSpacing);
            roomSettings.proceduralRoom = EditorGUILayout.Toggle("Fully Procedural Room", roomSettings.proceduralRoom);
            EditorGUILayout.Space(_smallSpacing);
            
            GUI.enabled = false;
            EditorGUILayout.TextField("MAIN SETTINGS", centeredStyle);
            GUI.enabled = true;

            EditorGUILayout.BeginHorizontal();
            roomSettings.mandatory = EditorGUILayout.Toggle("Is Mandatory", roomSettings.mandatory);
            roomSettings.roomType = (RoomType)EditorGUILayout.EnumPopup("Room Type", roomSettings.roomType);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(_smallSpacing);
            if (!roomSettings.proceduralRoom)
            {
                GUI.enabled = false;
                EditorGUILayout.TextField("ROOM ID INFO", centeredStyle);
                GUI.enabled = true;
                roomSettings.roomIDString = EditorGUILayout.TextField("Room ID String", roomSettings.roomIDString);
                roomSettings.proceduralCells = EditorGUILayout.Toggle("Have Procedural Cells", roomSettings.proceduralCells);
            }
        }
        
        if (roomSettings.proceduralRoom)
        {
            //________SECTION - PROCEDURAL PARAMETERS
            //Header Foldout - PROCEDURAL PARAMETERS
            CoreEditorUtils.DrawSplitter();
            proceduralSection = CoreEditorUtils.DrawHeaderFoldout("PROCEDURAL PARAMETERS", proceduralSection, false, null);
            if (proceduralSection)
            {
                EditorGUILayout.Space(_smallSpacing);
                
                GUI.enabled = false;
                EditorGUILayout.TextField("ROOM SETTINGS", centeredStyle);
                GUI.enabled = true;
                
                EditorGUILayout.LabelField("ROOM SIZE", EditorStyles.boldLabel);
                roomSettings.minRoomSize = EditorGUILayout.Vector2IntField("Min Room Size", roomSettings.minRoomSize);
                roomSettings.maxRoomSize = EditorGUILayout.Vector2IntField("Max Room Size", roomSettings.maxRoomSize);
                
                EditorGUILayout.Space(_smallSpacing);
                CoreEditorUtils.DrawFoldoutEndSplitter();
                EditorGUILayout.Space(_smallSpacing);
                
                EditorGUILayout.LabelField("SPECIFIC CELLS SETTINGS", EditorStyles.boldLabel);
                EditorGUILayout.BeginHorizontal();
                roomSettings.roomPourcentageOfMine = EditorGUILayout.IntField("Pourcentage of Mine", roomSettings.roomPourcentageOfMine);
                roomSettings.roomPourcentageOfNone = EditorGUILayout.IntField("Pourcentage of None", roomSettings.roomPourcentageOfNone);
                EditorGUILayout.EndHorizontal();
                roomSettings.haveStair = EditorGUILayout.Toggle("Have Stair", roomSettings.haveStair);
                
                EditorGUILayout.Space(_smallSpacing);
                CoreEditorUtils.DrawFoldoutEndSplitter();
                EditorGUILayout.Space(_smallSpacing);
                
                EditorGUILayout.LabelField("ITEMS GENERATION", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(itemGenerationProperty);
                EditorGUILayout.Space(_smallSpacing);
            }
        }

        if (roomSettings.proceduralCells && !roomSettings.proceduralRoom)
        {
            //________SECTION - PROCEDURAL CELLS PARAMETERS
            //Header Foldout - PROCEDURAL CELLS PARAMETERS
            CoreEditorUtils.DrawSplitter();
            proceduralCellsSection = CoreEditorUtils.DrawHeaderFoldout("PROCEDURAL CELLS PARAMETERS", proceduralCellsSection, false, null);
            if (proceduralCellsSection)
            {
                
            }
        }
        
        #region DEBUG
        //________SECTION - PROCEDURAL FUNCTIONS
        //Header Foldout - PROCEDURAL FUNCTIONS
        CoreEditorUtils.DrawSplitter();
        debugSection = CoreEditorUtils.DrawHeaderFoldout("DEBUG", debugSection, false, null);
        EditorGUILayout.Space(_smallSpacing);
        if (debugSection)
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
            EditorUtility.SetDirty(roomSettings);
        }
    }
}
