using NaughtyAttributes;
using UnityEngine;
using DG.Tweening;

public class CentralFeedbackController : MonoBehaviour
{
    public GameObject mainScreenSprite;
    private Material _mainScreenMaterial;
    private VisualManager _visualManager;
    private string _globalAlpha;
    private string _radialLine;
    private string _edgeSize;
    private string _haloFill;
    private string _sineQuantity;
    private string _squareFill;

    [ReadOnly] public float _baseEdgeSize = 0f;
    
    public void Init(VisualManager visualManager)
    {
        _visualManager = visualManager;
        _mainScreenMaterial = mainScreenSprite.GetComponent<Renderer>().material;
        
        _globalAlpha = "_GlobalAlpha";
        _radialLine = "_RadialLine";
        _edgeSize = "_EdgeSize";
        _haloFill = "_HaloFill";
        _sineQuantity = "_SineQuantity";
        _squareFill = "_SquareFill";
    }
    
    public void CellRevealFeedbackIn()
    {
        float currentFloat = _mainScreenMaterial.GetFloat(_edgeSize);
        DOTween.Kill(_mainScreenMaterial);
        if (currentFloat <= _baseEdgeSize + 0.05f)
        {
            _mainScreenMaterial.SetFloat(_edgeSize, currentFloat + 0.005f);
        }
        _visualManager.FadeProperty(_mainScreenMaterial, _edgeSize,_baseEdgeSize, 0.5f);
    }
    
    public void RoomCompletionFeedback()
    {
        ResetCentralSquare();
        _visualManager.FadeProperty(_mainScreenMaterial,_sineQuantity, 30, 0.4f);
        _visualManager.FadeProperty(_mainScreenMaterial, _squareFill, -0.5f, 0.4f);
        _visualManager.FadeProperty(_mainScreenMaterial, _squareFill, -1f, 0.8f, 0.5f);
    }

    private void ResetAllProperties()
    {
        _mainScreenMaterial.SetFloat(_globalAlpha, 1f);
        _mainScreenMaterial.SetFloat(_radialLine, 0f);
        _mainScreenMaterial.SetFloat(_haloFill, -1f);
        _mainScreenMaterial.SetFloat(_sineQuantity, 0);
        _mainScreenMaterial.SetFloat(_squareFill, 0);
    }

    private void ResetCentralSquare()
    {
        _mainScreenMaterial.SetFloat(_sineQuantity, 0);
        _mainScreenMaterial.SetFloat(_squareFill, 0);
    }
}
