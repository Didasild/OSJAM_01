using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "ColorPalette", menuName = "MineCrawler/ColorPalette")]
public class ColorPaletteScriptable : ScriptableObject
{
    public Color[] colors;
    public Color exterColor;
}
