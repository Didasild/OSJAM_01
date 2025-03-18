using System;
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
    private string _sineQuantity;
    private string _squareFill;

    public float _baseEdgeSize = 0f;
    
    public void Init(VisualManager visualManager)
    {
        _visualManager = visualManager;
        mainScreenMaterial = mainScreenSprite.GetComponent<Renderer>().material;
        
        _globalAlpha = "_GlobalAlpha";
        _radialLine = "_RadialLine";
        _edgeSize = "_EdgeSize";
        _haloFill = "_HaloFill";
        _sineQuantity = "_SineQuantity";
        _squareFill = "_SquareFill";
    }
    
    public void CellRevealFeedbackIn()
    {
        float currentFloat = mainScreenMaterial.GetFloat(_edgeSize);
        DOTween.Kill(mainScreenMaterial);
        if (currentFloat <= _baseEdgeSize + 0.05f)
        {
            mainScreenMaterial.SetFloat(_edgeSize, currentFloat + 0.005f);
        }
        FadeProperty(_edgeSize,_baseEdgeSize, 0.5f);
    }
    
    public void RoomCompletionFeedback()
    {
        ResetCentralSquare();
        FadeProperty(_sineQuantity, 30, 0.4f);
        FadeProperty(_squareFill, -0.5f, 0.4f);
        FadeProperty(_squareFill, -1f, 0.8f, 0.5f);
    }

    private void FadeProperty(string propertyID, float targetValue, float duration,  float delay = 0, Ease ease = Ease.Linear, bool resetProperty = false)
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
        mainScreenMaterial.SetFloat(propertyID, targetValue);
    }

    private void ResetAllProperties()
    {
        mainScreenMaterial.SetFloat(_globalAlpha, 1f);
        mainScreenMaterial.SetFloat(_radialLine, 0f);
        mainScreenMaterial.SetFloat(_haloFill, -1f);
        mainScreenMaterial.SetFloat(_sineQuantity, 0);
        mainScreenMaterial.SetFloat(_squareFill, 0);
    }

    private void ResetCentralSquare()
    {
        mainScreenMaterial.SetFloat(_sineQuantity, 0);
        mainScreenMaterial.SetFloat(_squareFill, 0);
    }
}
