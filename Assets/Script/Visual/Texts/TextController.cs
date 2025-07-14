using NaughtyAttributes;
using UnityEngine;

public class TextController : MonoBehaviour
{
    [ReadOnly] public TextColorUpdate[] tmpColoredTexts;
    [ReadOnly] public CurvedText[] tmpCurvedTexts;
    [HideInInspector] public VisualManager visualManager;
    
    public void Init(VisualManager manager)
    {
        visualManager = manager;
        tmpColoredTexts = FindObjectsByType<TextColorUpdate>(FindObjectsSortMode.None);
        tmpCurvedTexts = FindObjectsByType<CurvedText>(FindObjectsSortMode.None);
        foreach (TextColorUpdate textColorScript in tmpColoredTexts)
        {
            textColorScript.Init(this);
        }

        foreach (CurvedText curvedTextScript in tmpCurvedTexts)
        {
            curvedTextScript.Init(this);
        }
    }

    
    public void UpdateTextColors(float transitionDuration)
    {
        foreach (TextColorUpdate textColorScript in tmpColoredTexts)
        {
            textColorScript.UpdateTextColor(transitionDuration);
        }
    }
}
