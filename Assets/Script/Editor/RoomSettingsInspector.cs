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
    private bool _debugSection;
    private bool _generalSection = true;
    private bool _proceduralSection = true;
    private bool _proceduralCellsSection = true;
    
    //GUI variables
    private int _smallSpacing = 5;
    private Texture2D _customIcon;
    #endregion
    
    void OnEnable()
    {
        itemGenerationProperty = serializedObject.FindProperty("itemRanges");
        // Charger une icône personnalisée à partir des Assets
        _customIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Resources/2D/InspectorIcons/RoomSettingsIcon.png");
        ApplyCustomIcon();
    }
    public override void OnInspectorGUI()
    {
        #region SETUP
        // Synchronise les modifications dans l'inspecteur
        serializedObject.Update();
        RoomSettings roomSettings = (RoomSettings)target; //Style centered
        
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
            roomSettings.proceduralRoom = EditorGUILayout.Toggle("Fully Procedural Room", roomSettings.proceduralRoom);
            EditorGUILayout.Space(_smallSpacing);
            
            GUI.enabled = false;
            EditorGUILayout.TextField("MAIN SETTINGS", centeredStyle);
            GUI.enabled = true;

            EditorGUILayout.BeginHorizontal();
            roomSettings.mandatory = EditorGUILayout.Toggle("Is Mandatory", roomSettings.mandatory);
            roomSettings.roomType = (RoomType)EditorGUILayout.EnumPopup("Room Type", roomSettings.roomType);
            EditorGUILayout.EndHorizontal();
            roomSettings.isFoW = EditorGUILayout.Toggle("Is FoW", roomSettings.isFoW);
            EditorGUILayout.Space(_smallSpacing);
            if (!roomSettings.proceduralRoom)
            {
                GUI.enabled = false;
                EditorGUILayout.TextField("ROOM ID INFO", centeredStyle);
                GUI.enabled = true;
                roomSettings.roomIDString = EditorGUILayout.TextField("Room ID String", roomSettings.roomIDString);
                roomSettings.haveProceduralCells = EditorGUILayout.Toggle("Have Procedural Cells", roomSettings.haveProceduralCells);
            }
        }
        #endregion GENERAL

        #region PROCEDURAL
        if (roomSettings.proceduralRoom)
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
            
            GUI.enabled = false;
            CoreEditorUtils.DrawSplitter();
            _proceduralCellsSection = CoreEditorUtils.DrawHeaderFoldout("PROCEDURAL CELLS PARAMETERS", _proceduralCellsSection);
            GUI.enabled = true;
        }
        #endregion PROCEDURAL

        #region PROCEDURAL CELLS
        if (roomSettings.haveProceduralCells && !roomSettings.proceduralRoom)
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
                roomSettings.haveStair = EditorGUILayout.Toggle("Have Stair", roomSettings.haveStair);
                roomSettings.roomPourcentageOfMine = EditorGUILayout.IntField("Pourcentage of Mine", roomSettings.roomPourcentageOfMine);
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
            EditorUtility.SetDirty(roomSettings);
        }
    }

    private void ApplyCustomIcon()
    {
        if (_customIcon == null)
        {
            Debug.LogWarning("L'icône personnalisée n'a pas été trouvée. Assurez-vous qu'elle est placée dans 'Assets/Icons/RoomSettingIcon.png'.");
            return;
        }

        // Appliquer l'icône à l'objet sélectionné
        RoomSettings roomSetting = (RoomSettings)target;
        EditorGUIUtility.SetIconForObject(roomSetting, _customIcon);

        // Marquer la scène ou le projet comme modifié
        EditorUtility.SetDirty(roomSetting);

        Debug.Log("Icône personnalisée appliquée avec succès !");
    }
}
