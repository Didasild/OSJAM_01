using System.Collections.Generic;
using UnityEngine;

public enum Chapters
{
    Tutorial,
    Chapter1,
    Debug,
}
[CreateAssetMenu(fileName = "ChapterSettings", menuName = "DungeonGeneration/ChapterSettings")]
public class ChapterSettings : ScriptableObject
{

    #region PARAMETERS
    [Header("GENERAL")]
    public Chapters chapter;
    public List<FloorSettings> floorSettings;
    
     #endregion
}
