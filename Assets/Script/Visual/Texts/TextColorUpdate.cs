using DG.Tweening;
using Dida.Rendering;
using TMPro;
using UnityEngine;
using NaughtyAttributes;

public class TextColorUpdate : MonoBehaviour
{
    private TMP_Text text;
    private TextController _textController;
    [MinValue(0)][MaxValue(6)] public int colorIndex;
    [MinValue(0)][MaxValue(6)] public int colorIndexVariant;

    public void Init(TextController textController)
    {
        text = GetComponent<TMP_Text>();
        _textController = textController;
        UpdateTextColor(0f);
    }
    
    public void UpdateTextColor(float transitionDuration)
    {
        if (!_textController.visualManager.roomAmbianceController.transitionColorsVolume.profile.TryGet<GlobalColorSettings>(out var colorSettings))
            return;

        Color targetColor = text.color;
        int targetColorIndex = new int();
        
        if (colorSettings.UseVariantTextIndex.value)
        {
            targetColorIndex = colorIndexVariant;
        }
        else
        {
            targetColorIndex = colorIndex;
        }
        switch (targetColorIndex)
        {
            case 1: targetColor = colorSettings.Color1.value; break;
            case 2: targetColor = colorSettings.Color2.value; break;
            case 3: targetColor = colorSettings.Color3.value; break;
            case 4: targetColor = colorSettings.Color4.value; break;
            case 5: targetColor = colorSettings.Color5.value; break;
            case 6: targetColor = colorSettings.Color6.value; break;
        }
        text.DOKill();
        text.DOColor(targetColor, transitionDuration);
    }
}
