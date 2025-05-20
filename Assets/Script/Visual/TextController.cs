using NaughtyAttributes;
using UnityEngine;

public class TextController : MonoBehaviour
{
    [ReadOnly] public TextColorUpdate[] tmpTexts;
    [HideInInspector] public VisualManager visualManager;
    
    public void Init(VisualManager manager)
    {
        visualManager = manager;
        tmpTexts = FindObjectsByType<TextColorUpdate>(FindObjectsSortMode.None);
        foreach (TextColorUpdate textColorScript in tmpTexts)
        {
            textColorScript.Init(this);
        }
    }

    public void UpdateTextColors()
    {
        foreach (TextColorUpdate textColorScript in tmpTexts)
        {
            textColorScript.UpdateTextColor();
        }
    }
}
