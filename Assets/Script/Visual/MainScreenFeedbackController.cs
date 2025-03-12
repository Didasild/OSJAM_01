using NaughtyAttributes;
using UnityEngine;
using DG.Tweening;
using UnityEditor;

public class MainScreenFeedbackController : MonoBehaviour
{
    public GameObject mainScreenSprite;
    private Material mainScreenMaterial;
    private VisualManager _visualManager;
    private string _globalAlpha;
    private string _radialLine;
    private string _edgeSize;
    private string _haloFill;
    
    
    public void Init(VisualManager visualManager)
    {
        _visualManager = visualManager;
        mainScreenMaterial = mainScreenSprite.GetComponent<Renderer>().material;
        
        _globalAlpha = "_GlobalAlpha";
        _radialLine = "_RadialLine";
        _edgeSize = "_EdgeSize";
        _haloFill = "_HaloFill";
    }

    [Button]
    public void ShowFeedback()
    {
        ResetAllProperties();
        FadeProperty(1f, 0.3f, _globalAlpha);
        FadeProperty(1f, 0.6f, _radialLine);
        FadeProperty(0f, 0.2f, _haloFill, 1f);
        FadeProperty(-1, 0.2f, _haloFill, 1.2f, Ease.InBack, false);
    }

    private void FadeProperty(float targetValue, float duration, string propertyID, float delay = 0, Ease ease = Ease.Linear, bool resetProperty = false)
    {
        Tween tween = mainScreenMaterial.DOFloat(targetValue, propertyID, duration)
            .SetDelay(delay)
            .SetEase(ease);
        if (resetProperty)
        {
            tween.OnComplete(() => ResetProperty(0, propertyID));
        }
    }

    private void ResetProperty(float targetValue, string propertyID)
    {
        mainScreenMaterial.DOFloat(targetValue, propertyID, 0f);
    }

    private void ResetAllProperties()
    {
        mainScreenMaterial.SetFloat(_globalAlpha, 0f);
        mainScreenMaterial.SetFloat(_radialLine, 0f);
        mainScreenMaterial.SetFloat(_haloFill, -1f);
    }
}
