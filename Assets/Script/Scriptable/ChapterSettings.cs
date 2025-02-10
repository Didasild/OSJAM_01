using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Rendering;

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
    [Expandable] public FloorSettings[] floorSettings;
    public ColorPaletteScriptable chapterDefaultPalette;
    public VolumeProfile chapterDefaultColorsVolume;

    #endregion
}
