using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public enum Chapters
{
    Tutorial,
    Chapter1,
    Debug,
}
[CreateAssetMenu(fileName = "ChapterSettings", menuName = "MineCrawler/ChapterSettings")]
public class ChapterSettings : ScriptableObject
{

    #region PARAMETERS
    [Header("GENERAL")]
    public Chapters chapter; 
    [Expandable, AllowNesting] public FloorSettings[] floorSettings;
    
     #endregion
}
