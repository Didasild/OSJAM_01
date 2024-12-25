using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

[CustomEditor(typeof(RoomSettings))]
public class RoomSettingsInspector : Editor
{
    #region VARIABLES
    //Bool Sections
    private bool debugSection;
    
    //GUI variables
    private int smallSpacing = 5;
    #endregion
    public override void OnInspectorGUI()
    {
        RoomSettings roomSettings = (RoomSettings)target;

        #region DEBUG
        //________SECTION - PROCEDURAL FUNCTIONS
        //Header Foldout - PROCEDURAL FUNCTIONS
        CoreEditorUtils.DrawSplitter();
        debugSection = CoreEditorUtils.DrawHeaderFoldout("DEBUG", debugSection, false, null);
        if (debugSection)
        {
            EditorGUILayout.Space(smallSpacing);
            DrawDefaultInspector();
        }
        #endregion DEBUG
    }
}
