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
    
    private string _glowIntensity;
    private string _rightBool;
    private string _leftBool;
    private string _upBool;
    private string _downBool;

    [ReadOnly] public float _baseEdgeSize = 0f;
    
    public void Init(VisualManager visualManager)
    {
        _visualManager = visualManager;
        _mainScreenMaterial = mainScreenSprite.GetComponent<Renderer>().material;
        
        _globalAlpha = "_GlobalAlpha";
        
        _radialLine = "_RadialLine";
        _edgeSize = "_EdgeSize";
        
        _glowIntensity = "_GlowIntensity";
        _rightBool = "_Right";
        _leftBool = "_Left";
        _upBool = "_Up";
        _downBool = "_Down";
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
    
    public void RooUnlockFeedback(bool right, bool left, bool up, bool down)
    {
        ResetCentralSquare();
        _visualManager.FadeProperty(_mainScreenMaterial, _rightBool, right ? 1 : 0, 0);
        _visualManager.FadeProperty(_mainScreenMaterial, _leftBool, left ? 1 : 0, 0);
        _visualManager.FadeProperty(_mainScreenMaterial, _upBool, up ? 1 : 0, 0);
        _visualManager.FadeProperty(_mainScreenMaterial, _downBool, down ? 1 : 0, 0);
        _visualManager.FadeProperty(_mainScreenMaterial, _glowIntensity, 1f, 0.2f);
        _visualManager.FadeProperty(_mainScreenMaterial, _glowIntensity, 0f, 1.2f, 0.4f);
    }

    private void ResetAllProperties()
    {
        _mainScreenMaterial.SetFloat(_globalAlpha, 1f);
        _mainScreenMaterial.SetFloat(_radialLine, 0f);
        _mainScreenMaterial.SetFloat(_glowIntensity, 0f);
    }

    private void ResetCentralSquare()
    {
        _mainScreenMaterial.SetFloat(_glowIntensity, 0f);
    }
}
