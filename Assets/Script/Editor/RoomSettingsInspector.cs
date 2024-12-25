using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

[CustomEditor(typeof(RoomSettings))]
public class RoomSettingsInspector : Editor
{
    #region VARIABLES
    //Bool Sections
    private bool debugSection;
    private bool generalSection;
    private bool proceduralSection;
    
    //GUI variables
    private int smallSpacing = 5;
    #endregion
    public override void OnInspectorGUI()
    {
        #region SETUP
        // Synchronise les modifications dans l'inspecteur
        serializedObject.Update();
        RoomSettings roomSettings = (RoomSettings)target;
        Undo.RegisterCompleteObjectUndo(roomSettings, "Room Settings");
        #endregion SETUP


        //________SECTION - GENERAL
        //Header Foldout - GENERAL
        CoreEditorUtils.DrawSplitter();
        generalSection = CoreEditorUtils.DrawHeaderFoldout("GENERAL", generalSection, false, null);
        EditorGUILayout.Space(smallSpacing);
        if (generalSection)
        {
            roomSettings.proceduralRoom = EditorGUILayout.Toggle("Procedural Room", roomSettings.proceduralRoom);
        }


        if (roomSettings.proceduralRoom)
        {
            //________SECTION - PROCEDURAL PARAMETERS
            //Header Foldout - PROCEDURAL PARAMETERS
            EditorGUILayout.Space(smallSpacing);
            CoreEditorUtils.DrawSplitter();
            proceduralSection = CoreEditorUtils.DrawHeaderFoldout("PROCEDURAL PARAMETERS", proceduralSection, false, null);
            EditorGUILayout.Space(smallSpacing);
            if (proceduralSection)
            {
            
            }
        }
        else
        {
            roomSettings.roomLoadString = EditorGUILayout.TextField("Room Saved String", roomSettings.roomLoadString);
        }
        EditorGUILayout.Space(smallSpacing);
        
        #region DEBUG
        //________SECTION - PROCEDURAL FUNCTIONS
        //Header Foldout - PROCEDURAL FUNCTIONS
        CoreEditorUtils.DrawSplitter();
        debugSection = CoreEditorUtils.DrawHeaderFoldout("DEBUG", debugSection, false, null);
        EditorGUILayout.Space(smallSpacing);
        if (debugSection)
        {
            EditorGUILayout.Space(smallSpacing);
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
