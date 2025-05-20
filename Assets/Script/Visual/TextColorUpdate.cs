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

    public void Init(TextController textController)
    {
        text = GetComponent<TMP_Text>();
        _textController = textController;
        UpdateTextColor();
    }
    
    public void UpdateTextColor()
    {
        if (!_textController.visualManager.mainColorsVolume.profile.TryGet<GlobalColorSettings>(out var colorSettings))
            return;
        text.color = colorIndex switch
        {
            1 => colorSettings.Color1.value,
            2 => colorSettings.Color2.value,
            3 => colorSettings.Color3.value,
            4 => colorSettings.Color4.value,
            5 => colorSettings.Color5.value,
            6 => colorSettings.Color6.value,
            _ => text.color
        };
    }
}
